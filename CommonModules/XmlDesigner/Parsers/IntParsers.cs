using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner.Parsers
{
    public class IntParsers
    {
        public static int ToTrueFalseWithMinMax(object value, string minCondition, string maxCondition, int trueValue=1, int falseValue=0)
        {
            if (value is int || value is short || value is byte)
            {
                int minInt = Convert.ToInt32(minCondition);
                int maxInt = Convert.ToInt32(minCondition);
                int valueInt = Convert.ToInt32(value);
                
                return (valueInt >= minInt && valueInt <= maxInt) ? trueValue : falseValue;
                
            }
            else if (value is float || value is double || value is string)
            {
                double minInt = Convert.ToDouble(minCondition);
                double maxInt = Convert.ToDouble(minCondition);
                double valueInt = Convert.ToDouble(value);

                return (valueInt >= minInt && valueInt <= maxInt) ? trueValue : falseValue;
                
            }
            else
            {
                throw new Exception("변환불가능");
            }

        }

        public static int ToTrueFalse(object value, string passValue, int trueValue=1, int falseValue=0)
        {
            return (value.ToString().Equals(passValue)) ? trueValue : falseValue;
        }
        
        public static int RunParser(object value, String parserValue){
            int bracketStart = parserValue.IndexOf("(");
            int bracketEnd = parserValue.LastIndexOf(")");

            string funcName = parserValue.Substring(0, bracketStart).Trim();
            string args = parserValue.Substring(bracketStart + 1, (bracketEnd - bracketStart - 1));
            string[] arg = args.Split(",".ToCharArray());
            for (int i = 0; i < arg.Length; i++) arg[i] = arg[i].Trim();//각 arg의 빈칸 뺌..

            switch (funcName)
            {
                
                case "ToTrueFalseByMinMax":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 min 이 없습니다.");
                        if (arg.Length < 2 || arg[1].Length == 0) throw new Exception("두 번째 매개변수 max 가 없습니다.");
                        int trueValue = (arg.Length > 2 && arg[2].Length > 0) ? int.Parse(arg[2]) : 1;
                        int falseValue = (arg.Length > 3 && arg[3].Length > 0) ? int.Parse(arg[3]) : 0;

                        return ToTrueFalseWithMinMax(value, arg[0], arg[1], trueValue, falseValue);
                    }
                case "ToTrueFalse":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 trueValue 가 없습니다.");
                        int trueValue = (arg.Length > 1 && arg[1].Length > 0) ? int.Parse(arg[1]) : 1;
                        int falseValue = (arg.Length > 2 && arg[2].Length > 0) ? int.Parse(arg[2]) : 0;

                        return ToTrueFalse(value, arg[0], trueValue, falseValue);
                    }
                default:
                    throw new Exception("변환불가능 - parser명: StringParser." + funcName);

            }
        }
    }
}
