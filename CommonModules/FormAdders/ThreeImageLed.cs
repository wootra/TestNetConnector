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
    public partial class ThreeImageLed : UserControl
    {
        private FineImageList _images = new FineImageList();
        private int _imageIndex=0;
        
        public ThreeImageLed()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreeImageLed));
            //_onImage = ((System.Drawing.Image)(resources.GetObject("pic1.Image")));
            this.Resize += new EventHandler(ThreeImageLed_Resize);
            _images.Add(global::FormAdders.Properties.Resources.led_off);
            _images.Add(global::FormAdders.Properties.Resources.led_on);
            _images.Add(global::FormAdders.Properties.Resources.led_mid);
            Off();
        }

        void ThreeImageLed_Resize(object sender, EventArgs e)
        {
           // this.pic.Location = new Point((this.Width - this.pic.Width) / 2, (this.Height - this.pic.Height) / 2);
            this.pic.Refresh();
        }

        private delegate void OnPressed(Boolean isOn);


        private delegate void On2Pressed(ConnType connType);
        public void U_SetState(ConnType connType)
        {
            if (this.InvokeRequired)
            {
                On2Pressed onPressed = new On2Pressed(U_SetState);
                this.Invoke(onPressed, new object[] { connType });
            }else{
                lock (this)
                {
                    this.U_ImageIndex = (int)connType;
                   
                    //this.pic.Location = new Point((this.Width - this.pic.Width) / 2, (this.Height - this.pic.Height) / 2);
            
                    this.pic.Refresh();
                }
            }
            //this.pic.Refresh();
        }
        public void On()
        {
            U_SetState(ConnType.Connected);
        }

        public void Off()
        {
            U_SetState(ConnType.Disconnected);
            //this.pic.Refresh();
        }

        public void Mid()
        {
            U_SetState(ConnType.Connecting);
        }

        [BrowsableAttribute(true)]
        [SettingsBindable(true)]
        public int U_ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                if (value == _imageIndex) return; //같을 경우 아무것도 안한다.
                _imageIndex = value;
                if(_images.Count>value) this.pic.Image = _images[value];
               // this.pic.Location = new Point((this.Width - this.pic.Width) / 2, (this.Height - this.pic.Height) / 2);
                
                this.pic.Refresh();
            }
        }
        [BrowsableAttribute(true)]
        [SettingsBindable(true)]
        public FineImageList ImageList
        {
            get
            {
               return _images;
            }
            set
            {
                _images = value;
                this.U_ImageIndex = 0;
                this.pic.Refresh();
                Refresh();
                
               // this.pic.Location = new Point((this.Width - this.pic.Width) / 2, (this.Height - this.pic.Height) / 2);
            }
        }
    }
}
