using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormAdders
{
    public delegate void IndexSelectedEventHandler(IndexSelectedEventArgs e);
    public class IndexSelectedEventArgs : EventArgs
    {
        Object _item;
        Object _sender;
        int _index;
        public IndexSelectedEventArgs(Object sender, Object Item, int index)
        {
            _item = Item;
            _sender = sender;
            _index = index;
        }
       
        public Object Item { get { return _item; } }
        public Object Sender { get { return _sender; } }
        public int Index { get { return _index; } }
    }
}
