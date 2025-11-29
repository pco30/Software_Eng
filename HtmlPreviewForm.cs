using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
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
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void HtmlPreviewForm_Load(Object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.htmlPath))
                {
                    MessageBox.Show("HTML path is NULL or empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.ReportPreviewBrowser.Navigate(this.htmlPath); // WebBrowser.Navigate is asynchronous
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void ReportPreviewBrowser_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
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
