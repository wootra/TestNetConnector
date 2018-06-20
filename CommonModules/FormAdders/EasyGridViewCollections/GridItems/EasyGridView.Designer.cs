namespace FormAdders
{
    partial class EasyGridView
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
            this.V_Data = new FormAdders.EasyGridViewCollections.EasyGridViewParent();
//            ((System.ComponentModel.ISupportInitialize)(this.V_Data)).BeginInit();
            //this.SuspendLayout();
            // 
            // V_Data
            // 
            this.V_Data.ActionOnEnterInEditMode = FormAdders.EasyGridViewCollections.EnterActions.EditOnThePosition;
            this.V_Data.AllowUserToAddRows = false;
            this.V_Data.AllowUserToDeleteRows = false;
            this.V_Data.AllowUserToResizeRows = false;
            this.V_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.V_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.V_Data.Location = new System.Drawing.Point(0, 0);
            this.V_Data.Name = "V_Data";
            this.V_Data.RowHeadersVisible = false;
            this.V_Data.RowTemplate.Height = 23;
            this.V_Data.Size = new System.Drawing.Size(150, 150);
            this.V_Data.TabIndex = 0;
            // 
            // EasyGridView
            // 
            this.Controls.Add(this.V_Data);
            this.Name = "EasyGridView";
            //((System.ComponentModel.ISupportInitialize)(this.V_Data)).EndInit();
            //this.ResumeLayout(false);

        }

        #endregion

        private FormAdders.EasyGridViewCollections.EasyGridViewParent V_Data;
    }
}
