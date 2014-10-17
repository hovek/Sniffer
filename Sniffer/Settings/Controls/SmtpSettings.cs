using System;
using System.Windows.Forms;
using SO = Sniffer.Settings;

namespace Sniffer.Settings.Controls
{
	public partial class SmtpSettings : UserControl
	{
		private SO.SmtpSettings smtpSettings;
		public SO.SmtpSettings SmtpSettingsObject
		{
			get
			{
				return smtpSettings;
			}
			set
			{
				smtpSettings = value;
				dataBind();
			}
		}

		public SmtpSettings()
		{
			InitializeComponent();
		}

		private void dataBind()
		{
			tbSmtpHost.DataBindings.Clear();
			tbSmtpPsw.DataBindings.Clear();
			tbSmtpUN.DataBindings.Clear();
			cbUseCredentials.DataBindings.Clear();
			cbEnableSSL.DataBindings.Clear();

			if (SmtpSettingsObject != null)
			{
				tbSmtpHost.DataBindings.Add("Text", SmtpSettingsObject, "SmtpHost");
				tbSmtpPsw.DataBindings.Add("Text", SmtpSettingsObject, "Password");
				tbSmtpUN.DataBindings.Add("Text", SmtpSettingsObject, "UserName");
				tbSmtpPort.Text = SmtpSettingsObject.Port == null ? "" : SmtpSettingsObject.Port.ToString();
				switch (SmtpSettingsObject.EnableSsl)
				{
					case null:
						cbEnableSSL.CheckState = CheckState.Indeterminate;
						break;
					case true:
						cbEnableSSL.CheckState = CheckState.Checked;
						break;
					case false:
						cbEnableSSL.CheckState = CheckState.Unchecked;
						break;
				}
				switch (SmtpSettingsObject.UseCredentials)
				{
					case null:
						cbUseCredentials.CheckState = CheckState.Indeterminate;
						break;
					case true:
						cbUseCredentials.CheckState = CheckState.Checked;
						break;
					case false:
						cbUseCredentials.CheckState = CheckState.Unchecked;
						break;
				}
			}
		}

		private void tbSmtpPort_TextChanged(object sender, EventArgs e)
		{
			int port;
			if (int.TryParse(tbSmtpPort.Text, out port))
			{
				SmtpSettingsObject.Port = port;
			}
			else
			{
				SmtpSettingsObject.Port = null;
			}
		}

		private void cbEnableSSL_CheckStateChanged(object sender, EventArgs e)
		{
			switch (cbEnableSSL.CheckState)
			{
				case CheckState.Indeterminate:
					SmtpSettingsObject.EnableSsl = null;
					break;
				case CheckState.Checked:
					SmtpSettingsObject.EnableSsl = true;
					break;
				case CheckState.Unchecked:
					SmtpSettingsObject.EnableSsl = false;
					break;
			}
		}

		private void cbUseCredentials_CheckStateChanged(object sender, EventArgs e)
		{
			switch (cbUseCredentials.CheckState)
			{
				case CheckState.Indeterminate:
					SmtpSettingsObject.UseCredentials = null;
					break;
				case CheckState.Checked:
					SmtpSettingsObject.UseCredentials = true;
					break;
				case CheckState.Unchecked:
					SmtpSettingsObject.UseCredentials = false;
					break;
			}
		}
	}
}
