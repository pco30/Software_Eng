using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class HtmlPreviewForm : Form
    {
        private string _htmlPath;

        public HtmlPreviewForm()
        {
            InitializeComponent();
        }

        public HtmlPreviewForm(string htmlPath) : this()
        {
            _htmlPath = htmlPath;
        }

        private void HtmlPreviewForm_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_htmlPath))
            {
                MessageBox.Show("HTML path is NULL or empty", "Error");
                return;
            }

            MessageBox.Show("HTML file found:\n" + _htmlPath);
            string readHtmlContent = string.Empty;
            using (StreamReader streamReader = new StreamReader(_htmlPath))
            {
                readHtmlContent = streamReader.ReadToEnd();
            }
            ReportPreviewBrowser.DocumentText = readHtmlContent;
        }

        // Optional: Do something AFTER loading finishes
        private void ReportPreviewBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Text = ReportPreviewBrowser.DocumentTitle;
        }

        private void btExportJson_Click(object sender, EventArgs e)
        {

        }

        private void btViewExternal_Click(object sender, EventArgs e)
        {

        }
    }
}
