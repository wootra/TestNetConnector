using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner.PacketDatas
{
    public interface IScenarioItem
    {
        String Name { get; set; }
        ScenarioItemTypes ScenarioItemType { get; }
    }
    public enum ScenarioItemTypes{Command=0, Scenario, CommandGroup};
}
