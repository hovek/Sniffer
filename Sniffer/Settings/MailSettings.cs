using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Settings
{
	[Serializable]
	public class MailSettings
	{
		public string From { get; set; }
		public string To { get; set; }
		public string Cc { get; set; }
		public string Bcc { get; set; }

		public MailSettings()
		{
		}

		public MailSettings GetMailSettings(MailSettings predecessor)
		{
			if (predecessor == null)
			{
				return this;
			}

			MailSettings mailSettings = new MailSettings();
			mailSettings.From = string.IsNullOrEmpty(From) ? predecessor.From : From;
			mailSettings.To = string.IsNullOrEmpty(To) ? predecessor.To : To;
			mailSettings.Cc = string.IsNullOrEmpty(Cc) ? predecessor.Cc : Cc;
			mailSettings.Bcc = string.IsNullOrEmpty(Bcc) ? predecessor.Bcc : Bcc;

			return mailSettings;
		}
	}
}
