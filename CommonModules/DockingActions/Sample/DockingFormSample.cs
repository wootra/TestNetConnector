using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DockingActions.Sample
{
    public struct MyPoint
    {
        public int X;
        public int Y;
    }

    #region DockingForm Class
    /// <summary>
    /// A simple C# DockingForm. This form can only be used for
    /// MDI applications and is able to dock on it's MDI-Parent,
    /// without the use of DockingManagers(MagicLib) or any other 
    /// stuff that makes use uneasy.  
    /// 
    /// This code is based on code by: 
    ///		Phil Wright 10/16/2001 Intermediate, 
    ///		http://www.c-sharpcorner.com/winforms/DockingControl101PW.asp
    ///	The problem with this code, from my perspective, is that you have
    ///	to create an object that you want to dock and add it in the 
    ///	constructor. I think that almost everybody needs a dockingform that's
    ///	simple.	
    /// </summary>
    public class DockingFormSample : Form
    {
        #region Class instances and variables
        public static StatusBar statusBar;
        /// <summary>
        /// The bordersize in which to dock.
        /// </summary>
        private const int hotLength = 40;

        /// <summary>
        /// MsgID of WM_EXITSIZEMOVE
        /// </summary>
        private const int WM_EXITSIZEMOVE = 0x232;
        /// <summary>
        /// MsgID of WM_NCMOUSEMOVE
        /// </summary>
        private const int WM_NCMOUSEMOVE = 0xa0;
        /// <summary>
        /// MsgID of WM_NCLMOUSEDOWN
        /// </summary>
        private const int WM_NCLMOUSEDOWN = 0xa1;
        /// <summary>
        /// MsgID of WM_MOVING
        /// </summary>
        private const int WM_MOVING = 0x216;
        /// <summary>
        /// MsgID of WM_WINDOWPOSCHANGED
        /// </summary>
        private const int WM_WINDOWPOSCHANGED = 0x47;

        private const int SC_MOVE = 0xf012;

        private const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Determine wether we're dragging or sizing
        /// </summary>
        private bool inSizing = false;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// The resize bar showed when the form is in a docked state.
        /// </summary>
        private DockingResize resize;
        /// <summary>
        /// The size of the form before the DockStyle changes from None to something else,
        /// used to resize the form when DockStyle changes to None.
        /// </summary>
        private Size formSizeBeforeDock;

        private const int defaultSizeX = 150;

        private const int defaultSizeY = 450;

        private Size defaultSize = new Size(defaultSizeX, defaultSizeY);

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor, pass along a reference to the form we're docking against.
        /// This form is set as the MdiParent of this DockingForm instance.
        /// </summary>
        /// <param name="mdiParentForm">The Form that should be the MdiParent.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the MdiParent is unspecified.</exception>
        public DockingFormSample(Form mdiParentForm)
        {
            // Verify paremeters
            if (mdiParentForm == null)
                throw new ArgumentNullException("mdiParentForm");
            if (mdiParentForm.IsMdiContainer == false)
                throw new ArgumentException("Form should be MdiContainer.", "mdiParentForm");
            // Call the init routine from formdesigner
            InitializeComponent();
            // Create the resizebar.
            resize = new DockingResize(this);
            Controls.AddRange(new Control[] { resize });
            // Set class variables
            this.MdiParent = mdiParentForm;
            this.formSizeBeforeDock = this.Size = defaultSize;
        }
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            // 
            // DockingForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DockingForm";
            this.ShowInTaskbar = false;

        }
        #endregion

        #region Overridden methods ( WndProc, OnSizeChanged, Dock, Dispose)
        /// <summary>
        /// This method is overridden to be able to capture the event raised upon
        /// dropping this form. Both dropping and resizing raise the same event. To desern
        /// between them the boolean switch 'inSizing' is available. The overridden 
        /// 'OnSizeChanged' event is used to switch between a drop action and a size action.
        /// 
        /// When the form is dropped, the new screenlocation of the mouse is accessed. When the 
        /// form is dropped on a fixed size (hotLength) from the border, the form is docked to that
        /// border.
        /// </summary>
        /// <param name="m">The Window Message sent by Windows.</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (WM_EXITSIZEMOVE):
                    // Are we dropping or done with resizing??
                    if (this.inSizing)
                    {
                        // Check if DockStyle = DockStyle.None, if so
                        // remember the currentsize.
                        if (Dock == DockStyle.None)
                            formSizeBeforeDock = this.Size;
                        // Convert mouseposition to position on hostForm.
                        Point screenPoint = Cursor.Position;
                        Point parentPoint = MdiParent.PointToClient(screenPoint);

                        // Find the client rectangle of the form
                        Size parentSize = MdiParent.ClientRectangle.Size;

                        // Find new docking position
                        if (parentPoint.X < hotLength)
                            Dock = DockStyle.Left;
                        else if (parentPoint.Y < hotLength)
                            Dock = DockStyle.Top;
                        else if (parentPoint.X >= (parentSize.Width - hotLength))
                            Dock = DockStyle.Right;
                        else if (parentPoint.Y >= (parentSize.Height - hotLength))
                            Dock = DockStyle.Bottom;
                        else
                        {
                            Dock = DockStyle.None;
                            //this.Location = new Point(Cursor.Position.X - nonclientClickLocation.X,Cursor.Position.Y - nonclientClickLocation.Y);

                        }
                        ResizeForm();
                    }
                    else
                        this.inSizing = false;
                    break;
                case (WM_SYSCOMMAND):
                    if (m.WParam.ToInt32() == SC_MOVE)
                    {
                        this.Size = defaultSize;
                    }
                    break;
                /*case(WM_NCMOUSEMOVE):
                    Point pt = new Point( (int)m.LParam );
                    if (this.Capture)
                        WriteToStatusbar(pt.ToString());
                    break;
                case(WM_MOVING):
                    if (this.Capture)
                        this.Size = formSizeBeforeDock;
                    break;
                case(WM_NCLMOUSEDOWN):
                    Point mouseLocation = new Point( (int)m.LParam );
                    Point formLocation = this.Location;
                    nonclientClickLocation = new Point(mouseLocation.X - formLocation.X, mouseLocation.Y - formLocation.Y);
                    break;*/
                default:
                    Console.WriteLine(m.ToString());
                    break;
            };
            base.WndProc(ref m);
        }



        /// <summary>
        /// Changes the size of the form according to the following schema:
        /// Dock == Left | Right -> Width = ParentWidth / 3
        /// Dock == Top | Bottom -> Height = ParentHeight / 3
        /// Dock == None -> Size = formSizeBeforeDock
        /// </summary>
        private void ResizeForm()
        {
            Size parentSize = MdiParent.ClientRectangle.Size;
            if ((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
                this.Size = new Size(parentSize.Width / 3, this.Size.Height);
            else if ((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
                this.Size = new Size(this.Size.Width, parentSize.Height / 3);
            else
                this.Size = formSizeBeforeDock;

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Capture)
                WriteToStatusbar(e.X + "," + e.Y);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.inSizing = true;
            resize.Resize();
        }

        public override DockStyle Dock
        {
            set
            {
                resize.Dock = value;
                base.Dock = value;
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region DebugMethods
        private void WriteToStatusbar(string message)
        {
            if (DockingFormSample.statusBar != null)
                DockingFormSample.statusBar.Text = message;
        }
        #endregion

        public static void Main()
        {

            Form f = new Form();
            f.IsMdiContainer = true;
            StatusBar sb = new StatusBar();
            sb.Parent = f;
            DockingFormSample.statusBar = sb;

            f.Size = new Size(600, 600);
            DockingFormSample df = new DockingFormSample(f);
            df.Show();
            Application.Run(f);
        }
    }
    #endregion

    #region DockingRezize Class
    // A bar used to resize the parent DockingControl
    class DockingResize : UserControl
    {
        private DockingFormSample parent;
        private FormBorderStyle parentBorderStyle;

        // Class constants
        private const int _fixedLength = 4;

        // Instance variables
        private Point sizeStart;
        private Point sizeEnd;
        private Size parentSize;

        public DockingResize(DockingFormSample df)
        {
            if (df == null)
                throw new ArgumentNullException("df");
            this.parent = df;
            this.parentBorderStyle = df.FormBorderStyle;
            this.Dock = df.Dock;
        }

        public override DockStyle Dock
        {
            set
            {
                if (value != DockStyle.None)
                {
                    Visible = true;
                    RestyleBorder();
                }
                if (value == DockStyle.Bottom)
                    value = DockStyle.Top;
                else if (value == DockStyle.Top)
                    value = DockStyle.Bottom;
                else if (value == DockStyle.Left)
                    value = DockStyle.Right;
                else if (value == DockStyle.Right)
                    value = DockStyle.Left;
                else
                {
                    Visible = false;
                    parent.FormBorderStyle = parentBorderStyle;
                }
                base.Dock = value;

            }
        }

        public void RestyleBorder()
        {
            if (parent.FormBorderStyle == FormBorderStyle.SizableToolWindow)
                parent.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            else if (parent.FormBorderStyle == FormBorderStyle.Sizable)
                parent.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        public new void Resize()
        {
            Size parentSize = parent.Size;
            if ((Dock == DockStyle.Top) || (Dock == DockStyle.Bottom))
            {
                Size = new Size(parentSize.Width, 5);
            }
            else if ((Dock == DockStyle.Left) || (Dock == DockStyle.Right))
            {
                Size = new Size(5, parentSize.Height);
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Remember the mouse position and client size when capture occured
            sizeStart = sizeEnd = PointToScreen(new Point(e.X, e.Y));
            parentSize = Parent.ClientSize;

            // Ensure delegates are called
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Used for resizing the docked control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Cursor depends on if we a vertical or horizontal resize
            if ((this.Dock == DockStyle.Top) ||
                (this.Dock == DockStyle.Bottom))
                this.Cursor = Cursors.HSplit;
            else
                this.Cursor = Cursors.VSplit;

            // Can only resize if we have captured the mouse
            if (this.Capture)
            {
                // Find the new mouse position
                Point point = PointToScreen(new Point(e.X, e.Y));

                // Have we actually moved the mouse?
                if (point != sizeEnd)
                {
                    // Update the last processed mouse position
                    sizeEnd = point;

                    // Find delta from original position
                    int xDelta = sizeEnd.X - sizeStart.X;
                    int yDelta = sizeEnd.Y - sizeStart.Y;

                    // Resizing from bottom or right of form means inverse movements
                    if ((this.Dock == DockStyle.Top) ||
                        (this.Dock == DockStyle.Left))
                    {
                        xDelta = -xDelta;
                        yDelta = -yDelta;
                    }

                    // New size is original size plus delta
                    if ((this.Dock == DockStyle.Top) ||
                        (this.Dock == DockStyle.Bottom))
                        Parent.ClientSize = new Size(parentSize.Width, parentSize.Height + yDelta);
                    else
                        Parent.ClientSize = new Size(parentSize.Width + xDelta, parentSize.Height);

                    // Force a repaint of parent so we can see changed appearance
                    Parent.Refresh();
                }
            }

            // Ensure delegates are called
            base.OnMouseMove(e);
        }

        // Static variables defining colors for drawing
        //private static Pen _lightPen = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));
        private static Pen _lightPen = new Pen(Color.FromKnownColor(KnownColor.Azure));
        //		private static Pen _darkPen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
        private static Pen _darkPen = new Pen(Color.FromKnownColor(KnownColor.Blue));
        private static Brush _plainBrush = Brushes.Blue
            ;

        // Static properties for read-only access to drawing colors
        public static Pen LightPen { get { return _lightPen; } }
        public static Pen DarkPen { get { return _darkPen; } }
        public static Brush PlainBrush { get { return _plainBrush; } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Create objects used for drawing
            Point[] ptLight = new Point[2];
            Point[] ptDark = new Point[2];
            Rectangle rectMiddle = new Rectangle();

            // Drawing is relative to client area
            Size sizeClient = parent.ClientSize;

            // Painting depends on orientation
            if ((this.Dock == DockStyle.Top) ||
                (this.Dock == DockStyle.Bottom))
            {
                // Draw as a horizontal bar
                ptDark[1].Y = ptDark[0].Y = sizeClient.Height - 1;
                ptLight[1].X = ptDark[1].X = sizeClient.Width;
                rectMiddle.Width = sizeClient.Width;
                rectMiddle.Height = sizeClient.Height - 2;
                rectMiddle.X = 0;
                rectMiddle.Y = 1;
            }
            else if ((this.Dock == DockStyle.Left) ||
                (this.Dock == DockStyle.Right))
            {
                // Draw as a vertical bar
                ptDark[1].X = ptDark[0].X = sizeClient.Width - 1;
                ptLight[1].Y = ptDark[1].Y = sizeClient.Height;
                rectMiddle.Width = sizeClient.Width - 2;
                rectMiddle.Height = sizeClient.Height;
                rectMiddle.X = 1;
                rectMiddle.Y = 0;
            }

            // Use colors defined by docking control that is using us
            pe.Graphics.DrawLine(LightPen, ptLight[0], ptLight[1]);
            pe.Graphics.DrawLine(DarkPen, ptDark[0], ptDark[1]);
            pe.Graphics.FillRectangle(PlainBrush, rectMiddle);

            // Ensure delegates are called
            base.OnPaint(pe);
        }
    }
    #endregion
}
