using System;
using System.Windows.Forms;
using Sniffer.Common;
using SO = Sniffer.Settings;
using Sniffer.Sniffs;

namespace Sniffer.Settings.Forms
{
	public partial class SniffSettingsForm : Form
	{
		private SO.SniffSettings snifferSettings;
		private SniffEnum sniff;
		private MailSettings mailSettings = new MailSettings();
		private SmtpSettings smtpSettings = new SmtpSettings();

		public SniffSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(SniffEnum sniff, SO.SniffSettings snifferSettings)
		{
			SniffSettingsForm frm = new SniffSettingsForm();
			frm.snifferSettings = snifferSettings;
			frm.sniff = sniff;
			frm.fillControls();

			DialogResult rez;
			if ((rez = frm.ShowDialog()) == System.Windows.Forms.DialogResult.OK)
			{
				frm.updateSettings();
			}

			return rez;
		}

		private void fillControls()
		{
			ReflectionCopy<MailSettings>.Copy(snifferSettings.MailSettings, false).Paste(mailSettings);
			ReflectionCopy<SmtpSettings>.Copy(snifferSettings.SmtpSettings, false).Paste(smtpSettings);
			mailSettingsControl.MailSettingsObject = mailSettings;
			smtpSettingsControl.SmtpSettingsObject = smtpSettings;
		}

		private void updateSettings()
		{
			ReflectionCopy<MailSettings>.Copy(mailSettings, false).Paste(snifferSettings.MailSettings);
			ReflectionCopy<SmtpSettings>.Copy(smtpSettings, false).Paste(snifferSettings.SmtpSettings);
		}

		private void btnUserSettings_Click(object sender, EventArgs e)
		{
			UsersSettingsForm.Open(sniff, snifferSettings.UserSettings);
		}
	}
}
