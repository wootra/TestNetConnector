using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageCell : DataGridViewImageCell, IEasyGridCell
    {
        DataGridView _parent;
        ICollection<Image> _savedImages = null;
        public EasyGridImageCell(DataGridView parent)
            : base()
        {
            _parent = parent;
        }

        public ICollection<Image> Images
        {
            get { return _savedImages; }
            set { _savedImages = value; }
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
            get { return ItemTypes.Image; }
        }


        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
    }
}
