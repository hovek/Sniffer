using Gecko;
using Polenter.Serialization;
using Sniffer.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Sniffer.Sniffs.Veky
{
    public class Veky : Sniff
    {
        private SniffSettings _settings;
        private static bool firstTimeUsersStateCleaned = false;
        private System.Windows.Forms.Timer tmrUserStateCleanup = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer tmrUserStateSerialization = new System.Windows.Forms.Timer();
        private static VekyUserStateDictionary userStates = new VekyUserStateDictionary();
        private volatile static bool serializeUserState = false;

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

        public Veky(HoveWebBrowser webBrowser)
            : base(webBrowser)
        {
            tmrUserStateCleanup.Tick += new EventHandler(tmrUserStateCleanup_Tick);
            tmrUserStateCleanup.Interval = 3600000;
            tmrUserStateCleanup.Enabled = true;

            tmrUserStateSerialization.Tick += new EventHandler(tmrUserStateSerialization_Tick);
            tmrUserStateSerialization.Interval = 1000;
            tmrUserStateSerialization.Enabled = true;

            Sniffs.Add("process", process);
        }

        static Veky()
        {
            SharpSerializer serializer = new SharpSerializer();
            try
            {
                userStates = (VekyUserStateDictionary)serializer.Deserialize("Sniffs/Veky/VekyUsersStates.xml");
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
            serializer.Serialize(userStates, "Sniffs/Veky/VekyUsersStates.xml");
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
            foreach (KeyValuePair<string, VekyUserState> userState in userStates)
            {
                BlockingCollection<Post> validPosts = new BlockingCollection<Post>();

                foreach (var p in userState.Value.PreviousPosts.Where(p => p.CreatedOn >= twoWeeksInPast))
                    validPosts.Add(p);

                userState.Value.PreviousPosts = validPosts;
            }
        }

        private bool process(object state)
        {
            try
            {
                List<Post> readPosts = getPosts();

                foreach (VekyUserSettings userSettings in Settings.UserSettings)
                {
                    if (!userSettings.IsActive) continue;

                    if (!userStates.ContainsKey(userSettings.Key))
                        userStates[userSettings.Key] = new VekyUserState();

                    VekyUserState userState = userStates[userSettings.Key];

                    List<Post> previousPosts = new List<Post>(userState.PreviousPosts);
                    List<Post> previousAndNewPosts = new List<Post>(userState.PreviousPosts);

                    foreach (Post post in readPosts)
                    {
                        if (!isPostProcessed(post, previousPosts))
                        {
                            userState.PostsToSend.Add(post);
                            previousAndNewPosts.Add(post);
                        }
                    }

                    userState.PreviousPosts = new BlockingCollection<Post>();
                    previousAndNewPosts.ForEach(t => userState.PreviousPosts.Add(t));

                    generateAndSendMail(userSettings);
                }
                return true;
            }
            catch (Exception e)
            {
                Log(e);
                throw e;
            }
        }

        private bool isPostProcessed(Post post, List<Post> previousPosts)
        {
            foreach (Post oldPost in previousPosts)
            {
                if (post.Equals(oldPost))
                    return true;
            }

            return false;
        }

        private List<Post> getPosts()
        {
            List<Post> posts = new List<Post>();
            GeckoNodeCollection dates = WebBrowser.Document.GetElementsByClassName("blogdate");
            GeckoNodeCollection titles = WebBrowser.Document.GetElementsByClassName("blogtitle");
            GeckoNodeCollection blogtexts = WebBrowser.Document.GetElementsByClassName("blogtext");
            for (int i = 0; i < titles.Count; i++)
            {
                string titleHtml = ((GeckoHtmlElement)titles[i]).OuterHtml;
                int hrefIndex = titleHtml.IndexOf("/post/");
                if (hrefIndex != -1)
                {
                    titleHtml = titleHtml.Insert(hrefIndex, "http://veky86.bloger.index.hr");
                }

                PostData title = new PostData
                {
                    Text = titles[i].TextContent,
                    Html = titleHtml
                };

                PostData date = new PostData
                {
                    Text = dates[i].TextContent,
                    Html = ((GeckoHtmlElement)dates[i]).OuterHtml
                };

                List<PostData> texts = new List<PostData>();
                GeckoHtmlElement blogText = (GeckoHtmlElement)blogtexts[i];
                foreach (GeckoHtmlElement text in blogText.GetElements("span"))
                {
                    texts.Add(new PostData
                    {
                        Text = text.TextContent,
                        Html = text.OuterHtml
                    });
                }

                posts.Add(new Post
                {
                    Date = date,
                    Title = title,
                    Texts = texts,
                    AllText = new PostData
                    {
                        Text = blogText.TextContent,
                        Html = blogText.OuterHtml
                    }
                });
            }

            return posts;
        }

        private void generateAndSendMail(VekyUserSettings userSettings)
        {
            VekyUserState userState = userStates[userSettings.Key];

            if (userState.PostsToSend.Count == 0) return;

            string subject = string.Concat("Veky prognoze (", userState.PostsToSend.Count, ")");
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
                    userState.PostsToSend = new BlockingCollection<Post>();
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }).Start();
        }

        private string generateHTMLFormat(VekyUserState userState)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("<html><head></head><body>");
            body.AppendLine(@"<style type=""text/css"">");
            body.AppendLine(File.ReadAllText(Path.Combine(Application.StartupPath, @"Sniffs\Veky\Post.css")));
            body.AppendLine("</style>");

            foreach (Post post in userState.PostsToSend)
            {
                body.AppendLine(getPostHTML(post));
                body.AppendLine("<br/>");
            }

            body.Append("</body><html>");

            return body.ToString();
        }

        private string getPostHTML(Post post)
        {
            StringBuilder html = new StringBuilder();

            html.Append(post.Date.Html);
            html.Append(post.Title.Html);
            html.Append(post.AllText.Html);

            return html.ToString();
        }

        public override void Start()
        {
            base.Start("http://veky86.bloger.index.hr/default.aspx");
        }
    }

    public class Post : IEquatable<Post>
    {
        public DateTime CreatedOn { get; set; }
        public PostData Date { get; set; }
        public PostData Title { get; set; }
        public List<PostData> Texts { get; set; }
        public PostData AllText { get; set; }

        public Post()
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

        public bool Equals(Post other)
        {
            return Common.Common.FormatTextForEqualityComparison(this.GetCumulativeText()) == Common.Common.FormatTextForEqualityComparison(other.GetCumulativeText());
        }
    }

    public class PostData
    {
        public string Text { get; set; }
        public string Html { get; set; }
    }
}