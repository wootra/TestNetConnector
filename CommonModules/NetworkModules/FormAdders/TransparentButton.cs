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
    public partial class TransparentButton : Panel
    {
        private Color _foreColor;
        public new event EventHandler Click;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event MouseEventHandler MouseMove;
        private System.Windows.Forms.Label L_Text;
        private System.Windows.Forms.PictureBox Pic_Icon;


        private System.ComponentModel.IContainer components = null;
        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.L_Text = new System.Windows.Forms.Label();
            this.Pic_Icon = new System.Windows.Forms.PictureBox();
           // ((System.ComponentModel.ISupportInitialize)(this.Pic_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // L_Text
            // 
            this.L_Text.AutoSize = true;
            this.L_Text.Location = new System.Drawing.Point(43, 54);
            this.L_Text.Name = "L_Text";
            this.L_Text.Size = new System.Drawing.Size(38, 12);
            this.L_Text.TabIndex = 0;
            // 
            // Pic_Icon
            // 
            this.Pic_Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Pic_Icon.Location = new System.Drawing.Point(0, 0);
            this.Pic_Icon.Name = "Pic_Icon";
            this.Pic_Icon.Size = new System.Drawing.Size(100, 50);
            this.Pic_Icon.TabIndex = 0;
            this.Pic_Icon.TabStop = false;
            // 
            // TransparentButton
            // 
            this.Controls.Add(this.L_Text);
            this.Controls.Add(this.Pic_Icon);
            //((System.ComponentModel.ISupportInitialize)(this.Pic_Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        public TransparentButton()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            L_Text.BackColor = Color.Transparent;
            this.ForeColor = L_Text.ForeColor;
            this.BackColor = Color.Transparent;

            base.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            this.SizeChanged += new EventHandler(ImgButton_SizeChanged);
            base.MouseDown += new MouseEventHandler(ImgButton_MouseDown);
            base.MouseMove += new MouseEventHandler(TransparentButton_MouseMove);
            L_Text.MouseUp += new MouseEventHandler(ImgButton_MouseUp);
            L_Text.MouseDown += new MouseEventHandler(Label_MouseDown);
            L_Text.MouseMove += new MouseEventHandler(TransparentButton_MouseMove);
            Pic_Icon.SetBounds(0, (this.Height - Pic_Icon.Height) / 2, 0, 0, BoundsSpecified.All);
        }



        /// <summary>
        /// 버튼에 표시 될 내용
        /// </summary>
        ///
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [SettingsBindable(true)]
        [BrowsableAttribute(true)]
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

        public enum Align { Center, Left, Right };
        Align _align = Align.Center;
        [DefaultValue(Align.Center)]
        [BrowsableAttribute(true)]
        public Align TextAlign
        {
            get
            {
                return this._align;
            }
            set
            {
                _align = value;
                ImgButton_SizeChanged(null, null);
            }
        }

        Image _icon = null;
        [BrowsableAttribute(true)]
        [Bindable( BindableSupport.Yes)]
        [SettingsBindable(true)]
        public Image Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                _icon = value;
                if (_icon == null) return;
                Pic_Icon.Image = _icon;
                Pic_Icon.InitialImage = _icon;
                Pic_Icon.Visible = true;
                Pic_Icon.Enabled = true;
                                
                Pic_Icon.SetBounds(0, 0, _icon.Width, _icon.Height, BoundsSpecified.Size);
                Pic_Icon.Update();
                
                //Pic_Icon.Width = _icon.Width;
                //Pic_Icon.Height = _icon.Height;
                this.Refresh();
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
                Refresh();
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
                Refresh();
            }
        }

        public override void Refresh()
        {
            
            base.Refresh();

            if(_icon!=null) Pic_Icon.BackgroundImage = _icon;
            L_Text.Refresh();
        }

        void ImgButton_SizeChanged(object sender, EventArgs e)
        {
            Size size = this.L_Text.Size;
            Point newPoint = new Point();
            int newX=0;
            int newY=0;
            switch (_align)
            {
                case Align.Center:
                    newX =  (this.Width - size.Width) / 2;
                    newY = (this.Height - size.Height) / 2;
                    break;
                case Align.Left:
                    newX =  Pic_Icon.Width + 10;
                    newY = (this.Height - size.Height) / 2;
                    break;
                case Align.Right:
                    newX =  (this.Width - size.Width);
                    newY = (this.Height - size.Height) / 2;
                    break;
            }
            newPoint.X = newX;
            newPoint.Y = newY;
            Pic_Icon.SetBounds(0, (this.Height-Pic_Icon.Height)/2, 0, 0, BoundsSpecified.Location);
            this.L_Text.SetBounds(newX, newY,0,0, BoundsSpecified.Location);
            L_Text.Refresh();
        }

 

        void ImgButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null) MouseUp(sender, e);

            if (Click != null)
            {
                EventArgs ea = (EventArgs)e;
                int x = Control.MousePosition.X;
                int y = Control.MousePosition.Y;
                Point pt = this.PointToClient(new Point(x, y));
                if (pt.X > 0 && pt.X < this.Width && pt.Y > 0 && pt.Y < this.Height) Click(this, ea);
                //컨트롤의 영역 안에 있으면 클릭이벤트 활성화
            }
        }

        void ImgButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null) MouseDown(sender, e);
        }

        void Label_MouseDown(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            MouseEventArgs arg = new MouseEventArgs(e.Button, e.Clicks, e.X + label.Location.X, e.Y + label.Location.Y, e.Delta);
            

            if (MouseDown != null) MouseDown(this, arg);
        }
        
        void TransparentButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.MouseMove != null) MouseMove(sender, e);
        }
    }
}
