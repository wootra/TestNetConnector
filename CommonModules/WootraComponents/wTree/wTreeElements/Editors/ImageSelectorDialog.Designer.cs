namespace WootraComs.wTreeElements.Editors
{
    partial class ImageSelectorDialog
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
            this.Img1 = new System.Windows.Forms.Panel();
            this.Img2 = new System.Windows.Forms.Panel();
            this.T_Width = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.T_Height = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.B_Apply = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Img1
            // 
            this.Img1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Img1.Location = new System.Drawing.Point(12, 12);
            this.Img1.Name = "Img1";
            this.Img1.Size = new System.Drawing.Size(207, 213);
            this.Img1.TabIndex = 0;
            this.Img1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Img1_MouseClick);
            // 
            // Img2
            // 
            this.Img2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Img2.Location = new System.Drawing.Point(319, 12);
            this.Img2.Name = "Img2";
            this.Img2.Size = new System.Drawing.Size(207, 213);
            this.Img2.TabIndex = 0;
            // 
            // T_Width
            // 
            this.T_Width.Location = new System.Drawing.Point(225, 69);
            this.T_Width.Name = "T_Width";
            this.T_Width.Size = new System.Drawing.Size(88, 21);
            this.T_Width.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(226, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width";
            // 
            // T_Height
            // 
            this.T_Height.Location = new System.Drawing.Point(225, 108);
            this.T_Height.Name = "T_Height";
            this.T_Height.Size = new System.Drawing.Size(88, 21);
            this.T_Height.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Height";
            // 
            // B_Apply
            // 
            this.B_Apply.Location = new System.Drawing.Point(228, 164);
            this.B_Apply.Name = "B_Apply";
            this.B_Apply.Size = new System.Drawing.Size(85, 61);
            this.B_Apply.TabIndex = 3;
            this.B_Apply.Text = "Apply";
            this.B_Apply.UseVisualStyleBackColor = true;
            this.B_Apply.Click += new System.EventHandler(this.B_Apply_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "click the image to change...";
            // 
            // ImageSelectorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 255);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.B_Apply);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.T_Height);
            this.Controls.Add(this.T_Width);
            this.Controls.Add(this.Img2);
            this.Controls.Add(this.Img1);
            this.Name = "ImageSelectorDialog";
            this.Text = "ImageSelectorDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Img1;
        private System.Windows.Forms.Panel Img2;
        private System.Windows.Forms.TextBox T_Width;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox T_Height;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button B_Apply;
        private System.Windows.Forms.Label label3;
    }
}