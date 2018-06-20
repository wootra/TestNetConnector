using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void RowPositionChangedHandler(object sender, RowPositionChangedArgs args);
    public class RowPositionChangedArgs:EventArgs
    {
        public List<EasyGridRow> Rows = null;
        public int ChangedOffset = 0;
        public int ToIndex = -1;
        public RowPositionChangedArgs(int changedOffset, int toIndex, List<EasyGridRow> changedRow)
        {
            Rows = changedRow;
            ChangedOffset = changedOffset;
            ToIndex = toIndex;
        }
    }
}
