using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
//using System.Xml.Linq;
using System.Xml.Schema;

using System.Data;
using System.IO;


namespace XmlHandlers
{
    public class XmlGetter
    {
        public static XmlNode FirstChild(XmlNode node)
        {
            XmlNode child;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                child = node.ChildNodes[i];
                if (child.NodeType == XmlNodeType.Element) return child;
            }
            return null;
        }


        public static XmlNode FirstChild(XmlDocument doc)
        {
            XmlNode child;
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                child = doc.ChildNodes[i];
                if (child.NodeType == XmlNodeType.Element) return child;
            }
            return null;
        }

        public static String Attribute(XmlNode node, String attributeName, bool ignoreCase=true)
        {
            if (node!=null && node.Attributes != null)
            {
                
                XmlAttribute attr;
                if (ignoreCase)
                {
                    foreach (XmlAttribute attrItem in node.Attributes)
                    {
                        if (attrItem.Name.ToLower().Equals(attributeName.ToLower())) return attrItem.Value;
                    }
                }
                else
                {
                    attr = node.Attributes[attributeName];
                    if (attr != null) return attr.Value;
                }
            }
            return "";
        }

        public static String InnerText(XmlNode node)
        {
            if (node.InnerText == null) return "";
            else return node.InnerText;
        }

        

        public static XmlNode RootNode(out XmlDocument xDoc, String xmlFile, String schemaPath=null, ValidationEventHandler eventHandler=null)
        {
            
            xDoc = new XmlDocument();
            xDoc.PreserveWhitespace = false;
            if (schemaPath != null)
            {
                xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();
                XmlSchema schema = XmlSchema.Read(File.OpenRead(schemaPath), eventHandler);
                xDoc.Schemas.Add(schema);

               
            }
            xDoc.Load(xmlFile);
            return FirstChild(xDoc);
            /*
            String rootName= getFirstChild(_xDoc).Name;

            return _xDoc.SelectSingleNode("//"+rootName);
             */
        }
        XmlNode _rootNode = null;

        public XmlGetter(XmlNode rootNode)
        {
            _rootNode = rootNode;
        }
        public XmlNode Child(String childName)
        {
            return Child(_rootNode, childName);
        }

        public XmlNodeList Children(String childName)
        {
            return Children(_rootNode, childName);
        }


        public static XmlNode Child(XmlNode baseNode, String childPath, bool ignoreCase=true)
        {
            if (baseNode == null) return null;
            XmlNode node = baseNode.CloneNode(false);
            node.RemoveAll();
            String childName;
            String rest;

            int firstSep = childPath.IndexOf("/");
            if (firstSep > 0)
            {
                childName = childPath.Substring(0, firstSep);
                rest = childPath.Substring(firstSep + 1);
            }
            else
            {
                childName = childPath;
                rest = "";
            }


            for (int i = 0; i < baseNode.ChildNodes.Count; i++)
            {
                XmlNode child = baseNode.ChildNodes[i];
                if (ignoreCase)
                {
                    if (child.Name.ToLower().Equals(childName.ToLower()))
                    {
                        if (rest.Length > 0)
                        {
                            return Child(child, rest, ignoreCase);

                        }
                        else
                        {
                            return child;
                        }
                    }
                }
                else
                {
                    if (child.Name.Equals(childName))
                    {
                        if (rest.Length > 0)
                        {
                            return Child(child, rest, ignoreCase);

                        }
                        else
                        {
                            return child;
                        }
                    }
                }
            }
            return null;
        }
        
        public static XmlNodeList Children(XmlNode baseNode, String childPath)
        {
            if (baseNode == null) return new XmlDocument().ChildNodes;

            XmlNode node = baseNode.CloneNode(false);
            node.RemoveAll();
            String childName;
            String rest;

            int firstSep = childPath.IndexOf("/");
            if (firstSep > 0)
            {
                childName = childPath.Substring(0, firstSep);
                rest = childPath.Substring(firstSep + 1);
            }
            else
            {
                childName = childPath;
                rest = "";
            }

            for (int i = 0; i < baseNode.ChildNodes.Count; i++)
            {
                XmlNode child = baseNode.ChildNodes[i].Clone();
                if (child.Name.Equals(childName))
                {
                    if (rest.Length > 0)
                    {
                        XmlNodeList nodes = Children(child, rest);

                        for (int c = 0; c < nodes.Count; c++)
                        {
                            node.AppendChild(nodes[c].Clone());
                        }
                    }
                    else
                    {
                        node.AppendChild(child.CloneNode(true));
                    }
                    
                }
            }
            return node.CloneNode(true).ChildNodes;
        }

    }
}
