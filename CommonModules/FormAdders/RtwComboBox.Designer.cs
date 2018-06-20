namespace FormAdders
{
    partial class RtwComboBox
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.B_TextArea = new System.Windows.Forms.Button();
            this.B_Expand = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.B_TextArea, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.B_Expand, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(265, 37);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // B_TextArea
            // 
            this.B_TextArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.B_TextArea.Location = new System.Drawing.Point(0, 0);
            this.B_TextArea.Margin = new System.Windows.Forms.Padding(0);
            this.B_TextArea.Name = "B_TextArea";
            this.B_TextArea.Size = new System.Drawing.Size(245, 37);
            this.B_TextArea.TabIndex = 0;
            this.B_TextArea.Text = "ComboBox";
            this.B_TextArea.UseVisualStyleBackColor = true;
            // 
            // B_Expand
            // 
            this.B_Expand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.B_Expand.Location = new System.Drawing.Point(245, 0);
            this.B_Expand.Margin = new System.Windows.Forms.Padding(0);
            this.B_Expand.Name = "B_Expand";
            this.B_Expand.Size = new System.Drawing.Size(20, 37);
            this.B_Expand.TabIndex = 1;
            this.B_Expand.Text = "▼";
            this.B_Expand.UseVisualStyleBackColor = true;
            // 
            // RtwComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RtwComboBox";
            this.Size = new System.Drawing.Size(265, 37);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button B_TextArea;
        private System.Windows.Forms.Button B_Expand;
    }
}
