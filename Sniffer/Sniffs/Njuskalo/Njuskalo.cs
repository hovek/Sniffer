using Gecko;
using Polenter.Serialization;
using Sniffer.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace Sniffer.Sniffs.Njuskalo
{
    public class Njuskalo : Sniff
    {
        private SniffSettings _settings;
        private static bool firstTimeUsersStateCleaned = false;
        private System.Windows.Forms.Timer tmrUserStateCleanup = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer tmrUserStateSerialization = new System.Windows.Forms.Timer();
        private static UserStateDictionary userStates = new UserStateDictionary();
        private volatile static bool serializeUserState = false;

        private Dictionary<NjuskaloUserSettings, List<Ad>> userAdBuffer = new Dictionary<NjuskaloUserSettings, List<Ad>>();

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

        public Njuskalo(HoveWebBrowser webBrowser)
            : base(webBrowser)
        {
            tmrUserStateCleanup.Tick += new EventHandler(tmrUserStateCleanup_Tick);
            tmrUserStateCleanup.Interval = 3600000;
            tmrUserStateCleanup.Enabled = true;

            tmrUserStateSerialization.Tick += new EventHandler(tmrUserStateSerialization_Tick);
            tmrUserStateSerialization.Interval = 1000;
            tmrUserStateSerialization.Enabled = true;

            Sniffs.Add("Start", processStart);
        }

        static Njuskalo()
        {
            SharpSerializer serializer = new SharpSerializer();
            try
            {
                userStates = (UserStateDictionary)serializer.Deserialize("Sniffs/Njuskalo/UsersStates.xml");
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
            serializer.Serialize(userStates, "Sniffs/Njuskalo/UsersStates.xml");
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
        }

        private void cleanUsersStates()
        {
            DateTime twoWeeksInPast = DateTime.Now.AddDays(-14);
            foreach (KeyValuePair<string, UserState> userState in userStates)
            {
                BlockingCollection<Ad> validElements = new BlockingCollection<Ad>();

                foreach (var p in userState.Value.PreviousElements.Where(p => p.CreatedOn >= twoWeeksInPast))
                    validElements.Add(p);

                userState.Value.PreviousElements = validElements;
            }
        }

        #region Processes
        private bool processStart(object state)
        {
            foreach (NjuskaloUserSettings userSettings in Settings.UserSettings)
                Sniffs.Add(string.Concat("UserNavigate_", userSettings.Key), processUserNavigate, userSettings);

            return true;
        }

        private bool processUserNavigate(object state)
        {
            try
            {
                NjuskaloUserSettings userSettings = (NjuskaloUserSettings)state;
                if (!userSettings.IsActive) return true;

                Sniffs.InsertAfter(string.Concat("UserNavigate_", userSettings.Key), string.Concat("UserNavigated_", userSettings.Key), processUserNavigated, userSettings);
                WebBrowser.Navigate(userSettings.Url);

                return true;
            }
            catch (Exception e)
            {
                Log(e);
                throw e;
            }
        }

        private bool processUserNavigated(object state)
        {
            NjuskaloUserSettings userSettings = (NjuskaloUserSettings)state;
            if (!userAdBuffer.ContainsKey(userSettings)) userAdBuffer[userSettings] = new List<Ad>();
            userAdBuffer[userSettings].AddRange(getAds());

            if (goToNextPage())
                return false;

            return true;
        }

        private bool goToNextPage()
        {
            return true;
        }
        #endregion

        private bool isElementProcessed(Ad element, List<Ad> previousElement)
        {
            foreach (Ad oldElement in previousElement)
            {
                if (element.Equals(oldElement))
                    return true;
            }

            return false;
        }

        private List<Ad> getAds()
        {
            List<Ad> elements = new List<Ad>();
            GeckoNodeCollection ads = WebBrowser.Document.GetElementsByClassName("ad_item");

            for (int i = 0; i < ads.Count; i++)
            {
                GeckoElementCollection divs = ((GeckoHtmlElement)ads[i]).GetElementsByTagName("div");
                foreach (GeckoElement div in divs)
                {
                    if (div.GetAttribute("class") == "desc")
                    { }
                    else if (div.GetAttribute("class") == "price")
                    { }
                }
            }

            return elements;
        }

        private void generateAndSendMail(NjuskaloUserSettings userSettings)
        {
            UserState userState = userStates[userSettings.Key];

            if (userState.ElementsToSend.Count == 0) return;

            string subject = string.Concat("Njuskalo oglasi (", userState.ElementsToSend.Count, ")");
            string htmlFormat = generateHTMLFormat(userState);

            MailSettings mailSettings = userSettings.MailSettings.GetMailSettings(Settings.MailSettings);
            SmtpSettings smtpSettings = userSettings.SmtpSettings.GetSmtpSettings(Settings.SmtpSettings);

            new Thread(() =>
            {
                try
                {
                    Common.Common.SendMail(mailSettings.From ?? "", mailSettings.To, mailSettings.Cc ?? "",
                        mailSettings.Bcc ?? "", subject: subject, body: htmlFormat,
                        attachments: null,
                        enableSsl: smtpSettings.EnableSsl ?? true, useCredentials: smtpSettings.UseCredentials ?? false,
                        smtpHost: smtpSettings.SmtpHost, port: smtpSettings.Port ?? 25,
                        userName: smtpSettings.UserName, password: smtpSettings.Password);

                    serializeUserState = true;
                    userState.ElementsToSend = new BlockingCollection<Ad>();
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }).Start();
        }

        private string generateHTMLFormat(UserState userState)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("<html><head></head><body>");
            body.AppendLine(@"<style type=""text/css"">");
            body.AppendLine(File.ReadAllText(Path.Combine(Application.StartupPath, @"Sniffs\Njuskalo\Ad.css")));
            body.AppendLine("</style>");

            foreach (Ad element in userState.ElementsToSend)
            {
                body.AppendLine(getElementHTML(element));
                body.AppendLine("<br/>");
            }

            body.Append("</body><html>");

            return body.ToString();
        }

        private string getElementHTML(Ad element)
        {
            StringBuilder html = new StringBuilder();

            html.Append(element.Date.Html);
            html.Append(element.Title.Html);
            html.Append(element.AllText.Html);

            return html.ToString();
        }

        public override void Start()
        {
            base.Start();
        }
    }

    public class Ad : IEquatable<Ad>
    {
        public DateTime CreatedOn { get; set; }
        public AdData Date { get; set; }
        public AdData Title { get; set; }
        public List<AdData> Texts { get; set; }
        public AdData AllText { get; set; }

        public Ad()
        {
            CreatedOn = DateTime.Now;
        }

        public string GetCumulativeText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Title.Text);
            sb.Append(Date.Text);
            sb.Append(AllText.Text);
            return sb.ToString();
        }

        public bool Equals(Ad other)
        {
            return Common.Common.FormatTextForEqualityComparison(this.GetCumulativeText()) == Common.Common.FormatTextForEqualityComparison(other.GetCumulativeText());
        }
    }

    public class AdData
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }
}
