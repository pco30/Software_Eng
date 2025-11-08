using BrightIdeasSoftware;
using JCS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class WinChangeMonitorForm : Form
    {
        private DateTime? preInstallFoldersStarted = null;
        private DateTime? preInstallRegistryStarted = null;
        private DateTime? preInstallServicesStarted = null;
        private SortedDictionary<String, Boolean> folderContentsAdded = new SortedDictionary<String, Boolean>();
        private SortedDictionary<String, Boolean> folderContentsModified = new SortedDictionary<String, Boolean>();
        private SortedDictionary<String, RegistryEntryInfo> registryContentsAdded = new SortedDictionary<String, RegistryEntryInfo>();
        private SortedDictionary<String, RegistryEntryDiff> registryContentsModified = new SortedDictionary<String, RegistryEntryDiff>();
        private SortedDictionary<String, ServiceInfo> servicesAdded = new SortedDictionary<String, ServiceInfo>();
        private SortedDictionary<String, ServiceDiff> servicesModified = new SortedDictionary<String, ServiceDiff>();
        private FolderBrowserDialog fbdAddFolder = new FolderBrowserDialog();
        private RegistryKeyBrowserDialog rkbdAddKey = new RegistryKeyBrowserDialog();
        private DateTime? postInstallFoldersStarted = null;
        private DateTime? postInstallFoldersFinished = null;
        private DateTime? postInstallRegistryStarted = null;
        private DateTime? postInstallRegistryFinished = null;
        private DateTime? postInstallServicesStarted = null;
        private DateTime? postInstallServicesFinished = null;
        private String operation, current;

        public static SplashScreenForm SplashScreen { get; private set; }

        public Boolean ConfirmOnClose { get; set; } = true;

        private const String HKCR = "HKEY_CLASSES_ROOT";
        private const String HKCU = "HKEY_CURRENT_USER";
        private const String HKLM = "HKEY_LOCAL_MACHINE";
        private const String HKU = "HKEY_USERS";
        private const String HKCC = "HKEY_CURRENT_CONFIG";

        private static Color ToggleSwitchOnColor = Color.Green;

        public WinChangeMonitorForm()
        {
            try
            {
                InitializeComponent();

                this.cbFileSystemMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });
                this.cbRegistryMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });
                this.cbServicesMonitor.SetRenderer(new ToggleSwitchIphoneRenderer() { LeftSideBackColor1 = ToggleSwitchOnColor });

                this.statusStrip1.Renderer = new CustomRenderer();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private DateTime loadStarted, loadFinished;

        private void AutoSizeColumns(ObjectListView olv)
        {
            try
            {
                if (olv.Columns.Count > 0)
                {
                    int colWidth = olv.ClientSize.Width / olv.Columns.Count;

                    foreach (OLVColumn column in olv.Columns)
                    {
                        column.Width = colWidth;
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
                    this.bPostInstall.Enabled = this.bStartFresh.Enabled = false;

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
                        RetainedSettings.RegistryInventory[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = key.GetValue(valueName)?.ToString() };
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

                Utilities.TextBoxClear(this.tbOutput);

                if (this.cbFileSystemMonitor.Checked)
                {   
                    this.preInstallFoldersStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Started @ {this.preInstallFoldersStarted}{Environment.NewLine}");

                    this.operation = "Performing File System Inventory";

                    foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in RetainedSettings.FoldersToTrack)
                    {
                        PreInstallInventoryDirectory(folder.Folder, folder.IncludeSubFolders);
                    }

                    RetainedSettings.PreInstallFileSystemFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Finished @ {RetainedSettings.PreInstallFileSystemFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {RetainedSettings.PreInstallFileSystemFinished - this.preInstallFoldersStarted}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Inventoried {RetainedSettings.FileSystemInventory.Keys.Count.ToString("N0")} items{Environment.NewLine}");

                    RetainedSettings.SaveFileSystemSettings();
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.preInstallRegistryStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install Registry Inventory Started @ {this.preInstallRegistryStarted}{Environment.NewLine}");

                    RegistryKey key;

                    this.operation = "Performing Registry Inventory";

                    foreach (RetainedSettings.RegistrySettings.TrackedKey registryKey in RetainedSettings.KeysToTrack)
                    {
                        key = OpenRegistryKey(registryKey.Key);

                        PreInstallInventoryRegistry(key, registryKey.IncludeSubKeys);
                    }

                    RetainedSettings.PreInstallRegistryFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install Registry Inventory Finished @ {RetainedSettings.PreInstallRegistryFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {RetainedSettings.PreInstallRegistryFinished - this.preInstallRegistryStarted}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Inventoried {RetainedSettings.RegistryInventory.Keys.Count.ToString("N0")} items{Environment.NewLine}");

                    RetainedSettings.SaveRegistrySettings();
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.preInstallServicesStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install Services Inventory Started @ {this.preInstallServicesStarted}{Environment.NewLine}");

                    this.operation = "Performing Services Inventory";

                    PreInstallInventoryServices();

                    RetainedSettings.PreInstallServicesFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install Services Inventory Finished @ {RetainedSettings.PreInstallServicesFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {RetainedSettings.PreInstallServicesFinished - this.preInstallServicesStarted}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Inventoried {RetainedSettings.ServicesInventory.Keys.Count.ToString("N0")} items{Environment.NewLine}");

                    RetainedSettings.SaveServicesSettings();
                }

                RetainedSettings.SaveCommonInfo();


                this.tsslStatus.Text = "";
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

                this.bPostInstall.Enabled = this.bStartFresh.Enabled = true;

                this.tsslStatus.Text = "Current Folder/Key/Service Displayed Here";
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
                this.bPostInstall.Enabled = false;

                this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
                this.cbServicesMonitor.Enabled = false;
                this.bPreInstall.Enabled = this.bStartFresh.Enabled = false;

                this.bwPostInstall.RunWorkerAsync();
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
                    RetainedSettings.FoldersToTrack.RemoveAt(this.olvFoldersToTrack.SelectedIndex);

                    this.olvFoldersToTrack.ClearObjects();
                    this.olvFoldersToTrack.AddObjects(RetainedSettings.FoldersToTrack);

                    AutoSizeColumns(this.olvFoldersToTrack);

                    if (RetainedSettings.FoldersToTrack.Count == 0)
                    {
                        this.bRemoveFolder.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbRegistryMonitor.Checked && RetainedSettings.KeysToTrack.Count > 0) || this.cbServicesMonitor.Checked;
                    }

                    this.olvFoldersToTrack.SelectedItems.Clear();
                    this.bRemoveFolder.Enabled = false;
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
                    RetainedSettings.KeysToTrack.RemoveAt(this.olvKeysToTrack.SelectedIndex);

                    this.olvKeysToTrack.ClearObjects();
                    this.olvKeysToTrack.AddObjects(RetainedSettings.KeysToTrack);

                    AutoSizeColumns(this.olvKeysToTrack);

                    if (RetainedSettings.KeysToTrack.Count == 0)
                    {
                        this.bRemoveKey.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked && RetainedSettings.FoldersToTrack.Count > 0) || this.cbServicesMonitor.Checked;
                    }

                    this.olvKeysToTrack.SelectedItems.Clear();
                    this.bRemoveKey.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bStartFresh_Click(Object sender, EventArgs e)
        {
            try
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
                    foreach( RetainedSettings.RegistrySettings.TrackedKey key in this.olvKeysToTrack.Objects)
                    {
                        RetainedSettings.KeysToTrack.Add(key);
                    }

                    RetainedSettings.DeleteServicesSettings();

                    this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bDefaultTrackedFolders.Enabled =  this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                    this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bDefaultTrackedKeys.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
                    this.cbServicesMonitor.Enabled = true;

                    this.bPreInstall.Enabled = true;
                    this.bPostInstall.Enabled = this.bStartFresh.Enabled = false;

                    this.tbOutput.Clear();
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

                            if (!RetainedSettings.FileSystemInventory.Remove(file)) // remove file from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            {
                                Utilities.TextBoxAppendText(this.tbOutput, $"ERROR: FileSystemInventory.Remove(file [\"{file}\"]) FAILED{Environment.NewLine}");
                            }
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

                            if (!RetainedSettings.FileSystemInventory.Remove(subDirectory)) // remove directory from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            {
                                Utilities.TextBoxAppendText(this.tbOutput, $"ERROR: FileSystemInventory.Remove(subDirectory [\"{subDirectory}\"]) FAILED{Environment.NewLine}");
                            }
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
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = key.GetValue(valueName)?.ToString() };
                    }
                    else if (RetainedSettings.RegistryInventory[fullPath] == null) // value was a key before and it's a value now
                    {
                        //this.registryContentsRemoved[fullPath] = null;
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = key.GetValue(valueName)?.ToString() };
                    }
                    else // both preinstall and postinstall inventories contain the value, check to see if it was modified
                    {
                        if ((key.GetValueKind(valueName) != RetainedSettings.RegistryInventory[fullPath].Kind) || (key.GetValue(valueName)?.ToString() != RetainedSettings.RegistryInventory[fullPath].Value))
                        {
                            this.registryContentsModified[fullPath] = new RegistryEntryDiff(RetainedSettings.RegistryInventory[fullPath],
                                                                                            new RegistryEntryInfo { Kind = key.GetValueKind(valueName), Value = key.GetValue(valueName)?.ToString() });
                        }

                        if (!RetainedSettings.RegistryInventory.Remove(fullPath)) // remove value from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"ERROR: RegistryInventory.Remove(valueName [\"{fullPath}\"]) FAILED{Environment.NewLine}");
                        }
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
                        if (!RetainedSettings.RegistryInventory.Remove(fullPath))
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"ERROR: RegistryInventory.Remove(subkeyName [\"{fullPath}\"]) FAILED{Environment.NewLine}");
                        }
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
                            (service.StartType != RetainedSettings.ServicesInventory[service.ServiceName].StartType) ||
                            (service.ServicesDependedOn.Length != RetainedSettings.ServicesInventory[service.ServiceName].ServiceNamesDependedOn.Count))
                        {
                            this.servicesModified[service.ServiceName] = new ServiceDiff(RetainedSettings.ServicesInventory[service.ServiceName],
                                                                                         ServiceInfo.Parse(service));
                        }
                        else // everything else matches, now check to see if any of the services depended on changed
                        {
                            Boolean changed = false;

                            for (Int32 i = 0; !changed && (i < service.ServicesDependedOn.Length); ++i)
                            {
                                if (service.ServicesDependedOn[i].ServiceName != RetainedSettings.ServicesInventory[service.ServiceName].ServiceNamesDependedOn[i])
                                {
                                    changed = true;
                                }
                            }

                            if (changed)
                            {
                                this.servicesModified[service.ServiceName] = new ServiceDiff(RetainedSettings.ServicesInventory[service.ServiceName],
                                                                                             ServiceInfo.Parse(service));
                            }
                        }

                        if (!RetainedSettings.ServicesInventory.Remove(service.ServiceName)) // remove service from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"ERROR: ServicesInventory.Remove(\"{service.ServiceName}\") FAILED{Environment.NewLine}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private const Int32 TotalPaddedSize = 8;

        private void bwPostInstall_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                //TextBoxClear(this.tbOutput);

                Int32 count;

                if (this.cbFileSystemMonitor.Checked)
                {
                    this.postInstallFoldersStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install File/Folder Inventory Started @ {this.postInstallFoldersStarted}{Environment.NewLine}");

                    foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in RetainedSettings.FoldersToTrack)
                    {
                        PostInstallInventoryDirectory(folder.Folder, folder.IncludeSubFolders);
                    }

                    this.postInstallFoldersFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install File/Folder Inventory Finished @ {this.postInstallFoldersFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.postInstallFoldersFinished - this.postInstallFoldersStarted}{Environment.NewLine}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.folderContentsAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.folderContentsAdded.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.folderContentsAdded[key]}{Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.folderContentsModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.folderContentsModified.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.folderContentsModified[key]}{Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{RetainedSettings.FileSystemInventory.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String key in RetainedSettings.FileSystemInventory.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {RetainedSettings.FileSystemInventory[key]}{Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, Environment.NewLine);
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.postInstallRegistryStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install Registry Inventory Started @ {this.postInstallRegistryStarted}{Environment.NewLine}");

                    RegistryKey key;

                    foreach (RetainedSettings.RegistrySettings.TrackedKey regKey in RetainedSettings.KeysToTrack)
                    {

                        key = OpenRegistryKey(regKey.Key);

                        PostInstallInventoryRegistry(key, regKey.IncludeSubKeys);
                    }

                    this.postInstallRegistryFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install Registry Inventory Finished @ {this.postInstallRegistryFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.postInstallRegistryFinished - this.postInstallRegistryStarted}{Environment.NewLine}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.registryContentsAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in this.registryContentsAdded.Keys)
                    {
                        if (this.registryContentsAdded[entry] == null)
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = null{Environment.NewLine}");
                        }
                        else
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = ({this.registryContentsAdded[entry]}{Environment.NewLine}");
                        }
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.registryContentsModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in this.registryContentsModified.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = {this.registryContentsModified[entry]}{Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{RetainedSettings.RegistryInventory.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in RetainedSettings.RegistryInventory.Keys)
                    {
                        if (RetainedSettings.RegistryInventory[entry] == null)
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = null{Environment.NewLine}");
                        }
                        else
                        {
                            Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = ({RetainedSettings.RegistryInventory[entry]}){Environment.NewLine}");
                        }
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, Environment.NewLine);
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.postInstallServicesStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install Services Inventory Started @ {this.postInstallServicesStarted}{Environment.NewLine}");

                    PostInstallInventoryServices();

                    this.postInstallServicesFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Post-Install Services Inventory Finished @ {this.postInstallServicesFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.servicesAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.servicesAdded.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = ({this.servicesAdded[key]}){Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{this.servicesModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.servicesModified.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.servicesModified[key]}{Environment.NewLine}");
                    }

                    Utilities.TextBoxAppendText(this.tbOutput, $"{RetainedSettings.ServicesInventory.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String key in RetainedSettings.ServicesInventory.Keys)
                    {
                        Utilities.TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = ({RetainedSettings.ServicesInventory[key]}");
                    }
                }

                this.tsslStatus.Text = "";
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
                RetainedSettings.DeleteCommonInfo();
                RetainedSettings.DeleteFileSystemSettings();
                RetainedSettings.DeleteRegistrySettings();
                RetainedSettings.DeleteServicesSettings();

                this.cbFileSystemMonitor.Enabled = this.olvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                this.cbRegistryMonitor.Enabled = this.olvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
                this.cbServicesMonitor.Enabled = true;
                this.bPreInstall.Enabled = true;
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

        private void LoadTrackedFoldersFromConfig(String settingName)
        {
            try
            {
                Boolean valid = false;

                try
                {
                    String trackedFolders = ConfigurationManager.AppSettings[settingName];
                    if (trackedFolders != null)
                    {
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
                }
                catch (ConfigurationErrorsException)
                {
                    valid = false;
                }

                if (!valid) // handle the cases with incorrectly formatted App.config or AppSettings[settingName] == null
                {
                    RetainedSettings.FoldersToTrack.Clear();

                    // handle the case where user incorrectly modified App.config
                    RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder { Folder = @"C:\", IncludeSubFolders = true });
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
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

        private void LoadTrackedKeysFromConfig(String settingName)
        {
            try
            {
                Boolean valid = false;

                try
                {
                    String trackedKeys = ConfigurationManager.AppSettings[settingName];
                    if (trackedKeys != null)
                    {
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
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
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
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
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

                this.tsslStatus.Text = "Current Folder/Key/Service Displayed Here";

                if ((RetainedSettings.PreInstallFileSystemFinished == null) && (RetainedSettings.PreInstallRegistryFinished == null) && (RetainedSettings.PreInstallServicesFinished == null))
                {
                    LoadTrackedFoldersFromConfig("lastTrackedFolders");
                    LoadTrackedKeysFromConfig("lastTrackedKeys");

                    this.cbFileSystemMonitor.Checked = true;
                    this.olvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = true;
                    this.cbRegistryMonitor.Checked = true;
                    this.olvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = true;
                    this.cbServicesMonitor.Checked = true;

                    this.bPreInstall.Enabled = true;
                }
                else // a previous inventory exists
                {
                    this.tbOutput.AppendText($"Loaded in {this.loadFinished - this.loadStarted}{Environment.NewLine}");
                    this.tbOutput.AppendText($"{RetainedSettings.FileSystemInventory.Keys.Count.ToString("N0")} file system items{Environment.NewLine}");
                    this.tbOutput.AppendText($"{RetainedSettings.RegistryInventory.Keys.Count.ToString("N0")} registry items{Environment.NewLine}");
                    this.tbOutput.AppendText($"{RetainedSettings.ServicesInventory.Keys.Count.ToString("N0")} services items{Environment.NewLine}");

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
                    this.bStartFresh.Enabled = true;
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
}
