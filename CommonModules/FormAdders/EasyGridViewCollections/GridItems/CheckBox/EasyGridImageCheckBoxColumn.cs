using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageCheckBoxColumn:DataGridViewImageColumn, IEasyGridColumn, IEasyGridCheckBoxCell
    {
        EasyGridImageCheckBoxCell _cellTemplate;
        EasyGridViewParent _parent{
            get { return this.DataGridView; }
        }
        public EasyGridImageCheckBoxColumn()
            : base(false)
        {
            
            _images = new List<Image>();
            _images.Add(Properties.Resources.check_normal);
            _images.Add(Properties.Resources.check_red);
            _images.Add(Properties.Resources.check_inter);
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            
            this.CellTemplate = new EasyGridImageCheckBoxCell();
            _columnSpan = new ColumnSpan(this);
            
        }

        public new EasyGridViewParent DataGridView
        {
            get { return base.DataGridView as EasyGridViewParent; }
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

        ICollection<Image> _images;
        int _selectedIndex = 0;
/*
        public bool? Checked
        {
            get { return (_selectedIndex==0)?false : (_selectedIndex==1)?true: (bool?)null; }
            set { SelectedIndex = (value==true) ? 1 : (value == false) ? 0 : 2; }
        }
        */
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
                return Images.ElementAt(_selectedIndex);
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value>=0 && Images.Count > value)
                {
                    base.Image = Images.ElementAt(value);
                    _selectedIndex = value;
                }
                else
                {
                    base.Image = Images.ElementAt(0);
                    _selectedIndex = 0;
                }
                RePaint();
            }
        }

        public bool? IsChecked
        {
            get
            {
                return (_selectedIndex==1)?true: (_selectedIndex==0)?false:(bool?)null;
            }
            set
            {
                if (value == true) SelectedIndex = 1;
                else if (value == false) SelectedIndex = 0;
                else SelectedIndex = 2;
                
            }
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            //e.PaintBackground(e.ClipBounds, false);
            e.PaintBackground(e.ClipBounds, this.Selected);

            Graphics g = e.Graphics;
            Image img = this.Image;

            Point pt = Control.MousePosition;
            DataGridView _parent = base.DataGridView;
            if (_parent == null) return;

            pt = _parent.PointToClient(pt);
            Brush textColor = new SolidBrush(this.DefaultCellStyle.ForeColor);
            /*
            if (e.CellBounds.Contains(pt)) textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, _parent.GridColor, true);
            else textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, Color.WhiteSmoke, this.Selected);
            */
            if (_text != null && _text.Length > 0)
            {
                //Brush textColor = CellFunctions.DrawHeaderBack(e.CellBounds,g, _parent.GridColor, this.Selected);
                pt = CellFunctions.TextCenterInRact(e.CellBounds, g, _parent.Font, _text);
                pt = new Point(pt.X + img.Width / 2 + 1, pt.Y);

                g.DrawString(_text as String, _parent.Font, textColor, pt.X, pt.Y + 2);
                pt = new Point(pt.X - img.Width - 2, ((e.CellBounds.Height - img.Height) / 2) + e.CellBounds.Location.Y);
            }
            else
            {
                pt = new Point(((e.CellBounds.Width - img.Width) / 2) + e.CellBounds.Location.X, ((e.CellBounds.Height - img.Height) / 2) + e.CellBounds.Location.Y);

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
          


            
            
            e.Graphics.DrawImage(img, pt);

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

        String _text = "";
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.ImageCheckBox; }
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
