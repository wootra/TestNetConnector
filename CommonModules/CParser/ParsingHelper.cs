using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using System.Text.RegularExpressions;

namespace CustomParser
{
    public class ParsingHelper
    {
        public static int findPair(String str, int openBracketIndex, Char open, Char close)
        {
            int opens = 1;
            

            for (int i = openBracketIndex+1; i < str.Length; i++)
            {
                if (str[i] == open) opens++;
                else if (str[i] == close) opens--;
                if (opens == 0) return i;
            }
            return -1;
        }

        /// <summary>
        /// 구문을 Tree구조로 parsing함.
        /// </summary>
        /// <param name="orgStr">parsing할 구문</param>
        /// <param name="lineFinalizer">구문완료문자. 기본값은 ";".ToCharArray()</param>
        /// <param name="finalBracket">문맥괄호. 기본값은 "{}".ToCharArray()</param>
        /// <returns>Tree구조로 분리된 구문</returns>
        public static ContextTree ParseToTree(String orgStr, Char[] lineFinalizer=null, Char[] finalBracket=null, String lineComment="//", Char lineEndHeader='#')
        {
            int OPEN = 0;
            int CLOSE = 1;
            int seeker = 0;
            orgStr = orgStr.Replace("\r", "");
            orgStr = orgStr.Replace("\t", " ");
            if(finalBracket==null) finalBracket = "{}".ToCharArray();
            if(lineFinalizer == null) lineFinalizer = ";".ToCharArray();
            ContextTree root = new ContextTree(0,0,"");
            ContextTree openNode = root;
            ContextTree newNode = new ContextTree(0,0,"",root);
            ContextTree commentBaseNode = newNode;
            
            String cup="";
            
            bool isBracketClosed = false;
            bool isStringOpen = false;
            int lineIndex = 0;
            while (seeker < orgStr.Length)
            {
                if (orgStr[seeker] == '\n') lineIndex++;
                if (isStringOpen == true) //따옴표의 안에서는 문법이 무의미함. 그냥 진행.
                {
                    if (orgStr[seeker] == '"' && orgStr[seeker - 1] != '\\')
                    {
                        isStringOpen = false;
                    }
                    
                    cup += orgStr[seeker];
                    seeker++;
                    continue;
                }
                if (isStringOpen == false && orgStr[seeker] == '"')
                {
                    isStringOpen = true;
                    seeker++;
                    continue;
                }
                if (cup.Trim().Length == 0 && orgStr[seeker] == lineEndHeader) //이 문자가 라인의 가장 앞에 오면 끝문자와 상관없이 라인을 마무리한다.
                {
                    int endOfLine = orgStr.IndexOf('\n', seeker);
                    newNode.Header = orgStr.Substring(seeker, endOfLine - seeker).Trim();
                    openNode.Children.Add(newNode);
                    newNode = new ContextTree(seeker,lineIndex,"", openNode);
                    seeker = endOfLine + 1;
                    lineIndex++;
                    continue;
                }

                if (orgStr.Length > seeker + 1 && orgStr[seeker] == '/' && orgStr[seeker + 1] == '/') //주석 등장.
                {
                    int endOfLine = orgStr.IndexOf('\n', seeker);
                    if (orgStr.Length > seeker + 2 && orgStr[seeker + 2] == '@') //gst 설명변수. Footer에 저장.
                    {
                        commentBaseNode.Comment = orgStr.Substring(seeker, endOfLine - seeker).Trim();
                        
                    }
                    
                    seeker = endOfLine + 1;
                    lineIndex++;
                    continue;
                }
                
                if (orgStr[seeker] != finalBracket[OPEN] && orgStr[seeker] != finalBracket[CLOSE]) //i
                {
                    
                    cup += orgStr[seeker];
                    if(cup.Trim().Length>0) commentBaseNode = newNode; //한글자라도 있으면 그 다음에 오는 주석은 이 라인을 위한 것이다.
                    if(isBracketClosed==false && ArrayCompare(lineFinalizer, orgStr[seeker])){ //구문완료문자
                        newNode.Header = cup.Trim().Replace("\n"," ");
                        
                        openNode.Children.Add(newNode);
                        newNode = new ContextTree(seeker,lineIndex,"",openNode);
                        cup = "";
                    
                        
                    }
                    
                }
                else if (orgStr[seeker] == finalBracket[OPEN]) //finalbracket이 열릴 때
                {
                    newNode.Header = cup.Trim().Replace("\n", " ");
                    openNode = newNode;

                    newNode = new ContextTree(seeker,lineIndex, "", openNode);
                    cup = "";
                }
                
                else if(isBracketClosed==false) //finalbracket이 닫힐 때
                {
                    isBracketClosed = true; //isBracketClosed는 bracket이 닫히고 그 줄이 끝날때까지를 위한 변수이다. 실제 열리고 닫힌 상태와는 관계없다.
                }
                
                if (isBracketClosed && (ArrayCompare(lineFinalizer, orgStr[seeker]) || orgStr[seeker] == '\n'))
                {
                    openNode.Footer = cup.Trim().Replace("\n"," ");
                    cup = "";
                    openNode.Parent.Children.Add(openNode);
                    if (openNode.Parent != null) openNode = openNode.Parent;
                    else return openNode;
                    newNode = new ContextTree(seeker,lineIndex, "", openNode);
                    isBracketClosed = false;
                }
                seeker++;
                
            }
            return openNode;
        }
        public static bool ArrayCompare(Char[] arr, Char chr)
        {
            for (int i = 0; i < arr.Length; i++) if (arr[i] == chr) return true;
            return false;
        }
        public static bool ArrayCompare(String[] arr, String str)
        {
            for (int i = 0; i < arr.Length; i++) if (arr[i].Equals(str)) return true;
            return false;
        }


