namespace GraphModules
{
    partial class SimpleRealtimeGraph
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
            this.Pesgo1 = new Gigasoft.ProEssentials.Pesgo();
            this.SuspendLayout();
            // 
            // Pesgo1
            // 
            this.Pesgo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pesgo1.Location = new System.Drawing.Point(0, 0);
            this.Pesgo1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Pesgo1.Name = "Pesgo1";
            this.Pesgo1.Size = new System.Drawing.Size(319, 252);
            this.Pesgo1.TabIndex = 1;
            this.Pesgo1.Text = "pesgo1";
            // 
            // SimpleRealtimeGraphcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Pesgo1);
            this.Name = "SimpleRealtimeGraphcs";
            this.Size = new System.Drawing.Size(319, 252);
            this.ResumeLayout(false);

        }

        #endregion

        private Gigasoft.ProEssentials.Pesgo Pesgo1;


    }
}
