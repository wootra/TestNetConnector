using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XmlDesigner.ForEvents;
using DataHandling;

namespace XmlDesigner
{
    public class XmlItemInterface
    {
        IXmlItem _owner;
        XmlNode _node;
        XmlDocument _document;

        public enum XmlItemTypes { Item, Component };
        XmlItemTypes _itemType;

        CustomDictionary<String, String> _arguments = new CustomDictionary<string, String>();
        public CustomDictionary<String, String> Arguments { get { return _arguments; } }

        XmlEvents _events = new XmlEvents();
        public XmlEvents Events { get { return _events; } }

        public void RunFunction(String funcName, params object[] args)
        {
            if (RunFuncCallBack == null) return;
            else RunFuncCallBack(funcName, args);
        }

        public delegate void runfunc(String funcName, params object[] args);
        
        public runfunc RunFuncCallBack;

        /// <summary>
        /// Xml에서 지정된 Argument태그의 내용을 가져옴. 값이 없으면 null리턴.
        /// </summary>
        /// <param name="name">Name attribute</param>
        /// <returns>값이 없으면 null, 있으면 해당 Argument태그의 InnerText</returns>
        public String Argument(String name)
        {
            if (_arguments.ContainsKey(name)) return _arguments[name];
            else return null;
        }

        /// <summary>
        /// delegate object GetRealTimeArgsFunc(String argName);
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public delegate object GetRealTimeArgsFunc(String argName);
        
        /// <summary>
        /// public delegate object GetRealTimeArgsFunc(String argName);
        /// </summary>
        /// 

        public void RegisterEvent(String EventName, String ActionName, params String[] args)
        {
            XmlActionList list = new XmlDesigner.XmlActionList(); //xml에서 정의될 것을 코드상에서 가져온다..
            list.Add(new XmlAction(ActionName, args));

            XmlEvent evt = new XmlEvent(EventName, new XmlActionList[] { list }.ToList());
            this.Events.Add(EventName, evt);

        }

        public void RunEvent(String eventName)
        {
            XmlControlHandler.RunEvent(_owner, eventName);
        }

        public GetRealTimeArgsFunc GetRealTimeArgCallback = null;

        public object[] GetRealTimeArgs(string[] argNames)
        {
            if (GetRealTimeArgCallback != null)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < argNames.Length; i++)
                {
                    if (argNames[i].Length > 0 && argNames[i][0] == '@')
                    {
                        String newArg = argNames[i].Substring(1);//@로 시작하면 실시간 계산함..
                        if (GetRealTimeArgCallback != null) list.Add(GetRealTimeArgCallback(newArg));
                        else list.Add(argNames[i]);
                    }
                    else
                    {
                        list.Add(argNames[i]);
                    }
                }
                return list.ToArray();
            }
            else return null;

        }
        /// <summary>
        /// delegate String GetComponentValueFunc(String componentPath);
        /// </summary>
        /// <param name="componentPath"></param>
        /// <returns></returns>
        public delegate String GetComponentValueFunc(String componentPath);

        /// <summary>
        /// delegate String GetComponentValueFunc(String componentPath);
        /// </summary>
        public GetComponentValueFunc GetComponentValueFuncCallBack = null;

        public String GetComponentValue(String componentPath)
        {
            if (GetComponentValueFuncCallBack != null)
            {
                string value = GetComponentValueFuncCallBack(componentPath);
                if (value == null) throw new Exception("지정된 컴포넌트 내부 Path["+componentPath+"] 가 없습니다.");
                else return value;
            }
            else throw new Exception("지정된 노드는 내부Path 검색을 허용하지 않습니다.:"+_node.Name);
        }
            
        public XmlItemInterface(XmlNode node, XmlDocument document, IXmlItem item)
        {
            _node = node;
            _owner = item;
            _document = document;
            if (item is IXmlComponent) _itemType = XmlItemTypes.Component;
            else _itemType = XmlItemTypes.Item;

        }

        public XmlNode Node
        {
            get { return _node; }
        }

        public XmlDocument Document
        {
            get { return _document; }
            set { _document = value; }

        }

        public IXmlItem Owner
        {
            get { return _owner; }
        }

        public XmlItemTypes ItemType
        {
            get { return _itemType; }
        }
    }
}
