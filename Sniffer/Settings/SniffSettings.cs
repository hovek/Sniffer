using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Settings
{
	[Serializable]
	public class SniffSettings
	{
		public SmtpSettings SmtpSettings { get; set; }
		public MailSettings MailSettings { get; set; }
		public List<UserSettings> UserSettings { get; set; }

		public SniffSettings()
		{
			SmtpSettings = new global::Sniffer.Settings.SmtpSettings();
			MailSettings = new global::Sniffer.Settings.MailSettings();
			UserSettings = new List<UserSettings>();
		}

		public MailSettings GetPredecessor()
		{
			return MailSettings;
		}
	}
}
