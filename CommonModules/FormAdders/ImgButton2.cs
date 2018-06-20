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
using System.Drawing.Imaging;

namespace FormAdders
{
    public partial class ImgButton2 : Panel
    {
        private int _buttonType;
        private int _imageIndex=-1;
        private Color _foreColor;
        public new event EventHandler Click;
        //Timer _hoverTimer;
        //Timer _imageViewer;
        ToolTip _tooltip;
        static ImgButton2 Activated = null;
        PictureBox _picture = new PictureBox();
        public ImgButton2()
        {
            InitializeComponent();
            this.Controls.Add(_picture);
            _picture.BackColor = Color.Transparent;
            _picture.BackgroundImageLayout = ImageLayout.Zoom;
            //if(_hoverTimer==null) _hoverTimer = new Timer();
            //if(_imageViewer==null) _imageViewer = new Timer();

            this.DoubleBuffered = true;
            _buttonType = 0;
            L_Text.BackColor = Color.Transparent;
            this.ForeColor = L_Text.ForeColor;
            //_hoverTimer.Interval = 200;
            //_imageViewer.Interval = 1000;
            //_imageViewer.Tick += new EventHandler(_imageViewer_Tick);
            FormAdderGlobalTimer.InitTimer(200);
            FormAdderGlobalTimer.UiRefreshed += FormAdderGlobalTimer_UiRefreshed;
            base.BackColor = Color.Transparent;
            
            this.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            this.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            this.GotFocus += new EventHandler(ImgButton_GotFocus);
            this.LostFocus += new EventHandler(ImgButton_LostFocus);
            this.MouseEnter += ImgButton2_MouseEnter;
            this.MouseLeave += ImgButton2_MouseLeave;
            //this.MouseHover += new EventHandler(ImgButton_MouseHover);
            //this.MouseEnter +=new EventHandler(ImgButton_MouseHover);
            //this.MouseLeave += new EventHandler(ImgButton_MouseLeave);
            this.SizeChanged += new EventHandler(ImgButton_SizeChanged);
            base.Click += new EventHandler(ImgButton_Click);
            
            L_Text.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            L_Text.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            L_Text.GotFocus += new EventHandler(ImgButton_GotFocus);
            L_Text.LostFocus += new EventHandler(ImgButton_LostFocus);
            //L_Text.MouseHover += new EventHandler(ImgButton_MouseHover);
            L_Text.Click += new EventHandler(ImgButton_Click);
            L_Text.SizeChanged += new EventHandler(ImgButton_SizeChanged);

            _picture.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            _picture.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            _picture.GotFocus += new EventHandler(ImgButton_GotFocus);
            _picture.LostFocus += new EventHandler(ImgButton_LostFocus);
            //_picture.MouseHover += new EventHandler(ImgButton_MouseHover);
            //_picture.MouseLeave += new EventHandler(ImgButton_MouseLeave);
            _picture.Click += new EventHandler(ImgButton_Click);
            //L_Text.MouseLeave += new EventHandler(ImgButton_MouseLeave);
           
            //_imageViewer.Start();
            setTextPosition();
            this._tooltip = new ToolTip();
            this.BackgroundImageLayout = ImageLayout.Zoom;
            
        }

        void ImgButton2_MouseLeave(object sender, EventArgs e)
        {
            if (IsMouseIn() == false)
            {
                SetHoverOut();
            }
        }

        private bool IsMouseIn()
        {
            Point mousePt = Control.MousePosition;
            Point thisPt = this.PointToScreen(new Point(0, 0));
            if (thisPt.X <= mousePt.X && thisPt.X + this.Width > mousePt.X)
            {
                if (thisPt.Y <= mousePt.Y && thisPt.Y + this.Height > mousePt.Y)
                {
                    return true;
                }
                    
            }
            return false;
        }

        void ImgButton2_MouseEnter(object sender, EventArgs e)
        {
            if (IsMouseIn() == false)
            {
                SetHoveredIn();
            }
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }


        void FormAdderGlobalTimer_UiRefreshed(int timerGroup)
        {
            if (timerGroup == 200)
            {
                _hoverTimer_Tick();
            }
        }

        
       

