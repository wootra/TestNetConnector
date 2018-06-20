using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace FormAdders
{
    public partial class ImgButton : Panel
    {
        private int _buttonType;
        private int _imageIndex=-1;
        private Color _foreColor;
        public new event EventHandler Click;
        Timer _hoverTimer = new Timer();
        Timer _imageViewer = new Timer();
        public ImgButton()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _buttonType = 0;
            L_Text.BackColor = Color.Transparent;
            this.ForeColor = L_Text.ForeColor;
            _hoverTimer.Interval = 200;
            _imageViewer.Interval = 1000;
            _imageViewer.Tick += new EventHandler(_imageViewer_Tick);
            _hoverTimer.Tick += new EventHandler(_hoverTimer_Tick);
            this.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            this.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            this.GotFocus += new EventHandler(ImgButton_GotFocus);
            this.LostFocus += new EventHandler(ImgButton_LostFocus);
            
            this.MouseHover += new EventHandler(ImgButton_MouseHover);
            this.MouseEnter +=new EventHandler(ImgButton_MouseHover);
            //this.MouseLeave += new EventHandler(ImgButton_MouseLeave);
            this.SizeChanged += new EventHandler(ImgButton_SizeChanged);
            base.Click += new EventHandler(ImgButton_Click);

            L_Text.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            L_Text.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            L_Text.GotFocus += new EventHandler(ImgButton_GotFocus);
            L_Text.LostFocus += new EventHandler(ImgButton_LostFocus);
            L_Text.MouseHover += new EventHandler(ImgButton_MouseHover);
            //L_Text.MouseLeave += new EventHandler(ImgButton_MouseLeave);
            L_Text.SizeChanged += new EventHandler(ImgButton_SizeChanged);
            L_Text.Click += new EventHandler(ImgButton_Click);
            _imageViewer.Start();
            setTextPosition();
        }
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            _hoverTimer.Stop();
            _imageViewer.Stop();

            base.OnControlRemoved(e);
        }
        void ImgButton_SizeChanged(object sender, EventArgs e)
        {
            setTextPosition();
            Refresh();
        }

        void _imageViewer_Tick(object sender, EventArgs e)
        {
            if (_images!=null && _images.Count != 0)
            {
                if (_imageIndex >= 0)
                {
                    _imageViewer.Stop();
                    //this.BackgroundImage = _images[_imageIndex];
                    this.Refresh();
                }
            }
        }

        void _hoverTimer_Tick(object sender, EventArgs e)
        {
            Control s = this;
            try
            {
                Rectangle zero = s.RectangleToScreen(s.DisplayRectangle);
                if (Control.MousePosition.X < zero.X || Control.MousePosition.X > (zero.X + zero.Width) ||
                    Control.MousePosition.Y < zero.Y || Control.MousePosition.Y > (zero.Y + zero.Height))
                {

                    this._imageIndex = 0;
                    this.BackColor = _tempBackColor;
                    Refresh();
                    _hoverTimer.Stop();
                }
            }
            catch (ObjectDisposedException) { }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            setTextPosition();
        }
        void setTextPosition()
        {
            int x = 0, y = 0;
            switch (_textPosition)
            {
                case AnchorStyles.Left:
                    x = 0+this.Padding.Left;
                    y = (this.Height - L_Text.Height) / 2;
                    break;
                case AnchorStyles.Right:
                    x = this.Width - L_Text.Width - this.Padding.Right;
                    y = (this.Height - L_Text.Height) / 2;
                    break;
                case AnchorStyles.Top:
                    x = (this.Width - L_Text.Width) / 2;
                    y = this.Padding.Top;
                    break;
                case AnchorStyles.Bottom:
                    x = (this.Width - L_Text.Width) / 2;
                    y = (this.Height - L_Text.Height) - this.Padding.Bottom;
                    break;
                case AnchorStyles.None:
                default:
                    x = (this.Width - L_Text.Width) / 2;
                    y = (this.Height - L_Text.Height) / 2;
                    break;
            }
            L_Text.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
        }

        Color _activeBackColor = Color.FromArgb(30, Color.Cyan);
        public Color ActiveBackColor
        {
            get { return _activeBackColor; }
            set { _activeBackColor = value; }
        }

        AnchorStyles _textPosition = AnchorStyles.None;
        public AnchorStyles U_TextPosition
        {
            get
            {
                return _textPosition;
            }
            set
            {
                _textPosition = value;
                setTextPosition();
            }
        }

        int _buttonDisabledType = 0;
        public int U_ButtonDisabledType
        {
            get
            {
                return _buttonDisabledType;
            }
            set
            {
                _buttonDisabledType = value;
            }
            
        }

        int _tempButtonType = 0;
        public new bool Enabled{
            get
            {
                return base.Enabled;
            }
            set
            {
                if (base.Enabled == true && value == false)
                {
                    _tempButtonType = U_ButtonTypeIndex;
                    _buttonType = _buttonDisabledType;
                }
                else if(base.Enabled == false && value == true)
                {
                    _buttonType = _tempButtonType;
                }

                base.Enabled = value;
                Refresh();
            }
        }

        public override void Refresh()
        {
            
                try{
                    this.BackgroundImage = this._images[_buttonType * 3 + _imageIndex];
                }catch{
                    try{
                        this.BackgroundImage = this._images[_imageIndex];
                    }catch{
                        try{
                            this.BackgroundImage = this._images[0];
                        }catch{
                            this.BackgroundImage = null;
                        }
                    }
                }
                

            base.Refresh();
            //L_Text.Refresh();
            setTextPosition();
        }
        
        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.Refresh();
        }
        /// <summary>
        /// 버튼에 표시 될 내용
        /// </summary>
        ///
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [SettingsBindable(true)]
        [Browsable(true)]
        [EditorBrowsable]
        public override String Text
        {
            get
            {
                return L_Text.Text;
            }
            set
            {
                L_Text.Text = value;
            }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }
       

        public int U_ButtonTypeIndex
        {
            get
            {
                return _buttonType;
            }
            set
            {
                _buttonType = value;
                if (this.Enabled == false)
                {
                    _tempButtonType = value;
                    _buttonType = _buttonDisabledType;
                }
                Refresh();
            }
        }

        [DefaultValue(false)]
        [BrowsableAttribute(true)]
        public new Color ForeColor
        {
            get
            {
                return this._foreColor;
            }
            set
            {
                L_Text.ForeColor = value;
                this._foreColor = value;
                base.Refresh();
            }
        }
        public new Font Font
        {
            get
            {
                return L_Text.Font;
            }
            set
            {
                L_Text.Font = value;
                base.Refresh();
            }
        }

        [Browsable(true)]
        [EditorBrowsable]
        public int U_ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                if (value==_imageIndex) return; //같을 경우 아무것도 안한다.
                _imageIndex = value;
                if(U_ImageList.Count>0) Refresh();
            }
        }

        FineImageList _images = new FineImageList();
        [SettingsBindable(true)]
        public FineImageList U_ImageList
        {
            
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                /*
                if (value != null && value.Count > 0)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        _images[i] = value[i];
                    }
                }
                */

                if (_images == null || _images.Count==0 || _images[0] == null)
                {
                    //_imageIndex = -1;
                    Refresh();
                }
                else
                {
                    this._imageIndex = 0;
                    Refresh();
                }
            }
        }
        SolidBrush _borderColor = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
        SolidBrush _disabledBorderColor = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
        public Color U_BorderColor
        {
            get { return _borderColor.Color; }
            set { _borderColor.Color = value; }
        }
        public enum Border3DStyles { Up, Down };
        Border3DStyles _border3DStyle = Border3DStyles.Up;
        public Border3DStyles U_Border3DStyle
        {
            get{ return _border3DStyle;}
            set { _border3DStyle = value; }
        }

        #region /////////////////// Events///////////////////////

        SolidBrush _backColor = new SolidBrush(Color.Transparent);
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                _backColor.Color = value;
            }
        }
        public enum BorderStyles { Fixed3D_Up, Fixed3D_Down, FixedSingle, None };
        BorderStyles _borderStyle = BorderStyles.None;
        public new BorderStyles BorderStyle{
            get
            {
                return _borderStyle;
            }
            set{
                _borderStyle = value;
            }
        }

        protected override void  OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //e.Graphics.FillRectangle(_backColor, this.DisplayRectangle);
            Graphics g = e.Graphics;
            //g.Clear(_backColor.Color);
            switch (this.BorderStyle)
            {
                case BorderStyles.Fixed3D_Up:
                    //if (_border3DStyle == Border3DStyles.Up)
                    {
                        g.DrawLine(Pens.White, 0, 0, this.Width - 1, 0);
                        g.DrawLine(Pens.Gray, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                        g.DrawLine(Pens.Gray, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                        g.DrawLine(Pens.White, 0, this.Height - 1, 0, 0);
                    }
                    break;
                case BorderStyles.Fixed3D_Down:
                    //else
                    {
                        g.DrawLine(Pens.Gray, 0, 0, this.Width - 1, 0);
                        g.DrawLine(Pens.White, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                        g.DrawLine(Pens.White, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                        g.DrawLine(Pens.Gray, 0, this.Height - 1, 0, 0);
                    }
                    //g.DrawRectangle(Pens.Orange, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                    break;
                
                case  BorderStyles.FixedSingle:
                    Pen pen;
                    if (this.Enabled) pen = new Pen(_borderColor);
                    else pen = new Pen(_disabledBorderColor);
                    pen.Width = 0.1f;
                    g.DrawLine(pen, 0, 0, this.Width - 1, 0);
                    g.DrawLine(pen, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                    g.DrawLine(pen, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                    g.DrawLine(pen, 0, this.Height - 1, 0, 0);
                    break;
            }
            
        }

        void ImgButton_MouseLeave(object sender, EventArgs e)
        {

        }

        Color _tempBackColor = Color.Transparent;
        void ImgButton_MouseHover(object sender, EventArgs e)
        {
            

            if (_hoverTimer.Enabled == false)
            {
                this._imageIndex = 1;
                Refresh();

                _tempBackColor = this.BackColor;
                this.BackColor = _activeBackColor;
                _hoverTimer.Start();
            }
        }

        void ImgButton_LostFocus(object sender, EventArgs e)
        {
            this._imageIndex = 0;
            
           Refresh();
        }

        void ImgButton_GotFocus(object sender, EventArgs e)
        {
            this._imageIndex = 1;
            Refresh();
        }

        void ImgButton_MouseUp(object sender, MouseEventArgs e)
        {
            this._imageIndex = 0;
            Refresh();
        }

        void ImgButton_MouseDown(object sender, MouseEventArgs e)
        {
            this._imageIndex = 2;
            Refresh();
        }
        void ImgButton_Click(object sender, EventArgs e)
        {
            this._imageIndex = 0;
            if (Click != null) Click(this, e);
        }
        #endregion ///////////////////////////////////////////////

    }
}
