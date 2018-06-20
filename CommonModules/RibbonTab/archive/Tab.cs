using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;

namespace TabStripApp {

    // turn off the ability to add this in the DT, the TabPageSwitcher designer will 
    // provide this.
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
    public class Tab : ToolStripButton {

        private TabStripPage tabStripPage;
        
        public Tab() {
            Initialize();
        }
        public Tab(string text):base(text,null,null) {
            Initialize();
        }
        public Tab(Image image):base(null,image,null) {
            Initialize();
        }
        public Tab(string text, Image image):base(text,image,null) {
            Initialize();
        }
        public Tab(string text, Image image, EventHandler onClick):base(text,image,onClick) {
            Initialize();            
        }
        public Tab(string text, Image image, EventHandler onClick, string name):base(text,image,onClick,name) {
            Initialize();
        }

        private void Initialize() {
            CheckOnClick = true;
            TextAlign = ContentAlignment.MiddleCenter;
            // todo: place any initialization code here
        }

        // shadow CheckOnClick so we can control the serialization by specifiying a defaultValue attribute.
        [DefaultValue(true)]
        public new bool CheckOnClick {
            get { return base.CheckOnClick; }
            set { base.CheckOnClick = value; }
        }

        protected override ToolStripItemDisplayStyle DefaultDisplayStyle {
            get {
                return ToolStripItemDisplayStyle.ImageAndText;
            }
        }
        [Browsable(false)]
        public override ToolStripItemDisplayStyle DisplayStyle {
            get {               
                return base.DisplayStyle;
            }
            set {
                // Designer-friendlyness.  The ToolStripDesigner will
                // set the DisplayStyle to Image.
                if (!DesignMode) {
                    base.DisplayStyle = value;
                }
            }
        }
        protected override Padding DefaultPadding {
            get {
                return new Padding(35, 0, 6, 0);
            }
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        public override ContentAlignment TextAlign {
            get {
                return base.TextAlign;
            }
            set {
                base.TextAlign = value;
            }
        }

        [DefaultValue("null")]
        public TabStripPage TabStripPage {
            get {
                return tabStripPage;
            }
            set {
                tabStripPage = value;
            }
        }

    
      
    }
}
