using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner.Parsers
{
    public class TextParser
    {
        public enum StringFormats { Capitals=0, BigFirst, Smalls };
        public static String[] StringFormatTexts = new String[] { "Capitals","BigFirst", "Smalls"};

        public static String CustomTextByMinMax(object value, string minValue, string maxValue, String trueText, String falseText)
        {
            if (value is int || value is short || value is byte)
            {
                int minInt = Convert.ToInt32(minValue);
                int maxInt = Convert.ToInt32(minValue);
                int valueInt = Convert.ToInt32(value);
                
                return (valueInt >= minInt && valueInt <= maxInt) ? trueText : falseText;
                
            }
            else if (value is float || value is double || value is string)
            {
                double minInt = Convert.ToDouble(minValue);
                double maxInt = Convert.ToDouble(minValue);
                double valueInt = Convert.ToDouble(value);
                return (valueInt >= minInt && valueInt <= maxInt) ? trueText : falseText;
            }
            else
            {
                throw new Exception("변환불가능(ToTrueFalseWithMinMax) - value:" + value);
            }

        }

        public static String CustomTextByCorrectValue(object value, string passValue, String trueText, String falseText)
        {
            return (value.ToString().Equals(passValue)) ? trueText : falseText;
        }

        public static String ToTrueFalseWithMinMax(object value, string minValue, string maxValue, StringFormats format = StringFormats.BigFirst)
        {
            if (value is int || value is short || value is byte)
            {
                int minInt = Convert.ToInt32(minValue);
                int maxInt = Convert.ToInt32(minValue);
                int valueInt = Convert.ToInt32(value);
                if (format == StringFormats.BigFirst)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "True" : "False";
                }
                else if (format == StringFormats.Capitals)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "TRUE" : "FALSE";
                }
                else
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "true" : "false";
                }
            }
            else if (value is float || value is double || value is string)
            {
                double minInt = Convert.ToDouble(minValue);
                double maxInt = Convert.ToDouble(minValue);
                double valueInt = Convert.ToDouble(value);
                if (format == StringFormats.BigFirst)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "True" : "False";
                }
                else if (format == StringFormats.Capitals)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "TRUE" : "FALSE";
                }
                else
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "true" : "false";
                }
            }
            else
            {
                throw new Exception("변환불가능(ToTrueFalseWithMinMax) - value:"+value);
            }

        }

        public static String ToTrueFalse(object value, string passValue, StringFormats format = StringFormats.BigFirst)
        {
            if (format == StringFormats.BigFirst)
            {
                return (value.ToString().Equals(passValue)) ? "True" : "False";
            }
            else if (format == StringFormats.Capitals)
            {
                return (value.ToString().Equals(passValue)) ? "TRUE" : "FALSE";
            }
            else
            {
                return (value.ToString().Equals(passValue)) ? "true" : "false";
            }

        }

        public static String ToPassFailWithMinMax(object value, string minValue, string maxValue, StringFormats format = StringFormats.BigFirst)
        {
            if (value is long || value is int || value is short || value is byte)
            {
                int minInt = Convert.ToInt32(minValue);
                int maxInt = Convert.ToInt32(minValue);
                int valueInt = Convert.ToInt32(value);
                if (format == StringFormats.BigFirst)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "Pass" : "Fail";
                }
                else if (format == StringFormats.Capitals)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "PASS" : "FAIL";
                }
                else
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "pass" : "fail";
                }
            }
            else if (value is float || value is double || value is string)
            {
                double minInt = Convert.ToDouble(minValue);
                double maxInt = Convert.ToDouble(minValue);
                double valueInt = Convert.ToDouble(value);
                if (format == StringFormats.BigFirst)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "Pass" : "Fail";
                }
                else if (format == StringFormats.Capitals)
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "PASS" : "FAIL";
                }
                else
                {
                    return (valueInt >= minInt && valueInt <= maxInt) ? "pass" : "fail";
                }
            }
            else
            {
                throw new Exception("변환불가능(ToPassFailWithMinMax) - value :" + value);
            }

        }

        public static String ToPassFail(object value, string passValue, StringFormats format = StringFormats.BigFirst)
        {
            if (format == StringFormats.BigFirst)
            {
                return (value.ToString().Equals(passValue)) ? "Pass" : "Fail";
            }
            else if (format == StringFormats.Capitals)
            {
                return (value.ToString().Equals(passValue)) ? "PASS" : "FAIL";
            }
            else
            {
                return (value.ToString().Equals(passValue)) ? "pass" : "fail";
            }
           
        }

        public static String RunParser(object value, String parserValue){
            int bracketStart = parserValue.IndexOf("(");
            int bracketEnd = parserValue.LastIndexOf(")");

            string funcName = parserValue.Substring(0, bracketStart).Trim();
            string args = parserValue.Substring(bracketStart + 1, (bracketEnd - bracketStart - 1));
            string[] arg = args.Split(",".ToCharArray());
            for (int i = 0; i < arg.Length; i++) arg[i] = arg[i].Trim();//각 arg의 빈칸 뺌..
            return RunParser(value, funcName, arg);
        }

        public static String RunParser(object value, string funcName, params String[] arg){

            switch (funcName)
            {
                case "ToString":
                    return value.ToString();

                case "TrueFalseByMinMax":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 min 이 없습니다.");
                        if (arg.Length < 2 || arg[1].Length == 0) throw new Exception("두 번째 매개변수 max 가 없습니다.");
                        StringFormats format;
                        if (arg.Length < 3 || arg[2].Length == 0)
                        {
                            format = StringFormats.BigFirst;
                        }
                        else
                        {
                            format = (StringFormats)(StringFormatTexts.ToList().IndexOf(arg[2]));
                        }

                        return ToTrueFalseWithMinMax(value, arg[0], arg[1], format);
                    }
                case "TrueFalseByCorrectValue":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 trueValue 가 없습니다.");
                        StringFormats format;
                        if (arg.Length < 3 || arg[2].Length == 0)
                        {
                            format = StringFormats.BigFirst;
                        }
                        else
                        {
                            format = (StringFormats)(StringFormatTexts.ToList().IndexOf(arg[2]));
                        }

                        return ToTrueFalse(value, arg[0], format);
                    }
                case "PassFailByMinMax":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 min 이 없습니다.");
                        if (arg.Length < 2 || arg[1].Length == 0) throw new Exception("두 번째 매개변수 max 가 없습니다.");
                        StringFormats format;
                        if (arg.Length < 3 || arg[2].Length == 0)
                        {
                            format = StringFormats.BigFirst;
                        }
                        else
                        {
                            format = (StringFormats)(StringFormatTexts.ToList().IndexOf(arg[2]));
                        }

                        return ToPassFailWithMinMax(value, arg[0], arg[1], format);
                    }
                case "PassFailByCorrectValue":
                    {
                        if (arg.Length < 1 || arg[0].Length == 0) throw new Exception("첫번째 매개변수 trueValue 가 없습니다.");
                        StringFormats format;
                        if (arg.Length < 3 || arg[2].Length == 0)
                        {
                            format = StringFormats.BigFirst;
                        }
                        else
                        {
                            format = (StringFormats)(StringFormatTexts.ToList().IndexOf(arg[2]));
                        }

                        return ToPassFail(value, arg[0], format);
                    }
                default:
                   throw new Exception("변환불가능 - parser명: Text." + funcName);

            }
        }
    }
}
