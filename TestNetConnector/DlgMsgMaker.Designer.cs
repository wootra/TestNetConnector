namespace TestNetConnector
{
    partial class DlgMsgMaker
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
            this.B_Save = new System.Windows.Forms.Button();
            this.T_Msg = new System.Windows.Forms.RichTextBox();
            this.B_Help = new System.Windows.Forms.Button();
            this.T_Comment = new System.Windows.Forms.TextBox();
            this.L_Comment = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel1.Controls.Add(this.L_Comment);
            this.splitContainer1.Panel1.Controls.Add(this.T_Comment);
            this.splitContainer1.Panel1.Controls.Add(this.B_Help);
            this.splitContainer1.Panel1.Controls.Add(this.B_Save);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.T_Msg);
            this.splitContainer1.Size = new System.Drawing.Size(481, 628);
            this.splitContainer1.SplitterDistance = 43;
            this.splitContainer1.TabIndex = 0;
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(374, 3);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(104, 36);
            this.B_Save.TabIndex = 0;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // T_Msg
            // 
            this.T_Msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.T_Msg.Location = new System.Drawing.Point(0, 0);
            this.T_Msg.Name = "T_Msg";
            this.T_Msg.Size = new System.Drawing.Size(481, 581);
            this.T_Msg.TabIndex = 0;
            this.T_Msg.Text = "";
            // 
            // B_Help
            // 
            this.B_Help.Location = new System.Drawing.Point(275, 3);
            this.B_Help.Name = "B_Help";
            this.B_Help.Size = new System.Drawing.Size(93, 36);
            this.B_Help.TabIndex = 1;
            this.B_Help.Text = "Help";
            this.B_Help.UseVisualStyleBackColor = true;
            // 
            // T_Comment
            // 
            this.T_Comment.Location = new System.Drawing.Point(78, 12);
            this.T_Comment.Name = "T_Comment";
            this.T_Comment.Size = new System.Drawing.Size(191, 21);
            this.T_Comment.TabIndex = 2;
            // 
            // L_Comment
            // 
            this.L_Comment.AutoSize = true;
            this.L_Comment.Location = new System.Drawing.Point(12, 15);
            this.L_Comment.Name = "L_Comment";
            this.L_Comment.Size = new System.Drawing.Size(60, 12);
            this.L_Comment.TabIndex = 3;
            this.L_Comment.Text = "Comment";
            // 
            // DlgMsgMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 628);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DlgMsgMaker";
            this.Text = "DlgMsgMaker";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.RichTextBox T_Msg;
        private System.Windows.Forms.Label L_Comment;
        private System.Windows.Forms.TextBox T_Comment;
        private System.Windows.Forms.Button B_Help;
    }
}