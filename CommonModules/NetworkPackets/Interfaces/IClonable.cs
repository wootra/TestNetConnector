using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkPacket
{
    public interface IClonable
    {
        //void ClonedBy(Object cloneBase);
        Object Clone();
    }
}
