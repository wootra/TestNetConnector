using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XmlHandlers
{
    public class ValueParser
    {
        public static Brush StringToBrush(String colorString)
        {
            return new SolidBrush(StringToColor(colorString));
        }

        public static Boolean IsTrue(String attr)
        {
            if (attr.Equals("true") || attr.Equals("1")) return true;
            else return false;
        }

        public static string[] GetArgs(String parserName, String argsText)
        {
            string[] token = argsText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<String> args = new List<string>();
            args.Add(parserName);

            for (int i = 0; i < token.Length; i++)
            {
                if (token[i].ToUpper().Equals("NULL")) args.Add(null);
                else args.Add(token[i]);
            }
            return args.ToArray();

        }

        public static string[] getArgs(String argsText)
        {
            string[] token = argsText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<String> args = new List<string>();
            
            for (int i = 0; i < token.Length; i++)
            {
                if (token[i].ToUpper().Equals("NULL")) args.Add(null);
                else args.Add(token[i]);
            }
            return args.ToArray();

        }

        public static Color StringToColor(String colorString)
        {
            if (colorString.Length > 0 && colorString[0] == '#')
            {
                try
                {
                    int rr = int.Parse(colorString.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    int gg = int.Parse(colorString.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    int bb = int.Parse(colorString.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    return Color.FromArgb(rr, gg, bb);
                }
                catch
                {
                    throw new Exception("색깔정의를 Parsing할 수 없습니다 : " + colorString + "\r\n 옳은 예> Red 또는 #FF0000");
                }
            }
            else if (colorString.Length == 0)
            {
                throw new Exception("Color 정의가 비어있습니다.");
            }
            else
            {
                try
                {
                    return Color.FromName(colorString);
                    
                }
                catch
                {
                    throw new Exception("색깔정의를 Parsing할 수 없습니다 : " + colorString + "\r\n 옳은 예> Red 또는 #FF0000");

                }
            }
        }

        /// <summary>
        /// padding을 가져옵니다. paddingText의 형식은 1개의 int나 int,int,int,int 형식입니다.
        /// </summary>
        /// <param name="paddingText">padding문자열</param>
        /// <returns>Padding 형식</returns>
        public static Padding Padding(String paddingText)
        {
            if (paddingText.Length == 0) return new Padding(0);

            string[] paddings = paddingText.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (paddings.Length == 0) return new Padding(0);

            if (paddings.Length == 1)
            {
                int padInt;
                if (int.TryParse(paddingText, out padInt))
                {
                    return new Padding(padInt);
                }
                else
                {
                    throw new Exception(paddingText + "를 Padding으로 바꿀 수 없습니다.");
                }
            }
            else
            {
                int[] padding = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    padding[i] = int.Parse(paddings[i % paddings.Length]); //반복됨.
                }
                return new Padding(padding[0], padding[1], padding[2], padding[3]);
            }
            

        }

        public static String[] DockStyles = new String[] { "None", "Top", "Bottom", "Left", "Right", "Fill" };
        public static DockStyle GetDockStyle(string dockText)
        {

            int dockInt = -1;
            for (int i = 0; i < DockStyles.Length; i++)
            {
                if(DockStyles[i].ToLower().Equals(dockText.ToLower())) return (DockStyle)i;
            }
            return DockStyle.None;
        }

        public static AnchorStyles GetAnchorStyles(string anchorText)
        {
            string[] token = anchorText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (token.Length == 0) return AnchorStyles.Top | AnchorStyles.Left;
            else
            {
                AnchorStyles style = AnchorStyles.None;
                for (int i = 0; i < token.Length; i++)
                {
                    switch (token[i])
                    {
                        case "None":
                            style = style | AnchorStyles.None;
                            break;
                        case "Top":
                            style = style | AnchorStyles.Top;
                            break;
                        case "Bottom":
                            style = style | AnchorStyles.Bottom;
                            break;
                        case "Left":
                            style = style | AnchorStyles.Left;
                            break;
                        case "Right":
                            style = style | AnchorStyles.Right;
                            break;
                    }

                }
                return style;
            }
        }
    }
}
