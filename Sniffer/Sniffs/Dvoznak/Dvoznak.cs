using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Sniffer.Common;
using Gecko;
using System.ComponentModel;
using System.Net.Mail;
using System.Net.Mime;
using Sniffer.Settings;
using Polenter.Serialization;
using System.Collections.Concurrent;
using System.Globalization;
using System.Web;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Transactions;
using System.Data.SqlClient;
using System.Linq;

namespace Sniffer.Sniffs.Dvoznak
{
	public class Dvoznak : Sniff
	{
		private const string url = "http://dvoznak.com/?show=prognoze";
		#region Login
		private const string LoginEmail = "";
		private const string LoginPass = "";
		#endregion

		private static bool firstTimeUsersStateCleaned = false;
		private static UserStateDictionary userStates = new UserStateDictionary();
		private static ConcurrentDictionary<string, DataSet> cachedBadOrGoodData = new ConcurrentDictionary<string, DataSet>();
		private volatile static bool serializeUserState = false;

		private SniffSettings _settings;
		private Dictionary<Tip.Outcome, string> iconUrls = new Dictionary<Tip.Outcome, string>();
		private System.Windows.Forms.Timer tmrUserStateCleanup = new System.Windows.Forms.Timer();
		private System.Windows.Forms.Timer tmrUserStateSerialization = new System.Windows.Forms.Timer();
		private List<Tip> tipsBuffer = new List<Tip>();

