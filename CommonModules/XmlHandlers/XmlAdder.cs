using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace XmlHandlers
{
    public class XmlAdder
    {
        public static XmlAttribute Attribute(XmlDocument xDoc, String attrName, String value, XmlNode parent=null)
        {
            try
            {
                XmlAttribute attr = xDoc.CreateAttribute(attrName);
                attr.Value = value;
                if (parent != null) parent.Attributes.Append(attr);
                return attr;
            }
            catch (Exception ex)
            {
                throw new Exception("XmlAdder.Attribute : " + attrName + "::\r\n" + ex.Message, ex);
            }
            
        }
        public static XmlElement Element(XmlDocument xDoc, String elementName, String InnerText, XmlNode parent=null)
        {
            try{
                XmlElement element = xDoc.CreateElement(elementName);
                element.InnerText = InnerText;
                if (parent != null) parent.AppendChild(element);
                return element;
            }
            catch (Exception ex)
            {
                throw new Exception("XmlAdder.Element : " + elementName +"<="+InnerText+ "::\r\n" + ex.Message, ex);
            }
        }

        public static XmlElement Element(XmlDocument xDoc, String elementName, XmlNode parent=null)
        {
            try
            {
                XmlElement element = xDoc.CreateElement(elementName);
                if (parent != null) parent.AppendChild(element);
                else xDoc.AppendChild(element);
                return element;
            }
            catch (Exception ex)
            {
                throw new Exception("XmlAdder.Element : " + elementName + "::\r\n" + ex.Message, ex);
            }
        }

        public static XmlComment Comment(XmlDocument xDoc, String commentString, XmlNode parent = null)
        {
            try
            {
                XmlComment comment = xDoc.CreateComment(commentString);
                if (parent != null) parent.AppendChild(comment);
                else xDoc.AppendChild(comment);
                return comment;
            }
            catch (Exception ex)
            {
                throw new Exception("XmlAdder.Element : " + commentString + "::\r\n" + ex.Message, ex);
            }   
        }
    }
}
