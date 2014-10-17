using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using Gecko;
using System.Threading;
using Sniffer.Settings;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sniffer.Common
{
    public class Common
    {
        public static void SendMail(string from = "", string to = "", string cc = "", string bcc = "", string subject = "", string body = "",
            bool isBodyHtml = true, List<Attachment> attachments = null, string smtpHost = "", int port = 25, string userName = "",
            string password = "", bool enableSsl = true, bool useCredentials = false, SmtpClient smtpClient = null, bool disposeAfterSend = true)
        {
            MailMessage mm = new MailMessage();
            mm.IsBodyHtml = isBodyHtml;
            mm.From = new MailAddress(from);
            foreach (string address in to.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                mm.To.Add(address);
            }
            foreach (string address in cc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                mm.CC.Add(address);
            }
            foreach (string address in bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                mm.Bcc.Add(address);
            }
            mm.Subject = subject;
            mm.Body = body;

            if (attachments != null)
            {
                foreach (Attachment att in attachments)
                {
                    mm.Attachments.Add(att);
                }
            }

            if (smtpClient == null)
            {
                smtpClient = new SmtpClient();
            }
            smtpClient.Host = smtpHost;
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSsl;
            if (useCredentials)
            {
                NetworkCredential nc = new NetworkCredential(userName, password);
                smtpClient.Credentials = nc;
            }
            smtpClient.Send(mm);
            if (disposeAfterSend)
            {
                smtpClient.Dispose();
            }
        }

        public static string RemoveAllWhiteSpace(string text)
        {
            return text.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
        }

        public static string RemoveAllScriptTagsFromElement(HtmlElement element)
        {
            string elementContent = element.OuterHtml;

            foreach (HtmlElement script in element.GetElementsByTagName("script"))
            {
                elementContent = elementContent.Replace(script.OuterHtml, "");
            }

            return elementContent;
        }

        private static bool getSourceCompleted;
        public static string GetWebPageSource(string url)
        {
            getSourceCompleted = false;

            GeckoWebBrowser browser = new GeckoWebBrowser();
            browser.ProgressChanged += browser_ProgressChanged;
            browser.DocumentCompleted += browser_DocumentCompleted;
            Form form = new Form();
            form.Controls.Add(browser);
            form.Load += delegate { browser.Navigate("view-source:" + url); };
            form.WindowState = FormWindowState.Minimized;
            form.ShowInTaskbar = false;
            form.Show();
            form.Visible = false;

            DateTime end = DateTime.Now.AddSeconds(30);
            while ((!getSourceCompleted || browser.Document.DocumentElement == null) && end > DateTime.Now)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }

            string source = browser.Document.DocumentElement.TextContent;

            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1;
            tmr.Tick += (o, e) =>
            {
                tmr.Enabled = false;
                form.Close();
            };
            tmr.Start();

            return source;
        }

        static void browser_DocumentCompleted(object sender, EventArgs e)
        {
            getSourceCompleted = true;
        }

        static void browser_ProgressChanged(object sender, GeckoProgressEventArgs e)
        {
            if (e.CurrentProgress == e.MaximumProgress)
            {
                getSourceCompleted = true;
            }
        }

        //public static string GetWebPageSource(string url)
        //{
        //    string responseString = "";

        //    HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        //    myHttpWebRequest.Method = "GET";

        //    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

        //    TextReader tr = new StreamReader(myHttpWebResponse.GetResponseStream());
        //    responseString = tr.ReadToEnd();
        //    tr.Close();

        //    return Common.ReplaceHTMLSpecialCharacters(responseString);
        //}

        public static Exception GetMostInnerException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetMostInnerException(ex.InnerException);
            }

            return ex;
        }

        public static string ReplaceHTMLSpecialCharacters(string input)
        {
            return input
                .Replace("&amp;", "?")
                .Replace("&nbsp;", " ");
        }

        public static string ConstructQueryString(NameValueCollection parameters)
        {
            var sb = new StringBuilder();

            foreach (String name in parameters)
                sb.Append(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name]), "&"));

            if (sb.Length > 0)
                return sb.ToString(0, sb.Length - 1);

            return String.Empty;
        }

        public static string GetMD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(Encoding.Default.GetBytes(text));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static object GetValueBySqlDataType(string sqlDataType, string value)
        {
            sqlDataType = sqlDataType.ToLower();
            if (sqlDataType.Contains("varchar"))
            {
                return value;
            }
            else if (sqlDataType.Contains("int"))
            {
                return int.Parse(value.Trim());
            }
            else if (sqlDataType.Contains("decimal"))
            {
                return decimal.Parse(value.Trim(), CultureInfo.InvariantCulture);
            }
            else if (sqlDataType.Contains("bit"))
            {
                return bool.Parse(value.Trim());
            }
            else if (sqlDataType.Contains("datetime"))
            {
                return DateTime.Parse(value.Trim(), CultureInfo.InvariantCulture);
            }

            throw new NotImplementedException();
        }

        public static string FormatTextForEqualityComparison(string text)
        {
            return Regex.Replace(text, @"\s+", "").ToLower();
        }
    }
}
