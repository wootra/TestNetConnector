using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
    public class RecvDataPacket
    {
        String _name;
        Type _dataType;
        public enum PacketType { Dynamic, Const };
        PacketType _packetType;

        bool _exists = false;
        bool _valueExist = false;
        
        public RecvDataPacket(String name, String[] values)
        {
            this._name = name;

            this._exists = true;
            this._valueExist = true;
        }

        public RecvDataPacket(String name)
        {
            this._name = name;
            this._exists = true;
            this._valueExist = false;
        }

        public RecvDataPacket()
        {
            this._exists = false;
            this._valueExist = false;
        }

        public String Name {
            get { return _name; }
            set
            {
                _name = value;
                if (_name != null && _name.Length > 0) _exists = true;
                else _exists = false;
            }
        }

      

        public bool Exists { get { return _exists; } }
        public bool ValueExists { get { return _valueExist; } }
    }
}
