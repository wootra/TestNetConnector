using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace FormAdders
{
    public class SelectionInfo
    {
        
        public SelectionInfo(){
            SetSelectionStyle(SelectionStyles.FillBack|SelectionStyles.Rectangle, Color.RosyBrown, Color.LightBlue, 1);
        }

        SelectionStyles _selectionStyle;
        public SelectionStyles SelectionStyle
        {
            get { return _selectionStyle; }
            set
            {
                _selectionStyle = value;
                if (value == SelectionStyles.UnderLine && LineColor == null) LineColor = Color.AliceBlue;
                else if(value == SelectionStyles.FillBack && FillColor == null) FillColor = Color.AliceBlue;
            }
        }

        public void SetSelectionStyle(SelectionStyles style, Color lineColor, Color fillColor, int lineWidth=1){
            LineColor = lineColor;
            FillColor = fillColor;
            _selectionStyle = style;
        }

        public Color LineColor
        {
            get;
            set;
        }
        public Color FillColor
        {
            get;
            set;
        }
        public int LineWidth
        {
            get;
            set;
        }

        public static SelectionInfo DefaultStyle
        {
            get
            {
                return new SelectionInfo();
            }
        }

        public void DrawSelection(Graphics g, Rectangle SelectionArea){
            
            if ((int)(SelectionStyle & SelectionStyles.FillBack) >0)
            {
                g.FillRectangle(new SolidBrush(FillColor), SelectionArea);
            }

            if ((int)(SelectionStyle & SelectionStyles.Rectangle) > 0)
            {
                g.DrawRectangle(new Pen(LineColor, LineWidth), SelectionArea);
            }

            if ((int)(SelectionStyle & SelectionStyles.UnderLine) > 0)
            {
                g.DrawLine(new Pen(LineColor, LineWidth), SelectionArea.X, SelectionArea.Y + SelectionArea.Height - 1, SelectionArea.X + SelectionArea.Width - 1, SelectionArea.Y + SelectionArea.Height - 1);
            }
        }

        public void DrawSelection(Graphics g, Rectangle SelectionArea, Color otherColor)
        {

            if (SelectionStyle == SelectionStyles.UnderLine)
            {
                g.DrawLine(new Pen(otherColor), SelectionArea.X, SelectionArea.Y + SelectionArea.Height - 1, SelectionArea.X + SelectionArea.Width - 1, SelectionArea.Y + SelectionArea.Height - 1);
            }
            else if (SelectionStyle == SelectionStyles.FillBack)
            {
                g.FillRectangle(new SolidBrush(otherColor), SelectionArea);
            }
        }
    }
    public enum SelectionStyles { UnderLine = 1, FillBack=2, Rectangle=4 }

}
