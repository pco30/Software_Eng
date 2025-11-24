using System;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class HtmlPreviewForm : Form
    {
        public HtmlPreviewForm(string htmlPath)
        {
            InitializeComponent();

            // Create viewer control
            WebBrowser browser = new WebBrowser();
            browser.Dock = DockStyle.Fill;
            browser.Navigate(htmlPath);

            this.Controls.Add(browser);
            this.Text = "HTML Preview";
            this.Width = 900;
            this.Height = 700;
        }
    }
}