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
using XmlDesigner.ForEvents;


namespace XmlDesigner
{
    public class XmlTextArea : RichTextBox, IXmlComponent, IXmlItem
    {
        
        Control _control;
        Type _type;
        XmlDocument _xDoc;
        XmlItemTypes _comType = XmlItemTypes.TextArea;
        XmlEvents _events = new XmlEvents();
        Queue<String> _logToAdd = new Queue<string>();

        public XmlEvents Events { get { return _events; } }

        public event ValidationEventHandler E_XmlSchemaValidation;

        MenuItem wordWrapItem = new MenuItem("WordWrap");
        Timer _timer = new Timer();

        public XmlTextArea()
            : base()
        {
            this.WordWrap = true;
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.MenuItems.Add("Clear", new EventHandler(OnClear));
            wordWrapItem.Checked = this.WordWrap;
            wordWrapItem.Click += new EventHandler(wordWrapItem_Click);
            this.ContextMenu.MenuItems.Add(wordWrapItem);
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
        bool _writing = false;

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_writing == false)//타이머가 겹쳐지는 경우 방지..
            {
                _writing = true;
                //_timer.Stop();
                //_timer = null;
                try
                {
                    if (_logToAdd.Count > 0)
                    {
                        bool scrollToLast = false;
                        int firstIndex = GetCharIndexFromPosition(new Point(5, 5));
                        int lastIndex = GetCharIndexFromPosition(new Point(this.Width - 5, this.Height + 100));
                        if (lastIndex == this.Text.Length - 1 || lastIndex == 0)
                        {//제일 마지막에 스크롤바가 와 있다.
                            scrollToLast = true;
                        }
                        else
                        {
                            scrollToLast = false;
                        }

                        try
                        {
                            int max = 10;
                            for (int i = 0; i < _logToAdd.Count; i++)
                            {
                                String str = _logToAdd.Dequeue();
                                AppendText(str);
                                max--;
                                if (max == 0) break;//최대 max 라인만 출력하고 넘어감..
                            }
                            if (_logToAdd.Count > 300)
                            {
                                AppendText("WARNING::[" + _logToAdd.Count + "] line is hidden. see in the file[" + _saveFileName + "]\r\n");
                                _logToAdd.Clear();
                            }

                        }
                        catch { }

                        if (this.Text.Length > BufferSize)
                        {
                            if (scrollToLast)
                            {//제일 마지막에 스크롤바가 와 있다.

                                String txt = this.Text.Substring((int)(BufferSize * 0.1));
                                Clear();
                                //this.Clear();
                                AppendText(txt);
                                
                                //this.AppendText(txt);

                            }
                            else
                            {

                            }

                        }

                        if (scrollToLast)
                        {//제일 마지막에 스크롤바가 와 있다.

                            this.Select(this.Text.Length - 1, 1);
                            ScrollToCaret();
                        }
                        else
                        {
                            this.Select(firstIndex, 1);
                            ScrollToCaret();

                        }
                    }
                }
                catch { }
                _writing = false;
            }
        }


        void wordWrapItem_Click(object sender, EventArgs e)
        {
            this.wordWrapItem.Checked = this.WordWrap = !this.WordWrap;
        }

        void OnClear(object sender, EventArgs args)
        {
            this.Clear();
        }

        public void OnCloseProcess(){
            try
            {
                if (_fileWriter != null) _fileWriter.Close();
            }
            catch { }
        }

        ~XmlTextArea()
        {
            OnCloseProcess();
        }


        public XmlItemTypes XmlItemType { get { return _comType; } }

        public Control RealControl { get { return this; } }

        public Type Type { get { return this.GetType(); } }
        
        String _filePath = "";
        public string FilePath
        {
            get { return _filePath; }
        }

