namespace TestNetConnector
{
    partial class ValueForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValueForm));
            this.V_Data = new FormAdders.EasyGridView();
            this.SuspendLayout();
            // 
            // V_Data
            // 
            this.V_Data.ActionOnClicked = FormAdders.EasyGridViewCollections.Actions.Nothing;
            this.V_Data.ActionOnCtrlMoveKey = FormAdders.EasyGridViewCollections.ActionsOnCtrl_MoveKey.SelectRowsMove;
            this.V_Data.ActionOnDoubleClicked = FormAdders.EasyGridViewCollections.Actions.CheckBoxChecked;
            this.V_Data.ActionOnEnterInEditMode = FormAdders.EasyGridViewCollections.EnterActions.EditOnThePosition;
            this.V_Data.ActionOnRightClicked = FormAdders.EasyGridViewCollections.Actions.ContextMenu;
            this.V_Data.DataSource = null;
            this.V_Data.ScrollToLastRowWhenAddNew = true;
            this.V_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.V_Data.FirstDisplayedScrollingColIndex = -1;
            this.V_Data.FirstDisplayedScrollingRowIndex = -1;
            this.V_Data.IsTitleVisible = true;
            this.V_Data.Location = new System.Drawing.Point(0, 0);
            this.V_Data.MultiSelect = true;
            this.V_Data.Name = "V_Data";
            this.V_Data.RowHeaderWidth = 50;
            this.V_Data.SelectedRowsIndice = ((System.Collections.Generic.List<int>)(resources.GetObject("V_Data.SelectedRowsIndice")));
            this.V_Data.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.V_Data.Size = new System.Drawing.Size(312, 649);
            this.V_Data.TabIndex = 0;
            this.V_Data.TextAlignMode = FormAdders.EasyGridViewCollections.TextAlignModes.NumberOnlyRight;
            this.V_Data.TextViewMode = FormAdders.EasyGridViewCollections.TextViewModes.MultiLines;
            this.V_Data.U_BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.V_Data.U_GridColor = System.Drawing.SystemColors.ControlDark;
            this.V_Data.U_IndexWidth = 30;
            this.V_Data.U_ShowIndex = false;
            // 
            // ValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 649);
            this.Controls.Add(this.V_Data);
            this.Name = "ValueForm";
            this.Text = "Value";
            this.ResumeLayout(false);

        }

        #endregion

        private FormAdders.EasyGridView V_Data;
    }
}