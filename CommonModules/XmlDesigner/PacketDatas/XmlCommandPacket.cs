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
    public class XmlNetworkPacket : IXmlItem
    {
        public XmlNetworkPacket()
            : base()
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

        XmlPacketData _header = new XmlPacketData();
        public XmlPacketData Header { get { return _header; } }

        XmlPacketData _data = new XmlPacketData();
        public XmlPacketData Data { get { return _data; } }
        
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad = false)
        {
            if (rootNode == null) return;
            _xDoc = xDoc;  XmlControlHandler.GetDefaultXmlItemAttributes(rootNode, xDoc, this);

            XmlNode headerNode = XmlGetter.Child(rootNode, "Header");
            _header.LoadXml(_xDoc, headerNode);

            XmlNode dataNode = XmlGetter.Child(rootNode, "Data");
            _data.LoadXml(_xDoc, dataNode);

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

        /// <summary>
        /// offset부터 시작하여 내부의 내용을 복사하고, 다음으로 넣어야 할 offset을 리턴한다.
        /// </summary>
        /// <param name="targetBuffer">Target이 되는 Buffer</param>
        /// <param name="offset">byte size 버퍼 시작점.</param>
        /// <returns>다음 offset</returns>
        public int GetBuffer(Array targetBuffer, int offset=0)
        {
            offset += Header.copyBufferToArray(targetBuffer, offset);
            offset += Data.copyBufferToArray(targetBuffer, offset);
            return offset;
        }

        public Byte[] GetBuffer()
        {
            int offset = 0;
            Byte[] buff = new Byte[Header.bufferByteSize + Data.bufferByteSize];
            if(Header.bufferByteSize>0) offset += Header.copyBufferToArray(buff, offset);
            if(Data.bufferByteSize>0) offset += Data.copyBufferToArray(buff, offset);
            return buff;
        }

        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
