using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
    public class VariableInfo
    {
        String _name;
        bool _exists = false;
        bool _valueExist = false;
        String[] _values;
        
        public VariableInfo(String name, String[] values)
        {
            this._name = name;
            this._values = values;
            this._exists = true;
            this._valueExist = true;
        }

        public VariableInfo(String name)
        {
            this._name = name;
            this._exists = true;
            this._valueExist = false;
        }

        public VariableInfo()
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

        public String[] Values
        {
            get { return _values; }
            set
            {
                _values = value;
                if (value == null)
                {
                    _valueExist = false;
                }else _valueExist = true;
            }
        }

        public bool Exists { get { return _exists; } }
        public bool ValueExists { get { return _valueExist; } }
    }
}
