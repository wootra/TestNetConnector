using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRadioButtonColumn:DataGridViewColumn, IEasyGridColumn
    {
        public EasyGridRadioButtonColumn(EasyGridViewParent parent)
            : base()
        {
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridRadioButtonCell(parent);
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

        public EasyGridRadioButtonColumn(EasyGridRadioButtonCell cellTemplate, EasyGridViewParent parent)
            : base(cellTemplate)
        {
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            _columnSpan = new ColumnSpan(this);
            _dataGridView = parent;
        }

      
        

        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }


        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }

        EasyGridRadioButtonCollection _initItems = new EasyGridRadioButtonCollection(null);

        public EasyGridRadioButtonCollection Items
        {
            get
            {
                return _initItems;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _initItems.SelectedIndex;
            }
            set
            {
                _initItems.SelectedIndex = value;
            }
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            
        }


        public ItemTypes ItemType
        {
            get { return ItemTypes.RadioButton; }
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
