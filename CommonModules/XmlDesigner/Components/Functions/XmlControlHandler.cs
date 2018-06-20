using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using XmlHandlers;
using XmlDesigner.ForEvents;


namespace XmlDesigner
{
    public class XmlControlHandler
    {
        public static void RunEvent(IXmlItem comp, String evtType)
        {
            if (comp.Interface.Events.Keys.Contains(evtType) == false) return;

            XmlEvent evt = comp.Interface.Events[evtType];
            

            for (int i = 0; i < evt.Actions.Count; i++)
            {
                XmlActionList actionList = evt.Actions[i];
                if (actionList.Condition.GetCondition() && (actionList.ComCondition==null || actionList.ComCondition.GetCondition())) //condition이 true일 때..
                {
                    for (int a = 0; a < actionList.Count; a++)
                    {
                        XmlAction action = actionList[a];
                        string funcName = action.Name;

                        if (GlobalVars.Funcs.Keys.Contains(funcName))
                        {
                            object[] args;
                            args = comp.Interface.GetRealTimeArgs(action.Args);

                            if (comp is Control)
                            {
                                Control control = comp as Control;

                                if (control.InvokeRequired)
                                {
                                    GlobalVars.Funcs[funcName](comp, args);
                                }
                                else
                                {
                                    GlobalVars.Funcs[funcName].Invoke(comp, args);
                                }
                            }
                            else
                            {
                                GlobalVars.Funcs[funcName](comp, args);
                            }
                        }
                    }
                }

            }
        }

       

        public static IXmlComponent GetComponentByName(String name, IXmlComponent baseComponent)
        {
            String baseComName = (baseComponent as Control).Name;
            baseComName = baseComName.Replace("\\","/");
            string ns = baseComName.Substring(0, baseComName.LastIndexOf("/"));
            return GetComponentByName(name, ns);
        }

        public static IXmlComponent GetComponentByName(String name, String baseNamespace=null)
        {
            if (GlobalVars.Components.ContainsKey(name)) //정확한 경로에 있을 때 우선적으로 리턴
            {
                return GlobalVars.Components[name];
            }
            else if (baseNamespace != null) //기준 namespace가 있으면 그 namespace에 속한 control을 우선적으로 리턴.
            {
                if (GlobalVars.Components.ContainsKey(baseNamespace + GlobalVars.Seperator + name))
                {
                    return GlobalVars.Components[baseNamespace + GlobalVars.Seperator + name];
                }
                else //아니라면 전체에서 검색하여 리턴.
                {
                    foreach (String key in GlobalVars.Components.Keys)
                    {
                        int sepIndex = key.LastIndexOf(GlobalVars.Seperator) + 1;
                        string namepart = key.Substring(sepIndex);
                        if (namepart.Equals(name)) return GlobalVars.Components[key];
                    }
                }
            }
            else //기준 namespace가 없으면 전체에서 검색하여 리턴.
            {
                foreach(String key in GlobalVars.Components.Keys)
                {
                    int sepIndex = key.LastIndexOf(GlobalVars.Seperator)+1;
                    string namepart = key.Substring(sepIndex);
                    if (namepart.Equals(name)) return GlobalVars.Components[key];
                }

            }
            return null;
        }

        public static Panel LoadLayoutChildren(XmlNode LayoutFirstNode, Panel rootPanel, Dictionary<String, IXmlComponent> idList, String Namespace, IXmlLayout layout)
        {

            Panel targetPanel;

            if (LayoutFirstNode.Name.Equals("Panel"))
            {
                targetPanel = rootPanel;
            }
            else if (LayoutFirstNode.Name.Equals("Flow"))
            {

                targetPanel = new FlowLayoutPanel();
                targetPanel.Dock = DockStyle.Fill;
                rootPanel.Controls.Add(targetPanel);
            }
            else
            {
                throw new Exception("올바른 Layout 태그가 아닙니다. <Panel> 이나 <Flow> 중 하나를 자식으로 가져야 합니다.");
            }



            XmlNode xPanel = LayoutFirstNode;

            foreach (XmlNode xCom in xPanel.ChildNodes)
            {
                //XmlNode xCom = xPanel.ChildNodes[i];
                if (xCom.NodeType != XmlNodeType.Element) continue; //주석을 거른다.
                if (xCom.Name.Equals("Component") == false) continue; //오직 자식으로는 Component만을 가진다.

                XmlComponent xCompo = new XmlComponent(targetPanel, idList, Namespace);
                xCompo.LoadXml(layout.Interface.Document, xCom);
                layout.Components.Add(xCompo);
            }

            return targetPanel;
        }

        static void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlControlHandler.XmlSchemaValidation:" + e.Message);
        }

