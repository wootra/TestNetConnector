using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class MultiInputForm : Form
    {
        List<LabelTextPair> _pairs = new List<LabelTextPair>();
        public MultiInputForm()
        {
            InitializeComponent();
            B_OK.Click += new EventHandler(B_OK_Click);
            B_Cancel.Click += new EventHandler(B_Cancel_Click);
            
            //T_Input.PreviewKeyDown += new PreviewKeyDownEventHandler(T_Input_PreviewKeyDown);
            
        }

        public void AddNameValues(String name, String value)
        {
            LabelTextPair pair = new LabelTextPair(name,value);
            int topMargin = 20;
            int lineHeight = 25;
            int y = _pairs.Count * lineHeight + topMargin;
            pair.TitleLabel.Location = new Point(10, y);
            pair.TitleLabel.AutoSize = false;
            pair.TitleLabel.Width = 100;
            pair.ValueTextBox.Location = new Point(120, y);
            pair.ValueTextBox.Width = 100;
            this.Controls.Add(pair.TitleLabel);
            this.Controls.Add(pair.ValueTextBox);

            P_Buttons.SetBounds(0, y + 30, 0, 0, BoundsSpecified.Y);
            this.Size = new Size(this.Width, y + P_Buttons.Height + topMargin + lineHeight +topMargin);
            pair.ValueTextBox.PreviewKeyDown += new PreviewKeyDownEventHandler(NV_NameValues_PreviewKeyDown);
            
            _pairs.Add(pair);
        }

        public List<LabelTextPair> TitleValuePairs
        {
            get { return _pairs; }
        }

        void NV_NameValues_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Result = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        DialogResult Result = DialogResult.Cancel;
        void B_Cancel_Click(object sender, EventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        void B_OK_Click(object sender, EventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public new DialogResult ShowDialog(String title, Form parent = null)
        {
            this.Text = title;
            if (parent != null)
            {
                base.ShowDialog(parent);
                return Result;
            }
            else
            {
                base.ShowDialog();
                return Result;
            }
        }

    }

    public class LabelTextPair
    {
        public Label TitleLabel = new Label();
        public TextBox ValueTextBox = new TextBox();
        public String TitleText { 
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }
        public String ValueText
        {
            get { return ValueTextBox.Text; }
            set { ValueTextBox.Text = value; }
        }
        public LabelTextPair(String title, String value)
        {
            TitleLabel.Text = title;
            ValueTextBox.Text = value;
        }
        public LabelTextPair(String title)
        {
            TitleLabel.Text = title;
            ValueTextBox.Text = "";
        }
    }

    
}
