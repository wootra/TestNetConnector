using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Schema;

using XmlHandlers;
using FormAdders.EasyGridViewCollections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Windows.Forms;

using XmlDesigner.ForEvents.Conditions;

namespace XmlDesigner
{
    public class XmlAction : IXmlItem
    {
        public String[] Args;
        public String Name;
        //public bool IsRealTimeArgs=false;

        public XmlAction()
        {

        }


        public XmlAction(String name, params String[] args)
        {
            Name = name;
            Args = args;
        }

        public XmlItemTypes XmlItemType
        {
            get { return XmlItemTypes.Action; }
        }

        public Type Type
        {
            get { return typeof(XmlAction); }
        }

        XmlDocument _xDoc;  
        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            //DataSet dt = new DataSet();
            //dt.ReadXmlSchema(Properties.Resources.TableSchema);
            //dt.ReadXml(xmlFile); //schema를 불러와서 체크하기 위하여..
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc, _filePath, "./ComponentSchemas/ActionSchema.xsd", XmlSchemaValidation);
            /*
            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = false;
            _xDoc.Schemas = new System.Xml.Schema.XmlSchemaSet();
            XmlSchema schema = XmlSchema.Read(File.OpenRead("./ComponentSchemas/LabelSchema.xsd"), XmlScenarioTable_E_XmlSchemaValidation);
            _xDoc.Schemas.Add(schema);

            _xDoc.Load(xmlFile);

                        
            xNode = _xDoc.SelectSingleNode("//Label");
             */
            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show("XmlAction.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlAction.XmlSchemaValidation:" + e.Message);
        }

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            string name = XmlGetter.Attribute(rootNode, "Name");
            //string realTimeArgs = XmlGetter.Attribute(rootNode, "IsRealTimeArgs");

            Name = name;
            //IsRealTimeArgs = ValueParser.IsTrue(realTimeArgs);

            XmlNodeList argList = XmlGetter.Children(rootNode, "Arg"); //Arg로 하나씩 나누었을 때
            if (argList.Count == 0)
            {
                XmlNode node = XmlGetter.Child(rootNode, "Args");//Args로 빈칸으로 구분하여 넣었을 때
                if (node != null)
                {
                    string[] args = node.InnerText.Split(" ,".ToCharArray());
                    Args = args;
                }
            }else{

                List<String> args = new List<string>();
                foreach (XmlNode node in argList)
                {
                    args.Add(node.InnerText);
                }
                Args = args.ToArray();
            }
        }

        public void SaveXml(string xmlFile)
        {
            throw new NotImplementedException();
        }

        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public XmlNode GetXml(XmlDocument xDoc,XmlNode parent = null)
        {
            String args = "";
            for (int i = 0; i < Args.Length; i++)
            {
                if (i != 0) args += " ";
                args += Args[i];
            }

            XmlNode xAction = XmlAdder.Element(xDoc, "Action", args, parent);
            XmlAdder.Attribute(xDoc, "Name", Name, xAction);
            return xAction;
        }


        void IXmlItem.LoadXml(string xmlFile, bool refLoad)
        {
            LoadXml(xmlFile, refLoad);
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
