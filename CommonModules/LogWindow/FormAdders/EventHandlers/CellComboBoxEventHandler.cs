using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public delegate void CellComboBoxEventHandler(Object sender, CellComboBoxEventArgs e);

    public class CellComboBoxEventArgs : EventArgs
    {
        public DataGridView ListObj { get; set; }
        public DataGridViewRow Row { get; set; }
        public DataGridViewComboBoxCell Cell { get; set; }
        public int SelectedIndex { get; set; }
        public object SelectedObject { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public CellComboBoxEventArgs(int selectedIndex, object selectedObject, int rowIndex, int colIndex, DataGridView listObj = null, DataGridViewRow row=null, DataGridViewComboBoxCell cell = null)
        {
            ListObj = listObj;
            Row = row;
            Cell = cell;
            SelectedIndex = selectedIndex;
            SelectedObject = selectedObject;
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
    }
}
