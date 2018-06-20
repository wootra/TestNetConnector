namespace UserDialogs
{
    partial class MultiInputForm
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
            this.B_OK = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.P_Buttons = new System.Windows.Forms.Panel();
            this.P_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_OK
            // 
            this.B_OK.Location = new System.Drawing.Point(145, 6);
            this.B_OK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_OK.Name = "B_OK";
            this.B_OK.Size = new System.Drawing.Size(59, 29);
            this.B_OK.TabIndex = 1;
            this.B_OK.Text = "OK";
            this.B_OK.UseVisualStyleBackColor = true;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(212, 6);
            this.B_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(70, 29);
            this.B_Cancel.TabIndex = 1;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            // 
            // P_Buttons
            // 
            this.P_Buttons.Controls.Add(this.B_OK);
            this.P_Buttons.Controls.Add(this.B_Cancel);
            this.P_Buttons.Location = new System.Drawing.Point(12, 100);
            this.P_Buttons.Name = "P_Buttons";
            this.P_Buttons.Size = new System.Drawing.Size(292, 39);
            this.P_Buttons.TabIndex = 2;
            // 
            // MultiInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 149);
            this.Controls.Add(this.P_Buttons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MultiInputForm";
            this.Text = "Caption";
            this.P_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_OK;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Panel P_Buttons;
    }
}