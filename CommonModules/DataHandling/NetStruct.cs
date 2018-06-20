using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling
{
    public class NetStruct
    {
        public List<string> names = new List<string>();
        public List<string> types = new List<string>();
        public List<int> sizes = new List<int>();
        public List<string> init = new List<string>();
        public String errStr = "";
        public String nativeText = "";

        public int Count { get { return names.Count; } }

        public NetStructItem getItem(int num){
            return new NetStructItem(names[num], types[num], sizes[num]);
        }

        public void setNativeText(string str)
        {
            nativeText = str;
            parsing();
        }

        String removeWhiteSpace(String line)
        {
            line = line.Replace("\n", "");
            line = line.Replace("\r", "");
            line = line.Trim();//빈칸과 라인피드를 모두 삭제
            return line;
        }

        public NetStruct parsing(String str = null)
        {
            if (str == null || str.Length == 0)
            {
                str = nativeText;
            }
            else
            {
                nativeText = str;
            }


            NetStruct ns = new NetStruct();
            
            String[] lines = str.Split(";".ToCharArray()); //각 라인으로 나누어서
            
            for (int i = 0; i < lines.Length; i++)
            {//각 라인들
                String line = lines[i];

                line = removeWhiteSpace(line);

                if(line.Length==0) continue;

                int firstSpace = -1;
                int secondSpace = -1;
                int thirdSpace = -1;
                int typeSpace = -1;
                try
                {
                    firstSpace = line.IndexOf(' ');
                    secondSpace = line.IndexOf(' ', firstSpace);
                    thirdSpace = line.IndexOf(' ', secondSpace);
                }
                catch { }
                if (firstSpace < 0) firstSpace = line.IndexOf('[');
                if (firstSpace < 0) firstSpace = line.IndexOf('=');
                if (firstSpace < 0) firstSpace = line.Length;
                String type = line.Substring(0, firstSpace);

                if (TypeHandling.getTypeFromTypeName(type) == null)
                {//첫번째 토큰에서 제대로된 타입이 검출안되면
                    if (secondSpace > 0) type = line.Substring(0, secondSpace); //두번째 검색
                    else
                    {
                        setError("타입정의가 맞지 않습니다.", i, line); //두번째 스페이스가 없으면 에러
                        return null;
                    }

                    if (TypeHandling.getTypeFromTypeName(type) == null)
                    {//두번째 토큰에서 타입틀리면
                        if (thirdSpace > 0) type = line.Substring(0, thirdSpace);//세번째 검색
                        else
                        {
                            setError("타입정의가 맞지 않습니다.", i, line);//세번째 스페이스 없으면 에러
                            return null;
                        }

                        if (TypeHandling.getTypeFromTypeName(type) == null)
                        { //세번째 토큰에서도 타입이 틀리다면
                            setError("타입정의가 맞지 않습니다.", i, line); //무조건 에러
                            return null;
                        }
                        else
                        {
                            typeSpace = thirdSpace;
                        }
                    }
                    else
                    {
                        typeSpace = secondSpace;
                    }
                }
                else
                {
                    typeSpace = firstSpace;
                }

                String rest = line.Substring(typeSpace);

                rest = rest.Replace(" ", ""); //나머지에서는 빈칸이 필요없다.

                //초기값 입력받기
                String initValue = "0"; //기본값은 0이다.
                String[] token = rest.Split("=".ToCharArray());
                int openBracket;
                int closeBracket;
                if (token.Length == 2)
                {
                    initValue = token[1];
                }
                else if (token.Length > 2)
                {
                    setError("= 이 2번 이상 들어갑니다.", i, line);
                    return null;
                }
                
                rest = token[0]; //=오른쪽의 값은 잘라버림.
                //배열검사
                openBracket = rest.IndexOf('[');
                closeBracket = rest.IndexOf(']');

                int arrSize = 1;

                if (openBracket > 0 || closeBracket > 0) //배열인지 검사하여
                {
                    if (openBracket < 0 || closeBracket < 0)
                    {
                        setError("배열을 나타내는 [,] 기호 둘 중 하나가 없습니다", i, line);
                        return null;
                    }
                    String numStr = rest.Substring(openBracket + 1, closeBracket - openBracket - 1);
                    
                    if (numStr.Length == 0)
                    {
                        arrSize = initValue.Split(",".ToCharArray()).Length;
                        rest = rest.Substring(0, openBracket); //배열 크기를 가져왔으므로 배열기호그룹 삭제
                    }
                    else
                    {
                        int num = -1;
                        if (Int32.TryParse(numStr, out num) == false)
                        {
                            setError("배열기호[] 안에는 정수가 와야합니다.", i, line);
                            return null;
                        }
                        else
                        {
                            if (num <= 0)
                            {
                                setError("배열의 크기는 1보다 작을 수 없습니다.", i, line);
                                return null;
                            }
                            else
                            {
                                arrSize = num;
                                rest = rest.Substring(0, openBracket); //배열 크기를 가져왔으므로 배열기호그룹 삭제
                            }
                        }
                    }
                } //배열검사 끝.
                
                //초기값 검사
                openBracket = initValue.IndexOf('{');
                closeBracket = initValue.IndexOf('}');
                if (openBracket >= 0 || closeBracket >= 0 || initValue.IndexOf(',')>=0) //배열형식의 초기값이라면
                {
                    if (openBracket < 0 || closeBracket < 0)
                    {
                        setError("{ 나 } 중에서 하나가 없습니다.", i, line);
                        return null;
                    }
                    String numStr = initValue.Substring(openBracket + 1, closeBracket - openBracket - 1); //브래킷 내부의 내용 가져옴
                    token = numStr.Split(",".ToCharArray());
                    
                    if (token.Length > arrSize) //배열의 크기보다 클 때
                    {
                        setError("배열의 크기를 넘어서 초기화를 시도했습니다. size:" + arrSize + "  this:" + token.Length, i, line);
                        return null;
                    }

                    for (int j = 0; j < token.Length; j++) //초기값들의 타입을 검사한다.
                    {
                        if (TypeHandling.isValidType(token[j], type) == false)
                        {
                            setError("초기값이 타입과 다릅니다.Type:"+type + "  value:"+token[j], i, line);
                            return null;
                        }
                    }
                }else if (TypeHandling.isValidType(initValue, type) == false) //배열형식이 아니라 단순값일 때
                {
                    setError("초기값이 타입과 다릅니다.", i, line);
                    return null;
                }
                //초기값 검사 끝

                //변수명검사
                for (int j = 0; j < rest.Length; j++) //변수명 검사
                {
                    if (Char.IsLetterOrDigit(rest[j]) == false)
                    {
                        setError("변수명에는 기호가 들어갈 수 없습니다.", i, line);
                        return null;
                    }
                    else if (j == 0 && Char.IsDigit(rest[j]))
                    {
                        setError("변수명의 첫번째에는 숫자가 들어갈 수 없습니다.", i, line);
                        return null;
                    }
                }//변수명 검사 끝
                if (rest.Length == 0) rest = "var" + i;
                ns.names.Add(rest);
                ns.sizes.Add(arrSize);
                ns.types.Add(type);
                ns.init.Add(initValue);

            }//각 라인들 검색 끝
            this.names.Clear();;
            this.sizes.Clear();
            this.types.Clear();
            this.init.Clear();

            this.names = ns.names;
            this.sizes = ns.sizes;
            this.types = ns.types;
            this.init = ns.init;
            return ns;
        }

        void setError(string errMsg, int line, String lineStr)
        {
            this.errStr = errMsg + " Line(" + line + ") in \r\n" + lineStr;
            throw new Exception(this.errStr);
        }
    }
    public class NetStructItem
    {
        public string name;
        public string type;
        public int size;
        public NetStructItem(String name, String type, int size)
        {
            this.name = name;
            this.type = type;
            this.size = size;
        }
    }
}
