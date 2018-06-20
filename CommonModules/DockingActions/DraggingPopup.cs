using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UCoreComponents;


namespace DockingActions
{
    public class DraggingPopup:PopupForm
    {
        public static int windowId=10000;
        DockingRoot _root;
        DockingContainer _parent = null;
        MouseEventHandler _draggingEvent;
        internal DraggingPopup(DockingRoot root, DockingContainer parent, int no = -1, String name = "DockingForm")
            : base((no < 0) ? ++(DraggingPopup.windowId) : no, name)
        {
            InitializeComponent();
            _root = root;
            if (_parent != null) _parent = parent;
            _draggingEvent = new MouseEventHandler(DraggingPopup_MouseMove);
        }
        internal void startDragging()
        {
            this.MouseMove += _draggingEvent;
        }

        internal void stopDragging()
        {
            this.MouseMove -= _draggingEvent;
        }

        internal new void Show()
        {
            base.Show();
            //startDragging();
        }

        internal new void Hide()
        {
            base.Hide();

            //stopDragging();
        }

        enum ResizeCursorPos { Left = 0, Right, Top, Bottom, None };
        ResizeCursorPos _resizeCursorPos = ResizeCursorPos.Left;
        void DraggingPopup_MouseMove(object sender, MouseEventArgs e)
        {
            int range = 10;
            if (e.X < range && e.X > 0) _resizeCursorPos = ResizeCursorPos.Left;
            else if (e.X > this.Width - range && e.X < this.Width) _resizeCursorPos = ResizeCursorPos.Right;
            else if (e.Y > 0 && e.Y < range) _resizeCursorPos = ResizeCursorPos.Top;
            else if (e.Y > this.Height - range && e.Y < this.Height) _resizeCursorPos = ResizeCursorPos.Bottom;
            else _resizeCursorPos = ResizeCursorPos.None;

            switch (_resizeCursorPos)
            {
                case ResizeCursorPos.Left:
                case ResizeCursorPos.Right:
                    Cursor = Cursors.SizeWE;
                    break;
                case ResizeCursorPos.Top:
                case ResizeCursorPos.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;
                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        public Boolean setContent(DockingContainer c=null)
        {
            if (_parent == c) return false;
            //this.SuspendLayout();
            this.Padding = new Padding(5);
            if (c != null) _parent = c;
            this.Controls.Add(_parent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.ResumeLayout();
            return true;
        }
        public Boolean removeContent()
        {
            try
            {
                this.Controls.Remove(_parent);
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected override void OnMove(EventArgs e)
        {
            //_root.Dragging(_parent, Control.MousePosition.X, Control.MousePosition.Y);
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DraggingPopup
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(420, 530);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DraggingPopup";
            this.Opacity = 0.7D;
            this.ResumeLayout(false);

        }


    }
}