        ~ImgButton2()
        {
            //_imageViewer.Stop();
        }
        

        void ImgButton_SizeChanged(object sender, EventArgs e)
        {
            setTextPosition();
            Refresh();
        }
        /*
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
        */
        void _hoverTimer_Tick()
        {
            Control s = this;
            try
            {
                Rectangle zero = s.RectangleToScreen(s.DisplayRectangle);
                Point mousePos = Control.MousePosition;

                if (mousePos.X < zero.X || mousePos.X > (zero.X + zero.Width) ||
                    mousePos.Y < zero.Y || mousePos.Y > (zero.Y + zero.Height))
                {
                    
                     SetHoverOut();


                }
                else 
                    //if (Control.MousePosition.X > zero.X && Control.MousePosition.X < (zero.X + zero.Width) &&
                   //Control.MousePosition.Y > zero.Y && Control.MousePosition.Y < (zero.Y + zero.Height))
                {

                    SetHoveredIn();


                }
            }
            catch { }
        }


        void SetHoveredIn()
        {
            if (this.Enabled)
            {
                if (Activated == null || Activated.Equals(this) == false)
                {
                    if (Activated != null) Activated.SetHoverOut();

                    Activated = this;
                    this._imageIndex = 1;
                    Refresh();

                    base.BackColor = _activeBackColor;
                    if (_tooltipText != null && _tooltipText.Length > 0)
                        _tooltip.Show(_tooltipText, this, 0, this.Height);
                }
            }
        }

        private void SetHoverOut()
        {
            try
            {
                if (Activated!=null && Activated.Equals(this))
                {
                    this._imageIndex = 0;
                    base.BackColor = _tempBackColor;
                    Refresh();
                    if (_tooltip != null) _tooltip.Hide(this);
                    Activated = null;
                    _isButtonClicked = false;
                }
            }
            catch { }
        }

