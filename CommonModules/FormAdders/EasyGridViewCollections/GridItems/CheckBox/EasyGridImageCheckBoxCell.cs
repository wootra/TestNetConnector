using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageCheckBoxCell : DataGridViewImageCell, IEasyGridCell, IEasyGridCheckBoxCell
    {
        EasyGridViewParent Parent{
            get { return this.DataGridView as EasyGridViewParent; }
        }
        enum CheckBoxState { Normal = 0, Checked, Intermediate, Disabled };
        public EasyGridImageCheckBoxCell()
            : base()
        {
            
            _images = new List<Image>(); //초기이미지 설정..
            _images.Add(Properties.Resources.check_normal);
            _images.Add(Properties.Resources.check_red);
            _images.Add(Properties.Resources.check_inter);
            _images.Add(Properties.Resources.check_normal_press);
            _info = new CellSpanInfo(this);
            //this.Style.SelectionBackColor = parent.DefaultCellStyle.SelectionBackColor;
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        ICollection<Image> _images = null;
        public ICollection<Image> Images
        {
            get { return _images; }
            set { _images = value; }
        }
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
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
                else if(Enabled==false) Enabled = _tempEnabled;
                
                _tempEnabled = _enabled;
                RePaint();
            }
        }

        public new Type ValueType
        {
            get
            {
                return typeof(bool?);
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

        public ItemTypes ItemType
        {
            get { return ItemTypes.ImageCheckBox; }
        }

        public bool? IsChecked
        {
            get
            {
                if (_value == 0) return false;
                else if (_value == 1) return true;
                else return null;
            }
            set
            {
                if (Enabled && Visible)
                {
                    if (value == true) SetValue(1);// _value = 1;
                    else if (value == false) SetValue(0);// _value = 0;
                    else SetValue(2);// _value = 2;
                }
                //Value = _value;
            }
        }

        int _value = 0;
        public new object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (Enabled == false || Visible == false) return;
                if (value is EasyGridCellInfo)
                {
                    SetValue(value as EasyGridCellInfo);
                }
                else if (value is int)
                {
                    SetValue((int)value);
                }
                else if (value is bool?)
                {
                    SetValue((bool?)value);
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[]{ typeof(EasyGridCellInfo), typeof(int), typeof(bool?)});
                }
            }

        }

        public void SetValue(bool? isChecked)
        {
            this.IsChecked = isChecked;
        }

        public void SetValue(EasyGridCellInfo info)
        {

            if (info.Images != null) this.Images = info.Images;

            this.Value = info.CheckInt;
        }

        public void SetValue(int intValue)
        {
            if (Enabled == false || Visible == false) return;
            if (_images.Count > intValue && intValue >= 0)
            {
                base.Value = _images.ElementAt(intValue);
            }
            else if (_images.Count == 0)
            {
                throw new Exception("미리 지정된 Image의 개수가 0입니다.");
            }
            else
            {
                base.Value = _images.ElementAt(0);
            }
            _value = intValue;
        }

        public bool? GetValue()
        {
            if (Enabled == false || Visible == false) return false;
            return this.IsChecked;
        }

        public void RePaint()
        {
            if (Parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) Parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {

            /*
             value = null;// this.Images.ElementAt((int)CheckBoxState.Normal);
            if (Span.SpanPos != SpanPosition.Spanned)
            {

                if (_visible)
                {
                    if (_enabled) value = this.Images.ElementAt(_value);
                    else if (this.Images.Count > 3) value = this.Images.ElementAt((int)CheckBoxState.Disabled);
                    else if (this.Images.Count > 0) value = this.Images.ElementAt((int)CheckBoxState.Normal);
                }
            }
            else
            {
                EasyGridImageCheckBoxCell cell = Span.SpanBaseCell as EasyGridImageCheckBoxCell;

                if (_enabled) value = this.Images.ElementAt((int)(cell.Value));
                else if (cell.Images.Count > 3) value = cell.Images.ElementAt((int)CheckBoxState.Disabled);
                else if (cell.Images.Count > 0) value = cell.Images.ElementAt((int)CheckBoxState.Normal);
            }
             */
            Span.Paint(base.Paint, g, clipBounds, cellBounds, rowIndex, cellState, value, value, errorText, cellStyle, advancedBorderStyle, paintParts);
            /*
            CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
            Point pt = CellFunctions.RectCenterInRact(cellBounds, g, this.Images.ElementAt(_value).Size);

            if (_visible)
            {
                if (_enabled) g.DrawImage(this.Images.ElementAt(_value), pt);
                else if (this.Images.Count > 3) g.DrawImage(this.Images.ElementAt((int)CheckBoxState.Disabled), pt);
                else if (this.Images.Count > 0) g.DrawImage(this.Images.ElementAt((int)CheckBoxState.Normal), pt);
            }
            paintParts = DataGridViewPaintParts.None;
            */
            //advancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
        }

        

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if(cell!=null) cell.OnMouseDown(e);
            }else if (_enabled) base.OnMouseDown(e);

        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnMouseUp(e);
            }
            else if (_enabled) base.OnMouseUp(e);
        }
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnMouseClick(e);
            }
            else if (_enabled) base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnMouseDoubleClick(e);
            }
            else if (_enabled) base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseEnter(int rowIndex)
        {
            if (_enabled) if (Span.SpanPos == SpanPosition.Spanned)
                {
                    EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                    if (cell != null) cell.OnMouseEnter(rowIndex);
                }
                else base.OnMouseEnter(rowIndex);
        }
        protected override void OnMouseLeave(int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnMouseLeave(rowIndex);
            }
            else if (_enabled) base.OnMouseLeave(rowIndex);
        }
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnClick(e);
            }
            else if (_enabled) base.OnClick(e);
        }
        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnDoubleClick(e);
            }
            else if (_enabled) base.OnDoubleClick(e);
        }
        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnContentClick(e);
            }
            else if (_enabled) base.OnContentClick(e);
        }
        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnContentDoubleClick(e);
            }
            else if (_enabled) base.OnContentDoubleClick(e);
        }
        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnEnter(rowIndex, throughMouseClick);
            }
            else if (_enabled) base.OnEnter(rowIndex, throughMouseClick);
        }
        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnKeyDown(e, rowIndex);
            }
            else if (_enabled) base.OnKeyDown(e, rowIndex);
        }
        protected override void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnKeyPress(e, rowIndex);
            }
            else if (_enabled) base.OnKeyPress(e, rowIndex);
        }
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridImageCheckBoxCell cell = (Span.SpanBaseCell as EasyGridImageCheckBoxCell);
                if (cell != null) cell.OnKeyUp(e, rowIndex);
            }
            else if (_enabled) base.OnKeyUp(e, rowIndex);
        }
        /*
        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (_enabled) base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            else
            {
                g.FillRectangle(Brushes.WhiteSmoke, cellBounds);
                if(_images.Count>0) g.DrawImage(_images.ElementAt(0), 0, 0);
            }
        }
         */
    }
}
