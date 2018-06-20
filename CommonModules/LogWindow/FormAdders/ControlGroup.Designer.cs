namespace FormAdders
{
    partial class ControlGroup
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
            this.B_Title = new System.Windows.Forms.Button();
            this.P_Back = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // B_Title
            // 
            this.B_Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.B_Title.Image = global::FormAdders.Properties.Resources.tree_close;
            this.B_Title.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.B_Title.Location = new System.Drawing.Point(0, 0);
            this.B_Title.Name = "B_Title";
            this.B_Title.Size = new System.Drawing.Size(233, 23);
            this.B_Title.TabIndex = 7;
            this.B_Title.Text = "Title";
            this.B_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.B_Title.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.B_Title.UseVisualStyleBackColor = true;
            // 
            // P_Back
            // 
            this.P_Back.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.P_Back.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Back.Location = new System.Drawing.Point(0, 23);
            this.P_Back.Name = "P_Back";
            this.P_Back.Size = new System.Drawing.Size(233, 256);
            this.P_Back.TabIndex = 8;
            // 
            // LabelGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_Back);
            this.Controls.Add(this.B_Title);
            this.Name = "LabelGroup";
            this.Size = new System.Drawing.Size(233, 279);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_Title;
        private System.Windows.Forms.Panel P_Back;

    }
}
