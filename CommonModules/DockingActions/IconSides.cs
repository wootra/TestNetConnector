using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UCoreComponents;

namespace DockingActions
{
    internal partial class IconSides : UserControl
    {
        public enum ButtonPosition { B_Top, B_Bottom, B_Left, B_Right};
        
        ButtonPosition _bPos;
        DockingRoot _root;

        public IconSides(DockingRoot root, ButtonPosition pos)
        {
            InitializeComponent();
            _bPos = pos;
            switch (_bPos)
            {
                case ButtonPosition.B_Bottom:
                    this.BackgroundImage = global::DockingActions.Properties.Resources.bottom;
                    break;
                case ButtonPosition.B_Left:
                    this.BackgroundImage = global::DockingActions.Properties.Resources.left;
                    break;
                case ButtonPosition.B_Right:
                    this.BackgroundImage = global::DockingActions.Properties.Resources.right;
                    break;
                case ButtonPosition.B_Top:
                    this.BackgroundImage = global::DockingActions.Properties.Resources.top;
                    break;
            }
            _root = root;
            EventHandler mouseHover = new EventHandler(OnMouseHoverOnRootButton);
            EventHandler mouseLeave = new EventHandler(OnMouseOutFromButton);
            
            B_PosBtn.MouseHover += mouseHover;
            B_PosBtn.MouseLeave += mouseLeave;
        }

        void OnMouseHoverOnRootButton(object sender, EventArgs e)
        {
            _root.setHoverRootButton(_bPos);
        }

        void OnMouseOutFromButton(object sender, EventArgs e)
        {
            _root.setLeaveButton();
        }
    }
}
