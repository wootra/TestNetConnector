using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridComboBoxCell : DataGridViewComboBoxCell, IEasyGridCell
    {
        DataGridView _parent;
        int _selectedIndex=-1;
        String _selectedItem = null;

        ICollection<String> _items;
        public new ICollection<String> Items{
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                base.Items.AddRange(value);
                
                String tooltipText = "";


                if (_enabled == true)
                {
                    foreach (String aLine in value)
                    {
                        tooltipText += "," + aLine;
                    }
                    this.ToolTipText = tooltipText;
                }
                else
                {
                    this.ToolTipText = null;
                }
            }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        public EasyGridComboBoxCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            Items = new List<String>();
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        void _parent_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            ///throw new NotImplementedException();
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
                if (value == true)
                {
                    this.ToolTipText = null;
                }
                RePaint();
            }
        }
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
        }
        public ItemTypes ItemType
        {
            get { return ItemTypes.ComboBox; }
        }


        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            
            Brush textColor = CellFunctions.DrawPlainBackground(this, this.Enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
            
            //float y = (cellBounds.Height - textSize.Height) / 2 + cellBounds.Y;
            SizeF textSize = g.MeasureString(this.Value as String, _parent.Font);
            if ((textSize.Width + 13) > cellBounds.Width)
            {
                g.DrawString(this.Value as String, _parent.Font, textColor, new RectangleF(new PointF(cellBounds.X + 3.0f, cellBounds.Y), new SizeF(cellBounds.Width-13, cellBounds.Height)));
            }
            else
            {
                float y = CellFunctions.TextCenterYInRact(cellBounds, g, _parent.Font);
                g.DrawString(this.Value as String, _parent.Font, textColor, new PointF(cellBounds.X + 3.0f, y));
            }
            float margin = (cellBounds.Height - 10)/2;
            float pathTop = cellBounds.Y + margin; //path의 height를 10으로 한다.
            float PathBottom = cellBounds.Y + cellBounds.Height - margin;
            
            g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                new PointF[]{
                    new PointF(cellBounds.Right-10, pathTop),
                    new PointF(cellBounds.Right-10, PathBottom),
                    new PointF(cellBounds.Right-3, (pathTop + PathBottom)/2)},
                new byte[]{
                    (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));

                        

        }

        public String Text
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                base.Value = value;
                _selectedIndex = Items.ToList().IndexOf(value);
                _selectedItem = value;
            }
        }

        public String SelectedItem
        {
            get
            {
                if (_enabled) return _selectedItem;
                else return "";
            }
            set
            {
                base.Value = value;
                _selectedIndex = Items.ToList().IndexOf(value);
                _selectedItem = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                if (_enabled) return _selectedIndex;
                else return -1;
            }
            set
            {
                if (value>=0 && Items.Count > value)
                {
                    _selectedIndex = value;
                    _selectedItem = Items.ElementAt(value);
                    base.Value = _selectedItem;
                }
                else
                {
                    _selectedItem = null;
                    _selectedIndex = -1;
                    base.Value = "";
                }
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(String);
            }
        }

        public new object Value
        {
            get
            {
                if (_enabled) return Text;
                else return "";
            }
            set
            {
                if (value == null)
                {
                    _selectedIndex = -1;
                    _selectedItem = "";
                    
                }
                else if (value is EasyGridCellInfo)
                {
                    EasyGridCellInfo info = value as EasyGridCellInfo;
                    if (info.Items != null)
                    {
                        this.Items = info.Items;
                        ContextMenuStrip menu = (this.ContextMenuStrip==null)? new ContextMenuStrip() : this.ContextMenuStrip;
                        menu.Items.Clear();
                        //this.Items = new .Clear(); //기존 것을 지운다.
                        String tooltipText = "";
                        foreach (String aLine in Items)
                        {
                            menu.Items.Add(aLine);
                            tooltipText+=","+aLine;
                        }
                        if (_enabled == true) this.ToolTipText = tooltipText;
                        else this.ToolTipText = "";
                    }
                    this.Value = info.SelectedText;
                }
                else if (value is int)
                {
                    this.SelectedIndex = (int)value;
                }
                else if (value is String)
                {
                    this.SelectedItem = value as String;
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[]{ typeof(EasyGridCellInfo), typeof(int), typeof(String)});
                }
                RePaint();
            }
        }

        public void SetValue(EasyGridCellInfo info)
        {
            if (info.Items != null) this.Items = info.Items;
            this.Value = info.SelectedText;
        }

        public void SetValue(String value)
        {
             Text = value;
        }

        public String GetValue()
        {
            return Text;
        }

        public int GetValueIndex()
        {
            return _selectedIndex;
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
