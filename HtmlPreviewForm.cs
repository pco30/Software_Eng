using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static WinChangeMonitor.RetainedSettings.FileSystemSettings;
using static WinChangeMonitor.RetainedSettings.RegistrySettings;

namespace WinChangeMonitor
{
    public partial class HtmlPreviewForm : Form
    {
        private SaveFileDialog sfdExportJson = new SaveFileDialog();
        private String htmlPath = null;

        public HtmlPreviewForm(WinChangeMonitorForm owner, String htmlPath)
        {
            try
            {
                InitializeComponent();
                this.Owner = owner;
                this.htmlPath = htmlPath;

                this.sfdExportJson.FileName = "Report.json";
                this.sfdExportJson.Filter = "JSON Files (*.json)|*.json";
                this.sfdExportJson.InitialDirectory = RetainedSettings.DirectoryName;
                this.sfdExportJson.OverwritePrompt = true;
                this.sfdExportJson.ValidateNames = true;

                if (String.IsNullOrEmpty(this.htmlPath))
                {
                    MessageBox.Show("HTML path is NULL or empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.ReportPreviewBrowser.Navigate(this.htmlPath);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void ReportPreviewBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                this.Text = ReportPreviewBrowser.DocumentTitle;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private class JsonExportData
        {
            [JsonInclude]
            public List<TrackedFolder> TrackedFolders { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, Boolean> FolderContentsAdded { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, Boolean> FolderContentsModified { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, FileSystemEntryInfo> FolderContentsRemoved { get; private set; }

            [JsonInclude]
            public List<TrackedKey> TrackedKeys { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, RegistryEntryInfo> RegistryContentsAdded { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, RegistryEntryDiff> RegistryContentsModified { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, RegistryEntryInfo> RegistryContentsRemoved { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, ServiceInfo> ServicesAdded { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, ServiceDiff> ServicesModified { get; private set; }

            [JsonInclude]
            public SortedDictionary<String, ServiceInfo> ServicesRemoved { get; private set; }

            public JsonExportData(
                List<TrackedFolder> trackedFolders,
                SortedDictionary<String, Boolean> folderContentsAdded,
                SortedDictionary<String, Boolean> folderContentsModified,
                SortedDictionary<String, FileSystemEntryInfo> folderContentsRemoved,
                List<TrackedKey> trackedKeys,
                SortedDictionary<String, RegistryEntryInfo> registryContentsAdded,
                SortedDictionary<String, RegistryEntryDiff> registryContentsModified,
                SortedDictionary<String, RegistryEntryInfo> registryContentsRemoved,
                SortedDictionary<String, ServiceInfo> servicesAdded,
                SortedDictionary<String, ServiceDiff> servicesModified,
                SortedDictionary<String, ServiceInfo> servicesRemoved)
            {
                try
                {
                    this.TrackedFolders = trackedFolders;
                    this.FolderContentsAdded = folderContentsAdded;
                    this.FolderContentsModified = folderContentsModified;
                    this.FolderContentsRemoved = folderContentsRemoved;
                    this.TrackedKeys = trackedKeys;
                    this.RegistryContentsAdded = registryContentsAdded;
                    this.RegistryContentsModified = registryContentsModified;
                    this.RegistryContentsRemoved = registryContentsRemoved;
                    this.ServicesAdded = servicesAdded;
                    this.ServicesModified = servicesModified;
                    this.ServicesRemoved = servicesRemoved;
                }
                catch (Exception ex)
                {
                    Utilities.HandleException(ex);
                }
            }
        }

        private void btExportJson_Click(Object sender, EventArgs e)
        {
            try
            {
                if ((this.Owner is WinChangeMonitorForm owner) && (this.sfdExportJson.ShowDialog() == DialogResult.OK))
                {
                    JsonExportData data = new JsonExportData(
                        RetainedSettings.FoldersToTrack, owner.FolderContentsAdded, owner.FolderContentsModified, RetainedSettings.FileSystemInventory,
                        RetainedSettings.KeysToTrack, owner.RegistryContentsAdded, owner.RegistryContentsModified, RetainedSettings.RegistryInventory,
                        owner.ServicesAdded, owner.ServicesModified, RetainedSettings.ServicesInventory);

                    String jsonData = JsonSerializer.Serialize(data);
                    File.WriteAllText(this.sfdExportJson.FileName, jsonData);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void btViewExternal_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.htmlPath) && File.Exists(this.htmlPath))
                {
                    Process.Start(this.htmlPath);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
