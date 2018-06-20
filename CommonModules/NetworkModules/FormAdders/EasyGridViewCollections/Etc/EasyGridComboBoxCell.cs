using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridComboBoxCell : DataGridViewComboBoxCell, IEasyGridCell
    {
        DataGridView _parent;
        public EasyGridComboBoxCell(DataGridView parent)
            : base()
        {
            _parent = parent;
        }
        bool _enabled = true;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                RePaint();
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.ComboBox; }
        }


        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
            Rectangle halfUp = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height/2);
            Rectangle halfBottom = new Rectangle(cellBounds.X, cellBounds.Y+ rect.Height/2-1, cellBounds.Width - 1, cellBounds.Height/2+1);
            if (this.Selected)
            {
                SizeF textSize = TextSize("Tj",g);
                g.FillRectangle(new SolidBrush(_parent.GridColor), cellBounds);
                g.FillRectangle(new SolidBrush(SystemColors.ActiveCaption), rect);
                float y = (cellBounds.Height-textSize.Height)/2 + cellBounds.Y;
                g.DrawString(this.Value as String, SystemFonts.DefaultFont, new SolidBrush(SystemColors.HighlightText), new PointF(cellBounds.X + 3.0f, y));
            }
            else
            {
                SizeF textSize = TextSize("Tj", g);
                g.FillRectangle(new SolidBrush(_parent.GridColor), cellBounds);
                g.FillRectangle(new SolidBrush(Color.White), halfUp);
                g.FillRectangle(new SolidBrush(SystemColors.Control), halfBottom);
                
                
                float y = (cellBounds.Height - textSize.Height) / 2 + cellBounds.Y;
                g.DrawString(this.Value as String, SystemFonts.DefaultFont, new SolidBrush(Color.Black), new PointF(cellBounds.X+3.0f, y));
            }
            g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                new PointF[]{
                    new PointF(cellBounds.Right-10, cellBounds.Top+4),
                    new PointF(cellBounds.Right-10, cellBounds.Bottom -4),
                    new PointF(cellBounds.Right-3, (cellBounds.Top + cellBounds.Bottom)/2)},
                new byte[]{
                    (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));

                        

        }
        private SizeF TextSize(String text, Graphics g)
        {
            // Set the return value to the normal node bounds.

            if (text != null)
            {
                return g.MeasureString(text, SystemFonts.DefaultFont);
            }
            return new SizeF(0, 0);

        }
    }
}
