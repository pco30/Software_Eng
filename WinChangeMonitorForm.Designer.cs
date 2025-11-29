using JCS;

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
            this.bPostInstall = new System.Windows.Forms.Button();
            this.gbRegistryMonitor = new System.Windows.Forms.GroupBox();
            this.bDefaultTrackedKeys = new System.Windows.Forms.Button();
            this.olvKeysToTrack = new BrightIdeasSoftware.ObjectListView();
            this.olvcKey = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcIncludeSubKeys = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.bAddKey = new System.Windows.Forms.Button();
            this.bRemoveKey = new System.Windows.Forms.Button();
            this.cbRegistryMonitor = new JCS.ToggleSwitch();
            this.gbServicesMonitor = new System.Windows.Forms.GroupBox();
            this.cbServicesMonitor = new JCS.ToggleSwitch();
            this.ttHelp = new System.Windows.Forms.ToolTip(this.components);
            this.cbFileSystemMonitor = new JCS.ToggleSwitch();
            this.bRemoveFolder = new System.Windows.Forms.Button();
            this.bAddFolder = new System.Windows.Forms.Button();
            this.gbFileSystemMonitor = new System.Windows.Forms.GroupBox();
            this.bDefaultTrackedFolders = new System.Windows.Forms.Button();
            this.olvFoldersToTrack = new BrightIdeasSoftware.ObjectListView();
            this.olvcFolder = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcIncludeSubFolders = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.bwPostInstall = new System.ComponentModel.BackgroundWorker();
            this.bwLoader = new System.ComponentModel.BackgroundWorker();
            this.tStatus = new System.Windows.Forms.Timer(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiStartFresh = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.bExportAsJson = new System.Windows.Forms.Button();
            this.bwExportReport = new System.ComponentModel.BackgroundWorker();
            this.gbRegistryMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvKeysToTrack)).BeginInit();
            this.gbServicesMonitor.SuspendLayout();
            this.gbFileSystemMonitor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvFoldersToTrack)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // bPreInstall
            // 
            this.bPreInstall.Location = new System.Drawing.Point(12, 467);
            this.bPreInstall.Name = "bPreInstall";
            this.bPreInstall.Size = new System.Drawing.Size(102, 23);
            this.bPreInstall.TabIndex = 1;
            this.bPreInstall.Text = "Perform Pre-Install";
            this.bPreInstall.UseVisualStyleBackColor = true;
            this.bPreInstall.Click += new System.EventHandler(this.bPreInstall_Click);
            // 
            // bwPreInstall
            // 
            this.bwPreInstall.WorkerSupportsCancellation = true;
            this.bwPreInstall.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwPreInstall_DoWork);
            this.bwPreInstall.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwPreInstall_RunWorkerCompleted);
            // 
            // bPostInstall
            // 
            this.bPostInstall.Enabled = false;
            this.bPostInstall.Location = new System.Drawing.Point(120, 467);
            this.bPostInstall.Name = "bPostInstall";
            this.bPostInstall.Size = new System.Drawing.Size(107, 23);
            this.bPostInstall.TabIndex = 3;
            this.bPostInstall.Text = "Perform Post-Install";
            this.bPostInstall.UseVisualStyleBackColor = true;
            this.bPostInstall.Click += new System.EventHandler(this.bPostInstall_Click);
            // 
            // gbRegistryMonitor
            // 
            this.gbRegistryMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRegistryMonitor.Controls.Add(this.bDefaultTrackedKeys);
            this.gbRegistryMonitor.Controls.Add(this.olvKeysToTrack);
            this.gbRegistryMonitor.Controls.Add(this.bAddKey);
            this.gbRegistryMonitor.Controls.Add(this.bRemoveKey);
            this.gbRegistryMonitor.Controls.Add(this.cbRegistryMonitor);
            this.gbRegistryMonitor.Location = new System.Drawing.Point(12, 220);
            this.gbRegistryMonitor.Name = "gbRegistryMonitor";
            this.gbRegistryMonitor.Size = new System.Drawing.Size(767, 187);
            this.gbRegistryMonitor.TabIndex = 6;
            this.gbRegistryMonitor.TabStop = false;
            this.gbRegistryMonitor.Text = "Registry Monitor";
            // 
            // bDefaultTrackedKeys
            // 
            this.bDefaultTrackedKeys.AutoSize = true;
            this.bDefaultTrackedKeys.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bDefaultTrackedKeys.Location = new System.Drawing.Point(77, 158);
            this.bDefaultTrackedKeys.Name = "bDefaultTrackedKeys";
            this.bDefaultTrackedKeys.Size = new System.Drawing.Size(78, 23);
            this.bDefaultTrackedKeys.TabIndex = 12;
            this.bDefaultTrackedKeys.Text = "Use Defaults";
            this.bDefaultTrackedKeys.UseVisualStyleBackColor = true;
            this.bDefaultTrackedKeys.Click += new System.EventHandler(this.bDefaultTrackedKeys_Click);
            // 
            // olvKeysToTrack
            // 
            this.olvKeysToTrack.AllColumns.Add(this.olvcKey);
            this.olvKeysToTrack.AllColumns.Add(this.olvcIncludeSubKeys);
            this.olvKeysToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvKeysToTrack.CellEditUseWholeCell = false;
            this.olvKeysToTrack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcKey,
            this.olvcIncludeSubKeys});
            this.olvKeysToTrack.CopySelectionOnControlC = false;
            this.olvKeysToTrack.CopySelectionOnControlCUsesDragSource = false;
            this.olvKeysToTrack.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvKeysToTrack.FullRowSelect = true;
            this.olvKeysToTrack.HasCollapsibleGroups = false;
            this.olvKeysToTrack.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olvKeysToTrack.HideSelection = false;
            this.olvKeysToTrack.IsSearchOnSortColumn = false;
            this.olvKeysToTrack.Location = new System.Drawing.Point(77, 20);
            this.olvKeysToTrack.MultiSelect = false;
            this.olvKeysToTrack.Name = "olvKeysToTrack";
            this.olvKeysToTrack.SelectAllOnControlA = false;
            this.olvKeysToTrack.SelectColumnsMenuStaysOpen = false;
            this.olvKeysToTrack.SelectColumnsOnRightClick = false;
            this.olvKeysToTrack.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.olvKeysToTrack.SelectedBackColor = System.Drawing.SystemColors.Highlight;
            this.olvKeysToTrack.ShowFilterMenuOnRightClick = false;
            this.olvKeysToTrack.ShowGroups = false;
            this.olvKeysToTrack.ShowSortIndicators = false;
            this.olvKeysToTrack.Size = new System.Drawing.Size(684, 133);
            this.olvKeysToTrack.SortGroupItemsByPrimaryColumn = false;
            this.olvKeysToTrack.TabIndex = 11;
            this.olvKeysToTrack.UnfocusedSelectedBackColor = System.Drawing.SystemColors.Highlight;
            this.olvKeysToTrack.UnfocusedSelectedForeColor = System.Drawing.SystemColors.HighlightText;
            this.olvKeysToTrack.UseCompatibleStateImageBehavior = false;
            this.olvKeysToTrack.UseHotControls = false;
            this.olvKeysToTrack.View = System.Windows.Forms.View.Details;
            this.olvKeysToTrack.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.olvKeysToTrack_CellClick);
            this.olvKeysToTrack.SubItemChecking += new System.EventHandler<BrightIdeasSoftware.SubItemCheckingEventArgs>(this.olvKeysToTrack_SubItemChecking);
            this.olvKeysToTrack.SelectedIndexChanged += new System.EventHandler(this.olvKeysToTrack_SelectedIndexChanged);
            this.olvKeysToTrack.EnabledChanged += new System.EventHandler(this.olvKeysToTrack_EnabledChanged);
            // 
            // olvcKey
            // 
            this.olvcKey.AspectName = "Key";
            this.olvcKey.AutoCompleteEditor = false;
            this.olvcKey.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvcKey.Groupable = false;
            this.olvcKey.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.olvcKey.Hideable = false;
            this.olvcKey.IsEditable = false;
            this.olvcKey.Searchable = false;
            this.olvcKey.Sortable = false;
            this.olvcKey.Text = "Key";
            this.olvcKey.UseFiltering = false;
            // 
            // olvcIncludeSubKeys
            // 
            this.olvcIncludeSubKeys.AspectName = "IncludeSubKeys";
            this.olvcIncludeSubKeys.AutoCompleteEditor = false;
            this.olvcIncludeSubKeys.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvcIncludeSubKeys.CheckBoxes = true;
            this.olvcIncludeSubKeys.Groupable = false;
            this.olvcIncludeSubKeys.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.olvcIncludeSubKeys.Hideable = false;
            this.olvcIncludeSubKeys.Searchable = false;
            this.olvcIncludeSubKeys.Sortable = false;
            this.olvcIncludeSubKeys.Text = "Include Sub-Keys?";
            this.olvcIncludeSubKeys.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvcIncludeSubKeys.UseFiltering = false;
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
            this.bRemoveKey.Enabled = false;
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
            this.cbRegistryMonitor.Checked = true;
            this.cbRegistryMonitor.Location = new System.Drawing.Point(6, 23);
            this.cbRegistryMonitor.Name = "cbRegistryMonitor";
            this.cbRegistryMonitor.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRegistryMonitor.OffText = "OFF";
            this.cbRegistryMonitor.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbRegistryMonitor.OnText = "ON";
            this.cbRegistryMonitor.Size = new System.Drawing.Size(65, 23);
            this.cbRegistryMonitor.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Iphone;
            this.cbRegistryMonitor.TabIndex = 5;
            this.cbRegistryMonitor.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.cbRegistryMonitor_CheckedChanged);
            // 
            // gbServicesMonitor
            // 
            this.gbServicesMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbServicesMonitor.Controls.Add(this.cbServicesMonitor);
            this.gbServicesMonitor.Location = new System.Drawing.Point(12, 413);
            this.gbServicesMonitor.Name = "gbServicesMonitor";
            this.gbServicesMonitor.Size = new System.Drawing.Size(767, 48);
            this.gbServicesMonitor.TabIndex = 7;
            this.gbServicesMonitor.TabStop = false;
            this.gbServicesMonitor.Text = "Services Monitor";
            // 
            // cbServicesMonitor
            // 
            this.cbServicesMonitor.Checked = true;
            this.cbServicesMonitor.Location = new System.Drawing.Point(6, 20);
            this.cbServicesMonitor.Name = "cbServicesMonitor";
            this.cbServicesMonitor.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbServicesMonitor.OffText = "OFF";
            this.cbServicesMonitor.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbServicesMonitor.OnText = "ON";
            this.cbServicesMonitor.Size = new System.Drawing.Size(65, 23);
            this.cbServicesMonitor.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Iphone;
            this.cbServicesMonitor.TabIndex = 5;
            this.cbServicesMonitor.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.cbServicesMonitor_CheckedChanged);
            // 
            // cbFileSystemMonitor
            // 
            this.cbFileSystemMonitor.Checked = true;
            this.cbFileSystemMonitor.Location = new System.Drawing.Point(6, 23);
            this.cbFileSystemMonitor.Name = "cbFileSystemMonitor";
            this.cbFileSystemMonitor.OffFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFileSystemMonitor.OffText = "OFF";
            this.cbFileSystemMonitor.OnFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFileSystemMonitor.OnText = "ON";
            this.cbFileSystemMonitor.Size = new System.Drawing.Size(65, 23);
            this.cbFileSystemMonitor.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Iphone;
            this.cbFileSystemMonitor.TabIndex = 5;
            this.cbFileSystemMonitor.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.cbFileSystemMonitor_CheckedChanged);
            // 
            // bRemoveFolder
            // 
            this.bRemoveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRemoveFolder.Enabled = false;
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
            this.gbFileSystemMonitor.Controls.Add(this.bDefaultTrackedFolders);
            this.gbFileSystemMonitor.Controls.Add(this.olvFoldersToTrack);
            this.gbFileSystemMonitor.Controls.Add(this.bAddFolder);
            this.gbFileSystemMonitor.Controls.Add(this.bRemoveFolder);
            this.gbFileSystemMonitor.Controls.Add(this.cbFileSystemMonitor);
            this.gbFileSystemMonitor.Location = new System.Drawing.Point(12, 27);
            this.gbFileSystemMonitor.Name = "gbFileSystemMonitor";
            this.gbFileSystemMonitor.Size = new System.Drawing.Size(767, 187);
            this.gbFileSystemMonitor.TabIndex = 0;
            this.gbFileSystemMonitor.TabStop = false;
            this.gbFileSystemMonitor.Text = "File System Monitor";
            // 
            // bDefaultTrackedFolders
            // 
            this.bDefaultTrackedFolders.AutoSize = true;
            this.bDefaultTrackedFolders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bDefaultTrackedFolders.Location = new System.Drawing.Point(77, 158);
            this.bDefaultTrackedFolders.Name = "bDefaultTrackedFolders";
            this.bDefaultTrackedFolders.Size = new System.Drawing.Size(78, 23);
            this.bDefaultTrackedFolders.TabIndex = 11;
            this.bDefaultTrackedFolders.Text = "Use Defaults";
            this.bDefaultTrackedFolders.UseVisualStyleBackColor = true;
            this.bDefaultTrackedFolders.Click += new System.EventHandler(this.bDefaultTrackedFolders_Click);
            // 
            // olvFoldersToTrack
            // 
            this.olvFoldersToTrack.AllColumns.Add(this.olvcFolder);
            this.olvFoldersToTrack.AllColumns.Add(this.olvcIncludeSubFolders);
            this.olvFoldersToTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvFoldersToTrack.CellEditUseWholeCell = false;
            this.olvFoldersToTrack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcFolder,
            this.olvcIncludeSubFolders});
            this.olvFoldersToTrack.CopySelectionOnControlC = false;
            this.olvFoldersToTrack.CopySelectionOnControlCUsesDragSource = false;
            this.olvFoldersToTrack.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvFoldersToTrack.FullRowSelect = true;
            this.olvFoldersToTrack.HasCollapsibleGroups = false;
            this.olvFoldersToTrack.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olvFoldersToTrack.HideSelection = false;
            this.olvFoldersToTrack.IsSearchOnSortColumn = false;
            this.olvFoldersToTrack.Location = new System.Drawing.Point(77, 20);
            this.olvFoldersToTrack.MultiSelect = false;
            this.olvFoldersToTrack.Name = "olvFoldersToTrack";
            this.olvFoldersToTrack.SelectAllOnControlA = false;
            this.olvFoldersToTrack.SelectColumnsMenuStaysOpen = false;
            this.olvFoldersToTrack.SelectColumnsOnRightClick = false;
            this.olvFoldersToTrack.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.olvFoldersToTrack.SelectedBackColor = System.Drawing.SystemColors.Highlight;
            this.olvFoldersToTrack.ShowFilterMenuOnRightClick = false;
            this.olvFoldersToTrack.ShowGroups = false;
            this.olvFoldersToTrack.ShowSortIndicators = false;
            this.olvFoldersToTrack.Size = new System.Drawing.Size(684, 133);
            this.olvFoldersToTrack.SortGroupItemsByPrimaryColumn = false;
            this.olvFoldersToTrack.TabIndex = 10;
            this.olvFoldersToTrack.UnfocusedSelectedBackColor = System.Drawing.SystemColors.Highlight;
            this.olvFoldersToTrack.UnfocusedSelectedForeColor = System.Drawing.SystemColors.HighlightText;
            this.olvFoldersToTrack.UseCompatibleStateImageBehavior = false;
            this.olvFoldersToTrack.UseHotControls = false;
            this.olvFoldersToTrack.View = System.Windows.Forms.View.Details;
            this.olvFoldersToTrack.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.olvFoldersToTrack_CellClick);
            this.olvFoldersToTrack.SubItemChecking += new System.EventHandler<BrightIdeasSoftware.SubItemCheckingEventArgs>(this.olvFoldersToTrack_SubItemChecking);
            this.olvFoldersToTrack.SelectedIndexChanged += new System.EventHandler(this.olvFoldersToTrack_SelectedIndexChanged);
            this.olvFoldersToTrack.EnabledChanged += new System.EventHandler(this.olvFoldersToTrack_EnabledChanged);
            this.olvFoldersToTrack.Resize += new System.EventHandler(this.olvFoldersToTrack_Resize);
            // 
            // olvcFolder
            // 
            this.olvcFolder.AspectName = "Folder";
            this.olvcFolder.AutoCompleteEditor = false;
            this.olvcFolder.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvcFolder.Groupable = false;
            this.olvcFolder.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.olvcFolder.Hideable = false;
            this.olvcFolder.IsEditable = false;
            this.olvcFolder.Searchable = false;
            this.olvcFolder.Sortable = false;
            this.olvcFolder.Text = "Folder";
            this.olvcFolder.UseFiltering = false;
            // 
            // olvcIncludeSubFolders
            // 
            this.olvcIncludeSubFolders.AspectName = "IncludeSubFolders";
            this.olvcIncludeSubFolders.AutoCompleteEditor = false;
            this.olvcIncludeSubFolders.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
            this.olvcIncludeSubFolders.CheckBoxes = true;
            this.olvcIncludeSubFolders.Groupable = false;
            this.olvcIncludeSubFolders.HeaderCheckBoxUpdatesRowCheckBoxes = false;
            this.olvcIncludeSubFolders.Hideable = false;
            this.olvcIncludeSubFolders.Searchable = false;
            this.olvcIncludeSubFolders.Sortable = false;
            this.olvcIncludeSubFolders.Text = "Include Sub-Folders?";
            this.olvcIncludeSubFolders.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvcIncludeSubFolders.UseFiltering = false;
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
            this.tStatus.Interval = 1000;
            this.tStatus.Tick += new System.EventHandler(this.tStatus_Tick);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(791, 24);
            this.menuStrip.TabIndex = 9;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.exitToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exitToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.optionsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreToolStripMenuItem,
            this.toolStripSeparator1,
            this.tsmiStartFresh});
            this.optionsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // ignoreToolStripMenuItem
            // 
            this.ignoreToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ignoreToolStripMenuItem.Checked = true;
            this.ignoreToolStripMenuItem.CheckOnClick = true;
            this.ignoreToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ignoreToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ignoreToolStripMenuItem.Name = "ignoreToolStripMenuItem";
            this.ignoreToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.ignoreToolStripMenuItem.Text = "Ignore Unnecessary Folders";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // tsmiStartFresh
            // 
            this.tsmiStartFresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tsmiStartFresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiStartFresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsmiStartFresh.Name = "tsmiStartFresh";
            this.tsmiStartFresh.Size = new System.Drawing.Size(218, 22);
            this.tsmiStartFresh.Text = "Start Fresh";
            this.tsmiStartFresh.Click += new System.EventHandler(this.startFreshToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.helpToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.aboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.aboutToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 493);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(791, 22);
            this.statusBar.TabIndex = 10;
            this.statusBar.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tsslStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(745, 17);
            this.tsslStatus.Spring = true;
            this.tsslStatus.Text = "tsslStatus";
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bExportAsJson
            // 
            this.bExportAsJson.Enabled = false;
            this.bExportAsJson.Location = new System.Drawing.Point(233, 467);
            this.bExportAsJson.Name = "bExportAsJson";
            this.bExportAsJson.Size = new System.Drawing.Size(107, 23);
            this.bExportAsJson.TabIndex = 11;
            this.bExportAsJson.Text = "Export Report";
            this.bExportAsJson.UseVisualStyleBackColor = true;
            this.bExportAsJson.Click += new System.EventHandler(this.bExportAsJson_Click);
            // 
            // bwExportReport
            // 
            this.bwExportReport.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwExportReport_DoWork);
            this.bwExportReport.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwExportReport_RunWorkerCompleted);
            // 
            // WinChangeMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 515);
            this.Controls.Add(this.bExportAsJson);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.gbServicesMonitor);
            this.Controls.Add(this.gbRegistryMonitor);
            this.Controls.Add(this.bPostInstall);
            this.Controls.Add(this.bPreInstall);
            this.Controls.Add(this.gbFileSystemMonitor);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "WinChangeMonitorForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinChangeMonitor - BU MET CS 673/473 Group #3 - Fall 2025";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinChangeMonitorForm_FormClosing);
            this.Load += new System.EventHandler(this.WinChangeMonitorForm_Load);
            this.Resize += new System.EventHandler(this.WinChangeMonitorForm_Resize);
            this.gbRegistryMonitor.ResumeLayout(false);
            this.gbRegistryMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvKeysToTrack)).EndInit();
            this.gbServicesMonitor.ResumeLayout(false);
            this.gbFileSystemMonitor.ResumeLayout(false);
            this.gbFileSystemMonitor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvFoldersToTrack)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bPreInstall;
        private System.ComponentModel.BackgroundWorker bwPreInstall;
        private System.Windows.Forms.Button bPostInstall;
        private System.Windows.Forms.GroupBox gbRegistryMonitor;
        private JCS.ToggleSwitch cbRegistryMonitor;
        private System.Windows.Forms.GroupBox gbServicesMonitor;
        private JCS.ToggleSwitch cbServicesMonitor;
        private System.Windows.Forms.ToolTip ttHelp;
        private JCS.ToggleSwitch cbFileSystemMonitor;
        private System.Windows.Forms.Button bRemoveFolder;
        private System.Windows.Forms.Button bAddFolder;
        private System.Windows.Forms.GroupBox gbFileSystemMonitor;
        private System.Windows.Forms.Button bAddKey;
        private System.Windows.Forms.Button bRemoveKey;
        private System.ComponentModel.BackgroundWorker bwPostInstall;
        private System.ComponentModel.BackgroundWorker bwLoader;
        private System.Windows.Forms.Timer tStatus;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private BrightIdeasSoftware.ObjectListView olvFoldersToTrack;
        private BrightIdeasSoftware.OLVColumn olvcFolder;
        private BrightIdeasSoftware.OLVColumn olvcIncludeSubFolders;
        private BrightIdeasSoftware.ObjectListView olvKeysToTrack;
        private BrightIdeasSoftware.OLVColumn olvcKey;
        private BrightIdeasSoftware.OLVColumn olvcIncludeSubKeys;
        private System.Windows.Forms.ToolStripMenuItem ignoreToolStripMenuItem;
        private System.Windows.Forms.Button bDefaultTrackedFolders;
        private System.Windows.Forms.Button bDefaultTrackedKeys;
        private System.Windows.Forms.ToolStripMenuItem tsmiStartFresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button bExportAsJson;
        private System.ComponentModel.BackgroundWorker bwExportReport;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
    }
}

