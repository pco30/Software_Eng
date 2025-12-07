namespace WinChangeMonitor
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.llAbout = new System.Windows.Forms.LinkLabel();
            this.bOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // llAbout
            // 
            this.llAbout.AutoSize = true;
            this.llAbout.BackColor = System.Drawing.SystemColors.Control;
            this.llAbout.LinkArea = new System.Windows.Forms.LinkArea(18, 37);
            this.llAbout.Location = new System.Drawing.Point(12, 9);
            this.llAbout.Name = "llAbout";
            this.llAbout.Size = new System.Drawing.Size(208, 154);
            this.llAbout.TabIndex = 0;
            this.llAbout.TabStop = true;
            this.llAbout.Text = "WinChangeMonitor\r\nhttps://github.com/pco30/Software_Eng\r\n\r\nBoston University\r\nMET" +
    " CS 673/473 Software Engineering\r\n\r\nGroup 3\r\nAnjian Chen\r\nYeryoung Kim\r\nPrincely" +
    " Oseji\r\nJeff Rose\r\nYu Wu";
            this.llAbout.UseCompatibleTextRendering = true;
            this.llAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAbout_LinkClicked);
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.Location = new System.Drawing.Point(148, 166);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 201);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.llAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About WinChangeMonitor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel llAbout;
        private System.Windows.Forms.Button bOK;
    }
}