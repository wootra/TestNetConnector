using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCheckBoxColumn:DataGridViewCheckBoxColumn, IEasyGridColumn, IEasyGridCheckBoxCell
    {
        EasyGridViewParent _parent;
        public EasyGridCheckBoxColumn(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridCheckBoxCell(parent);
            _columnSpan = new ColumnSpan(this);
        }

        ColumnSpan _columnSpan;
        public ColumnSpan Span { get { return _columnSpan; } }

        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }

        public EasyGridCheckBoxColumn(EasyGridViewParent parent, bool threeStates)
            : base(threeStates)
        {
            _parent = parent;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridCheckBoxCell(parent);
            _columnSpan = new ColumnSpan(this);
            
        }

        public new EasyGridViewParent DataGridView
        {
            get { return _parent; }
        }

        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
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
            get { return ItemTypes.CheckBox; }
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
            }
        }

        
    }
}
