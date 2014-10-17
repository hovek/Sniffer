using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Sniffer.Sniffs.Dvoznak
{
	public class TipsterSuccess
	{
		public string Tipster { get; set; }
		public int Prognoza { get; set; }
		public int Uspjesnost { get; set; }
		public int Povrata { get; set; }
		public decimal Prihod { get; set; }
		public decimal ROI { get; set; }
	}

	public class UserStateDictionary : Dictionary<string, UserState>
	{
		public event EventHandler OnDestruction;

        /// <summary>
        /// Prijašnje snimljene prognoze u bazu.
        /// </summary>
		public BlockingCollection<Tip> PreviousTips;
		public List<Tip> _PreviousTips
		{
			get
			{
				return new List<Tip>(PreviousTips);
			}
			set
			{
				PreviousTips = new BlockingCollection<Tip>();
				foreach (Tip tip in value)
				{
					PreviousTips.Add(tip);
				}
			}
		}

		public BlockingCollection<DateTime> DaysToCheck;
		public List<DateTime> _DaysToCheck
		{
			get
			{
				return new List<DateTime>(DaysToCheck);
			}
			set
			{
				DaysToCheck = new BlockingCollection<DateTime>();
				foreach (DateTime day in value)
				{
					DaysToCheck.Add(day);
				}
			}
		}

		/// <summary>
		/// Prošli završeni tipovi koji su tek u prošlosti označeni kao završeni, ubacuju se u ovu listu i tretiraju se kao da su se pojavili na današnji dan.
		/// </summary>
		public ConcurrentDictionary<DateTime, List<Tip>> OldFinishedTips;
		public Dictionary<DateTime, List<Tip>> _OldFinishedTips
		{
			get
			{
				Dictionary<DateTime, List<Tip>> newFT = new Dictionary<DateTime, List<Tip>>();
				foreach (KeyValuePair<DateTime, List<Tip>> t in OldFinishedTips)
				{
					newFT.Add(t.Key, t.Value);
				}
				return newFT;
			}
			set
			{
				OldFinishedTips = new ConcurrentDictionary<DateTime, List<Tip>>();
				foreach (KeyValuePair<DateTime, List<Tip>> t in value)
				{
					OldFinishedTips[t.Key] = t.Value;
				}
			}
		}

		public DateTime PreviousTipsLastChecked { get; set; }

		public UserStateDictionary()
		{
			PreviousTips = new BlockingCollection<Tip>();
			DaysToCheck = new BlockingCollection<DateTime>();
			OldFinishedTips = new ConcurrentDictionary<DateTime, List<Tip>>();
			PreviousTipsLastChecked = DateTime.MinValue;
		}

		~UserStateDictionary()
		{
			if (OnDestruction != null)
			{
				OnDestruction(this, EventArgs.Empty);
			}
		}
	}

	public class UserState
	{
		public BlockingCollection<Tip> PreviousTips;
		public List<Tip> _PreviousTips
		{
			get
			{
				return new List<Tip>(PreviousTips);
			}
			set
			{
				PreviousTips = new BlockingCollection<Tip>();
				foreach (Tip tip in value)
				{
					PreviousTips.Add(tip);
				}
			}
		}

		public BlockingCollection<UserTip> TipsToSend;
		public List<UserTip> _TipsToSend
		{
			get
			{
				return new List<UserTip>(TipsToSend);
			}
			set
			{
				TipsToSend = new BlockingCollection<UserTip>();
				foreach (UserTip tip in value)
				{
					TipsToSend.Add(tip);
				}
			}
		}

		public string CumulativeCSVFile
		{ get; set; }

		public DateTime CumulativeCSVFileValidFrom
		{ get; set; }

		public DateTime CumulativeCSVFileValidTo
		{ get; set; }

		public UserState()
		{
			TipsToSend = new BlockingCollection<UserTip>();
			PreviousTips = new BlockingCollection<Tip>();
		}
	}
}
