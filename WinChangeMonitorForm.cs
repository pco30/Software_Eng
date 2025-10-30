using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.ServiceProcess;
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

        public static SplashScreenForm SplashScreen { get; private set; } = new SplashScreenForm();

        private static String HKCR = "HKEY_CLASSES_ROOT";
        private static String HKCU = "HKEY_CURRENT_USER";
        private static String HKLM = "HKEY_LOCAL_MACHINE";
        private static String HKU = "HKEY_USERS";
        private static String HKCC = "HKEY_CURRENT_CONFIG";

        public WinChangeMonitorForm()
        {
            try
            {
                InitializeComponent();
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
                this.MinimumSize = this.Size;

                this.fbdAddFolder.ShowNewFolderButton = false;
                this.fbdAddFolder.RootFolder = Environment.SpecialFolder.MyComputer;

                SplashScreen.Show(this);

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
                this.bPreInstall.Enabled = false;

                this.cbFileSystemMonitor.Enabled = this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                this.cbRegistryMonitor.Enabled = this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
                this.cbServicesMonitor.Enabled = false;
                this.bPostInstall.Enabled = this.bStartFresh.Enabled = false;

                this.bwPreInstall.RunWorkerAsync();
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
                Utilities.ControlSetText(this.lStatus, directory);

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        RetainedSettings.FileSystemInventory[file] = false;
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        RetainedSettings.FileSystemInventory[subDirectory] = true;

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
                Utilities.ControlSetText(this.lStatus, key.ToString());

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        RetainedSettings.RegistryInventory[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
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
                    Utilities.ControlSetText(this.lStatus, service.ServiceName);

                    if (!RetainedSettings.ServicesInventory.ContainsKey(service.ServiceName))
                    {
                        RetainedSettings.ServicesInventory[service.ServiceName] = new ServiceInfo(service);
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

                String[] split = keyPath.Split('\\');

                for (Int32 i = 1; i < split.Length; ++i)
                {
                    key = key.OpenSubKey(split[i]);
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
                    Utilities.CheckBoxSetChecked(this.cbFileSystemMonitor, false);
                }

                if (this.cbRegistryMonitor.Checked && RetainedSettings.KeysToTrack.Count == 0)
                {
                    Utilities.CheckBoxSetChecked(this.cbRegistryMonitor, false);
                }

                Utilities.TextBoxClear(this.tbOutput);

                if (this.cbFileSystemMonitor.Checked)
                {   
                    this.preInstallFoldersStarted = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Started @ {this.preInstallFoldersStarted}{Environment.NewLine}");

                    foreach (RetainedSettings.FileSystemSettings.TrackedFolder folder in RetainedSettings.FoldersToTrack)
                    {
                        PreInstallInventoryDirectory(folder.Folder, folder.IncludeSubDirectories);
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

                    PreInstallInventoryServices();

                    RetainedSettings.PreInstallServicesFinished = DateTime.Now;

                    Utilities.TextBoxAppendText(this.tbOutput, $"Pre-Install Services Inventory Finished @ {RetainedSettings.PreInstallServicesFinished}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Time Elapsed: {RetainedSettings.PreInstallServicesFinished - this.preInstallServicesStarted}{Environment.NewLine}");

                    Utilities.TextBoxAppendText(this.tbOutput, $"Inventoried {RetainedSettings.ServicesInventory.Keys.Count.ToString("N0")} items{Environment.NewLine}");

                    RetainedSettings.SaveServicesSettings();
                }

                Utilities.ControlSetText(this.lStatus, "");;
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
                this.bPostInstall.Enabled = this.bStartFresh.Enabled = true;

                this.lStatus.Text = "Current Folder/Key/Service Displayed Here";
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

                this.cbFileSystemMonitor.Enabled = this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                this.cbRegistryMonitor.Enabled = this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
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
                this.dgvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = this.cbFileSystemMonitor.Checked;

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
                this.dgvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = this.cbRegistryMonitor.Checked;

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
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.dgvFoldersToTrack.CurrentRow.Cells[0].Value}?", "Remove Folder?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RetainedSettings.FoldersToTrack.RemoveAt(this.dgvFoldersToTrack.CurrentRow.Index);

                    if (RetainedSettings.FoldersToTrack.Count == 0)
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
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.dgvKeysToTrack.CurrentRow.Cells[0].Value}?", "Remove Key?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RetainedSettings.KeysToTrack.RemoveAt(this.dgvKeysToTrack.CurrentRow.Index);

                    if (RetainedSettings.KeysToTrack.Count == 0)
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

        private void bStartFresh_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete the existing inventory?", "Delete Inventory?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RetainedSettings.DeleteFileSystemSettings();
                    RetainedSettings.DeleteRegistrySettings();
                    RetainedSettings.DeleteServicesSettings();

                    this.cbFileSystemMonitor.Enabled = this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                    this.cbRegistryMonitor.Enabled = this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
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
                    RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder(this.fbdAddFolder.SelectedPath, false));

                    this.dgvFoldersToTrack.ClearSelection();
                    this.dgvFoldersToTrack.CurrentCell = this.dgvFoldersToTrack.Rows[this.dgvFoldersToTrack.Rows.Count - 1].Cells[1];
                    this.dgvFoldersToTrack.Rows[this.dgvFoldersToTrack.Rows.Count - 1].Cells[1].Selected = true;
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
                    
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(this.rkbdAddKey.SelectedKeyPath, false));

                    this.dgvKeysToTrack.ClearSelection();
                    this.dgvKeysToTrack.CurrentCell = this.dgvKeysToTrack.Rows[this.dgvKeysToTrack.Rows.Count - 1].Cells[1];
                    this.dgvKeysToTrack.Rows[this.dgvKeysToTrack.Rows.Count - 1].Cells[1].Selected = true;
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
                Utilities.ControlSetText(this.lStatus, directory);

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        if (!RetainedSettings.FileSystemInventory.ContainsKey(file))
                        {
                            this.folderContentsAdded[file] = false;
                        }
                        else if (RetainedSettings.FileSystemInventory[file] == true) // file was a folder before and it's a file now
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
                        else if (RetainedSettings.FileSystemInventory[subDirectory] == false) // subDirectory was a file before and it's a directory now
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
                Utilities.ControlSetText(this.lStatus, key.ToString());

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!RetainedSettings.RegistryInventory.ContainsKey(fullPath))
                    {
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
                    }
                    else if (RetainedSettings.RegistryInventory[fullPath] == null) // value was a key before and it's a value now
                    {
                        //this.registryContentsRemoved[fullPath] = null;
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
                    }
                    else // both preinstall and postinstall inventories contain the value, check to see if it was modified
                    {
                        if ((key.GetValueKind(valueName) != RetainedSettings.RegistryInventory[fullPath].Kind) || (key.GetValue(valueName)?.ToString() != RetainedSettings.RegistryInventory[fullPath].Value))
                        {
                            this.registryContentsModified[fullPath] = new RegistryEntryDiff(RetainedSettings.RegistryInventory[fullPath],
                                                                                            new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString()));
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
                    Utilities.ControlSetText(this.lStatus, service.ServiceName);

                    if (!RetainedSettings.ServicesInventory.ContainsKey(service.ServiceName))
                    {
                        this.servicesAdded[service.ServiceName] = new ServiceInfo(service);
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
                                                                                         new ServiceInfo(service));
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
                                                                                             new ServiceInfo(service));
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
                        PostInstallInventoryDirectory(folder.Folder, folder.IncludeSubDirectories);
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

                Utilities.ControlSetText(this.lStatus, "");
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
                RetainedSettings.DeleteFileSystemSettings();
                RetainedSettings.DeleteRegistrySettings();
                RetainedSettings.DeleteServicesSettings();

                this.cbFileSystemMonitor.Enabled = this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                this.cbRegistryMonitor.Enabled = this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
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

        private void bwLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SplashScreen.Close();

                this.lStatus.Text = "";

                if ((RetainedSettings.PreInstallFileSystemFinished == null) && (RetainedSettings.PreInstallRegistryFinished == null) && (RetainedSettings.PreInstallServicesFinished == null))
                {
                    RetainedSettings.FoldersToTrack.Add(new RetainedSettings.FileSystemSettings.TrackedFolder(@"C:\", false));

                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(HKCR, false));
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(HKCU, false));
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(HKLM, false));
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(HKU, false));
                    RetainedSettings.KeysToTrack.Add(new RetainedSettings.RegistrySettings.TrackedKey(HKCC, false));

                    this.cbFileSystemMonitor.Checked = true;
                    this.dgvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = true;
                    this.cbRegistryMonitor.Checked = true;
                    this.dgvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = true;
                    this.cbServicesMonitor.Checked = true;

                    this.bPreInstall.Enabled = true;
                }
                else // a previous inventory exists
                {
                    if (RetainedSettings.PreInstallFileSystemFinished != null)
                    {
                        this.cbFileSystemMonitor.Checked = true;
                        this.cbFileSystemMonitor.Enabled = false;
                        this.dgvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = true;
                        this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = false;
                    }

                    if (RetainedSettings.PreInstallRegistryFinished != null)
                    {
                        this.cbRegistryMonitor.Checked = true;
                        this.cbRegistryMonitor.Enabled = false;
                        this.dgvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = true;
                        this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = false;
                    }

                    if (RetainedSettings.PreInstallServicesFinished != null)
                    {
                        this.cbServicesMonitor.Checked = true;
                        this.cbServicesMonitor.Enabled = false;
                    }

                    this.bPreInstall.Enabled = false;
                    this.bPostInstall.Enabled = true;
                    this.bStartFresh.Enabled = true;
                }

                this.dgvFoldersToTrack.DataSource = new BindingSource { DataSource = RetainedSettings.FoldersToTrack };
                this.dgvKeysToTrack.DataSource = new BindingSource { DataSource = RetainedSettings.KeysToTrack };
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
