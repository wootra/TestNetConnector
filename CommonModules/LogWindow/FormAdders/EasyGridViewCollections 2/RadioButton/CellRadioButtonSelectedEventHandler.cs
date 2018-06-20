using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void CellRadioButtonSelectedEventHandler(object sender, CellRadioButtonSelectedEventArgs args);

    public class CellRadioButtonSelectedEventArgs:EventArgs
    {
        public int SelectedIndex;
        public int RowIndex;
        public int ColIndex;
        public String Text;
        public EasyGridRadioButtonCell Cell;
        public CellRadioButtonSelectedEventArgs(int index, int row, int col, String text, EasyGridRadioButtonCell cell){
            SelectedIndex = index;
            RowIndex = row;
            ColIndex = col;
            Text = text;
            Cell = cell;
        }
    }
}
