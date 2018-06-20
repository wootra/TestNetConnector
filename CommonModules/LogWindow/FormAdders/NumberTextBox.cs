using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataHandling;

namespace FormAdders
{
    public partial class NumberTextBox : TextBox
    {
        TypeHandling.TypeName _type = TypeHandling.TypeName.Integer;
        Int32 _intValue = 0;
        Int64 _longValue = 0;
        float _floatValue = 0;
        double _doubleValue = 0;

        public NumberTextBox():base()
        {
            this.Click += new EventHandler(NumberTextBox_Click);
            this.LostFocus += new EventHandler(NumberTextBox_LostFocus);
            this.GotFocus += new EventHandler(NumberTextBox_GotFocus);
            this.TextChanged += new EventHandler(NumberTextBox_TextChanged);
            this.HideSelection = false;
            
            base.Text = "";
            AppendText("0");
            this.Refresh();
        }

        void NumberTextBox_TextChanged(object sender, EventArgs e)
        {
            U_Text = base.Text;
            if (_Text.Length > 0)
            {
                TypeHandling.TypeName _type = TypeHandling.getTypeKind(_Text);
                setNumber(_type);
            }
        }

        void NumberTextBox_Click(object sender, EventArgs e)
        {
            this.SelectAll();
        }

        void NumberTextBox_GotFocus(object sender, EventArgs e)
        {
            this.SelectAll();
        }

        String _Text="0";
        [SettingsBindable(true)]
        [Bindable(true)]
        [Browsable(true)]
        public string U_Text
        {
            get
            {
                //if (_Text.Length == 0) return "0";
                return _Text;
            }
            set
            {
                if (value==null || value.Length == 0)
                {
                    _Text = "";
                    return;
                }
                _Text = value;

                base.Text = value;
                TypeHandling.TypeName typeName = TypeHandling.getTypeKind(value);
                
                if (typeName == TypeHandling.TypeName.String || typeName == TypeHandling.TypeName.Char)
                {
                    //BackSpace();
                }
                else
                {
                    _type = typeName;
                    //base.Text = value; 
                    //AppendText(value);
                    try
                    {
                        setNumber(typeName);
                    }
                    catch { }
                    this.Refresh();
                    base.Refresh();
                }
                
            }
        }
        void setNumber(TypeHandling.TypeName typeName)
        {
            if (typeName == TypeHandling.TypeName.Integer)
            {
                _longValue = Int64.Parse(_Text);
                _intValue = (int)_longValue;
                _floatValue = (float)_longValue;
                _doubleValue = (double)_longValue;
            }
            else if (typeName == TypeHandling.TypeName.HexString)
            {
                _longValue = TypeHandling.getHexNumber(_Text);
                _intValue = (int)_longValue;
                _floatValue = (float)_longValue;
                _doubleValue = (double)_longValue;
            }
            else if (typeName == TypeHandling.TypeName.Float)
            {
                Double d=0;
                Double.TryParse(_Text, out d);
                _doubleValue = d;
                _floatValue = (float)_doubleValue;
                _intValue = (int)_doubleValue;
                _longValue = (long)_doubleValue;
            }
            else if (typeName == TypeHandling.TypeName.OctString)
            {
                _longValue = TypeHandling.getOctNumber(_Text);
                _intValue = (int)_longValue;
                _floatValue = (float)_longValue;
                _doubleValue = (double)_longValue;
            }
            else
            {
                BackSpace();
            }
        }
        void BackSpace()
        {
            //if(_Text.Length>0) _Text = _Text.Substring(0, _Text.Length - 1);
            //base.Text = _Text;
        }

        void NumberTextBox_LostFocus(object sender, EventArgs e)
        {
            if (this._Text.Length == 0)
            {
                base.Text = "0";
            }
            else
            {
                TypeHandling.TypeName typeName = TypeHandling.getTypeKind(base.Text);
                if (typeName == TypeHandling.TypeName.String || typeName == TypeHandling.TypeName.Char)
                {
                    MessageBox.Show("숫자가 아닙니다.");
                    BackSpace();
                    this.Focus();
                    this.SelectAll();
                    return;
                }
                else
                {
                    setNumber(typeName);
                }
            }
            this.DeselectAll();

        }

        public TypeHandling.TypeName TypeName { get { return _type; } }
        public int IntValue { get { return _intValue; } }
        public long LongValue { get { return _longValue; } }
        public float FloatValue { get { return _floatValue; } }
        public double DoubleValue { get { return _doubleValue; } }

    }
}
