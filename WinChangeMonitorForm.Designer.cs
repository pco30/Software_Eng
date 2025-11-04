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
            this.components = new System.ComponentModel.Container();
            this.bPreInstall = new System.Windows.Forms.Button();
            this.bwPreInstall = new System.ComponentModel.BackgroundWorker();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.bPostInstall = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.gbRegistryMonitor = new System.Windows.Forms.GroupBox();
            this.bAddKey = new System.Windows.Forms.Button();
            this.bRemoveKey = new System.Windows.Forms.Button();
            this.cbRegistryMonitor = new System.Windows.Forms.CheckBox();
            this.dgvKeysToTrack = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gbServicesMonitor = new System.Windows.Forms.GroupBox();
            this.cbServicesMonitor = new System.Windows.Forms.CheckBox();
            this.ttHelp = new System.Windows.Forms.ToolTip(this.components);
            this.dgvFoldersToTrack = new System.Windows.Forms.DataGridView();
            this.cbFileSystemMonitor = new System.Windows.Forms.CheckBox();
            this.bRemoveFolder = new System.Windows.Forms.Button();
            this.bAddFolder = new System.Windows.Forms.Button();
            this.gbFileSystemMonitor = new System.Windows.Forms.GroupBox();
            this.bStartFresh = new System.Windows.Forms.Button();
            this.bwPostInstall = new System.ComponentModel.BackgroundWorker();
            this.bwLoader = new System.ComponentModel.BackgroundWorker();
            this.tStatus = new System.Windows.Forms.Timer(this.components);
            this.tbcolFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbcolIncludeSubFolders = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gbRegistryMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeysToTrack)).BeginInit();
            this.gbServicesMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFoldersToTrack)).BeginInit();
            this.gbFileSystemMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // bPreInstall
            // 
            this.bPreInstall.Location = new System.Drawing.Point(12, 450);
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
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Location = new System.Drawing.Point(12, 479);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(767, 199);
            this.tbOutput.TabIndex = 2;
            // 
            // bPostInstall
            // 
            this.bPostInstall.Enabled = false;
            this.bPostInstall.Location = new System.Drawing.Point(120, 450);
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
            this.lStatus.Location = new System.Drawing.Point(12, 681);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(546, 13);
            this.lStatus.TabIndex = 0;
            this.lStatus.Text = "Current Folder/Key/Service Displayed Here";
            // 
            // gbRegistryMonitor
            // 
            this.gbRegistryMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRegistryMonitor.Controls.Add(this.bAddKey);
            this.gbRegistryMonitor.Controls.Add(this.bRemoveKey);
            this.gbRegistryMonitor.Controls.Add(this.cbRegistryMonitor);
            this.gbRegistryMonitor.Controls.Add(this.dgvKeysToTrack);
            this.gbRegistryMonitor.Location = new System.Drawing.Point(12, 205);
            this.gbRegistryMonitor.Name = "gbRegistryMonitor";
            this.gbRegistryMonitor.Size = new System.Drawing.Size(767, 187);
            this.gbRegistryMonitor.TabIndex = 6;
            this.gbRegistryMonitor.TabStop = false;
            this.gbRegistryMonitor.Text = "Registry Monitor";
            // 
            // bAddKey
            // 
            this.bAddKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bAddKey.Location = new System.Drawing.Point(641, 158);
            this.bAddKey.Name = "bAddKey";
            this.bAddKey.Size = new System.Drawing.Size(57, 23);
            this.bAddKey.TabIndex = 10;
            this.bAddKey.Text = "Add";
            this.bAddKey.UseVisualStyleBackColor = true;
            this.bAddKey.Click += new System.EventHandler(this.bAddKey_Click);
            // 
            // bRemoveKey
            // 
            this.bRemoveKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRemoveKey.Location = new System.Drawing.Point(704, 158);
            this.bRemoveKey.Name = "bRemoveKey";
            this.bRemoveKey.Size = new System.Drawing.Size(57, 23);
            this.bRemoveKey.TabIndex = 10;
            this.bRemoveKey.Text = "Remove";
            this.bRemoveKey.UseVisualStyleBackColor = true;
            this.bRemoveKey.Click += new System.EventHandler(this.bRemoveKey_Click);
            // 
            // cbRegistryMonitor
            // 
            this.cbRegistryMonitor.AutoSize = true;
            this.cbRegistryMonitor.Location = new System.Drawing.Point(6, 23);
            this.cbRegistryMonitor.Name = "cbRegistryMonitor";
            this.cbRegistryMonitor.Size = new System.Drawing.Size(65, 17);
            this.cbRegistryMonitor.TabIndex = 5;
            this.cbRegistryMonitor.Text = "Enabled";
            this.cbRegistryMonitor.UseVisualStyleBackColor = true;
            this.cbRegistryMonitor.CheckedChanged += new System.EventHandler(this.cbRegistryMonitor_CheckedChanged);
            // 
            // dgvKeysToTrack
            // 
            this.dgvKeysToTrack.AllowUserToAddRows = false;
            this.dgvKeysToTrack.AllowUserToDeleteRows = false;
            this.dgvKeysToTrack.AllowUserToResizeColumns = false;
            this.dgvKeysToTrack.AllowUserToResizeRows = false;
            this.dgvKeysToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvKeysToTrack.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKeysToTrack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKeysToTrack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewCheckBoxColumn1});
            this.dgvKeysToTrack.Location = new System.Drawing.Point(77, 19);
            this.dgvKeysToTrack.Name = "dgvKeysToTrack";
            this.dgvKeysToTrack.RowHeadersVisible = false;
            this.dgvKeysToTrack.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvKeysToTrack.Size = new System.Drawing.Size(684, 133);
            this.dgvKeysToTrack.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Key";
            this.dataGridViewTextBoxColumn1.HeaderText = "Key";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.DataPropertyName = "IncludeSubKeys";
            this.dataGridViewCheckBoxColumn1.HeaderText = "Include Sub-Keys?";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            // 
            // gbServicesMonitor
            // 
            this.gbServicesMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbServicesMonitor.Controls.Add(this.cbServicesMonitor);
            this.gbServicesMonitor.Location = new System.Drawing.Point(12, 398);
            this.gbServicesMonitor.Name = "gbServicesMonitor";
            this.gbServicesMonitor.Size = new System.Drawing.Size(767, 46);
            this.gbServicesMonitor.TabIndex = 7;
            this.gbServicesMonitor.TabStop = false;
            this.gbServicesMonitor.Text = "Services Monitor";
            // 
            // cbServicesMonitor
            // 
            this.cbServicesMonitor.AutoSize = true;
            this.cbServicesMonitor.Location = new System.Drawing.Point(6, 23);
            this.cbServicesMonitor.Name = "cbServicesMonitor";
            this.cbServicesMonitor.Size = new System.Drawing.Size(65, 17);
            this.cbServicesMonitor.TabIndex = 5;
            this.cbServicesMonitor.Text = "Enabled";
            this.cbServicesMonitor.UseVisualStyleBackColor = true;
            this.cbServicesMonitor.CheckedChanged += new System.EventHandler(this.cbServicesMonitor_CheckedChanged);
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
            this.cbcolIncludeSubFolders});
            this.dgvFoldersToTrack.Location = new System.Drawing.Point(77, 19);
            this.dgvFoldersToTrack.MultiSelect = false;
            this.dgvFoldersToTrack.Name = "dgvFoldersToTrack";
            this.dgvFoldersToTrack.RowHeadersVisible = false;
            this.dgvFoldersToTrack.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFoldersToTrack.Size = new System.Drawing.Size(684, 133);
            this.dgvFoldersToTrack.TabIndex = 4;
            // 
            // cbFileSystemMonitor
            // 
            this.cbFileSystemMonitor.AutoSize = true;
            this.cbFileSystemMonitor.Location = new System.Drawing.Point(6, 23);
            this.cbFileSystemMonitor.Name = "cbFileSystemMonitor";
            this.cbFileSystemMonitor.Size = new System.Drawing.Size(65, 17);
            this.cbFileSystemMonitor.TabIndex = 5;
            this.cbFileSystemMonitor.Text = "Enabled";
            this.cbFileSystemMonitor.UseVisualStyleBackColor = true;
            this.cbFileSystemMonitor.CheckedChanged += new System.EventHandler(this.cbFileSystemMonitor_CheckedChanged);
            // 
            // bRemoveFolder
            // 
            this.bRemoveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRemoveFolder.Location = new System.Drawing.Point(704, 158);
            this.bRemoveFolder.Name = "bRemoveFolder";
            this.bRemoveFolder.Size = new System.Drawing.Size(57, 23);
            this.bRemoveFolder.TabIndex = 8;
            this.bRemoveFolder.Text = "Remove";
            this.bRemoveFolder.UseVisualStyleBackColor = true;
            this.bRemoveFolder.Click += new System.EventHandler(this.bRemoveFolder_Click);
            // 
            // bAddFolder
            // 
            this.bAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bAddFolder.Location = new System.Drawing.Point(641, 158);
            this.bAddFolder.Name = "bAddFolder";
            this.bAddFolder.Size = new System.Drawing.Size(57, 23);
            this.bAddFolder.TabIndex = 9;
            this.bAddFolder.Text = "Add";
            this.bAddFolder.UseVisualStyleBackColor = true;
            this.bAddFolder.Click += new System.EventHandler(this.bAddFolder_Click);
            // 
            // gbFileSystemMonitor
            // 
            this.gbFileSystemMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFileSystemMonitor.Controls.Add(this.bAddFolder);
            this.gbFileSystemMonitor.Controls.Add(this.bRemoveFolder);
            this.gbFileSystemMonitor.Controls.Add(this.cbFileSystemMonitor);
            this.gbFileSystemMonitor.Controls.Add(this.dgvFoldersToTrack);
            this.gbFileSystemMonitor.Location = new System.Drawing.Point(12, 12);
            this.gbFileSystemMonitor.Name = "gbFileSystemMonitor";
            this.gbFileSystemMonitor.Size = new System.Drawing.Size(767, 187);
            this.gbFileSystemMonitor.TabIndex = 0;
            this.gbFileSystemMonitor.TabStop = false;
            this.gbFileSystemMonitor.Text = "File System Monitor";
            // 
            // bStartFresh
            // 
            this.bStartFresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bStartFresh.Enabled = false;
            this.bStartFresh.Location = new System.Drawing.Point(672, 450);
            this.bStartFresh.Name = "bStartFresh";
            this.bStartFresh.Size = new System.Drawing.Size(107, 23);
            this.bStartFresh.TabIndex = 8;
            this.bStartFresh.Text = "Start Fresh";
            this.bStartFresh.UseVisualStyleBackColor = true;
            this.bStartFresh.Click += new System.EventHandler(this.bStartFresh_Click);
            // 
            // bwPostInstall
            // 
            this.bwPostInstall.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwPostInstall_DoWork);
            this.bwPostInstall.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwPostInstall_RunWorkerCompleted);
            // 
            // bwLoader
            // 
            this.bwLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoader_DoWork);
            this.bwLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoader_RunWorkerCompleted);
            // 
            // tStatus
            // 
            this.tStatus.Interval = 250;
            this.tStatus.Tick += new System.EventHandler(this.tStatus_Tick);
            // 
            // tbcolFolder
            // 
            this.tbcolFolder.DataPropertyName = "Folder";
            this.tbcolFolder.HeaderText = "Folder";
            this.tbcolFolder.Name = "tbcolFolder";
            this.tbcolFolder.ReadOnly = true;
            this.tbcolFolder.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.tbcolFolder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cbcolIncludeSubFolders
            // 
            this.cbcolIncludeSubFolders.DataPropertyName = "IncludeSubFolders";
            this.cbcolIncludeSubFolders.HeaderText = "Include Sub-Folders?";
            this.cbcolIncludeSubFolders.Name = "cbcolIncludeSubFolders";
            this.cbcolIncludeSubFolders.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // WinChangeMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 703);
            this.Controls.Add(this.bStartFresh);
            this.Controls.Add(this.gbServicesMonitor);
            this.Controls.Add(this.gbRegistryMonitor);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bPostInstall);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.bPreInstall);
            this.Controls.Add(this.gbFileSystemMonitor);
            this.Name = "WinChangeMonitorForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinChangeMonitor - BU MET CS 673/473 Group #3 - Fall 2025";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinChangeMonitorForm_FormClosing);
            this.Load += new System.EventHandler(this.WinChangeMonitorForm_Load);
            this.gbRegistryMonitor.ResumeLayout(false);
            this.gbRegistryMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeysToTrack)).EndInit();
            this.gbServicesMonitor.ResumeLayout(false);
            this.gbServicesMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFoldersToTrack)).EndInit();
            this.gbFileSystemMonitor.ResumeLayout(false);
            this.gbFileSystemMonitor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bPreInstall;
        private System.ComponentModel.BackgroundWorker bwPreInstall;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button bPostInstall;
        public System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.GroupBox gbRegistryMonitor;
        private System.Windows.Forms.CheckBox cbRegistryMonitor;
        private System.Windows.Forms.DataGridView dgvKeysToTrack;
        private System.Windows.Forms.GroupBox gbServicesMonitor;
        private System.Windows.Forms.CheckBox cbServicesMonitor;
        private System.Windows.Forms.ToolTip ttHelp;
        private System.Windows.Forms.DataGridView dgvFoldersToTrack;
        private System.Windows.Forms.CheckBox cbFileSystemMonitor;
        private System.Windows.Forms.Button bRemoveFolder;
        private System.Windows.Forms.Button bAddFolder;
        private System.Windows.Forms.GroupBox gbFileSystemMonitor;
        private System.Windows.Forms.Button bAddKey;
        private System.Windows.Forms.Button bRemoveKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.Button bStartFresh;
        private System.ComponentModel.BackgroundWorker bwPostInstall;
        private System.ComponentModel.BackgroundWorker bwLoader;
        private System.Windows.Forms.Timer tStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn tbcolFolder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cbcolIncludeSubFolders;
    }
}

