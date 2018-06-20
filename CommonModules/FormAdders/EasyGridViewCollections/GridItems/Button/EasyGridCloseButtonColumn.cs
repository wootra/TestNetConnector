using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCloseButtonColumn : DataGridViewColumn, IEasyGridColumn
    {
         EasyGridViewParent _parent;
         public EasyGridCloseButtonColumn(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridCloseButtonCell(parent);
            _columnSpan = new ColumnSpan(this);
            _dataGridView = parent;
        }

         EasyGridViewParent _dataGridView;
         public EasyGridViewParent DataGridView
         {
             get { return _dataGridView; }
         }
         ColumnSpan _columnSpan;
         public ColumnSpan Span { get { return _columnSpan; } }

         EasyGridHeaderCell _headerCell;
         public new EasyGridHeaderCell HeaderCell
         {
             get { return _headerCell; }
         }


         CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
         public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.ClipBounds, this.Selected);

            //SizeF textSize = TextSize("Tj", g);
            Graphics g = e.Graphics;

            Point pt = Control.MousePosition;
            pt = _parent.PointToClient(pt);
            Brush textColor = (this.Selected) ?
                new SolidBrush(this.DefaultCellStyle.SelectionForeColor) :
                new SolidBrush(this.DefaultCellStyle.ForeColor);

            e.PaintContent(e.ClipBounds);
            /*
            Point pt = Control.MousePosition;
            pt = _parent.PointToClient(pt);
            Brush textColor;

            if (e.CellBounds.Contains(pt)) textColor =CellFunctions.DrawHeaderBack(e.CellBounds, g, _parent.GridColor, true);
            else textColor= CellFunctions.DrawHeaderBack(e.CellBounds, g, Color.WhiteSmoke, this.Selected);
            
            //Brush textColor = CellFunctions.DrawHeaderBack(e.CellBounds,g, _parent.GridColor, this.Selected);
            pt = CellFunctions.TextCenterInRact(e.CellBounds, g, _parent.Font, this.Text);
            g.DrawString(this.Text as String, _parent.Font, textColor, pt.X, pt.Y);
             */
            /*
            g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                new PointF[]{
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Top+4),
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Bottom -4),
                    new PointF(e.CellBounds.Right-3, (e.CellBounds.Top + e.CellBounds.Bottom)/2)},
                new byte[]{
                    (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));
             */
            e.Handled = true;
            
        }

        String _text = "";
        public String Text
        {
            get
            {
                return _text;
            }
            set
            {
                try
                {
                    _text = value;
                    
                }
                catch { }
            }
        }


        public ItemTypes ItemType
        {
            get { return ItemTypes.CloseButton; }
        }

        public int Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                if (value == 0) this.Visible = false;
                else this.Visible = true;
            }
        }
    }
}
