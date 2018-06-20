using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkPacket
{
    public interface INetPacket
    {
        Array ArrayBuffer { get; set; }
        List<Array> Children { get; set; }
        int ChildOffset { get; }
        int bufferByteSize { get; }
        Object Clone();
        //void ClonedBy(Object cloneBase);
    }
}
