using System.Windows.Forms;
using Sniffer.Common;

namespace Sniffer.Settings.Forms
{
	public partial class MailSettingsForm : Form
	{
		private MailSettings originalMailSettings;
		private MailSettings mailSettings = new MailSettings();

		public MailSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(MailSettings mailSettings)
		{
			MailSettingsForm frm = new MailSettingsForm();
			frm.originalMailSettings = mailSettings;
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
			ReflectionCopy<MailSettings>.Copy(originalMailSettings, false).Paste(mailSettings);
			mailSettingsControl.MailSettingsObject = mailSettings;
		}

		private void updateSettings()
		{
			ReflectionCopy<MailSettings>.Copy(mailSettings, false).Paste(originalMailSettings);
		}
	}
}
