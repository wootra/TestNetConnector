using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace FormAdders
{
    public partial class ActiveLabel : Button
    {
        Boolean _isOut = true;
        Object _item;
        Boolean _isDisableWhenNullItem = false;
        Boolean _isSelected = false;
        Color _activeColor = Color.FromArgb(0xff, 0xff, 0xdd, 0xdd);
        Color _normalColor = Color.FromArgb(0xff, 0xdd, 0xdd, 0xdd);
        Color _backColor = Color.FromArgb(0xff, 0xdd, 0xdd, 0xdd);
        Color _selectedColor = Color.FromArgb(0xff, 0xff, 0xee, 0xdd);
        Color _selectedActiveColor = Color.FromArgb(0xff, 0xff, 0xff, 0xdd);
        Boolean _isDrawRectOnOut = true;
        public ActiveLabel()
        {
            init();
            if (Parent != null) _normalColor = Parent.BackColor;
            this.Text = "";
            _item = null;
            
        }
        public ActiveLabel(String text, Object item)
        {
            init();
            if(Parent!=null) _normalColor = Parent.BackColor;
            this.Text = text;
            _item = item;
            
        }
        public Boolean IsDisableWhenNullItem { get { return _isDisableWhenNullItem; } set { _isDisableWhenNullItem = value; } }
        
        public Boolean IsSelected { get { return _isSelected; } set { _isSelected = value; } }
        public Boolean IsDrawRact { get { return _isDrawRectOnOut; } set { _isDrawRectOnOut = value; } }

        public void setSelectedColor(Color normalColor, Color activeColor)
        {
            _selectedColor = normalColor;
            _selectedActiveColor = activeColor;
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            if (_item != null)
            {
                this.BackColor = _normalColor;
                
                _backColor = _normalColor;
            }
        }
        public void setNormalColor(Color color)
        {
            _normalColor = color;
        }
        public void setBackColor(Color color)
        {
            _backColor = Color.FromArgb(0xff, color);
            this.BackColor = Color.FromArgb(0xff, color);
        }
        public void setActiveColor(Color color)
        {
            _activeColor = Color.FromArgb(0xff, color);
        }

        public Object getItem()
        {
            return _item;
        }

        void init()
        {
            InitializeComponent();
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            if (_item == null && _isDisableWhenNullItem) return;
            if (_isSelected) _backColor = _selectedActiveColor;
            else _backColor = _activeColor;
            _isOut = false;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (_item == null && _isDisableWhenNullItem) return;
            if (_isSelected)  _backColor = _selectedColor;
            else _backColor = _normalColor;
            _isOut = true;
            base.OnMouseLeave(e);
        }

        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(_backColor);

            if (!_isOut || _isDrawRectOnOut)
            {
                g.DrawLine(Pens.White, 0, 0, this.Width - 1, 0);
                g.DrawLine(Pens.White, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                g.DrawLine(Pens.Gray, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                g.DrawLine(Pens.Gray, 0, this.Height - 1, 0, 0);
                //g.DrawRectangle(Pens.Orange, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }

            g.DrawString(this.Text, this.Font, Brushes.Black, new PointF(10, (this.Height - this.Font.Height) / 2));

            if (this.Image != null) g.DrawImage(this.Image, 0, (this.Height - this.Image.Height) / 2, Image.Width, Image.Height);
            else if (_item != null && !_isDisableWhenNullItem)
            {

                Image.GetThumbnailImageAbort a = callBackAbort;
                Image icon = Properties.Resources.ArrowRight.GetThumbnailImage(10, 10, a, IntPtr.Zero);
                g.DrawImage(icon, 0, (this.Height - icon.Height) / 2, icon.Width, icon.Height);
            }
            //g.DrawLine(Pens.AliceBlue, new Point(0, 0), new Point(100, 100));
        }
        
        Boolean callBackAbort()
        {
            return false;
        }

        public void ActiveLabel_MouseHover()
        {
            if (_isOut)
            {
            
                _isOut = false;
            }
            this.BackColor = Color.Orange;
            this.Refresh();
        }
    }
}
