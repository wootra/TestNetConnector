using System.Windows.Forms;

namespace FormAdders
{
    public partial class IPAddressInputBox
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
            this.BackPanel = new System.Windows.Forms.Panel();
            this.Ip0 = new System.Windows.Forms.MaskedTextBox();
            this.L_p1 = new System.Windows.Forms.Label();
            this.Ip1 = new System.Windows.Forms.MaskedTextBox();
            this.L_p2 = new System.Windows.Forms.Label();
            this.Ip2 = new System.Windows.Forms.MaskedTextBox();
            this.L_p3 = new System.Windows.Forms.Label();
            this.Ip3 = new System.Windows.Forms.MaskedTextBox();
            this.BackPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BackPanel
            // 
            this.BackPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BackPanel.Controls.Add(this.Ip0);
            this.BackPanel.Controls.Add(this.L_p1);
            this.BackPanel.Controls.Add(this.Ip1);
            this.BackPanel.Controls.Add(this.L_p2);
            this.BackPanel.Controls.Add(this.Ip2);
            this.BackPanel.Controls.Add(this.L_p3);
            this.BackPanel.Controls.Add(this.Ip3);
            this.BackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackPanel.Location = new System.Drawing.Point(0, 0);
            this.BackPanel.Name = "BackPanel";
            this.BackPanel.Size = new System.Drawing.Size(153, 26);
            this.BackPanel.TabIndex = 1;
            // 
            // Ip0
            // 
            this.Ip0.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Ip0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Ip0.Location = new System.Drawing.Point(5, 6);
            this.Ip0.Margin = new System.Windows.Forms.Padding(0);
            this.Ip0.Mask = "000";
            this.Ip0.Name = "Ip0";
            this.Ip0.PromptChar = ' ';
            this.Ip0.Size = new System.Drawing.Size(19, 14);
            this.Ip0.TabIndex = 11;
            this.Ip0.Tag = "0";
            this.Ip0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // L_p1
            // 
            this.L_p1.AutoSize = true;
            this.L_p1.Location = new System.Drawing.Point(30, 3);
            this.L_p1.Name = "L_p1";
            this.L_p1.Size = new System.Drawing.Size(9, 12);
            this.L_p1.TabIndex = 13;
            this.L_p1.Text = ".";
            // 
            // Ip1
            // 
            this.Ip1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Ip1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Ip1.Location = new System.Drawing.Point(45, 6);
            this.Ip1.Margin = new System.Windows.Forms.Padding(0);
            this.Ip1.Mask = "000";
            this.Ip1.Name = "Ip1";
            this.Ip1.PromptChar = ' ';
            this.Ip1.Size = new System.Drawing.Size(19, 14);
            this.Ip1.TabIndex = 12;
            this.Ip1.Tag = "1";
            this.Ip1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // L_p2
            // 
            this.L_p2.AutoSize = true;
            this.L_p2.Location = new System.Drawing.Point(70, 3);
            this.L_p2.Name = "L_p2";
            this.L_p2.Size = new System.Drawing.Size(9, 12);
            this.L_p2.TabIndex = 15;
            this.L_p2.Text = ".";
            // 
            // Ip2
            // 
            this.Ip2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Ip2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Ip2.Location = new System.Drawing.Point(85, 6);
            this.Ip2.Margin = new System.Windows.Forms.Padding(0);
            this.Ip2.Mask = "000";
            this.Ip2.Name = "Ip2";
            this.Ip2.PromptChar = ' ';
            this.Ip2.Size = new System.Drawing.Size(19, 14);
            this.Ip2.TabIndex = 16;
            this.Ip2.Tag = "2";
            this.Ip2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // L_p3
            // 
            this.L_p3.AutoSize = true;
            this.L_p3.Location = new System.Drawing.Point(110, 3);
            this.L_p3.Name = "L_p3";
            this.L_p3.Size = new System.Drawing.Size(9, 12);
            this.L_p3.TabIndex = 14;
            this.L_p3.Text = ".";
            // 
            // Ip3
            // 
            this.Ip3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Ip3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Ip3.Location = new System.Drawing.Point(125, 6);
            this.Ip3.Margin = new System.Windows.Forms.Padding(0);
            this.Ip3.Mask = "000";
            this.Ip3.Name = "Ip3";
            this.Ip3.PromptChar = ' ';
            this.Ip3.Size = new System.Drawing.Size(19, 14);
            this.Ip3.TabIndex = 17;
            this.Ip3.Tag = "3";
            this.Ip3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // IPAddressInputBox
            // 
            this.AccessibleDescription = "IPAddressInputBox";
            this.AccessibleName = "IPAddressInputBox";
            this.AutoSize = true;
            this.Controls.Add(this.BackPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(120, 20);
            this.Name = "IPAddressInputBox";
            this.Size = new System.Drawing.Size(153, 26);
            this.BackPanel.ResumeLayout(false);
            this.BackPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel BackPanel;
        private MaskedTextBox Ip0;
        private Label L_p1;
        private MaskedTextBox Ip1;
        private Label L_p2;
        private MaskedTextBox Ip2;
        private Label L_p3;
        private MaskedTextBox Ip3;

    }
}
