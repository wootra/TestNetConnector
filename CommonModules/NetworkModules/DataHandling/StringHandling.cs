using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetworkModules
{
    public static class StringHandling
    {
        public static String[] SepWithStr(String str, String seperator, Boolean isTrimWhite)
        {
            int sepPos;
            int sepSize = seperator.Length;
            List<String> sep = new List<String>();
            while ((sepPos = str.IndexOf(seperator)) >= 0)
            {
                if (isTrimWhite)
                {
                    sep.Add(str.Substring(0, sepPos).Trim());
                    str = str.Substring(sepPos + sepSize).Trim();
                }
                else
                {
                    sep.Add(str.Substring(0, sepPos));
                    str = str.Substring(sepPos + sepSize);
                }
            }
            if (isTrimWhite) //마지막에 남은 부분을 추가한다.
            {
                sep.Add(str.Trim());
            }
            else
            {
                sep.Add(str);
            }
            return sep.ToArray();
        }

        public static String[] SepWithChars(String str, String seperators, Boolean isTrimWhite)
        {
            int sepPos=0;
            int sepSize = seperators.Length;
            List<String> sep = new List<String>();
            while ((sepPos = findPosition(str, seperators, sepPos)) >= 0)
            {
                if (isTrimWhite)
                {
                    sep.Add(str.Substring(0, sepPos).Trim());
                    str = str.Substring(sepPos + sepSize).Trim();
                }
                else
                {
                    sep.Add(str.Substring(0, sepPos));
                    str = str.Substring(sepPos + sepSize);
                }
            }
            if (isTrimWhite) //마지막에 남은 부분을 추가한다.
            {
                sep.Add(str.Trim());
            }
            else
            {
                sep.Add(str);
            }
            return sep.ToArray();
        }
        public static int findPosition(String str, String findChars, int startIndex)
        {
            for (int i = startIndex; i < findChars.Length; i++)
            {
                if (str.IndexOf(findChars[i], startIndex) >= 0) return i;
            }
            return -1;
        }

        public static int findNthPosition(string str, string findingStr, int nthNumber)
        {
            int findPos=0;
            int count =0;
            while ((findPos = str.IndexOf(findingStr, findPos)) >= 0)
            {
                if (++count == nthNumber) return findPos;
            }
            return -1;
        }
        public static int countStrInStr(string str, string findingStr, int startIndex)
        {
            int findPos = 0;
            int count = 0;
            while (findPos<str.Length && (findPos = str.IndexOf(findingStr, findPos)+1) >= 0) ++count;
            
            return count;
        }

        public static int countCharsInStr(string str, string findingChars, int startIndex)
        {
            int count = str.Substring(startIndex).Split(findingChars.ToCharArray()).Length;
            return count;
        }
        public static String concatArr(Array arr, String seperator = "")
        {
            if (arr.Length < 1) return "";
            Boolean isNumber = false;
            int test = 0;
            int arrLen = arr.Length;
            int arrTypeSize = 1;
            if (Int32.TryParse(arr.GetValue(0).ToString(), out test))
            {
                isNumber = true;
                arrTypeSize = Marshal.SizeOf(test);
            }

            String concatStr = "";

            for (int i = 0; i < arrLen; i++)
            {
                if (isNumber) concatStr += String.Format("{0:X" + arrTypeSize * 2 + "}", arr.GetValue(i));
                else concatStr += arr.GetValue(i).ToString();

                if (i + 1 < arrLen) concatStr += seperator;
            }

            return concatStr;
        }
        public static String stripBracket(String str, Boolean stripAll=false)
        {
            do
            {
                if (str.IndexOf('{') >= 0)
                {
                    int nameStart = str.IndexOf('{') + 1;
                    int nameLength = str.LastIndexOf('}') - nameStart;
                    str = str.Substring(nameStart, nameLength);
                }
                else if (str.IndexOf('[') >= 0)
                {
                    int nameStart = str.IndexOf('[') + 1;
                    int nameLength = str.LastIndexOf(']') - nameStart;
                    str = str.Substring(nameStart, nameLength);
                }
                else if (str.IndexOf('(') >= 0)
                {
                    int nameStart = str.IndexOf('(') + 1;
                    int nameLength = str.LastIndexOf(')') - nameStart;
                    str = str.Substring(nameStart, nameLength);
                }
                else
                {
                    break;
                }
            } while (stripAll);

            return str;
        }
    }

}
