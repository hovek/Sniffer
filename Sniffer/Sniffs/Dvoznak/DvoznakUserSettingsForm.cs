using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sniffer.Settings;
using Sniffer.Common;
using System.Text;

namespace Sniffer.Sniffs.Dvoznak
{
	public partial class DvoznakUserSettingsForm : Form
	{
		private DvoznakUserSettings originalUserSettings;
		private DvoznakUserSettings newUserSettings = new DvoznakUserSettings();
		private MailSettings newMailSettings = new MailSettings();
		private SmtpSettings newSmtpSettings = new SmtpSettings();

		public DvoznakUserSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(UserSettings userSettings)
		{
			DvoznakUserSettingsForm frm = new DvoznakUserSettingsForm();
			frm.originalUserSettings = (DvoznakUserSettings)userSettings;
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
			ReflectionCopy<DvoznakUserSettings>.Copy(originalUserSettings, false).Paste(newUserSettings);
			ReflectionCopy<MailSettings>.Copy(originalUserSettings.MailSettings, false).Paste(newMailSettings);
			ReflectionCopy<SmtpSettings>.Copy(originalUserSettings.SmtpSettings, false).Paste(newSmtpSettings);
			mailSettingsControl.MailSettingsObject = newMailSettings;
			smtpSettingsControl.SmtpSettingsObject = newSmtpSettings;

			cbSendCSVFile.DataBindings.Add("Checked", newUserSettings, "SendCSVFile");
			cbCumulativeCSVFile.DataBindings.Add("Checked", newUserSettings, "CumulateCSVFile");
			dtpCSVFrom.DataBindings.Add("Value", newUserSettings, "CumulateCSVFileFrom");
			dtpCSVTo.DataBindings.Add("Value", newUserSettings, "CumulateCSVFileTo");
			dtpValidFrom.DataBindings.Add("Value", newUserSettings, "ValidFrom");
			dtpValidTo.DataBindings.Add("Value", newUserSettings, "ValidTo");
			cbSendNew.DataBindings.Add("Checked", newUserSettings, "SendNew");
			cbSendCompleted.DataBindings.Add("Checked", newUserSettings, "SendCompleted");
			cbBadOrGoodFilter.DataBindings.Add("Checked", newUserSettings, "IsBadOrGoodFilterOn");
			tbBadOrGoodFilter.DataBindings.Add("Text", newUserSettings, "BadOrGoodFilterSQL");
			tbBadOrGoodFilterParameters.DataBindings.Add("Text", newUserSettings, "BadOrGoodFilterParameters");
		}

		private void updateSettings()
		{
			ReflectionCopy<DvoznakUserSettings>.Copy(newUserSettings, false).Paste(originalUserSettings);
			ReflectionCopy<MailSettings>.Copy(newMailSettings, false).Paste(originalUserSettings.MailSettings);
			ReflectionCopy<SmtpSettings>.Copy(newSmtpSettings, false).Paste(originalUserSettings.SmtpSettings);

			List<ProcedureParameter> parameters = Dvoznak.GetBadOrGoodFilterParameters(originalUserSettings.BadOrGoodFilterParameters);
			StringBuilder spChecksum = new StringBuilder();
			foreach (ProcedureParameter param in parameters)
			{
				spChecksum.Append(param.Name);
				spChecksum.Append(param.DataType);
			}
			spChecksum.Append(originalUserSettings.BadOrGoodFilterSQL.Trim());
			originalUserSettings.BadOrGoodFilterSPName = string.Concat("spBadOrGoodFilter_", Common.Common.GetMD5Hash(spChecksum.ToString()));
		}
	}
}
