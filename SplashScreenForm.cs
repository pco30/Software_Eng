using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinChangeMonitor
{
    public partial class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            InitializeComponent();
        }

        private String statusMessage = null;
        private UInt64 statusCount = 0;

        public void InitializeStatus(String status)
        {
            this.statusMessage = status;
            this.statusCount = 0;

            Utilities.ControlSetText(this.lStatus, status);
        }

        public void IncrementStatus()
        {
            try
            {
                ++this.statusCount;
                Utilities.ControlSetText(this.lStatus, $"{this.statusMessage} [{this.statusCount}]");
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
                this.Location = this.Owner.Location;

                this.Size = this.Owner.Size;

                this.Text = this.Owner.Text;
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
                this.Owner.WindowState = this.WindowState;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
