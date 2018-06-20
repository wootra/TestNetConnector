using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DataHandling
{
    public class CoodinateHandling
    {
        public static Point GetScreenPoint(Control c, int offsetX=0, int offsetY=0)
        {
            return c.PointToScreen(new Point(offsetX, offsetY));
        }
        public static Rectangle GetScreenRect(Control c, int offsetX=0, int offsetY=0)
        {
            Rectangle rect = c.RectangleToScreen(new Rectangle(offsetX,offsetY,c.Width, c.Height));
            return rect;
        }
        public static Rectangle GetClientRectInOtherForm(Control c, Form otherForm, int offsetX=0, int offsetY=0)
        {
            Rectangle rect = c.RectangleToScreen(new Rectangle(offsetX, offsetY, c.Width, c.Height));
            Point client = otherForm.PointToClient(new Point(0, 0));
            rect.X += client.X;
            rect.Y += client.Y;
            return rect;
        }
        public static Point GetWindowPointFromControlOffset(Form form, Control baseControl, int offsetX=0, int offsetY=0){
            Point client = form.PointToClient(new Point(0, 0));
            Point window = form.Bounds.Location;
            Point diff = new Point(-(window.X + client.X), -(window.Y + client.Y));
            Point control = FromClientToClientPoint(baseControl, form,offsetX, offsetY);
            return new Point(control.X + diff.X, control.Y + diff.Y);
        }
        

        public static Point GetFormPositionIfControlInForm(Control c, Form form, int offsetX=0, int offsetY=0)
        {
            Point client = form.PointToClient(new Point(0, 0));
            Point window = form.Bounds.Location;
            Point diff = new Point(-(window.X + client.X), -(window.Y + client.Y));
            Point control = c.PointToScreen(new Point(offsetX, offsetY));
            Point ctl = new Point(control.X, control.Y);
            ctl.X -= diff.X;
            ctl.Y -= diff.Y;
            return ctl;
        }

        public static Rectangle GetFormBoundWithControlsBound(Control c, Form form, int offsetX = 0, int offsetY = 0)
        {
            
            Point client = form.PointToClient(new Point(0, 0));
            Point window = form.Bounds.Location;
            Point diff = new Point(-(window.X + client.X), -(window.Y + client.Y));
            Point control = c.PointToScreen(new Point(offsetX, offsetY));
            Point ctl = new Point(control.X, control.Y);
            ctl.X -= diff.X;
            ctl.Y -= diff.Y;
            return new Rectangle(ctl.X, ctl.Y, c.Width, c.Height);
        }

        public static Boolean isCrossed(Control c1, Control c2)
        {
            Rectangle avr = new Rectangle();
            avr.Width = (c1.Width + c2.Width) / 2;
            avr.Height = (c1.Height + c2.Height) / 2;
            int xDiff = Math.Abs(avr.Width - c1.Width);
            int yDiff = Math.Abs(avr.Height - c1.Height);
            
            
            Rectangle r1 = c1.RectangleToScreen(avr);
            Rectangle r2 = c2.RectangleToScreen(avr);
            if (c1.Width > c2.Width) r1.X += xDiff;
            else r2.X += xDiff;
            if (c1.Height > c2.Height) r1.Y += yDiff;
            else r2.Y += yDiff;


            return r1.IntersectsWith(r2);
        }
        public static Boolean isEntered(Control c, int offsetX=0, int offsetY=0)
        {
            Rectangle rect = c.RectangleToScreen(new Rectangle(0,0,c.Width,c.Height));
            Point pt = new Point(Control.MousePosition.X - offsetX, Control.MousePosition.Y - offsetY);
            if (pt.X >= rect.X && pt.X <= rect.X + rect.Width && pt.Y >= rect.Y && pt.Y <= rect.Y + rect.Height) return true;
            else return false;
        }
        public static Rectangle FromClientToClient(Control fromControl, Control destClient, int offsetX = 0, int offsetY = 0)
        {
            Rectangle fromRect = GetScreenRect(fromControl, offsetX, offsetY);
            return destClient.RectangleToClient(fromRect);
        }
        public static Point FromClientToClientPoint(Control fromControl, Control destClient, int offsetX = 0, int offsetY = 0)
        {
            Point fromPoint = GetScreenPoint(fromControl, offsetX, offsetY);
            return destClient.PointToClient(fromPoint);
        }

        public static Point FromScreenToClient(int fromX, int fromY, Control destClient)
        {
            return destClient.PointToClient(new Point(fromX, fromY));
        }
        public static Point getCenter(Rectangle area, int innerObjectWidth=0, int innerObjectHeight=0)
        {
                return new Point(area.X + (area.Width-innerObjectWidth) / 2, area.Y + (area.Height - innerObjectHeight) / 2);
        }
        public static Rectangle RectPosDifference(Rectangle fromObj, Rectangle toObj)
        {
            return new Rectangle(toObj.X - fromObj.X, toObj.Y - fromObj.Y, toObj.Width-fromObj.Width, toObj.Height-fromObj.Height);
        }


    }
}
