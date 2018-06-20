using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{

    public enum Direction { Input = 1, Output = 2, NONE };
    public enum ContextKind { VARIABLE, ARRAY, STRUCT, GLOBAL_MAIN };
    public enum DataType
    {
        CHAR=0, SHORT, INT, LONGLONG, LONG,
        UCHAR, USHORT, UINT, ULONGLONG, ULONG,
        CHAR_P,
        FLOAT, DOUBLE, STRUCT, NONE
    };
}
