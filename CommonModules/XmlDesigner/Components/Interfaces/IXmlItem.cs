using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public interface IXmlItem
    {
        //XmlItemTypes XmlItemType { get; }
        XmlItemInterface Interface { get; set; }
        
        void LoadXml(String xmlFile, bool refLoad = false);
        void LoadXml(XmlDocument xDoc, XmlNode rootNode, bool refLoad = false);
        void SaveXml(String xmlFile);
        String FilePath { get; }
        XmlNode GetXml(XmlDocument xDoc, XmlNode parent=null);
    }
}
