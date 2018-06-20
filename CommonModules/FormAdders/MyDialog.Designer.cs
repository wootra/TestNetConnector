namespace FormAdders
{
    partial class MyDialog
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
            this.L_TextArea = new System.Windows.Forms.Label();
            this.P_Buttons = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // L_TextArea
            // 
            this.L_TextArea.AutoSize = true;
            this.L_TextArea.Location = new System.Drawing.Point(13, 13);
            this.L_TextArea.Name = "L_TextArea";
            this.L_TextArea.Size = new System.Drawing.Size(38, 12);
            this.L_TextArea.TabIndex = 0;
            this.L_TextArea.Text = "label1";
            // 
            // P_Buttons
            // 
            this.P_Buttons.Location = new System.Drawing.Point(14, 72);
            this.P_Buttons.Name = "P_Buttons";
            this.P_Buttons.Size = new System.Drawing.Size(200, 100);
            this.P_Buttons.TabIndex = 1;
            // 
            // MyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.P_Buttons);
            this.Controls.Add(this.L_TextArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MyDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MyDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label L_TextArea;
        private System.Windows.Forms.Panel P_Buttons;


    }
}