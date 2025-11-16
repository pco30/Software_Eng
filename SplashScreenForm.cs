using System;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm(WinChangeMonitorForm owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }

        public Boolean ConfirmOnClose = true;

        private String statusMessage = null;
        private int statusCount = 0, totalCount = 0;

        public void InitializeStatus(String status, int totalCount)
        {
            this.statusMessage = status;
            this.statusCount = 0;
            this.totalCount = totalCount;

            Utilities.ControlSetText(this.lStatus, status);

            this.tReportStatus.Start();
        }

        public void IncrementStatus()
        {
            try
            {
                ++this.statusCount;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void SplashScreenForm_Load(Object sender, EventArgs e)
        {
            try
            {
                if (this.Owner != null)
                {
                    this.Location = this.Owner.Location;

                    this.Size = this.Owner.Size;

                    this.Text = this.Owner.Text;
                }
                // Show message when splash screen is starts loading
                lTitle.Text = "System Analyzer Loading...";
                lStatus.Text = "Loading from disk...";
                lStatus.Refresh();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void SplashScreenForm_Resize(Object sender, EventArgs e)
        {
            try
            {
                if (this.Owner != null)
                {
                    this.Owner.WindowState = this.WindowState;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void SplashScreenForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.Visible)
                {
                    if (this.ConfirmOnClose && MessageBox.Show("Are you sure you want to close the program?", "Confirm Exit", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        this.ConfirmOnClose = false;

                        if (this.Owner is WinChangeMonitorForm)
                        {
                            ((WinChangeMonitorForm)this.Owner).ConfirmOnClose = false;
                        }
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void SplashScreenForm_Move(Object sender, EventArgs e)
        {
            try
            {
                if (this.Owner != null)
                {
                    this.Owner.Location = this.Location;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private void lStatus_Click(object sender, EventArgs e)
        {
            lStatus.Text = "Loading from  disk...";
        }

        private void lTitle_Click(object sender, EventArgs e)
        {
            lTitle.Text = "WinChangeMonitor is starting...";
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

        }

        private void tReportStatus_Tick(Object sender, EventArgs e)
        {
            try
            {
                if (this.statusCount > 0)
                {
                    if (this.totalCount > 0)
                    {
                        Utilities.ControlSetText(this.lStatus, $"{this.statusMessage} [{this.statusCount.ToString("N0")} of {this.totalCount.ToString("N0")}]");
                    }
                    else // alternate display format in case user deleted _Common.bin and totalCount is unavailable
                    {
                        Utilities.ControlSetText(this.lStatus, $"{this.statusMessage} [{this.statusCount.ToString("N0")}]");
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
