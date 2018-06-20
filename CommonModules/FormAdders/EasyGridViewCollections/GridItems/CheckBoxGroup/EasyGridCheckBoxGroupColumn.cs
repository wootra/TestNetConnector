using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCheckBoxGroupColumn:DataGridViewColumn, IEasyGridColumn
    {
        EasyGridViewParent _parent;
        public EasyGridCheckBoxGroupColumn(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridCheckBoxGroupCell(parent);
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


        public EasyGridCheckBoxGroupColumn(EasyGridCheckBoxGroupCell cellTemplate)
            : base(cellTemplate)
        {
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            _columnSpan = new ColumnSpan(this);
        }
        
        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }


        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }


        EasyGridCheckBoxGroupCollection _initItems = new EasyGridCheckBoxGroupCollection(null);

        public EasyGridCheckBoxGroupCollection Items
        {
            get
            {
                return _initItems;
            }
        }
        
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            /*
            if (this.Selected)
            {
            }
            else
            {
            }
            int y = CellFunctions.TextCenterYInRact(titleBounds, g, _parent.Font);
            g.DrawString(this.HeaderText, _parent.Font, new SolidBrush(_parent.ForeColor), new PointF(titleBounds.X, y));
            */
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
            get { return ItemTypes.CheckBoxGroup; }
        }

        public ICollection<int> GetValue()
        {
            List<int> selected = new List<int>();
            for (int i = 0; i < _initItems.Count; i++)
            {
                if (_initItems[i].Checked) selected.Add(i);
            }
            return selected;
        }

        public void SetValue(EasyGridCellInfo info)
        {
            if (info.Items != null)
            {
                this.Items.Clear();
                this.Items.Add(info.Items);
            }
            SetValue(info.SelectedIndices);
        }

        public void SetValue(ICollection<int> value)
        {
            if (value != null)
            {
                for (int i = 0; i < _initItems.Count; i++)
                {

                    if (value.Contains(i)) _initItems[i].Checked = true;
                    else _initItems[i].Checked = false;
                }
                
            }
            else
            {
                for (int i = 0; i < _initItems.Count; i++)
                {
                    _initItems[i].Checked = false;
                }
                
            }
        }

        public new int Width
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
