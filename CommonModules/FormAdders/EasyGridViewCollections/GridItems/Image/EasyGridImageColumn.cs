using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageColumn:DataGridViewImageColumn, IEasyGridColumn
    {
        EasyGridViewParent _parent;

        public EasyGridImageColumn(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridImageCell(parent);
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



        public EasyGridImageColumn(EasyGridViewParent parent, bool valuesAreIcon)
            : base(valuesAreIcon)
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridImageCell(parent);
            _dataGridView = parent;
        }


        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }


        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }


        ICollection<Image> _images;
        int _selectedIndex = 0;

        public ICollection<Image> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
            }
        }

        public new Image Image
        {
            get
            {
                if (_selectedIndex >= 0 && Images.Count > _selectedIndex) return null;
                return base.Image;// null;// Images.ElementAt(_selectedIndex);
            }
            set
            {
                base.Image = value;
                _selectedIndex = -1;
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value >= 0 && Images.Count > value)
                {
                    base.Image = Images.ElementAt(value);
                    _selectedIndex = value;
                }
                else
                {
                    //base.Image = Images.ElementAt(0);
                    _selectedIndex = -1;
                }
                RePaint();
            }
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            
            Graphics g = e.Graphics;
            Image img = null;
            if(Images!=null && Images.Count>_selectedIndex && _selectedIndex>=0){
                img = this.Images.ElementAt(_selectedIndex);
            }
            
            Point pt = Control.MousePosition;
            DataGridView _parent = base.DataGridView;
            if (_parent == null) return;

            pt = _parent.PointToClient(pt);
            Brush textColor = new SolidBrush(this.DefaultCellStyle.ForeColor);

            e.PaintBackground(e.ClipBounds, this.Selected);
            /*
            if (e.CellBounds.Contains(pt)) textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, _parent.GridColor, true);
            else textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, Color.WhiteSmoke, this.Selected);
            */

            if (this.HeaderText != null && this.HeaderText.Length > 0)
            {
                //Brush textColor = CellFunctions.DrawHeaderBack(e.CellBounds,g, _parent.GridColor, this.Selected);
                pt = CellFunctions.TextCenterInRact(e.CellBounds, g, _parent.Font, this.HeaderText);
                if (img != null)
                {
                    pt = new Point(pt.X + img.Width / 2 + 1, pt.Y);
                }
                g.DrawString(this.HeaderText as String, _parent.Font, textColor, pt.X, pt.Y + 2);
                if (img != null)
                {
                    pt = new Point(pt.X - img.Width - 2, ((e.CellBounds.Height - img.Height) / 2) + e.CellBounds.Location.Y);
                    e.Graphics.DrawImage(img, pt);
                }
            }
            else
            {
                if (img != null)
                {
                    pt = new Point(((e.CellBounds.Width - img.Width) / 2) + e.CellBounds.Location.X, ((e.CellBounds.Height - img.Height) / 2) + e.CellBounds.Location.Y);
                    e.Graphics.DrawImage(img, pt);
                }

            }
            /*
            g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                new PointF[]{
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Top+4),
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Bottom -4),
                    new PointF(e.CellBounds.Right-3, (e.CellBounds.Top + e.CellBounds.Bottom)/2)},
                new byte[]{
                    (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));
             */



            if (img != null)
            {

                e.Graphics.DrawImage(img, pt);
            }
            e.Handled = true;
            
        }

        void RePaint()
        {
            try
            {
                _parent.InvalidateCell(this.HeaderCell);
            }
            catch { }
        }


        public ItemTypes ItemType
        {
            get { return ItemTypes.Image; }
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
