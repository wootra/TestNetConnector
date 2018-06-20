using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner.PacketDatas
{
    public enum PacketTypes { IO = 1, Logic = 2, M1553 = 3, None = 0xff };
    public enum FieldTypes { Static = 0, LoopCount, VariableSize, Dynamic, Auto, DataSize, Variable, NULL };
    public enum DynamicFieldTypes { BoardID, Channel, ValueTypeSize, Repeat, ValueMask, Normal, NULL };
    public enum AutoFieldTypes { StartEventID, StopEventID, TimerID, NULL };
    public enum ResponseFieldTypes { Fixed = 0, Variable };
    public enum PacketHandlingTypes { Static = 0, Loop, Serial };
    public enum StructType { Command, Response };
}
