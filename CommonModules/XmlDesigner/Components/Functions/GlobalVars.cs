using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using XmlDesigner.ForEvents;

namespace XmlDesigner
{
    public class GlobalVars
    {
        static String[] _contentAlignments = new String[]{"TopLeft","TopCenter","TopRight","MiddleLeft","MiddleCenter","MiddleRight","BottomLeft","BottomCenter","BottomRight"};
        static int[] _conetentAlignsNumber = new int[] { 1, 2, 4, 16, 32, 64, 256, 512, 1024 };
        public static String[] TableTextAlignModes = new String[] { "Right", "Left", "NumberOnlyRight", "Center", "NumberRightTextCenter", "None" };
        public static String[] ImageLayouts = new String[] { "None", "Tile", "Center", "Stretch", "Zoom" };
        //public static String[] EventTypes = new String[] { "OnClick", "OnDoubleClick", "OnRightClick", "OnTextChanged", "OnChecked", "OnIndexChanged" };
        
        public static String[] XmlItemTypes = new String[]{ "Table", "Layout", "LayoutCollection", "Label", "TextBox", "TreeView", "Button", "ImageButton", "Graph", "TabControl", "Tab", "ImageList", "Condition", "ActionList", "Action", "EventList", "Event" };
        //public static String BasePath = "";
        public static String Seperator = "/";
        //public static String PathSeperator = "/";
        
        public static ContentAlignment ContentAlignment(String text)
        {
            int index =  _contentAlignments.ToList().IndexOf(text);
            if (index < 0) return System.Drawing.ContentAlignment.MiddleLeft;
            else return (ContentAlignment)_conetentAlignsNumber[index];
        }
        public delegate void func(IXmlItem sender, object[] args);
        public delegate void ActionTrigger(IXmlItem sender, String target, object[] args);
        public static Dictionary<String, func> Funcs = new Dictionary<string, func>();
        public static Dictionary<String, ActionTrigger> ActionTriggers = new Dictionary<string, ActionTrigger>();

        public static Dictionary<String, IXmlComponent> Components = new Dictionary<string, IXmlComponent>();

        static Dictionary<String, VariableDefinition> _variables = new Dictionary<string, VariableDefinition>();
        
        public static Dictionary<String, object> GlobalVariables = new Dictionary<string, object>();

        public static void Clear()
        {
            _variables.Clear();
        }
        public static bool ContainsKey(String name)
        {
            return _variables.ContainsKey(name);
        }
        public static VariableDefinition Var(String name)
        {
            return _variables[name];
        }

        public static int Count { get { return _variables.Count; } }

        public static Dictionary<String, VariableDefinition>.ValueCollection.Enumerator Vars
        {
            get { return _variables.Values.GetEnumerator(); }
        }

        public static Dictionary<String, VariableDefinition>.KeyCollection.Enumerator Keys
        {
            get { return _variables.Keys.GetEnumerator(); }
        }
    }


    public class VariableDefinition
    {
        public String Name;
        public Type Type;
        public object Value;
        public object RelativeObject;
        public VariableDefinition()
        {

        }
    }
}
