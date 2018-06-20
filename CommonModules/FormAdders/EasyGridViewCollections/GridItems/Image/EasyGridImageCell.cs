using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageCell : DataGridViewImageCell, IEasyGridCell
    {
        DataGridView _parent;
        ICollection<Image> _savedImages = new List<Image>();
        int ImageFixedWidth = 0;
        int ImageFixedHeight = 0;

        public EasyGridImageCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            this.ImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        public ICollection<Image> Images
        {
            get { return _savedImages; }
            set { _savedImages = value; }
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
        int _colSpan = 1;
        public int ColSpan
        {
            get { return _colSpan; }
            set { _colSpan = value; }
        }
        int _rowSpan = 1;
        public int RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = value; }
        }
        
        bool _visible = true;
        bool _tempEnabled = false;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {

                _visible = value;
                if (value == false) Enabled = false;// _enabled = false;
                else if (Enabled == false) Enabled = _tempEnabled;

                _tempEnabled = _enabled;
                RePaint();
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.Image; }
        }

        public new Type ValueType
        {
            get
            {
                return typeof(Image);
            }
        }

        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            
            //if (cellBounds.Y == 0) return;
            //CellFunctions.DrawPlainBackground(this, this.Enabled, cellBounds, g, rowIndex, this.Selected, this.DataGridView.GridColor);

            if (_visible)
            {

                if (_imageLayout != System.Windows.Forms.ImageLayout.Stretch && _imageLayout != System.Windows.Forms.ImageLayout.Zoom && (ImageFixedWidth > 0 || ImageFixedHeight > 0))
                {
                    paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.Focus;
                    base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);


                    if (_selectedIndex > 0)
                    {
                        if (_imageLayout == System.Windows.Forms.ImageLayout.Center)
                        {
                            int wid = (ImageFixedWidth == 0) ? this.Images.ElementAt(_selectedIndex).Width : ImageFixedWidth;
                            int hig = (ImageFixedHeight == 0) ? this.Images.ElementAt(_selectedIndex).Height : ImageFixedHeight;
                            int x = cellBounds.Location.X + (cellBounds.Width + wid) / 2;
                            int y = cellBounds.Location.Y + (cellBounds.Height + hig) / 2;
                            g.DrawImage(this.Images.ElementAt(_selectedIndex), new Rectangle(x, y, wid, hig));
                        }
                        else
                        {
                            int wid = (ImageFixedWidth == 0) ? this.Images.ElementAt(_selectedIndex).Width : ImageFixedWidth;
                            int hig = (ImageFixedHeight == 0) ? this.Images.ElementAt(_selectedIndex).Height : ImageFixedHeight;
                            int x = cellBounds.Location.X;
                            int y = cellBounds.Location.Y + (cellBounds.Height + hig) / 2;
                            g.DrawImage(this.Images.ElementAt(_selectedIndex), new Rectangle(x, y, wid, hig));
                        }
                    }
                    
                }
                else
                {
                    
                    //CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, rowIndex, Selected, _parent.GridColor);
                    //this.Style.SelectionBackColor = CellFunctions.BackColor(this, this.RowIndex);
                    cellStyle.SelectionBackColor = CellFunctions.BackColor(this);
                    //this.Style.BackColor = CellFunctions.BackColor(this,this.RowIndex);
                    cellStyle.BackColor = CellFunctions.BackColor(this);
                    //paintParts = DataGridViewPaintParts.ContentForeground;
                    paintParts = DataGridViewPaintParts.All;//.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.Focus|DataGridViewPaintParts.ContentForeground;
                    base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                }
                
            }
            else
            {
                //CellFunctions.DrawPlainBackground(this, this.Enabled, cellBounds, g, rowIndex, this.Selected, Color.Transparent);
               
            }
            
            
        }
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            /*
            Point pt = this.DataGridView.PointToClient(Control.MousePosition);
            if (e.CellBounds.Contains(pt))
            {
                CellFunctions.DrawPlainBackground(this, this.Enabled, e.CellBounds, e.Graphics, e.RowIndex, true, this.DataGridView.GridColor);
            }
            else
            {
                CellFunctions.DrawPlainBackground(this, this.Enabled, e.CellBounds, e.Graphics, e.RowIndex, false, this.DataGridView.GridColor);
            }
            */

        }
        int _selectedIndex = -1;
        public new object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if(value is EasyGridCellInfo){
                    SetValue(value as EasyGridCellInfo);
                }
                else if (value is Image)
                {
                    SetValue(value as Image);
                }
                else if (value is int)
                {
                    SetValue((int)value);
                }
                else if (value is Enum){
                    SetValue((int)(Convert.ChangeType(value, typeof(int))));
                }
                else if (value is string)
                {
                    int intValue;
                    if (int.TryParse(value as String, out intValue)) SetValue(intValue);
                    else if (File.Exists(value as string)) SetValue(Image.FromFile(value as String));
                    //else throw new InvalidTypeException(value, new Type[] { typeof(EasyGridCellInfo), typeof(int), typeof(Image) });
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[] { typeof(EasyGridCellInfo), typeof(int), typeof(Image) });
                    //throw new Exception("테이블 [" + this.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, Image");
                }
            }
        }
        public void SetValue(EasyGridCellInfo info){
            if (info.Images != null) this.Images = info.Images;

            if (info.SelectedIndex >= 0)
            {
                
                _selectedIndex = info.SelectedIndex;
                base.Value = (Images.Count>0)? Images.ElementAt(_selectedIndex) : null;
                this.ImageLayout = info.ImageLayout;
            }
        }
        ImageLayout _imageLayout = ImageLayout.Stretch;
        public new ImageLayout ImageLayout
        {
            get
            {
                return _imageLayout;
            }
            set
            {
                _imageLayout = value;
                switch (value)
                {
                    case System.Windows.Forms.ImageLayout.Center:
                        base.ImageLayout = DataGridViewImageCellLayout.Normal;
                        break;
                    case System.Windows.Forms.ImageLayout.None:
                        base.ImageLayout = DataGridViewImageCellLayout.NotSet;
                        break;
                    case System.Windows.Forms.ImageLayout.Stretch:
                        base.ImageLayout = DataGridViewImageCellLayout.Stretch;
                        break;
                    case System.Windows.Forms.ImageLayout.Tile:
                        base.ImageLayout = DataGridViewImageCellLayout.NotSet;
                        break;
                    case System.Windows.Forms.ImageLayout.Zoom:
                        base.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        break;
                }
            }
        }

        public void SetValue(Image image)
        {
            if (base.Value == null || base.Value.Equals(image) == false)
            {
                base.Value = image;
                RePaint();
            }
            _selectedIndex = -1;
        }

        public void SetValue(int index)
        {
            if (index < 0)
            {
                base.Value = null;
                RePaint();
            }else if (Images != null && Images.Count > index)
            {
                Image image = Images.ElementAt(index);
                if (base.Value==null || base.Value.Equals(image) == false)
                {
                    base.Value = image;
                    RePaint();
                }
            }
            _selectedIndex = index;
        }

        public int GetValueInt()
        {
            return _selectedIndex;
        }

        public Image GetValue()
        {
            return base.Value as Image;
        }

        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }


        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseUp(e);
        }
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseEnter(int rowIndex)
        {
            if (_enabled) base.OnMouseEnter(rowIndex);
        }
        protected override void OnMouseLeave(int rowIndex)
        {
            if (_enabled) base.OnMouseLeave(rowIndex);
        }
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnClick(e);
        }
        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnDoubleClick(e);
        }
        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnContentClick(e);
        }
        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnContentDoubleClick(e);
        }
        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (_enabled) base.OnEnter(rowIndex, throughMouseClick);
        }
        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyDown(e, rowIndex);
        }
        protected override void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyPress(e, rowIndex);
        }
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyUp(e, rowIndex);
        }

    }
}
