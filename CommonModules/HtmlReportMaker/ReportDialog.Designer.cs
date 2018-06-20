namespace HtmlReportMaker
{
    partial class ReportDialog
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
            this.ReportView = new System.Windows.Forms.WebBrowser();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.Tool_Print = new System.Windows.Forms.ToolStrip();
            this.B_ToolPrint = new System.Windows.Forms.ToolStripButton();
            this.B_ToolHtml = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.Tool_Print.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReportView
            // 
            this.ReportView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportView.Location = new System.Drawing.Point(0, 0);
            this.ReportView.MinimumSize = new System.Drawing.Size(20, 20);
            this.ReportView.Name = "ReportView";
            this.ReportView.Size = new System.Drawing.Size(779, 538);
            this.ReportView.TabIndex = 0;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.ReportView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(779, 538);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(779, 563);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.Tool_Print);
            // 
            // Tool_Print
            // 
            this.Tool_Print.Dock = System.Windows.Forms.DockStyle.None;
            this.Tool_Print.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.B_ToolPrint,
            this.B_ToolHtml});
            this.Tool_Print.Location = new System.Drawing.Point(3, 0);
            this.Tool_Print.Name = "Tool_Print";
            this.Tool_Print.Size = new System.Drawing.Size(89, 25);
            this.Tool_Print.TabIndex = 0;
            // 
            // B_ToolPrint
            // 
            this.B_ToolPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.B_ToolPrint.Image = global::HtmlReportMaker.Properties.Resources.printer;
            this.B_ToolPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.B_ToolPrint.Name = "B_ToolPrint";
            this.B_ToolPrint.Size = new System.Drawing.Size(23, 22);
            this.B_ToolPrint.Text = "Print";
            this.B_ToolPrint.Click += new System.EventHandler(this.B_ToolPrint_Click);
            // 
            // B_ToolHtml
            // 
            this.B_ToolHtml.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.B_ToolHtml.Image = global::HtmlReportMaker.Properties.Resources.save;
            this.B_ToolHtml.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.B_ToolHtml.Name = "B_ToolHtml";
            this.B_ToolHtml.Size = new System.Drawing.Size(23, 22);
            this.B_ToolHtml.Text = "Make Html";
            this.B_ToolHtml.Click += new System.EventHandler(this.B_ToolHtml_Click);
            // 
            // ReportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 563);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "ReportDialog";
            this.Text = "ReportDialog";
            this.TopMost = true;
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.Tool_Print.ResumeLayout(false);
            this.Tool_Print.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser ReportView;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip Tool_Print;
        private System.Windows.Forms.ToolStripButton B_ToolPrint;
        private System.Windows.Forms.ToolStripButton B_ToolHtml;

    }
}