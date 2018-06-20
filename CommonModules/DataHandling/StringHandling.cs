using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataHandling
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
            if (arr == null) return "";
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

        /// <summary>
        /// strip brackets from string..
        /// </summary>
        /// <param name="str">{} or [] or ()</param>
        /// <param name="stripAll">all bracket in the string will be stripped..</param>
        /// <returns>string that is stripped the brackets</returns>
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
        public static String removeWhiteSpace(String str)
        {
            str = str.Replace(" ", "");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            str = str.Replace("\t", "");
            return str;
        }

        /// <summary>
        /// name 안에 이름sep숫자형식 예)sep이 _일때, name_0 형식으로 되어있으면 그 다음숫자로 numbering을 한다.
        /// 만일 그 형식이 아니면 _0을 붙여준다.
        /// </summary>
        /// <param name="nameList">중복된 이름이 들어있는 리스트</param>
        /// <param name="nameToNumbering">형식이 들어있거나 형식을 만들어줄 이름</param>
        /// <param name="sep">숫자 앞에 붙일 구분자 예> "_"</param>
        /// <returns>완성된 이름. </returns>
        public static String getNewNameWithNumber(ICollection<String> nameList, String nameToNumbering, String sep)
        {
            int num = 0;
            string newName;
            
            if (nameToNumbering.LastIndexOf("_") > 0)
            {
                int underbar = nameToNumbering.LastIndexOf("_");

                string name = nameToNumbering.Substring(0, underbar);
                

                if (nameToNumbering.Length > underbar + 1)
                {//_뒤에 숫자가 있다.
                    
                    string numStr = nameToNumbering.Substring(underbar + 1);

                    if (int.TryParse(numStr, out num))
                    {
                        num++;
                        newName = name + "_"+ (num);
                        while (nameList.Contains(newName))
                        {
                            num++;
                            newName = name + "_"+ (num);
                        }
                        return newName;
                    }
                }
            }

            num=0;
            newName = nameToNumbering + "_" + (num);
            while (nameList.Contains(newName))
            {
                num++;
                newName = nameToNumbering + "_"+ (num);
            }
            return newName;
            
        }

        
    }

}
