namespace DockingActions
{
    partial class TitleBar
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
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.B_X = new UCoreComponents.TransparentButton();
            this.B_Pin = new UCoreComponents.TransparentButton();
            this.B_DownArrow = new UCoreComponents.TransparentButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.B_X);
            this.panel1.Controls.Add(this.B_Pin);
            this.panel1.Controls.Add(this.B_DownArrow);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(391, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(68, 18);
            this.panel1.TabIndex = 1;
            // 
            // B_X
            // 
            this.B_X.BackColor = System.Drawing.Color.Transparent;
            this.B_X.BackgroundImage = global::DockingActions.Properties.Resources.xBtn;
            this.B_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_X.ForeColor = System.Drawing.SystemColors.ControlText;
            this.B_X.Icon = null;
            this.B_X.Location = new System.Drawing.Point(48, 0);
            this.B_X.Name = "B_X";
            this.B_X.Size = new System.Drawing.Size(18, 18);
            this.B_X.TabIndex = 0;
            // 
            // B_Pin
            // 
            this.B_Pin.BackColor = System.Drawing.Color.Transparent;
            this.B_Pin.BackgroundImage = global::DockingActions.Properties.Resources.pin;
            this.B_Pin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Pin.ForeColor = System.Drawing.SystemColors.ControlText;
            this.B_Pin.Icon = null;
            this.B_Pin.Location = new System.Drawing.Point(24, 0);
            this.B_Pin.Name = "B_Pin";
            this.B_Pin.Size = new System.Drawing.Size(18, 18);
            this.B_Pin.TabIndex = 0;
            // 
            // B_DownArrow
            // 
            this.B_DownArrow.BackColor = System.Drawing.Color.Transparent;
            this.B_DownArrow.BackgroundImage = global::DockingActions.Properties.Resources.downArrow;
            this.B_DownArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_DownArrow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.B_DownArrow.Icon = null;
            this.B_DownArrow.Location = new System.Drawing.Point(0, 0);
            this.B_DownArrow.Name = "B_DownArrow";
            this.B_DownArrow.Size = new System.Drawing.Size(18, 18);
            this.B_DownArrow.TabIndex = 0;
            // 
            // TitleBar
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(70)))), ((int)(((byte)(109)))));
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(459, 18);
            this.TextAlign = UCoreComponents.TransparentButton.Align.Left;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private UCoreComponents.TransparentButton B_DownArrow;
        private UCoreComponents.TransparentButton B_X;
        private UCoreComponents.TransparentButton B_Pin;
    }
}
