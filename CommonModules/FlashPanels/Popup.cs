using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;

namespace FlashPanels
{
    public partial class Popup : PopupForm
    {
        public NameValueTextPairList list;
        Object[] _arg = null;
        TypeCode _retType = TypeCode.Int32;
        public Popup()
        {
            InitializeComponent();
            Init();
        }
        public Popup(int popupNo, Object[] arg, TypeCode retType = TypeCode.Int32):base(popupNo)
        {
            InitializeComponent();
            Init();
            _arg = arg;
            if (_arg != null && _arg.Length > 0) _arg[0] = -1;
        }

        private void Init(){
            list = nameValueTextPairList1;
            this.Resize += new EventHandler(Popup_Resize);
            
        }


        void Popup_Resize(object sender, EventArgs e)
        {

            splitContainer1.Panel2.Height = 45;
            button1.Location = new Point(this.Width - button1.Width - 10, 0);
        }
        void setValue()
        {
            if (_arg != null)
            {
                if (_arg.Length >= list.Count)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (_retType == TypeCode.Int32) _arg[i] = list[i].IntValue;
                        else if (_retType == TypeCode.Int64) _arg[i] = list[i].LongValue;
                        else if (_retType == TypeCode.Single) _arg[i] = list[i].FloatValue;
                        else if (_retType == TypeCode.Double) _arg[i] = list[i].DoubleValue;
                    }
                }
            }
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            try
            {
                setValue();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Close();
        }
    }
}