        string _tooltipText = "";
        [Browsable(true)]
        public String ToolTipText
        {
            get { return _tooltipText; }
            set { _tooltipText = value; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            setTextPosition();
        }
        void setTextPosition()
        {
            int x = 0, y = 0;
            if ((_textPosition & AnchorStyles.Top) > 0)
            {
                y = this.Padding.Top + this.Padding.Top;
                if ((_textPosition & AnchorStyles.Left) > 0)
                {
                    x = 0 + this.Padding.Left;
                }
                else if ((_textPosition & AnchorStyles.Right) > 0)
                {
                    x = this.Width - L_Text.Width - this.Padding.Right;
                }
                else
                {
                    x = (this.Width - L_Text.Width) / 2;
                }
            }
            else if ((_textPosition & AnchorStyles.Bottom) > 0)
            {
                y = (this.Height - L_Text.Height) - this.Padding.Bottom;
                if ((_textPosition & AnchorStyles.Left) > 0)
                {
                    x = 0 + this.Padding.Left;
                }
                else if ((_textPosition & AnchorStyles.Right) > 0)
                {
                    x = this.Width - L_Text.Width - this.Padding.Right;
                }
                else
                {
                    x = (this.Width - L_Text.Width) / 2;
                }
            }
            else
            {
                y = (this.Height - L_Text.Height) / 2;
                if ((_textPosition & AnchorStyles.Left) > 0)
                {
                    x = 0 + this.Padding.Left;
                }
                else if ((_textPosition & AnchorStyles.Right) > 0)
                {
                    x = this.Width - L_Text.Width - this.Padding.Right;
                }
                else
                {
                    x = (this.Width - L_Text.Width) / 2;
                    
                }
            }
            /*
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
             */
            L_Text.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
        }

        Color _activeBackColor = Color.FromArgb(30, Color.Cyan);
        public Color ActiveBackColor
        {
            get { return _activeBackColor; }
            set { _activeBackColor = value; }
        }

        public ContentAlignment TextAlign
        {
            get
            {
                if ((_textPosition & AnchorStyles.Top)>0)
                {
                    if ((_textPosition & AnchorStyles.Left) > 0)
                    {
                        return ContentAlignment.TopLeft;
                    }
                    else if ((_textPosition & AnchorStyles.Right) > 0)
                    {
                        return ContentAlignment.TopRight;
                    }
                    else
                    {
                        return ContentAlignment.TopCenter;
                    }
                }
                else if ((_textPosition & AnchorStyles.Bottom) > 0)
                {
                    if ((_textPosition & AnchorStyles.Left) > 0)
                    {
                        return ContentAlignment.BottomLeft;
                    }
                    else if ((_textPosition & AnchorStyles.Right) > 0)
                    {
                        return ContentAlignment.BottomRight;
                    }
                    else
                    {
                        return ContentAlignment.BottomCenter;
                    }
                }
                else
                {
                    if ((_textPosition & AnchorStyles.Left) > 0)
                    {
                        return ContentAlignment.MiddleLeft;
                    }
                    else if ((_textPosition & AnchorStyles.Right) > 0)
                    {
                        return ContentAlignment.MiddleRight;
                    }
                    else
                    {
                        return ContentAlignment.MiddleCenter;
                    }
                }
            }
            set
            {
                if (value == ContentAlignment.BottomCenter)
                {
                    _textPosition = AnchorStyles.Bottom;
                }else if (value == ContentAlignment.BottomLeft)
                {
                    _textPosition = AnchorStyles.Bottom | AnchorStyles.Left;
                }else if (value == ContentAlignment.BottomRight)
                {
                    _textPosition = AnchorStyles.Bottom | AnchorStyles.Right;
                }else if (value == ContentAlignment.MiddleCenter)
                {
                    _textPosition = AnchorStyles.None;

                }else if (value == ContentAlignment.MiddleLeft)
                {
                    _textPosition = AnchorStyles.Left;
                }else if (value == ContentAlignment.MiddleRight)
                {
                    _textPosition = AnchorStyles.Right;
                }else if (value == ContentAlignment.TopCenter)
                {
                    _textPosition = AnchorStyles.Top;
                }else if (value == ContentAlignment.TopLeft)
                {
                    _textPosition = AnchorStyles.Top | AnchorStyles.Left;
                }
                else if (value == ContentAlignment.TopRight)
                {
                    _textPosition = AnchorStyles.Top | AnchorStyles.Right;
                }
                setTextPosition();
            }
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

            try
            {
                this.BackgroundImage = this._images[_buttonType * 3 + _imageIndex];
            }
            catch
            {
                try
                {
                    this.BackgroundImage = this._images[_imageIndex];
                }
                catch
                {
                    try
                    {
                        this.BackgroundImage = this._images[0];
                    }
                    catch
                    {
                        this.BackgroundImage = null;
                    }
                }
            }


            SetLayout();
        }

        private void SetLayout()
        {
            //this.Hide();
            if (this.TextAlign == ContentAlignment.BottomCenter)
            {

                //SizeF textSize = g.MeasureString(this.Text, this.Font);

                float x = (this.Width - L_Text.Width) / 2;
                float y = (this.Height - L_Text.Height) / 2;

                if (BackgroundImage != null)
                {
                    int ix = (this.Width - _picture.Width) / 2;
                    _picture.SetBounds(ix, this.Padding.Top, _picture.Width, _picture.Height);
                    L_Text.SetBounds((int)x, this.Height - (int)(L_Text.Height) - this.Padding.Bottom, 0, 0, BoundsSpecified.Location);
                    /*
                        DrawImage(g, this.BackgroundImage, this.Padding.Left, this.Padding.Top);
                        //g.DrawImage(this.BackgroundImage, new Point(this.Padding.Left, this.Padding.Top));
                        g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(x, this.Height - textSize.Height - this.Padding.Bottom));
                        */
                }
                else
                {
                    L_Text.SetBounds((int)x, (int)y, 0, 0, BoundsSpecified.Location);
                    //g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(x, y));
                }
            }
            else if (this.TextAlign == ContentAlignment.MiddleRight)
            {
                //float textHeight = g.MeasureString("Tj", this.Font).Height;
                float y = (this.Height - L_Text.Height) / 2;

                if (BackgroundImage != null)
                {
                    int iy = (this.Height - _picture.Height) / 2;
                    _picture.SetBounds(this.Padding.Left, iy, _picture.Width, _picture.Height);
                    L_Text.SetBounds(this._picture.Width + this.Padding.Left, (int)y, 0, 0, BoundsSpecified.Location);

                    //DrawImage(g, this.BackgroundImage, this.Padding.Left, this.Padding.Top);
                    //g.DrawImage(this.BackgroundImage, new Point(this.Padding.Left, this.Padding.Top));
                    //g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(this.BackgroundImage.Width + this.Padding.Left, y));
                }
                else
                {
                    L_Text.SetBounds(this._picture.Width + this.Padding.Left, (int)y, 0, 0, BoundsSpecified.Location);
                    //g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(this.BackgroundImage.Width + this.Padding.Left, y));
                }

            }

            /*
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
              */
            
            //L_Text.Refresh();
            setTextPosition();
            //this.Show();
        }
        

        string _text = "";
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
                Refresh();
            }
        }
        /*
         
        public new String Text
        {
            get { return _text; }
            set { _text = value; }
        }
        */

