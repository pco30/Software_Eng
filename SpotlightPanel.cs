using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinChangeMonitor
{
    internal class SpotlightPanel : Panel
    {
        private Control targetControl = null;
        private Double opacity;

        private Double Opacity
        {
            get => this.opacity;
            set
            {
                this.opacity = value;
                this.Invalidate();
            }
        }

        public SpotlightPanel() // this contructor can be used directly to gray-out the whole form or for non-Controls like menu items
        {
            try
            {
                this.BackColor = Color.Black;
                this.Opacity = 0.7;
                this.SetStyle(ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public SpotlightPanel(Control target) : this()
        {
            try
            {
                this.targetControl = target;

            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public SpotlightPanel(ToolStripItem target)
        {
            try
            {
                this.BackColor = Color.Black;
                this.Opacity = 0.7;
                this.SetStyle(ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (this.targetControl != null)
                {
                    Rectangle targetRect = this.RectangleToClient(this.targetControl.RectangleToScreen(this.targetControl.ClientRectangle));

                    e.Graphics.ExcludeClip(targetRect);
                }

                using (SolidBrush brush = new SolidBrush(Color.FromArgb((Int32)(255 * this.Opacity), this.BackColor)))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }

                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
