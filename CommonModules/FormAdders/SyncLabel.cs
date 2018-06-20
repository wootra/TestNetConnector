using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.ComponentModel;

namespace FormAdders
{
    public partial class SyncLabel : Label
    {
        public SyncLabel():base()
        {
            
            //this.Text = "";
            //_refreshTimer.Tick += new EventHandler(_refreshTimer_Tick);
            //_refreshTimer.Interval = 10;
            _movingLineTimer.Interval = 200;
            _movingLineTimer.Tick += new EventHandler(_movingLineTimer_Tick);
            _tempText = "";
            //_refreshTimer.Start();
        }

        void _movingLineTimer_Tick(object sender, EventArgs e)
        {
            if(_rectStyle == RectStyles.Moving) DrawMovingLine();
        }
        Timer _movingLineTimer = new Timer();
        /*
        void SetTimerInterval(int timeInMs)
        {
            _refreshTimer.Stop();
            _refreshTimer.Interval = timeInMs;
            _refreshTimer.Start();
        }

        void _refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_tempText.Length > 0)
            {
                if (base.Text.Equals(_tempText) == false)
                {
                    base.Text = _tempText;
                    Refresh();
                }
                _refreshTimer.Stop();
            }
        }
        */
        //Timer _refreshTimer = new Timer();
        Boolean _isDrawRectOnOut = true;
        
        public Boolean IsDrawRact { get { return _isDrawRectOnOut; } set { _isDrawRectOnOut = value; } }
        public enum RectStyles { ButtonUp, ButtonDown, Solid, Moving };
        RectStyles _rectStyle = RectStyles.Solid;
        public RectStyles RectStyle { get { return _rectStyle; } set { _rectStyle = value; } }

        public SolidBrush _solidBrush = new SolidBrush(Color.Black);
        public Color LineColor{ get{ return _solidBrush.Color; } set{ _solidBrush.Color = value;}}

        void init()
        {
            //InitializeComponent();
        }

        String _tempText = "";
        [Browsable(true)]
        [EditorBrowsable]
        public override string Text
        {
            get
            {
                return _tempText;
            }
            set
            {
                if (value != null) _tempText = value;
                else _tempText = "";
                func funcfunc = textRefresh;
                try
                {
                    if (this.InvokeRequired) this.Invoke(funcfunc);
                    else textRefresh();
                }
                catch { }
            }
        }
        
        delegate void func();
        void textRefresh()
        {
            
            base.Text = _tempText;
            
            Graphics g = this.CreateGraphics();
            SizeF size = g.MeasureString(_tempText, this.Font);
            Control parent = Parent as Control;

            if (parent != null)
            {
                this.AutoSize = false;
                parent.SuspendLayout();
                //this.Refresh();
                //this.SetBounds(0, 0, (int)size.Width, (int)size.Height, BoundsSpecified.Size);

                this.Size = new Size((int)size.Width + 30, (int)size.Height + 3);
                //this.Width = (int)size.Width;
                parent.ResumeLayout();
            }
            else
            {
                this.AutoSize = false;
                //this.SetBounds(0, 0, (int)size.Width, (int)size.Height, BoundsSpecified.Size);
                //this.ClientSize = new Size((int)size.Width, (int)size.Height);
                this.Size = new Size((int)size.Width+30, (int)size.Height+3);
                //this.Refresh();

            }
            this.Refresh();
            /*
            Graphics g = this.CreateGraphics();
            SizeF size = g.MeasureString(_tempText, this.Font);
            Control parent = Parent as Control;
            
            if (parent != null)
            {
                this.AutoSize = false;
                parent.SuspendLayout();
                //this.Refresh();
                //this.SetBounds(0, 0, (int)size.Width, (int)size.Height, BoundsSpecified.Size);
                
                this.Size = new Size((int)size.Width+1, (int)size.Height+1);
                //this.Width = (int)size.Width;
                parent.ResumeLayout();
            }
            else
            {
                
                //this.SetBounds(0, 0, (int)size.Width, (int)size.Height, BoundsSpecified.Size);
                //this.ClientSize = new Size((int)size.Width, (int)size.Height);
                this.Size = new Size((int)size.Width, (int)size.Height);
                //this.Refresh();
                
            }
             *  */
            //if (_refreshTimer.Enabled == false) _refreshTimer.Start();
            
        }
        
