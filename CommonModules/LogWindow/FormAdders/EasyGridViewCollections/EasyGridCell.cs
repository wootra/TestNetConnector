using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCell:DataGridViewCell,IEasyGridCell
    {
         DataGridView _parent;
         public EasyGridCell(DataGridView parent)
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
            get { return ItemTypes.CloseButton; }
        }


        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
    }
}
