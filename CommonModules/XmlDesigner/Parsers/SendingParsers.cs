using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;

namespace XmlDesigner.Parsers
{
    public class SendingParsers
    {
        public static String ToDecimal(string value)
        {
            TypeHandling.TypeName type = TypeHandling.getTypeKind(value);
            switch (type)
            {
                case TypeHandling.TypeName.HexString:
                    return TypeHandling.getHexNumber<int>(value).ToString();
                case TypeHandling.TypeName.Float:
                    return ((int)double.Parse(value)).ToString();
                case TypeHandling.TypeName.Integer:
                    return value;
                default:
                    return "-1";
            }
        }

        public static object RunParser(object value, String parserValue){
            int bracketStart = parserValue.IndexOf("(");
            int bracketEnd = parserValue.LastIndexOf(")");

            string funcName = parserValue.Substring(0, bracketStart).Trim();
            string args = parserValue.Substring(bracketStart + 1, (bracketEnd - bracketStart - 1));
            string[] arg = args.Split(",".ToCharArray());
            for (int i = 0; i < arg.Length; i++) arg[i] = arg[i].Trim();//각 arg의 빈칸 뺌..
            return RunParser(value, funcName, arg);
        }

        public static object RunParser(object value, string funcName, params String[] arg){

            switch (funcName)
            {
                case "ToDecimal":
                    return ToDecimal(value.ToString());
                default:
                    return value.ToString();

            }
        }
    }
}
