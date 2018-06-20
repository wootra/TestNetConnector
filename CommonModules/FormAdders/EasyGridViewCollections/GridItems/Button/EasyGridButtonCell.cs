using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridButtonCell : DataGridViewCell, IEasyGridCell
    {
        DataGridView _parent;
        public EasyGridButtonCell(DataGridView parent)
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

        CustomDictionary<String,Object> _relativeObject = new CustomDictionary<string,object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        public new String ToolTipText
        {
            get
            {
                return base.ToolTipText;
            }
            set
            {
                base.ToolTipText = value;
            }
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
        public new bool Visible
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
            get { return ItemTypes.Button; }
        }



        public override Type ValueType
        {
            get
            {
                return typeof(String);
            }
        }


        Font _font = new Font(SystemFonts.DefaultFont, FontStyle.Regular);
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                RePaint();
            }
        }

        Brush _fontColor = Brushes.Black;
        public Brush FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                RePaint();
            }
        }
        String _baseText = "";
        public String BaseText
        {
            get
            {
                return _baseText;
            }
            set
            {
                if (value == null) _baseText = "";
                else _baseText = value;
            }
        }

        public String Text
        {
            get
            {
                if (base.Value == null) return "";
                else return base.Value as String;
            }
            set
            {
                base.Value = value;
            }
        }

        public new object Value
        {
            get
            {
                if (base.Value == null) return "";
                else return base.Value as String;
            }
            set
            {
                if (value == null)
                {
                    base.Value = BaseText;
                }
                else if (value is string)
                {
                    base.Value = value;
                }
                else if (value is EasyGridCellInfo)
                {
                    SetValue(value as EasyGridCellInfo);
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[]{
                        typeof(EasyGridCellInfo), typeof(string)});
                }
                RePaint();
            }
        }

        public void SetValue(EasyGridCellInfo info)
        {
            base.Value = info.Text;
        }

        public void SetValue(String text)
        {
            base.Value = text;
        }

        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (_visible)
            {
                if (_enabled == false)
                {
                    g.FillRectangle(new SolidBrush(Color.WhiteSmoke), cellBounds);
                    //g.DrawRectangle(new Pen(Color.Gray, 0.5f), cellBounds);
                    //int textY = CellFunctions.TextCenterYInRact(cellBounds, g, _font);// cellBounds.Y + (int)((cellBounds.Height - textSize.Height) / 2.0);
                    Point textPos = CellFunctions.TextCenterInRact(cellBounds, g, _font, this.Text);
                    g.DrawString(Text, _font, _fontColor, textPos.X, textPos.Y);
                    g.DrawLine(new Pen(_parent.GridColor, 0.1f), cellBounds.X, cellBounds.Y + cellBounds.Height - 1, cellBounds.X + cellBounds.Width, cellBounds.Y + cellBounds.Height - 1);
                    g.DrawLine(new Pen(Color.FromArgb(200, 200, 200), 0.1f), cellBounds.X + cellBounds.Width - 1, cellBounds.Y, cellBounds.X + cellBounds.Width - 1, cellBounds.Y + cellBounds.Height - 1);
                }
                else
                {
                    Brush textColor = CellFunctions.DrawLensBack(this, cellBounds, g, _parent.GridColor, this.Selected, _enabled);
                    Point pt = CellFunctions.TextCenterInRact(cellBounds, g, _parent.Font, this.Text);
                    g.DrawString(this.Text as String, _parent.Font, textColor, pt.X, pt.Y);
                }
            }
            else
            {
                CellFunctions.DrawPlainBackground(this, Enabled, cellBounds, g, rowIndex, Selected, DataGridView.GridColor);
            }
            paintParts = DataGridViewPaintParts.None;
            
            base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            /*
            g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                new PointF[]{
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Top+4),
                    new PointF(e.CellBounds.Right-10, e.CellBounds.Bottom -4),
                    new PointF(e.CellBounds.Right-3, (e.CellBounds.Top + e.CellBounds.Bottom)/2)},
                new byte[]{
                    (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));
             */
            //e.Handled = true;
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
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
