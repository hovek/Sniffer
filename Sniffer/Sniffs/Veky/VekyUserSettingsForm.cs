using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sniffer.Settings;
using Sniffer.Common;
using System.Text;

namespace Sniffer.Sniffs.Veky
{
	public partial class VekyUserSettingsForm : Form
	{
		private VekyUserSettings originalUserSettings;
        private VekyUserSettings newUserSettings = new VekyUserSettings();
		private MailSettings newMailSettings = new MailSettings();
		private SmtpSettings newSmtpSettings = new SmtpSettings();

		public VekyUserSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(UserSettings userSettings)
		{
			VekyUserSettingsForm frm = new VekyUserSettingsForm();
            frm.originalUserSettings = (VekyUserSettings)userSettings;
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
            ReflectionCopy<VekyUserSettings>.Copy(originalUserSettings, false).Paste(newUserSettings);
			ReflectionCopy<MailSettings>.Copy(originalUserSettings.MailSettings, false).Paste(newMailSettings);
			ReflectionCopy<SmtpSettings>.Copy(originalUserSettings.SmtpSettings, false).Paste(newSmtpSettings);
			mailSettingsControl.MailSettingsObject = newMailSettings;
			smtpSettingsControl.SmtpSettingsObject = newSmtpSettings;

			dtpValidFrom.DataBindings.Add("Value", newUserSettings, "ValidFrom");
			dtpValidTo.DataBindings.Add("Value", newUserSettings, "ValidTo");
		}

		private void updateSettings()
		{
            ReflectionCopy<VekyUserSettings>.Copy(newUserSettings, false).Paste(originalUserSettings);
			ReflectionCopy<MailSettings>.Copy(newMailSettings, false).Paste(originalUserSettings.MailSettings);
			ReflectionCopy<SmtpSettings>.Copy(newSmtpSettings, false).Paste(originalUserSettings.SmtpSettings);
		}
	}
}
