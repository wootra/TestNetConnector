using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridHeaderCell:DataGridViewHeaderCell, IEasyGridCell
    {
        DataGridViewColumnHeaderCell _templateHeaderCell;
        public EasyGridHeaderCell(DataGridViewColumnHeaderCell templateHeaderCell)
            : base()
        {
            _templateHeaderCell = templateHeaderCell;
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        public DataGridViewColumnHeaderCell TemplateCell
        {
            get { return _templateHeaderCell; }
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
            }
        }

        public new int RowIndex{
            get { return _templateHeaderCell.RowIndex; }
        }

        public new int ColumnIndex
        {
            get { return _templateHeaderCell.ColumnIndex; }
        }



        public ItemTypes ItemType
        {
            get { return ItemTypes.Header; }
        }

        public virtual void SetValue(EasyGridCellInfo info)
        {
            SetValue(info.Text);
        }


        public void SetValue(String text)
        {
            if (base.Value == null || base.Value.Equals(text) == false)
            {
                base.Value = text;
                RePaint();
            }
        }

        public virtual void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            
        }

        int _colSpan = 1;
        public int ColSpan
        {
            get
            {
                return _colSpan;
            }
            set
            {
                _colSpan = value;
            }
        }

        int _rowSpan = 1;
        public int RowSpan
        {
            get
            {
                return _rowSpan;
            }
            set
            {
                _rowSpan = value;
            }
        }

        public virtual void RePaint()
        {
        }

        public DataHandling.CustomDictionary<string, object> RelativeObject
        {
            get { throw new NotImplementedException(); }
        }

        
    }
}
