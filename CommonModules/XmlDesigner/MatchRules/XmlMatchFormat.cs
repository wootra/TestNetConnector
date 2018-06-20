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
using System.Drawing;
using XmlDesigner.Parsers;

namespace TestNgineData.PacketDatas
{
    public class XmlMatchFormat: IXmlItem
    {
        public static XmlMatchFormat NowLoading;

        public XmlMatchFormat()
        {
        }

         XmlDocument _xDoc;  public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath = XmlScenario.NowLoadingPath + XmlScenario.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlSchemaValidation);

            try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        public Color Color { get { return _color; } }

        Color _color = Color.Black;

        List<FormatItem> _formatItems = new List<FormatItem>();
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  
            XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);
            NowLoading = this;
            try
            {
                _color = ValueParser.StringToColor(XmlGetter.Attribute(rootNode, "Color"));
            }
            catch
            {
                _color = Color.Black;
            }
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode child = rootNode.ChildNodes[i];

                if (child.Name.Equals("String"))
                {
                    _formatItems.Add(new FormatItem(FormatItem.FormatItemType.String, child.InnerText));
                }
                else if(child.Name.Equals("FieldItem"))
                {
                    String parser = XmlGetter.Attribute(child, "Parser");
                    if (parser.Length > 0)
                    {
                        string[] args = ValueParser.GetArgs(parser, XmlGetter.Attribute(child, "Args"));
                        _formatItems.Add(new FormatItem(FormatItem.FormatItemType.FieldItem, args));
                    }
                    else
                    {
                        _formatItems.Add(new FormatItem(FormatItem.FormatItemType.FieldItem));
                    }
                }
                
            }
        }

        public string GetFormatedValue(string value)
        {
            if (_formatItems.Count == 0) return value;//그대로 돌려줌..
            string totalText = "";
            for(int i=0; i<_formatItems.Count; i++){
                if(_formatItems[i].ItemType == FormatItem.FormatItemType.String) totalText+= _formatItems[i].Text;
                else{
                    string parserName = _formatItems[i].ParserName;
                    String[] parserToken = parserName.Split(".".ToCharArray());

                    string[] args = _formatItems[i].Args.ToArray();
                    String parser = parserToken[0];
                    String funcName = parserToken[1];
                    if (parser.Equals("Text"))
                    {
                        totalText += TextParser.RunParser(value, funcName, args);
                    }
                }
            }
            return totalText;
            
        }

 


        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public Type Type
        {
            get { return typeof(XmlImageList); }
        }

        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {

            return null;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }


    }

    public class FormatItem
    {
        public enum FormatItemType { String, FieldItem };
        public enum FormatParser { ToString };
        public List<string> Args = new List<string>();

        FormatItemType _itemType;
        String _formatParser;
        String _formatStringValue;

        public String Text { get { return _formatStringValue; } }

        public String ParserName { get { return _formatParser; } }

        public FormatItem()
        {
            _itemType = FormatItemType.String;
            _formatStringValue = "";
        }

        public FormatItem(FormatItemType type, params string[] args)
        {
            SetFormatItem(type, args);
        }

        public FormatItemType ItemType { get { return _itemType; } }

        public void SetFormatItem(FormatItemType type, params string[] args)
        {
            _itemType = type;
            if (args.Length > 0)
            {
                if (args[0].Trim().Length > 0)
                {
                    if (type == FormatItemType.String)
                    {
                        _formatStringValue = args[0];
                    }
                    else
                    {
                        _formatParser = args[0];
                        for (int i = 1; i < args.Length; i++)
                        {
                            Args.Add(args[i]);
                        }
                    }
                }
            }
        }


    }
}
