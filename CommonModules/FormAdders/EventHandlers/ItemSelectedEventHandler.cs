using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormAdders
{
    public delegate void ItemSelectedEventHandler(ItemSelectedEventArgs e);
    public class ItemSelectedEventArgs : EventArgs
    {
        Object _item;
        Object _sender;
        String _name;
        public ItemSelectedEventArgs(Object sender, Object Item, String name="")
        {
            _item = Item;
            _sender = sender;
            _name = name;
        }
       
        public Object Item { get { return _item; } }
        public Object Sender { get { return _sender; } }
        public Object Name { get { return _name; } }
    }
}
