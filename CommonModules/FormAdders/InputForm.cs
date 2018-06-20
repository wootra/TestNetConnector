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
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            B_OK.Click += new EventHandler(B_OK_Click);
            B_Cancel.Click += new EventHandler(B_Cancel_Click);
            T_Input.PreviewKeyDown += new PreviewKeyDownEventHandler(T_Input_PreviewKeyDown);
        }

        void T_Input_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (CheckNameSyntax())
                {
                    Result = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
        }

        private bool CheckNameSyntax()
        {
            if (T_Input.Text.Contains("."))
            {
                MessageBox.Show("Scenario name cannot have '.' for the name...");
                return false;
            }
            return true;
        }

        DialogResult Result = DialogResult.Cancel;
        void B_Cancel_Click(object sender, EventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        void B_OK_Click(object sender, EventArgs e)
        {
            if (CheckNameSyntax())
            {
                Result = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
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

        public String InputText
        {
            get
            {
                return T_Input.Text;
            }
            set
            {
                T_Input.Text = value;
            }
        }
    }
}
