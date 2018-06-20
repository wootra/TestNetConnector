using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridTextBoxCell:DataGridViewTextBoxCell,IEasyGridCell
    {
        DataGridView _parent;
        public EasyGridTextBoxCell(DataGridView parent)
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
            get { return ItemTypes.TextBox; }
        }

        bool _isEditable = true;
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = true; }
        }


        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
    }
}