        //Image _backgroundImage = null;
        [Browsable(false)]
        public new Image BackgroundImage
        {
            get
            {
                return _picture.BackgroundImage;
                //return base.BackgroundImage;
            }
            set
            {
                if (value != null)
                {
                    _picture.BackgroundImage = value;

                    float percent1 = 1.0f;
                    if (this.Width < value.Width) percent1 = this.Width * 1.0f / value.Width;
                    float percent2 = 1.0f;
                    if (this.Height < value.Height) percent2 = this.Height * 1.0f / value.Height;
                    
                    if (percent1 < percent2) _picture.SetBounds(0,0, (int)( value.Width * percent1), (int)(value.Height * percent1), BoundsSpecified.Size);// .BackgroundImage _backgroundImage = ScaleByPercent(value, percent1);
                    else  _picture.SetBounds(0,0, (int)( value.Width * percent1), (int)(value.Height * percent2), BoundsSpecified.Size);// .BackgroundImage _backgroundImage = ScaleByPercent(value, percent1);// _backgroundImage = ScaleByPercent(value, percent2);
                }
                else
                {
                    _picture.BackgroundImage = null;
                }
                _picture.Refresh();
                SetLayout();
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
                if (_images != null)
                {
                    _images.E_ImageChanged -= _images_E_ImageChanged;
                }
                _images = value;
                if (_images != null)
                {
                    _images.E_ImageChanged += _images_E_ImageChanged;
                }
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

        void _images_E_ImageChanged(object sender, EventArgs e)
        {
            Refresh();
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

        Color _backColor = Color.Transparent;
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (_backColor.Equals(value) == false)
                {
                    _tempBackColor = this.BackColor;//다시 돌아올 색깔이다.
                    base.BackColor = value;
                    _backColor = value;
                }
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
        
        Image ScaleByPercent(Image imgPhoto, float rate)
        {

            //퍼센트 0.8 or 0.5 ..

            float nPercent = rate;// ((float)Percent / 100);

            //넓이와 높이

            int OriginalWidth = imgPhoto.Width;

            int OriginalHeight = imgPhoto.Height;

            //소스의 처음 위치

            int OriginalX = 0;

            int OriginalY = 0;

            //움직일 위치

            int adjustX = 0;

            int adjustY = 0;

            //조절될 퍼센트 계산

            int adjustWidth = (int)(OriginalWidth * nPercent);

            int adjustHeight = (int)(OriginalHeight * nPercent);

            //비어있는 비트맵 객체 생성

            Bitmap bmPhoto = new Bitmap(adjustWidth, adjustHeight, PixelFormat.Format24bppRgb);

            //이미지를 그래픽 객체로 만든다.

            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //사각형을 그린다.

            //그릴 이미지객체 크기, 그려질 이미지객체 크기

            grPhoto.DrawImage(imgPhoto,

                   new Rectangle(adjustX, adjustY, adjustWidth, adjustHeight),

                   new Rectangle(OriginalX, OriginalY, OriginalWidth, OriginalHeight),

                   GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;

        }

        void DrawImage(Graphics ge, Image img, float x, float y)
        {
            Image backbuff = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(backbuff))
            {
                g.Clear(Color.Transparent);
                
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                
                ImageAttributes attr = new ImageAttributes();
                attr.SetColorKey(Color.FromArgb(0,Color.Black), Color.FromArgb(20, Color.Black));
                

                Rectangle src_rect = new Rectangle(0, 0, img.Width, img.Height);
                //Rectangle dest_rect = new Rectangle(0, 0, this.Width, this.Height);
                // 이미지를 그린다.               
                g.DrawImage(img, src_rect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
            }
            // 백버퍼에 있는 내용을 화면에 그린다.
            ge.DrawImage(backbuff, x, y);

        }

        void DrawBack(Graphics ge)
        {
            Image backbuff = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppPArgb);
            
            // 백버퍼에 있는 내용을 화면에 그린다.
            ge.DrawImage(backbuff, 0, 0);

        }

       /*
        protected override void  OnPaint(PaintEventArgs e)
        {
            
            base.OnPaint(e);
            //e.Graphics.FillRectangle(_backColor, this.DisplayRectangle);
            Graphics g = e.Graphics;
            try
            {
                //g.FillRegion(new SolidBrush(Color.Transparent), new Region(e.ClipRectangle));//.Clear(Color.Transparent);
                //g.FillRectangle(Brushes.Transparent, new Rectangle(0,0, this.Width, this.Height));
                //DrawBack(g);
                //g.Clear(Color.Transparent);
                //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                if (this.TextAlign == ContentAlignment.BottomCenter)
                {

                    SizeF textSize = g.MeasureString(this.Text, this.Font);

                    float x = (this.Width - textSize.Width) / 2;
                    float y = (this.Height - textSize.Height) / 2;
                    if (BackgroundImage != null)
                    {
                        
                        DrawImage(g, this.BackgroundImage, this.Padding.Left, this.Padding.Top);
                        //g.DrawImage(this.BackgroundImage, new Point(this.Padding.Left, this.Padding.Top));
                        g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(x, this.Height - textSize.Height - this.Padding.Bottom));
                        
                    }
                    else
                    {
                        g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(x, y));
                    }
                }
                else if (this.TextAlign == ContentAlignment.MiddleRight)
                {
                    float textHeight = g.MeasureString("Tj", this.Font).Height;
                    float y = (this.Height - textHeight) / 2;

                    if (BackgroundImage != null)
                    {
                        
                        
                        DrawImage(g, this.BackgroundImage, this.Padding.Left, this.Padding.Top);
                        //g.DrawImage(this.BackgroundImage, new Point(this.Padding.Left, this.Padding.Top));
                        g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(this.BackgroundImage.Width + this.Padding.Left, y));
                    }
                    else
                    {
                        g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(this.BackgroundImage.Width + this.Padding.Left, y));
                    }
                }
                else
                {
                    base.OnPaint(e);
                }
            }
            catch
            {
                base.OnPaint(e);
            }
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
        */
        void ImgButton_MouseLeave(object sender, EventArgs e)
        {
            //SetHoverOut();

        }

        Color _tempBackColor = Color.Transparent;
        

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
            //if (Click != null) Click(this, e);
        }

        void ImgButton_MouseDown(object sender, MouseEventArgs e)
        {
            this._imageIndex = 2;
            _isButtonClicked = false;
            Refresh();
        }
        bool _isButtonClicked = false;
        void ImgButton_Click(object sender, EventArgs e)
        {
            if (_isButtonClicked == false)
            {
                _isButtonClicked = true;
                this._imageIndex = 0;
                if (Click != null) Click(this, e);
                //base.OnClick(e);
            }
            
        }
        #endregion ///////////////////////////////////////////////

    }

    public class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        public const Int32 ULW_COLORKEY = 0x00000001;
        public const Int32 ULW_ALPHA = 0x00000002;
        public const Int32 ULW_OPAQUE = 0x00000004;

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);
    }

}
