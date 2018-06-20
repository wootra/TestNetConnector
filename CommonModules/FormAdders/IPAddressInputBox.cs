using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace FormAdders
{
    public partial class IPAddressInputBox : UserControl
    {
        public event TextChangedEventHandler E_IPTextChanged;
        private MaskedTextBox[] _ipArr;
        private int _focusedBox;
        private Label[] _points;
        Char _tempKey;

        String _ipString = "0.0.0.0";
        //Padding _pading;

        public String IpAddress
        {
            get
            {
                return getIp();
            }
            set
            {
                setIPText(value);
            }
        }
                    
        public IPAddress getIpAddress()
        {
            byte[] ip = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                
                ip[i] = Byte.Parse(_ipArr[i].Text);
            }
            IPAddress ipAddr = new IPAddress(ip);
            return ipAddr;
        }
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                Ip0.ForeColor = value;
                Ip1.ForeColor = value;
                Ip2.ForeColor = value;
                Ip3.ForeColor = value;
                
                base.ForeColor = value;
            }
        }
        public override Color BackColor
        {
            get
            {
                return BackPanel.BackColor;
            }
            set
            {
                BackPanel.BackColor = value;
                Ip0.BackColor = value;
                Ip1.BackColor = value;
                Ip2.BackColor = value;
                Ip3.BackColor = value;
            }
        }
        public Color AroundBackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        public new BorderStyle BorderStyle
        {
            get
            {
                return BackPanel.BorderStyle;
            }
            set
            {
                BackPanel.BorderStyle = value;
            }
        }

        
        
        public IPAddressInputBox()
        {

            InitializeComponent();
            
            _ipArr = new MaskedTextBox[]{Ip0,Ip1,Ip2,Ip3};
            _points = new Label[]{L_p1,L_p2,L_p3};
            

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            for (int i = 0; i < 4; i++)
            {
                _ipArr[i].GotFocus += new EventHandler(IpInputControl_GotFocus);
                _ipArr[i].KeyUp += new KeyEventHandler(IpInputControl_KeyUp);
                _ipArr[i].KeyPress += new KeyPressEventHandler(IPAddressInputBox_KeyPress);
                _ipArr[i].LostFocus += new EventHandler(IpInputControl_LostFocus);
                _ipArr[i].MouseUp += new MouseEventHandler(IPAddressInputBox_MouseUp);

                _ipArr[i].MaskInputRejected += new MaskInputRejectedEventHandler(IPAddressInputBox_MaskInputRejected);
            }
            this.Resize += new EventHandler(IPAddressInputBox_Resize);
            this.LostFocus += new EventHandler(IPAddressInputBox_LostFocus);
        }

        void IPAddressInputBox_LostFocus(object sender, EventArgs e)
        {
            
        }

        void IPAddressInputBox_Resize(object sender, EventArgs e)
        {
            int ptWidth = L_p1.Width;
            int leftMargin = 3;
            int rightMargin = 6;
            int width= (this.Width - (ptWidth * 3) - leftMargin - rightMargin - 8) / 4-1;
            
            int y = (this.Height - _ipArr[0].Height) / 2;
            for(int i=0; i<3;i++){
                
                _ipArr[i].Width = width;
                _ipArr[i].Location = new Point(width*i + ptWidth*i + leftMargin,y);
                _points[i].Location = new Point(_ipArr[i].Location.X + width,y);
            }
            _ipArr[3].Width = width;
            _ipArr[3].Location = new Point(width*3 + ptWidth*3+leftMargin, y);
            
        }

        void IPAddressInputBox_MouseUp(object sender, MouseEventArgs e)
        {
            MaskedTextBox text = (MaskedTextBox)sender;
            text.SelectAll();
        }

        void IPAddressInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            _tempKey = e.KeyChar;
            if (_tempKey == '.') if (_focusedBox < 3) _ipArr[_focusedBox + 1].Focus();
        }

        void IPAddressInputBox_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            MaskedTextBox text = (MaskedTextBox)sender;
            if (_tempKey > 0 || _tempKey < 1) return;
            String tempText = text.Text.Trim();
            if (tempText.Length < 3) text.Text = tempText + _tempKey.ToString();
            else
            {
                if (_focusedBox < 3)
                {
                    _ipArr[_focusedBox + 1].Focus();
                    _ipArr[_focusedBox].Text = "";
                    _ipArr[_focusedBox].AppendText(_tempKey.ToString());
                }
            }
        }

        void IpInputControl_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox Text = ((MaskedTextBox)sender);
            int index = _ipArr.ToList().IndexOf(Text);
            int num=0;
            Int32.TryParse(Text.Text.ToString(), out num);
            if (num > 255) num = 255;
            Text.Text = num.ToString();
            //throw new NotImplementedException();
            if (getIp().Equals(_ipString) == false)
            {
                if (E_IPTextChanged != null)
                {
                    TextChangedEventArgs args = new TextChangedEventArgs(_ipString, getIp());
                    E_IPTextChanged(this, args);
                    if (args.IsCancel)
                    {
                        setIPText(_ipString);
                    }
                    else
                    {
                        _ipString = args.Text;
                    }
                }
            }
        }

        void IpInputControl_KeyUp(object sender, KeyEventArgs e)
        {
            MaskedTextBox text = (MaskedTextBox)sender;
            switch (e.KeyData)
            {
                case Keys.Space:
//                case Keys.OemPeriod: //KeyPress에서 처리
                case Keys.Right:
                    if (_focusedBox < 3) _ipArr[_focusedBox+1].Focus();
                    break;
                case Keys.Back:
                    if (_ipArr[_focusedBox].Text.Length == 0 && _focusedBox > 0) _ipArr[_focusedBox - 1].Focus();
                    break;
                case Keys.Left:
                    if (_focusedBox > 0) _ipArr[_focusedBox-1].Focus();
                    break;
                default:
                    break;
            }
        }

        void IpInputControl_GotFocus(object sender, EventArgs e)
        {
            MaskedTextBox Text = ((MaskedTextBox)sender);
            String temp = Text.Text;
            Text.Text = "";
            Text.AppendText(temp);
            Text.SelectAll();

            _focusedBox = Int32.Parse(Text.Tag.ToString());
        }
        private void setIPText(String ip){
            String[] ips = ip.Split(".".ToCharArray());
            _ipString = ip;
            int len = (ips.Length >= _ipArr.Length)? _ipArr.Length : ips.Length;
            for (int i = 0; i < len; i++)
            {
                _ipArr[i].Text = ips[i];
                _ipArr[i].Refresh();
            }
            this.Refresh();
        }

        private String getIp()
        {
            string ip = "";
            string temp = "";
            for (int i = 0; i < 4; i++)
            {
                temp = _ipArr[i].Text;
                temp.Replace(" ", "");
                ip += temp;
                if (i != 3) ip += ".";
            }
            return ip;
        }
     }
}
