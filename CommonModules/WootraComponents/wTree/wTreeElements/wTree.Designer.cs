namespace WootraComs
{
    partial class wTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Scroll_Vertical = new System.Windows.Forms.VScrollBar();
            this.Scroll_Horizontal = new System.Windows.Forms.HScrollBar();
            this.B_FocusGetter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Scroll_Vertical
            // 
            this.Scroll_Vertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.Scroll_Vertical.Location = new System.Drawing.Point(471, 0);
            this.Scroll_Vertical.Name = "Scroll_Vertical";
            this.Scroll_Vertical.Size = new System.Drawing.Size(17, 392);
            this.Scroll_Vertical.TabIndex = 0;
            // 
            // Scroll_Horizontal
            // 
            this.Scroll_Horizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Scroll_Horizontal.Location = new System.Drawing.Point(0, 375);
            this.Scroll_Horizontal.Name = "Scroll_Horizontal";
            this.Scroll_Horizontal.Size = new System.Drawing.Size(471, 17);
            this.Scroll_Horizontal.TabIndex = 1;
            // 
            // B_FocusGetter
            // 
            this.B_FocusGetter.Location = new System.Drawing.Point(452, 0);
            this.B_FocusGetter.Name = "B_FocusGetter";
            this.B_FocusGetter.Size = new System.Drawing.Size(18, 20);
            this.B_FocusGetter.TabIndex = 2;
            this.B_FocusGetter.Text = "button1";
            this.B_FocusGetter.UseVisualStyleBackColor = true;
            // 
            // wTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.B_FocusGetter);
            this.Controls.Add(this.Scroll_Horizontal);
            this.Controls.Add(this.Scroll_Vertical);
            this.Name = "wTree";
            this.Size = new System.Drawing.Size(488, 392);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.VScrollBar Scroll_Vertical;
        internal System.Windows.Forms.HScrollBar Scroll_Horizontal;
        internal System.Windows.Forms.Button B_FocusGetter;


    }
}
