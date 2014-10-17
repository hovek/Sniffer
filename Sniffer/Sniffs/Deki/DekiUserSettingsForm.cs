using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sniffer.Settings;
using Sniffer.Common;
using System.Text;

namespace Sniffer.Sniffs.Deki
{
	public partial class DekiUserSettingsForm : Form
	{
		private DekiUserSettings originalUserSettings;
        private DekiUserSettings newUserSettings = new DekiUserSettings();
		private MailSettings newMailSettings = new MailSettings();
		private SmtpSettings newSmtpSettings = new SmtpSettings();

		public DekiUserSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(UserSettings userSettings)
		{
			DekiUserSettingsForm frm = new DekiUserSettingsForm();
            frm.originalUserSettings = (DekiUserSettings)userSettings;
			frm.fillControls();

			DialogResult rez;
			if ((rez = frm.ShowDialog()) == DialogResult.OK)
			{
				frm.updateSettings();
			}

			return rez;
		}

		private void fillControls()
		{
            ReflectionCopy<DekiUserSettings>.Copy(originalUserSettings, false).Paste(newUserSettings);
			ReflectionCopy<MailSettings>.Copy(originalUserSettings.MailSettings, false).Paste(newMailSettings);
			ReflectionCopy<SmtpSettings>.Copy(originalUserSettings.SmtpSettings, false).Paste(newSmtpSettings);
			mailSettingsControl.MailSettingsObject = newMailSettings;
			smtpSettingsControl.SmtpSettingsObject = newSmtpSettings;

			dtpValidFrom.DataBindings.Add("Value", newUserSettings, "ValidFrom");
			dtpValidTo.DataBindings.Add("Value", newUserSettings, "ValidTo");
		}

		private void updateSettings()
		{
            ReflectionCopy<DekiUserSettings>.Copy(newUserSettings, false).Paste(originalUserSettings);
			ReflectionCopy<MailSettings>.Copy(newMailSettings, false).Paste(originalUserSettings.MailSettings);
			ReflectionCopy<SmtpSettings>.Copy(newSmtpSettings, false).Paste(originalUserSettings.SmtpSettings);
		}
	}
}