        Image _image = null;
        [Browsable(true)]
        [EditorBrowsable]
        public Image Image {
            get { return _image; }
            set
            {
                _image = value;
                this.Refresh();
            }
        }

        SolidBrush _backBrush = new SolidBrush(Color.Transparent);
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                _backBrush = new SolidBrush(value);
            }
        }

        int moveOffset=0;
        int dashSize=5;
        int blankSize=5;
        Graphics g;
        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            g = e.Graphics;
            Control parent = this as Control;
            Graphics pg = parent.CreateGraphics();
            Rectangle prect = new Rectangle(e.ClipRectangle.X+this.Location.X, e.ClipRectangle.Y+this.Location.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
            
            if (this.Image != null)
            {
                //g.FillRectangle(_backBrush, e.ClipRectangle);
                //g.DrawImage(this.Image, 0, (this.Height - this.Image.Height) / 2, Image.Width, Image.Height);
                //g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new Point(this.Image.Width, (this.Height - this.Font.Height) / 2));
                pg.FillRectangle(_backBrush, prect);
                pg.DrawImage(this.Image, prect.X, prect.Y + (this.Height - this.Image.Height) / 2, Image.Width, Image.Height);
                pg.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new Point(prect.X + this.Image.Width, prect.Y+ (this.Height - this.Font.Height) / 2));
            }
            else
            {
                //pg.FillRectangle(_backBrush, prect);
                //pg.DrawImage(this.Image, prect.X, prect.Y + (this.Height - this.Image.Height) / 2, Image.Width, Image.Height);
                //pg.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new Point(prect.X, prect.Y));
                base.OnPaint(e);
                //g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new PointF(10, (this.Height - this.Font.Height) / 2));
            }
            
            //g.Clear(this.BackColor);
            Pen pen;
            if (_isDrawRectOnOut)
            {
                switch (_rectStyle)
                {
                    case RectStyles.ButtonUp:
                        g.DrawLine(Pens.White, 0, 0, this.Width - 1, 0);
                        g.DrawLine(Pens.White, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                        g.DrawLine(Pens.Gray, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                        g.DrawLine(Pens.Gray, 0, this.Height - 1, 0, 0);
                        //g.DrawRectangle(Pens.Orange, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                        break;
                    case RectStyles.ButtonDown:
                        g.DrawLine(Pens.Gray, 0, 0, this.Width - 1, 0);
                        g.DrawLine(Pens.Gray, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                        g.DrawLine(Pens.White, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                        g.DrawLine(Pens.White, 0, this.Height - 1, 0, 0);
                        break;
                    case RectStyles.Moving:
                        DrawMovingLine();
                        break;
                    case RectStyles.Solid:
                        pen = new Pen(_solidBrush);
                        g.DrawLine(pen, 0, 0, this.Width - 1, 0);
                        g.DrawLine(pen, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                        g.DrawLine(pen, this.Width - 1, this.Height - 1, 0, this.Height - 1);
                        g.DrawLine(pen, 0, this.Height - 1, 0, 0);
                        break;
                }

            }

            

            
        }
         */

        void DrawMovingLine()
        {
            g = this.CreateGraphics();
            Pen pen = new Pen(_solidBrush);
            Pen blankPen = new Pen(this.BackColor);
            int size = (blankSize + dashSize);
            int dotNum = this.Width / size;
            int remain = moveOffset - blankSize;
            Point pt1 = new Point(0, 0);
            Point pt2 = new Point(this.Width - 1, 0);
            Point oldPt = pt1;
            Point outPt1 = new Point();
            Point outPt2 = new Point();
            int index = 0;
            while (GetPoint(pt1, pt2, dashSize, blankSize, moveOffset, index++, ref outPt1, ref  outPt2))
            {
                g.DrawLine(blankPen, oldPt, outPt1);
                oldPt = outPt2;
                g.DrawLine(pen, outPt1, outPt2);

            }
            pt1 = new Point(this.Width - 1, 0);
            pt2 = new Point(this.Width - 1, this.Height - 1);
            index = 0;
            while (GetPoint(pt1, pt2, dashSize, blankSize, moveOffset, index++, ref outPt1, ref  outPt2))
            {
                g.DrawLine(blankPen, oldPt, outPt1);
                oldPt = outPt2;

                g.DrawLine(pen, outPt1, outPt2);
            }
            pt1 = new Point(this.Width - 1, this.Height - 1);
            pt2 = new Point(0, this.Height - 1);
            index = 0;
            while (GetPoint(pt1, pt2, dashSize, blankSize, moveOffset, index++, ref outPt1, ref  outPt2))
            {
                g.DrawLine(blankPen, oldPt, outPt1);
                oldPt = outPt2;
                g.DrawLine(pen, outPt1, outPt2);
            }
            pt1 = new Point(0, this.Height - 1);
            pt2 = new Point(0, 0);
            index = 0;
            while (GetPoint(pt1, pt2, dashSize, blankSize, moveOffset, index++, ref outPt1, ref  outPt2))
            {
                g.DrawLine(blankPen, oldPt, outPt1);
                oldPt = outPt2;
                g.DrawLine(pen, outPt1, outPt2);
            }
            /*
            g.DrawLine(pen, 0, 0, this.Width - 1, 0);
            g.DrawLine(pen, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            g.DrawLine(pen, this.Width - 1, this.Height - 1, 0, this.Height - 1);
            g.DrawLine(pen, 0, this.Height - 1, 0, 0);
            */
            moveOffset = (moveOffset + 1) % size;
        }
        bool GetPoint(Point pt1, Point pt2, int dashSize, int blankSize,int offset, int index, ref Point outPt1, ref Point outPt2)
        {
            int wid = pt2.X - pt1.X;
            int hei = pt2.Y - pt1.Y;
            int size = dashSize + blankSize;
            outPt1.X = pt1.X;
            outPt1.Y = pt1.Y;
            outPt2.X = pt1.X;
            outPt2.Y = pt1.Y;
            if (offset > blankSize && index == 0) //blank보다 많이 움직이면 빈 곳이 생긴다.
            {
                outPt1 = pt1;
                if (hei == 0)
                { //가로선
                    if(wid<0) outPt2.X = pt1.X - (offset - blankSize); //우->좌
                    else outPt2.X = pt1.X + (offset - blankSize); //좌->우
                }
                else if (wid == 0)//세로선
                {
                    if (hei < 0) outPt2.Y = pt1.Y - (offset - blankSize); //아래->위
                    else outPt2.Y = pt1.Y + (offset - blankSize); //위->아래
                }
                else
                {
                    return false;
                }

            }
            else
            {
                if (offset > blankSize && index > 0) index--;
                if (hei == 0) //가로선
                {
                    if (wid < 0)
                    {
                        outPt1.X = pt1.X - (offset + index * size);
                        outPt2.X = pt1.X - (offset + index * size + dashSize); //우->좌
                        if (outPt1.X < pt2.X) return false;
                        else if (outPt2.X < pt2.X) outPt2.X = pt2.X;
                    }
                    else
                    {
                        outPt1.X = pt1.X + (offset + index * size);
                        outPt2.X = pt1.X + (offset + index * size + dashSize); //좌->우
                        if (outPt1.X > pt2.X) return false;
                        else if (outPt2.X > pt2.X) outPt2.X = pt2.X;
                    }
                    
                }
                else if (wid == 0) //세로선
                {
                    if (hei < 0)
                    {
                        outPt1.Y = pt1.Y - (offset + index * size);
                        outPt2.Y = pt1.Y - (offset + index * size + dashSize); //아래->위
                        if (outPt1.Y < pt2.Y) return false;
                        else if (outPt2.Y < pt2.Y) outPt2.Y = pt2.Y;
                    }
                    else
                    {
                        outPt1.Y = pt1.Y + (offset + index * size);
                        outPt2.Y = pt1.Y + (offset + index * size + dashSize);  //위->아래
                        if (outPt1.Y > pt2.Y) return false;
                        else if (outPt2.Y > pt2.Y) outPt2.Y = pt2.Y;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
