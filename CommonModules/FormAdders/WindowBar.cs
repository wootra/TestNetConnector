using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.ComponentModel;
using FormAdders; using DataHandling;

namespace FormAdders
{
    public class WindowBar:TransparentButton
    {
        Control _parent;
        Point _clickPt;
        Point _movingPt;
        MouseEventHandler _mouseMoveEvent;
        public event MouseEventHandler U_DragBegin;
        public event MouseEventHandler U_Moving;
        public event MouseEventHandler U_DragEnd;

        public WindowBar():base()
        {
            _parent = null;
            _clickPt = new Point(0, 0);
            _movingPt = new Point(0, 0);
            
            _mouseMoveEvent = new MouseEventHandler(WindowBar_MouseMove);
            this.MouseDown += new MouseEventHandler(WindowBar_MouseDown);
            this.MouseUp += new MouseEventHandler(WindowBar_MouseUp);
        }

        public void setParent(Control parent){
            _parent = parent;
        }

        [Browsable(true)]
        public Control U_Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        MouseEventArgs moveWindow(MouseEventArgs e)
        {
            _movingPt.X = Control.MousePosition.X - _clickPt.X;
            _movingPt.Y = Control.MousePosition.Y - _clickPt.Y;

            _parent.Location = _movingPt;


            return new MouseEventArgs(e.Button, e.Clicks, _movingPt.X, _movingPt.Y, e.Delta);
        }

        protected void WindowBar_MouseUp(object sender, MouseEventArgs e)
        {
            //_moveTimer.Stop();
            this.MouseMove -= _mouseMoveEvent;
            MouseEventArgs arg = moveWindow(e);
            if (U_DragEnd != null) U_DragEnd(_parent, arg);
            isClicked = false;
        }
        Boolean isClicked = false;
        protected void WindowBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (_parent == null) _parent = this.Parent;

            if (_parent != null && isClicked==false)
            {
                _clickPt.X = e.X + this.Location.X;
                _clickPt.Y = e.Y + this.Location.Y;
                //Point zero = PointToScreen(new Point(Parent.Location.X, Parent.Location.Y));
                //_clickPt = ;
                
                this.MouseMove += _mouseMoveEvent;
                //_moveTimer.Start();
                isClicked = true;

            }
            MouseEventArgs arg = moveWindow(e);
            if (U_DragBegin != null)
            {
                U_DragBegin(_parent, arg);
            }
        }


        protected void WindowBar_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs arg = moveWindow(e);
            if (U_Moving != null)
            {
                U_Moving(_parent, arg);
            }
        }
    }
}
