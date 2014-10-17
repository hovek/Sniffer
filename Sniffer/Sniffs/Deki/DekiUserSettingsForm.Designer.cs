namespace Sniffer.Sniffs.Deki
{
	partial class DekiUserSettingsForm
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpValidTo = new System.Windows.Forms.DateTimePicker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dtpValidFrom = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.smtpSettingsControl = new Sniffer.Settings.Controls.SmtpSettings();
            this.mailSettingsControl = new Sniffer.Settings.Controls.MailSettings();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(656, 202);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(575, 202);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 166);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Valid to";
            // 
            // dtpValidTo
            // 
            this.dtpValidTo.CustomFormat = "dd.MM.yyyy H:mm:ss";
            this.dtpValidTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpValidTo.Location = new System.Drawing.Point(68, 164);
            this.dtpValidTo.Name = "dtpValidTo";
            this.dtpValidTo.Size = new System.Drawing.Size(144, 20);
            this.dtpValidTo.TabIndex = 7;
            // 
            // dtpValidFrom
            // 
            this.dtpValidFrom.CustomFormat = "dd.MM.yyyy H:mm:ss";
            this.dtpValidFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpValidFrom.Location = new System.Drawing.Point(68, 141);
            this.dtpValidFrom.Name = "dtpValidFrom";
            this.dtpValidFrom.Size = new System.Drawing.Size(144, 20);
            this.dtpValidFrom.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Valid from";
            // 
            // smtpSettingsControl
            // 
            this.smtpSettingsControl.Location = new System.Drawing.Point(12, 12);
            this.smtpSettingsControl.Name = "smtpSettingsControl";
            this.smtpSettingsControl.Size = new System.Drawing.Size(357, 126);
            this.smtpSettingsControl.SmtpSettingsObject = null;
            this.smtpSettingsControl.TabIndex = 0;
            // 
            // mailSettingsControl
            // 
            this.mailSettingsControl.Location = new System.Drawing.Point(375, 12);
            this.mailSettingsControl.MailSettingsObject = null;
            this.mailSettingsControl.Name = "mailSettingsControl";
            this.mailSettingsControl.Size = new System.Drawing.Size(357, 126);
            this.mailSettingsControl.TabIndex = 1;
            // 
            // DekiUserSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 237);
            this.Controls.Add(this.dtpValidFrom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpValidTo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.smtpSettingsControl);
            this.Controls.Add(this.mailSettingsControl);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DekiUserSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deki user settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
		private Settings.Controls.MailSettings mailSettingsControl;
        private Settings.Controls.SmtpSettings smtpSettingsControl;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpValidTo;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DateTimePicker dtpValidFrom;
        private System.Windows.Forms.Label label4;

	}
}