using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Security;
using System.ServiceProcess;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class WinChangeMonitorForm : Form
    {
        private DataTable dtFoldersToTrack = new DataTable();
        private DataTable dtKeysToTrack = new DataTable();
        private SortedDictionary<String, Boolean> folderContentsPreInstall = new SortedDictionary<String, Boolean>(); // key is full path to file/folder, value is whether this is a folder or file (true if folder, false if file)
        private SortedDictionary<String, RegistryEntryInfo> registryContentsPreInstall = new SortedDictionary<String, RegistryEntryInfo>();
        private SortedDictionary<String, ServiceInfo> servicesPreInstall = new SortedDictionary<String, ServiceInfo>();
        private DateTime? preInstallFoldersStarted = null;
        private DateTime? preInstallFoldersFinished = null;
        private DateTime? preInstallRegistryStarted = null;
        private DateTime? preInstallRegistryFinished = null;
        private DateTime? preInstallServicesStarted = null;
        private DateTime? preInstallServicesFinished = null;
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

        private static String HKCR = "HKEY_CLASSES_ROOT";
        private static String HKCU = "HKEY_CURRENT_USER";
        private static String HKLM = "HKEY_LOCAL_MACHINE";
        private static String HKU = "HKEY_USERS";
        private static String HKCC = "HKEY_CURRENT_CONFIG";

        private void HandleException(Exception ex)
        {
            MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        public WinChangeMonitorForm()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void WinChangeMonitorForm_Load(Object sender, EventArgs e)
        {
            try
            {
                this.cbFileSystemMonitor.Checked = false;
                this.dgvFoldersToTrack.Visible = this.bAddFolder.Visible = this.bRemoveFolder.Visible = false;
                this.cbRegistryMonitor.Checked = false;
                this.dgvKeysToTrack.Visible = this.bAddKey.Visible = this.bRemoveKey.Visible = false;
                this.cbServicesMonitor.Checked = false;

                this.bPreInstall.Enabled = false;

                this.dtFoldersToTrack.Columns.Add("Folder");
                this.dtFoldersToTrack.Columns.Add("IncludeSubDirectories");

                this.dtFoldersToTrack.Rows.Add(@"C:\", false);

                this.dgvFoldersToTrack.DataSource = this.dtFoldersToTrack;

                this.dtKeysToTrack.Columns.Add("Key");
                this.dtKeysToTrack.Columns.Add("IncludeSubKeys");

                this.dtKeysToTrack.Rows.Add(HKCR, false);
                this.dtKeysToTrack.Rows.Add(HKCU, false);
                this.dtKeysToTrack.Rows.Add(HKLM, false);
                this.dtKeysToTrack.Rows.Add(HKU, false);
                this.dtKeysToTrack.Rows.Add(HKCC, false);

                this.dgvKeysToTrack.DataSource = this.dtKeysToTrack;

                this.MinimumSize = this.Size;

                this.fbdAddFolder.ShowNewFolderButton = false;
                this.fbdAddFolder.RootFolder = Environment.SpecialFolder.MyComputer;
                
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
                HandleException(ex);
            }
        }

        private void PreInstallInventoryDirectory(String directory, Boolean recursive)
        {
            try
            {
                ControlSetText(this.lStatus, directory);

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        this.folderContentsPreInstall[file] = false;
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        this.folderContentsPreInstall[subDirectory] = true;

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
                HandleException(ex);
            }
        }

        private void PreInstallInventoryRegistry(RegistryKey key, Boolean recursive)
        {
            try
            {
                ControlSetText(this.lStatus, key.ToString());

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!this.registryContentsPreInstall.ContainsKey(fullPath))
                    {
                        this.registryContentsPreInstall[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
                    }
                }

                String[] subkeyNames = key.GetSubKeyNames();

                foreach (String subkeyName in subkeyNames)
                {
                    fullPath = $@"{key.Name}\{subkeyName}";

                    if (!this.registryContentsPreInstall.ContainsKey(fullPath))
                    {
                        this.registryContentsPreInstall[fullPath] = null;
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
                HandleException(ex);
            }
        }

        private void PreInstallInventoryServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    ControlSetText(this.lStatus, service.ServiceName);

                    if (!this.servicesPreInstall.ContainsKey(service.ServiceName))
                    {
                        this.servicesPreInstall[service.ServiceName] = new ServiceInfo(service);
                    }
                }
            }
            catch(Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void TextBoxClearDelegate(TextBox textBox);
        private void TextBoxClear(TextBox textBox)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(new TextBoxClearDelegate(TextBoxClear), new Object[] { textBox });
                }
                else
                {
                    textBox.Clear();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void ControlSetTextDelegate(Control control, String text);
        private void ControlSetText(Control control, String text)
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(new ControlSetTextDelegate(ControlSetText), new Object[] { control, text });
                }
                else
                {
                    control.Text = text;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void TextBoxAppendTextDelegate(TextBox textBox, String text);
        private void TextBoxAppendText(TextBox textBox, String text)
        {
            try
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(new TextBoxAppendTextDelegate(TextBoxAppendText), new Object[] { textBox, text });
                }
                else
                {
                    textBox.AppendText(text);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        delegate void CheckBoxSetCheckedDelegate(CheckBox checkBox, Boolean value);
        private void CheckBoxSetChecked(CheckBox checkBox, Boolean value)
        {
            try
            {
                if (checkBox.InvokeRequired)
                {
                    checkBox.Invoke(new CheckBoxSetCheckedDelegate(CheckBoxSetChecked), new object[] { checkBox, value });
                }
                else
                {
                    checkBox.Checked = value;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
                HandleException(ex);
                return null;
            }
        }

        private void bwPreInstall_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                if (this.cbFileSystemMonitor.Checked && this.dtFoldersToTrack.Rows.Count == 0)
                {
                    CheckBoxSetChecked(this.cbFileSystemMonitor, false);
                }

                if (this.cbRegistryMonitor.Checked && this.dtKeysToTrack.Rows.Count == 0)
                {
                    CheckBoxSetChecked(this.cbRegistryMonitor, false);
                }

                TextBoxClear(this.tbOutput);

                if (this.cbFileSystemMonitor.Checked)
                {   
                    this.preInstallFoldersStarted = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Started @ {this.preInstallFoldersStarted}{Environment.NewLine}");

                    foreach (DataRow row in this.dtFoldersToTrack.Rows)
                    {
                        Boolean recursive = false;

                        Boolean.TryParse(row["IncludeSubDirectories"].ToString(), out recursive);

                        PreInstallInventoryDirectory(row["Folder"].ToString(), recursive);
                    }

                    this.preInstallFoldersFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Finished @ {this.preInstallFoldersFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.preInstallFoldersFinished - this.preInstallFoldersStarted}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Inventoried {this.folderContentsPreInstall.Keys.Count.ToString("N0")} items{Environment.NewLine}");
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.preInstallRegistryStarted = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install Registry Inventory Started @ {this.preInstallRegistryStarted}{Environment.NewLine}");

                    RegistryKey key;

                    foreach (DataRow row in this.dtKeysToTrack.Rows)
                    {
                        key = OpenRegistryKey(row["Key"].ToString());

                        Boolean recursive = false;

                        Boolean.TryParse(row["IncludeSubKeys"].ToString(), out recursive);

                        PreInstallInventoryRegistry(key, recursive);
                    }

                    this.preInstallRegistryFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install Registry Inventory Finished @ {this.preInstallRegistryFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.preInstallRegistryFinished - this.preInstallRegistryStarted}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Inventoried {this.registryContentsPreInstall.Keys.Count.ToString("N0")} items{Environment.NewLine}");
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.preInstallServicesStarted = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install Services Inventory Started @ {this.preInstallServicesStarted}{Environment.NewLine}");

                    PreInstallInventoryServices();

                    this.preInstallServicesFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Pre-Install Services Inventory Finished @ {this.preInstallServicesFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.preInstallServicesFinished - this.preInstallServicesStarted}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Inventoried {this.servicesPreInstall.Keys.Count.ToString("N0")} items{Environment.NewLine}");
                }

                ControlSetText(this.lStatus, "");;
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
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
                HandleException(ex);
            }
        }

        private void bRemoveFolder_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.dgvFoldersToTrack.CurrentRow.Cells[0].Value}?", "Remove Folder?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    this.dtFoldersToTrack.Rows.RemoveAt(this.dgvFoldersToTrack.CurrentRow.Index);

                    if (this.dtFoldersToTrack.Rows.Count == 0)
                    {
                        this.bRemoveFolder.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbRegistryMonitor.Checked && this.dtKeysToTrack.Rows.Count > 0) || this.cbServicesMonitor.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void bRemoveKey_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to remove {this.dgvKeysToTrack.CurrentRow.Cells[0].Value}?", "Remove Key?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    this.dtKeysToTrack.Rows.RemoveAt(this.dgvKeysToTrack.CurrentRow.Index);

                    if (this.dtKeysToTrack.Rows.Count == 0)
                    {
                        this.bRemoveKey.Enabled = false;

                        this.bPreInstall.Enabled = (this.cbFileSystemMonitor.Checked && this.dtFoldersToTrack.Rows.Count > 0) || this.cbServicesMonitor.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void bStartFresh_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete the existing inventory?", "Delete Inventory?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    this.folderContentsPreInstall = new SortedDictionary<String, Boolean>();
                    this.registryContentsPreInstall = new SortedDictionary<String, RegistryEntryInfo>();
                    this.servicesPreInstall = new SortedDictionary<String, ServiceInfo>();

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
                HandleException(ex);
            }
        }

        private void bAddFolder_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = this.fbdAddFolder.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.dtFoldersToTrack.Rows.Add(this.fbdAddFolder.SelectedPath, false);

                    this.dgvFoldersToTrack.ClearSelection();
                    this.dgvFoldersToTrack.CurrentCell = this.dgvFoldersToTrack.Rows[this.dgvFoldersToTrack.Rows.Count - 1].Cells[1];
                    this.dgvFoldersToTrack.Rows[this.dgvFoldersToTrack.Rows.Count - 1].Cells[1].Selected = true;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void bAddKey_Click(Object sender, EventArgs e)
        {
            try
            {
                DialogResult result = this.rkbdAddKey.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.dtKeysToTrack.Rows.Add(this.rkbdAddKey.SelectedKeyPath, false);

                    this.dgvKeysToTrack.ClearSelection();
                    this.dgvKeysToTrack.CurrentCell = this.dgvKeysToTrack.Rows[this.dgvKeysToTrack.Rows.Count - 1].Cells[1];
                    this.dgvKeysToTrack.Rows[this.dgvKeysToTrack.Rows.Count - 1].Cells[1].Selected = true;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void PostInstallInventoryDirectory(String directory, Boolean recursive)
        {
            try
            {
                ControlSetText(this.lStatus, directory);

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        if (!this.folderContentsPreInstall.ContainsKey(file))
                        {
                            this.folderContentsAdded[file] = false;
                        }
                        else if (this.folderContentsPreInstall[file] == true) // file was a folder before and it's a file now
                        {
                            //this.folderContentsRemoved[file] = true; // the original folder was deleted
                            this.folderContentsAdded[file] = false; // the new file was added
                        }
                        else // both preinstall and postinstall inventories contain the file, check to see if it was modified
                        {
                            if (File.GetLastWriteTime(file) > this.preInstallFoldersFinished)
                            {
                                this.folderContentsModified[file] = false;
                            }

                            if (!this.folderContentsPreInstall.Remove(file)) // remove file from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            {
                                TextBoxAppendText(this.tbOutput, $"ERROR: folderContentsPreInstall.Remove(file [\"{file}\"]) FAILED{Environment.NewLine}");
                            }
                        }
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        if (!this.folderContentsPreInstall.ContainsKey(subDirectory))
                        {
                            this.folderContentsAdded[subDirectory] = true;
                        }
                        else if (this.folderContentsPreInstall[subDirectory] == false) // subDirectory was a file before and it's a directory now
                        {
                            //this.folderContentsRemoved[subDirectory] = false; // the original file was deleted
                            this.folderContentsAdded[subDirectory] = true; // the new directory was added
                        }
                        else // both preinstall and postinstall inventories contain the directory, check to see if it was modified
                        {
                            if (Directory.GetLastWriteTime(subDirectory) > this.preInstallFoldersFinished)
                            {
                                this.folderContentsModified[subDirectory] = true;
                            }

                            if (!this.folderContentsPreInstall.Remove(subDirectory)) // remove directory from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            {
                                TextBoxAppendText(this.tbOutput, $"ERROR: folderContentsPreInstall.Remove(subDirectory [\"{subDirectory}\"]) FAILED{Environment.NewLine}");
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
                HandleException(ex);
            }
        }

        private void PostInstallInventoryRegistry(RegistryKey key, Boolean recursive)
        {
            try
            {
                ControlSetText(this.lStatus, key.ToString());

                String[] valueNames = key.GetValueNames();

                String fullPath;

                foreach (String valueName in valueNames)
                {
                    fullPath = $@"{key.Name}\{valueName}";

                    if (!this.registryContentsPreInstall.ContainsKey(fullPath))
                    {
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
                    }
                    else if (this.registryContentsPreInstall[fullPath] == null) // value was a key before and it's a value now
                    {
                        //this.registryContentsRemoved[fullPath] = null;
                        this.registryContentsAdded[fullPath] = new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString());
                    }
                    else // both preinstall and postinstall inventories contain the value, check to see if it was modified
                    {
                        if ((key.GetValueKind(valueName) != this.registryContentsPreInstall[fullPath].Kind) || (key.GetValue(valueName)?.ToString() != this.registryContentsPreInstall[fullPath].Value))
                        {
                            this.registryContentsModified[fullPath] = new RegistryEntryDiff(this.registryContentsPreInstall[fullPath],
                                                                                             new RegistryEntryInfo(key.GetValueKind(valueName), key.GetValue(valueName)?.ToString()));
                        }

                        if (!this.registryContentsPreInstall.Remove(fullPath)) // remove value from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        {
                            TextBoxAppendText(this.tbOutput, $"ERROR: registryContentsPreInstall.Remove(valueName [\"{fullPath}\"]) FAILED{Environment.NewLine}");
                        }

                    }
                }

                String[] subkeyNames = key.GetSubKeyNames();

                foreach (String subkeyName in subkeyNames)
                {
                    fullPath = $@"{key.Name}\{subkeyName}";

                    if (!this.registryContentsPreInstall.ContainsKey(fullPath))
                    {
                        this.registryContentsAdded[fullPath] = null;
                    }
                    else if (this.registryContentsPreInstall[fullPath] != null) // subkey was a value before and it's a key now
                    {
                        //this.registryContentsRemoved[fullPath] = this.registryContentsPreInstall[fullPath];
                        this.registryContentsAdded[fullPath] = null;
                    }
                    else // both preinstall and postinstall inventories contain the subkey
                    {
                        if (!this.registryContentsPreInstall.Remove(fullPath))
                        {
                            TextBoxAppendText(this.tbOutput, $"ERROR: registryContentsPreInstall.Remove(subkeyName [\"{fullPath}\"]) FAILED{Environment.NewLine}");
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
                HandleException(ex);
            }
        }

        private void PostInstallInventoryServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    ControlSetText(this.lStatus, service.ServiceName);

                    if (!this.servicesPreInstall.ContainsKey(service.ServiceName))
                    {
                        this.servicesAdded[service.ServiceName] = new ServiceInfo(service);
                    }
                    else // both preinstall and postinstall inventories contain the service, check to see if it was modified
                    {
                        if ((service.CanPauseAndContinue != this.servicesPreInstall[service.ServiceName].CanPauseAndContinue) ||
                            (service.CanShutdown != this.servicesPreInstall[service.ServiceName].CanShutdown) ||
                            (service.CanStop != this.servicesPreInstall[service.ServiceName].CanStop) ||
                            (service.DisplayName != this.servicesPreInstall[service.ServiceName].DisplayName) ||
                            (service.ServiceType != this.servicesPreInstall[service.ServiceName].ServiceType) ||
                            (service.StartType != this.servicesPreInstall[service.ServiceName].StartType) ||
                            (service.ServicesDependedOn.Length != this.servicesPreInstall[service.ServiceName].ServiceNamesDependedOn.Count))
                        {
                            this.servicesModified[service.ServiceName] = new ServiceDiff(this.servicesPreInstall[service.ServiceName],
                                                                                         new ServiceInfo(service));
                        }
                        else // everything else matches, now check to see if any of the services depended on changed
                        {
                            Boolean changed = false;

                            for (Int32 i = 0; !changed && (i < service.ServicesDependedOn.Length); ++i)
                            {
                                if (service.ServicesDependedOn[i].ServiceName != this.servicesPreInstall[service.ServiceName].ServiceNamesDependedOn[i])
                                {
                                    changed = true;
                                }
                            }

                            if (changed)
                            {
                                this.servicesModified[service.ServiceName] = new ServiceDiff(this.servicesPreInstall[service.ServiceName],
                                                                                             new ServiceInfo(service));
                            }
                        }

                        if (!this.servicesPreInstall.Remove(service.ServiceName)) // remove service from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                        {
                            TextBoxAppendText(this.tbOutput, $"ERROR: servicesPreInstall.Remove(\"{service.ServiceName}\") FAILED{Environment.NewLine}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
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

                    TextBoxAppendText(this.tbOutput, $"Post-Install File/Folder Inventory Started @ {this.postInstallFoldersStarted}{Environment.NewLine}");

                    foreach (DataRow row in this.dtFoldersToTrack.Rows)
                    {
                        Boolean recursive = false;

                        Boolean.TryParse(row["IncludeSubDirectories"].ToString(), out recursive);

                        PostInstallInventoryDirectory(row["Folder"].ToString(), recursive);
                    }

                    this.postInstallFoldersFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Post-Install File/Folder Inventory Finished @ {this.postInstallFoldersFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.postInstallFoldersFinished - this.postInstallFoldersStarted}{Environment.NewLine}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"{this.folderContentsAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.folderContentsAdded.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.folderContentsAdded[key]}{Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.folderContentsModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.folderContentsModified.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.folderContentsModified[key]}{Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.folderContentsPreInstall.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.folderContentsPreInstall.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.folderContentsPreInstall[key]}{Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, Environment.NewLine);
                }

                if (this.cbRegistryMonitor.Checked)
                {
                    this.postInstallRegistryStarted = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Post-Install Registry Inventory Started @ {this.postInstallRegistryStarted}{Environment.NewLine}");

                    RegistryKey key;

                    foreach (DataRow row in this.dtKeysToTrack.Rows)
                    {
                        key = OpenRegistryKey(row["Key"].ToString());

                        Boolean recursive = false;

                        Boolean.TryParse(row["IncludeSubKeys"].ToString(), out recursive);

                        PostInstallInventoryRegistry(key, recursive);
                    }

                    this.postInstallRegistryFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Post-Install Registry Inventory Finished @ {this.postInstallRegistryFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"Time Elapsed: {this.postInstallRegistryFinished - this.postInstallRegistryStarted}{Environment.NewLine}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"{this.registryContentsAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in this.registryContentsAdded.Keys)
                    {
                        if (this.registryContentsAdded[entry] == null)
                        {
                            TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = null{Environment.NewLine}");
                        }
                        else
                        {
                            TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = ({this.registryContentsAdded[entry]}{Environment.NewLine}");
                        }
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.registryContentsModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in this.registryContentsModified.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = {this.registryContentsModified[entry]}{Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.registryContentsPreInstall.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String entry in this.registryContentsPreInstall.Keys)
                    {
                        if (this.registryContentsPreInstall[entry] == null)
                        {
                            TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = null{Environment.NewLine}");
                        }
                        else
                        {
                            TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {entry} = ({this.registryContentsPreInstall[entry]}){Environment.NewLine}");
                        }
                    }

                    TextBoxAppendText(this.tbOutput, Environment.NewLine);
                }

                if (this.cbServicesMonitor.Checked)
                {
                    this.postInstallServicesStarted = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Post-Install Services Inventory Started @ {this.postInstallServicesStarted}{Environment.NewLine}");

                    PostInstallInventoryServices();

                    this.postInstallServicesFinished = DateTime.Now;

                    TextBoxAppendText(this.tbOutput, $"Post-Install Services Inventory Finished @ {this.postInstallServicesFinished}{Environment.NewLine}");

                    TextBoxAppendText(this.tbOutput, $"{this.servicesAdded.Keys.Count.ToString("N0")} items were added{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.servicesAdded.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = ({this.servicesAdded[key]}){Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.servicesModified.Keys.Count.ToString("N0")} items were modified{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.servicesModified.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = {this.servicesModified[key]}{Environment.NewLine}");
                    }

                    TextBoxAppendText(this.tbOutput, $"{this.servicesPreInstall.Keys.Count.ToString("N0")} items were removed{Environment.NewLine}");

                    count = 0;

                    foreach (String key in this.servicesPreInstall.Keys)
                    {
                        TextBoxAppendText(this.tbOutput, $"{(++count).ToString().PadLeft(TotalPaddedSize)}. {key} = ({this.servicesPreInstall[key]}");
                    }
                }

                ControlSetText(this.lStatus, "");
            }
            catch(Exception ex)
            {
                HandleException(ex);
            }
        }

        private void bwPostInstall_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.cbFileSystemMonitor.Enabled = this.dgvFoldersToTrack.Enabled = this.bAddFolder.Enabled = this.bRemoveFolder.Enabled = true;
                this.cbRegistryMonitor.Enabled = this.dgvKeysToTrack.Enabled = this.bAddKey.Enabled = this.bRemoveKey.Enabled = true;
                this.cbServicesMonitor.Enabled = true;
                this.bPreInstall.Enabled = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
