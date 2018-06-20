using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormAdders
{
    public delegate void DragLeaveEventHandler(DragLeaveEventArgs e);
    public class DragLeaveEventArgs : EventArgs
    {
        Object _item;
        public DragLeaveEventArgs(Object Item)
        {
            _item = Item;
        }
        public Object Item { get { return _item; } }
    }
}
