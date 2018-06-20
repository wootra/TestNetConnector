using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserDialogs
{
    public class PopupForm : Form
    {
        private String _formName;
        private int _popupNumber;
        private String _format = "{0:f10}";
        protected Object Sender = null;
        public event PopupClosedEventHandler U_PopupClosed;
        public new event FormClosedEventHandler FormClosed;
        private System.ComponentModel.IContainer components = null;

        public PopupForm(int popNo, String formName="")
        {
            InitializeComponent();
            this._formName = formName;
            this._popupNumber = popNo;
            init();
            
        }
        public String NumberFormat { set { _format = value; } }
        public String U_NumberFormat { set { _format = value; } }

        public String NumberText(Object value)
        {
            return String.Format(_format, value.ToString());
        }
        public String U_NumberText(Object value)
        {
            return String.Format(_format, value.ToString());
        }

        public String Time(int h, int m, int s)
        {
            String time = String.Format("{0:D2}:", h);
            time += String.Format("{0:D2}:", m);
            time += String.Format("{0:D2}", s);
            return time;
        }
        
        public PopupForm()
        {
            InitializeComponent();
            _formName = "";
            _popupNumber = -1;
            init();
        }
        void init()
        {
            base.FormClosed += new FormClosedEventHandler(PopupForm_FormClosed);
        }

        void PopupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnClosed();
            if (Sender == null) Sender = this;
            if (U_PopupClosed != null) U_PopupClosed(Sender, new PopupClosedEventArgs(_popupNumber, _formName));
            if (this.FormClosed != null) this.FormClosed(this, e);
        }
        protected virtual void OnClosed() { }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PopupForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "PopupForm";
            this.ResumeLayout(false);

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class PopupClosedEventArgs : EventArgs
    {
        public int popUpNo;
        public String popupName;
        public Object[] args;
        public PopupClosedEventArgs(int no, String popupName, params Object[] etc)
        {
            this.popUpNo = no;
            this.popupName = popupName;
            if (etc != null && etc.Length > 0)
            {
                this.args = new Object[etc.Length];
                for (int i = 0; i < etc.Length; i++)
                {
                    args[i] = etc[i];
                }
            }
            else
            {
                etc = null;
            }
        }
    }
    public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs args);
}
