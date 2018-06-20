using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridButtonCell : DataGridViewButtonCell, IEasyGridCell
    {
        DataGridView _parent;
        public EasyGridButtonCell(DataGridView parent)
            : base()
        {
            _parent = parent;
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

        public ItemTypes ItemType
        {
            get { return ItemTypes.Button; }
        }


        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
    }
}
