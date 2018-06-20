using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public interface IXmlComponentCondition : IXmlItem
    {
        bool GetCondition();
    }
}
