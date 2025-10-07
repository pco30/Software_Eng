using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class WinChangeMonitorForm : Form
    {
        private DataTable dtFoldersToTrack = new DataTable();
        private DataTable dtRegistryKeysToTrack = new DataTable();
        // List<String>.Contains(String) is O(n)
        // SortedSet<String>.Contains(String) is O(log n)
        // Dictionary<String, ...>.ContainsKey(String) approaches O(1)
        private SortedDictionary<String, Boolean> folderContentsPreInstall = new SortedDictionary<String, Boolean>(); // key is full path to file/folder, value is whether this is a folder or file (true if folder, false if file)
        private SortedDictionary<String, String> registryContentsPreInstall = new SortedDictionary<String, String>(); // TODO - enhance this so values are (Type [Key|ValueDataType], String)
        private DateTime? preInstallStarted = null;
        private DateTime? preInstallFinished = null;
        private SortedDictionary<String, Boolean> folderContentsAdded = new SortedDictionary<String, Boolean>();
        private SortedDictionary<String, Boolean> folderContentsModified = new SortedDictionary<String, Boolean>();

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
                this.dtFoldersToTrack.Columns.Add("Folder");
                this.dtFoldersToTrack.Columns.Add("IncludeSubDirectories");

                this.dtFoldersToTrack.Rows.Add(@"C:\", false);

                this.dgvFoldersToTrack.DataSource = this.dtFoldersToTrack;
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
                this.tbOutput.Clear();
                this.preInstallStarted = DateTime.Now;
                this.tbOutput.AppendText($"Pre-Install File/Folder Inventory Started @ {this.preInstallStarted}{Environment.NewLine}");

                this.tcWhatToTrack.Enabled = false;
                this.bPreInstall.Enabled = false;

                this.bwPreInstall.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void InventoryDirectory(SortedDictionary<String, Boolean> inventory, String directory, Boolean recursive, Boolean isPreinstall = true)
        {
            try
            {
                ControlSetText(this.lStatus, directory);

                try
                {
                    String[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String file in files)
                    {
                        if (isPreinstall)
                        {
                            inventory[file] = false;
                        }
                        else // !isPreinstall (this is a Post-Install Inventory/Diff)
                        {
                            if (!inventory.ContainsKey(file))
                            {
                                this.folderContentsAdded[file] = false;
                            }
                            else // both preinstall and postinstall inventories contain the file, check to see if it was modified
                            {
                                if (File.GetLastWriteTime(file) > this.preInstallFinished)
                                {
                                    this.folderContentsModified[file] = false;
                                }

                                inventory.Remove(file); // remove file from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            }
                        }
                    }

                    String[] subDirectories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

                    foreach (String subDirectory in subDirectories)
                    {
                        if (isPreinstall)
                        {
                            inventory[subDirectory] = true;
                        }
                        else // !isPreinstall (this is a Post-Install Inventory/Diff)
                        {
                            if (!inventory.ContainsKey(subDirectory))
                            {
                                this.folderContentsAdded[subDirectory] = true;
                            }
                            else // both preinstall and postinstall inventories contain the folder, so it's neither new nor deleted
                            {
                                if (Directory.GetLastWriteTime(subDirectory) > this.preInstallFinished)
                                {
                                    this.folderContentsModified[subDirectory] = true;
                                }

                                inventory.Remove(subDirectory); // remove subDirectory from preinstall inventory so that all keys contained in inventory at the end were those removed by the tracked executable/script
                            }
                        }

                        if (recursive)
                        {
                            InventoryDirectory(inventory, subDirectory, recursive: true, isPreinstall: isPreinstall);
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

        

        private void InventoryRegistry(SortedDictionary<String, String> inventory, RegistryKey key, Boolean recursive)
        {
            try
            {
                String[] valueNames = key.GetValueNames();

                foreach (String valueName in valueNames)
                {
                    if (!inventory.ContainsKey(valueName))
                    {
                        // TODO - inventory[valueName] = (key.GetValueKind(valueName), key.GetValue(valueName).ToString())
                        inventory[valueName] = key.GetValue(valueName).ToString();
                    }
                }

                String[] subkeyNames = key.GetSubKeyNames();

                foreach (String subkeyName in subkeyNames)
                {
                    if (!inventory.ContainsKey(subkeyName))
                    {
                        inventory[subkeyName] = "Key";
                    }

                    if (recursive)
                    {
                        InventoryRegistry(inventory, key.OpenSubKey(subkeyName), true);
                    }
                }
            }
            catch (Exception ex)
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

        private void bwPreInstall_DoWork(Object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                foreach (DataRow row in this.dtFoldersToTrack.Rows)
                {
                    InventoryDirectory(this.folderContentsPreInstall, row["Folder"].ToString(), Boolean.Parse(row["IncludeSubDirectories"].ToString()));
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
                this.preInstallFinished = DateTime.Now;

                TextBoxAppendText(this.tbOutput, $"Pre-Install File/Folder Inventory Finished @ {this.preInstallFinished}{Environment.NewLine}");

                this.tbOutput.AppendText($"Time Elapsed: {this.preInstallFinished - this.preInstallStarted}{Environment.NewLine}");

                TextBoxAppendText(this.tbOutput, $"Inventoried {this.folderContentsPreInstall.Keys.Count.ToString("N0")} items");



                this.bPreInstall.Enabled = true;
                this.bPostInstall.Enabled = true;
                this.tcWhatToTrack.Enabled = true;

                //foreach (String entry in this.folderContentsPreInstall.Keys)
                //{
                //    TextBoxAppendText(this.tbOutput, $"{Environment.NewLine}{entry}");
                //}
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
                //RegistryKey baseKey;
                //foreach (DataRow row in this.dtRegistryKeysToTrack.Rows)
                //{
                //    if (row)
                //}
                //    = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                //InventoryRegistry(this.registryContentsPreInstall, hklm);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
