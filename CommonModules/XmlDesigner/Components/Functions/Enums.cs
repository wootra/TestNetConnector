using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner
{
    public enum XmlItemTypes { ScenarioTable, Layout, LayoutCollection, Label, TextBox, TreeView, Button, ImageButton, Graph, TabControl, Tab, ImageList, Condition, ActionList, Action, EventList, Event, Led, TextArea, Custom };
    public enum XmlConditionTypes { And, Or, Equal, More, MoreOrEqual, True, False};
    public enum EventTypes { OnClick, OnDoubleClick, OnRightClick, OnTextChanged, OnChecked, OnIndexChanged };
    public enum PanelTypes { Panel, Flow };
}
