using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkPacket;
using XmlDesigner;
using System.Xml;
using XmlHandlers;
using System.Xml.Schema;
using System.IO;
using System.Windows.Forms;

namespace XmlDesigner
{
    public class XmlComConditions
    {
        public static IXmlComponentCondition New()
        {
             
            return new XmlItemCondition(XmlControlHandler.NowEventLoadingXmlItem);
            /*
            switch (XmlControlHandler.NowEventLoadingXmlItem.XmlItemType)
            {
                case XmlItemTypes.ScenarioTable:
                    return new XmlItemCondition(XmlControlHandler.NowEventLoadingXmlItem);
                default:
                    return new TrueCondition();
            }
             */
        }

    }
}
