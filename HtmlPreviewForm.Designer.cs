namespace WinChangeMonitor
{
    partial class HtmlPreviewForm
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
            this.ReportPreviewBrowser = new System.Windows.Forms.WebBrowser();
            this.btViewExternal = new System.Windows.Forms.Button();
            this.btExportJson = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReportPreviewBrowser
            // 
            this.ReportPreviewBrowser.Location = new System.Drawing.Point(12, 49);
            this.ReportPreviewBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.ReportPreviewBrowser.Name = "ReportPreviewBrowser";
            this.ReportPreviewBrowser.Size = new System.Drawing.Size(776, 654);
            this.ReportPreviewBrowser.TabIndex = 0;
            this.ReportPreviewBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.ReportPreviewBrowser_DocumentCompleted);
            // 
            // btViewExternal
            // 
            this.btViewExternal.Location = new System.Drawing.Point(12, 12);
            this.btViewExternal.Name = "btViewExternal";
            this.btViewExternal.Size = new System.Drawing.Size(185, 31);
            this.btViewExternal.TabIndex = 1;
            this.btViewExternal.Text = "View in External Browser";
            this.btViewExternal.UseVisualStyleBackColor = true;
            this.btViewExternal.Click += new System.EventHandler(this.btViewExternal_Click);
            // 
            // btExportJson
            // 
            this.btExportJson.Location = new System.Drawing.Point(203, 12);
            this.btExportJson.Name = "btExportJson";
            this.btExportJson.Size = new System.Drawing.Size(141, 31);
            this.btExportJson.TabIndex = 2;
            this.btExportJson.Text = "Export as Json";
            this.btExportJson.UseVisualStyleBackColor = true;
            this.btExportJson.Click += new System.EventHandler(this.btExportJson_Click);
            // 
            // HtmlPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 715);
            this.Controls.Add(this.btExportJson);
            this.Controls.Add(this.btViewExternal);
            this.Controls.Add(this.ReportPreviewBrowser);
            this.Name = "HtmlPreviewForm";
            this.Text = "HtmlPreviewForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser ReportPreviewBrowser;
        private System.Windows.Forms.Button btViewExternal;
        private System.Windows.Forms.Button btExportJson;
    }
}