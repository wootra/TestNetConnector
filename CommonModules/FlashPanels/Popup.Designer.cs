namespace FlashPanels
{
    partial class Popup
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.nameValueTextPairList1 = new FormAdders.NameValueTextPairList();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.nameValueTextPairList1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Size = new System.Drawing.Size(284, 262);
            this.splitContainer1.SplitterDistance = 207;
            this.splitContainer1.TabIndex = 0;
            // 
            // nameValueTextPairList1
            // 
            this.nameValueTextPairList1.AutoSize = true;
            this.nameValueTextPairList1.BackColor = System.Drawing.Color.Transparent;
            this.nameValueTextPairList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameValueTextPairList1.Location = new System.Drawing.Point(0, 0);
            this.nameValueTextPairList1.Name = "nameValueTextPairList1";
            this.nameValueTextPairList1.Size = new System.Drawing.Size(284, 207);
            this.nameValueTextPairList1.TabIndex = 1;
            this.nameValueTextPairList1.U_BackColor = System.Drawing.Color.Transparent;
            this.nameValueTextPairList1.U_FixedColWidth = 250;
            this.nameValueTextPairList1.U_FixedValuePosition = 100;
            this.nameValueTextPairList1.U_ForeColors = new System.Drawing.Color[] {
        System.Drawing.Color.Black,
        System.Drawing.Color.Black};
            this.nameValueTextPairList1.U_InitValuesPair = null;
            this.nameValueTextPairList1.U_IsAutoRedraw = false;
            this.nameValueTextPairList1.U_IsColWidthFixed = false;
            this.nameValueTextPairList1.U_IsScrollEnabled = false;
            this.nameValueTextPairList1.U_IsUnderLine = true;
            this.nameValueTextPairList1.U_PercentValuePosition = 50F;
            this.nameValueTextPairList1.U_RowPadding = 5;
            this.nameValueTextPairList1.U_TextBackColorForTransparent = System.Drawing.SystemColors.Window;
            this.nameValueTextPairList1.U_TextBoxBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.nameValueTextPairList1.U_TextHeight = 14;
            this.nameValueTextPairList1.U_TextWidth = 100;
            this.nameValueTextPairList1.U_UnderLineColor = System.Drawing.Color.Gray;
            this.nameValueTextPairList1.U_ValuePosition = FormAdders.NameValueTextPairList.ValuePositionMode.HalfPosition;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(148, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 45);
            this.button1.TabIndex = 0;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Apply_Click);
            // 
            // Popup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Popup";
            this.Text = "Popup";
            this.TopMost = true;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private FormAdders.NameValueTextPairList nameValueTextPairList1;
        private System.Windows.Forms.Button button1;

    }
}