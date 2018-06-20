using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace XmlDesigner
{
    public interface IXmlComponent : IXmlItem
    {
        XmlItemTypes XmlItemType { get; }
        Control RealControl { get; }
        Type Type { get; }
        String Name { get; set; }
    }
}
