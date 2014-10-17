using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Sniffer.Sniffs.Club180
{
	//public class Club180 : Sniffer
	//{
	//    private const string url = "http://club180.com.hr/";
	//    string checksum = null;
	//    public string MailingList;
	//    public string MailFrom;
	//    bool inTimer = false;

	//    public Club180(WebBrowserWrapper webBrowser)
	//        : base(webBrowser)
	//    {
	//        webBrowser.ScriptErrorsSuppressed = true;
	//        Sniffs.Add("process1", process1);
	//    }

	//    new public WebBrowserWrapper WebBrowser
	//    {
	//        get
	//        {
	//            return (WebBrowserWrapper)base.WebBrowser;
	//        }
	//        set
	//        {
	//            base.WebBrowser = value;
	//        }
	//    }

	//    private bool process1(ref string sniffOverride)
	//    {
	//        HtmlElementCollection divs = WebBrowser.Document.GetElementsByTagName("div");
	//        StringBuilder sb = new StringBuilder();

	//        foreach (HtmlElement element in divs)
	//        {
	//            if (element.GetAttribute("classname") == "art-article")
	//            {
	//                sb.Append(Common.Common.RemoveAllWhiteSpace(Common.Common.RemoveAllScriptTagsFromElement(element)).ToLower());
	//            }
	//        }

	//        string checksum = Common.Common.CalculateMD5Hash(sb.ToString());

	//        if (this.checksum != null && checksum != this.checksum)
	//        {
	//            string error = "";
	//            try
	//            {
	//                Common.Common.SendMail(MailFrom, MailingList, "Club180 je promijenio blog!!!", @"<a href=""" + url + @""">Club180 je promijenio blog!</a>");
	//            }
	//            catch (Exception ex)
	//            {
	//                error = ex.Message;
	//                new Thread(() =>
	//                {
	//                    MessageBox.Show("Club180 je promijenio blog!!! - " + DateTime.Now.ToString() + Environment.NewLine + error);
	//                }).Start();
	//            }
	//        }

	//        this.checksum = checksum;

	//        return true;
	//    }

	//    public override void Start()
	//    {
	//        Start(url);
	//    }
	//}
}
