﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace Sniffer.Sniffs.SmicerBlog
{
	//public class SmicerBlog : Sniffer
	//{
	//    private const string url = "http://smicer.bloger.hr/";
	//    string checksum = null;
	//    public string MailingList;
	//    public string MailFrom;
	//    bool inTimer = false;

	//    public SmicerBlog(WebBrowserWrapper webBrowser)
	//        : base(webBrowser)
	//    {
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
	//            if (element.GetAttribute("classname") == "blogtext")
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
	//                Common.Common.SendMail(MailFrom, MailingList, "Šmicer je promijenio blog!!!", @"<a href=""" + url + @""">Šmicer je promijenio blog!<a/>");
	//            }
	//            catch (Exception ex)
	//            {
	//                error = ex.Message;
	//                new Thread(() =>
	//                {
	//                    MessageBox.Show("Šmicer je promijenio blog!!! - " + DateTime.Now.ToString() + Environment.NewLine + error);
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