        public void LoadXml(String xmlFile, Boolean refLoad=false)
        {
            if (XmlLayoutCollection.NowLoadingPath.Length > 0) _filePath = XmlLayoutCollection.NowLoadingPath +  XmlLayoutCollection.PathSeperator + xmlFile;
            else _filePath = xmlFile;

            XmlNode xNode = XmlGetter.RootNode(out _xDoc,_filePath, "./ComponentSchemas/LabelSchema.xsd", XmlScenarioTable_E_XmlSchemaValidation);
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
                MessageBox.Show("XmlTextArea.LoadXml:" + e.Message + ":" + xmlFile);
            }
        }



        void XmlScenarioTable_E_XmlSchemaValidation(object sender, ValidationEventArgs e)
        {
            MessageBox.Show("XmlTextArea.XmlScenarioTable_E_XmlSchemaValidation:" + e.Message);
        }


        public void SaveXml(String xmlFile)
        {

        }

        int _initBufferSize = 10000;
        int _bufferSize = 10000;
        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        //delegate void textFunc(string text, bool writeInfo);

        public void AddLog(String text, bool writeInfo)
        {
            if (_fileWriter == null) throw new Exception("Set FileName to Save with SetSaveFile(filePath, format, ext);");

                _logToAdd.Enqueue(text);
                //AppendText(text);
                if (_fileWriter != null)
                {
                    try
                    {
                        TextWriter writer = TextWriter.Synchronized(_fileWriter);
                        if (writeInfo) writer.WriteLine("Written at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff") + "-->");
                        
                        writer.WriteLine(text);
                        writer.Flush();
                        //_fileWriter.WriteLine(text);
                    }
                    catch (Exception ex)
                    {
                        
                        TextWriter writer = TextWriter.Synchronized(_fileWriter);
                        if (writeInfo) writer.WriteLine("Written at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff") + "-->");
                        writer.WriteLine(ex.Message);

                        writer.WriteLine(text);
                    }
                }
                
            

            

        }

        String _saveFileName = "";
        /// <summary>
        /// path가 null이면, 자동으로  GlobalConfigs.TestNgineSavingFolder+"Logs/TextArea" 로 셋팅됨..
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileFormat"></param>
        /// <param name="extension"></param>
        public void SetSaveFile(String path, string fileFormat="yyMMdd_HHmmss", string extension="log")
        {
            
            path = path.Replace("/", "\\");
            path = Path.GetFullPath(path);

            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            _saveFileName = path + "\\" + DateTime.Now.ToString(fileFormat) + "." + extension;
            _fileWriter = new StreamWriter(File.OpenWrite(_saveFileName));
            _fileWriter.AutoFlush = true;
        }

        StreamWriter _fileWriter = null;
        public void LoadXml(XmlDocument xDoc, XmlNode rootNode, Boolean refLoad=false)
        {
            if (rootNode == null) return;
            XmlControlHandler.GetDefaultControlAttributes(rootNode, xDoc,   this);

            foreach (XmlNode child in rootNode.ChildNodes)
            {
                if (child.Name.Equals("BufferSize"))
                {
                    if (int.TryParse(child.InnerText, out _bufferSize) == false) _bufferSize = _initBufferSize;
                }
                else if (child.Name.Equals("SaveFile"))
                {
                    String path = XmlGetter.Attribute(child, "Path");
                    if (path.Contains(":") == false)//절대경로가 아닐 경우..
                        path = XmlLayoutCollection.NowLoadingPath + "\\" + path;
                    String fileFormat = XmlGetter.Attribute(child, "SavingFormat");
                    String ext = XmlGetter.Attribute(child, "Extension");
                    SetSaveFile(path, fileFormat, ext);
                }
            }
            
        }


        public XmlNode GetXml(XmlDocument xDoc, XmlNode parent = null)
        {
            XmlNode root = XmlAdder.Element(xDoc, "Label", parent);
            XmlAdder.Attribute(xDoc, "Text", this.Text, parent);
            //XmlAdder.Attribute(xDoc, "Enabled", this.Enabled ? "true" : "false", parent);
            
            if (this.ForeColor.IsKnownColor) XmlAdder.Attribute(xDoc, "TextColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "TextColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);

            if (this.BackColor.IsKnownColor) XmlAdder.Attribute(xDoc, "BackColor", this.ForeColor.Name, parent);
            else XmlAdder.Attribute(xDoc, "BackColor", String.Format("#{0:X6}", this.ForeColor.ToArgb()), parent);

            return root;
        }
        XmlItemInterface _interface;
        public XmlItemInterface Interface { get { return _interface; } set { _interface = value; } }
    }
}
