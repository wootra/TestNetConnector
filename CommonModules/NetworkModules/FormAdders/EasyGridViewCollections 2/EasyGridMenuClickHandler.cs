using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{
    public delegate void EasyGridMenuClickHandler(Object sender, EasyGridMenuClickArgs args);// String text, int index, int selectedRow, int selectedCol, EasyGridMenuItem MenuItem, int depth);

    public class EasyGridMenuClickArgs : EventArgs
    {
        public String Text;
        public int IndexInMenu = -1;
        public int RowIndex = -1;
        public int ColIndex = -1;
        public EasyGridMenuItem MenuItem;
        public int Depth = 0;
        public EasyGridMenuClickArgs(String text, int indexInMenu, int row, int col, EasyGridMenuItem menuItem, int depth = 0)
        {
            Text = text;
            IndexInMenu = indexInMenu;
            RowIndex = row;
            ColIndex = col;
            MenuItem = menuItem;
            this.Depth = depth;
        }
    }
}
