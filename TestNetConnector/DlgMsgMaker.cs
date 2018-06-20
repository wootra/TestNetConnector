using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataHandling;
using FormAdders;
using System.IO;
using CustomParser;

namespace TestNetConnector
{
    public partial class DlgMsgMaker : PopupForm
    {
        CPacketStruct _ns;
        String _saveFile = "./msg.txt";
        String _xmlFile = "./msg.xml";
        bool _swapWhenMakePacket = true;
        
        /// <summary>
        /// don't use This constructor..
        /// </summary>
        public DlgMsgMaker():base()
        {
            //don't use this constructor..
            InitializeComponent();
        }
        
        public DlgMsgMaker(CPacketStruct ns, String saveFile, bool swapWhenMakePacket)
            : base()
        {
            InitializeComponent();
            T_Comment.Text = ns.Infos.Comment;
            String dir = saveFile.Substring(0, saveFile.LastIndexOf("\\"));
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
            _xmlFile = saveFile.Substring(0, saveFile.LastIndexOf("\\")+1) + "msg.xml";
            _saveFile = saveFile;
            _swapWhenMakePacket = swapWhenMakePacket;
            _ns = ns;
            if (ns == null)
            {
                if (File.Exists(_xmlFile))
                {
                    //List<CStructItem> items =  CustomParser.XMLParser.FromXml(_xmlFile);
                    T_Msg.Text = StructXMLParser.XmlToCode(_xmlFile);
                }
                else if (File.Exists(_saveFile))
                {
                    T_Msg.Text = File.ReadAllText(_saveFile);
                }
            }
            else
            {
                T_Msg.Text = ns.NativeText;
            }
            B_Help.Click += new EventHandler(B_Help_Click);
            /*
            if (ns.nativeText != null && ns.nativeText.Length > 0)
            {
                T_Msg.Text = ns.nativeText;
            }
            else
            {
                try
                {
                    if (File.Exists(_xmlFile))
                    {
                        //List<CStructItem> items =  CustomParser.XMLParser.FromXml(_xmlFile);
                        T_Msg.Text = XMLParser.ToCode(_xmlFile);
                    }
                    else if (File.Exists(_saveFile))
                    {
                        T_Msg.Text = File.ReadAllText(_saveFile);
                    }
                }
                catch { }
            }
             */
        }

        void B_Help_Click(object sender, EventArgs e)
        {
            MyDialog dlg = new MyDialog();
            String helpText = "Please Write in C Language grammer. ex>int a[5]={1,2,3,4,5};\r\n"+
                "you can omit the variable name. ex>int [5]={1,2,3,4,5};\r\n"+
                "you can write only one value for an array having same values."+
                "ex> int [5]=1;\r\n"+
                "if you want to swap, insert swap@ before variable name. ex> int swap@a=1;\r\n";
            
            dlg.AddText(helpText);
            dlg.ShowDialog();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                _ns.setNativeText(T_Msg.Text);
            }catch(Exception ex){
                MessageBox.Show(ex.Message);
                return;
            }
             */
            //File.WriteAllText(_saveFile, T_Msg.Text);
            try
            {
                SaveFile();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
           
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveFile()
        {
            
            //File.WriteAllText(_saveFile, T_Msg.Text);
            _ns.NativeText = T_Msg.Text;//.setNativeText(ss);
            //_ns.Items = 
            StructXMLParser.CodeToItems(T_Msg.Text, _ns);

            if( _ns.IsDynamicPacket==false) _ns.MakePacket(_swapWhenMakePacket);
            CStructInfo info = new CStructInfo(T_Comment.Text, DateTime.Now);
            StructXMLParser.ItemsToXml(_ns.Items, _xmlFile, info);
            
        }
    }
}
