using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCheckBoxCell : DataGridViewCheckBoxCell, IEasyGridCell, IEasyGridCheckBoxCell
    {
        DataGridView _parent;
        public EasyGridCheckBoxCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

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
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
        }
        bool? _isChecked = false;
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                base.Value = value;
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
        
        public override Type ValueType
        {
            get
            {
                return typeof(bool?);
            }
        }

        public new object Value
        {
            get {
                return _isChecked;
            }
            set
            {
                if (value is EasyGridCellInfo)
                {
                    EasyGridCellInfo info = value as EasyGridCellInfo;
                    base.Value = info.Checked;
                }
                else if(value is int)
                {
                    _isChecked = (value.Equals(0)) ? false : (value.Equals(1)) ? true : (bool?)null;
                }
                else if (value is bool?)
                {
                    _isChecked = (bool?)value;
                    /*
                    if (value.Equals(true)) base.Value = 1;
                    else if (value.Equals(false)) base.Value = 0;
                    else if (value.Equals(null)) base.Value = 2;
                    else if (value.Equals(0) || value.Equals(1) || value.Equals(2))
                    {
                        base.Value = value;
                    }
                    else base.Value = 0;
                     */
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[]{
                        typeof(EasyGridCellInfo), typeof(int), typeof(bool?)});
                }
            }
        }

        public void SetValue(EasyGridCellInfo info)
        {
            base.Value = info.Checked;
        }


        public ItemTypes ItemType
        {
            get { return ItemTypes.CheckBox; }
        }


        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if(_enabled) base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if(_enabled) base.OnMouseUp(e);
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

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (_enabled) base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            else g.FillRectangle(Brushes.WhiteSmoke, cellBounds);
        }
    }
}