        public static String getAllocTokens(String strConsiderArray, out String type, out int innerSize)
        {
            String exStr = @"(?<type>([a-zA-Z_]{1}[a-zA-Z0-9_]*([ ]+[a-zA-Z_]{1}[a-zA-Z0-9_]*)*)?)[ ]+(?<name>([a-zA-Z_]{1}[a-zA-Z0-9_]*))(\[(?<innerNum>([1-9]{1}[0-9]*))\])?"; //변수명

            String num = "([1-9]{1}[0-9]*([.]{1}[0-9]+)?){1}";
            String str = "([\"']{1}.[\"']{1}){1}";
            String equal = "([ ]*=[ ]*){1}";
            String comma = "([ ]*,[ ]*){1}";
            string strOrNum = "(" + str + "|" + num + "){1}";

            //exStr += "("+equal;//할당자
            //exStr += "{(?<inner>([0-9.]+(" + comma +"[0-9.]+)?)?)})?";//할당초기변수
            //exStr += "{(?<inner>"+strOrNum+"("+comma+strOrNum+")?){1}})?";//할당초기변수
            Regex exp = new Regex(exStr);
            Match m = exp.Match(strConsiderArray);
            if (m.Success)
            {
                String name = m.Groups["name"].Value;
                String innerNum = m.Groups["innerNum"].Value;
                if (Int32.TryParse(innerNum, out innerSize) == false) innerSize = -1;

                //String[] inArrayTokens = m.Groups["inner"].Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //if (inArrayTokens != null && inArrayTokens.Length > 0) inner = inArrayTokens;
                //else inner = null;

                type = m.Groups["type"].Value;

                return name;
            }
            else
            {
                type = null;
                //inner = null;
                innerSize = -1;
                return null;
            }
        }


 
    
    }
    
    public class ContextTree:TreeDataNode
    {
        public String Header="";
        public String Footer = "";
        public String Comment = "";
        public int id;
        public new List<ContextTree> Children = new List<ContextTree>();
        public new ContextTree Parent = null;
        public int CharIndex;
        public int LineIndex;
        public ContextTree(int charIndex, int lineIndex, String header, ContextTree parent = null, String name="", Object relativeObject=null):base(name, relativeObject, parent)
        {
            this.CharIndex = charIndex;
            this.Header = header;
            this.Parent = parent;
        }
        public Boolean isFinal()
        {
            return Children.Count == 0;
        }
        /// <summary>
        /// 현재 노드와 상위노드에서 검색하여 해당 이름을 가진 노드를 리턴한다. 없으면 null
        /// 변수를 미리 define했을 때에 그 변수가 해당 범위안에 있는지를 알아내는 함수이다.
        /// </summary>
        /// <param name="name">찾을 Name</param>
        /// <returns>해당 Name을 가진 노드</returns>
        public ContextTree getActiveMember(String name)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Name.Equals(name)) return Children[i];
            }
            ContextTree m = Parent.getActiveMember(name);

            if (m != null) return m;
            else return null;
        }
    }

}
