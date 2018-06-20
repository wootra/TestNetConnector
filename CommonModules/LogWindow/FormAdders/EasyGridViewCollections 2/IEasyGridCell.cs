using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;

namespace FormAdders.EasyGridViewCollections
{
    public interface IEasyGridCell
    {
        bool Enabled { get; set; }
        ItemTypes ItemType { get;}
    }
}
