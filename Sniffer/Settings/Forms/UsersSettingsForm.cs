using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sniffer.Common;
using Sniffer.Sniffs;

namespace Sniffer.Settings.Forms
{
	public partial class UsersSettingsForm : Form
	{
		private List<UserSettings> originalUserSettings;
		private SniffEnum sniff;
		private UserSettings newUserSettings;
		private TSList<UserSettings> copyOfUserSettings;

		public UsersSettingsForm()
		{
			InitializeComponent();
		}

		public static DialogResult Open(SniffEnum sniff, List<UserSettings> userSettings)
		{
			UsersSettingsForm frm = new UsersSettingsForm();
			frm.originalUserSettings = userSettings;
			frm.sniff = sniff;
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
			copyOfUserSettings = new TSList<UserSettings>();
			copyOfUserSettings.AllowRemove = true;
			copyOfUserSettings.AllowEdit = true;
			copyOfUserSettings.AutoSort = false;
			copyOfUserSettings.GetNew += copyOfUserSettings_AddingNew;
			copyOfUserSettings.OnCancelNew += onCancelNew;
			foreach (UserSettings us in originalUserSettings)
			{
				UserSettings newUs = (UserSettings)ReflectionHelper.CreateNewInstance(SniffMappings.SniffsMappings[sniff].UserSettings);
				ReflectionCopy<UserSettings>.Copy(us, false).Paste(newUs);
				copyOfUserSettings.Add(newUs);
			}
			dgvUsers.DataSource = copyOfUserSettings;
		}

		UserSettings copyOfUserSettings_AddingNew()
		{
			newUserSettings = (UserSettings)ReflectionHelper.CreateNewInstance(SniffMappings.SniffsMappings[sniff].UserSettings);
			return newUserSettings;
		}

		private void updateSettings()
		{
			originalUserSettings.Clear();
			originalUserSettings.AddRange(copyOfUserSettings);
		}

		private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dgvUsers.Columns[e.ColumnIndex].Name == "Settings" && e.RowIndex > -1)
			{
				UserSettings us;
				if (dgvUsers.Rows[e.RowIndex].DataBoundItem == null)
				{
					us = newUserSettings;
				}
				else
				{
					us = (UserSettings)dgvUsers.Rows[e.RowIndex].DataBoundItem;
				}
				object userSettingsForm = ReflectionHelper.CreateNewInstance(SniffMappings.SniffsMappings[sniff].UserSettingsForm);
				SniffMappings.SniffsMappings[sniff].UserSettingsForm.GetMethod("Open", new Type[] { typeof(UserSettings) }).Invoke(userSettingsForm, new object[] { us });
			}
		}

		private void dgvUsers_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
		{
			UserSettings selectedUserSettings;
			try
			{
				selectedUserSettings = (UserSettings)dgvUsers.Rows[e.RowIndex].DataBoundItem;
				if (selectedUserSettings == null)
				{
					return;
				}
			}
			catch
			{
				return;
			}

			int index = 0;
			foreach (UserSettings us in copyOfUserSettings)
			{
				if (index != e.RowIndex && (us.Key ?? "") == (selectedUserSettings.Key ?? "").ToString())
				{
					e.Cancel = true;
					MessageBox.Show("Key must be unique.");
					return;
				}

				index++;
			}
		}

		private void dgvUsers_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (dgvUsers.Columns[e.ColumnIndex].Name == "Settings")
			{
				e.Value = "Settings";
			}
		}

		private void onCancelNew(int index)
		{
			copyOfUserSettings.Remove(newUserSettings);
		}
	}
}
