using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;

namespace DataHandling
{
    public class XmlHandling
    {
        XmlReaderSettings _setting;
        XmlElement _xMain;
        List<XmlNode> _nodeItems = new List<XmlNode>();
        

        public XmlHandling()
        {
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.IgnoreComments = true;
            setting.IgnoreProcessingInstructions = true;
            setting.IgnoreWhitespace = true;
            _setting = setting;
        }
        public void openXmlDoc(String xmlFileName,String directory=null)
        {
            XmlDocument xFile = new XmlDocument();
            if (directory == null) directory = Directory.GetCurrentDirectory();
            try
            {
                xFile.Load(directory+"\\"+xmlFileName);
                _xMain = xFile.DocumentElement;   
            }
            catch (Exception e)
            {
                throw new Exception("Error when load card command... \n CommandListHandler.insertCommands()\n" + e.ToString());
            }
            
        }

        public List<XmlNode> getChilds(XmlNode parent = null,String name=null)
        {
            XmlNodeList xElements;// = xCommandsFile.DocumentElement;
            if (parent == null) xElements = _xMain.ChildNodes;
            else xElements = parent.ChildNodes;

            _nodeItems.Clear();
            List<XmlNode> aList = new List<XmlNode>();

            foreach (XmlNode aNode in xElements)
            {
                if (name == null) aList.Add(aNode);
                else if(aNode.Name.Equals(name)) aList.Add(aNode);
                _nodeItems.Add(aNode);
                
            }
            return aList;
        }

        public List<XmlNode> getChildsInSameParent(String name=null)
        {
            List<XmlNode> aList = new List<XmlNode>();

            foreach (XmlNode aNode in _nodeItems)
            {
                if (name == null) aList.Add(aNode);
                else if (aNode.Name.Equals(name)) aList.Add(aNode);
                _nodeItems.Add(aNode);

            }
            return aList;
        }

        public Boolean hasChildren(XmlNode aNode)
        {
            return aNode.ChildNodes.Count > 0;
        }

        public String getAttribute(XmlNode aNode, String attrName)
        {
            return aNode.Attributes[attrName].Value;
        }

        public String getContent(XmlNode aNode)
        {
            return aNode.InnerText;
        }

        
    }
}
