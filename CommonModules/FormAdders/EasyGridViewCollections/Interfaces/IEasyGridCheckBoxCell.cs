using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public interface IEasyGridCheckBoxCell
    {
        bool? IsChecked { get; set; }
    }
}
