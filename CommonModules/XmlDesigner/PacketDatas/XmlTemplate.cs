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

namespace XmlDesigner.PacketDatas
{
    public class XmlTemplate: IXmlItem
    {
        XmlCommandInfo _info = new XmlCommandInfo();
        XmlCommandStructDefinition _command;
        XmlCommandStructDefinition _response;
        XmlCommandStructDefinition _autoResponse;

        public XmlCommandInfo Info { get { return _info; } }
        public XmlCommandStructDefinition Command { get { return _command; } }
        public XmlCommandStructDefinition Response { get { return _response; } }
        public XmlCommandStructDefinition AutoResponse { get { return _autoResponse; } }

        String _imagePath=null;
        public String ImagePath{
            get{ return _imagePath;}
            set{ _imagePath = value;}
        }

        public XmlTemplate Clone()
        {
            XmlTemplate template = new XmlTemplate(_fullPath);
            if(_command!=null) template._command = _command.Clone();
            if(_response!=null) template._response = _response.Clone();
            if(_autoResponse!=null) template._autoResponse = _autoResponse.Clone();
            template._info = _info;//info는 공용으로 써도 된다.
            return template;
        }

        public XmlTemplate(String fullPath)
        {
            _fullPath = fullPath;
             
        }

        String _fullPath;
        public String FullPath { get { return _fullPath; } }

        XmlDocument _xDoc;  
        
        public void LoadXml(String xmlFile, Boolean refLoad = false)
        {
            if (XmlScenario.NowLoadingPath.Length > 0) _filePath =  xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, null, XmlSchemaValidation);

            //try
            {
                LoadXml(_xDoc, xNode, refLoad);
            }
            //catch (Exception e)
            {
                //MessageBox.Show(e.Message + ":" + xmlFile);
            }
        }



        void XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        

        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);
            try
            {
                XmlNode infoNode = XmlGetter.Child(rootNode, "Info");
                _info.LoadXml(xDoc, infoNode, refLoad);
            }
            catch (Exception e)
            {
                throw new Exception("Error On Loading Template [" + _filePath + "].Info.." + e.Message);
            }

            try
            {
                XmlNode commandNode = XmlGetter.Child(rootNode, "Command");
                if (commandNode != null)
                {
                    _command = new XmlCommandStructDefinition("CommandHeader.xml", StructType.Command);
                    _command.LoadXml(_xDoc, commandNode);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error on Loading Template [" + _info.Name + "].Command..." + e.Message);
            }

            try
            {
                XmlNodeList responseNodeList = XmlGetter.Children(rootNode, "Response");
                foreach (XmlNode responseNode in responseNodeList)
                {
                    String type = XmlGetter.Attribute(responseNode, "Type");
                    if (type.ToLower().Equals("continuous"))
                    {
                        _autoResponse = new XmlCommandStructDefinition("ResponseHeader.xml", StructType.Response);
                        _autoResponse.LoadXml(_xDoc, responseNode);

                    }
                    else
                    {
                        _response = new XmlCommandStructDefinition("ResponseHeader.xml", StructType.Response);
                        _response.LoadXml(_xDoc, responseNode);
                    }
                    
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error on Loading Template [" + _info.Name + "].Response..." + e.Message);
            }


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
}
