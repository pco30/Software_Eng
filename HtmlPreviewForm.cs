using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class HtmlPreviewForm : Form
    {
        private String htmlPath = null;

        public HtmlPreviewForm()
        {
            try
            {
                InitializeComponent();
                this.Load += new EventHandler(this.HtmlPreviewForm_Load);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public HtmlPreviewForm(String htmlPath) : this()
        {
            try
            {
                this.htmlPath = htmlPath;
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
                    MessageBox.Show("HTML path is NULL or empty", "Error");
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

        // Optional: Do something AFTER loading finishes
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

        private void btExportJson_Click(object sender, EventArgs e)
        {
            try
            {

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
