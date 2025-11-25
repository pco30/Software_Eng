using BrightIdeasSoftware;
using JCS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class WinChangeMonitorForm : Form
    {
        public static SplashScreenForm SplashScreen { get; private set; }

        public Boolean ConfirmOnClose { get; set; } = true;

        private const String HKCR = "HKEY_CLASSES_ROOT";
        private const String HKCU = "HKEY_CURRENT_USER";
        private const String HKLM = "HKEY_LOCAL_MACHINE";
        private const String HKU = "HKEY_USERS";
        private const String HKCC = "HKEY_CURRENT_CONFIG";

        private static Color ToggleSwitchOnColor = Color.Green;
        private static Dictionary<RegistryValueKind, String> translateRegKind = new Dictionary<RegistryValueKind, String> {
            { RegistryValueKind.Binary, "REG_BINARY" },
            { RegistryValueKind.DWord, "REG_DWORD" },
            { RegistryValueKind.ExpandString, "REG_EXPAND_SZ" },
            { RegistryValueKind.MultiString, "REG_MULTI_SZ" },
            { RegistryValueKind.QWord, "REG_QWORD" },
            { RegistryValueKind.String, "REG_SZ" }
        };

        public SortedDictionary<String, Boolean> FolderContentsAdded { get { return this.folderContentsAdded; } }
        public SortedDictionary<String, Boolean> FolderContentsModified { get { return this.folderContentsModified; } }
        public SortedDictionary<String, RegistryEntryInfo> RegistryContentsAdded { get { return this.registryContentsAdded; } }
        public SortedDictionary<String, RegistryEntryDiff> RegistryContentsModified { get { return this.registryContentsModified; } }
        public SortedDictionary<String, ServiceInfo> ServicesAdded { get { return this.servicesAdded; } }
        public SortedDictionary<String, ServiceDiff> ServicesModified {  get { return this.servicesModified; } }

        private DateTime loadStarted, loadFinished;
        private DateTime? preInstallFoldersStarted = null;
        private DateTime? preInstallRegistryStarted = null;
        private DateTime? preInstallServicesStarted = null;
        private DateTime? postInstallFoldersStarted = null;
        private DateTime? postInstallFoldersFinished = null;
        private DateTime? postInstallRegistryStarted = null;
        private DateTime? postInstallRegistryFinished = null;
        private DateTime? postInstallServicesStarted = null;
        private DateTime? postInstallServicesFinished = null;
        private FolderBrowserDialog fbdAddFolder = new FolderBrowserDialog();
        private RegistryKeyBrowserDialog rkbdAddKey = new RegistryKeyBrowserDialog();
        private SaveFileDialog sfdSaveReport = new SaveFileDialog();
        private SortedDictionary<String, Boolean> folderContentsAdded = new SortedDictionary<String, Boolean>();
        private SortedDictionary<String, Boolean> folderContentsModified = new SortedDictionary<String, Boolean>();
        private SortedDictionary<String, RegistryEntryInfo> registryContentsAdded = new SortedDictionary<String, RegistryEntryInfo>();
        private SortedDictionary<String, RegistryEntryDiff> registryContentsModified = new SortedDictionary<String, RegistryEntryDiff>();
        private SortedDictionary<String, ServiceInfo> servicesAdded = new SortedDictionary<String, ServiceInfo>();
        private SortedDictionary<String, ServiceDiff> servicesModified = new SortedDictionary<String, ServiceDiff>();
        private String operation, current;

        public WinChangeMonitorForm()
        {
            try
            {
                InitializeComponent();

                this.menuStrip.Renderer = new CustomToolStripProfessionalRenderer();

                this.cbFileSystemMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });
                this.cbRegistryMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });
                this.cbServicesMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });

                this.statusBar.Renderer = new CustomRenderer();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void AutoSizeColumns(ObjectListView olv)
        {
            try
            {
                if (olv.Columns.Count > 0)
                {
                    int colWidth = olv.ClientSize.Width / olv.Columns.Count;

                    foreach (OLVColumn column in olv.Columns)
                    {
                        column.Width = column.MaximumWidth = column.MinimumWidth = colWidth;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void WinChangeMonitorForm_Load(Object sender, EventArgs e)
        {
            try
            {
                AutoSizeColumns(this.olvFoldersToTrack);
                AutoSizeColumns(this.olvKeysToTrack);

                SplashScreen = new SplashScreenForm(this);

                this.MinimumSize = this.Size;

                this.fbdAddFolder.ShowNewFolderButton = false;
                this.fbdAddFolder.RootFolder = Environment.SpecialFolder.MyComputer;
                
                this.sfdSaveReport.FileName = "Report.html";
                this.sfdSaveReport.Filter = "HTML Files (*.htm, *.html)|*.htm;*.html";
                this.sfdSaveReport.InitialDirectory = RetainedSettings.DirectoryName;
                this.sfdSaveReport.OverwritePrompt = true;
                this.sfdSaveReport.ValidateNames = true;

                SplashScreen.Show(this);

                this.loadStarted = DateTime.Now;

                this.bwLoader.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bPreInstall_Click(Object sender, EventArgs e)
        {
            try
            {
                if (this.cbFileSystemMonitor.Checked || this.cbRegistryMonitor.Checked || this.cbServicesMonitor.Checked)
                {
                    this.bPreInstall.Enabled = false;

                    this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bDefaultTrackedFolders.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                    this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bDefaultTrackedKeys.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
                    this.cbServicesMonitor.Enabled = false;
                    this.bPostInstall.Enabled = this.tsmiStartFresh.Enabled = false;

                    this.tStatus.Start();

                    this.bwPreInstall.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Please enable at least one monitor to perform a Pre-Installation Inventory.", "Enable Monitor(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PreInstallInventoryDirectory(String directory, Boolean recursive)
        {
            try
            {
                this.current = directory;

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        RetainedSettings.FileSystemInventory[file] = new RetainedSettings.FileSystemSettings.FileSystemEntryInfo { IsFolder = false };
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        RetainedSettings.FileSystemInventory[subDirectory] = new RetainedSettings.FileSystemSettings.FileSystemEntryInfo { IsFolder = true };

                        if (recursive)
                        {
                            PreInstallInventoryDirectory(subDirectory, recursive);
                        }
                    }
                }
                catch (UnauthorizedAccessException) { }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PreInstallInventoryRegistry(RegistryKey key, Boolean recursive)
        {
            try
            {
                this.current = key.ToString();

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        RetainedSettings.RegistryInventory[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = Utilities.PrettyString(key.GetValue(valueName)) };
                    }
                }

                String[] subkeyNames = key.GetSubKeyNames();

                foreach (String subkeyName in subkeyNames)
                {
                    fullPath = $@"{key.Name}\{subkeyName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        RetainedSettings.RegistryInventory[fullPath] = null;
                    }

                    if (recursive)
                    {
                        try
                        {
                            PreInstallInventoryRegistry(key.OpenSubKey(subkeyName), recursive);
                        }
                        catch (SecurityException) { }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PreInstallInventoryServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    this.current = service.ServiceName;

                    if (!RetainedSettings.ServicesInventory.ContainsKey(service.ServiceName))
                    {
                        RetainedSettings.ServicesInventory[service.ServiceName] = ServiceInfo.Parse(service);
                    }
                }
            }
            catch(Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private RegistryKey OpenRegistryKey(String keyPath)
        {
            try
            {
                RegistryKey key = null;

                if (keyPath.StartsWith($@"{HKCR}"))
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
                }
                else if (keyPath.StartsWith($@"{HKCU}"))
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                }
                else if (keyPath.StartsWith($@"{HKLM}"))
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                }
                else if (keyPath.StartsWith($@"{HKU}"))
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
                }
                else if (keyPath.StartsWith($@"{HKCC}"))
                {
                    key = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
                }

                if (key == null)
                {
                    return null;
                }

                String[] split = keyPath.Split('\\');

                for (Int32 i = 1; i < split.Length; ++i)
                {
                    key = key.OpenSubKey(split[i]);

                    if (key == null)
                    {
                        return null;
                    }
                }
                
                return key;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                return null;
            }
        }

        private void bwPreInstall_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                if (this.cbFileSystemMonitor.Checked && RetainedSettings.FoldersToTrack.Count == 0)
                {
                    Utilities.ToggleSwitchSetChecked(this.cbFileSystemMonitor, false);
                }

                if (this.cbRegistryMonitor.Checked && RetainedSettings.KeysToTrack.Count == 0)
                {
                    Utilities.ToggleSwitchSetChecked(this.cbRegistryMonitor, false);
                }

                if (this.cbFileSystemMonitor.Checked)
                {
                    SaveTrackedFoldersToConfig();
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    SaveTrackedKeysToConfig();
                }

                if (this.cbFileSystemMonitor.Checked)
                {   
                    this.preInstallFoldersStarted = DateTime.Now;

                    this.operation = "Performing File System Inventory";

                    foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in RetainedSettings.FoldersToTrack)
                    {
                        PreInstallInventoryDirectory(folder.Folder, folder.IncludeSubFolders);
                    }

                    RetainedSettings.PreInstallFileSystemFinished = DateTime.Now;

                    RetainedSettings.SaveFileSystemSettings();
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.preInstallRegistryStarted = DateTime.Now;

                    RegistryKey key;

                    this.operation = "Performing Registry Inventory";

                    foreach (RetainedSettings.RegistrySettings.TrackedKey registryKey in RetainedSettings.KeysToTrack)
                    {
                        key = OpenRegistryKey(registryKey.Key);

                        PreInstallInventoryRegistry(key, registryKey.IncludeSubKeys);
                    }

                    RetainedSettings.PreInstallRegistryFinished = DateTime.Now;

                    RetainedSettings.SaveRegistrySettings();
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.preInstallServicesStarted = DateTime.Now;

                    this.operation = "Performing Services Inventory";

                    PreInstallInventoryServices();

                    RetainedSettings.PreInstallServicesFinished = DateTime.Now;

                    RetainedSettings.SaveServicesSettings();
                }

                RetainedSettings.SaveCommonInfo();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bwPreInstall_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.tStatus.Stop();

                this.bPostInstall.Enabled = this.tsmiStartFresh.Enabled = true;

                this.tsslStatus.Text = "Pre-Install Inventory Complete";
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bPostInstall_Click(Object sender, EventArgs e)
        {
            try
            {
                if (this.sfdSaveReport.ShowDialog() == DialogResult.OK)
                {
                    this.bPostInstall.Enabled = false;

                    this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                    this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
                    this.cbServicesMonitor.Enabled = false;
                    this.bPreInstall.Enabled = this.tsmiStartFresh.Enabled = false;

                    this.bwPostInstall.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void cbFileSystemMonitor_CheckedChanged(Object sender, EventArgs e)
        {
            try
            {
                this.olvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.cbFileSystemMonitor.Checked;
                if (this.olvFoldersToTrack.SelectedIndex != -1)
                {
                    this.bRemoveFolder.Enabled = true;
                }
                else
                {
                    this.bRemoveFolder.Enabled = false;
                }
                this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked || this.cbRegistryMonitor.Checked || this.cbServicesMonitor.Checked);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void cbRegistryMonitor_CheckedChanged(Object sender, EventArgs e)
        {
            try
            {
                this.olvKeysToTrack.Enabled = this.bAddKey.Enabled = this.cbRegistryMonitor.Checked;
                if (this.olvKeysToTrack.SelectedIndex != -1)
                {
                    this.bRemoveKey.Enabled = true;
                }
                else
                {
                    this.bRemoveKey.Enabled = false;
                }
                this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked || this.cbRegistryMonitor.Checked || this.cbServicesMonitor.Checked);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void cbServicesMonitor_CheckedChanged(Object sender, EventArgs e)
        {
            try
            {
                this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked || this.cbRegistryMonitor.Checked || this.cbServicesMonitor.Checked);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bRemoveFolder_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.olvFoldersToTrack.Items[this.olvFoldersToTrack.SelectedIndex].Text}?", "Remove Folder?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Int32 oldIndex = this.olvFoldersToTrack.SelectedIndex;

                    RetainedSettings.FoldersToTrack.RemoveAt(this.olvFoldersToTrack.SelectedIndex);

                    this.olvFoldersToTrack.ClearObjects();
                    this.olvFoldersToTrack.AddObjects(RetainedSettings.FoldersToTrack);

                    AutoSizeColumns(this.olvFoldersToTrack); // the scrollbar may be visible now and column sizes need to consider that

                    this.olvFoldersToTrack.DeselectAll();

                    if (RetainedSettings.FoldersToTrack.Count > 0)
                    {
                        if (oldIndex < RetainedSettings.FoldersToTrack.Count)
                        {
                            // an item before the last one was removed, select the new one at that position
                            this.olvFoldersToTrack.Items[oldIndex].Selected = true;
                        }
                        else
                        {
                            // the last item was removed, select the new last one
                            this.olvFoldersToTrack.Items[this.olvFoldersToTrack.Items.Count - 1].Selected = true;
                        }

                        this.olvFoldersToTrack.Focus();
                    }
                    else // RetainedSettings.FoldersToTrack.Count == 0
                    {
                        this.bRemoveFolder.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbRegistryMonitor.Checked && RetainedSettings.KeysToTrack.Count > 0) || this.cbServicesMonitor.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bRemoveKey_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.olvKeysToTrack.Items[this.olvKeysToTrack.SelectedIndex].Text}?", "Remove Key?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Int32 oldIndex = this.olvKeysToTrack.SelectedIndex;

                    RetainedSettings.KeysToTrack.RemoveAt(this.olvKeysToTrack.SelectedIndex);

                    this.olvKeysToTrack.ClearObjects();
                    this.olvKeysToTrack.AddObjects(RetainedSettings.KeysToTrack);

                    AutoSizeColumns(this.olvKeysToTrack); // the scrollbar may be visible now and column sizes need to consider that

                    this.olvKeysToTrack.DeselectAll();

                    if (RetainedSettings.KeysToTrack.Count > 0)
                    {
                        if (oldIndex < RetainedSettings.KeysToTrack.Count)
                        {
                            // an item before the last one was removed, select the new one at that position
                            this.olvKeysToTrack.Items[oldIndex].Selected = true;
                        }
                        else
                        {
                            // the last one was removed, select the new last one
                            this.olvKeysToTrack.Items[this.olvKeysToTrack.Items.Count - 1].Selected = true;
                        }

                        this.olvKeysToTrack.Focus();
                    }
                    else // RetainedSettings.KeysToTrack.Count == 0
                    { 
                        this.bRemoveKey.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked && RetainedSettings.FoldersToTrack.Count > 0) || this.cbServicesMonitor.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bAddFolder_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = this.fbdAddFolder.ShowDialog();

                if (result == DialogResult.OK)
                {
                    RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder { Folder = this.fbdAddFolder.SelectedPath, IncludeSubFolders = false });

                    this.olvFoldersToTrack.ClearObjects();
                    this.olvFoldersToTrack.AddObjects(RetainedSettings.FoldersToTrack);

                    AutoSizeColumns(this.olvFoldersToTrack);

                    this.olvFoldersToTrack.DeselectAll();
                    this.olvFoldersToTrack.Items[this.olvFoldersToTrack.Items.Count - 1].Selected = true;
                    this.olvFoldersToTrack.Focus();
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bAddKey_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = this.rkbdAddKey.ShowDialog();

                if (result == DialogResult.OK)
                {
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = this.rkbdAddKey.SelectedKeyPath, IncludeSubKeys = false });

                    this.olvKeysToTrack.ClearObjects();
                    this.olvKeysToTrack.AddObjects(RetainedSettings.KeysToTrack);

                    AutoSizeColumns(this.olvKeysToTrack);

                    this.olvKeysToTrack.DeselectAll();
                    this.olvKeysToTrack.Items[this.olvKeysToTrack.Items.Count - 1].Selected = true;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PostInstallInventoryDirectory(String directory, Boolean recursive)
        {
            try
            {
                this.operation = "Performing File System Inventory";
                this.current = directory;

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        if (!RetainedSettings.FileSystemInventory.ContainsKey(file))
                        {
                            this.folderContentsAdded[file] = false;
                        }
                        else if (RetainedSettings.FileSystemInventory[file].IsFolder == true) // file was a folder before and it's a file now
                        {
                            //this.folderContentsRemoved[file] = true; // the original folder was deleted
                            this.folderContentsAdded[file] = false; // the new file was added
                        }
                        else // both preinstall and postinstall inventories contain the file, check to see if it was modified
                        {
                            if (File.GetLastWriteTime(file) > RetainedSettings.PreInstallFileSystemFinished)
                            {
                                this.folderContentsModified[file] = false;
                            }

                            RetainedSettings.FileSystemInventory.Remove(file); // remove file from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        }
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        if (!RetainedSettings.FileSystemInventory.ContainsKey(subDirectory))
                        {
                            this.folderContentsAdded[subDirectory] = true;
                        }
                        else if (RetainedSettings.FileSystemInventory[subDirectory].IsFolder == false) // subDirectory was a file before and it's a directory now
                        {
                            //this.folderContentsRemoved[subDirectory] = false; // the original file was deleted
                            this.folderContentsAdded[subDirectory] = true; // the new directory was added
                        }
                        else // both preinstall and postinstall inventories contain the directory, check to see if it was modified
                        {
                            if (Directory.GetLastWriteTime(subDirectory) > RetainedSettings.PreInstallFileSystemFinished)
                            {
                                this.folderContentsModified[subDirectory] = true;
                            }

                            RetainedSettings.FileSystemInventory.Remove(subDirectory); // remove directory from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        }

                        if (recursive)
                        {
                            PostInstallInventoryDirectory(subDirectory, recursive);
                        }
                    }
                }
                catch (UnauthorizedAccessException) { }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PostInstallInventoryRegistry(RegistryKey key, Boolean recursive)
        {
            try
            {
                this.tsslStatus.Text = key.ToString();

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = Utilities.PrettyString(key.GetValue(valueName)) };
                    }
                    else if (RetainedSettings.RegistryInventory[fullPath] == null) // value was a key before and it's a value now
                    {
                        //this.registryContentsRemoved[fullPath] = null;
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = Utilities.PrettyString(key.GetValue(valueName)) };
                    }
                    else // both preinstall and postinstall inventories contain the value, check to see if it was modified
                    {
                        if ((key.GetValueKind(valueName) != RetainedSettings.RegistryInventory[fullPath].Kind) || (key.GetValue(valueName)?.ToString() != RetainedSettings.RegistryInventory[fullPath].Value))
                        {
                            this.registryContentsModified[fullPath] = new RegistryEntryDiff(RetainedSettings.RegistryInventory[fullPath],
                                                                                            new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = Utilities.PrettyString(key.GetValue(valueName)) });
                        }

                        RetainedSettings.RegistryInventory.Remove(fullPath); // remove value from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                    }
                }

                String[] subkeyNames = key.GetSubKeyNames();

                foreach (String subkeyName in subkeyNames)
                {
                    fullPath = $@"{key.Name}\{subkeyName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        this.registryContentsAdded[fullPath] = null;
                    }
                    else if (RetainedSettings.RegistryInventory[fullPath] != null) // subkey was a value before and it's a key now
                    {
                        //this.registryContentsRemoved[fullPath] = this.registryContentsPreInstall[fullPath];
                        this.registryContentsAdded[fullPath] = null;
                    }
                    else // both preinstall and postinstall inventories contain the subkey
                    {
                        RetainedSettings.RegistryInventory.Remove(fullPath);
                    }

                    if (recursive)
                    {
                        try
                        {
                            PostInstallInventoryRegistry(key.OpenSubKey(subkeyName), recursive);
                        }
                        catch (SecurityException) { }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void PostInstallInventoryServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    this.tsslStatus.Text = service.ServiceName;

                    if (!RetainedSettings.ServicesInventory.ContainsKey(service.ServiceName))
                    {
                        this.servicesAdded[service.ServiceName] = ServiceInfo.Parse(service);
                    }
                    else // both preinstall and postinstall inventories contain the service, check to see if it was modified
                    {
                        if ((service.CanPauseAndContinue != RetainedSettings.ServicesInventory[service.ServiceName].CanPauseAndContinue) ||
                            (service.CanShutdown != RetainedSettings.ServicesInventory[service.ServiceName].CanShutdown) ||
                            (service.CanStop != RetainedSettings.ServicesInventory[service.ServiceName].CanStop) ||
                            (service.DisplayName != RetainedSettings.ServicesInventory[service.ServiceName].DisplayName) ||
                            (service.ServiceType != RetainedSettings.ServicesInventory[service.ServiceName].ServiceType) ||
                            (service.StartType != RetainedSettings.ServicesInventory[service.ServiceName].StartType))
                        {
                            this.servicesModified[service.ServiceName] = new ServiceDiff(RetainedSettings.ServicesInventory[service.ServiceName],
                                                                                         ServiceInfo.Parse(service));
                        }
                        else // everything else matches, now check to see if any of the services depended on changed
                        {
                            HashSet<String> serviceNamesDependedOn = new HashSet<String>();

                            foreach (ServiceController serviceController in service.ServicesDependedOn)
                            {
                                serviceNamesDependedOn.Add(serviceController.ServiceName);
                            }

                            if (!serviceNamesDependedOn.SetEquals(RetainedSettings.ServicesInventory[service.ServiceName].ServiceNamesDependedOn))
                            {
                                this.servicesModified[service.ServiceName] = new ServiceDiff(RetainedSettings.ServicesInventory[service.ServiceName],
                                                                                             ServiceInfo.Parse(service));
                            }
                        }

                        RetainedSettings.ServicesInventory.Remove(service.ServiceName); // remove service from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bwPostInstall_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                if (this.cbFileSystemMonitor.Checked)
                {
                    this.postInstallFoldersStarted = DateTime.Now;

                    foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in RetainedSettings.FoldersToTrack)
                    {
                        PostInstallInventoryDirectory(folder.Folder, folder.IncludeSubFolders);
                    }

                    this.postInstallFoldersFinished = DateTime.Now;
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.postInstallRegistryStarted = DateTime.Now;

                    RegistryKey key;

                    foreach (RetainedSettings.RegistrySettings.TrackedKey regKey in RetainedSettings.KeysToTrack)
                    {
                        key = OpenRegistryKey(regKey.Key);

                        PostInstallInventoryRegistry(key, regKey.IncludeSubKeys);
                    }

                    this.postInstallRegistryFinished = DateTime.Now;
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.postInstallServicesStarted = DateTime.Now;

                    PostInstallInventoryServices();

                    this.postInstallServicesFinished = DateTime.Now;
                }

                GenerateIntallationReportHTML(this.sfdSaveReport.FileName);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bwPostInstall_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // the report generation only works correctly if sfdSaveReport.FileName is valid, otherwise do nothing
                if (!String.IsNullOrEmpty(this.sfdSaveReport.FileName))
                {
                    HtmlPreviewForm preview = new HtmlPreviewForm(this, this.sfdSaveReport.FileName);
                    preview.MinimumSize = this.MinimumSize;
                    // this needs to be .ShowDialog() (a blocking call) so the following RetainedSettings.Delete...() methods are not run until the form is closed (HTMLPreviewForm's 'Export as JSON' functionality requires some of the RetainedSettngs)
                    preview.ShowDialog();

                    RetainedSettings.DeleteCommonInfo();
                    RetainedSettings.DeleteFileSystemSettings();
                    RetainedSettings.DeleteRegistrySettings();
                    RetainedSettings.DeleteServicesSettings();

                    this.tsslStatus.Text = "";

                    this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bDefaultTrackedFolders.Enabled = this.bAddFolder.Enabled = true;
                    this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bDefaultTrackedKeys.Enabled = this.bAddKey.Enabled = true;
                    this.cbServicesMonitor.Enabled = true;
                    this.bPreInstall.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bwLoader_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                RetainedSettings.Initialize(SplashScreen);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void WinChangeMonitorForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.ConfirmOnClose && MessageBox.Show("Are you sure you want to close the program?", "Confirm Exit", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void exitToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void WinChangeMonitorForm_Resize(Object sender, EventArgs e)
        {
            try
            {
                AutoSizeColumns(this.olvFoldersToTrack);
                AutoSizeColumns(this.olvKeysToTrack);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvFoldersToTrack_CellClick(Object sender, CellClickEventArgs e)
        {
            try
            {
                if (e.Item != null)
                {
                    this.bRemoveFolder.Enabled = true;
                    e.Item.Selected = true;
                }
                else
                {
                    this.bRemoveFolder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvKeysToTrack_CellClick(Object sender, CellClickEventArgs e)
        {
            try
            {
                if (e.Item != null)
                {
                    this.bRemoveKey.Enabled = true;
                    e.Item.Selected = true;
                }
                else
                {
                    this.bRemoveKey.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvFoldersToTrack_SubItemChecking(Object sender, SubItemCheckingEventArgs e)
        {
            try
            {
                RetainedSettings.FoldersToTrack[e.ListViewItem.Index].IncludeSubFolders = (e.NewValue == CheckState.Checked);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvKeysToTrack_SubItemChecking(Object sender, SubItemCheckingEventArgs e)
        {
            try
            {
                RetainedSettings.KeysToTrack[e.ListViewItem.Index].IncludeSubKeys = (e.NewValue == CheckState.Checked);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvFoldersToTrack_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try
            {
                if (this.olvFoldersToTrack.SelectedIndex != -1)
                {
                    this.bRemoveFolder.Enabled = true;
                }
                else
                {
                    this.bRemoveFolder.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvKeysToTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.olvKeysToTrack.SelectedIndex != -1)
                {
                    this.bRemoveKey.Enabled = true;
                }
                else
                {
                    this.bRemoveKey.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvFoldersToTrack_Resize(Object sender, EventArgs e)
        {
            try
            {
                AutoSizeColumns(this.olvFoldersToTrack);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvFoldersToTrack_EnabledChanged(Object sender, EventArgs e)
        {
            try
            {
                this.olvFoldersToTrack.SelectedItems.Clear();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void olvKeysToTrack_EnabledChanged(Object sender, EventArgs e)
        {
            try
            {
                this.olvKeysToTrack.SelectedItems.Clear();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private Boolean LoadTrackedFoldersFromConfig(String settingName)
        {
            try
            {
                Boolean valid = false;

                try
                {
                    String trackedFolders = ConfigurationManager.AppSettings[settingName];
                    if (trackedFolders == null)
                    {
                        return false;
                    }

                    String[] folderPairs = trackedFolders.Split('|');

                    RetainedSettings.FoldersToTrack.Clear();

                    foreach (String folderPair in folderPairs)
                    {
                        String[] folderIncludeSub = folderPair.Split('?');

                        if ((folderIncludeSub.Length > 0) && !String.IsNullOrWhiteSpace(folderIncludeSub[0]))
                        {
                            if (Directory.Exists(folderIncludeSub[0]))
                            {
                                Boolean includeSubFolders = false; // default to false if not present or parsing fails

                                if (folderIncludeSub.Length > 1)
                                {
                                    try { includeSubFolders = Boolean.Parse(folderIncludeSub[1]); } catch (Exception) { } // if parsing fails, includeSubFolders is already set to false
                                }

                                RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder { Folder = folderIncludeSub[0], IncludeSubFolders = includeSubFolders });
                            }
                        }
                    }

                    valid = true;
                }
                catch (ConfigurationErrorsException)
                {
                    valid = false;
                }

                if (!valid) // handle the case with incorrectly formatted App.config
                {
                    RetainedSettings.FoldersToTrack.Clear();

                    // handle the case where user incorrectly modified App.config
                    RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder { Folder = @"C:\", IncludeSubFolders = true });
                }

                return true;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                throw;
            }
        }

        private void UpdateAppSetting(String settingName, String value)
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                AppSettingsSection appSettings = configuration.GetSection("appSettings") as AppSettingsSection;

                if (appSettings == null) // this should never be null, but check to be safe
                {
                    appSettings = new AppSettingsSection();
                    configuration.Sections.Add("appSettings", appSettings);
                }

                if (appSettings.Settings[settingName] != null)
                {
                    appSettings.Settings[settingName].Value = value; // update the existing value
                }
                else
                {
                    appSettings.Settings.Add(settingName, value); // add the new setting with given value
                }

                configuration.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void SaveTrackedFoldersToConfig(String settingName = "lastTrackedFolders")
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < RetainedSettings.FoldersToTrack.Count; ++i)
                {
                    sb.Append($"{RetainedSettings.FoldersToTrack[i].Folder}?{RetainedSettings.FoldersToTrack[i].IncludeSubFolders}");

                    // if it isn't the last tracked folder, append '|' as delimiter
                    if (i < RetainedSettings.FoldersToTrack.Count - 1)
                    {
                        sb.Append("|");
                    }
                }

                UpdateAppSetting(settingName, sb.ToString());
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private Boolean LoadTrackedKeysFromConfig(String settingName)
        {
            try
            {
                Boolean valid = false;

                try
                {
                    String trackedKeys = ConfigurationManager.AppSettings[settingName];
                    if (trackedKeys == null)
                    {
                        return false;
                    }

                    String[] keyPairs = trackedKeys.Split('|');

                    RetainedSettings.KeysToTrack.Clear();

                    foreach (String keyPair in keyPairs)
                    {
                        String[] keyIncludeSub = keyPair.Split('?');

                        if (keyIncludeSub.Length > 0)
                        {
                            if (OpenRegistryKey(keyIncludeSub[0]) != null)
                            {
                                Boolean includeSubKeys = false; // default to false if not present or parsing fails

                                if (keyIncludeSub.Length > 1)
                                {
                                    try { includeSubKeys = Boolean.Parse(keyIncludeSub[1]); } catch (Exception) { } // if parsing fails, includeSubKeys is already set to false
                                }

                                RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = keyIncludeSub[0], IncludeSubKeys = includeSubKeys });
                            }
                        }
                    }

                    valid = true;
                }
                catch (ConfigurationErrorsException)
                {
                    valid = false;
                }
                
                if (!valid)
                {
                    RetainedSettings.KeysToTrack.Clear();

                    // handle the cases with incorrectly formatted App.config or AppSettings[settingName] == null
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = HKCR, IncludeSubKeys = true });
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = HKCU, IncludeSubKeys = true });
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = HKLM, IncludeSubKeys = true });
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = HKU, IncludeSubKeys = true });
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey { Key = HKCC, IncludeSubKeys = true });
                }

                return true;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                throw;
            }
        }

        private void SaveTrackedKeysToConfig(String settingName = "lastTrackedKeys")
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < RetainedSettings.KeysToTrack.Count; ++i)
                {
                    sb.Append($"{RetainedSettings.KeysToTrack[i].Key}?{RetainedSettings.KeysToTrack[i].IncludeSubKeys}");

                    // if it isn't the last tracked folder, append '|' as delimiter
                    if (i < RetainedSettings.KeysToTrack.Count - 1)
                    {
                        sb.Append("|");
                    }
                }

                UpdateAppSetting(settingName, sb.ToString());
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bDefaultTrackedFolders_Click(Object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Delete current File System Monitor settings and use defaults instead?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LoadTrackedFoldersFromConfig("defaultTrackedFolders");
                    this.olvFoldersToTrack.ClearObjects();
                    this.olvFoldersToTrack.AddObjects(RetainedSettings.FoldersToTrack);
                    this.bRemoveFolder.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bDefaultTrackedKeys_Click(Object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Delete current Registry Monitor settings and use defaults instead?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    LoadTrackedKeysFromConfig("defaultTrackedKeys");
                    this.olvKeysToTrack.ClearObjects();
                    this.olvKeysToTrack.AddObjects(RetainedSettings.KeysToTrack);
                    this.bRemoveKey.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void GenerateIntallationReportHTML(String htmlFile)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(htmlFile, false, Encoding.UTF8))
                {
                    writer.WriteLine("<!DOCTYPE html>");
                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine("<title>WinChangeMonitor Installation Report</title>");
                    writer.WriteLine("<style>");
                    writer.WriteLine("table {");
                    writer.WriteLine("table-layout: fixed;");
                    writer.WriteLine("}");
                    writer.WriteLine("table, th, td {");
                    writer.WriteLine("border: 1px solid black;");
                    writer.WriteLine("border-collapse: collapse;");
                    writer.WriteLine("padding: 8px;");
                    writer.WriteLine("}");
                    writer.WriteLine("th {");
                    writer.WriteLine("background-color: #3AA5EB;");
                    writer.WriteLine("}");
                    writer.WriteLine("#number-td {");
                    writer.WriteLine("width: 50px");
                    writer.WriteLine("}");
                    writer.WriteLine("#name-td {");
                    writer.WriteLine("text-align: left;");
                    writer.WriteLine("}");
                    writer.WriteLine("#type-td {");
                    writer.WriteLine("width: 150px;");
                    writer.WriteLine("}");
                    writer.WriteLine("#value-td {");
                    writer.WriteLine("width: 150px;");
                    writer.WriteLine("}");
                    writer.WriteLine("td {");
                    writer.WriteLine("text-align: center;");
                    writer.WriteLine("}");
                    writer.WriteLine("</style>");
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");
                    writer.WriteLine("<h1>WinChangeMonitor - Installation Report</h1>");


                    writer.WriteLine("<table style=\"border: 0px\">");
                    writer.WriteLine("<tr>");

                    if (this.cbFileSystemMonitor.Checked)
                    {
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#file_system_added\">File System Contents Added</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#file_system_modified\">File System Contents Modified</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#file_system_removed\">File System Contents Removed</a></td>");
                    }

                    if (this.cbRegistryMonitor.Checked)
                    {
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#registry_added\">Registry Contents Added</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#registry_modified\">Registry Contents Modified</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#registry_removed\">Registry Contents Removed</a></td>");
                    }

                    if (this.cbServicesMonitor.Checked)
                    {
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#services_added\">Services Added</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#services_modified\">Services Modified</a></td>");
                        writer.WriteLine("<td style=\"border: 0px\"><a href=\"#services_removed\">Services Removed</a></td>");
                    }

                    writer.WriteLine("</tr>");
                    writer.WriteLine("</table>");


                    if (this.cbFileSystemMonitor.Checked)
                    {
                        // --- FILE SYSTEM INVENTORY ---
                        writer.WriteLine("<h2>File System Inventory</h2>");

                        writer.WriteLine("<h3>Tracked Folders</h3>");
                        writer.WriteLine("<table>");
                        writer.WriteLine("<tr>");
                        writer.WriteLine("<th>Folder</th>");
                        writer.WriteLine("<th>Include Sub-Folders?</th>");
                        writer.WriteLine("</tr>");
                        foreach (RetainedSettings.FileSystemSettings.TrackedFolder trackedFolder in RetainedSettings.FoldersToTrack)
                        {
                            writer.WriteLine("<tr>");
                            writer.WriteLine($"<td>{trackedFolder.Folder}</td>");
                            writer.WriteLine($"<td>{(trackedFolder.IncludeSubFolders ? "Yes" : "No")}</td>");
                            writer.WriteLine("</tr>");
                        }
                        writer.WriteLine("</table>");

                        writer.WriteLine("<h3 id=\"file_system_added\">File System Contents Added</h3>");

                        if (this.folderContentsAdded.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was added</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"value-td\">Folder/File</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, Boolean> addedItem in this.folderContentsAdded)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{WebUtility.HtmlEncode(addedItem.Key)}</td>");
                                writer.WriteLine($"<td>{(addedItem.Value ? "Folder" : "File")}</td>");
                                writer.WriteLine("</tr>");
                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"file_system_modified\">File System Contents Modified</h3>");

                        if (this.folderContentsModified.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was modified</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"value-td\">Folder/File</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, Boolean> modifiedItem in this.folderContentsModified)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{WebUtility.HtmlEncode(modifiedItem.Key)}</td>");
                                writer.WriteLine($"<td>{(modifiedItem.Value ? "Folder" : "File")}</td>");
                                writer.WriteLine("</tr>");
                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"file_system_removed\">File System Contents Removed</h3>");

                        if (RetainedSettings.FileSystemInventory.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was removed</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"value-td\">Folder/File</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, RetainedSettings.FileSystemSettings.FileSystemEntryInfo> removedItem in RetainedSettings.FileSystemInventory)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\"><i>{WebUtility.HtmlEncode(removedItem.Key)}</td>");
                                writer.WriteLine($"<td><i>{(removedItem.Value.IsFolder ? "Folder" : "File")}</i></td>");
                                writer.WriteLine("</tr>");
                            }
                            writer.WriteLine("</table>");
                        }
                    }

                    if (this.cbRegistryMonitor.Checked)
                    {
                        // --- REGISTRY INVENTORY ---
                        writer.WriteLine("<h2>Registry Inventory</h2>");

                        writer.WriteLine("<h3>Tracked Keys</h3>");
                        writer.WriteLine("<table>");
                        writer.WriteLine("<tr>");
                        writer.WriteLine("<th>Key</th>");
                        writer.WriteLine("<th>Include Sub-Keys?</th>");
                        writer.WriteLine("</tr>");
                        foreach (RetainedSettings.RegistrySettings.TrackedKey trackedKey in RetainedSettings.KeysToTrack)
                        {
                            writer.WriteLine("<tr>");
                            writer.WriteLine($"<td>{trackedKey.Key}</td>");
                            writer.WriteLine($"<td>{(trackedKey.IncludeSubKeys ? "Yes" : "No")}</td>");
                            writer.WriteLine("</tr>");
                        }
                        writer.WriteLine("</table>");

                        writer.WriteLine("<h3 id=\"registry_added\">Registry Contents Added</h3>");

                        if (this.registryContentsAdded.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was added</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"type-td\">Type</th>");
                            writer.WriteLine("<th id=\"value-td\">Value</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, RegistryEntryInfo> addedItem in this.registryContentsAdded)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{WebUtility.HtmlEncode(addedItem.Key)}</td>");

                                if (addedItem.Value == null)
                                {
                                    writer.WriteLine("<td>Key</td>");
                                    writer.WriteLine("<td></td>");
                                }
                                else
                                {
                                    writer.WriteLine($"<td>{WebUtility.HtmlEncode(translateRegKind[addedItem.Value.Kind])}</td>");
                                    writer.WriteLine("<td>");
                                    writer.WriteLine(Utilities.HandleValueWithZeroDelimiter(addedItem.Value.Value));
                                    writer.WriteLine("</td>");
                                }

                                writer.WriteLine("</tr>");
                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"registry_modified\">Registry Contents Modified</h3>");

                        if (this.registryContentsModified.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was modified</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"type-td\">Type</th>");
                            writer.WriteLine("<th id=\"value-td\">Value</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, RegistryEntryDiff> modifiedItem in this.registryContentsModified)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td rowspan=\"2\">{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{WebUtility.HtmlEncode(modifiedItem.Key)}</td>");

                                if (modifiedItem.Value == null) // this isn't really possible to modify a registry key (renaming it would report the original key as deleted and the renamed key as new)
                                {
                                    writer.WriteLine("<td>Key</td>");
                                    writer.WriteLine("<td></td>");
                                }
                                else
                                {  
                                    writer.WriteLine($"<td>{WebUtility.HtmlEncode(translateRegKind[modifiedItem.Value.Current.Kind])}</td>");
                                    writer.WriteLine($"<td>{WebUtility.HtmlEncode(modifiedItem.Value.Current.Value)}</td>");
                                    writer.WriteLine("</tr>");
                                    writer.WriteLine("<tr>");
                                    writer.WriteLine("<td><i>(original if different)</i></td>");
                                    writer.WriteLine("<td>");
                                    if (modifiedItem.Value.Current.Kind != modifiedItem.Value.Initial.Kind)
                                    {
                                        writer.WriteLine($"<i>{WebUtility.HtmlEncode(translateRegKind[modifiedItem.Value.Initial.Kind])}</i>");
                                    }
                                    writer.WriteLine("</td>");
                                    writer.WriteLine("<td>");
                                    if (modifiedItem.Value.Current.Value != modifiedItem.Value.Initial.Value)
                                    {
                                        writer.WriteLine("<i>");
                                        writer.WriteLine(Utilities.HandleValueWithZeroDelimiter(modifiedItem.Value.Initial.Value));
                                        writer.WriteLine("</i>");
                                    }
                                    writer.WriteLine("</td>");
                                }

                                writer.WriteLine("</tr>");
                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"registry_removed\">Registry Contents Removed</h3>");

                        if (RetainedSettings.RegistryInventory.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was removed</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th>Name</th>");
                            writer.WriteLine("<th id=\"type-td\">Type</th>");
                            writer.WriteLine("<th id=\"value-td\">Value</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, RegistryEntryInfo> removedItem in RetainedSettings.RegistryInventory)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\"><i>{WebUtility.HtmlEncode(removedItem.Key)}</i></td>");

                                if (removedItem.Value == null)
                                {
                                    writer.WriteLine($"<td><i>Key</i></td>");
                                    writer.WriteLine($"<td></td>");
                                }
                                else
                                {
                                    writer.WriteLine($"<td><i>{WebUtility.HtmlEncode(translateRegKind[removedItem.Value.Kind])}</i></td>");
                                    writer.WriteLine("<td><i>");
                                    writer.WriteLine(Utilities.HandleValueWithZeroDelimiter(removedItem.Value.Value));
                                    writer.WriteLine($"</i></td>");
                                }

                                writer.WriteLine("</tr>");
                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }
                    }

                    if (this.cbServicesMonitor.Checked)
                    {
                        // --- SERVICES INVENTORY ---
                        writer.WriteLine("<h2>Services Inventory</h2>");

                        writer.WriteLine("<h3 id=\"services_added\">Services Added</h3>");

                        if (this.servicesAdded.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was added</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th id=\"value-td\">Service</th>");
                            writer.WriteLine("<th id=\"value-td\">CanPauseAndContinue</th>");
                            writer.WriteLine("<th id=\"value-td\">CanShutdown</th>");
                            writer.WriteLine("<th id=\"value-td\">CanStop</th>");
                            writer.WriteLine("<th>DisplayName</th>");
                            writer.WriteLine("<th id=\"value-td\">ServicesDependedOn</th>");
                            writer.WriteLine("<th id=\"value-td\">ServiceType</th>");
                            writer.WriteLine("<th id=\"value-td\">StartType</th>");
                            writer.WriteLine("</tr>");

                            UInt64 count = 1;
                            foreach (KeyValuePair<String, ServiceInfo> addedItem in this.servicesAdded)
                            {
                                writer.WriteLine("<tr>");
                                
                                ServiceInfo s = addedItem.Value; // allows "s." to get properties instead of "addedItem.Value."
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{addedItem.Key}</td>");
                                writer.WriteLine($"<td>{s.CanPauseAndContinue}</td>");
                                writer.WriteLine($"<td>{s.CanShutdown}</td>");
                                writer.WriteLine($"<td>{s.CanStop}</td>");
                                writer.WriteLine($"<td>{s.DisplayName}</td>");
                                writer.WriteLine("<td>");
                                foreach (String serviceNameDependedOn in s.ServiceNamesDependedOn)
                                {
                                    writer.WriteLine($"<div>{serviceNameDependedOn}</div>");
                                }
                                writer.WriteLine("</td>");
                                writer.WriteLine($"<td>{s.StartType}</td>");
                                writer.WriteLine("</tr>");

                                ++count;
                            }

                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"services_modified\">Services Modified</h3>");

                        if (this.servicesModified.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was modified</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th id=\"value-td\">Service</th>");
                            writer.WriteLine("<th id=\"value-td\">CanPauseAndContinue</th>");
                            writer.WriteLine("<th id=\"value-td\">CanShutdown</th>");
                            writer.WriteLine("<th id=\"value-td\">CanStop</th>");
                            writer.WriteLine("<th>DisplayName</th>");
                            writer.WriteLine("<th id=\"value-td\">ServicesDependedOn</th>");
                            writer.WriteLine("<th id=\"value-td\">ServiceType</th>");
                            writer.WriteLine("<th id=\"value-td\">StartType</th>");
                            writer.WriteLine("</tr>");
                            UInt64 count = 1;
                            foreach (KeyValuePair<String, ServiceDiff> modifiedItem in this.servicesModified)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td rowspan=\"2\">{count}</td>");
                                writer.WriteLine($"<td id=\"name-td\">{modifiedItem.Key}</td>");
                                ServiceInfo curr = modifiedItem.Value.Current; // allows "curr." to get Current properties instead of "modifiedItem.Value.Current."
                                writer.WriteLine($"<td>{curr.CanPauseAndContinue}</td>");
                                writer.WriteLine($"<td>{curr.CanShutdown}</td>");
                                writer.WriteLine($"<td>{curr.CanStop}</td>");
                                writer.WriteLine($"<td>{curr.DisplayName}</td>");
                                writer.WriteLine("<td>");
                                foreach (String serviceNameDependedOn in curr.ServiceNamesDependedOn)
                                {
                                    writer.WriteLine($"<div>{serviceNameDependedOn}</div>");
                                }
                                writer.WriteLine("</td>");
                                writer.WriteLine($"<td>{curr.ServiceType}</td>");
                                writer.WriteLine($"<td>{curr.StartType}</td>");
                                writer.WriteLine("</tr>");
                                writer.WriteLine($"<tr>");
                                writer.WriteLine("<td><i>(original if different)</i></td>");
                                ServiceInfo init = modifiedItem.Value.Initial; //allows "init." to get Initial properties instead of "modifiedItem.Value.Initial."
                                writer.WriteLine($"<td><i>{(curr.CanPauseAndContinue != init.CanPauseAndContinue ? init.CanPauseAndContinue.ToString() : "")}</i></td>");
                                writer.WriteLine($"<td><i>{(curr.CanShutdown != init.CanShutdown ? init.CanShutdown.ToString() : "")}</i></td>");
                                writer.WriteLine($"<td><i>{(curr.CanStop != init.CanStop ? init.CanStop.ToString() : "")}</i></td>");
                                writer.WriteLine($"<td><i>{(curr.DisplayName != init.DisplayName ? init.DisplayName : "")}</i></td>");
                                writer.WriteLine("<td>");
                                if (!curr.ServiceNamesDependedOn.SetEquals(init.ServiceNamesDependedOn))
                                {
                                    foreach(String serviceNameDependedOn in init.ServiceNamesDependedOn)
                                    {
                                        writer.WriteLine($"<div><i>{serviceNameDependedOn}</i></div>");
                                    }
                                }
                                writer.WriteLine("</td>");
                                writer.WriteLine($"<td><i>{(curr.ServiceType != init.ServiceType ? init.ServiceType.ToString() : "")}</i></td>");
                                writer.WriteLine($"<td><i>{(curr.StartType != init.StartType ? init.StartType.ToString() : "")}</i></td>");
                                writer.WriteLine("</tr>");

                                ++count;
                            }
                            writer.WriteLine("</table>");
                        }

                        writer.WriteLine("<h3 id=\"services_removed\">Services Removed</h3>");

                        if (RetainedSettings.ServicesInventory.Count == 0)
                        {
                            writer.WriteLine("<div>Nothing was removed</div>");
                        }
                        else
                        {
                            writer.WriteLine("<table width=\"100%\">");
                            writer.WriteLine("<tr>");
                            writer.WriteLine("<th id=\"number-td\">#</th>");
                            writer.WriteLine("<th id=\"value-td\">Service</th>");
                            writer.WriteLine("<th id=\"value-td\">CanPauseAndContinue</th>");
                            writer.WriteLine("<th id=\"value-td\">CanShutdown</th>");
                            writer.WriteLine("<th id=\"value-td\">CanStop</th>");
                            writer.WriteLine("<th>DisplayName</th>");
                            writer.WriteLine("<th id=\"value-td\">ServicesDependedOn</th>");
                            writer.WriteLine("<th id=\"value-td\">ServiceType</th>");
                            writer.WriteLine("<th id=\"value-td\">StartType</th>");
                            writer.WriteLine("</tr>");

                            UInt64 count = 1;
                            foreach (KeyValuePair<String, ServiceInfo> removedItem in RetainedSettings.ServicesInventory)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine($"<td>{count}</td>");

                                ServiceInfo s = removedItem.Value; // allows "s." to get properties instead of "removedItem.Value."
                                writer.WriteLine($"<td id=\"name-td\"><i>{removedItem.Key}</i></td>");
                                writer.WriteLine($"<td><i>{s.CanPauseAndContinue}</i></td>");
                                writer.WriteLine($"<td><i>{s.CanShutdown}</i></td>");
                                writer.WriteLine($"<td><i>{s.CanStop}</i></td>");
                                writer.WriteLine($"<td><i>{s.DisplayName}</i></td>");
                                writer.WriteLine("<td>");
                                foreach (String serviceNameDependedOn in s.ServiceNamesDependedOn)
                                {
                                    writer.WriteLine($"<div><i>{serviceNameDependedOn}</i></div>");
                                }
                                writer.WriteLine("</td>");
                                writer.WriteLine($"<td><i>{s.ServiceType}</i></td>");
                                writer.WriteLine($"<td><i>{s.StartType}</i></td>");
                                writer.WriteLine("</tr>");

                                ++count;
                            }

                            writer.WriteLine("</table>");
                        }
                    }

                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void startFreshToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete the existing inventory?", "Delete Inventory?", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                RetainedSettings.DeleteCommonInfo();

                RetainedSettings.DeleteFileSystemSettings();
                foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in this.olvFoldersToTrack.Objects)
                {
                    RetainedSettings.FoldersToTrack.Add(folder);
                }

                RetainedSettings.DeleteRegistrySettings();
                foreach (RetainedSettings.RegistrySettings.TrackedKey key in this.olvKeysToTrack.Objects)
                {
                    RetainedSettings.KeysToTrack.Add(key);
                }

                RetainedSettings.DeleteServicesSettings();

                this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bDefaultTrackedFolders.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bDefaultTrackedKeys.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
                this.cbServicesMonitor.Enabled = true;

                this.bPreInstall.Enabled = true;
                this.bPostInstall.Enabled = this.tsmiStartFresh.Enabled = false;
            }
        }

        private void tStatus_Tick(Object sender, EventArgs e)
        {
            try
            {
                if (this.operation != null)
                {
                    if (this.current != null)
                    {
                        this.tsslStatus.Text = $"{this.operation} - {this.current}";
                    }
                    else // gracefully handle the case where current isn't set
                    {
                        this.tsslStatus.Text = this.operation;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bwLoader_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.loadFinished = DateTime.Now;

                this.Location = SplashScreen.Location;
                this.WindowState = SplashScreen.WindowState;

                this.Show();

                SplashScreen.Hide();

                this.tsslStatus.Text = "";

                if ((RetainedSettings.PreInstallFileSystemFinished == null) && (RetainedSettings.PreInstallRegistryFinished == null) && (RetainedSettings.PreInstallServicesFinished == null))
                {
                    if (!LoadTrackedFoldersFromConfig("lastTrackedFolders"))
                    {
                        LoadTrackedFoldersFromConfig("defaultTrackedFolders");
                    }
                    if (!LoadTrackedKeysFromConfig("lastTrackedKeys"))
                    {
                        LoadTrackedKeysFromConfig("defaultTrackedKeys");
                    }

                    this.cbFileSystemMonitor.Checked = true;
                    this.olvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = true;
                    this.cbRegistryMonitor.Checked = true;
                    this.olvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = true;
                    this.cbServicesMonitor.Checked = true;

                    this.bPreInstall.Enabled = true;
                    this.bPostInstall.Enabled = false;
                    this.tsmiStartFresh.Enabled = false;
                }
                else // a previous inventory exists
                {
                    // the following IF block could be condensed to one line, but it makes the code harder for a human to read
                    if (RetainedSettings.PreInstallFileSystemFinished != null)
                    {
                        this.cbFileSystemMonitor.Checked = true; 
                    }
                    else
                    {
                        this.cbFileSystemMonitor.Checked = false;
                    }
                    this.cbFileSystemMonitor.Enabled = false;
                    this.olvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;

                    // the following IF block could be condensed to one line, but it makes the code harder for a human to read
                    if (RetainedSettings.PreInstallRegistryFinished != null)
                    {
                        this.cbRegistryMonitor.Checked = true;
                    }
                    else
                    {
                        this.cbRegistryMonitor.Checked = false;
                    }
                    this.cbRegistryMonitor.Enabled = false;
                    this.olvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;

                    // the following IF block could be condensed to one line, but it makes the code harder for a human to read
                    if (RetainedSettings.PreInstallServicesFinished != null)
                    {
                        this.cbServicesMonitor.Checked = true;
                    }
                    else
                    {
                        this.cbServicesMonitor.Checked = false;
                    }
                    this.cbServicesMonitor.Enabled = false;

                    this.bPreInstall.Enabled = false;
                    this.bPostInstall.Enabled = true;
                    this.tsmiStartFresh.Enabled = true;
                }

                this.olvFoldersToTrack.AddObjects(RetainedSettings.FoldersToTrack);
                this.olvKeysToTrack.AddObjects(RetainedSettings.KeysToTrack);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }

    public class CustomRenderer : ToolStripRenderer
    {
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is ToolStripStatusLabel)
            {
                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, e.TextColor, Color.Transparent, e.TextFormat | TextFormatFlags.EndEllipsis);
            }
            else
            {
                base.OnRenderItemText(e);
            }
        }
    }

    public class CustomToolStripProfessionalRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            try
            {
                if (e.Item.Enabled)
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            try
            {
                if (e.Item.Enabled)
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }

}
