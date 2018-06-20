using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void CellCheckBoxGroupSelectedEventHandler(object sender, CellCheckBoxGroupSelectedEventArgs args);

    public class CellCheckBoxGroupSelectedEventArgs : EventArgs
    {
        public int SelectedIndex;
        public int RowIndex;
        public int ColIndex;
        public ICollection<int> SelectedIndice;
        public EasyGridCheckBoxGroupCell Cell;
        public CellCheckBoxGroupSelectedEventArgs(int index, int row, int col, ICollection<int> selectedIndice, EasyGridCheckBoxGroupCell cell)
        {
            SelectedIndex = index;
            RowIndex = row;
            ColIndex = col;
            SelectedIndice = selectedIndice;
            Cell = cell;
        }
    }
}
