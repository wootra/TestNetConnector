namespace UserDialogs
{
    partial class DataViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewer));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageTabControl1 = new FormAdders.ImageTabControl();
            this.Tab_Pattern = new FormAdders.ImageTabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.T_Diff = new System.Windows.Forms.TextBox();
            this.C_TimeStamp = new System.Windows.Forms.CheckBox();
            this.B_PatternCheck = new System.Windows.Forms.Button();
            this.B_Stop = new System.Windows.Forms.Button();
            this.imageTabControl1_tabPage2 = new FormAdders.ImageTabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.B_Next = new System.Windows.Forms.Button();
            this.T_From = new System.Windows.Forms.TextBox();
            this.B_Prev = new System.Windows.Forms.Button();
            this.B_Reload = new System.Windows.Forms.Button();
            this.Track_From = new System.Windows.Forms.TrackBar();
            this.T_NumLines = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.L_Line = new FormAdders.SyncLabel();
            this.B_FileOpen = new System.Windows.Forms.Button();
            this.L_Log = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.L_Total = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.C_Title = new System.Windows.Forms.ComboBox();
            this.V_Data = new FormAdders.EasyGridView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.imageTabControl1.SuspendLayout();
            this.Tab_Pattern.SuspendLayout();
            this.imageTabControl1_tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Track_From)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.imageTabControl1);
            this.splitContainer1.Panel1.Controls.Add(this.T_NumLines);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.L_Line);
            this.splitContainer1.Panel1.Controls.Add(this.B_FileOpen);
            this.splitContainer1.Panel1.Controls.Add(this.L_Log);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.L_Total);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.C_Title);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.V_Data);
            this.splitContainer1.Size = new System.Drawing.Size(731, 786);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // imageTabControl1
            // 
            this.imageTabControl1.ActiveTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.imageTabControl1.BackColor = System.Drawing.Color.Transparent;
            this.imageTabControl1.BorderLine = System.Drawing.Color.Empty;
            this.imageTabControl1.Controls.Add(this.imageTabControl1_tabPage2);
            this.imageTabControl1.Controls.Add(this.Tab_Pattern);
            this.imageTabControl1.InactiveTextColor = System.Drawing.Color.Gray;
            this.imageTabControl1.Location = new System.Drawing.Point(383, 42);
            this.imageTabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageTabControl1.Name = "imageTabControl1";
            this.imageTabControl1.ScrollButtonStyle = FormAdders.ImageTabControlEnums.YaScrollButtonStyle.Always;
            this.imageTabControl1.SelectedIndex = 1;
            this.imageTabControl1.Size = new System.Drawing.Size(251, 135);
            this.imageTabControl1.TabDock = System.Windows.Forms.DockStyle.Top;
            this.imageTabControl1.TabHeight = 20;
            this.imageTabControl1.TabIndex = 1002;
            this.imageTabControl1.TabItemsMargin = 2;
            this.imageTabControl1.TextColor = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
        System.Drawing.Color.Gray};
            this.imageTabControl1.UseImageAsTabBackground = false;
            // 
            // Tab_Pattern
            // 
            this.Tab_Pattern.ActiveImage = null;
            this.Tab_Pattern.ActiveTextColor = null;
            this.Tab_Pattern.Controls.Add(this.label3);
            this.Tab_Pattern.Controls.Add(this.T_Diff);
            this.Tab_Pattern.Controls.Add(this.C_TimeStamp);
            this.Tab_Pattern.Controls.Add(this.B_PatternCheck);
            this.Tab_Pattern.Controls.Add(this.B_Stop);
            this.Tab_Pattern.InActiveImage = null;
            this.Tab_Pattern.InactiveTextColor = null;
            this.Tab_Pattern.Location = new System.Drawing.Point(0, 20);
            this.Tab_Pattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Tab_Pattern.Name = "Tab_Pattern";
            this.Tab_Pattern.Size = new System.Drawing.Size(251, 115);
            this.Tab_Pattern.TabIndex = 0;
            this.Tab_Pattern.Text = "패턴체크";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "차이";
            // 
            // T_Diff
            // 
            this.T_Diff.Location = new System.Drawing.Point(53, 6);
            this.T_Diff.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.T_Diff.Name = "T_Diff";
            this.T_Diff.Size = new System.Drawing.Size(78, 25);
            this.T_Diff.TabIndex = 1;
            // 
            // C_TimeStamp
            // 
            this.C_TimeStamp.AutoSize = true;
            this.C_TimeStamp.Checked = true;
            this.C_TimeStamp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.C_TimeStamp.Location = new System.Drawing.Point(10, 46);
            this.C_TimeStamp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.C_TimeStamp.Name = "C_TimeStamp";
            this.C_TimeStamp.Size = new System.Drawing.Size(100, 19);
            this.C_TimeStamp.TabIndex = 7;
            this.C_TimeStamp.Text = "TimeStamp";
            this.C_TimeStamp.UseVisualStyleBackColor = true;
            // 
            // B_PatternCheck
            // 
            this.B_PatternCheck.Location = new System.Drawing.Point(6, 74);
            this.B_PatternCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_PatternCheck.Name = "B_PatternCheck";
            this.B_PatternCheck.Size = new System.Drawing.Size(106, 29);
            this.B_PatternCheck.TabIndex = 0;
            this.B_PatternCheck.Text = "패턴체크";
            this.B_PatternCheck.UseVisualStyleBackColor = true;
            this.B_PatternCheck.Click += new System.EventHandler(this.B_PatternCheck_Click);
            // 
            // B_Stop
            // 
            this.B_Stop.Location = new System.Drawing.Point(130, 74);
            this.B_Stop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_Stop.Name = "B_Stop";
            this.B_Stop.Size = new System.Drawing.Size(105, 29);
            this.B_Stop.TabIndex = 14;
            this.B_Stop.Text = "중지";
            this.B_Stop.UseVisualStyleBackColor = true;
            this.B_Stop.Click += new System.EventHandler(this.B_Stop_Click);
            // 
            // imageTabControl1_tabPage2
            // 
            this.imageTabControl1_tabPage2.ActiveImage = null;
            this.imageTabControl1_tabPage2.ActiveTextColor = null;
            this.imageTabControl1_tabPage2.Controls.Add(this.label5);
            this.imageTabControl1_tabPage2.Controls.Add(this.B_Next);
            this.imageTabControl1_tabPage2.Controls.Add(this.T_From);
            this.imageTabControl1_tabPage2.Controls.Add(this.B_Prev);
            this.imageTabControl1_tabPage2.Controls.Add(this.B_Reload);
            this.imageTabControl1_tabPage2.Controls.Add(this.Track_From);
            this.imageTabControl1_tabPage2.InActiveImage = null;
            this.imageTabControl1_tabPage2.InactiveTextColor = null;
            this.imageTabControl1_tabPage2.Location = new System.Drawing.Point(0, 20);
            this.imageTabControl1_tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageTabControl1_tabPage2.Name = "imageTabControl1_tabPage2";
            this.imageTabControl1_tabPage2.Size = new System.Drawing.Size(251, 115);
            this.imageTabControl1_tabPage2.TabIndex = 1;
            this.imageTabControl1_tabPage2.Text = "검색";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "시작줄";
            // 
            // B_Next
            // 
            this.B_Next.Location = new System.Drawing.Point(146, 71);
            this.B_Next.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_Next.Name = "B_Next";
            this.B_Next.Size = new System.Drawing.Size(86, 29);
            this.B_Next.TabIndex = 1001;
            this.B_Next.Text = "Next";
            this.B_Next.UseVisualStyleBackColor = true;
            this.B_Next.Click += new System.EventHandler(this.B_Next_Click);
            // 
            // T_From
            // 
            this.T_From.Location = new System.Drawing.Point(65, 13);
            this.T_From.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.T_From.Name = "T_From";
            this.T_From.Size = new System.Drawing.Size(79, 25);
            this.T_From.TabIndex = 16;
            this.T_From.Text = "0";
            // 
            // B_Prev
            // 
            this.B_Prev.Location = new System.Drawing.Point(54, 71);
            this.B_Prev.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_Prev.Name = "B_Prev";
            this.B_Prev.Size = new System.Drawing.Size(86, 29);
            this.B_Prev.TabIndex = 1001;
            this.B_Prev.Text = "Prev";
            this.B_Prev.UseVisualStyleBackColor = true;
            this.B_Prev.Click += new System.EventHandler(this.B_Prev_Click);
            // 
            // B_Reload
            // 
            this.B_Reload.Location = new System.Drawing.Point(150, 10);
            this.B_Reload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_Reload.Name = "B_Reload";
            this.B_Reload.Size = new System.Drawing.Size(86, 29);
            this.B_Reload.TabIndex = 18;
            this.B_Reload.Text = "Reload";
            this.B_Reload.UseVisualStyleBackColor = true;
            this.B_Reload.Click += new System.EventHandler(this.B_Reload_Click);
            // 
            // Track_From
            // 
            this.Track_From.LargeChange = 1000;
            this.Track_From.Location = new System.Drawing.Point(17, 46);
            this.Track_From.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Track_From.Maximum = 0;
            this.Track_From.Name = "Track_From";
            this.Track_From.Size = new System.Drawing.Size(216, 56);
            this.Track_From.TabIndex = 1000;
            this.Track_From.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Track_From.Scroll += new System.EventHandler(this.Track_From_Scroll);
            // 
            // T_NumLines
            // 
            this.T_NumLines.Location = new System.Drawing.Point(649, 16);
            this.T_NumLines.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.T_NumLines.Name = "T_NumLines";
            this.T_NumLines.Size = new System.Drawing.Size(79, 25);
            this.T_NumLines.TabIndex = 16;
            this.T_NumLines.Text = "30";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(591, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "표시줄";
            // 
            // L_Line
            // 
            this.L_Line.Image = null;
            this.L_Line.IsDrawRact = false;
            this.L_Line.LineColor = System.Drawing.Color.Black;
            this.L_Line.Location = new System.Drawing.Point(435, 14);
            this.L_Line.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.L_Line.Name = "L_Line";
            this.L_Line.RectStyle = FormAdders.SyncLabel.RectStyles.Solid;
            this.L_Line.Size = new System.Drawing.Size(87, 22);
            this.L_Line.TabIndex = 15;
            this.L_Line.Text = "0";
            // 
            // B_FileOpen
            // 
            this.B_FileOpen.Location = new System.Drawing.Point(642, 45);
            this.B_FileOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.B_FileOpen.Name = "B_FileOpen";
            this.B_FileOpen.Size = new System.Drawing.Size(86, 29);
            this.B_FileOpen.TabIndex = 13;
            this.B_FileOpen.Text = "FileOpen";
            this.B_FileOpen.UseVisualStyleBackColor = true;
            this.B_FileOpen.Click += new System.EventHandler(this.B_FileOpen_Click);
            // 
            // L_Log
            // 
            this.L_Log.FormattingEnabled = true;
            this.L_Log.ItemHeight = 15;
            this.L_Log.Location = new System.Drawing.Point(17, 68);
            this.L_Log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.L_Log.Name = "L_Log";
            this.L_Log.Size = new System.Drawing.Size(361, 109);
            this.L_Log.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(527, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "/";
            // 
            // L_Total
            // 
            this.L_Total.AutoSize = true;
            this.L_Total.Location = new System.Drawing.Point(546, 18);
            this.L_Total.Name = "L_Total";
            this.L_Total.Size = new System.Drawing.Size(39, 15);
            this.L_Total.TabIndex = 10;
            this.L_Total.Text = "Total";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(382, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "진행줄";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Title";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Log";
            // 
            // C_Title
            // 
            this.C_Title.FormattingEnabled = true;
            this.C_Title.Location = new System.Drawing.Point(65, 14);
            this.C_Title.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.C_Title.Name = "C_Title";
            this.C_Title.Size = new System.Drawing.Size(313, 23);
            this.C_Title.TabIndex = 2;
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
            this.V_Data.Location = new System.Drawing.Point(0, 0);
            this.V_Data.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.V_Data.MaxLines = 100;
            this.V_Data.MultiSelect = true;
            this.V_Data.Name = "V_Data";
            this.V_Data.RowHeaderWidth = 30;
            this.V_Data.SelectedRowsIndice = ((System.Collections.Generic.List<int>)(resources.GetObject("V_Data.SelectedRowsIndice")));
            this.V_Data.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.V_Data.Size = new System.Drawing.Size(731, 601);
            this.V_Data.TabIndex = 0;
            this.V_Data.TextAlignMode = FormAdders.EasyGridViewCollections.TextAlignModes.NumberOnlyRight;
            this.V_Data.TextViewMode = FormAdders.EasyGridViewCollections.TextViewModes.MultiLines;
            this.V_Data.U_BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.V_Data.U_GridColor = System.Drawing.SystemColors.ControlDark;
            this.V_Data.U_IndexWidth = 30;
            this.V_Data.U_ShowIndex = false;
            // 
            // DataViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 786);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DataViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pattern Analyzer";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.imageTabControl1.ResumeLayout(false);
            this.Tab_Pattern.ResumeLayout(false);
            this.Tab_Pattern.PerformLayout();
            this.imageTabControl1_tabPage2.ResumeLayout(false);
            this.imageTabControl1_tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Track_From)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.DataGridViewTextBoxColumn Name1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label L_Total;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox C_TimeStamp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox C_Title;
        private System.Windows.Forms.TextBox T_Diff;
        private System.Windows.Forms.Button B_PatternCheck;
        private System.Windows.Forms.ListBox L_Log;
        private System.Windows.Forms.Button B_FileOpen;
        private System.Windows.Forms.Button B_Stop;
        private FormAdders.EasyGridView V_Data;
        private FormAdders.SyncLabel L_Line;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox T_NumLines;
        private System.Windows.Forms.TextBox T_From;
        private System.Windows.Forms.Button B_Reload;
        private System.Windows.Forms.TrackBar Track_From;
        private System.Windows.Forms.Button B_Next;
        private System.Windows.Forms.Button B_Prev;
        private FormAdders.ImageTabControl imageTabControl1;
        private FormAdders.ImageTabPage imageTabControl1_tabPage2;
        private FormAdders.ImageTabPage Tab_Pattern;

    }
}