namespace WinChangeMonitor
{
    partial class WinChangeMonitorForm
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
            this.gbWhatToTrack = new System.Windows.Forms.GroupBox();
            this.tcWhatToTrack = new System.Windows.Forms.TabControl();
            this.tpFolders = new System.Windows.Forms.TabPage();
            this.dgvFoldersToTrack = new System.Windows.Forms.DataGridView();
            this.tbcolFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbcolIncludeSubdirs = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tpRegistry = new System.Windows.Forms.TabPage();
            this.bPreInstall = new System.Windows.Forms.Button();
            this.bwPreInstall = new System.ComponentModel.BackgroundWorker();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.bPostInstall = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.gbWhatToTrack.SuspendLayout();
            this.tcWhatToTrack.SuspendLayout();
            this.tpFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFoldersToTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // gbWhatToTrack
            // 
            this.gbWhatToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWhatToTrack.Controls.Add(this.tcWhatToTrack);
            this.gbWhatToTrack.Location = new System.Drawing.Point(12, 12);
            this.gbWhatToTrack.Name = "gbWhatToTrack";
            this.gbWhatToTrack.Size = new System.Drawing.Size(776, 188);
            this.gbWhatToTrack.TabIndex = 0;
            this.gbWhatToTrack.TabStop = false;
            this.gbWhatToTrack.Text = "What To Track";
            // 
            // tcWhatToTrack
            // 
            this.tcWhatToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcWhatToTrack.Controls.Add(this.tpFolders);
            this.tcWhatToTrack.Controls.Add(this.tpRegistry);
            this.tcWhatToTrack.Location = new System.Drawing.Point(6, 19);
            this.tcWhatToTrack.Name = "tcWhatToTrack";
            this.tcWhatToTrack.SelectedIndex = 0;
            this.tcWhatToTrack.Size = new System.Drawing.Size(764, 163);
            this.tcWhatToTrack.TabIndex = 0;
            // 
            // tpFolders
            // 
            this.tpFolders.Controls.Add(this.dgvFoldersToTrack);
            this.tpFolders.Location = new System.Drawing.Point(4, 22);
            this.tpFolders.Name = "tpFolders";
            this.tpFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tpFolders.Size = new System.Drawing.Size(756, 137);
            this.tpFolders.TabIndex = 0;
            this.tpFolders.Text = "Folders";
            this.tpFolders.UseVisualStyleBackColor = true;
            // 
            // dgvFoldersToTrack
            // 
            this.dgvFoldersToTrack.AllowUserToAddRows = false;
            this.dgvFoldersToTrack.AllowUserToDeleteRows = false;
            this.dgvFoldersToTrack.AllowUserToResizeColumns = false;
            this.dgvFoldersToTrack.AllowUserToResizeRows = false;
            this.dgvFoldersToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFoldersToTrack.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFoldersToTrack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFoldersToTrack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tbcolFolder,
            this.cbcolIncludeSubdirs});
            this.dgvFoldersToTrack.Location = new System.Drawing.Point(6, 6);
            this.dgvFoldersToTrack.Name = "dgvFoldersToTrack";
            this.dgvFoldersToTrack.RowHeadersVisible = false;
            this.dgvFoldersToTrack.Size = new System.Drawing.Size(744, 125);
            this.dgvFoldersToTrack.TabIndex = 0;
            // 
            // tbcolFolder
            // 
            this.tbcolFolder.DataPropertyName = "Folder";
            this.tbcolFolder.HeaderText = "Folder";
            this.tbcolFolder.Name = "tbcolFolder";
            this.tbcolFolder.ReadOnly = true;
            // 
            // cbcolIncludeSubdirs
            // 
            this.cbcolIncludeSubdirs.DataPropertyName = "IncludeSubDirectories";
            this.cbcolIncludeSubdirs.HeaderText = "Include Sub-Directories?";
            this.cbcolIncludeSubdirs.Name = "cbcolIncludeSubdirs";
            // 
            // tpRegistry
            // 
            this.tpRegistry.BackColor = System.Drawing.Color.Red;
            this.tpRegistry.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tpRegistry.Location = new System.Drawing.Point(4, 22);
            this.tpRegistry.Name = "tpRegistry";
            this.tpRegistry.Padding = new System.Windows.Forms.Padding(3);
            this.tpRegistry.Size = new System.Drawing.Size(756, 137);
            this.tpRegistry.TabIndex = 1;
            this.tpRegistry.Text = "Registry";
            // 
            // bPreInstall
            // 
            this.bPreInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bPreInstall.Location = new System.Drawing.Point(12, 206);
            this.bPreInstall.Name = "bPreInstall";
            this.bPreInstall.Size = new System.Drawing.Size(102, 23);
            this.bPreInstall.TabIndex = 1;
            this.bPreInstall.Text = "Perform Pre-Install";
            this.bPreInstall.UseVisualStyleBackColor = true;
            this.bPreInstall.Click += new System.EventHandler(this.bPreInstall_Click);
            // 
            // bwPreInstall
            // 
            this.bwPreInstall.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwPreInstall_DoWork);
            this.bwPreInstall.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwPreInstall_RunWorkerCompleted);
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Location = new System.Drawing.Point(12, 235);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(776, 203);
            this.tbOutput.TabIndex = 2;
            // 
            // bPostInstall
            // 
            this.bPostInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bPostInstall.Enabled = false;
            this.bPostInstall.Location = new System.Drawing.Point(120, 206);
            this.bPostInstall.Name = "bPostInstall";
            this.bPostInstall.Size = new System.Drawing.Size(107, 23);
            this.bPostInstall.TabIndex = 3;
            this.bPostInstall.Text = "Perform Post-Install";
            this.bPostInstall.UseVisualStyleBackColor = true;
            this.bPostInstall.Click += new System.EventHandler(this.bPostInstall_Click);
            // 
            // lStatus
            // 
            this.lStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lStatus.Location = new System.Drawing.Point(233, 211);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(555, 13);
            this.lStatus.TabIndex = 0;
            // 
            // WinChangeMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bPostInstall);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.bPreInstall);
            this.Controls.Add(this.gbWhatToTrack);
            this.Name = "WinChangeMonitorForm";
            this.ShowIcon = false;
            this.Text = "WinChangeMonitor - BU MET CS 673/473 Group #3 - Fall 2025";
            this.Load += new System.EventHandler(this.WinChangeMonitorForm_Load);
            this.gbWhatToTrack.ResumeLayout(false);
            this.tcWhatToTrack.ResumeLayout(false);
            this.tpFolders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFoldersToTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWhatToTrack;
        private System.Windows.Forms.TabControl tcWhatToTrack;
        private System.Windows.Forms.TabPage tpFolders;
        private System.Windows.Forms.DataGridView dgvFoldersToTrack;
        private System.Windows.Forms.Button bPreInstall;
        private System.ComponentModel.BackgroundWorker bwPreInstall;
        private System.Windows.Forms.DataGridViewTextBoxColumn tbcolFolder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cbcolIncludeSubdirs;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button bPostInstall;
        private System.Windows.Forms.TabPage tpRegistry;
        private System.Windows.Forms.Label lStatus;
    }
}

