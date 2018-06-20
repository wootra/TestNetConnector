using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FormAdders.EasyGridViewCollections;
namespace FormAdders
{
    public class CellFunctions
    {
        public static SizeF TextSize(String text, Graphics g, Font font)
        {
            // Set the return value to the normal node bounds.

            if (text != null)
            {
                return g.MeasureString(text, font);
            }
            return new SizeF(0, 0);

        }

        public static bool MouseHitTest(Control baseControl, Rectangle cellBounds)
        {
            Point pt = Control.MousePosition;
            //pt = baseControl.FindForm().PointToClient(pt);//.PointToScreen
            //pt = baseControl.FindForm().PointToScreen(pt);
            Rectangle rect = baseControl.RectangleToScreen(cellBounds);
            //Point newPt =  baseControl.tosc.PointToClient(pt);
            if (rect.Contains(pt)) return true;
            //if (cellBounds.Contains(newPt)) return true;
            else return false;
           
        }

        public static int TextCenterYInRact(Rectangle cellBounds, float textHeight)
        {
            return cellBounds.Y + (int)((cellBounds.Height - textHeight) / 2.0);
        }

        public static int TextCenterYInRact(Rectangle cellBounds, Graphics g, Font font)
        {
            float textHeight = TextSize("Tj", g, font).Height;
            
            return cellBounds.Y + (int)((((cellBounds.Height - textHeight) / 2.0))+1);
        }


        public static Point TextCenterInRact(Rectangle cellBounds, Graphics g, Font font, String text)
        {
            SizeF size = TextSize(text, g, font);
            int y = cellBounds.Y + (int)((cellBounds.Height - size.Height) / 2.0)+1;
            int x = cellBounds.X + (int)((cellBounds.Width - size.Width) / 2.0)+1;
            if (x < cellBounds.X) x = cellBounds.X;
            return new Point(x,y);
        }


        public static Point RectCenterInRact(Rectangle cellBounds, Graphics g, Size size)
        {
            int y = cellBounds.Y + (int)((cellBounds.Height - size.Height) / 2.0);
            int x = cellBounds.X + (int)((cellBounds.Width - size.Width) / 2.0);
            return new Point(x, y);
        }

        public static int CenterY(int cellHeight, int cellY, Graphics g, int itemHeight)
        {
            int y = cellY + (int)((cellHeight - itemHeight) / 2.0);
            return y;
        }

        public static int CenterTextY(int cellHeight, int cellY, Graphics g, Font font)
        {
            SizeF size = TextSize("Tj", g, font);
            
            int y = cellY + (int)((cellHeight - size.Height) / 2.0);
            return y;
        }

        public static Brush DrawLensBack(DataGridViewCell cell, Rectangle cellBounds, Graphics g, Color GridColor, bool Selected, bool Enabled)
        {
            Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y , cellBounds.Width, cellBounds.Height+1);
            int yHalf = rect.Height / 2;
            Rectangle halfUp = new Rectangle(rect.X, rect.Y, rect.Width, yHalf + 1);
            Rectangle halfBottom = new Rectangle(rect.X, rect.Bottom - yHalf, rect.Width, yHalf);
            
