using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders.EasyGridViewCollections;


namespace FormAdders
{


    [Designer(typeof(FormAdders.Designer.ImageTabPageDesigner))]
    public class ImageTabPage : Panel
    {
        /*
        [Browsable(false)]
        public ContainerControl Panel
        {
            get { return this; }
        }
        */
        internal int _index = -1;
        public int Index
        {
            get
            {
                return _index;
            }
        }


        [Category("Custom")]
        [Browsable(true)]
        [EditorBrowsable]
        public Image ActiveImage { get; set; }

        [Category("Custom")]
        [Browsable(true)]
        [EditorBrowsable]
        public Image InActiveImage { get; set; }

        [Category("Custom")]
        [Browsable(true)]
        [EditorBrowsable]
        public Brush ActiveTextColor { get; set; }

        [Category("Custom")]
        [Browsable(true)]
        [EditorBrowsable]
        public Brush InactiveTextColor { get; set; }

        [Category("Custom")]
        [Browsable(true)]
        [EditorBrowsable]
        public override String Text
        {
            get { return base.Text; }
            set {
                base.Text = value;
                if (Parent != null)
                {
                    (Parent as ImageTabControl).InU();
                }
            }
        }

        internal bool _selected;
        [Browsable(false)]
        public bool Selected
        {
            get { return _selected; }
        }

        public ImageTabPage()
            : base()
        {
            Text = "";
        }

        public ImageTabPage(String text)
            : base()
        {
            this.Text = text;

        }

        public ImageTabPage(String text, Image activeImage, Image inactiveImage)
            : base()
        {
            this.Text = text;
            this.ActiveImage = activeImage;
            this.InActiveImage = inactiveImage;
        }

        public ImageTabPage(String text, Image activeImage, Image inActiveImage, Brush activeTextColor, Brush inactiveTextColor)
            : base()
        {
            this.Text = text;
            this.ActiveImage = activeImage;
            this.InActiveImage = inActiveImage;
            this.ActiveTextColor = activeTextColor;
            this.InactiveTextColor = inactiveTextColor;

        }
    }
}
