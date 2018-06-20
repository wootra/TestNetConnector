namespace TestNetConnector
{
    partial class DlgViewStruct
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
            this.Tr_Struct = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // Tr_Struct
            // 
            this.Tr_Struct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tr_Struct.Location = new System.Drawing.Point(0, 0);
            this.Tr_Struct.Name = "Tr_Struct";
            this.Tr_Struct.Size = new System.Drawing.Size(501, 615);
            this.Tr_Struct.TabIndex = 0;
            // 
            // DlgViewStruct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 615);
            this.Controls.Add(this.Tr_Struct);
            this.Name = "DlgViewStruct";
            this.Text = "DlgViewStruct";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView Tr_Struct;
    }
}