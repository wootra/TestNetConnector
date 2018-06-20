using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlDesigner;

namespace TestNgineData.PacketDatas
{
    public interface IXmlMatchInfo :IXmlItem
    {
        String TargetName { get; }
        IXmlComponent Component { get; }
        object GetValue(params object[] args);
        void SetValue(params object[] args);
    }
}