        public static Control AddControl(string name, XmlDocument xDoc, XmlNode comNode, Panel targetPanel, Dictionary<String, IXmlComponent> idList, String Namespace = null)
        {
            Control control = null;
            String controlName = comNode.Name;
            switch (comNode.Name)
            {
                
                case "Label":
                    control = AddControlToLayout(name, new XmlLabel(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "TextBox":
                    control = AddControlToLayout(name, new XmlTextBox(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "Button":
                    control = AddControlToLayout(name, new XmlButton(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "ImageButton":
                    control = AddControlToLayout(name, new XmlImageButton(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "Led":
                    control = AddControlToLayout(name, new XmlImageLed(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "TextArea":
                    control = AddControlToLayout(name, new XmlTextArea(), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "Layout":
                    control = AddControlToLayout(name, new XmlLayout(idList, Namespace), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                case "TabControl":
                    control = AddControlToLayout(name, new XmlTabControl(idList, Namespace), xDoc, comNode, targetPanel, idList, Namespace);
                    break;
                
                case "CustomControl":
                    if (GetCustomXmlComponent != null)
                    {
                        controlName = XmlGetter.Attribute(comNode, "ControlName");
                        if (controlName.Length == 0) controlName = name;
                        IXmlComponent compon = GetCustomXmlComponent(controlName, Namespace, idList);
                        if (compon != null)
                        {
                            control = AddControlToLayout(name, compon, xDoc, comNode, targetPanel, idList, Namespace);
                        }
                    }
                    break;
                default:
                    if (GetCustomXmlComponent != null)
                    {
                        IXmlComponent compon = GetCustomXmlComponent(comNode.Name, Namespace, idList);
                        if (compon != null)
                        {
                            control = AddControlToLayout(name, compon, xDoc, comNode, targetPanel, idList, Namespace);
                        }
                    }


                    break;
            }
            if (control == null) throw new Exception(controlName + "은 유효한 컴포넌트명이 아닙니다. Namespace:"+Namespace);
            return control;
        }

        public delegate IXmlComponent GetCustomXmlComponentByNameFunc(String componentName, String Namespace, Dictionary<String, IXmlComponent> idList, params object[] args);

        /// <summary>
        /// delegate IXmlComponent GetCustomXmlComponentByNameFunc(String componentName, String Namespace, Dictionary &lt;String, IXmlComponent&gt; idList)
        /// </summary>
        public static event GetCustomXmlComponentByNameFunc GetCustomXmlComponent = null;

        static Control AddControlToLayout(String name, IXmlComponent com, XmlDocument xDoc, XmlNode comNode, Panel targetPanel, Dictionary<String, IXmlComponent> IdList, String Namespace = null)
        {
            Control control = com as Control;
            com.LoadXml(xDoc, comNode);
            
            //String name = name;// XmlGetter.Attribute(comNode, "Name");
            if (name.Length == 0) name = (comNode.Name);
            string newName = name;
            int count = 0;
            while (IdList.Keys.Contains(newName))
            {
                newName = name + count;
                count++;
            }
            if (Namespace != null && Namespace.Length > 0) newName = Namespace + GlobalVars.Seperator + newName;
            IdList.Add(newName, com);
            control.Name = newName;
            targetPanel.Controls.Add(control);
            return control;
        }

        public static IXmlItem NowEventLoadingXmlItem;
        public static void GetDefaultControlAttributes(
            XmlNode rootNode, XmlDocument document, IXmlComponent xmlComponent, bool refLoad = false, 
            XmlItemInterface.GetRealTimeArgsFunc argFunc=null, XmlItemInterface.GetComponentValueFunc comValueFunc=null )
        {
            xmlComponent.Interface = new XmlItemInterface(rootNode, document, xmlComponent);
            xmlComponent.Interface.GetRealTimeArgCallback = argFunc;
            xmlComponent.Interface.GetComponentValueFuncCallBack = comValueFunc;

            if (refLoad == false)
            {
                String refPath = XmlGetter.Attribute(rootNode, "Ref");
                if (refPath.Length > 0)
                {

                    xmlComponent.LoadXml(refPath, true);

                }
            }

            Control targetControl = xmlComponent as Control;

            if(XmlGetter.Attribute(rootNode,"Margin").Length>0) targetControl.Margin = ValueParser.Padding(XmlGetter.Attribute(rootNode, "Margin"));
            if (XmlGetter.Attribute(rootNode, "Padding").Length > 0) targetControl.Padding = ValueParser.Padding(XmlGetter.Attribute(rootNode, "Padding"));
            if (XmlGetter.Attribute(rootNode, "Enabled").Length > 0) targetControl.Enabled = (XmlGetter.Attribute(rootNode, "Enabled").Equals("false") == false);

            
            DockStyle dock = ValueParser.GetDockStyle(XmlGetter.Attribute(rootNode, "Dock"));

            switch (dock)
            {
                case DockStyle.Bottom:
                case DockStyle.Top:
                    {
                        String hgt = XmlGetter.Attribute(rootNode, "Height");
                        if (hgt.Length != 0) targetControl.Height = int.Parse(hgt);

                        break;
                    }
                case DockStyle.Left:
                case DockStyle.Right:
                    {
                        String wid = XmlGetter.Attribute(rootNode, "Width");
                        if (wid.Length != 0) targetControl.Width = int.Parse(wid);
                        break;
                    }
                case DockStyle.Fill:
                    //필요없음..
                    break;
                case DockStyle.None:
                    {
                        String hgt = XmlGetter.Attribute(rootNode, "Height");
                        if (hgt.Length != 0) targetControl.Height = int.Parse(hgt);
                        String wid = XmlGetter.Attribute(rootNode, "Width");
                        if (wid.Length != 0) targetControl.Width = int.Parse(wid);
                        String x = XmlGetter.Attribute(rootNode, "X");
                        Point location = new Point();
                        if (x.Length != 0) location.X = int.Parse(x);
                        String y = XmlGetter.Attribute(rootNode, "Y");
                        if (y.Length != 0) location.Y = int.Parse(y);
                        targetControl.Location = location;
                        break;
                    }
            }
            targetControl.Dock = dock; //이 control이 붙을 parent에 docking하는 모드임..

            if(dock!= DockStyle.Fill)
                targetControl.Anchor = ValueParser.GetAnchorStyles(XmlGetter.Attribute(rootNode, "Anchor"));


            string txt;

            txt = XmlGetter.Attribute(rootNode, "Name");
            if (txt.Length > 0) targetControl.Name = txt;
            
            txt =  XmlGetter.Attribute(rootNode, "Text");// attr.Value;
            if(txt.Length>0) targetControl.Text = txt;


            string color = XmlGetter.Attribute(rootNode, "TextColor");

            if (color.Length > 0) targetControl.ForeColor = ValueParser.StringToColor(color);


            color = XmlGetter.Attribute(rootNode, "BackColor");
            if (color.Length == 0) color = XmlGetter.Attribute(rootNode, "Background-Color");
            try
            {
                if (color.Length > 0) 
                    targetControl.BackColor = ValueParser.StringToColor(color);
            }
            catch(Exception e)
            {
                if (color.Length > 0)
                {
                    if (color.Equals("Transparent"))
                    {
                        if (targetControl.Parent != null) targetControl.BackColor = targetControl.Parent.BackColor;
                        else targetControl.BackColor = Color.FromKnownColor(KnownColor.Control);
                    }
                    else throw new Exception("XmlControlHandler: 색 변환 중 에러.. color:"+color+"\r\n"+ e.Message);
                }
            }

            LoadInterface(xmlComponent, rootNode, document);
        }

        public static void LoadInterface(IXmlItem xmlComponent, XmlNode rootNode, XmlDocument document)
        {
            if (xmlComponent.Interface == null)
            {
                xmlComponent.Interface = new XmlItemInterface(rootNode, document, xmlComponent);
            }
            if (rootNode == null) return;

            NowEventLoadingXmlItem = xmlComponent; //Action에서 각각의 type에 따라서 적절한 Condition을 가져오기 위해 사용됨..
            XmlNode xEvents = XmlGetter.Child(rootNode, "Events");// rootNode.SelectSingleNode("Events");

            if (xEvents != null) xmlComponent.Interface.Events.LoadXml(document, xEvents);

            XmlNodeList xArguments = XmlGetter.Children(rootNode, "Arguments/Argument");
            if (xArguments != null)
            {
                foreach (XmlNode xArg in xArguments)
                {
                    String name = XmlGetter.Attribute(xArg, "Name");
                    String value = xArg.InnerText;
                    xmlComponent.Interface.Arguments.Add(name, value);
                }
            }
        }


        public static void GetDefaultXmlItemAttributes(XmlNode rootNode, XmlDocument document, IXmlItem xmlItem, bool refLoad = false)
        {
            xmlItem.Interface = new XmlItemInterface(rootNode, document, xmlItem);

            if (refLoad == false)
            {
                String refPath = XmlGetter.Attribute(rootNode, "Ref");
                if (refPath.Length > 0)
                {
                    xmlItem.LoadXml(refPath, true);

                }
            }

            
        }


        public static void GetTextAttributes(XmlNode rootNode, IXmlComponent xmlComponent)
        {
            Button b = new Button();
            
        }
    }
}
