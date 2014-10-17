namespace Sniffer.Sniffs.Njuskalo
{
    partial class NjuskaloUserSettingsForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtHeaderNotText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescriptionNotText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.smtpSettingsControl = new Sniffer.Settings.Controls.SmtpSettings();
            this.mailSettingsControl = new Sniffer.Settings.Controls.MailSettings();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(656, 233);
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
            this.btnOk.Location = new System.Drawing.Point(575, 233);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Url";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(322, 141);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(409, 20);
            this.txtUrl.TabIndex = 21;
            // 
            // txtHeaderNotText
            // 
            this.txtHeaderNotText.Location = new System.Drawing.Point(322, 167);
            this.txtHeaderNotText.Name = "txtHeaderNotText";
            this.txtHeaderNotText.Size = new System.Drawing.Size(409, 20);
            this.txtHeaderNotText.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Title not text";
            // 
            // txtDescriptionNotText
            // 
            this.txtDescriptionNotText.Location = new System.Drawing.Point(322, 193);
            this.txtDescriptionNotText.Name = "txtDescriptionNotText";
            this.txtDescriptionNotText.Size = new System.Drawing.Size(409, 20);
            this.txtDescriptionNotText.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(218, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Description not text";
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
            // NjuskaloUserSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 268);
            this.Controls.Add(this.txtDescriptionNotText);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtHeaderNotText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.label2);
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
            this.Name = "NjuskaloUserSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Njuškalo user settings";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtHeaderNotText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDescriptionNotText;
        private System.Windows.Forms.Label label5;

	}
}