namespace TestNetConnector
{
    partial class Con_PacketGroups
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
            this.TR_Groups = new FormAdders.RtwTreeView2();
            this.SuspendLayout();
            // 
            // TR_Groups
            // 
            this.TR_Groups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TR_Groups.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.TR_Groups.ImageIndex = 0;
            this.TR_Groups.Location = new System.Drawing.Point(0, 0);
            this.TR_Groups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TR_Groups.Name = "TR_Groups";
            this.TR_Groups.SelectedImageIndex = 0;
            this.TR_Groups.SelectionEventMode = FormAdders.RtwTreeView2.SelectionEventModes.IndexSelection;
            this.TR_Groups.ShowCheckBoxes = false;
            this.TR_Groups.ShowLines = false;
            this.TR_Groups.ShowPlusMinus = false;
            this.TR_Groups.ShowRootLines = false;
            this.TR_Groups.Size = new System.Drawing.Size(244, 284);
            this.TR_Groups.TabIndex = 0;
            // 
            // Con_PacketGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TR_Groups);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Con_PacketGroups";
            this.Size = new System.Drawing.Size(244, 284);
            this.ResumeLayout(false);

        }

        #endregion

        private FormAdders.RtwTreeView2 TR_Groups;
    }
}
