using SO = Sniffer.Settings;
namespace Sniffer.Settings.Forms
{
	partial class SniffSettingsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mailSettingsControl = new Sniffer.Settings.Controls.MailSettings();
			this.smtpSettingsControl = new Sniffer.Settings.Controls.SmtpSettings();
			this.btnUserSettings = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mailSettingsControl
			// 
			this.mailSettingsControl.Location = new System.Drawing.Point(12, 144);
			this.mailSettingsControl.MailSettingsObject = null;
			this.mailSettingsControl.Name = "mailSettingsControl";
			this.mailSettingsControl.Size = new System.Drawing.Size(357, 126);
			this.mailSettingsControl.TabIndex = 1;
			// 
			// smtpSettingsControl
			// 
			this.smtpSettingsControl.Location = new System.Drawing.Point(12, 12);
			this.smtpSettingsControl.Name = "smtpSettingsControl";
			this.smtpSettingsControl.Size = new System.Drawing.Size(357, 126);
			this.smtpSettingsControl.SmtpSettingsObject = null;
			this.smtpSettingsControl.TabIndex = 0;
			// 
			// btnUserSettings
			// 
			this.btnUserSettings.Location = new System.Drawing.Point(12, 276);
			this.btnUserSettings.Name = "btnUserSettings";
			this.btnUserSettings.Size = new System.Drawing.Size(52, 23);
			this.btnUserSettings.TabIndex = 2;
			this.btnUserSettings.Text = "Users";
			this.btnUserSettings.UseVisualStyleBackColor = true;
			this.btnUserSettings.Click += new System.EventHandler(this.btnUserSettings_Click);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(214, 276);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(295, 276);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// SniffSettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 311);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnUserSettings);
			this.Controls.Add(this.smtpSettingsControl);
			this.Controls.Add(this.mailSettingsControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SniffSettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Settings";
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.MailSettings mailSettingsControl;
		private Controls.SmtpSettings smtpSettingsControl;
		private System.Windows.Forms.Button btnUserSettings;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}