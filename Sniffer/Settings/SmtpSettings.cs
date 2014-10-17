using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Settings
{
	[Serializable]
	public class SmtpSettings
	{
		public string SmtpHost { get; set; }
		public int? Port { get; set; }
		public bool? EnableSsl { get; set; }
		public bool? UseCredentials { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		public SmtpSettings()
		{
			Port = null;
		}

		public SmtpSettings GetSmtpSettings(SmtpSettings predecessor)
		{
			if (predecessor == null)
			{
				return this;
			}

			SmtpSettings smtpSettings = new SmtpSettings();
			smtpSettings.SmtpHost = string.IsNullOrEmpty(SmtpHost) ? predecessor.SmtpHost : SmtpHost;
			smtpSettings.Port = Port == null ? predecessor.Port : Port;
			smtpSettings.EnableSsl = EnableSsl == null ? predecessor.EnableSsl : EnableSsl;
			smtpSettings.UseCredentials = UseCredentials == null ? predecessor.UseCredentials : UseCredentials;
			smtpSettings.UserName = string.IsNullOrEmpty(UserName) ? predecessor.UserName : UserName;
			smtpSettings.Password = string.IsNullOrEmpty(Password) ? predecessor.Password : Password;

			return smtpSettings;
		}
	}
}
