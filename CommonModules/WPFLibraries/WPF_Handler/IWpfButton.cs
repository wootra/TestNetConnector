using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Handler
{
    public interface IWpfControls
    {
        EventHelper Events { get; }
        String Text { get; set; }
    }
}
