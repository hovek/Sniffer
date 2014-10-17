namespace Sniffer.Settings.Forms
{
	partial class UsersSettingsForm
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
			this.dgvUsers = new System.Windows.Forms.DataGridView();
			this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Settings = new System.Windows.Forms.DataGridViewButtonColumn();
			this.userSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.userSettingsBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvUsers
			// 
			this.dgvUsers.AllowUserToOrderColumns = true;
			this.dgvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvUsers.AutoGenerateColumns = false;
			this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvUsers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.Key,
            this.activeDataGridViewCheckBoxColumn,
            this.Settings});
			this.dgvUsers.DataSource = this.userSettingsBindingSource;
			this.dgvUsers.Location = new System.Drawing.Point(12, 12);
			this.dgvUsers.MultiSelect = false;
			this.dgvUsers.Name = "dgvUsers";
			this.dgvUsers.Size = new System.Drawing.Size(607, 403);
			this.dgvUsers.TabIndex = 0;
			this.dgvUsers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUsers_CellContentClick);
			this.dgvUsers.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvUsers_CellValueNeeded);
			this.dgvUsers.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvUsers_RowValidating);
			// 
			// nameDataGridViewTextBoxColumn
			// 
			this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
			this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
			this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
			this.nameDataGridViewTextBoxColumn.Width = 250;
			// 
			// Key
			// 
			this.Key.DataPropertyName = "Key";
			this.Key.HeaderText = "Key";
			this.Key.Name = "Key";
			// 
			// activeDataGridViewCheckBoxColumn
			// 
			this.activeDataGridViewCheckBoxColumn.DataPropertyName = "Active";
			this.activeDataGridViewCheckBoxColumn.HeaderText = "Active";
			this.activeDataGridViewCheckBoxColumn.Name = "activeDataGridViewCheckBoxColumn";
			this.activeDataGridViewCheckBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// Settings
			// 
			this.Settings.HeaderText = "Settings";
			this.Settings.Name = "Settings";
			this.Settings.Text = "Settings";
			this.Settings.UseColumnTextForButtonValue = true;
			this.Settings.Width = 70;
			// 
			// userSettingsBindingSource
			// 
			this.userSettingsBindingSource.DataSource = typeof(Sniffer.Settings.UserSettings);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(544, 421);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(463, 421);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// UsersSettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(631, 456);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.dgvUsers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UsersSettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Users settings";
			((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.userSettingsBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvUsers;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.BindingSource userSettingsBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Key;
		private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn;
		private System.Windows.Forms.DataGridViewButtonColumn Settings;

	}
}