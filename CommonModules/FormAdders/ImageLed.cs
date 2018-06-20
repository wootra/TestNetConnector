using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RtwEnums.Network;

namespace FormAdders
{
    public partial class ImageLed : UserControl
    {
        public FineImageList _images = new FineImageList();
        Timer _refreshTimer = new Timer();
        bool _isRefresh = true;

        public ConnType _state = ConnType.Disconnected;
        public ToolTip Tooltip;
        public ImageLed()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnOffLed));
            //_onImage = ((System.Drawing.Image)(resources.GetObject("pic1.Image")));
            _refreshTimer.Interval = 200;
            _refreshTimer.Tick += new EventHandler(_refreshTimer_Tick);
            _images.Images = new Image[]{
                Properties.Resources.led_off,
                Properties.Resources.led_on,
                Properties.Resources.led_mid};
            Off();
            Tooltip = new ToolTip();
            
        }

        void _refreshTimer_Tick(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
            if (_isRefresh)
            {
                _isRefresh = true;
                setStateNow(_state);
                _isRefresh = false;
            }
            
        }

        void setStateNow(ConnType state)
        {
            _state = state;
            this.Invalidate(true);//.Refresh();
            this.Update();
        }

        public void setImage(Image onImage, Image offImage, Image midImage=null){
            _images.Add(offImage);
            _images.Add(onImage);
            _images.Add(midImage);
        }

        Brush _backColor = new SolidBrush(Color.Transparent);
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                _backColor = new SolidBrush(value);
            }
        }

        public ConnType getState() { return _state; }

        private delegate void OnPressed(Boolean isOn);

        
        private delegate void On2Pressed(ConnType connType);
        public void SetState(ConnType connType)
        {
            try
            {
                if (_state != connType)
                {
                    if (this.InvokeRequired)
                    {
                        On2Pressed func = setStateNow;
                        this.Invoke(func, connType);
                    }
                    else
                    {
                        setStateNow(connType);
                        //_state = connType;

                        //_isRefresh = true;
                        //_refreshTimer.Start();
                    }
                }
            }
            catch { }
            //this.pic.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (_isRefresh)
            {
                Graphics g = e.Graphics;
                //g.FillRectangle(_backColor, this.ClientRectangle);
                try
                {
                    if (_images[(int)_state] != null)
                    {
                        g.DrawImage(_images[(int)_state], ClientRectangle);
                    }
                }
                catch { }
                //_isRefresh = false;
            }
        }
        
        public void On()
        {
            SetState(ConnType.Connected);
        }

        public void Off()
        {
            SetState(ConnType.Disconnected);
            //this.pic.Refresh();
        }
        public void Mid()
        {
            SetState(ConnType.Connecting);
        }

        public FineImageList LedImages
        {
            get { return _images; }
            set
            {
                _images = value;
                this.Refresh();
            }
        }
    }
}
