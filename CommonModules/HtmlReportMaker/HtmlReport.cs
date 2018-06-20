using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlHandlers;
using System.Drawing;
using System.Data;

namespace HtmlReportMaker
{
    public class HtmlReport
    {
        List<IHtmlReportElement> _elements = new List<IHtmlReportElement>();

        public HtmlReport()
        {
            
        }

        public HtmlTable AddTable(ICollection<int> columnWidths, ICollection<string> columnNames = null, int baseRowHeight = 30)
        {
            HtmlTable table = new HtmlTable(columnWidths, columnNames, baseRowHeight );
            _elements.Add(table);
            return table;
        }

        public String GetHtml()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement html = XmlAdder.Element(xDoc, "Html");
            XmlElement head = XmlAdder.Element(xDoc, "Head", html);
            XmlElement meta = XmlAdder.Element(xDoc, "META", head);
            XmlAdder.Attribute(xDoc, "HTTP-EQUIV", "utf-8", meta);
            //<meta charset="utf-8" />

            meta = XmlAdder.Element(xDoc, "META", head);
            XmlAdder.Attribute(xDoc, "http-equiv", "Content-Type", meta);
            XmlAdder.Attribute(xDoc, "content", "text/html; charset=utf-8", meta);
            //http-equiv="Content-Type" content="text/html; charset=utf-8" 
            
            XmlElement title = XmlAdder.Element(xDoc, "Title", head);
            title.InnerText = "";
            XmlElement body = XmlAdder.Element(xDoc, "Body", html);
            foreach (IHtmlReportElement ele in _elements)
            {
                ele.GetXml(xDoc, body);
            }
            return xDoc.InnerXml;
            //return GetFormatedXml(xDoc.InnerXml);
        }

        string GetFormatedXml(String innerXml)
        {

            int index = 0;
            string findToken = "<";
            int openLevel = 0;
            int startIndex = 0;
            int openIndex = 0;
            int closeIndex = -1;
            string newStr = "";
            bool inComment = false;
            Stack<String> tagName = new Stack<string>();
            string lastTag = "";

            while ((index = innerXml.IndexOf(findToken, startIndex)) >= 0)
            {
                if (findToken.Equals("<"))
                {
                    startIndex = index + 1;


                    findToken = ">";

                    openIndex = index;
                    if (closeIndex > 0) //innerText를 가져온다.
                    {
                        int innerTextSize = (openIndex - 1) - (closeIndex + 1) + 1;
                        if (innerTextSize > 0)
                        {
                            newStr += innerXml.Substring(closeIndex + 1, innerTextSize);
                        }
                    }
                    if (innerXml.Substring(openIndex + 1, 3).Equals("!--")) //comment 시작
                    {
                        inComment = true;
                        lastTag = "Comment";
                    }

                    continue;
                }
                else if (findToken.Equals(">"))
                {
                    startIndex = index + 1;

                    //oldIndex = index+1;
                    if (inComment)
                    {
                        if (innerXml.Substring(index - 2, 2).Equals("--"))
                        {
                            if (newStr.Length != 0)
                            {
                                newStr += "\r\n";

                            }
                            newStr += GetIndents(openLevel);
                            newStr += innerXml.Substring(openIndex, index - openIndex + 1);
                            findToken = "<";
                            inComment = false;
                            closeIndex = index;
                        }
                    }
                    else //comment전에는 comment 끝나기 전까지는 무시한다.
                    {
                        closeIndex = index;

                        if (innerXml[index - 1] == '/') //한줄 끝 태그..
                        {
                            //level--;
                            if (newStr.Length != 0)
                            {
                                newStr += "\r\n";

                            }
                            newStr += GetIndents(openLevel);
                            lastTag = getTagName(innerXml, openIndex, closeIndex);
                        }
                        else if (innerXml[openIndex + 1] == '/') //멀티 끝 태그..
                        {
                            openLevel--;
                            string openTagName = tagName.Pop();
                            if (openTagName.Equals(lastTag) == false)
                            {
                                if (newStr.Length != 0)
                                {
                                    newStr += "\r\n";

                                }
                                newStr += GetIndents(openLevel);
                            }
                        }
                        else if (innerXml[openIndex + 1] == '?' && innerXml[index - 1] == '?')
                        {
                            //newStr += GetIndents(openLevel);
                            lastTag = "Opening";
                        }
                        else //일반 여는 태그
                        {
                            if (newStr.Length != 0)
                            {
                                newStr += "\r\n";

                            }
                            newStr += GetIndents(openLevel);
                            openLevel++;
                            lastTag = getTagName(innerXml, openIndex, closeIndex);
                            tagName.Push(lastTag);
                        }

                        newStr += innerXml.Substring(openIndex, index - openIndex + 1);

                        findToken = "<";
                    }
                    continue;
                }


            }
            //str += str.Substring(openIndex, str.Length - openIndex - 1);

            return newStr;
        }

        private string getTagName(string str, int openIndex, int closeIndex)
        {
            return str.Substring(openIndex + 1, closeIndex - openIndex - 1);
        }

        private string GetIndents(int level)
        {
            string indent = "";
            for (int i = 0; i < level; i++)
            {
                indent += "\t";
            }
            return indent;
        }
    }


   







    
}
