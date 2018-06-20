using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public delegate void CellTextChangedEventHandler(object sender, CellTextChangedEventArgs e);

    public class CellTextChangedEventArgs : EventArgs
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public DataGridViewTextBoxCell Cell { get; set; }
        public DataGridViewRow Row { get; set; }
        public String Text { get; set; }
        public String BeforeText { get; set; }
        public bool IsCancel { get; set; }
        public CellTextChangedEventArgs(String beforeText, String afterText, int col, int row = -1, DataGridViewRow rowObj=null, DataGridViewTextBoxCell cellObj=null)
        {
            RowIndex = row;
            ColIndex = col;
            Row = rowObj;
            Cell = cellObj;
            Text = afterText;
            BeforeText = beforeText;
            IsCancel = false;
        }
    }
}
