using System.Windows.Forms;
using SO = Sniffer.Settings;

namespace Sniffer.Settings.Controls
{
	public partial class MailSettings : UserControl
	{
		private SO.MailSettings mailSettings;
		public SO.MailSettings MailSettingsObject
		{
			get
			{
				return mailSettings;
			}
			set
			{
				mailSettings = value;
				dataBind();
			}
		}

		public MailSettings()
		{
			InitializeComponent();
		}

		private void dataBind()
		{
			tbFrom.DataBindings.Clear();
			tbTo.DataBindings.Clear();
			tbCc.DataBindings.Clear();
			tbBcc.DataBindings.Clear();

			if (MailSettingsObject != null)
			{
				tbFrom.DataBindings.Add("Text", MailSettingsObject, "From");
				tbTo.DataBindings.Add("Text", MailSettingsObject, "To");
				tbCc.DataBindings.Add("Text", MailSettingsObject, "Cc");
				tbBcc.DataBindings.Add("Text", MailSettingsObject, "Bcc");
			}
		}
	}
}
