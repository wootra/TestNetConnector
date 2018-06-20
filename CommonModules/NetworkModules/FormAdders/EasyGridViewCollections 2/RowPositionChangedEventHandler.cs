using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void RowPositionChangedHandler(object sender, RowPositionChangedArgs args);
    public class RowPositionChangedArgs:EventArgs
    {
        List<EasyGridRow> Rows = null;
        int ChangedOffset = 0;
        int ToIndex = -1;
        public RowPositionChangedArgs(int changedOffset, int toIndex, List<EasyGridRow> changedRow)
        {
            Rows = changedRow;
            ChangedOffset = changedOffset;
            ToIndex = toIndex;
        }
    }
}
