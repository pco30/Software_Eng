using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class AboutForm : Form
    {
        public AboutForm(Form owner)
        {
            try
            {
                InitializeComponent();
                this.Owner = owner;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void llAbout_LinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/pco30/Software_Eng");
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void bOK_Click(Object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
