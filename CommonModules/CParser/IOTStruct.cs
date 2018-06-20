using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
    [Serializable]
    public class IOTStruct
    {
        public String Type;
        public String Name;
        public int Index;
        public String Key;
        public IOTStruct(String key, int index, String type, String name)
        {
            Key = key;
            Index = index;
            Name = name;
            Type = type;
        }
    }
}
