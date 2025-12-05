namespace WinChangeMonitor
{
    partial class RegistryKeyBrowserDialog
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
            this.tvBrowser = new System.Windows.Forms.TreeView();
            this.lSelectedKey = new System.Windows.Forms.Label();
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvBrowser
            // 
            this.tvBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvBrowser.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.tvBrowser.HideSelection = false;
            this.tvBrowser.Location = new System.Drawing.Point(12, 35);
            this.tvBrowser.Name = "tvBrowser";
            this.tvBrowser.Size = new System.Drawing.Size(280, 261);
            this.tvBrowser.TabIndex = 0;
            this.tvBrowser.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvBrowser_BeforeExpand);
            this.tvBrowser.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvBrowser_DrawNode);
            this.tvBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvBrowser_AfterSelect);
            // 
            // lSelectedKey
            // 
            this.lSelectedKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lSelectedKey.Location = new System.Drawing.Point(12, 9);
            this.lSelectedKey.Name = "lSelectedKey";
            this.lSelectedKey.Size = new System.Drawing.Size(280, 23);
            this.lSelectedKey.TabIndex = 1;
            this.lSelectedKey.Text = "label1";
            this.lSelectedKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.Enabled = false;
            this.bOK.Location = new System.Drawing.Point(136, 302);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(217, 302);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // RegistryKeyBrowserDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(304, 337);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.lSelectedKey);
            this.Controls.Add(this.tvBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "RegistryKeyBrowserDialog";
            this.ShowIcon = false;
            this.Text = "Browse For Registry Key";
            this.Load += new System.EventHandler(this.RegistryKeyBrowserDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvBrowser;
        private System.Windows.Forms.Label lSelectedKey;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
    }
}