using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sniffer.Settings;
using Sniffer.Common;
using System.Text;

namespace Sniffer.Sniffs.Njuskalo
{
	public partial class NjuskaloUserSettingsForm : Form
	{
		private NjuskaloUserSettings originalUserSettings;
        private NjuskaloUserSettings newUserSettings = new NjuskaloUserSettings();
		private MailSettings newMailSettings = new MailSettings();
		private SmtpSettings newSmtpSettings = new SmtpSettings();

        public NjuskaloUserSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(UserSettings userSettings)
		{
            NjuskaloUserSettingsForm frm = new NjuskaloUserSettingsForm();
            frm.originalUserSettings = (NjuskaloUserSettings)userSettings;
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
            ReflectionCopy<NjuskaloUserSettings>.Copy(originalUserSettings, false).Paste(newUserSettings);
			ReflectionCopy<MailSettings>.Copy(originalUserSettings.MailSettings, false).Paste(newMailSettings);
			ReflectionCopy<SmtpSettings>.Copy(originalUserSettings.SmtpSettings, false).Paste(newSmtpSettings);
			mailSettingsControl.MailSettingsObject = newMailSettings;
			smtpSettingsControl.SmtpSettingsObject = newSmtpSettings;

			dtpValidFrom.DataBindings.Add("Value", newUserSettings, "ValidFrom");
			dtpValidTo.DataBindings.Add("Value", newUserSettings, "ValidTo");
            txtUrl.DataBindings.Add("Text", newUserSettings, "Url");
            txtHeaderNotText.DataBindings.Add("Text", newUserSettings, "TitleNotText");
            txtDescriptionNotText.DataBindings.Add("Text", newUserSettings, "DescriptionNotText");
        }

		private void updateSettings()
		{
            ReflectionCopy<NjuskaloUserSettings>.Copy(newUserSettings, false).Paste(originalUserSettings);
			ReflectionCopy<MailSettings>.Copy(newMailSettings, false).Paste(originalUserSettings.MailSettings);
			ReflectionCopy<SmtpSettings>.Copy(newSmtpSettings, false).Paste(originalUserSettings.SmtpSettings);
		}
	}
}
