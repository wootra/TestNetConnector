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
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public int ColumnIndex { get; set; }
        //public object Source { get; set; }
        //public object OriginalSource { get; set; }
        public List<int> AddedRows { get; set; }
        public List<int> RemovedRows { get; set; }
        public bool IsCancel = false;
        public CellCheckedEventArgs(bool? isChecked, int col_index, int startRowIndex, int endRowIndex, List<int> addRows, List<int> removedRows)
        {
            Checked = isChecked;
            StartRowIndex = startRowIndex;
            EndRowIndex = endRowIndex;
            ColumnIndex = col_index;
            AddedRows = addRows;
            RemovedRows = removedRows;
        }
    }
}
