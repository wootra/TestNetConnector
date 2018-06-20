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
            this.TextChanged += new EventHandler(NumberTextBox_TextChanged);
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(NumberTextBox_PreviewKeyDown);
            this.HideSelection = false;
            
            base.Text = "";
            AppendText("0");
            this.Refresh();
        }
        Keys KeyCode = Keys.D0;
        void NumberTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            KeyCode = e.KeyCode;
        }

        void NumberTextBox_TextChanged(object sender, EventArgs e)
        {
            if ((int)KeyCode >= (int)Keys.A && (int)KeyCode <= (int)Keys.F)
            {
                if (U_Text.Length >= 2 && U_Text.Substring(0, 2).ToLower().Equals("0x"))
                {
                    _Text = base.Text.Substring(2);
                    setNumber(TypeHandling.TypeName.HexString);
                }
                else
                {
                    U_Text = "";
                    setNumber(TypeHandling.TypeName.Integer);
                    return;
                }
            }
            else if (KeyCode == Keys.X)
            {
                if (U_Text.Length >= 1 && U_Text[0] == '0')
                {
                    //U_Text = base.Text;
                    _Text = "";
                    setNumber(TypeHandling.TypeName.Integer);
                }
                else
                {
                    U_Text = "";
                    setNumber(TypeHandling.TypeName.Integer);
                }
            }
            else if ((int)KeyCode >= (int)Keys.D0 && (int)KeyCode <= (int)Keys.D9)
            {
                U_Text = base.Text;
                TypeHandling.TypeName typeName = TypeHandling.getTypeKind(U_Text);
                setNumber(typeName);
            }
            else if ((int)KeyCode >= (int)Keys.NumPad0 && (int)KeyCode <= (int)Keys.NumPad9)
            {
                U_Text = base.Text;
                TypeHandling.TypeName typeName = TypeHandling.getTypeKind(U_Text);
                setNumber(typeName);
            }
            else if (KeyCode == Keys.OemPeriod)
            {
                if(U_Text.Count(p => p.Equals('.'))==1 && U_Text.Length>0){
                    //U_Text = base.Text;
                }else{
                    U_Text = "";
                    setNumber(TypeHandling.TypeName.Integer);
                }
            }
            else
            {
                U_Text = "";
                setNumber(TypeHandling.TypeName.Integer);
            }
        }

        void NumberTextBox_Click(object sender, EventArgs e)
        {
            this.SelectAll();
        }

        [SettingsBindable(true)]
        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                U_Text = value;
            }
        }

        String _Text="0";
        [SettingsBindable(true)]
        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable( EditorBrowsableState.Always)]
        public string U_Text
        {
            get
            {
                //if (_Text.Length == 0) return "0";
                return base.Text;
            }
            set
            {
                if (value==null || value.Length == 0)
                {
                    _Text = "";
                    base.Text = value;
                    setNumber(TypeHandling.TypeName.Integer);
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
            if (_Text.Length == 0) _Text = "0";
            if (typeName == TypeHandling.TypeName.Integer)
            {
                _longValue = Int64.Parse(_Text);
                _intValue = (int)_longValue;
                _floatValue = (float)_longValue;
                _doubleValue = (double)_longValue;
            }
            else if (typeName == TypeHandling.TypeName.HexString)
            {
                _longValue = TypeHandling.getHexNumber<long>(_Text);
                _intValue = (int)_longValue;
                _floatValue =(float)_longValue;
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
            if(_Text.Length>0) _Text = _Text.Substring(0, _Text.Length - 1);
            base.Text = _Text;
        }

       

        public TypeHandling.TypeName TypeName { get { return _type; } }
        public int IntValue { get { return _intValue; } }
        public long LongValue { get { return _longValue; } }
        public float FloatValue { get { return _floatValue; } }
        public double DoubleValue { get { return _doubleValue; } }

    }
}
