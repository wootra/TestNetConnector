using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void CellClickEventHandler(Object sender, CellClickEventArgs e);

    public class CellClickEventArgs : EventArgs
    {
        public DataGridView ListObj { get; set; }
        public DataGridViewRow ListRowItem { get; set; }
        public DataGridViewCell SelectedItem { get; set; }
        public ItemTypes ItemType;
        public object Value;
        public object BeforeValue;
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public bool? IsCancel { get; set; }
        public EventArgs MouseEventArg { get; set; }

        public CellClickEventArgs(int rowIndex, int colIndex, DataGridViewRow row, ItemTypes itemType,
            DataGridViewCell selectedItem, DataGridView listObj, EventArgs e, object value)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            ListRowItem = row;
            ItemType = itemType;
            SelectedItem = selectedItem;
            ListObj = listObj;
            MouseEventArg = e;
            Value = value;
        }
    }
}
