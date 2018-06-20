using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;
using DataHandling;
using UCoreComponents;

namespace DockingActions
{
    public partial class TitleBar : TransparentButton
    {
        DockingContainer _parent;
        DraggingPopup _popup;
        DockingRoot _root;
        MouseEventHandler _mouseDown;
        MouseEventHandler _mouseUp;
        MouseEventHandler _mouseMove;

        internal TitleBar(DockingContainer parent, DraggingPopup popup, DockingRoot root)
            : base()
        {
            InitializeComponent();
            _parent = parent;
            _popup = popup;
            _root = root;
            _mouseDown = new MouseEventHandler(TitleBar_MouseDown);
            _mouseUp = new MouseEventHandler(TitleBar_MouseUp);
            _mouseMove = new MouseEventHandler(TitleBar_MouseMove);

            this.Text = parent.Name;
            B_X.Click += new EventHandler(B_X_Click);
            B_Pin.Click += new EventHandler(B_Pin_Click);
            B_DownArrow.Click += new EventHandler(B_DownArrow_Click);
            this.MouseDown += _mouseDown;
            //this.MouseMove += _mouseMove;
        }

        Point _pressedPt;
        Boolean _isDragging = false;
        void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            
            if(_parent.isInPopup()==false){
                Rectangle rect = CoodinateHandling.GetFormBoundWithControlsBound(_parent, _popup);

                _popup.SetBounds(rect.X, rect.Y, rect.Width+_popup.Padding.Left+_popup.Padding.Right, rect.Height+_popup.Padding.Top+_popup.Padding.Bottom, BoundsSpecified.All);
                _popup.Show();
                _popup.setContent(this._parent);
                _parent.setNowInPopup(true);
                _root.Disconnect(_parent.Name);
                _parent.PopThisFromParent();
                _popup.BringToFront();
            }
            _pressedPt = CoodinateHandling.GetWindowPointFromControlOffset(_popup, this, e.X, e.Y);

            _isDragging = true;
            _popup.Opacity = 0.7;
            this.MouseDown -= _mouseDown;
            this.MouseMove += _mouseMove;
            //_popup.MouseMove += _mouseMove;
            this.MouseUp += _mouseUp;
            _root.DragBegin(_parent, Control.MousePosition.X, Control.MousePosition.Y);
        }
        void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            this.MouseDown += _mouseDown;
            this.MouseMove -= _mouseMove;
            this.MouseUp -= _mouseUp;
            _isDragging = false;
            _popup.Opacity = 1.0;
            _root.DragEnd(_parent, Control.MousePosition.X, Control.MousePosition.Y);
        }
        void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            
            if(_isDragging == true){
                _popup.SetBounds(Control.MousePosition.X - _pressedPt.X, Control.MousePosition.Y - _pressedPt.Y, 0, 0, BoundsSpecified.Location);
                _root.Dragging(_parent, Control.MousePosition.X, Control.MousePosition.Y);
            }
        }
        DockingContainer getOutOfPopup()
        {
            _popup.removeContent();
            _parent.setNowInPopup(false);
            return _parent;
        }
        // <summary>
        // Don't use this.. This is for Designer
        // </summary>
        public TitleBar()
            : base()
        {
            InitializeComponent();
            
        }

        void B_DownArrow_Click(object sender, EventArgs e)
        {
           //reserved
        }

        void B_Pin_Click(object sender, EventArgs e)
        {
            if (_parent.isInPopup())
            {
                _root.addToTab(_parent, _parent.initDock());
                _popup.Hide();
            }
            else
            {
                _root.addToTab(_parent, _parent.initDock());
                _parent.PopThisFromParent();
            }
        }

        void B_X_Click(object sender, EventArgs e)
        {
            if (_parent.isInPopup())
            {
                _popup.Hide();
                //_root.Disconnect(_parent);
                _parent.Dispose();
            }
            else
            {
                _parent.PopThisFromParent();
                _root.Disconnect(_parent.Name);
                _parent.Dispose();

                /*
                if (_isInPopup == false)
                {
                    Rectangle rect = CoodinateHandling.GetFormBoundWithControlsBound(_parent, _popup);

                    _popup.SetBounds(rect.X, rect.Y, rect.Width + _popup.Padding.Left + _popup.Padding.Right, rect.Height + _popup.Padding.Top + _popup.Padding.Bottom, BoundsSpecified.All);
                    _popup.Show();
                    _popup.setContent(this._parent);
                    
                    _isInPopup = true;
                    _popup.BringToFront();
                }
                _pressedPt = CoodinateHandling.GetWindowPointFromControlOffset(_popup, this, e.X, e.Y);

                _isDragging = true;
                _popup.Opacity = 0.7;
                this.MouseDown -= _mouseDown;
                this.MouseMove += _mouseMove;
                //_popup.MouseMove += _mouseMove;
                this.MouseUp += _mouseUp;
                _root.DragBegin(_parent, Control.MousePosition.X, Control.MousePosition.Y);
                 */
            }
        }
    }
}
