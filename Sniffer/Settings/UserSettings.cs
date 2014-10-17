using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Settings
{
	[Serializable]
	public class UserSettings
	{
		public bool Active { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }
		public SmtpSettings SmtpSettings { get; set; }
		public MailSettings MailSettings { get; set; }

		public UserSettings()
		{
			Key = "";
			SmtpSettings = new SmtpSettings();
			MailSettings = new MailSettings();
		}
	}
}
