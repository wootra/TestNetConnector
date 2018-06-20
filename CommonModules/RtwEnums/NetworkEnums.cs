using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RtwEnums.Network
{
    public enum ConnType { Disconnected,Connected, Connecting };
    public enum NetPosition { Server = 0, Client };
    public enum NetModeKind { Tcp = 0, Udp };
    public enum Endians { Big = 0, Little };

#region For networkPacket
    public enum MswPos { Before, After };
#endregion

}
