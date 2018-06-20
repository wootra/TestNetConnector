using Gigasoft.ProEssentials;

namespace GraphModules
{
    partial class MultiLineGraphWinRealTime
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                G_Science.Dispose();
            }
            catch { }
            if (_savingFile != null)
            {
                try
                {
                    _savingFile.Close();
                }
                catch { }
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.G_Science = new Gigasoft.ProEssentials.Pesgo();
            this.T_TopDown = new System.Windows.Forms.TableLayoutPanel();
            this.P_TitleBar = new System.Windows.Forms.Panel();
            this.B_SnapShot = new System.Windows.Forms.Button();
            this.B_Close = new System.Windows.Forms.Button();
            this.C_GraphName = new System.Windows.Forms.CheckBox();
            this.T_TopDown.SuspendLayout();
            this.P_TitleBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // G_Science
            // 
            this.G_Science.Dock = System.Windows.Forms.DockStyle.Fill;
            this.G_Science.Location = new System.Drawing.Point(1, 21);
            this.G_Science.Margin = new System.Windows.Forms.Padding(1);
            this.G_Science.Name = "G_Science";
            this.G_Science.Size = new System.Drawing.Size(283, 192);
            this.G_Science.TabIndex = 0;
            this.G_Science.Text = "pesgo1";
            // 
            // T_TopDown
            // 
            this.T_TopDown.ColumnCount = 1;
            this.T_TopDown.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.T_TopDown.Controls.Add(this.G_Science, 0, 1);
            this.T_TopDown.Controls.Add(this.P_TitleBar, 0, 0);
            this.T_TopDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.T_TopDown.Location = new System.Drawing.Point(0, 0);
            this.T_TopDown.Name = "T_TopDown";
            this.T_TopDown.RowCount = 2;
            this.T_TopDown.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.T_TopDown.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.T_TopDown.Size = new System.Drawing.Size(285, 214);
            this.T_TopDown.TabIndex = 1;
            // 
            // P_TitleBar
            // 
            this.P_TitleBar.BackColor = System.Drawing.Color.CornflowerBlue;
            this.P_TitleBar.Controls.Add(this.B_SnapShot);
            this.P_TitleBar.Controls.Add(this.B_Close);
            this.P_TitleBar.Controls.Add(this.C_GraphName);
            this.P_TitleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_TitleBar.Location = new System.Drawing.Point(1, 1);
            this.P_TitleBar.Margin = new System.Windows.Forms.Padding(1);
            this.P_TitleBar.Name = "P_TitleBar";
            this.P_TitleBar.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.P_TitleBar.Size = new System.Drawing.Size(283, 18);
            this.P_TitleBar.TabIndex = 1;
            // 
            // B_SnapShot
            // 
            this.B_SnapShot.Dock = System.Windows.Forms.DockStyle.Right;
            this.B_SnapShot.FlatAppearance.BorderSize = 0;
            this.B_SnapShot.Font = new System.Drawing.Font("돋움", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.B_SnapShot.Location = new System.Drawing.Point(190, 0);
            this.B_SnapShot.Margin = new System.Windows.Forms.Padding(0);
            this.B_SnapShot.Name = "B_SnapShot";
            this.B_SnapShot.Size = new System.Drawing.Size(75, 18);
            this.B_SnapShot.TabIndex = 8;
            this.B_SnapShot.Text = "snap shot";
            this.B_SnapShot.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.B_SnapShot.UseVisualStyleBackColor = true;
            // 
            // B_Close
            // 
            this.B_Close.Dock = System.Windows.Forms.DockStyle.Right;
            this.B_Close.Location = new System.Drawing.Point(265, 0);
            this.B_Close.Margin = new System.Windows.Forms.Padding(10);
            this.B_Close.Name = "B_Close";
            this.B_Close.Size = new System.Drawing.Size(18, 18);
            this.B_Close.TabIndex = 7;
            this.B_Close.Text = "X";
            this.B_Close.UseVisualStyleBackColor = true;
            // 
            // C_GraphName
            // 
            this.C_GraphName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.C_GraphName.AutoSize = true;
            this.C_GraphName.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.C_GraphName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.C_GraphName.Location = new System.Drawing.Point(1, 0);
            this.C_GraphName.Margin = new System.Windows.Forms.Padding(0);
            this.C_GraphName.Name = "C_GraphName";
            this.C_GraphName.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.C_GraphName.Size = new System.Drawing.Size(63, 15);
            this.C_GraphName.TabIndex = 6;
            this.C_GraphName.Text = "Graph";
            this.C_GraphName.UseVisualStyleBackColor = true;
            // 
            // MultiLineGraphWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.T_TopDown);
            this.Name = "MultiLineGraphWin";
            this.Size = new System.Drawing.Size(285, 214);
            this.T_TopDown.ResumeLayout(false);
            this.P_TitleBar.ResumeLayout(false);
            this.P_TitleBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Pesgo G_Science;
        private System.Windows.Forms.TableLayoutPanel T_TopDown;
        private System.Windows.Forms.Panel P_TitleBar;
        private System.Windows.Forms.Button B_SnapShot;
        private System.Windows.Forms.Button B_Close;
        private System.Windows.Forms.CheckBox C_GraphName;

    }
}
