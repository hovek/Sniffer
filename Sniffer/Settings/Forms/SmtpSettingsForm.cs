using System.Windows.Forms;
using Sniffer.Common;

namespace Sniffer.Settings.Forms
{
	public partial class SmtpSettingsForm : Form
	{
		private SmtpSettings originalSmtpSettings;
		private SmtpSettings smtpSettings = new SmtpSettings();

		public SmtpSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(SmtpSettings smtpSettings)
		{
			SmtpSettingsForm frm = new SmtpSettingsForm();
			frm.originalSmtpSettings = smtpSettings;
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
			ReflectionCopy<SmtpSettings>.Copy(originalSmtpSettings, false).Paste(smtpSettings);
			smtpSettingsControl.SmtpSettingsObject = smtpSettings;
		}

		private void updateSettings()
		{
			ReflectionCopy<SmtpSettings>.Copy(smtpSettings, false).Paste(originalSmtpSettings);
		}
	}
}
