using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataHandling;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
namespace CustomParser
{
    [Serializable]
    public class GSTParser
    {
        ContextTree _root;
        GstNode _main;
        
        public GSTParser(String text)
        {


            _root = ParsingHelper.ParseToTree(text);
            GstNode member = new GstNode();
            _main = parseToGst(_root,member);

        }
        public GstNode ContextMain
        {
            get
            {
                return _main;
            }
        }
        public ContextTree ContextRoot
        {
            get
            {
                return _root;
            }
        }

        GstNode parseToGst(ContextTree tree, GstNode member){
            String title;//
            String type, name;
            int size;
            GstNode aNode;

            if (tree.Children.Count == 0) //변수지정이나 할당. 한 줄에 끝나는 구문. GST파일에서는 GST가 유일하다.
            {
                title = tree.Header.Trim();
                name = ParsingHelper.getAllocTokens(title, out type, out size);
                if (type.Contains("struct") && member.Kind == ContextKind.VARIABLE) //depth가 깊어짐.
                {
                    aNode = member.getActiveStruct(type); //같은 struct타입을 가져와서 넣어준다.
                    foreach (GstNode node in aNode.Children)
                    {
                        member.addChild(node);
                    }
                }
                return null;
                
            }
            else if (tree.Header.Trim().Length == 0) //main
            {
                foreach (ContextTree aChild in tree.Children)
                {
                    title = aChild.Header.Trim();                    
                    if (title.Split(" ".ToCharArray()).Length == 2) //struct 
                    {
                        aNode = new GstNode(aChild.Header.Trim(), member);
                        member.addChild(aNode);
                        parseToGst(aChild, aNode);
                    }
                    else //gst
                    {
                        name = ParsingHelper.getAllocTokens(title, out type, out size);
                        aNode = new GstNode(type, name, member);
                        member.addChild(aNode);
                        parseToGst(aChild, aNode);
                    }
                    
                }
            }
            else if (tree.Header.Contains("struct"))//struct
            {

                
                //for (int i = 0; i < tree.Child.Count; i++)
                foreach (ContextTree aChild in tree.Children)
                {
                    title = aChild.Header.Trim();

                    name = ParsingHelper.getAllocTokens(title, out type, out size);
                    

                    if (type.Contains("struct")) //depth가 깊어짐.
                    {
                        aNode = new GstNode(type, name, member);
                        member.addChild(aNode);
                        parseToGst(aChild, aNode);
                    }
                    else //struct가 아닐 경우, 일반 변수나 배열
                    {

                        if (size > 0)//배열
                        {
                            for (int i = 0; i < size; i++)
                            {
                                aNode = new GstNode(member.TypeFromStr(type), name + "[" + i + "]", member);
                                member.addChild(aNode);
                            }
                        }
                        else //일반 변수
                        {
                            aNode = new GstNode(member.TypeFromStr(type), name, member);
                            member.addChild(aNode);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("구문분석 오류..");
            }

            return member;
        }


 
        String replaceSymbols(String orgStr, String exceptionStr="")
        {
            String changeStr = orgStr.Trim();

            if(exceptionStr.Equals("==")==false) changeStr = changeStr.Replace("==", "<---->");
            if (exceptionStr.Equals("!=") == false) changeStr = changeStr.Replace("!=", "<!-->");
            if (exceptionStr.Equals("<=") == false) changeStr = changeStr.Replace("<=", "<<-->");
            if (exceptionStr.Equals(">=") == false) changeStr = changeStr.Replace(">=", "<>-->");
            if (exceptionStr.Equals("=") == false) changeStr = changeStr.Replace("=", "<-->");
            return changeStr;
        }
        String recoverSymbols(String orgStr, String exceptionStr="")
        {
            String changeStr = orgStr.Trim();
            if (exceptionStr.Equals("==") == false) changeStr = changeStr.Replace("<---->","==");
            if (exceptionStr.Equals("!=") == false) changeStr = changeStr.Replace("<!-->","!=");
            if (exceptionStr.Equals("<=") == false) changeStr = changeStr.Replace("<<-->","<=");
            if (exceptionStr.Equals(">=") == false) changeStr = changeStr.Replace("<>-->",">=");
            if (exceptionStr.Equals("=") == false) changeStr = changeStr.Replace("<-->","=");
            return changeStr;
        }
    }



}
