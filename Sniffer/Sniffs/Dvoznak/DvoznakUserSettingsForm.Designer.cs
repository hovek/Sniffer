namespace Sniffer.Sniffs.Dvoznak
{
	partial class DvoznakUserSettingsForm
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
			this.cbSendCSVFile = new System.Windows.Forms.CheckBox();
			this.cbCumulativeCSVFile = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dtpValidTo = new System.Windows.Forms.DateTimePicker();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dtpCSVTo = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.dtpCSVFrom = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.dtpValidFrom = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.cbSendCompleted = new System.Windows.Forms.CheckBox();
			this.cbSendNew = new System.Windows.Forms.CheckBox();
			this.cbBadOrGoodFilter = new System.Windows.Forms.CheckBox();
			this.tbBadOrGoodFilter = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbBadOrGoodFilterParameters = new System.Windows.Forms.TextBox();
			this.smtpSettingsControl = new Sniffer.Settings.Controls.SmtpSettings();
			this.mailSettingsControl = new Sniffer.Settings.Controls.MailSettings();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(656, 380);
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
			this.btnOk.Location = new System.Drawing.Point(575, 380);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// cbSendCSVFile
			// 
			this.cbSendCSVFile.AutoSize = true;
			this.cbSendCSVFile.Location = new System.Drawing.Point(13, 165);
			this.cbSendCSVFile.Name = "cbSendCSVFile";
			this.cbSendCSVFile.Size = new System.Drawing.Size(91, 17);
			this.cbSendCSVFile.TabIndex = 2;
			this.cbSendCSVFile.Text = "Send CSV file";
			this.cbSendCSVFile.UseVisualStyleBackColor = true;
			// 
			// cbCumulativeCSVFile
			// 
			this.cbCumulativeCSVFile.AutoSize = true;
			this.cbCumulativeCSVFile.Location = new System.Drawing.Point(110, 144);
			this.cbCumulativeCSVFile.Name = "cbCumulativeCSVFile";
			this.cbCumulativeCSVFile.Size = new System.Drawing.Size(110, 17);
			this.cbCumulativeCSVFile.TabIndex = 5;
			this.cbCumulativeCSVFile.Text = "Cumulate CSV file";
			this.cbCumulativeCSVFile.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(429, 173);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Valid to";
			// 
			// dtpValidTo
			// 
			this.dtpValidTo.CustomFormat = "dd.MM.yyyy H:mm:ss";
			this.dtpValidTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpValidTo.Location = new System.Drawing.Point(485, 171);
			this.dtpValidTo.Name = "dtpValidTo";
			this.dtpValidTo.Size = new System.Drawing.Size(144, 20);
			this.dtpValidTo.TabIndex = 7;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dtpCSVTo);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.dtpCSVFrom);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(100, 147);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(216, 48);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// dtpCSVTo
			// 
			this.dtpCSVTo.CustomFormat = "H:mm:ss";
			this.dtpCSVTo.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtpCSVTo.Location = new System.Drawing.Point(138, 19);
			this.dtpCSVTo.Name = "dtpCSVTo";
			this.dtpCSVTo.Size = new System.Drawing.Size(68, 20);
			this.dtpCSVTo.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(116, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "to";
			// 
			// dtpCSVFrom
			// 
			this.dtpCSVFrom.Cursor = System.Windows.Forms.Cursors.Default;
			this.dtpCSVFrom.CustomFormat = "H:mm:ss";
			this.dtpCSVFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtpCSVFrom.Location = new System.Drawing.Point(42, 19);
			this.dtpCSVFrom.Name = "dtpCSVFrom";
			this.dtpCSVFrom.Size = new System.Drawing.Size(68, 20);
			this.dtpCSVFrom.TabIndex = 8;
			this.dtpCSVFrom.Value = new System.DateTime(2012, 3, 10, 15, 27, 0, 0);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "from";
			// 
			// dtpValidFrom
			// 
			this.dtpValidFrom.CustomFormat = "dd.MM.yyyy H:mm:ss";
			this.dtpValidFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpValidFrom.Location = new System.Drawing.Point(485, 148);
			this.dtpValidFrom.Name = "dtpValidFrom";
			this.dtpValidFrom.Size = new System.Drawing.Size(144, 20);
			this.dtpValidFrom.TabIndex = 19;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(429, 150);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 13);
			this.label4.TabIndex = 18;
			this.label4.Text = "Valid from";
			// 
			// cbSendCompleted
			// 
			this.cbSendCompleted.AutoSize = true;
			this.cbSendCompleted.Location = new System.Drawing.Point(322, 172);
			this.cbSendCompleted.Name = "cbSendCompleted";
			this.cbSendCompleted.Size = new System.Drawing.Size(103, 17);
			this.cbSendCompleted.TabIndex = 20;
			this.cbSendCompleted.Text = "Send completed";
			this.cbSendCompleted.UseVisualStyleBackColor = true;
			// 
			// cbSendNew
			// 
			this.cbSendNew.AutoSize = true;
			this.cbSendNew.Location = new System.Drawing.Point(322, 149);
			this.cbSendNew.Name = "cbSendNew";
			this.cbSendNew.Size = new System.Drawing.Size(74, 17);
			this.cbSendNew.TabIndex = 21;
			this.cbSendNew.Text = "Send new";
			this.cbSendNew.UseVisualStyleBackColor = true;
			// 
			// cbBadOrGoodFilter
			// 
			this.cbBadOrGoodFilter.AutoSize = true;
			this.cbBadOrGoodFilter.Location = new System.Drawing.Point(6, 0);
			this.cbBadOrGoodFilter.Name = "cbBadOrGoodFilter";
			this.cbBadOrGoodFilter.Size = new System.Drawing.Size(15, 14);
			this.cbBadOrGoodFilter.TabIndex = 22;
			this.cbBadOrGoodFilter.UseVisualStyleBackColor = true;
			// 
			// tbBadOrGoodFilter
			// 
			this.tbBadOrGoodFilter.Location = new System.Drawing.Point(3, 71);
			this.tbBadOrGoodFilter.Multiline = true;
			this.tbBadOrGoodFilter.Name = "tbBadOrGoodFilter";
			this.tbBadOrGoodFilter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbBadOrGoodFilter.Size = new System.Drawing.Size(701, 132);
			this.tbBadOrGoodFilter.TabIndex = 23;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.panel1);
			this.groupBox2.Controls.Add(this.cbBadOrGoodFilter);
			this.groupBox2.Location = new System.Drawing.Point(1, 201);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(730, 173);
			this.groupBox2.TabIndex = 25;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "     Bad or good filter";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.tbBadOrGoodFilterParameters);
			this.panel1.Controls.Add(this.tbBadOrGoodFilter);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(724, 154);
			this.panel1.TabIndex = 23;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 55);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(28, 13);
			this.label6.TabIndex = 26;
			this.label6.Text = "SQL";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 13);
			this.label5.TabIndex = 25;
			this.label5.Text = "Parameters";
			// 
			// tbBadOrGoodFilterParameters
			// 
			this.tbBadOrGoodFilterParameters.Location = new System.Drawing.Point(3, 16);
			this.tbBadOrGoodFilterParameters.Multiline = true;
			this.tbBadOrGoodFilterParameters.Name = "tbBadOrGoodFilterParameters";
			this.tbBadOrGoodFilterParameters.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbBadOrGoodFilterParameters.Size = new System.Drawing.Size(701, 32);
			this.tbBadOrGoodFilterParameters.TabIndex = 24;
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
			// DvoznakUserSettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 415);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cbSendNew);
			this.Controls.Add(this.cbSendCompleted);
			this.Controls.Add(this.dtpValidFrom);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbCumulativeCSVFile);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.dtpValidTo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.smtpSettingsControl);
			this.Controls.Add(this.mailSettingsControl);
			this.Controls.Add(this.cbSendCSVFile);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DvoznakUserSettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dvoznak user settings";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.CheckBox cbSendCSVFile;
		private Settings.Controls.MailSettings mailSettingsControl;
		private Settings.Controls.SmtpSettings smtpSettingsControl;
		private System.Windows.Forms.CheckBox cbCumulativeCSVFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtpValidTo;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DateTimePicker dtpCSVTo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker dtpCSVFrom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DateTimePicker dtpValidFrom;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox cbSendCompleted;
		private System.Windows.Forms.CheckBox cbSendNew;
		private System.Windows.Forms.CheckBox cbBadOrGoodFilter;
		private System.Windows.Forms.TextBox tbBadOrGoodFilter;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbBadOrGoodFilterParameters;

	}
}