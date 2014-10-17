namespace Sniffer.Settings.Controls
{
	partial class SmtpSettings
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbSmtpUN = new System.Windows.Forms.TextBox();
			this.cbUseCredentials = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbSmtpPsw = new System.Windows.Forms.TextBox();
			this.cbEnableSSL = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tbSmtpHost = new System.Windows.Forms.TextBox();
			this.tbSmtpPort = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbSmtpUN
			// 
			this.tbSmtpUN.Location = new System.Drawing.Point(233, 19);
			this.tbSmtpUN.Name = "tbSmtpUN";
			this.tbSmtpUN.Size = new System.Drawing.Size(114, 20);
			this.tbSmtpUN.TabIndex = 2;
			// 
			// cbUseCredentials
			// 
			this.cbUseCredentials.AutoSize = true;
			this.cbUseCredentials.Checked = true;
			this.cbUseCredentials.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.cbUseCredentials.Location = new System.Drawing.Point(10, 80);
			this.cbUseCredentials.Name = "cbUseCredentials";
			this.cbUseCredentials.Size = new System.Drawing.Size(99, 17);
			this.cbUseCredentials.TabIndex = 4;
			this.cbUseCredentials.Text = "Use credentials";
			this.cbUseCredentials.ThreeState = true;
			this.cbUseCredentials.UseVisualStyleBackColor = true;
			this.cbUseCredentials.CheckStateChanged += new System.EventHandler(this.cbUseCredentials_CheckStateChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(169, 22);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 44;
			this.label4.Text = "User name";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(169, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 13);
			this.label5.TabIndex = 46;
			this.label5.Text = "Password";
			// 
			// tbSmtpPsw
			// 
			this.tbSmtpPsw.Location = new System.Drawing.Point(233, 45);
			this.tbSmtpPsw.Name = "tbSmtpPsw";
			this.tbSmtpPsw.PasswordChar = '*';
			this.tbSmtpPsw.Size = new System.Drawing.Size(114, 20);
			this.tbSmtpPsw.TabIndex = 3;
			// 
			// cbEnableSSL
			// 
			this.cbEnableSSL.AutoSize = true;
			this.cbEnableSSL.Checked = true;
			this.cbEnableSSL.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.cbEnableSSL.Location = new System.Drawing.Point(10, 103);
			this.cbEnableSSL.Name = "cbEnableSSL";
			this.cbEnableSSL.Size = new System.Drawing.Size(82, 17);
			this.cbEnableSSL.TabIndex = 5;
			this.cbEnableSSL.Text = "Enable SSL";
			this.cbEnableSSL.ThreeState = true;
			this.cbEnableSSL.UseVisualStyleBackColor = true;
			this.cbEnableSSL.CheckStateChanged += new System.EventHandler(this.cbEnableSSL_CheckStateChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 42;
			this.label3.Text = "Host";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(26, 13);
			this.label7.TabIndex = 48;
			this.label7.Text = "Port";
			// 
			// tbSmtpHost
			// 
			this.tbSmtpHost.Location = new System.Drawing.Point(42, 19);
			this.tbSmtpHost.Name = "tbSmtpHost";
			this.tbSmtpHost.Size = new System.Drawing.Size(114, 20);
			this.tbSmtpHost.TabIndex = 0;
			// 
			// tbSmtpPort
			// 
			this.tbSmtpPort.Location = new System.Drawing.Point(42, 45);
			this.tbSmtpPort.Name = "tbSmtpPort";
			this.tbSmtpPort.Size = new System.Drawing.Size(114, 20);
			this.tbSmtpPort.TabIndex = 1;
			this.tbSmtpPort.TextChanged += new System.EventHandler(this.tbSmtpPort_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.tbSmtpUN);
			this.groupBox1.Controls.Add(this.tbSmtpPort);
			this.groupBox1.Controls.Add(this.cbUseCredentials);
			this.groupBox1.Controls.Add(this.tbSmtpHost);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.cbEnableSSL);
			this.groupBox1.Controls.Add(this.tbSmtpPsw);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(357, 126);
			this.groupBox1.TabIndex = 51;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Smtp settings";
			// 
			// SmtpSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "SmtpSettings";
			this.Size = new System.Drawing.Size(357, 126);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox tbSmtpUN;
		private System.Windows.Forms.CheckBox cbUseCredentials;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbSmtpPsw;
		private System.Windows.Forms.CheckBox cbEnableSSL;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbSmtpHost;
		private System.Windows.Forms.TextBox tbSmtpPort;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
