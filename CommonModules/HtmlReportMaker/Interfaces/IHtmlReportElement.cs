using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HtmlReportMaker
{
    internal interface IHtmlReportElement
    {
        void GetXml(XmlDocument xDoc, XmlNode parent);
    }

}
