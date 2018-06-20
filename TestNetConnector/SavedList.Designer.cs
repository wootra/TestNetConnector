namespace TestNetConnector
{
    partial class SavedList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SavedList));
            this.panel1 = new System.Windows.Forms.Panel();
            this.C_OnTop = new System.Windows.Forms.CheckBox();
            this.V_Data = new FormAdders.EasyGridView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.C_OnTop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 27);
            this.panel1.TabIndex = 1;
            // 
            // C_OnTop
            // 
            this.C_OnTop.AutoSize = true;
            this.C_OnTop.Location = new System.Drawing.Point(4, 4);
            this.C_OnTop.Name = "C_OnTop";
            this.C_OnTop.Size = new System.Drawing.Size(62, 16);
            this.C_OnTop.TabIndex = 0;
            this.C_OnTop.Text = "OnTop";
            this.C_OnTop.UseVisualStyleBackColor = true;
            // 
            // V_Data
            // 
            this.V_Data.ActionOnClicked = FormAdders.EasyGridViewCollections.Actions.Nothing;
            this.V_Data.ActionOnCtrlMoveKey = FormAdders.EasyGridViewCollections.ActionsOnCtrl_MoveKey.SelectRowsMove;
            this.V_Data.ActionOnDoubleClicked = FormAdders.EasyGridViewCollections.Actions.CheckBoxChecked;
            this.V_Data.ActionOnEnterInEditMode = FormAdders.EasyGridViewCollections.EnterActions.EditOnThePosition;
            this.V_Data.ActionOnRightClicked = FormAdders.EasyGridViewCollections.Actions.ContextMenu;
            this.V_Data.BaseRowHeight = 27;
            this.V_Data.ClearLinesWhenMaxLines = 10;
            this.V_Data.DataSource = null;
            this.V_Data.ScrollToLastRowWhenAddNew = true;
            this.V_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.V_Data.FirstDisplayedScrollingColIndex = -1;
            this.V_Data.FirstDisplayedScrollingRowIndex = -1;
            this.V_Data.IsTitleVisible = true;
            this.V_Data.Location = new System.Drawing.Point(0, 27);
            this.V_Data.MaxLines = 100;
            this.V_Data.MultiSelect = true;
            this.V_Data.Name = "V_Data";
            this.V_Data.RowHeaderWidth = 30;
            this.V_Data.SelectedRowsIndice = ((System.Collections.Generic.List<int>)(resources.GetObject("V_Data.SelectedRowsIndice")));
            this.V_Data.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.V_Data.Size = new System.Drawing.Size(292, 347);
            this.V_Data.TabIndex = 2;
            this.V_Data.TextAlignMode = FormAdders.EasyGridViewCollections.TextAlignModes.NumberOnlyRight;
            this.V_Data.TextViewMode = FormAdders.EasyGridViewCollections.TextViewModes.MultiLines;
            this.V_Data.U_BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.V_Data.U_GridColor = System.Drawing.SystemColors.ControlDark;
            this.V_Data.U_IndexWidth = 30;
            this.V_Data.U_ShowIndex = false;
            // 
            // SavedList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 374);
            this.Controls.Add(this.V_Data);
            this.Controls.Add(this.panel1);
            this.Name = "SavedList";
            this.Text = "SavedList";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox C_OnTop;
        private FormAdders.EasyGridView V_Data;
    }
}