namespace TestNetConnector
{
    partial class ConMsgList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConMsgList));
            this.P_Table = new System.Windows.Forms.TableLayoutPanel();
            this.V_Data = new FormAdders.EasyGridView();
            this.V_Contents = new FormAdders.EasyGridView();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_HideButton = new FormAdders.ImgButton2();
            this.P_Table.SuspendLayout();
            this.SuspendLayout();
            // 
            // P_Table
            // 
            this.P_Table.ColumnCount = 1;
            this.P_Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.P_Table.Controls.Add(this.V_Data, 0, 0);
            this.P_Table.Controls.Add(this.V_Contents, 0, 2);
            this.P_Table.Controls.Add(this.B_Save, 0, 3);
            this.P_Table.Controls.Add(this.B_HideButton, 0, 1);
            this.P_Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Table.Location = new System.Drawing.Point(0, 0);
            this.P_Table.Name = "P_Table";
            this.P_Table.RowCount = 4;
            this.P_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.P_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.P_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.P_Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.P_Table.Size = new System.Drawing.Size(247, 407);
            this.P_Table.TabIndex = 0;
            // 
            // V_Data
            // 
            this.V_Data.ActionOnClicked = FormAdders.EasyGridViewCollections.Actions.Nothing;
            this.V_Data.ActionOnCtrlMoveKey = FormAdders.EasyGridViewCollections.ActionsOnCtrl_MoveKey.SelectRowsMove;
            this.V_Data.ActionOnDoubleClicked = FormAdders.EasyGridViewCollections.Actions.CheckBoxChecked;
            this.V_Data.ActionOnEnterInEditMode = FormAdders.EasyGridViewCollections.EnterActions.EditOnThePosition;
            this.V_Data.ActionOnRightClicked = FormAdders.EasyGridViewCollections.Actions.ContextMenu;
            this.V_Data.BaseRowHeight = 27;
            this.V_Data.ClearLinesWhenMaxLines = 100;
            this.V_Data.ColumnHeaderHeight = 4;
            this.V_Data.DataSource = null;
            this.V_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.V_Data.FirstDisplayedScrollingColIndex = -1;
            this.V_Data.FirstDisplayedScrollingRowIndex = -1;
            this.V_Data.IsTitleVisible = true;
            this.V_Data.Location = new System.Drawing.Point(0, 0);
            this.V_Data.Margin = new System.Windows.Forms.Padding(0);
            this.V_Data.MaxLines = 1000;
            this.V_Data.MultiSelect = true;
            this.V_Data.Name = "V_Data";
            this.V_Data.RowHeaderWidth = 30;
            this.V_Data.ScrollToLastRowWhenAddNew = true;
            this.V_Data.SelectedRowsIndice = ((System.Collections.Generic.List<int>)(resources.GetObject("V_Data.SelectedRowsIndice")));
            this.V_Data.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.V_Data.Size = new System.Drawing.Size(247, 367);
            this.V_Data.TabIndex = 1;
            this.V_Data.TextAlignMode = FormAdders.EasyGridViewCollections.TextAlignModes.NumberOnlyRight;
            this.V_Data.TextViewMode = FormAdders.EasyGridViewCollections.TextViewModes.MultiLines;
            this.V_Data.U_BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.V_Data.U_GridColor = System.Drawing.SystemColors.ControlDark;
            this.V_Data.U_IndexWidth = 30;
            this.V_Data.U_ShowIndex = false;
            // 
            // V_Contents
            // 
            this.V_Contents.ActionOnClicked = FormAdders.EasyGridViewCollections.Actions.Nothing;
            this.V_Contents.ActionOnCtrlMoveKey = FormAdders.EasyGridViewCollections.ActionsOnCtrl_MoveKey.SelectRowsMove;
            this.V_Contents.ActionOnDoubleClicked = FormAdders.EasyGridViewCollections.Actions.CheckBoxChecked;
            this.V_Contents.ActionOnEnterInEditMode = FormAdders.EasyGridViewCollections.EnterActions.EditOnThePosition;
            this.V_Contents.ActionOnRightClicked = FormAdders.EasyGridViewCollections.Actions.ContextMenu;
            this.V_Contents.BaseRowHeight = 27;
            this.V_Contents.ClearLinesWhenMaxLines = 10;
            this.V_Contents.ColumnHeaderHeight = 4;
            this.V_Contents.DataSource = null;
            this.V_Contents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.V_Contents.FirstDisplayedScrollingColIndex = -1;
            this.V_Contents.FirstDisplayedScrollingRowIndex = -1;
            this.V_Contents.IsTitleVisible = true;
            this.V_Contents.Location = new System.Drawing.Point(0, 385);
            this.V_Contents.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.V_Contents.MaxLines = 1000;
            this.V_Contents.MultiSelect = true;
            this.V_Contents.Name = "V_Contents";
            this.V_Contents.RowHeaderWidth = 30;
            this.V_Contents.ScrollToLastRowWhenAddNew = true;
            this.V_Contents.SelectedRowsIndice = ((System.Collections.Generic.List<int>)(resources.GetObject("V_Contents.SelectedRowsIndice")));
            this.V_Contents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.V_Contents.Size = new System.Drawing.Size(247, 2);
            this.V_Contents.TabIndex = 1;
            this.V_Contents.TextAlignMode = FormAdders.EasyGridViewCollections.TextAlignModes.NumberOnlyRight;
            this.V_Contents.TextViewMode = FormAdders.EasyGridViewCollections.TextViewModes.MultiLines;
            this.V_Contents.U_BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.V_Contents.U_GridColor = System.Drawing.SystemColors.ControlDark;
            this.V_Contents.U_IndexWidth = 30;
            this.V_Contents.U_ShowIndex = false;
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(0, 387);
            this.B_Save.Margin = new System.Windows.Forms.Padding(0);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(247, 20);
            this.B_Save.TabIndex = 2;
            this.B_Save.Text = "변경사항 저장";
            this.B_Save.UseVisualStyleBackColor = true;
            // 
            // B_HideButton
            // 
            this.B_HideButton.ActiveBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.B_HideButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.B_HideButton.BorderStyle = FormAdders.ImgButton2.BorderStyles.None;
            this.B_HideButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.B_HideButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.B_HideButton.Location = new System.Drawing.Point(0, 367);
            this.B_HideButton.Margin = new System.Windows.Forms.Padding(0);
            this.B_HideButton.Name = "B_HideButton";
            this.B_HideButton.Size = new System.Drawing.Size(247, 15);
            this.B_HideButton.TabIndex = 3;
            this.B_HideButton.Text = "▼";
            this.B_HideButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.B_HideButton.U_Border3DStyle = FormAdders.ImgButton2.Border3DStyles.Up;
            this.B_HideButton.U_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.B_HideButton.U_ButtonDisabledType = 0;
            this.B_HideButton.U_ButtonTypeIndex = 0;
            this.B_HideButton.U_ImageIndex = -1;
            this.B_HideButton.U_TextPosition = System.Windows.Forms.AnchorStyles.None;
            // 
            // ConMsgList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_Table);
            this.Name = "ConMsgList";
            this.Size = new System.Drawing.Size(247, 407);
            this.P_Table.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel P_Table;
        private FormAdders.EasyGridView V_Data;
        private FormAdders.EasyGridView V_Contents;
        private System.Windows.Forms.Button B_Save;
        private FormAdders.ImgButton2 B_HideButton;

    }
}
