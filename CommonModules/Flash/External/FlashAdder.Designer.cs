namespace Flash.External
{
    partial class FlashAdder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlashAdder));
            this.FlashMovie = new AxShockwaveFlashObjects.AxShockwaveFlash();
            
            this.SuspendLayout();
            
            ((System.ComponentModel.ISupportInitialize)(this.FlashMovie)).BeginInit();
            // 
            // FlashMovie
            // 
            this.FlashMovie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlashMovie.Enabled = true;
            this.FlashMovie.Location = new System.Drawing.Point(0, 0);
            this.FlashMovie.Name = "FlashMovie";
            this.FlashMovie.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("FlashMovie.OcxState")));
            this.FlashMovie.Size = new System.Drawing.Size(150, 150);
            this.FlashMovie.TabIndex = 0;
            // 
            // FlashAdder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FlashMovie);
            this.Name = "FlashAdder";
            ((System.ComponentModel.ISupportInitialize)(this.FlashMovie)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxShockwaveFlashObjects.AxShockwaveFlash FlashMovie;



    }
}
