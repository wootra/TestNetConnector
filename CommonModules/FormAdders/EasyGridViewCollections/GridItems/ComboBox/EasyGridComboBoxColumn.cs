using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; 
using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridComboBoxColumn:DataGridViewComboBoxColumn, IEasyGridColumn
    {
        EasyGridViewParent _parent;
        public EasyGridComboBoxColumn(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _initItems = new List<String>();
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridComboBoxCell(parent);
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

        ICollection<String> _initItems;

        public new ICollection<String> Items
        {
            get
            {
                return _initItems;
            }
            set
            {
                _initItems = value;
                if (_selectedIndex < 0 && _initItems.Count>0) _selectedIndex = 0;
            }
        }

        int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value>=0 && value < _initItems.Count) _selectedIndex = value;
                else if (_initItems.Count > 0) _selectedIndex = 0;
                else _selectedIndex = -1;
            }
        }

        public String SelectedItem
        {
            get
            {
                if (_initItems.Count > 0 && _selectedIndex >= 0) return _initItems.ElementAt(_selectedIndex);
                else if (_initItems.Count > 0) return _initItems.ElementAt(0);
                else return null;
            }
        }


        public ItemTypes ItemType
        {
            get { return ItemTypes.ComboBox; }
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
            Graphics g = e.Graphics;

            Point pt = Control.MousePosition;
            pt = this.DataGridView.PointToClient(pt);
            Brush textColor;

            if (e.CellBounds.Contains(pt)) textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, this.DataGridView.GridColor, true);
            else textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, Color.WhiteSmoke, this.Selected);

            //Brush textColor = CellFunctions.DrawHeaderBack(e.CellBounds,g, _parent.GridColor, this.Selected);
            pt = CellFunctions.TextCenterInRact(e.CellBounds, g, this.DataGridView.Font, this.HeaderText);
            g.DrawString(this.HeaderText as String, this.DataGridView.Font, textColor, pt.X, pt.Y + 2);
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

    }
}
