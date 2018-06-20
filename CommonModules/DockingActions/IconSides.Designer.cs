using System.Drawing;

namespace DockingActions
{
    partial class IconSides
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
            this.B_PosBtn = new UCoreComponents.TransparentButton();
            this.SuspendLayout();
            // 
            // B_PosBtn
            // 
            this.B_PosBtn.BackColor = System.Drawing.Color.Transparent;
            this.B_PosBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.B_PosBtn.Icon = null;
            this.B_PosBtn.Location = new System.Drawing.Point(0, 0);
            this.B_PosBtn.Name = "B_PosBtn";
            this.B_PosBtn.Size = new System.Drawing.Size(31, 31);
            this.B_PosBtn.TabIndex = 0;
            this.B_PosBtn.Text = " ";
            // 
            // ShowingIconForRoot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::DockingActions.Properties.Resources.bottom;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(31, 31);
            this.Controls.Add(this.B_PosBtn);
            this.DoubleBuffered = true;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ShowingIconForRoot";
            //this.Opacity = 0.7D;
            //this.Text = "ShowingIcon";
            //this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.BackColor = Color.Transparent;
            this.ResumeLayout(false);

        }

        #endregion

        private UCoreComponents.TransparentButton B_PosBtn;
    }
}