		public SniffSettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
				if (!firstTimeUsersStateCleaned)
				{
					firstTimeUsersStateCleaned = true;
					cleanUsersStates();
				}
			}
		}

		static Dvoznak()
		{
			SharpSerializer serializer = new SharpSerializer();
			try
			{
				userStates = (UserStateDictionary)serializer.Deserialize("Sniffs/Dvoznak/UsersStates.xml");
			}
			catch { }

			userStates.OnDestruction += new EventHandler(usersStates_OnDestruction);
		}

		static void usersStates_OnDestruction(object sender, EventArgs e)
		{
			serializeUsersStates();
		}

		private static void serializeUsersStates()
		{
			SharpSerializer serializer = new SharpSerializer();
			serializer.Serialize(userStates, "Sniffs/Dvoznak/UsersStates.xml");
		}

		public Dvoznak(HoveWebBrowser webBrowser)
			: base(webBrowser)
		{
			NavigationTimeoutInterval = 30000;

			tmrUserStateCleanup.Tick += new EventHandler(tmrUserStateCleanup_Tick);
			tmrUserStateCleanup.Interval = 3600000;
			tmrUserStateCleanup.Enabled = true;

			tmrUserStateSerialization.Tick += new EventHandler(tmrUserStateSerialization_Tick);
			tmrUserStateSerialization.Interval = 1000;
			tmrUserStateSerialization.Enabled = true;

			this.SniffingCompleted += Dvoznak_SniffingCompleted;

			iconUrls.Add(Tip.Outcome.Unfinished, "http://dvoznak.com/img/icons/question.png");
			iconUrls.Add(Tip.Outcome.Loss, "http://dvoznak.com/img/icons/cross.png");
			iconUrls.Add(Tip.Outcome.Won, "http://dvoznak.com/img/icons/check.png");
			iconUrls.Add(Tip.Outcome.Won2, "http://dvoznak.com/img/icons/check.png");
		}

		void Dvoznak_SniffingCompleted(object sender, EventArgs e)
		{
			tipsBuffer.Clear();
		}

		private void tmrUserStateSerialization_Tick(object sender, EventArgs e)
		{
			if (serializeUserState)
			{
				serializeUserState = false;
				serializeUsersStates();
			}
		}

		void tmrUserStateCleanup_Tick(object sender, EventArgs e)
		{
			cleanUsersStates();
			cachedBadOrGoodData.Clear();
		}

		#region Sniffs
		private void processCoordinator()
		{
			try
			{
				Sniffs.Clear();

				DateTime now = DateTime.Now;
				DateTime nextCheck = new DateTime(userStates.PreviousTipsLastChecked.Year,
													userStates.PreviousTipsLastChecked.Month,
													userStates.PreviousTipsLastChecked.Day,
													userStates.PreviousTipsLastChecked.Hour, 0, 0).AddHours(1);
				bool goingToCheckTipCompletion = false;
				if (nextCheck <= now)
				{
					DateTime today = new DateTime(now.Year, now.Month, now.Day);

					//clean old finished tips
					foreach (DateTime day in userStates.OldFinishedTips.Keys.Where(d => d < today))
					{
						List<Tip> dummy;
						userStates.OldFinishedTips.TryRemove(day, out dummy);
					}

					List<DateTime> daysToCheck = new List<DateTime>(userStates.DaysToCheck);
					daysToCheck.Remove(today);

					goingToCheckTipCompletion = daysToCheck.Count > 0;
					string url = @"http://dvoznak.com/?show=arhiva_prognoza&arhiva_dan=";
					bool loginProcessSet = false;
					for (int i = 0; i < daysToCheck.Count; i++)
					{
						DateTime day = daysToCheck[i];
						Sniffs.Add(string.Concat("processCheckTipCompletionNavigate", i.ToString()), processNavigate, string.Concat(url, day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
						if (!loginProcessSet)
						{
							Sniffs.Add("processLogin", processLogin);
							loginProcessSet = true;
						}
						Sniffs.Add(string.Concat("processCheckTipCompletion", i.ToString()), processCheckTipCompletion, day);
					}
					Sniffs.Add("processCheckTipCompletionEnd", processCheckTipCompletionEnd);
				}

				Sniffs.Add("processNavigate", processNavigate, Dvoznak.url);
				if (!goingToCheckTipCompletion)
				{
					Sniffs.Add("processLogin", processLogin);
				}
				Sniffs.Add("processTips", processTips);

				base.Start();
			}
			catch (Exception ex)
			{
				Log(ex);
				throw ex;
			}
		}

		private bool processNavigate(object state = null)
		{
			try
			{
				WebBrowser.Navigate(state.ToString());
				return true;
			}
			catch (Exception ex)
			{
				Log(ex);
				throw ex;
			}
		}

		private bool processCheckTipCompletion(object state = null)
		{
			try
			{
				tipsBuffer.AddRange(getTips());
				if (goToNextPage())
				{
					return false;
				}

				DateTime forDay = (DateTime)state;
				DateTime nextDay = forDay.AddDays(1);
				Dictionary<Tip, Tip> newTips = getDatabaseVersionTips(tipsBuffer);
				List<Tip> prevTips = (from t in TipElderDb.I.Tip
									  where t.Odigravanje >= forDay && t.Odigravanje < nextDay
									  select t).ToList();
				prevTips.ForEach(t =>
				{
					if (t.Zarada < 0)
					{
						t.Uspjesnost = Tip.Outcome.Loss;
					}
					else if (t.Zarada > 0)
					{
						t.Uspjesnost = Tip.Outcome.Won;
					}
					else
					{
						t.Uspjesnost = Tip.Outcome.Won2;
					}
				});

				DateTime now = DateTime.Now;
				DateTime today = new DateTime(now.Year, now.Month, now.Day);
				if (!userStates.OldFinishedTips.ContainsKey(today))
				{
					userStates.OldFinishedTips[today] = new List<Tip>();
				}
				List<UserTip> newTipsWithChangesForUpdate = new List<UserTip>();
				List<UserTip> tipsWithChangedZarada = new List<UserTip>();
				bool hasUnfinishedTips = false;
				getTipsWithChanges(newTips.Keys.ToList(), prevTips).ForEach(t =>
				{
					if (t.Tip.Uspjesnost == Tip.Outcome.Unfinished)
					{
						hasUnfinishedTips = true;
					}
					else
					{
						newTipsWithChangesForUpdate.Add(t);
						if (t.SimilarTip == null)
							userStates.OldFinishedTips[today].Add(newTips[t.Tip]);
					}

					if (t.SimilarTip != null && t.DifferesOnFields.Contains(Tip.Field.Zarada) && t.SimilarTip.Zarada.HasValue && t.Tip.Zarada.HasValue)
					{
						tipsWithChangedZarada.Add(t);
					}
				});

				//logiranje prognoza kojima se promijenila zarada
				tipsWithChangedZarada.ForEach(t =>
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(string.Concat("Promjena zarade za prognozu (", t.SimilarTip.Id, "): ", t.SimilarTip.Dogadjaj));
					sb.AppendLine(string.Concat("\t", "Tipster: ", t.SimilarTip.Tipster));
					sb.Append(string.Concat("\t", "Sa/na: ", t.SimilarTip.Zarada, "/", t.Tip.Zarada));
					Log(sb.ToString());
				});

				List<Tip> insertedTips;
				List<Tip> updatedTips;
				saveFinishedTipsToDatabase(newTipsWithChangesForUpdate, out insertedTips, out updatedTips);
				insertedTips.ForEach(t => userStates.PreviousTips.Add(t));

				TipElderDb context = TipElderDb.I;
				List<Tip> tipsForDelete = new List<Tip>();
				getTipsWithChanges(prevTips, newTips.Keys.ToList()).ForEach(t =>
				{
					if (t.SimilarTip == null)
					{
						context.Tip.Add(t.Tip);
						context.Entry(t.Tip).State = EntityState.Deleted;
						tipsForDelete.Add(t.Tip);
					}
				});
				if (tipsForDelete.Count > 0)
				{
					using (var scope = new TransactionScope(TransactionScopeOption.Required,
									new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
					{
						context.SaveChanges();
						scope.Complete();
					}

					tipsForDelete.ForEach(dt =>
					{
						foreach (Tip pt in userStates.PreviousTips)
						{
							if (pt.Id == dt.Id)
							{
								pt.Id = -1;
							}
						}
					});
				}

				if (!hasUnfinishedTips)
				{
					List<DateTime> days = userStates.DaysToCheck.Where(d => d != forDay).ToList();
					userStates.DaysToCheck = new BlockingCollection<DateTime>();
					days.ForEach(d => userStates.DaysToCheck.Add(d));
				}

				tipsBuffer.Clear();
				return true;
			}
			catch (Exception ex)
			{
				Log(ex);
				throw ex;
			}
		}

		private bool processCheckTipCompletionEnd(object state = null)
		{
			userStates.PreviousTipsLastChecked = DateTime.Now;
			serializeUserState = true;
			return true;
		}

		private bool processLogin(object state = null)
		{
			try
			{
				GeckoElement loginElement = WebBrowser.Document.GetElementById("lf_wrap");
				GeckoElement userBar = WebBrowser.Document.GetElementById("userbar_s");

				DateTime end = DateTime.Now.AddSeconds(30);
				while ((loginElement.GetAttribute("style") ?? "").Contains("none") && (userBar.GetAttribute("style") ?? "").Contains("none")
					&& DateTime.Now < end)
				{
					Application.DoEvents();
				}

				//ulogiravanje
				if (loginElement != null && (!(loginElement.GetAttribute("style") ?? "").Contains("none")
					|| (userBar.GetAttribute("style") ?? "").Contains("none")))
				{
					GeckoElement emailElement = WebBrowser.Document.GetElementById("paoamcpwoi");
					if (emailElement != null)
					{
						emailElement.SetAttribute("value", LoginEmail);
					}

					GeckoElement passElement = WebBrowser.Document.GetElementById("kuevgiue");
					if (passElement != null)
					{
						passElement.SetAttribute("value", LoginPass);
					}

					WebBrowser.Navigate("javascript: document.forms['prijava'].submit();");
					return false;
				}

				return true;
			}
			catch (Exception e)
			{
				Log(e);
				throw e;
			}
		}

		private bool processTips(object state = null)
		{
			try
			{
				tipsBuffer.AddRange(getTips());
				if (goToNextPage())
				{
					return false;
				}

				saveFinishedTipsToDatabase(tipsBuffer);

				List<Tip> oldFinishedTips = getOldFinishedTips();
				bool changed;
				List<Tip> mergedOldAndNewTips = mergeTips(getTipsWithChanges(tipsBuffer, oldFinishedTips), oldFinishedTips, out changed);
				tipsBuffer.Clear();
				tipsBuffer.AddRange(mergedOldAndNewTips);

				foreach (DvoznakUserSettings userSettings in Settings.UserSettings)
				{
					if (!userSettings.IsActive)
					{
						continue;
					}

					updateUserState(userSettings);
					UserState userState = userStates[userSettings.Key];
					List<UserTip> tipsForUser = getTipsForUser(userSettings, tipsBuffer, new List<Tip>(userState.PreviousTips));
					List<Tip> prevTips = new List<Tip>(userState.PreviousTips);
					foreach (UserTip tip in tipsForUser)
					{
						userState.TipsToSend.Add(tip);
						prevTips.Add(tip.Tip);
						if (tip.SimilarTip != null)
						{
							prevTips.Remove(tip.SimilarTip);
						}
					}
					userState.PreviousTips = new BlockingCollection<Tip>();
					prevTips.ForEach(t => userState.PreviousTips.Add(t));
				}

				foreach (DvoznakUserSettings userSettings in Settings.UserSettings)
				{
					if (userStates.ContainsKey(userSettings.Key) && userStates[userSettings.Key].TipsToSend.Count > 0)
					{
						generateAndSendMail(userSettings);
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Log(e);
				throw e;
			}
		}
		#endregion

		private List<Tip> getOldFinishedTips()
		{
			DateTime now = DateTime.Now;
			DateTime today = new DateTime(now.Year, now.Month, now.Day);
			if (userStates.OldFinishedTips.ContainsKey(today))
			{
				return userStates.OldFinishedTips[today];
			}
			return new List<Tip>();
		}

		private Dictionary<Tip, Tip> getDatabaseVersionTips(List<Tip> tips)
		{
			Dictionary<Tip, Tip> adaptedTips = new Dictionary<Tip, Tip>();
			tips.ForEach(t =>
			{
				adaptedTips.Add(
					new Tip
					{
						Id = t.Id,
						Liga = t.Liga,
						Dogadjaj = t.Dogadjaj,
						TipOdigravanja = t.TipOdigravanja,
						Odigravanje = t.Odigravanje,
						Tipster = t.Tipster,
						Koeficijent = t.Koeficijent,
						Ulog = t.Ulog,
						MinKoef = t.MinKoef,
						Objavljeno = t.Objavljeno,
						Kladionica = t.Kladionica,
						Rezultat = t.Rezultat,
						Sport = t.Sport,
						Zarada = t.Zarada,
						Premium = t.Premium,
						IsMargina = t.IsMargina,
						Uspjesnost = t.Uspjesnost
					},
					t);
			});
			return adaptedTips;
		}

		private void saveFinishedTipsToDatabase(List<Tip> newTips)
		{
			List<Tip> prevTips = new List<Tip>(userStates.PreviousTips);
			List<Tip> newUnfinishedTips = new List<Tip>();
			newTips.ForEach(t =>
			{
				if (t.Uspjesnost != Tip.Outcome.Unfinished)
				{
					newUnfinishedTips.Add(t);
				}
			});

			Dictionary<Tip, Tip> newDatabaseVersionTips = getDatabaseVersionTips(newUnfinishedTips);
			List<UserTip> tipsWithChanges = getTipsWithChanges(newDatabaseVersionTips.Keys.ToList(), prevTips);
			List<Tip> insertedTips;
			List<Tip> updatedTips;
			saveFinishedTipsToDatabase(tipsWithChanges, out insertedTips, out updatedTips);
			if (insertedTips.Count > 0)
			{
				cachedBadOrGoodData.Clear();
			}
			insertedTips.ForEach(t => newDatabaseVersionTips[t].Id = t.Id);

			bool changed;
			prevTips = mergeTips(tipsWithChanges, prevTips, out changed);
			userStates.PreviousTips = new BlockingCollection<Tip>();
			prevTips.ForEach(t => userStates.PreviousTips.Add(t));
			if (changed)
			{
				historyDaysCheckUpdate(tipsWithChanges);
				serializeUserState = true;
			}
		}

		private void historyDaysCheckUpdate(List<UserTip> tipsWithChanges)
		{
			foreach (UserTip ut in tipsWithChanges)
			{
				DateTime day = new DateTime(ut.Tip.Odigravanje.Year, ut.Tip.Odigravanje.Month, ut.Tip.Odigravanje.Day);
				if (!userStates.DaysToCheck.Contains(day))
				{
					userStates.DaysToCheck.Add(day);
				}
			}
		}

		private List<Tip> mergeTips(List<UserTip> tipsWithChanges, List<Tip> previousTips, out bool changed)
		{
			changed = false;
			List<Tip> mergedTips = new List<Tip>(previousTips);

			foreach (UserTip twc in tipsWithChanges)
			{
				if (twc.SimilarTip == null)
				{
					mergedTips.Add(twc.Tip);
					changed = true;
				}
				else
				{
					mergedTips.Remove(twc.SimilarTip);
					mergedTips.Add(twc.Tip);
					changed = true;
				}
			}

			return mergedTips;
		}

		private int saveFinishedTipsToDatabase(List<UserTip> tipsWithChanges, out List<Tip> insertedTips, out List<Tip> updatedTips)
		{
			insertedTips = new List<Tip>();
			updatedTips = new List<Tip>();
			TipElderDb context = TipElderDb.I;
			foreach (UserTip twc in tipsWithChanges)
			{
				if (twc.SimilarTip == null)
				{
					if (twc.Tip.Uspjesnost != Tip.Outcome.Unfinished && twc.Tip.Id == 0)
					{
						context.Tip.Add(twc.Tip);
						insertedTips.Add(twc.Tip);
					}
				}
				//-1 znači da je obrisan
				else if (twc.Tip.Uspjesnost != Tip.Outcome.Unfinished && twc.SimilarTip.Id != -1)
				{
					twc.Tip.Id = twc.SimilarTip.Id;
					context.Tip.Add(twc.Tip);
					if (twc.Tip.Id > 0)
					{
						context.Entry<Tip>(twc.Tip).State = EntityState.Modified;
						updatedTips.Add(twc.Tip);
					}
					else
					{
						insertedTips.Add(twc.Tip);
					}
				}
			}

			int rez = 0;
			if (context.Tip.Local.Count > 0)
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required,
								new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
				{
					rez = context.SaveChanges();
					scope.Complete();
				}
			}

			return rez;
		}

		private bool goToNextPage()
		{
			NameValueCollection qs = HttpUtility.ParseQueryString(WebBrowser.Url.Query);
			int pageNumber;
			int.TryParse((qs["page"] ?? "1").Trim(), out pageNumber);

			foreach (GeckoElement ol in WebBrowser.Document.GetElementsByTagName("ol"))
			{
				if (ol.GetAttribute("class").Contains("numbering"))
				{
					foreach (GeckoElement a in ol.GetElementsByTagName("a"))
					{
						Uri uri = new Uri(String.Concat(WebBrowser.Url.Scheme, "://", WebBrowser.Url.Host, "/", a.GetAttribute("href")));
						NameValueCollection qsTmp = HttpUtility.ParseQueryString(uri.Query);
						int newPageNumber;
						int.TryParse((qsTmp["page"] ?? "1").Trim(), out newPageNumber);
						if (newPageNumber > pageNumber)
						{
							qs["page"] = (pageNumber + 1).ToString();
							string urlNextPage = String.Concat(WebBrowser.Url.Scheme, "://", WebBrowser.Url.Host, WebBrowser.Url.AbsolutePath, "?", Common.Common.ConstructQueryString(qs));
							WebBrowser.Navigate(urlNextPage);
							return true;
						}
					}
				}
			}

			return false;
		}

		private void cleanUsersStates()
		{
			TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan twoDay = new TimeSpan(2, 0, 0, 0);

			#region Clean individual user state
			Dictionary<string, UserState> copyOfUserStates = new Dictionary<string, UserState>();
			foreach (KeyValuePair<string, UserState> userState in userStates)
			{
				copyOfUserStates.Add(userState.Key, userState.Value);
			}

			foreach (KeyValuePair<string, UserState> userState in copyOfUserStates)
			{
				BlockingCollection<Tip> tips = new BlockingCollection<Tip>();

				foreach (Tip tip in userState.Value.PreviousTips)
				{
					if ((now - new TimeSpan(tip.Odigravanje.Ticks)) < twoDay)
					{
						tips.Add(tip);
					}
				}

				userState.Value.PreviousTips = tips;

				//remove user states for users that doesn't exist anymore
				bool userExists = false;
				foreach (UserSettings user in Settings.UserSettings)
				{
					if (user.Key == userState.Key)
					{
						userExists = true;
						break;
					}
				}
				if (!userExists)
				{
					userStates.Remove(userState.Key);
				}
			}
			#endregion
			#region Clean saved tips
			BlockingCollection<Tip> svdTips = new BlockingCollection<Tip>();
			List<Tip> allOldFinishedTips = new List<Tip>();
			userStates.OldFinishedTips.Values.ToList().ForEach(l => allOldFinishedTips.AddRange(l));
			foreach (Tip tip in userStates.PreviousTips)
			{
				if ((now - new TimeSpan(tip.Odigravanje.Ticks)) < twoDay || allOldFinishedTips.Find(t => t.Id == tip.Id) != null)
				{
					svdTips.Add(tip);
				}
			}

			userStates.PreviousTips = svdTips;
			#endregion
		}

		private void updateUserState(DvoznakUserSettings userSettings)
		{
			DateTime dateNow = DateTime.Now;
			if (!userStates.ContainsKey(userSettings.Key))
			{
				userStates[userSettings.Key] = new UserState();
			}

			UserState userState = userStates[userSettings.Key];

			if (userSettings.SendCSVFile && userSettings.CumulateCSVFile)
			{
				if (userState.CumulativeCSVFileValidFrom.TimeOfDay != userSettings.CumulateCSVFileFrom.TimeOfDay
					|| userState.CumulativeCSVFileValidTo.TimeOfDay != userSettings.CumulateCSVFileTo.TimeOfDay
					|| userState.CumulativeCSVFileValidTo < dateNow)
				{
					if (userState.CumulativeCSVFileValidTo < dateNow)
					{
						userState.CumulativeCSVFile = "";
					}

					DateTime to = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, userSettings.CumulateCSVFileTo.Hour, userSettings.CumulateCSVFileTo.Minute, userSettings.CumulateCSVFileTo.Second);
					if (to < dateNow)
					{
						to = to.AddDays(1);
					}
					userState.CumulativeCSVFileValidTo = to;
					DateTime from = new DateTime(to.Year, to.Month, to.Day, userSettings.CumulateCSVFileFrom.Hour, userSettings.CumulateCSVFileFrom.Minute, userSettings.CumulateCSVFileFrom.Second);
					if (from > to)
					{
						from = from.AddDays(-1);
					}
					userState.CumulativeCSVFileValidFrom = from;

					if (userState.CumulativeCSVFileValidTo < dateNow || userState.CumulativeCSVFileValidFrom > dateNow)
					{
						userState.CumulativeCSVFile = "";
					}
				}
			}
		}

		private List<UserTip> getTipsForUser(DvoznakUserSettings userSettings, List<Tip> tips, List<Tip> previousTips)
		{
			List<UserTip> tipsWithChanges = getTipsWithChanges(tips, previousTips);
			DataSet badOrGoodFilterData = getBadOrGoodFilterData(userSettings);

			List<UserTip> tipsForUser = new List<UserTip>();
			foreach (UserTip tip in tipsWithChanges)
			{
				if (NewCompletedFilter(userSettings, tip.Tip.Uspjesnost)
					&& ChangeFilter(userSettings, tip)
					&& badOrGoodFilter(tip, badOrGoodFilterData))
				{
					tipsForUser.Add(tip);
				}
			}

			return tipsForUser;
		}

		#region Filters
		#region BadOrGood
		private bool badOrGoodFilter(UserTip tip, DataSet data)
		{
			if (data == null)
				return true;

			Tip tipOverride = null;
			//ako je tip promijenjen to znači da ga je korisnik već dobio i treba ga opet dobiti sa promjenama
			bool alwaysGood = tip.DifferesOnFields.Count > 0;
			bool ret = alwaysGood;

			foreach (DataTable tbl in data.Tables)
			{
				bool filter = false;
				Dictionary<string, bool> matchColumns = new Dictionary<string, bool>();
				List<string> overrideColumns = new List<string>();
				foreach (DataColumn dc in tbl.Columns)
				{
					if (dc.ColumnName != "BadOrGood")
					{
						if (dc.ColumnName.Length > 7 && dc.ColumnName.Substring(dc.ColumnName.Length - 8, 8) == "Override")
						{
							overrideColumns.Add(dc.ColumnName);
						}
						else
						{
							bool regExColumn;
							string columnName;
							if (regExColumn = dc.ColumnName.Substring(0, 1) == "#")
							{
								columnName = dc.ColumnName.TrimStart('#');
							}
							else
							{
								columnName = dc.ColumnName;
							}
							matchColumns.Add(columnName, regExColumn);
						}
					}
					else
					{
						if (ret)
						{
							continue;
						}
						filter = true;
					}
				}

				foreach (DataRow dr in tbl.Rows)
				{
					bool match = true;
					foreach (KeyValuePair<string, bool> col in matchColumns)
					{
						//if RegEx
						if (col.Value)
						{
							Regex regEx = new Regex(dr[string.Concat("#", col.Key)].ToString(), RegexOptions.IgnoreCase);
							if (!regEx.Match(tip.Tip.GetPropertyValue(col.Key).ToString()).Success)
							{
								match = false;
								break;
							}
						}
						else if (!dr[col.Key].ToString().Equals(tip.Tip.GetPropertyValue(col.Key).ToString(), StringComparison.InvariantCultureIgnoreCase))
						{
							match = false;
							break;
						}
					}
					if (!match)
					{
						continue;
					}

					if (filter)
					{
						if (!(ret = Convert.ToBoolean((int)dr["BadOrGood"]) || alwaysGood))
						{
							return false;
						}
					}

					if (overrideColumns.Count > 0)
					{
						if (tipOverride == null)
						{
							tipOverride = (Tip)tip.Tip.Clone();
							tip.TipOverride = tipOverride;
						}
						foreach (string column in overrideColumns)
						{
							string propertyName = column.Replace("Override", "");
							tipOverride.SetPropertyValue(propertyName, dr[column]);
						}
					}

					if (filter)
					{
						break;
					}
				}
			}

			return ret;
		}

		private DataSet getBadOrGoodFilterData(DvoznakUserSettings userSettings)
		{
			if (!userSettings.IsBadOrGoodFilterOn)
			{
				return null;
			}

			List<ProcedureParameter> parameters = getBadOrGoodFilterParameters(userSettings.BadOrGoodFilterParameters);
			StringBuilder paramChecksum = new StringBuilder();
			paramChecksum.Append(userSettings.BadOrGoodFilterSPName);
			foreach (ProcedureParameter param in parameters)
			{
				paramChecksum.Append(param.Name);
				paramChecksum.Append(param.Value.ToString());
			}

			string cacheKey = Common.Common.GetMD5Hash(paramChecksum.ToString());

			if (cachedBadOrGoodData.ContainsKey(cacheKey))
			{
				return cachedBadOrGoodData[cacheKey];
			}

			SqlCommand cmd = new SqlCommand(userSettings.BadOrGoodFilterSPName);
			cmd.CommandType = CommandType.StoredProcedure;
			foreach (ProcedureParameter param in parameters)
			{
				cmd.Parameters.Add(new SqlParameter(param.Name, param.Value));
			}

			DataSet ds = getBadOrGoodFilterData(cmd);
			if (ds == null)
			{
				createBadOrGoodProcedure(userSettings.BadOrGoodFilterSPName, parameters, userSettings.BadOrGoodFilterSQL);
				ds = getBadOrGoodFilterData(cmd, false);
			}

			cachedBadOrGoodData[cacheKey] = ds;

			return ds;
		}

		private DataSet getBadOrGoodFilterData(SqlCommand cmd, bool catchException = true)
		{
			DataSet ds = new DataSet();
			try
			{
				using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["TipElderDb"].ConnectionString))
				{
					cmd.Connection = cnn;
					cnn.Open();
					new SqlDataAdapter(cmd).Fill(ds);
				}
			}
			catch (Exception ex)
			{
				if (catchException)
				{
					return null;
				}
				throw ex;
			}

			return ds;
		}

		private void createBadOrGoodProcedure(string spName, List<ProcedureParameter> parameters, string sql)
		{
			StringBuilder proc = new StringBuilder();
			proc.Append("CREATE PROCEDURE ");
			proc.AppendLine(spName);
			foreach (ProcedureParameter param in parameters)
			{
				proc.Append(param.Name);
				proc.Append(" ");
				proc.AppendLine(param.DataType);
			}
			proc.AppendLine("AS");
			proc.Append(sql);

			SqlCommand cmd = new SqlCommand(proc.ToString());
			cmd.CommandType = CommandType.Text;
			using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["TipElderDb"].ConnectionString))
			{
				cmd.Connection = cnn;
				cnn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public List<ProcedureParameter> getBadOrGoodFilterParameters(string paramsRaw)
		{
			List<ProcedureParameter> parameters = GetBadOrGoodFilterParameters(paramsRaw);
			fillBadOrGoodFilterParametersValue(parameters);
			return parameters;
		}

		public static List<ProcedureParameter> GetBadOrGoodFilterParameters(string paramsRaw)
		{
			List<ProcedureParameter> parameters = new List<ProcedureParameter>();

			string[] paramLines = paramsRaw.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string paramLine in paramLines)
			{
				string[] paramParts = paramLine.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
				string paramName = paramParts[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
				string paramDataType = paramParts[0].Replace(paramName, "").Trim();
				string paramValue = paramParts.Length > 1 ? paramParts[1] : "";

				parameters.Add(new ProcedureParameter { Name = paramName, DataType = paramDataType, ValueString = paramValue });
			}

			return parameters;
		}

		private void fillBadOrGoodFilterParametersValue(List<ProcedureParameter> parameters)
		{
			foreach (ProcedureParameter param in parameters)
			{
				switch (param.Name)
				{
					default:
						param.Value = Common.Common.GetValueBySqlDataType(param.DataType, param.ValueString);
						break;
				}
			}
		}
		#endregion

		private bool NewCompletedFilter(DvoznakUserSettings userSettings, Tip.Outcome outcome)
		{
			if (outcome == Tip.Outcome.Unfinished)
			{
				return userSettings.SendNew;
			}
			else if (userSettings.SendCompleted)
			{
				return true;
			}

			return false;
		}

		private bool ChangeFilter(DvoznakUserSettings userSettings, UserTip tip)
		{
			if (tip.DifferesOnFields.Count == 0)
			{
				return tip.Tip.Uspjesnost == Tip.Outcome.Unfinished || !userSettings.SendNew;
			}
			else
			{
				return tip.DifferesOnFields.Contains(Tip.Field.Koeficijent)
							|| tip.DifferesOnFields.Contains(Tip.Field.Ulog)
							|| tip.DifferesOnFields.Contains(Tip.Field.MinKoef)
							|| tip.DifferesOnFields.Contains(Tip.Field.IsMargina)
							|| tip.DifferesOnFields.Contains(Tip.Field.Kladionica)
							|| tip.DifferesOnFields.Contains(Tip.Field.Uspjesnost);
			}
		}
		#endregion

		private List<UserTip> getTipsWithChanges(List<Tip> newTips, List<Tip> previousTips)
		{
			List<UserTip> newTipsWithChanges = new List<UserTip>();
			List<Tip> remainingPreviousTips = new List<Tip>(previousTips);

			foreach (Tip tip in newTips)
			{
				List<Tip> similarTips = Tip.FindSimilar(remainingPreviousTips, tip);
				if (similarTips.Count > 0)
				{
					Tip lastSimilarTip = null;
					List<Tip.Field> lastFieldsThatDiffer = null;
					foreach (Tip similarTip in similarTips)
					{
						if (tip.IsEqualTo(similarTip))
						{
							remainingPreviousTips.Remove(similarTip);
							lastSimilarTip = null;
							break;
						}

						List<Tip.Field> currentFieldsThatDiffer = tip.GetFieldsThatDiffer(similarTip);
						if (lastSimilarTip == null
							|| currentFieldsThatDiffer.Count < lastFieldsThatDiffer.Count
							|| (currentFieldsThatDiffer.Count == lastFieldsThatDiffer.Count
								&& similarTip.Objavljeno >= lastSimilarTip.Objavljeno))
						{
							lastSimilarTip = similarTip;
							lastFieldsThatDiffer = currentFieldsThatDiffer;
						}
					}
					if (lastSimilarTip != null)
					{
						remainingPreviousTips.Remove(lastSimilarTip);
						newTipsWithChanges.Add(new UserTip() { Tip = tip, SimilarTip = lastSimilarTip, DifferesOnFields = tip.GetFieldsThatDiffer(lastSimilarTip) });
					}
				}
				else
				{
					newTipsWithChanges.Add(new UserTip() { Tip = tip });
				}
			}

			return newTipsWithChanges;
		}

		private List<Tip> getTips(bool getSource = true)
		{
			string documentText = getSource ? Common.Common.GetWebPageSource(WebBrowser.Url.ToString()) : ((GeckoHtmlElement)WebBrowser.Document.DocumentElement).OuterHtml;
			GeckoElementCollection uls = WebBrowser.Document.GetElementsByTagName("ul");
			StringBuilder sb = new StringBuilder();
			List<Tip> tips = new List<Tip>();

			bool paid = isPaid(WebBrowser.Document);

			foreach (GeckoElement element in uls)
			{
				if (element.GetAttribute("class") == "tiprog_list tiprog_list_block" && (!isPremium(element) || paid))
				{
					Tip tip = getTip(element, documentText);
					if (tip != null)
					{
						tips.Add(tip);
					}
				}
			}

			return tips;
		}

		private void generateAndSendMail(DvoznakUserSettings userSettings)
		{
			UserState userState = userStates[userSettings.Key];

			int numOfNewTips = 0;
			int numOfChangedTips = 0;
			foreach (UserTip tip in userState.TipsToSend)
			{
				if (tip.DifferesOnFields.Count > 0)
				{
					numOfChangedTips++;
				}
				else
				{
					numOfNewTips++;
				}
			}
			string subject = "Prognoze: novih (" + numOfNewTips.ToString() + "), promijenjenih (" + numOfChangedTips.ToString() + ")";

			string htmlFormat = generateHTMLFormat(userState);
			List<Attachment> attachments = new List<Attachment>();
			Attachment csvAtt = generateCSVAttachment(userSettings, userState);
			if (csvAtt != null)
			{
				attachments.Add(csvAtt);
			}

			MailSettings mailSettings = userSettings.MailSettings.GetMailSettings(Settings.MailSettings);
			SmtpSettings smtpSettings = userSettings.SmtpSettings.GetSmtpSettings(Settings.SmtpSettings);

			new Thread(() =>
			{
				try
				{
					Common.Common.SendMail(mailSettings.From ?? "", mailSettings.To, mailSettings.Cc ?? "",
						mailSettings.Bcc ?? "", subject: subject, body: htmlFormat,
						attachments: attachments,
						enableSsl: smtpSettings.EnableSsl ?? true, useCredentials: smtpSettings.UseCredentials ?? false,
						smtpHost: smtpSettings.SmtpHost, port: smtpSettings.Port ?? 25,
						userName: smtpSettings.UserName, password: smtpSettings.Password);

					serializeUserState = true;
					userState.TipsToSend = new BlockingCollection<UserTip>();
				}
				catch (Exception ex)
				{
					Log(ex);
				}
			}).Start();
		}

		#region Not in use
		//private Attachment generateTipsters30DSuccessCSVAttachment(DvoznakUserSettings userSettings, UserState userState)
		//{
		//    Attachment csvAtt = null;
		//    DateTime now = DateTime.Now;
		//    if (userSettings.SendTipsterSuccessLast30DCSVFile
		//        && userState.Last30DaysTipsterSuccessSentOn < new DateTime(now.Year, now.Month, now.Day))
		//    {
		//        string csv = generateTipstersSuccessCSVFormat(new List<TipsterSuccess>(userStates.Last30DaysTipsterSuccess.Values));
		//        csvAtt = new Attachment(new MemoryStream(Encoding.GetEncoding(Encoding.Default.CodePage).GetBytes(csv)), "TipsterUspjesnostZadnjih30D.csv");
		//    }

		//    return csvAtt;
		//}

		//private string generateTipstersSuccessCSVFormat(List<TipsterSuccess> tipstersSuccess)
		//{
		//    StringBuilder table = new StringBuilder();

		//    table.Append(string.Format(@"""{0}"";", "Tipster"));
		//    table.Append(string.Format(@"""{0}"";", "Prognoza"));
		//    table.Append(string.Format(@"""{0}"";", "Uspješnost"));
		//    table.Append(string.Format(@"""{0}"";", "Prihod"));
		//    table.Append(string.Format(@"""{0}"";", "ROI%"));

		//    foreach (TipsterSuccess ts in tipstersSuccess)
		//    {
		//        table.AppendLine();
		//        table.Append(string.Format(@"=""{0}"";", ts.Tipster));
		//        table.Append(string.Format(@"=""{0}"";", ts.Prognoza));
		//        table.Append(string.Format(@"=""{0}{1}"";", ts.Uspjesnost, ts.Povrata > 0 ? " (" + ts.Povrata.ToString() + ")" : ""));
		//        table.Append(string.Format(@"=""{0}"";", ts.Prihod));
		//        table.Append(string.Format(@"=""{0}"";", ts.ROI));
		//    }

		//    return table.ToString();
		//}
		#endregion

		private Attachment generateCSVAttachment(DvoznakUserSettings userSettings, UserState userState)
		{
			Attachment csvAtt = null;
			if (userSettings.SendCSVFile)
			{
				bool cumulateCSVFile = userSettings.CumulateCSVFile &&
					userState.CumulativeCSVFileValidFrom <= DateTime.Now && userState.CumulativeCSVFileValidTo >= DateTime.Now;

				bool generateHeader = !cumulateCSVFile || userState.CumulativeCSVFile.Length == 0;
				string csvFormat = generateCSVFormat(userState.TipsToSend, generateHeader);

				if (cumulateCSVFile)
				{
					if (!generateHeader)
					{
						csvFormat = userState.CumulativeCSVFile + csvFormat;
					}
					userState.CumulativeCSVFile = csvFormat;
				}
				csvAtt = new Attachment(new MemoryStream(Encoding.GetEncoding(Encoding.Default.CodePage).GetBytes(csvFormat)), "Prognoze.csv");
			}

			return csvAtt;
		}

		private string generateCSVFormat(BlockingCollection<UserTip> tips, bool generateHeader)
		{
			StringBuilder table = new StringBuilder();

			if (generateHeader)
			{
				table.Append(string.Format(@"""{0}"";", Tip.Field.Odigravanje.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Kladionica.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Tipster.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Sport.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Liga.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Dogadjaj.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.TipOdigravanja.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Ulog.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Koeficijent.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.MinKoef.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.IsMargina));
				table.Append(string.Format(@"""{0}"";", "TipsterUspjesnost"));
				table.Append(string.Format(@"""{0}"";", "TipsterUlogPovrat"));
				table.Append(string.Format(@"""{0}"";", "TipsterROI"));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Objavljeno.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Premium.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Zarada.ToString()));
				table.Append(string.Format(@"""{0}"";", Tip.Field.Misc.ToString()));
			}

			foreach (UserTip t in tips)
			{
				Tip tip = t.TipOverride ?? t.Tip;
				table.AppendLine();
				table.Append(string.Format(@"=""{0}"";", tip.Odigravanje.ToString("dd.MM.yy HH:mm")));
				table.Append(string.Format(@"=""{0}"";", tip.Kladionica));
				table.Append(string.Format(@"=""{0}"";", tip.Tipster));
				table.Append(string.Format(@"=""{0}"";", tip.Sport));
				table.Append(string.Format(@"=""{0}"";", tip.Liga));
				table.Append(string.Format(@"=""{0}"";", tip.Dogadjaj));
				table.Append(string.Format(@"=""{0}"";", tip.TipOdigravanja));
				table.Append(string.Format(@"=""{0}/10"";", tip.Ulog));
				table.Append(string.Format(@"=""{0}"";", tip.Koeficijent));
				table.Append(string.Format(@"=""{0}"";", tip.MinKoef));
				table.Append(string.Format(@"=""{0}"";", tip.IsMargina));
				table.Append(string.Format(@"=""{0}%: {1} / {2}{3}"";", tip.TipsterBrojOklada == 0 ? 0 : (int)((decimal)tip.TipsterBrojPogodaka / tip.TipsterBrojOklada * 100), tip.TipsterBrojOklada, tip.TipsterBrojPogodaka,
					(tip.TipsterBrojPovrata == 0 ? "" : " (" + tip.TipsterBrojPovrata.ToString() + ")")));
				table.Append(string.Format(@"=""{0} / {1} ({2})"";", tip.TipsterUlog, tip.TipsterPovrat, (tip.TipsterPovrat - tip.TipsterUlog).ToString("0.00")));
				table.Append(string.Format(@"=""{0}"";", tip.TipsterROI.ToString("0.0")));
				table.Append(string.Format(@"=""{0}"";", tip.Objavljeno.ToString("dd.MM. HH:mm")));
				table.Append(string.Format(@"=""{0}"";", tip.Premium));
				table.Append(string.Format(@"=""{0}"";", tip.Zarada == null ? "" : tip.Zarada.ToString()));
				table.Append(string.Format(@"=""{0}"";", tip.Misc ?? ""));
			}

			return table.ToString();
		}

		private string generateHTMLFormat(UserState userState)
		{
			StringBuilder body = new StringBuilder();

			body.AppendLine("<html><head></head><body>");
			body.AppendLine(@"<style type=""text/css"">");
			body.AppendLine(File.ReadAllText(Path.Combine(Application.StartupPath, @"Sniffs\Dvoznak\Tip.css")));
			body.AppendLine("</style>");

			foreach (UserTip tip in userState.TipsToSend)
			{
				body.AppendLine(getTipHTML(tip));
			}

			body.Append("</body><html>");

			return body.ToString();
		}

		private string getTipHTML(UserTip userTip)
		{
			Tip tip = userTip.TipOverride ?? userTip.Tip;
			List<Tip.Field> changedFields = userTip.DifferesOnFields;
			decimal ulogRazlika = tip.TipsterPovrat - tip.TipsterUlog;

			string s = @"<ul class=""tiprog_list tiprog_list_block"">
				<li title=""" + tip.Sport + ", " + tip.Liga + ": " + tip.Dogadjaj + @""">
					<div class=""tpl_center"">
						<img src=""" + tip.SportImgUrl + @""" width=""16"" height=""16"" alt=""" + tip.Sport + @""" title=""" + tip.Sport + @""" class=""on_sport_ic""> 
						<div class=""flag_spacing""><span class=""" + tip.ZastavaClass + @""" title=""" + tip.Liga + @""">&nbsp;</span></div>
					</div>
					<div class=""tpl_circle"">
						<div class=""tpl_circle_margin""><span class=""" + (changedFields.Count > 0 ? "circleChanged" : "circleNew") + @"""></span></div>
					</div>"
					+ (tip.Premium ? @"<img src=""http://dvoznak.com/img/icons/gold_star.png"" width=""32"" height=""32"" class=""gold_tip_mark""></img>" : "")
					+ @"<table class=""tpl_right"">
						<tr><td>
							<div class=""tpl_liga""" + (changedFields.Contains(Tip.Field.Liga) ? @" style=""color:red;""" : "") + @">" + tip.Liga + " (" + tip.Sport + @")</div> 
							<div class=""tpl_match" + (changedFields.Contains(Tip.Field.Dogadjaj) ? " changed" : "") + @"""><a href=""" + ""/*tip.DogadjajUrl*/ + @""">" + tip.Dogadjaj + @"</a></div>
						</td></tr>
					</table>
					<div class=""clear""></div>
				</li>
				<li>
					<div class=""tipro_title" + (tip.Premium ? " goldt_title_bg" : "") + @""">
						<div class=""tiprot_left""><strong class=""" + (changedFields.Contains(Tip.Field.TipOdigravanja) ? "changed" : "") + @""">" + tip.TipOdigravanja + @"</strong></div>
						<div class=""tiprot_right" + (changedFields.Contains(Tip.Field.Odigravanje) ? " changed" : "") + @""">" + tip.Odigravanje.ToString("dd.MM. HH:mm") + @"</div>
					</div>
					<div class=""clear""></div>
					<div class=""tipoprog_content_wrap"">
						<div class=""tipoprog_cw_data"">
							<table class=""tipoprog_table_small"">
								<tbody>"
								+ (!string.IsNullOrEmpty(tip.Rezultat) ?
									@"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Rezultat) ? " changed" : "") + @""">Rezultat:</td>
										<td class=""tipot_value"">" + tip.Rezultat + @"</td>
									</tr>" : "")
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Tipster) ? " changed" : "") + @""">
											<a class=""dvotip"" href=""" + ""/*tip.TipsterUrl*/ + @""">" + tip.Tipster + @"</a>
										</td>
										<td class=""tipot_value" + (changedFields.Contains(Tip.Field.TipsterBrojOklada) || changedFields.Contains(Tip.Field.TipsterBrojPogodaka)
																 || changedFields.Contains(Tip.Field.TipsterUlog) || changedFields.Contains(Tip.Field.TipsterPovrat) ? " changed" : "") + @""">"
										+ string.Format(@"Uspješnost ({0}%): {1} / {2}{3}<br/>Ulog / Povrat: {4} / {5} ({9}{6})<br/>ROI: {7}{8}%",
										tip.TipsterBrojOklada == 0 ? 0 : (int)((decimal)tip.TipsterBrojPogodaka / tip.TipsterBrojOklada * 100), tip.TipsterBrojOklada, tip.TipsterBrojPogodaka,
											(tip.TipsterBrojPovrata == 0 ? "" : " (" + tip.TipsterBrojPovrata.ToString() + ")"),
											tip.TipsterUlog, tip.TipsterPovrat, ulogRazlika.ToString("0.00"),
											(tip.TipsterROI < 0 ? "" : "+"), tip.TipsterROI.ToString("0.0"), (ulogRazlika < 0 ? "" : "+")) + @"</td>
									</tr>"
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Koeficijent) ? " changed" : "") + @""">Koeficijent:</td>
										<td class=""tipot_value"">" + tip.Koeficijent.ToString("0.00") + @"</td>
									</tr>"
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Ulog) ? " changed" : "") + @""">Ulog:</td>
										<td class=""tipot_value"">" + string.Format(@"{0}/10", tip.Ulog) + @"</td>
									</tr>"
								+ (tip.Zarada != null ?
									@"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Zarada) ? " changed" : "") + @""">Zarada:</td>
										<td class=""tipot_value""><font color=""" + (tip.Zarada.Value < 0 ? "red" : "green") + @""">" + string.Format(@"{0}{1}", tip.Zarada.Value > 0 ? "+" : "", tip.Zarada.Value) + @"</font></td>
									</tr>" : "")
								+ (tip.MinKoef != null ?
									@"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.MinKoef) ? " changed" : "") + @""">Min. koef.:</td>
										<td class=""tipot_value"">" + tip.MinKoef.Value.ToString("0.00") + @"</td>
									</tr>" : "")
								+ (!string.IsNullOrEmpty(tip.IsMargina) ?
									@"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.IsMargina) ? " changed" : "") + @""">Is. margina:</td>
										<td class=""tipot_value"">" + tip.IsMargina + @"</td>
									</tr>" : "")
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Objavljeno) ? " changed" : "") + @""">Objavljeno:</td>
										<td class=""tipot_value"">" + tip.Objavljeno.ToString("dd.MM. HH:mm") + @"</td>
									</tr>"
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Kladionica) ? " changed" : "") + @""">Kladionica:</td>
										<td class=""tipot_value""><a href=""" + tip.KladionicaUrl + @""" target=""_blank"" title=""" + tip.Kladionica + @"""><img src=""" + tip.KladionicaImgUrl + @""" width=""78"" height=""15""></a></td>
									</tr>"
								+ @"<tr>
										<td class=""tipot_title" + (changedFields.Contains(Tip.Field.Uspjesnost) || changedFields.Contains(Tip.Field.UspjesnostTitle) ? " changed" : "") + @""">Uspješnost:</td>
										<td class=""tipot_value""><span class=""nlsnbt_title_uspjesnost dvotip"" title=""" + (tip.UspjesnostTitle ?? "") + @"""><img src=""" + iconUrls[tip.Uspjesnost] + @"""></img></span></td>
									</tr>"
								+ (!string.IsNullOrEmpty(tip.Misc) ?
									@"<tr>
										<td class=""tipot_title"">Misc:</td>
										<td class=""tipot_value"">" + tip.Misc + @"</td>
									</tr>" : "")
								+ @"</tbody>
						</table>
						</div><span class=""" + (changedFields.Contains(Tip.Field.Opis) ? "changed" : "") + @""">" + tip.Opis + @"</span>
						<!--a href=""" + ""/*tip.DogadjajUrl*/ + @""">Detaljnije</a-->
					</div>
				</li>
			</ul>";

			return s;
		}

		private bool isPaid(GeckoDocument htmlDoc)
		{
			foreach (GeckoElement div in htmlDoc.GetElementsByTagName("div"))
			{
				if (div.GetAttribute("id") == "userbar" && (div.GetAttribute("style") ?? "").Contains("none"))
				{
					return true;
				}
			}

			return false;
		}

		private bool isPremium(GeckoElement element)
		{
			foreach (GeckoElement li in element.GetElementsByTagName("li"))
			{
				if (li.GetAttribute("class") == "gold_tip_mark")
				{
					return true;
				}
			}

			return false;
		}

		private Tip getTip(GeckoElement element, string documentText)
		{
			Tip tip = new Tip();

			//HTML
			//tip.HTML = element.OuterHtml;

			//Premium
			foreach (GeckoElement li in element.GetElementsByTagName("li"))
			{
				if (li.GetAttribute("class") == "gold_tip_mark")
				{
					tip.Premium = true;
					break;
				}
			}

			//Sport
			foreach (GeckoElement div in element.GetElementsByTagName("div"))
			{
				if (div.GetAttribute("class") == "tpl_center")
				{
					foreach (GeckoElement img in div.GetElementsByTagName("img"))
					{
						tip.Sport = img.GetAttribute("title").Trim();
						tip.SportImgUrl = WebBrowser.Url.Scheme + "://" + WebBrowser.Url.Host + "/" + img.GetAttribute("src");
						break;
					}
					break;
				}
			}

			//Liga, Događaj
			foreach (GeckoElement div in element.GetElementsByTagName("div"))
			{
				if (div.GetAttribute("class") == "tpl_right")
				{
					//Liga
					foreach (GeckoElement span in div.GetElementsByTagName("span"))
					{
						tip.Liga = span.TextContent.Trim();
						break;
					}

					//Događaj
					foreach (GeckoElement a in div.GetElementsByTagName("a"))
					{
						tip.Dogadjaj = a.TextContent.Trim();
						break;
					}
				}
				else if (div.GetAttribute("class") == "tpl_center")
				{
					foreach (GeckoElement span in element.GetElementsByTagName("span"))
					{
						tip.ZastavaClass = span.GetAttribute("class");
						break;
					}
				}
			}

			//Tip, Odigravanje
			foreach (GeckoElement div in element.GetElementsByTagName("div"))
			{
				//Tip
				if (div.GetAttribute("class").Contains("tiprot_left"))
				{
					foreach (GeckoElement strong in div.GetElementsByTagName("strong"))
					{
						tip.TipOdigravanja = strong.TextContent.Trim();
						break;
					}
				}

				//Odigravanje
				if (div.GetAttribute("class").Contains("tiprot_right"))
				{
					DateTime odigravanje;
					if (DateTime.TryParse(div.TextContent.Trim(), out odigravanje))
					{
						tip.Odigravanje = odigravanje;
					}
					else
					{
						tip.Odigravanje = DateTime.Parse(div.TextContent.Trim().Insert(6, DateTime.Now.Year.ToString()));
					}
					break;
				}
			}

			GeckoElement divTable = null;
			foreach (GeckoElement div in element.GetElementsByTagName("div"))
			{
				if (div.GetAttribute("class").Contains("tipoprog_cw_data"))
				{
					divTable = div;
					break;
				}
			}

			if (divTable == null)
			{
				return null;
			}

			//Ostalo
			foreach (GeckoHtmlElement table in divTable.GetElementsByTagName("table"))
			{
				if (table.GetAttribute("class") == "tipoprog_table_small")
				{
					GeckoElementCollection trs = table.GetElementsByTagName("tr");
					foreach (GeckoHtmlElement tr in trs)
					{
						string type = "";
						foreach (GeckoElement td in tr.GetElementsByTagName("td"))
						{
							if (td.GetAttribute("class") == "tipot_title")
							{
								type = td.TextContent.Replace(":", "").Trim();
							}
							break;
						}

						//ako se radi o povijesti onda nema labela Uspješnost
						if (type.Trim().Length == 0)
						{
							foreach (GeckoHtmlElement td in tr.GetElementsByTagName("span"))
							{
								if (td.GetAttribute("class").Contains("nlsnbt_title_uspjesnost"))
								{
									type = "Uspješnost";
								}
								break;
							}
						}

						switch (type)
						{
							case "Rezultat":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.Rezultat = td.TextContent.Trim();
										break;
									}
								}
								break;
							case "Tipster":
								foreach (GeckoHtmlElement a in tr.GetElementsByTagName("a"))
								{
									tip.TipsterUrl = WebBrowser.Url.Scheme + "://" + WebBrowser.Url.Host + "/" + a.GetAttribute("href");
									tip.Tipster = a.TextContent.Trim();
									int brojOklada, brojPogodaka, brojPovrata;
									decimal ulog, povrat;
									getTipsterUspjesnostSegments(getTipsterTitle(a, documentText), out brojOklada, out  brojPogodaka, out  brojPovrata, out  ulog, out  povrat);
									tip.TipsterBrojOklada = brojOklada;
									tip.TipsterBrojPogodaka = brojPogodaka;
									tip.TipsterBrojPovrata = brojPovrata;
									tip.TipsterUlog = ulog;
									tip.TipsterPovrat = povrat;
									break;
								}
								break;
							case "Koeficijent":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.Koeficijent = decimal.Parse(td.TextContent.Trim(), NumberFormatInfo.InvariantInfo);
										break;
									}
								}
								break;
							case "Ulog":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.Ulog = int.Parse(td.TextContent.Trim().Split(new string[] { "/" }, StringSplitOptions.None)[0]);
										break;
									}
								}
								break;
							case "Zarada":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.Zarada = decimal.Parse(td.TextContent.Trim(), NumberFormatInfo.InvariantInfo);
										break;
									}
								}
								break;
							case "Min. koef.":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										string minKoef = td.TextContent.Trim();
										tip.MinKoef = minKoef.Length == 0 ? null : (decimal?)decimal.Parse(minKoef, NumberFormatInfo.InvariantInfo);
										break;
									}
								}
								break;
							case "Objavljeno":
								foreach (GeckoHtmlElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.Objavljeno = DateTime.Parse(td.TextContent.Trim().Insert(6, tip.Odigravanje.Year.ToString()));
										if ((tip.Odigravanje - tip.Objavljeno).Days < -300)
										{
											tip.Objavljeno = tip.Objavljeno.AddYears(-1);
										}
										break;
									}
								}
								break;
							case "Kladionica":
								foreach (GeckoHtmlElement a in tr.GetElementsByTagName("a"))
								{
									tip.Kladionica = a.GetAttribute("title");
									tip.KladionicaUrl = a.GetAttribute("href");
									foreach (GeckoElement img in a.GetElementsByTagName("img"))
									{
										tip.KladionicaImgUrl = WebBrowser.Url.Scheme + "://" + WebBrowser.Url.Host + "/" + img.GetAttribute("src");
										break;
									}
									break;
								}
								break;
							case "Uspješnost":
								Regex regex = new Regex(@"<span class=""?nlsnbt_title_uspjesnost (u[0-9]?)", RegexOptions.IgnoreCase);
								Match match = regex.Match(Common.Common.ReplaceHTMLSpecialCharacters(tr.InnerHtml));
								if (match.Success)
								{
									tip.Uspjesnost = EnumStringValue.GetEnumValue<Tip.Outcome>(match.Groups[1].Captures[0].Value).Value;
								}
								//tip.UspjesnostTitle = getUspjesnostTitle(table, documentText);
								break;
							case "Is. margina":
								foreach (GeckoElement td in tr.GetElementsByTagName("td"))
								{
									if (td.GetAttribute("class") == "tipot_value")
									{
										tip.IsMargina = td.TextContent.Trim();
										break;
									}
								}
								break;
						}
					}
					break;
				}
			}

			divTable.ParentElement.RemoveChild(divTable);

			//Opis, Detaljnije
			foreach (GeckoHtmlElement div in element.GetElementsByTagName("div"))
			{
				if (div.GetAttribute("class") == "tipoprog_content_wrap")
				{
					//Detaljnije
					foreach (GeckoHtmlElement a in div.GetElementsByTagName("a"))
					{
						if (a.TextContent == "Detaljnije")
						{
							tip.DogadjajUrl = WebBrowser.Url.Scheme + "://" + WebBrowser.Url.Host + "/" + a.GetAttribute("href");
							a.Parent.RemoveChild(a);
							break;
						}
					}

					tip.Opis = div.InnerHtml;

					break;
				}
			}

			return tip;
		}

		private void getTipsterUspjesnostSegments(string uspjesnost, out int brojOklada, out int brojPogodaka, out int brojPovrata, out decimal ulog, out decimal povrat)
		{
			brojOklada = 0;
			brojPogodaka = 0;
			brojPovrata = 0;
			ulog = 0;
			povrat = 0;
			try
			{
				string[] rows = uspjesnost.Split(new string[] { "\r\n" }, StringSplitOptions.None);
				string[] segments = rows[0].Split(new string[] { ":", "/", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
				string[] segments2 = rows[1].Split(new string[] { ":", "/", "(" }, StringSplitOptions.None);
				brojOklada = int.Parse(segments[1]);
				brojPogodaka = int.Parse(segments[2]);
				brojPovrata = int.Parse(segments.Length > 3 ? segments[3] : "0");
				ulog = decimal.Parse(segments2[2], NumberFormatInfo.InvariantInfo);
				povrat = decimal.Parse(segments2[3], NumberFormatInfo.InvariantInfo);
			}
			catch
			{
			}
		}

		private string getTipsterTitle(GeckoHtmlElement a, string documentText)
		{
			Regex regex = new Regex(@"<[aA].*href *= *""?.*id_autor *= *([0-9]+)""?.*>");
			Match match = regex.Match(Common.Common.ReplaceHTMLSpecialCharacters(a.Parent.InnerHtml));

			if (match.Success)
			{
				string idAutor = match.Groups[1].Captures[0].Value;
				regex = new Regex(@"<[aA].*href *= *""?.*id_autor *= *" + idAutor + @"""?.*title *= *""(.*)"".*>.*</[aA]>");
				match = regex.Match(documentText);
				if (match.Success)
				{
					return match.Groups[1].Captures[0].Value.Replace("<br>", "\r\n");
				}
			}

			return "";
		}

		private string getUspjesnostTitle(GeckoHtmlElement table, string documentText)
		{
			Regex regex = new Regex(@"<[aA].*href *= *""?.*id_autor *= *([0-9]+)""?.*>");
			Match match = regex.Match(Common.Common.ReplaceHTMLSpecialCharacters(table.InnerHtml));

			if (match.Success)
			{
				string idAutor = match.Groups[1].Captures[0].Value;
				regex = new Regex(@"<[aA].*href *= *""?.*id_autor *= *" + idAutor + @"""?.*title *= *""(.*)"".*>.*</[aA]>");
				match = regex.Match(documentText);
				if (match.Success)
				{
					return match.Groups[1].Captures[0].Value.Replace("<br>", "\r\n");
				}
			}

			return "";
		}

		public override void Start()
		{
			processCoordinator();
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}

	public class ProcedureParameter
	{
		public string Name { get; set; }
		public string DataType { get; set; }
		public object Value { get; set; }
		public string ValueString { get; set; }
	}

	public class UserTip
	{
		public Tip SimilarTip;

		public Tip Tip { get; set; }
		public List<Tip.Field> DifferesOnFields { get; set; }
		public Tip TipOverride { get; set; }

		public UserTip()
		{
			DifferesOnFields = new List<Tip.Field>();
		}
	}
}
