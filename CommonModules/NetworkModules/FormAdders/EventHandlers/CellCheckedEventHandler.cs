using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{

    public delegate void CellCheckedEventHandler(Object sender, CellCheckedEventArgs e);

    public class CellCheckedEventArgs : EventArgs
    {
        public bool? Checked { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        //public object Source { get; set; }
        //public object OriginalSource { get; set; }
        public List<int> AddedRows { get; set; }
        public List<int> RemovedRows { get; set; }
        
        public CellCheckedEventArgs(bool? isChecked, int row_index, int col_index, List<int> addRows, List<int> removedRows)
        {
            Checked = isChecked;
            RowIndex = row_index;
            ColumnIndex = col_index;
            AddedRows = addRows;
            RemovedRows = removedRows;
        }
    }
}
