using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules3
{
    public interface IClonable
    {
        //void ClonedBy(Object cloneBase);
        Object Clone();
    }
}
