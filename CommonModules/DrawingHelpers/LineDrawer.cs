using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DrawingHelpers
{
    public class LineDrawer
    {
        public LineDrawer()
        {

        }
        public void DrawRoundRect(Graphics g, Rectangle drawRect, Padding padding, Size round, Pen pen)
        {

            //Pen pen = new Pen(Brushes.Brown, 2);
            //pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            //pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            int left = padding.Left;
            int right = padding.Right;
            int top = padding.Top;
            int bottom = padding.Bottom;
            int roundW = round.Width * 2;
            int roundH = round.Height * 2;
            int roundX = round.Width;
            int roundY = round.Height;
            
            g.DrawArc(pen, new Rectangle(left, top, roundW, roundH), 180, 90);//lefttoparc

            g.DrawArc(pen, new Rectangle(drawRect.Width - (right + roundX), top, roundW, roundH), -90, 90);//righttop arc
            g.DrawArc(pen, new Rectangle(drawRect.Width - (right + roundX), drawRect.Height - 26, roundW, roundH), 0, 90);//right bottom
            g.DrawArc(pen, new Rectangle(left, drawRect.Height - (bottom + roundY), roundW, roundH), 90, 90);//left bottom arc
            

            //g.DrawLine(pen, 16, 6, drawRect.Width - 16, 6);
            //g.DrawLine(pen, (left+roundX), top, 25, top);//top line (left->right)
            //g.DrawLine(pen, 120, 6, drawRect.Width - 16, top);// top
            
            g.DrawLine(pen, (left + roundX), top, (drawRect.Width - roundX), top);//top line (left->right)
            g.DrawLine(pen, (left + roundX), drawRect.Height - top, (drawRect.Width - roundX), drawRect.Height - top);//bottomline
            g.DrawLine(pen, left, (top+roundY), left, drawRect.Height - (bottom+roundY));//left line
            g.DrawLine(pen, drawRect.Width - right, top+roundY, drawRect.Width - right, drawRect.Height - (bottom + roundY));//rightline
            
        }

        public Pen getRoundCapPen(Color penColor, int thickness)
        {
            Pen pen = new Pen(new SolidBrush(penColor), thickness);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            return pen;
        }

        public static Pen getRoundCapPen(Brush penColor, int thickness)
        {
            Pen pen = new Pen(penColor, thickness);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            return pen;
        }
    }
}
