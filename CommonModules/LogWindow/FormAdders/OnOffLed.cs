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
    public partial class OnOffLed : UserControl
    {
        public static Image _onImage = Properties.Resources.led_on;
        public static Image _offImage = Properties.Resources.led_off;
        public static Image _midImage = Properties.Resources.led_mid;

        public ConnType _state = ConnType.Disconnected;

        public OnOffLed()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnOffLed));
            //_onImage = ((System.Drawing.Image)(resources.GetObject("pic1.Image")));
            _onImage = Properties.Resources.led_on; ;
            _offImage = Properties.Resources.led_off; ;
            _midImage = Properties.Resources.led_mid; ;
            Off();
        }
        public static void setImage(Image onImage, Image offImage, Image midImage=null){
            _onImage = onImage;
            _offImage = offImage;
            _midImage = midImage;
        }

        public ConnType getState() { return _state; }

        private delegate void OnPressed(Boolean isOn);
        public void On()
        {
            SetState(ConnType.Connected);
        }
        
        private delegate void On2Pressed(ConnType connType);
        public void SetState(ConnType connType)
        {
            if (_state != connType)
            {
                _state = connType;
                if (this.InvokeRequired)
                {
                    On2Pressed onPressed = new On2Pressed(setStateInvoke);
                    try
                    {
                        this.Invoke(onPressed, new object[] { connType });
                    }
                    catch { }
                }
                else
                {
                    setStateInvoke(connType);
                }
            }
            //this.pic.Refresh();
        }
        public void setStateInvoke(ConnType connType)
        {
            lock (this)
            {
                if (connType == ConnType.Connected) this.pic.Image = _onImage;
                else if (connType == ConnType.Connecting) this.pic.Image = _midImage;
                else this.pic.Image = _offImage;
            }
        }

        public void Off()
        {
            SetState(ConnType.Disconnected);
            //this.pic.Refresh();
        }

    }
}
