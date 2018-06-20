using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphModules
{
    public interface IGraphParent
    {
        MultiLineGraphWin IG_ReturnGraph{get;set;}
        Control IG_Parent{ get;set;}
    }
}