            Brush textColor;
            if (Enabled)
            {
                if (Selected)
                {

                    g.FillRectangle(new SolidBrush(Color.FromArgb(227, 247, 255)), halfUp);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), halfBottom);

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                    textColor = new SolidBrush(Color.Black);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.White), halfUp);
                    g.FillRectangle(new SolidBrush(SystemColors.Control), halfBottom);
                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                    textColor = Brushes.Black;
                }
            }
            else
            {
                if (Selected)
                {

                    g.FillRectangle(new SolidBrush(Color.FromArgb(227, 247, 255)), halfUp);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), halfBottom);

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                    textColor = new SolidBrush(Color.Black);
                }
                else
                {
                    if(cell!=null) DrawDisabledBack(cell, rect, g, BackColor(cell));
                    
                    //g.FillRectangle(new SolidBrush(Color.WhiteSmoke), rect);
                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                    textColor = Brushes.Gray;
                }
                
            }

            int y = rect.Y;// +cellBounds.Height - 1;
            //int y = cellBounds.Y;// +cellBounds.Height;
            int x = rect.X + rect.Width - 1;
            //g.DrawLine(new Pen(Color.FromArgb(213, 213, 213)), rect.X, y, rect.X + rect.Width, y);//가로줄
            //g.DrawLine(new Pen(Color.FromArgb(222, 222, 233)), x, rect.Y + rect.Height, x, rect.Y);//세로줄
           
           // g.DrawLine(new Pen(Color.FromArgb(220,220,250), 0.1f), cellBounds.X, y, cellBounds.X + cellBounds.Width, y);
            
            return textColor;
        }
        public static Brush DrawHeaderBack(Rectangle cellBounds, Graphics g, Color GridColor, bool Selected)
        {
            Rectangle rect = new Rectangle(cellBounds.X-1, cellBounds.Y-1, cellBounds.Width+2 , cellBounds.Height+2 );
            Rectangle halfUp = new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height / 2);
            Rectangle halfBottom = new Rectangle(rect.X, rect.Y + rect.Height / 2 - 1, rect.Width, rect.Height / 2 + 2);
            Brush textColor;
            if (Selected)
            {

                g.FillRectangle(new SolidBrush(Color.FromArgb(227,247,255)), halfUp);
                g.FillRectangle(new SolidBrush(Color.FromArgb(188,236,254)), halfBottom);

                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                
                textColor = new SolidBrush(Color.Black);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.White), halfUp);
                g.FillRectangle(new SolidBrush(Color.FromArgb(245,245,245)), halfBottom);
                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                textColor = Brushes.Black;
            }
            //g.DrawLine(new Pen(Color.FromArgb(213,213,213)), rect.X, rect.Y + rect.Height - 3, rect.X + rect.Width, rect.Y + rect.Height - 3);
            g.DrawLine(new Pen(GridColor), rect.X, rect.Y + rect.Height - 3, rect.X + rect.Width, rect.Y + rect.Height - 3);
            int y = rect.Y + rect.Height - 1;
            g.DrawLine(new Pen(Color.FromArgb(150,150,150), 0.1f), rect.X, y, rect.X + rect.Width, y);
            g.DrawLine(new Pen(Color.FromArgb(220, 220, 220), 0.1f), rect.X, y - 2, rect.X + rect.Width, y - 2);
            
            g.DrawLine(new Pen(Color.Gray, 1), rect.X, 1, rect.X + rect.Width, 1);
            return textColor;
        }

        public static Brush DrawRowHeaderBack(Rectangle cellBounds, Graphics g, Color GridColor, bool Selected)
        {
            Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
            Rectangle halfUp = new Rectangle(cellBounds.X, cellBounds.Y + 2, cellBounds.Width, cellBounds.Height / 2);
            Rectangle halfBottom = new Rectangle(cellBounds.X, cellBounds.Y + rect.Height / 2 - 1, cellBounds.Width, cellBounds.Height / 2 + 2);
            Brush textColor;
            if (Selected)
            {

                g.FillRectangle(new SolidBrush(Color.FromArgb(227, 247, 255)), halfUp);
                g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), halfBottom);

                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);

                textColor = new SolidBrush(Color.Black);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.White), halfUp);
                g.FillRectangle(new SolidBrush(SystemColors.Control), halfBottom);
                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                textColor = Brushes.Black;
            }
            //g.DrawLine(new Pen(Color.FromArgb(213,213,213)), cellBounds.X, cellBounds.Y + cellBounds.Height - 3, cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height - 3);
            g.DrawLine(new Pen(GridColor), cellBounds.X, cellBounds.Y + cellBounds.Height - 3, cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height - 3);
            int y = cellBounds.Y + cellBounds.Height - 1;
            g.DrawLine(new Pen(Color.Black, 0.1f), cellBounds.X, y, cellBounds.X + cellBounds.Width, y);

            return textColor;
        }

        public static Brush DrawPlainHeaderBack(Rectangle cellBounds, Graphics g, Color GridColor, bool Selected)
        {
            Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
            Rectangle halfUp = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height / 2);
            Rectangle halfBottom = new Rectangle(cellBounds.X, cellBounds.Y + rect.Height / 2 - 2, cellBounds.Width, cellBounds.Height / 2 + 3);
            Rectangle BottomIn = new Rectangle(cellBounds.X+2, cellBounds.Y + rect.Height / 4, cellBounds.Width-4, cellBounds.Height * 3/4 -3);
            
            Brush textColor;
            if (Selected)
            {

                g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), rect);
                g.FillRectangle(new SolidBrush(SystemColors.Control), BottomIn);

                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);

                textColor = new SolidBrush(Color.Black);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.WhiteSmoke), rect);
                g.FillRectangle(new SolidBrush(SystemColors.Control), BottomIn);

                //g.FillRectangle(new SolidBrush(Color.White), halfUp);
                //g.FillRectangle(new SolidBrush(SystemColors.Control), halfBottom);
                g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                textColor = Brushes.Black;
            }
            //g.DrawLine(new Pen(Color.FromArgb(213,213,213)), cellBounds.X, cellBounds.Y + cellBounds.Height - 3, cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height - 3);
            g.DrawLine(new Pen(GridColor), cellBounds.X, cellBounds.Y + cellBounds.Height - 3, cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height - 3);
            
            int y = cellBounds.Y + cellBounds.Height - 1;
            //g.DrawLine(new Pen(GridColor, 0.1f), cellBounds.X, y, cellBounds.X + cellBounds.Width, y);
            
            return textColor;
        }

        public static Brush DrawDisabledBack(DataGridViewCell cell, Rectangle cellBounds, Graphics g, Color originalColor){
            Brush textColor;
            if (DisabledBackColorMode == DisabledColorModes.CellDefaultBack)
            {
                g.FillRectangle(Brushes.WhiteSmoke, cellBounds);
            }
            else if (DisabledBackColorMode == DisabledColorModes.CustomBack)
            {
                g.FillRectangle(new SolidBrush(CustomDisabledColor), cellBounds);
            }
            else// if (DisabledBackColorMode == DisabledColorModes.CellDefaultBack)
            {
                g.FillRectangle(new SolidBrush(originalColor), cellBounds);
            }
            DrawDefaultLines(cellBounds, g, Color.FromArgb(222, 222, 233));
                //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                textColor = Brushes.Gray;
            return textColor;
        }
        public static void DrawDefaultLines(Rectangle cellBounds, Graphics g, Color lineColor){
            int y = cellBounds.Y;// +cellBounds.Height;
            int x = cellBounds.X + cellBounds.Width - 1;
            g.DrawLine(new Pen(lineColor), cellBounds.X,y , cellBounds.X + cellBounds.Width, y);//가로줄
            g.DrawLine(new Pen(lineColor), x, cellBounds.Y + cellBounds.Height, x, cellBounds.Y);//세로줄
        }
        public static Brush DrawPlainBack(DataGridViewCell cell, Rectangle cellBounds, Graphics g, Color GridColor, Color BackColor, bool Selected, bool Enabled)
        {
            Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y-1, cellBounds.Width, cellBounds.Height+2);
            //Rectangle halfUp = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height / 2);
            //Rectangle halfBottom = new Rectangle(cellBounds.X, cellBounds.Y + rect.Height / 2 - 2, cellBounds.Width, cellBounds.Height / 2 + 3);
            Brush textColor;
            if (Enabled)
            {
                if (Selected)
                {
                    DrawLensBack(cell, cellBounds, g, GridColor, true, Enabled);
                    //g.FillRectangle(new SolidBrush(Color.FromArgb(227, 247, 255)), halfUp);
                    //g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), halfBottom);

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);

                    
                }
                else
                {
                    g.FillRectangle(new SolidBrush(BackColor), rect);

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                }
                textColor = new SolidBrush(Color.Black);
            }
            else
            {
                if (Selected)
                {
                    textColor = DrawLensBack(cell, cellBounds, g, GridColor, true, Enabled);
                    //g.FillRectangle(new SolidBrush(Color.FromArgb(227, 247, 255)), halfUp);
                    //g.FillRectangle(new SolidBrush(Color.FromArgb(188, 236, 254)), halfBottom);

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);


                }
                else
                {
                    textColor = DrawDisabledBack(cell, cellBounds, g, CellFunctions.BackColor(cell));

                    //g.DrawRectangle(new Pen(new SolidBrush(GridColor), 0.1f), rect);
                }
                
            }
            DrawDefaultLines(cellBounds, g, GridColor);
            return textColor;
        }

        public static Color GroupSelectionColor = Color.FromArgb(222, 222, 255);
        public static Color CustomDisabledColor = Color.WhiteSmoke;
        public enum DisabledColorModes { DefaultDisabledBack, CustomBack, CellDefaultBack };
        public static DisabledColorModes DisabledBackColorMode = DisabledColorModes.CellDefaultBack;

        public static Brush DrawPlainBackground(DataGridViewCell cell, bool _enabled, Rectangle cellBounds, Graphics g, int RowIndex, bool Selected, Color GridColor)
        {
            Brush fontColor;
            
                //base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            fontColor = DrawPlainBack(cell, cellBounds, g, GridColor, BackColor(cell), Selected, _enabled);

            
            /*
            if (_enabled == false)
            {
                DrawDisabledBack(cellBounds, g, BackColor(cell));
                if (Selected) fontColor = Brushes.Black;
                else fontColor = Brushes.Gray;
            }

             else if (Selected)
             {

                 fontColor = DrawLensBack(cell, cellBounds, g, GridColor, true, _enabled);

             }
            else 
            {
                //base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                DrawPlainBack(cell, cellBounds, g, GridColor, BackColor(cell), false,_enabled);
                
                fontColor = Brushes.Black;

            }
            */

            int y = cellBounds.Y ;// +cellBounds.Height - 1;
            //g.DrawLine(new Pen(GridColor, 0.1f), cellBounds.X, y, cellBounds.X + cellBounds.Width, y);
            
            return fontColor;
        }

        public static Color BackColor(DataGridViewCell cell)
        {
            EasyGridRow row = (cell.OwningRow as EasyGridRow);
            RowBackModes rowMode = row.RowBackMode;
            int rowIndex = row.Index;
            if (rowMode == RowBackModes.CellColor)
            {
                
                if (row.CustomColorCells.Contains(cell))
                {
                    return cell.Style.BackColor;
                }
                else
                {
                    if (rowIndex % 2 == 0) return Color.White;
                    else return Color.FromArgb(244, 244, 255);
                }
            }else if (rowMode == RowBackModes.CustomColor)
            {
                return row.RowBackCustomColor;
            }
            else if (rowMode == RowBackModes.Blue)
            {
                return Color.FromArgb(220, 220, 255);
            }
             else if (rowMode == RowBackModes.Red)
             {
                 return Color.FromArgb(255, 220, 220);
             }
             else if (rowMode == RowBackModes.Gray)
             {
                 return Color.FromArgb(220, 220, 220);
             }
             else// if (rowMode == EasyGridRow.RowBackModes.None)
             {
                 if (rowIndex % 2 == 0) return Color.White;
                 else return Color.FromArgb(244, 244, 255);
             }
             
        }
    }
}
