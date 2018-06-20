using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using NetworkModules4;
using System.Net;
using DataHandling;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using CustomParser;
using FormAdders;
using RtwEnums.Network;
using FormAdders.EasyGridViewCollections;
using NetworkPacket;
namespace TestNetConnector
{
    public partial class Con_Peer : UserControl
    {


        String _version = "  v2.2[2012/10/09 Rev.1] - 그룹과 Timer오류, Contents Table크기오류수정...";
        //BindingSource _history = new BindingSource();
        Boolean _isSendMsgOk = false;
        INetConnector _conn = null;
        String[] sendIndex = new String[1];

        Byte[] _recvBuff = new Byte[8172];
        Byte[] _sendBuff = new Byte[8172];
        Byte[] _recvHeader = null;
        int _totalRecvSize = 0;

        int _sendSize = 0;
        #region set Client/server
        void setTcpClient()
        {
            if (_conn != null) { _conn.Close(); }


            _conn = new TcpClientBase();

            ((TcpClientBase)_conn).E_Connection += new ConnectionEventHandler(_tcp_ConnectionEvent);
            ((TcpClientBase)_conn).E_OnReceivedInQueue += new TransferEventHandler(onReceiveData);
            ((TcpClientBase)_conn).E_NetError += new NetworkErrorEventHandler(_tcp_NetError);
        }
        enum SendType { Normal = 0, Multicast = 1, Broadcast = 2 };
        void setUdpClient(SendType type=SendType.Normal)
        {
            if (_conn != null) { _conn.Close(); }

            _conn = new UdpClientBase(-1,(UdpClientBase.SendType)type);
            ((UdpClientBase)_conn).E_Connection += new ConnectionEventHandler(_tcp_ConnectionEvent);
            ((UdpClientBase)_conn).E_OnReceivedInQueue += new TransferEventHandler(onReceiveData);
            ((UdpClientBase)_conn).E_NetError += new NetworkErrorEventHandler(_tcp_NetError);
        }

        void setTcpServer(int maxClients=1)
        {
            if (_conn != null && _conn.isConnected()) { _conn.Close(); }

            _conn = new TcpServerBase(maxClients);

            ((TcpServerBase)_conn).E_Connection += new ConnectionEventHandler(_tcp_ConnectionEvent);
            ((TcpServerBase)_conn).E_OnReceivedInQueue += new TransferEventHandler(onReceiveData);
            ((TcpServerBase)_conn).E_NetError += new NetworkErrorEventHandler(_tcp_NetError);
        }

        void setUdpServer(SendType type=SendType.Normal)
        {
            if (_conn != null) { _conn.Close(); }

            _conn = new UdpServerBase(0,(UdpServerBase.SendType)type);

            ((UdpServerBase)_conn).E_Connection += new ConnectionEventHandler(_tcp_ConnectionEvent);
            ((UdpServerBase)_conn).E_OnReceivedInQueue += new TransferEventHandler(onReceiveData);
            ((UdpServerBase)_conn).E_NetError += new NetworkErrorEventHandler(_tcp_NetError);
        }
        #endregion

        ToolStripMenuItem[] _conPosList;
        ToolStripMenuItem[] _connTypes;
        RadioButton[] _rNetPositions;
        RadioButton[] _rconnTypes;

        ToolStripMenuItem[] _echoModes;
        ToolStripMenuItem[] _encodings;
        ToolStripMenuItem[] _endians;
        ToolStripMenuItem[] _formats;
        ToolStripMenuItem[] _structWhat;
        ToolStripMenuItem[] _typeSizes;
        ToolStripMenuItem[] _views;
        ConMsgList _savedList;
        Dictionary<String, String> _env = new Dictionary<string, string>();
        String _saveEnvPath;

        enum Titles { size, ip, data };
        //ConMsgList _savedList = new ConMsgList();
        public Con_Peer(String envFilePath)
        {

            InitializeComponent();
            _saveEnvPath = envFilePath;

            //this.Text = this.Text + _version;
            setTcpServer(1);

            //_tcp.setFuncInLoop(RecvLoop);

            //timer1.Enabled = false;
            //timer1.Tick += _timeout;
            //timer1.Interval = T_Timeout.IntValue;

            //_recvTimeoutTimer.Start();
            _responseTimer.Tick += new EventHandler(_responseTimer_Tick);
            D_History.AddTitleTextBoxColumn(50, Titles.size.ToString(), "Size", false);
            D_History.AddTitleTextBoxColumn(100, Titles.ip.ToString(), "Source", false);
            D_History.AddTitleTextBoxColumn(-1, Titles.data.ToString(), "Data", false);
            
            //D_History.DataSource = _history;
            D_History.ActionOnRightClicked = Actions.ContextMenu;
            D_History.AddContextMenuItem(new String[] { "Clear History" });
            D_History.E_ContextMenuClicked += new EasyGridMenuClickHandler(D_History_E_ContextMenuClicked);
            #region radio buttons
            R_MsgFormatBin.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_MsgFormatDec.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_MsgFormatHex.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_MsgFormatStr.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_MsgFormatFloat.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_MsgFormatELog.Click += new EventHandler(R_AnalysisMsgFormat_Click);
            R_EchoMode.E_RadioButtonSelected += new IndexSelectedEventHandler(R_EchoMode_E_RadioButtonSelected);
            /*
            R_ModeEcho.Click +=new EventHandler(R_ModeChanged);  
            R_ModeEchoWithMsg.Click +=new EventHandler(R_ModeChanged);
            R_ModeSendMsg.Click += new EventHandler(R_ModeChanged);



            R_BigEndians.Click += new EventHandler(R_Endian_Click);
            R_LittleEndians.Click += new EventHandler(R_Endian_Click);

            R_Encoding8.Click += new EventHandler(R_Encoding_Click);
            R_Encoding7.Click += new EventHandler(R_Encoding_Click);
            R_Encoding16.Click += new EventHandler(R_Encoding_Click);
            R_Encoding32.Click += new EventHandler(R_Encoding_Click);

            R_Tcp.Click += new EventHandler(R_ConnMode_Click);
            R_Udp.Click += new EventHandler(R_ConnMode_Click);

            R_Server.Click += new EventHandler(R_ServerClient_Click);
            R_Client.Click += new EventHandler(R_ServerClient_Click);
             */
            #endregion

            #region Menustrip items
            M_ConnSiteClient.Click += new EventHandler(M_ConnSite_Click);
            M_ConnSiteServer.Click += new EventHandler(M_ConnSite_Click);
            R_ConnPosClient.Click += new EventHandler(M_ConnSite_Click);
            R_ConnPosServer.Click += new EventHandler(M_ConnSite_Click);
            _conPosList = new ToolStripMenuItem[] { M_ConnSiteServer, M_ConnSiteClient };
            _rNetPositions = new RadioButton[] { R_ConnPosServer, R_ConnPosClient };

            M_ConnModeTcp.Click += new EventHandler(M_ConnType_Click);
            M_ConnModeUdp.Click += new EventHandler(M_ConnType_Click);
            M_ConnTypeUdpMulticast.Click += new EventHandler(M_ConnType_Click);
            R_ConnModeTcp.Click += new EventHandler(M_ConnType_Click);
            R_ConnModeUdp.Click += new EventHandler(M_ConnType_Click);
            _connTypes = new ToolStripMenuItem[] { M_ConnModeTcp, M_ConnModeUdp, M_ConnTypeUdpMulticast };
            _rconnTypes = new RadioButton[] { R_ConnModeTcp, R_ConnModeUdp };


            M_EchoModeNoEcho.Click += new EventHandler(M_EchoMode_Click);
            M_EchoModeReceived.Click += new EventHandler(M_EchoMode_Click);
            M_EchoModeNext.Click += new EventHandler(M_EchoMode_Click);
            M_EchoModeSelected.Click += new EventHandler(M_EchoMode_Click);
            
            _echoModes = new ToolStripMenuItem[] { M_EchoModeNoEcho, M_EchoModeReceived, M_EchoModeNext, M_EchoModeSelected };

            M_StringWithNull.Click += new EventHandler(M_StringWithNull_Click);

            M_Encoding16.Click += new EventHandler(M_Encoding_Click);
            M_Encoding32.Click += new EventHandler(M_Encoding_Click);
            M_Encoding7.Click += new EventHandler(M_Encoding_Click);
            M_Encoding8.Click += new EventHandler(M_Encoding_Click);
            M_EncodingASCII.Click += new EventHandler(M_Encoding_Click);
            _encodings = new ToolStripMenuItem[] { M_Encoding8, M_Encoding7, M_Encoding16, M_Encoding32, M_EncodingASCII };

            M_EndianBig.Click += new EventHandler(M_Endian_Click);
            M_EndianLittle.Click += new EventHandler(M_Endian_Click);
            _endians = new ToolStripMenuItem[] { M_EndianBig, M_EndianLittle };

            M_FormatBin.Click += new EventHandler(M_Format_Click);
            M_FormatDec.Click += new EventHandler(M_Format_Click);
            M_FormatELog.Click += new EventHandler(M_Format_Click);
            M_FormatFloat.Click += new EventHandler(M_Format_Click);
            M_FormatHex.Click += new EventHandler(M_Format_Click);
            M_FormatString.Click += new EventHandler(M_Format_Click);
            _formats = new ToolStripMenuItem[] { M_FormatHex, M_FormatDec, M_FormatBin, M_FormatString, M_FormatFloat, M_FormatELog };

            M_TypeSize1.Click += new EventHandler(M_TypeSize_Click);
            M_TypeSize2.Click += new EventHandler(M_TypeSize_Click);
            M_TypeSize4.Click += new EventHandler(M_TypeSize_Click);
            M_TypeSize8.Click += new EventHandler(M_TypeSize_Click);
            M_TypeSizeStruct.Click += new EventHandler(M_TypeSize_Click);
            _typeSizes = new ToolStripMenuItem[] { M_TypeSize4, M_TypeSize2, M_TypeSize1, M_TypeSize8, M_TypeSizeStruct };

            M_StructDefine.Click += new EventHandler(M_Struct_Click);
            M_StructView.Click += new EventHandler(M_Struct_Click);
            _structWhat = new ToolStripMenuItem[] { M_StructDefine, M_StructView };

            M_ViewHorizontral.Click += new EventHandler(M_View_Click);
            M_ViewVertical.Click += new EventHandler(M_View_Click);
            _views = new ToolStripMenuItem[] { M_ViewHorizontral, M_ViewVertical };

            //M_ViewMsgList.Click += new EventHandler(M_ViewMsgList_Click);
            M_ViewConnLed.Click += new EventHandler(M_ViewConnLed_Click);
            M_ViewConnModePos.Click += new EventHandler(M_ViewConnModePos_Click);
            M_ViewEchoMode.Click += new EventHandler(M_ViewEchoMode_Click);
            M_ViewHeaderDefine.Click += new EventHandler(M_ViewHeaderDefine_Click);
            M_ViewProgress.Click += new EventHandler(M_ViewProgress_Click);
            //M_ViewMsgBox.Click += new EventHandler(M_ViewMsgBox_Click);
            M_ViewTimerSettings.Click += new EventHandler(M_ViewTimerSettings_Click);
            M_ViewAnalysis.Click += new EventHandler(M_ViewAnalysis_Click);
            M_ViewNetInfo.Click += new EventHandler(M_ViewNetInfo_Click);
            M_ViewListControlButtons.Click += new EventHandler(M_ViewListControlButtons_Click);
            M_CutData.Click += new EventHandler(M_CutData_Click);
            
           
            M_LoadMsg.Click += new EventHandler(M_Load_Click);
            M_LoadMsgConfig.Click += new EventHandler(M_Load_Click);
            M_LoadConfig.Click += new EventHandler(M_Load_Click);
            M_LoadData.Click += new EventHandler(M_Load_Click);
            M_LoadRecvStruct.Click += new EventHandler(M_Load_Click);
            M_LoadMsgStruct.Click += new EventHandler(M_Load_Click);

            M_Reserved.Click += new EventHandler(M_Save_Click);
            M_SaveMsgConfig.Click += new EventHandler(M_Save_Click);
            M_SaveData.Click += new EventHandler(M_Save_Click);


            M_Help.Click += new EventHandler(M_Help_Click);
            M_VersionInfo.Click += new EventHandler(M_Help_Click);
            
            Ch_IsStringWithNull.CheckedChanged += new EventHandler(Ch_IsStringWithNull_CheckedChanged);
            T_IP.E_IPTextChanged += new TextChangedEventHandler(T_IP_E_IPTextChanged);
            T_Port.TextChanged += new EventHandler(T_Port_TextChanged);
            #endregion

            #region before Ip load..
            String filePath = _saveEnvPath;
            if (File.Exists(filePath))
            {
               // FileStream file = null;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(_saveEnvPath);
                XmlNodeList xEnvs = xDoc.SelectNodes("//Peer//Envs//Env");
                for(int i=0 ;i<xEnvs.Count; i++){
                    String name = xEnvs[i].Attributes["Name"].Value;
                    String value = xEnvs[i].InnerText;
                    _env[name] = value;
                    switch (name)
                    {
                        case "IP":
                            T_IP.IpAddress = value;
                            break;
                        case "Port":
                            T_Port.U_Text = value;
                            break;
                        case "Endian":
                            _endian = (value.ToLower().Equals("big")) ? Endians.Big : Endians.Little;
                            L_Endian.Text = _endian.ToString();
                            M_EndianBig.Checked = (_endian == Endians.Big);
                            M_EndianLittle.Checked = (_endian == Endians.Little);
                            M_Endian_Click(_endians[(int)_endian], null);
                            break;
                        case "ConnPos":
                            _connPos = (value.ToLower().Equals("server")) ? NetPosition.Server : NetPosition.Client;
                            L_ConnSite.Text = _connPos.ToString();
                            M_ConnSiteServer.Checked = R_ConnPosServer.Checked = (_connPos == NetPosition.Server);
                            M_ConnSiteClient.Checked = R_ConnPosClient.Checked = (_connPos == NetPosition.Client);
                            M_ConnSite_Click(_conPosList[(int)_connPos], null);
                            break;
                        case "ConnMode":
                            _connMode = (value.ToLower().Equals("tcp")) ? ConnModes.Tcp : ConnModes.Udp;
                            L_ConnType.Text = _connMode.ToString();
                            M_ConnModeTcp.Checked = R_ConnModeTcp.Checked = (_connMode == ConnModes.Tcp);
                            M_ConnModeUdp.Checked = R_ConnModeUdp.Checked = (_connMode == ConnModes.Udp);
                            M_ConnType_Click(_connTypes[(int)_connMode], null);
                            break;
                        case "ResponseMode":
                            int modeInt;
                            if (int.TryParse(value, out modeInt) && _responseModeTexts.Length<=(modeInt+1)) _responseMode = (ResponseMode)modeInt;
                            else _responseMode = (ResponseMode)_responseModeTexts.ToList().IndexOf(value);
                            if (_responseMode < 0)
                            {
                                _responseMode = ResponseMode.EchoNext;
                            }
                            M_EchoMode_Click(_echoModes[(int)_responseMode], null);
                            R_EchoMode.SelectedIndex = (int)_responseMode;
                            break;
                        case "StringWithNull":
                            bool stringWithNull = (value.ToLower().Equals("true")) ? true : false;
                            M_StringWithNull.Checked = stringWithNull;
                            Ch_IsStringWithNull.Checked = stringWithNull;
                            //_env["StringWithNull"] = (M_StringWithNull.Checked) ? "True" : "False";
                            break;
                        case "CutData":
                            bool cutData = (value.ToLower().Equals("true")) ? true : false;
                            M_CutData.Checked = cutData;
                            Ch_CutData.Checked = cutData;
                            //_env["CutData"] = (M_CutData.Checked) ? "True" : "False";
                            break;
                        case "GroupName":
                            _savedList = new ConMsgList(value);
                            break;
                    }
                }
#region old
                /*
                try
                {
                    file = File.OpenRead(_saveEnvPath);
                    try
                    {
                        XmlReaderSettings setting = new XmlReaderSettings();
                        setting.IgnoreWhitespace = true;
                        
                        xReader = XmlReader.Create(file,setting);
                        
                        xReader.MoveToContent();
                        while (xReader.Read())
                        {
                            String value = xReader.ReadString();
                            _env[xReader.Name] = value;
                            switch (xReader.Name)
                            {
                                case "IP":
                                    T_IP.IpAddress = value;
                                    break;
                                case "Port":
                                    T_Port.U_Text = value;
                                    break;
                                case "Endian":
                                    _endian = (value.ToLower().Equals("big")) ? Endians.Big : Endians.Little;
                                    L_Endian.Text = _endian.ToString();
                                    M_EndianBig.Checked = (_endian == Endians.Big);
                                    M_EndianLittle.Checked = (_endian == Endians.Little);
                                    M_Endian_Click(_endians[(int)_endian], null);
                                    break;
                                case "ConnPos":
                                    _connPos = (value.ToLower().Equals("server"))? NetPosition.Server:NetPosition.Client;
                                    L_ConnSite.Text = _connPos.ToString();
                                    M_ConnSiteServer.Checked = R_ConnPosServer.Checked = (_connPos == NetPosition.Server);
                                    M_ConnSiteClient.Checked = R_ConnPosClient.Checked = (_connPos == NetPosition.Client);
                                    M_ConnSite_Click(_conPosList[(int)_connPos], null);
                                    break;
                                case "ConnMode":
                                    _connMode = (value.ToLower().Equals("tcp"))? ConnModes.Tcp: ConnModes.Udp;
                                    L_ConnType.Text = _connMode.ToString();
                                    M_ConnModeTcp.Checked = R_ConnModeTcp.Checked = (_connMode == ConnModes.Tcp);
                                    M_ConnModeUdp.Checked = R_ConnModeUdp.Checked = (_connMode == ConnModes.Udp);
                                    M_ConnType_Click(_connTypes[(int)_connMode], null);
                                    break;
                                case "ResponseMode":
                                    _responseMode = (ResponseMode)(int.Parse(value));
                                    M_EchoMode_Click(_echoModes[(int)_responseMode], null);
                                    break;
                                case "StringWithNull":
                                    bool stringWithNull = (value.ToLower().Equals("true")) ? true : false;
                                    M_StringWithNull.Checked = stringWithNull;
                                    Ch_IsStringWithNull.Checked = stringWithNull;
                                    _env["StringWithNull"] = (M_StringWithNull.Checked) ? "True" : "False";
                                    break;
                                case "CutData":
                                    bool cutData = (value.ToLower().Equals("true")) ? true : false;
                                    M_CutData.Checked = cutData;
                                    Ch_CutData.Checked = cutData;
                                    _env["CutData"] = (M_CutData.Checked) ? "True" : "False";
                                    break;
                                case "GroupName":
                                    _savedList = new ConMsgList(value);
                                    break;
                            }
                        }
                        xReader.Close();
                    }
                    catch { }
                    if (xReader != null) xReader.Close();
                    if(file!=null) file.Close();
                    
                    
                }
                catch {
                    if (file != null) file.Close();
                }
                 */
#endregion
            }
            #endregion

            #region buttons
            //B_Send.Click += new EventHandler(B_Send_Click);

            B_ApplyHeader.Click += new EventHandler(B_ApplyHeader_Click);
            B_ResponseTimer.Click += new EventHandler(B_ResponseTimer_Click);
            B_Start.Click += new EventHandler(B_Start_Click);
            B_Stop.Click += new EventHandler(B_Stop_Click);
            B_StopRecv.Click += new EventHandler(B_StopRecv_Click);
            //B_MakeMsg.Click += new EventHandler(B_MakeMsg_Click);

            //B_ShowList.Click += new EventHandler(B_ShowList_Click);
            B_RunNext.Click += new EventHandler(B_RunNext_Click);
            #endregion

            Ch_CutData.CheckedChanged += new EventHandler(Ch_CutData_CheckedChanged);

            L_MyIp.Text = NetFunctions.getMyIP();
            L_MsgSize.Text = "0";
            this.Resize += new EventHandler(TestServerForm_Resize);
            //T_Msg.KeyDown += new KeyEventHandler(T_Msg_KeyDown);
            //this.Move += new EventHandler(MainForm_Move);
            //_savedList.Show();//일단 한번 show했다가 hide해야 제대로 동작한다.
            //_savedList.Hide();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible == true)
            {
                StructXMLParser.VariablesList["index"] = new VariableInfo("Index", sendIndex);

            }
            base.OnVisibleChanged(e);
        }

        void T_Port_TextChanged(object sender, EventArgs e)
        {
            _env["Port"] = T_Port.Text;
        }

        void T_IP_E_IPTextChanged(object sender, TextChangedEventArgs e)
        {
            _env["IP"] = e.Text;
        }

        public Endians Endian { get { return _endian; } }

        public ConnModes ConnMode { get { return _connMode; } }

        public NetPosition ConnPos { get { return _connPos; } }

        public bool IsStringWithNull { get { return Ch_IsStringWithNull.Checked; } }

        public StringEncodings StringEncoding { get { return _encoding; } }

        public void SetName(String name)
        {
            this.Name = name;
            L_PeerName.Text = name;
        }

        void D_History_E_ContextMenuClicked(object sender, EasyGridMenuClickArgs args)
        {
            switch (args.Text)
            {
                case "Clear History":
                    D_History.ClearData();
                    break;
            }
        }


        /*

        void MainForm_Move(object sender, EventArgs e)
        {
            try
            {
                _savedList.BringToFront();
                _savedList.SetBounds(this.Location.X - _savedList.Width, this.Location.Y, 0, 0, BoundsSpecified.Location);
            }
            catch { }
        }
        
       

        
        void M_ViewMsgList_Click(object sender, EventArgs e)
        {
            _savedList.setEnv(SendMsgToNet, _endian, Ch_IsStringWithNull.Checked, (ConMsgList.StrEncoding)_encoding);
            _savedList.Show();
        }
         */

        #region functions for useful
        /*
        public int CalculateMsgSize()
        {
            BuildMsg();
            return _sendSize;
        }
        */
        //Byte[] _sendBuff = new Byte[8192];
        int _sendCount = 0;
        int _totalSendBytes = 0;
        /*
        public void SendMsg()
        {
            //BuildMsg();
            try
            {
                if (_sendSize > 0)
                {
                    SendMsgToNet(_sendBuff, 0, _sendSize);
                }
            }
            catch
            {

            }
        }
         */
        public int SendMsgToNet(Byte[] buff, int offset, int size, CPacketStruct parser)
        {
            
            String text = getDataString(parser, buff, offset, size);
            if (_conn.isConnected())
            {
                int ret = _conn.Interface.U_Write(buff, size);

                _sendCount++;
                Pr_SendProgress.Value = _sendCount % 100;
                
                sendIndex[0] = _sendCount.ToString();

                _totalSendBytes += ret;
                L_Tx.Text = _sendCount + "(" + _totalSendBytes + ")";
                if (Ch_CutData.Checked)
                {
                    string tooltip = text;
                    text = (text.Length > 100) ? text.Substring(0, 100) + "..." : text;
                    EasyGridRow row = D_History.AddARow(new object[] { ret.ToString(), "This", text });
                    row[Titles.data].ToolTipText = tooltip;
                }
                else
                {
                    EasyGridRow row = D_History.AddARow(new object[] { ret.ToString(), "This", text });
                }
                //_history.Add(new dataHistory(ret, text, "This"));
                return ret;
            }
            else return -1;

        }

        /*
        public void BuildMsg()
        {

            String msg = T_Msg.Text.Trim();

            _sendSize = 0;


            if (_msgFormat == MsgFormat.String)
            {
                msg += " "; //for adding null on end of string.
                switch (_encoding)
                {
                    case StringEncodings.Unicode:
                        _sendSize = Encoding.Unicode.GetBytes(msg, 0, msg.Length, _sendBuff, 0);
                        break;
                    case StringEncodings.UTF7:
                        _sendSize = Encoding.UTF7.GetBytes(msg, 0, msg.Length, _sendBuff, 0);
                        break;
                    case StringEncodings.UTF32:
                        _sendSize = Encoding.UTF32.GetBytes(msg, 0, msg.Length, _sendBuff, 0);
                        break;
                    case StringEncodings.UTF8:
                    default:
                        _sendSize = Encoding.UTF8.GetBytes(msg, 0, msg.Length, _sendBuff, 0);
                        break;

                }
                _sendBuff[_sendSize - 1] = 0;
            }
            else
            {
                String[] kinds = msg.Split("/".ToCharArray());
                int totalSendSize = 0;
                int structIndex = 0;
                Boolean unsigned = false;
                for (int i = 0; i < kinds.Length; i++)
                {
                    String[] units;
                    units = kinds[i].Split(",".ToCharArray());//형식은 type,값,값/type,값,값...형식으로 이루어짐.

                    int typeSize = 4;
                    if (units[units.Length - 1].Length == 0) units[units.Length - 1] = "0";
                    String type = units[0].Trim().ToLower();
                    Array packets = getArray(type, units, out typeSize, out unsigned);

                    if (packets == null || packets.Length < 1 || typeSize<0)
                    {
                        MessageBox.Show("타입명세실패. 타입명세 후 값을 적으십시오.\r\n 형식: 타입,데이터,데이터/타입,데이터,데이터,데이터...\r\n ex> int,1,2,3/short,1,2,3");
                        _isSendMsgOk = false;
                        return;
                    }
                    //Byte[] buff;

                    if (type.Equals("char") || type.Equals("string"))
                    {
                        Buffer.BlockCopy(packets, 0, _sendBuff, totalSendSize, packets.Length);
                        totalSendSize += packets.Length;

                    }
                    else
                    {
                        for (int k = 1; k < units.Length; k++)
                        {
                            TypeHandling.TypeName typeName = TypeHandling.getTypeKind(units[k]);
                            
                            if (typeName == TypeHandling.TypeName.Integer)
                            {

                                if(unsigned == false){
                                    if (typeSize == 1) 
                                        packets.SetValue(SByte.Parse(units[k]), k - 1);
                                    else if (typeSize == 2) 
                                        packets.SetValue(Int16.Parse(units[k]), k - 1);
                                    else if (typeSize == 4)
                                        packets.SetValue(Int32.Parse(units[k]), k - 1);
                                    else 
                                        packets.SetValue(Int64.Parse(units[k]), k - 1);
                                }else{
                                    if (typeSize == 1) 
                                        packets.SetValue(Byte.Parse(units[k]), k - 1);
                                    else if (typeSize == 2) 
                                        packets.SetValue(UInt16.Parse(units[k]), k - 1);
                                    else if (typeSize == 4)
                                        packets.SetValue(UInt32.Parse(units[k]), k - 1);
                                    else 
                                        packets.SetValue(UInt64.Parse(units[k]), k - 1);
                                }
                            }
                            else if (typeName == TypeHandling.TypeName.HexString)
                            {

                                if (typeSize == 1) packets.SetValue((byte)TypeHandling.getHexNumber(units[k]), k - 1);
                                else if (typeSize == 2) packets.SetValue((short)TypeHandling.getHexNumber(units[k]), k - 1);
                                else if (typeSize == 4) packets.SetValue((int)TypeHandling.getHexNumber(units[k]), k - 1);
                                else packets.SetValue((Int64)TypeHandling.getHexNumber(units[k]), k - 1);
                            }
                            else if (typeName == TypeHandling.TypeName.Float)
                            {
                                if (typeSize == 4) packets.SetValue(float.Parse(units[k]), k - 1);
                                else packets.SetValue(Double.Parse(units[k]), k - 1);
                            }
                            else if (typeName == TypeHandling.TypeName.Bin)
                            {
                                if (typeSize == 1) packets.SetValue((byte)TypeHandling.getBinNumber(units[k]), k - 1);
                                else if (typeSize == 2) packets.SetValue((short)TypeHandling.getBinNumber(units[k]), k - 1);
                                else if (typeSize == 4) packets.SetValue((int)TypeHandling.getBinNumber(units[k]), k - 1);
                                else packets.SetValue((Int64)TypeHandling.getBinNumber(units[k]), k - 1);
                            }

                        }
                        //buff = new Byte[Marshal.SizeOf(packets.GetValue(0)) * packets.Length];
                        if (_endian == Endians.Big) Swaper.swapWithSize(packets, _sendBuff, typeSize, Buffer.ByteLength(packets), 0, totalSendSize);
                        else Buffer.BlockCopy(packets, 0, _sendBuff, totalSendSize, Buffer.ByteLength(packets));
                        totalSendSize += Buffer.ByteLength(packets);

                    }
                }

                _sendSize = totalSendSize;
            }
            _isSendMsgOk = true;
        }
        */
        void makeStruct(CPacketStruct ns)
        {
            _structDic.Clear();
            _structList.Clear();
            String[] arr;
            int openBracket = -1;
            int closeBracket = -1;
            String numStr;
            String[] token;

            for (int i = 0; i < ns.Count; i++)
            {
                arr = new String[ns.Items[i].size];

                openBracket = ns.Items[i].InitString.IndexOf('{');
                closeBracket = ns.Items[i].InitString.IndexOf('}');
                if (openBracket >= 0)
                {
                    numStr = ns.Items[i].InitString.Substring(openBracket + 1, closeBracket - openBracket - 1); //브래킷 내부의 내용 가져옴
                }
                else
                {
                    numStr = ns.Items[i].InitString;
                }
                token = numStr.Split(",".ToCharArray());


                for (int j = 0; j < arr.Length; j++)
                {
                    if (token.Length > j) numStr = token[j];
                    arr[j] = numStr;
                }
                _structDic.Add(ns.Items[i].Name, arr);
                _structList.Add(arr);
            }
        }

        /// <summary>
        /// int,1,2,3/short,1,2,3  형태의 메시지를 보고 알맞는 array를 생성해 돌려줌..
        /// </summary>
        /// <param name="type"></param>
        /// <param name="units"></param>
        /// <param name="typeSize"></param>
        /// <param name="unsigned"></param>
        /// <returns></returns>
        private Array getArray(string type, String[] units, out int typeSize, out Boolean unsigned)
        {
            Array packets = null;
            unsigned = false;
            if (type.Equals("int64"))
            {
                packets = new Int64[units.Length - 1];
                typeSize = 8;
            }
            else if (type.Equals("uint64"))
            {
                packets = new UInt64[units.Length - 1];
                typeSize = 8;
                unsigned = true;
            }
            if (type.Equals("uint"))
            {
                packets = new uint[units.Length - 1];
                typeSize = 4;
                unsigned = true;
            }
            else if (type.Equals("ushort"))
            {
                packets = new ushort[units.Length - 1];
                typeSize = 2;
                unsigned = true;
            }
            else if (type.Equals("int"))
            {
                packets = new int[units.Length - 1];
                typeSize = 4;
            }
            else if (type.Equals("short"))
            {
                packets = new short[units.Length - 1];
                typeSize = 2;
            }
            else if (type.Equals("byte")||type.Equals("uchar")||type.Equals("unsigned char")||type.Equals("1byte"))
            {
                packets = new byte[units.Length - 1];
                typeSize = 1;
                unsigned = true;
            }
            else if (type.Equals("sbyte"))
            {
                packets = new sbyte[units.Length - 1];
                typeSize = 1;
                unsigned = false;
            }
            else if (type.Equals("char") || type.Equals("string"))
            {
                if (Ch_IsStringWithNull.Checked) units[1] += "\0";
                if (_encoding == StringEncodings.UTF8) packets = Encoding.UTF8.GetBytes(units[1]);
                else if (_encoding == StringEncodings.Unicode) packets = Encoding.Unicode.GetBytes(units[1]);
                else if (_encoding == StringEncodings.UTF32) packets = Encoding.UTF32.GetBytes(units[1]);
                else if (_encoding == StringEncodings.UTF7) packets = Encoding.UTF7.GetBytes(units[1]);
                typeSize = packets.Length;
                //packets.SetValue(packets.Length - 1, 0);
            }
            else if (type.Equals("long"))
            {
                packets = new Int64[units.Length - 1];
                typeSize = 8;
            }
            else if (type.Equals("float"))
            {
                packets = new float[units.Length - 1];
                typeSize = 4;
            }
            else if (type.Equals("double"))
            {
                packets = new Double[units.Length - 1];
                typeSize = 8;
            }
            else
            {
                typeSize = -1;
            }

            return packets;
        }

        private Array getArray(int typeSize, int arraySize)
        {

            switch (typeSize)
            {
                case 2:
                    return new short[arraySize];
                case 4:
                    if (_msgFormat == MsgFormat.Float || _msgFormat == MsgFormat.ELog) return new float[arraySize];
                    else return new int[arraySize];
                case 8:
                    if (_msgFormat == MsgFormat.Float || _msgFormat == MsgFormat.ELog) return new double[arraySize];
                    else return new Int64[arraySize];
                case 1:
                case -1:
                default:
                    return new byte[arraySize];

            }
        }

        private String getDataString(CPacketStruct parser, Byte[] buff, int offset, int size)
        {
            String dataStr = "";

            
            if (_msgFormat != MsgFormat.String)
            {
                if (_dataTypeSize == -1) //struct
                {
                    int structIndex = offset;
                    int typeSize = 0;
                    Array dataBuff;
                    String unit="";
                    int buffSize = 0;

                    for (int i = 0; i < parser.Count; i++)
                    {
                        typeSize = TypeHandling.getTypeSize(parser.Items[i].TypeString);
                        if (typeSize < 0)
                        {
                            dataBuff = null;
                        }
                        else
                        {
                            dataBuff = getArray(typeSize, parser.Items[i].size);
                        }
                        
                        if(dataBuff!=null) buffSize = Buffer.ByteLength(dataBuff);

                        if (i != 0) dataStr += "/"; //타입이 바뀌거나 구조체에서 다른 변수로 넘어갔을 때.

                        dataStr += parser.Items[i].TypeString;
                        if (parser.Items[i].TypeString.Equals("string"))
                        {
                            dataStr += ",";
                            unit = "";
                            int strSize = 0;
                            for (int j = structIndex; j < size; j++)
                            {
                                if (buff[j] == 0)
                                {
                                    strSize = j - structIndex;
                                    break;
                                }
                            }

                            switch (_encoding)
                            {
                                case StringEncodings.UTF7:
                                    unit += Encoding.UTF7.GetString(_recvBuff,structIndex, strSize);
                                    break;
                                case StringEncodings.UTF8:
                                    unit += Encoding.UTF8.GetString(_recvBuff, structIndex, strSize);
                                    break;
                                case StringEncodings.UTF32:
                                    unit += Encoding.UTF32.GetString(_recvBuff, structIndex, strSize);
                                    break;
                                case StringEncodings.Unicode:
                                default:
                                    unit += Encoding.Unicode.GetString(_recvBuff, structIndex, strSize);
                                    break;

                            }
                            structIndex += strSize + 1; // 1 is null char.
                            dataStr += unit;
                            
                        }
                        else
                        {
                            try
                            {
                                if (_endian == Endians.Big)
                                    Swaper.swapWithSize(buff, dataBuff, typeSize, buffSize, structIndex, 0);
                                else
                                    Buffer.BlockCopy(buff, structIndex, dataBuff, 0, buffSize);
                            }
                            catch
                            {
                                MessageBox.Show("받은 메시지가 구조체보다 작습니다. 채울 수 있는 곳까지만 채웁니다.");
                            } //채울 수 있을 때까지만 채우고 에러가 나면 그냥 무시하고 나옴.
                            for (int j = 0; j < dataBuff.Length; j++)
                            {
                                
                                switch (_msgFormat)
                                {
                                    case MsgFormat.Bin:
                                        unit = TypeHandling.getBinNumber(dataBuff.GetValue(j), typeSize * 8, ":");
                                        break;
                                    case MsgFormat.Hex:
                                        unit = String.Format("0x{0:X" + (typeSize * 2) + "}", dataBuff.GetValue(j));
                                        break;
                                    case MsgFormat.Float:
                                        unit = String.Format("{0:f8}", dataBuff.GetValue(j));
                                        break;
                                    case MsgFormat.ELog:
                                        unit =  String.Format("{0:e8}", dataBuff.GetValue(j));
                                        break;
                                    case MsgFormat.Dec:
                                    default:
                                        unit = dataBuff.GetValue(j).ToString();
                                        break;

                                }
                                dataStr += ","+unit;
                               
                            }//struct내부의 변수(배열) 하나의 내부를 채웠음.
                        }
                        structIndex += buffSize;
                        
                    }//모든 struct를 채웠음.

                }
                else
                {
                    Array dataBuff = getArray(1, size);

                    if (_endian == Endians.Big) Swaper.swapWithSize(buff, dataBuff, _dataTypeSize, Buffer.ByteLength(dataBuff));
                    else Buffer.BlockCopy(buff, 0, dataBuff, 0, Buffer.ByteLength(dataBuff));


                    //_dataTypeSize = dataTypeSize;
                    
                    for (int i = 0; i < dataBuff.Length; i++)
                    {
                        switch (_msgFormat)
                        {
                            case MsgFormat.Bin:
                                dataStr += ((i != 0) ? "," : "") + TypeHandling.getBinNumber(dataBuff.GetValue(i), _dataTypeSize * 8, ":");
                                break;
                            case MsgFormat.Dec:
                                dataStr += ((i != 0) ? "," : "") + dataBuff.GetValue(i);
                                break;
                            case MsgFormat.Hex:
                                dataStr += ((i != 0) ? "," : "") + String.Format("0x{0:X" + (_dataTypeSize * 2) + "}", dataBuff.GetValue(i));
                                break;
                            case MsgFormat.Float:
                                dataStr += ((i != 0) ? "," : "") + String.Format("{0:f8}", dataBuff.GetValue(i));
                                break;
                            case MsgFormat.ELog:
                                dataStr += ((i != 0) ? "," : "") + String.Format("{0:e8}", dataBuff.GetValue(i));
                                break;
                            
                        }
                    }
                }
            }
            else
            {

                switch (_encoding)
                {
                    case StringEncodings.UTF7:
                        dataStr = Encoding.UTF7.GetString(buff, 0, size);
                        break;
                    case StringEncodings.UTF8:
                        dataStr = Encoding.UTF8.GetString(buff, 0, size);
                        break;
                    case StringEncodings.UTF32:
                        dataStr = Encoding.UTF32.GetString(buff, 0, size);
                        break;
                    case StringEncodings.Unicode:
                    default:
                        dataStr = Encoding.Unicode.GetString(buff, 0, size);
                        break;

                }

            }
            
            return dataStr;

        }

        void FillRecvDataViewer()
        {
            if (_dlgViewStruct != null)
            {
                _dlgViewStruct.makeTree(_nsReceived, _structDic, _structList);
            }
        }

        int _dataSize = 0;
        private int dataSizeFromHeader()
        {
            if (_recvHeader == null || _headerSize == 0) return -1;
            if (_endian == Endians.Big)
            {
                Swaper.swapWithSize(_recvHeader, _headerSizeItem, _headerDataItemSize, _headerDataItemSize, _dataItemOffset, 0);
            }
            else
            {
                Buffer.BlockCopy(_recvHeader, _dataItemOffset, _headerSizeItem, 0, _headerDataItemSize);
            }
            _dataSize = (int)(_headerSizeItem.GetValue(0));
            return _dataSize;
            // if(_data
        }

        void CheckAndTheOtherUnCheck(ToolStripMenuItem c, ToolStripMenuItem[] others)
        {
            c.Checked = true;
            foreach (ToolStripMenuItem other in others)
            {
                if (other.Equals(c) == false) other.Checked = false;
            }
        }
        void CheckAndTheOtherUnCheckRadio(RadioButton c, RadioButton[] others)
        {
            c.Checked = true;
            foreach (RadioButton other in others)
            {
                if (other.Equals(c) == false) other.Checked = false;
            }
        }
#endregion

        #region RadioButton & MenuStrip Click/////////////////////////////

        void R_EchoMode_E_RadioButtonSelected(IndexSelectedEventArgs e)
        {
            for (int i = 0; i < _echoModes.Length; i++)
            {
                if (e.Index == i) _echoModes[i].Checked = true;
                else _echoModes[i].Checked = false;

            }
            setResponseMode((ResponseMode)e.Index);
            _env["ResponseMode"] = e.Index.ToString();
            //saveCfg();
            //saveData();
            SaveEnvs();
        }

        void Ch_IsStringWithNull_CheckedChanged(object sender, EventArgs e)
        {

            if (_savedList != null) _savedList.setEnv(SendMsgToNet, _endian, Ch_IsStringWithNull.Checked, _encoding);
            if (M_StringWithNull.Checked != Ch_IsStringWithNull.Checked)
                M_StringWithNull.Checked = Ch_IsStringWithNull.Checked;
            _env["StringWithNull"] = (M_StringWithNull.Checked) ? "True" : "False";
            SaveEnvs();
        }
        void B_RunNext_Click(object sender, EventArgs e)
        {
            if (_savedList != null) _savedList.RunNext();
        }


        public void SetMsgList(String groupName)
        {
            if (_env.ContainsKey("GroupName")==false || _env["GroupName"].Equals(groupName) == false)
            {
                _env["GroupName"] = groupName;
                _savedList = new ConMsgList(groupName);// msgList;
                SaveEnvs();
                _savedList.setEnv(SendMsgToNet, Endian, IsStringWithNull, StringEncoding);
            }
            
        }
        public void AddMsgListToPanel( Panel targetPanel)
        {
            StructXMLParser.FunctionsList["index"] = new FunctionInfo("index", getIndex);
            targetPanel.Controls.Clear();
            if (_savedList != null)
            {
                _savedList.Dock = DockStyle.Fill;
                targetPanel.Controls.Add(_savedList);
                
                _savedList.setEnv(
                    SendMsgToNet,
                    Endian,
                    IsStringWithNull,
                    (StringEncodings)StringEncoding);
                
                _savedList.Show();
            }

        }

        public object getIndex(params object[] args)
        {
            return sendIndex[0];
        }

        public String Env(String name)
        {
            if (_env.Keys.Contains(name)) return _env[name];
            else return null;
        }

        public void InitList()
        {
            if (_savedList != null) _savedList.initList();
        }

        void M_ViewListControlButtons_Click(object sender, EventArgs e)
        {
            M_ViewListControlButtons.Checked = !M_ViewListControlButtons.Checked;
            G_ListControlButtons.Visible = M_ViewListControlButtons.Checked;
        }

        void M_StringWithNull_Click(object sender, EventArgs e)
        {
            M_StringWithNull.Checked = !M_StringWithNull.Checked;
            Ch_IsStringWithNull.Checked = M_StringWithNull.Checked;
            _env["StringWithNull"] = (M_StringWithNull.Checked) ? "True" : "False";
            SaveEnvs();
        }

        void Ch_CutData_CheckedChanged(object sender, EventArgs e)
        {
            M_CutData.Checked = Ch_CutData.Checked;
        }

        void M_CutData_Click(object sender, EventArgs e)
        {
            M_CutData.Checked = !M_CutData.Checked;
            Ch_CutData.Checked = M_CutData.Checked;
            _env["CutData"] = (M_CutData.Checked) ? "True" : "False";
            SaveEnvs();
        }
        #region M_View...
        void M_ViewNetInfo_Click(object sender, EventArgs e)
        {
            M_ViewNetInfo.Checked = !M_ViewNetInfo.Checked;
            G_NetInfo.Visible = M_ViewNetInfo.Checked;
        }

        void M_ViewAnalysis_Click(object sender, EventArgs e)
        {
            M_ViewAnalysis.Checked = !M_ViewAnalysis.Checked;
            G_Analysis.Visible = M_ViewAnalysis.Checked;
        }

        void M_ViewTimerSettings_Click(object sender, EventArgs e)
        {
            M_ViewTimerSettings.Checked = !M_ViewTimerSettings.Checked;
            G_TimerSettings.Visible = M_ViewTimerSettings.Checked;
        }

        void M_ViewProgress_Click(object sender, EventArgs e)
        {
            M_ViewProgress.Checked = !M_ViewProgress.Checked;
            G_Progress.Visible = M_ViewProgress.Checked;
        }

        void M_ViewHeaderDefine_Click(object sender, EventArgs e)
        {
            M_ViewHeaderDefine.Checked = !M_ViewHeaderDefine.Checked;
            G_HeaderDefine.Visible = M_ViewHeaderDefine.Checked;
        }

        void M_ViewEchoMode_Click(object sender, EventArgs e)
        {
            M_ViewEchoMode.Checked = !M_ViewEchoMode.Checked;
            G_EchoMode.Visible = M_ViewEchoMode.Checked;
        }

        void M_ViewConnModePos_Click(object sender, EventArgs e)
        {
            M_ViewConnModePos.Checked = !M_ViewConnModePos.Checked;
            G_ConnModePos.Visible = M_ViewConnModePos.Checked;
        }

        void M_ViewConnLed_Click(object sender, EventArgs e)
        {
            M_ViewConnLed.Checked = !M_ViewConnLed.Checked;
            G_ConnLeds.Visible = M_ViewConnLed.Checked;
        }

        #endregion

        enum HelpCode { HelpWindow = 0, VersionInfo };
        void M_Help_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            switch ((HelpCode)tag)
            {
                case HelpCode.HelpWindow:
                    HelpPopup popup = new HelpPopup();
                    popup.Location = this.Location;
                    popup.ShowDialog();

                    break;
                case HelpCode.VersionInfo:
                    MessageBox.Show(_version, "버전정보");
                    break;
            }
        }

        enum DoWhatStruct { Define = 0, View = 1 };
        CPacketStruct _nsReceived = new CPacketStruct();
        ListDic<String, String[]> _structDic = new ListDic<string, string[]>();
        List<String[]> _structList = new List<string[]>();
        DlgViewStruct _dlgViewStruct = null;
        void M_Struct_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            DlgDefineStruct dlg;
            M_TypeSize_Click(M_TypeSizeStruct, e);//type을 struct로바꿈..
            switch ((DoWhatStruct)tag)
            {
                case DoWhatStruct.Define:

                    if (File.Exists("./recvMsg.txt")) _nsReceived.setNativeText(File.ReadAllText("./recvMsg.txt"), _endian == Endians.Big);

                    dlg = new DlgDefineStruct(_nsReceived, "./recvMsg.txt", _endian == Endians.Big);
                    dlg.Location = this.Location;
                    dlg.StartPosition = FormStartPosition.CenterParent;
                    dlg.ShowDialog(this);
                    makeStruct(_nsReceived);
                    break;
                case DoWhatStruct.View:
                    if (_nsReceived.Count == 0)
                    {
                        dlg = new DlgDefineStruct(_nsReceived, "./recvMsg.txt", _endian == Endians.Big);
                        dlg.Location = this.Location;
                        dlg.StartPosition = FormStartPosition.CenterParent;
                        dlg.ShowDialog(this);
                        makeStruct(_nsReceived);
                    }
                    else
                    {
                        if (_dlgViewStruct == null)
                        {
                            _dlgViewStruct = new DlgViewStruct(_nsReceived, _structDic, _structList);
                            _dlgViewStruct.U_PopupClosed += new PopupClosedEventHandler(_dlgViewStruct_U_PopupClosed);
                            _dlgViewStruct.Location = this.Location;
                            _dlgViewStruct.StartPosition = FormStartPosition.CenterParent;
                            _dlgViewStruct.Show();
                        }
                    }
                    
                    break;
            }
        }



        String _lowVersion = "1.81";
        void saveCfg()
        {

            FileDialog dlg = new SaveFileDialog();

            dlg.AddExtension = true;
            dlg.SupportMultiDottedExtensions = true;
            dlg.DefaultExt = "cfg.xml";
            if (_initSaveDir == null || _initSaveDir.IndexOf(Directory.GetCurrentDirectory()) > 0)
            {
                _initSaveDir = Directory.GetCurrentDirectory() + "\\cfg";
                if (Directory.Exists(_initSaveDir) == false)
                {
                    Directory.CreateDirectory(_initSaveDir);
                }
            }

            dlg.InitialDirectory = _initSaveDir;
            
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Abort || result == System.Windows.Forms.DialogResult.Cancel) return;
            _initSaveDir = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));

            TextWriter file = File.CreateText(dlg.FileName);

            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "    ";
            setting.NewLineChars = "\r\n";
            setting.NewLineHandling = NewLineHandling.Replace;

            XmlWriter xWriter = XmlWriter.Create(file, setting);


            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("TestNetConnector");
            xWriter.WriteStartElement("FileInfo");

            xWriter.WriteElementString("version", _version.Substring(_version.IndexOf('v') + 1).Trim());
            xWriter.WriteElementString("fileType", "Cfg");
            xWriter.WriteEndElement();
            for (int i = 0; i < _env.Count; i++)
            {
                xWriter.WriteElementString(_env.Keys.ElementAt(i), _env.Values.ElementAt(i));
            }
            /*
            xWriter.WriteElementString("ConnMode", ((int)_connMode).ToString());
            xWriter.WriteElementString("NetPosition", ((int)_connPos).ToString());
            xWriter.WriteElementString("Endian", ((int)_endian).ToString());
            xWriter.WriteElementString("Encoding", ((int)_encoding).ToString());
            xWriter.WriteElementString("MsgFormat", ((int)_msgFormat).ToString());
            xWriter.WriteElementString("DataTypeSize", ((int)_dataTypeSize).ToString());
            xWriter.WriteElementString("ResponseMode", ((int)_responseMode).ToString());
            xWriter.WriteElementString("MsgText", _nsSend.nativeText);
            xWriter.WriteElementString("Msg", T_Msg.Text);
             */
            xWriter.WriteEndElement();
            xWriter.WriteEndDocument();
            xWriter.Close();
            file.Close();
        }

        void saveData()
        {
            FileDialog dlg = new SaveFileDialog();

            dlg.AddExtension = true;
            dlg.SupportMultiDottedExtensions = true;
            dlg.DefaultExt = "data.xml";
            if (_initSaveDir == null || _initSaveDir.IndexOf(Directory.GetCurrentDirectory())>0)
            {
                _initSaveDir = Directory.GetCurrentDirectory() + "\\data";
                if (Directory.Exists(_initSaveDir) == false)
                {
                    Directory.CreateDirectory(_initSaveDir);
                }
            }

            dlg.InitialDirectory = _initSaveDir;
            
            DialogResult result = dlg.ShowDialog();
            
            if (result == System.Windows.Forms.DialogResult.Abort || result == System.Windows.Forms.DialogResult.Cancel) return;
            _initSaveDir = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));


            TextWriter file = File.CreateText(dlg.FileName);

            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "    ";
            setting.NewLineChars = "\r\n";
            setting.NewLineHandling = NewLineHandling.Replace;

            XmlWriter xWriter = XmlWriter.Create(file, setting);


            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("TestNetConnector");//root
            xWriter.WriteStartElement("FileInfo");//fileInfo
            xWriter.WriteElementString("version", _version.Substring(_version.IndexOf('v')+1).Trim());
            xWriter.WriteElementString("fileType", "Data");
            xWriter.WriteElementString("lineSize", D_History.RowCount.ToString());
            
            xWriter.WriteEndElement();//fileInfo
            xWriter.WriteStartElement("Data");
            foreach (EasyGridRow row in D_History.Rows)//dataHistory d in _history)
            {
                xWriter.WriteStartElement("Line");
                xWriter.WriteAttributeString("Size", row[Titles.size].Value as String);
                
                xWriter.WriteAttributeString("Source", row[Titles.ip].Value as String);
                xWriter.WriteString(row[Titles.data].Value as String);// d.data);
                xWriter.WriteEndElement();//Line
            }
            xWriter.WriteEndElement();//data
            xWriter.WriteEndElement();//root
            xWriter.WriteEndDocument();
            xWriter.Close();
            file.Close();
        }

        void saveStruct()
        {
            FileDialog dlg = new SaveFileDialog();

            dlg.AddExtension = true;
            dlg.SupportMultiDottedExtensions = true;
            //dlg.DefaultExt = "";
            if (_initSaveDir == null || _initSaveDir.IndexOf(Directory.GetCurrentDirectory()) > 0)
            {
                _initSaveDir = Directory.GetCurrentDirectory() + "\\struct";
                if (Directory.Exists(_initSaveDir) == false)
                {
                    Directory.CreateDirectory(_initSaveDir);
                }
            }

            dlg.InitialDirectory = _initSaveDir;
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Abort || result == System.Windows.Forms.DialogResult.Cancel) return;
            _initSaveDir = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));


            File.WriteAllText(dlg.FileName + "_recv.struct.c", _nsReceived.NativeText);
            File.WriteAllText(dlg.FileName + "_send.struct.c", _nsSend.NativeText);
        }

        enum SaveMode { MsgConfig=0, Data=1, Struct=2 };
        void M_Save_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            switch ((SaveMode)tag)
            {
                case SaveMode.MsgConfig:
                    saveCfg();
                    break;
                case SaveMode.Data:
                    saveData();
                    break;
                case SaveMode.Struct:
                    saveStruct();
                    break;
            }
        }
        String _initSaveDir = null;
        void loadConfigMsg(LoadMode mode)
        {

            FileStream file = null;
            XmlReader xReader = null;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "cfg.xml";
            dlg.InitialDirectory = _initSaveDir;
            dlg.ShowDialog();

            try
            {
                file = File.OpenRead(dlg.FileName);
                try
                {
                    XmlReaderSettings setting = new XmlReaderSettings();
                    setting.IgnoreWhitespace = true;
                    
                    xReader = XmlReader.Create(file, setting);

                    xReader.MoveToContent();
                    string value = "";
                    string name = "";
                    int valueInt = 0;
                    String mother = "";
                    while (xReader.Read())
                    {
                        value = xReader.ReadString();
                        Int32.TryParse(value, out valueInt);
                        name = xReader.Name;

                        switch (name)
                        {

                            case "version":
                                String version = value;
                                if (version.CompareTo(_lowVersion) < 0)
                                {
                                    MessageBox.Show("버전이 너무 낮아서 호환되지 않을 수 있습니다.");
                                }
                                break;
                            case "fileType":
                                if (value.Equals("Cfg") == false)
                                {
                                    MessageBox.Show("이 파일은 형식이 맞지 않습니다.");
                                    break;
                                }
                                break;
                            case "ConnMode":
                                //_connMode = (ConnMode)valueInt;
                                if (mode != LoadMode.Msg) M_ConnType_Click(_connTypes[valueInt], null);
                                break;
                            case "NetPosition":
                                //_connPos = (NetPosition)valueInt;
                                if (mode != LoadMode.Msg) M_ConnSite_Click(_conPosList[valueInt], null);
                                break;
                            case "Endian":
                                //_endian = (Endian)valueInt;
                                if (mode != LoadMode.Msg) M_Endian_Click(_endians[valueInt], null);
                                break;
                            case "Encoding":
                                if (mode != LoadMode.Msg) M_Encoding_Click(_encodings[valueInt], null);
                                break;
                            case "MsgFormat":
                                if (mode != LoadMode.Msg) M_Format_Click(_formats[valueInt], null);
                                break;
                            case "DataTypeSize":
                                if (mode != LoadMode.Msg) M_TypeSize_Click(_typeSizes[valueInt], null);
                                break;
                            case "ResponseMode":
                                if (mode != LoadMode.Msg) M_EchoMode_Click(_echoModes[valueInt], null);
                                break;
                            case "MsgText":
                                _nsSend.setNativeText(value, _endian == Endians.Big);
                                break;

                        }
                    }
                    xReader.Close();
                }
                catch { }
                if (xReader != null) xReader.Close();
                if (file != null) file.Close();


            }
            catch
            {
                if (file != null) file.Close();
            }
        }
        void loadData()
        {
            FileStream file = null;
            XmlReader xReader = null;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "cfg.xml";
            dlg.InitialDirectory = _initSaveDir;
            dlg.ShowDialog();

            try
            {
                file = File.OpenRead(dlg.FileName);
                try
                {
                    XmlReaderSettings setting = new XmlReaderSettings();
                    setting.IgnoreWhitespace = true;

                    xReader = XmlReader.Create(file, setting);

                    xReader.MoveToContent();
                    string value = "";
                    int valueInt = 0;
                    while (xReader.Read())
                    {
                        value = xReader.ReadString();
                        Int32.TryParse(value, out valueInt);
                        switch (xReader.Name)
                        {

                            case "version":
                                String version = value;
                                if (version.CompareTo(_lowVersion) < 0)
                                {
                                    MessageBox.Show("버전이 너무 낮아서 호환되지 않을 수 있습니다.");
                                }
                                break;
                            case "fileType":
                                if (value.Equals("Data") == false)
                                {
                                    MessageBox.Show("올바른 파일형식이 아닙니다.");
                                    goto outOfLoop;
                                }
                                break;
                            case "Line":
                                string ip = xReader.GetAttribute("Source");
                                int size = Int32.Parse(xReader.GetAttribute("Size"));
                                string data = value;
                                if (Ch_CutData.Checked)
                                {
                                    string tooltip = data;
                                    data = (data.Length > 100) ? data.Substring(0, 100) + "..." : data;
                                    EasyGridRow row = D_History.AddARow(new object[] { size.ToString(), ip, data });
                                    row[Titles.data].ToolTipText = tooltip;
                                }
                                else
                                {
                                    EasyGridRow row = D_History.AddARow(new object[] { size.ToString(), ip, data });
                                    
                                }
                                
                                //_history.Add(new dataHistory(size, data, ip));
                                break;
                        }
                    }
                    outOfLoop:
                    xReader.Close();
                }
                catch { }
                if (xReader != null) xReader.Close();
                if (file != null) file.Close();


            }
            catch
            {
                if (file != null) file.Close();
            }
        }
        void loadRecvStruct()
        {
            StreamReader file = null;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "struct.c";
            dlg.InitialDirectory = _initSaveDir;
            dlg.ShowDialog();

            try
            {
                file = File.OpenText(dlg.FileName);
                char[] buff = new char[2048];
                String str = "";

                while (file.Read(buff, 0, 2048) > 0)
                {
                    str += new String(buff);
                }

                file.Close();
                _nsReceived.setNativeText(str, _endian == Endians.Big);
                makeStruct(_nsReceived);
            }
            catch
            {
                if (file != null) file.Close();
            }
        }
        /*
        void loadMsgStruct()
        {
            StreamReader file = null;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "struct.c";
            dlg.InitialDirectory = _initSaveDir;
            dlg.ShowDialog();

            try
            {
                file = File.OpenText(dlg.FileName);
                char[] buff = new char[2048];
                String str = "";

                while (file.Read(buff, 0, 2048) > 0)
                {
                    str += new String(buff);
                }

                file.Close();
                _nsSend.setNativeText(str);
                MakeMsg();
                //makeStruct(_nsSend);
            }
            catch(Exception e)
            {
                if (file != null) file.Close();
            }
        }
        */
        enum LoadMode { Msg = 0, MsgConfig = 1, Config, Data, StructRecv, StructSend };
        void M_Load_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            LoadMode mode = (LoadMode)tag;

            switch (mode)
            {
                case LoadMode.Data:
                    loadConfigMsg(mode);
                    break;
                case LoadMode.Config:
                    loadConfigMsg(mode);
                    break;
                case LoadMode.Msg:
                    loadConfigMsg(mode);
                    break;
                case LoadMode.MsgConfig:
                    loadConfigMsg(mode);
                    break;
                case LoadMode.StructRecv:
                    loadRecvStruct();
                    break;
            }
        }


        public enum ConnModes { Tcp = 0, Udp = 1, UdpMulticast=2 };
        ConnModes _connMode = ConnModes.Tcp;

        void M_ConnType_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem c = (ToolStripMenuItem)sender;
                int tag = Int32.Parse(c.Tag.ToString());
                _connMode = (ConnModes)tag;
                
                L_ConnType.Text = c.Text;
            }
            else
            {
                RadioButton c = (RadioButton)sender;
                int tag = Int32.Parse(c.Tag.ToString());
                _connMode = (ConnModes)tag;
                
                L_ConnType.Text = c.Text;
            }
            _env["ConnMode"] = _connMode.ToString();
            CheckAndTheOtherUnCheck(_connTypes[(int)_connMode], _connTypes);
            CheckAndTheOtherUnCheckRadio(_rconnTypes[(int)_connMode], _rconnTypes);
            switch (_connMode)
            {
                case ConnModes.Tcp:
                    if (_connPos == NetPosition.Client) setTcpClient();
                    else setTcpServer();
                    G_HeaderDefine.Enabled = true;
                    G_Timeout.Enabled = true;
                    break;
                case ConnModes.Udp:
                    if (_connPos == NetPosition.Client) setUdpClient();
                    else setUdpServer();
                    G_HeaderDefine.Enabled = false;
                    G_Timeout.Enabled = false;
                    break;
                case ConnModes.UdpMulticast:
                    if (_connPos == NetPosition.Client) setUdpClient(SendType.Multicast);
                    else setUdpServer(SendType.Multicast);
                    G_HeaderDefine.Enabled = false;
                    G_Timeout.Enabled = false;
                    break;
            }

        }

        NetPosition _connPos = NetPosition.Server;

        void M_ConnSite_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem c = (ToolStripMenuItem)sender;
                int tag = Int32.Parse(c.Tag.ToString());
                _connPos = (NetPosition)tag;
                
                L_ConnSite.Text = c.Text;
            }
            else
            {
                RadioButton c = (RadioButton)sender;
                int tag = Int32.Parse(c.Tag.ToString());
                _connPos = (NetPosition)tag;
                
                L_ConnSite.Text = c.Text;
            }

            CheckAndTheOtherUnCheck(_conPosList[(int)_connPos], _conPosList);
            CheckAndTheOtherUnCheckRadio(_rNetPositions[(int)_connPos], _rNetPositions);
            if (_connPos == NetPosition.Server)
            {
                if (_connMode == ConnModes.Tcp) setTcpServer();
                else setUdpServer();
                //if (T_IP.IpAddress.Equals("127.0.0.1")) T_IP.IpAddress = "0.0.0.0";
            }
            else
            {
                if (_connMode == ConnModes.Tcp) setTcpClient();
                else setUdpClient();
                if (T_IP.IpAddress.Equals("0.0.0.0")) T_IP.IpAddress = NetFunctions.getMyIP();
            }
            _env["ConnPos"] = _connPos.ToString();
        }

        //public enum StringEncodings { UTF8 = 0, UTF7 = 1, UTF16, UTF32, ASCII };
        StringEncodings _encoding = StringEncodings.UTF8;

        void M_Encoding_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            _encoding = (StringEncodings)tag;
            CheckAndTheOtherUnCheck(c, _encodings);
            if (_msgFormat == MsgFormat.String) L_Format.Text = "String(" + _encoding.ToString() + ")";
            _env["Encoding"] = _encoding.ToString();
        }

        Endians _endian = Endians.Big;

        void M_Endian_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            _endian = (Endians)tag;
            CheckAndTheOtherUnCheck(c, _endians);
            L_Endian.Text = _endian.ToString() + " Endian";
            _env["Endian"] = _endian.ToString();
        }

        enum ResponseMode { NoEcho=0, EchoRecved, EchoNext, EchoSelected };
        String[] _responseModeTexts = new String[] { "NoEcho", "EchoRecved", "EchoNext", "EchoSelected" };
        ResponseMode _responseMode = ResponseMode.NoEcho;

        void M_EchoMode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            setResponseMode((ResponseMode)tag);
            CheckAndTheOtherUnCheck(c, _echoModes);
            
        }

        void setResponseMode(ResponseMode mode){
            _responseMode = mode;
            L_EchoMode.Text = mode.ToString();
            _env["ResponseMode"] = _responseMode.ToString();
        }

        enum MsgFormat { Hex = 0, Dec = 1, Bin, String, Float, ELog };
        MsgFormat _msgFormat = MsgFormat.Hex;
        MsgFormat _tempMsgFormat = MsgFormat.Hex;
        void R_AnalysisMsgFormat_Click(object sender, EventArgs e)
        {
            RadioButton c = (RadioButton)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            _tempMsgFormat = _msgFormat;
            _msgFormat = (MsgFormat)tag;

            _totalRecvSize = _tempRecvSize;
            //_history.Add(new dataHistory(_totalRecvSize, getDataString(), _sentIp));

            D_History.AddARow(new object[] { _totalRecvSize.ToString(), _sentIp, getDataString(_nsReceived, _recvBuff, 0, _totalRecvSize)});// size.ToString(), ip, data });
            _totalRecvSize = 0;
            _msgFormat = _tempMsgFormat;
            _env["MsgFormat"] = _msgFormat.ToString();
        }

        void M_Format_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            _msgFormat = (MsgFormat)tag;
            CheckAndTheOtherUnCheck(c, _formats);
            if (_msgFormat == MsgFormat.String) L_Format.Text = "String(" + _encoding.ToString() + ")";
            else if (_dataTypeSize >= 0)
            {
                L_Format.Text = _msgFormat.ToString() + "(" + _dataTypeSize + "Bytes)";
            }
            else //dataTypeSize : Struct
            {
                L_Format.Text = _msgFormat.ToString() + "(Struct)";
            }
            if (_msgFormat == MsgFormat.Float || _msgFormat == MsgFormat.ELog)
            {
                if (_dataTypeSize < 4)
                {
                    M_TypeSize_Click(M_TypeSize4, null);
                }
                M_TypeSize1.Enabled = false;
                M_TypeSize2.Enabled = false;
            }
            else
            {
                M_TypeSize1.Enabled = true;
                M_TypeSize2.Enabled = true;
            }
            _env["MsgFormat"] = _msgFormat.ToString();
        }


        enum ViewOrientation { Horizontal = 0, Vertical = 1 };
        void M_View_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            int tag = Int32.Parse(c.Tag.ToString());
            CheckAndTheOtherUnCheck(c, _views);

            switch ((ViewOrientation)tag)
            {
                case ViewOrientation.Vertical:
                    SP_Spliter.Orientation = Orientation.Vertical;
                    TestServerForm_Resize(this, null);
                    break;
                case ViewOrientation.Horizontal:
                    SP_Spliter.Orientation = Orientation.Horizontal;
                    TestServerForm_Resize(this, null);
                    break;
            }
        }


        int _dataTypeSize = 1;
        enum TypeSize { b4 = 0, b2 = 1, b1 = 2, b8 = 3, Structure = 4 };
        void M_TypeSize_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem c = (ToolStripMenuItem)sender;
            CheckAndTheOtherUnCheck(c, _typeSizes);
            switch ((TypeSize)int.Parse(c.Tag.ToString()))
            {
                case TypeSize.b4://int
                    _dataTypeSize = 4;
                    break;
                case TypeSize.b2://short
                    _dataTypeSize = 2;
                    break;
                case TypeSize.b1://byte
                    _dataTypeSize = 1;
                    break;
                case TypeSize.b8://int64
                    _dataTypeSize = 8;
                    break;
                case TypeSize.Structure:
                    _dataTypeSize = -1;
                    break;

            }
            if (_msgFormat == MsgFormat.String)
            {
                M_Format_Click(M_FormatDec, null);
            }

            if (_dataTypeSize >= 0)
            {
                L_Format.Text = _msgFormat.ToString() + "(" + _dataTypeSize + "Bytes)";
            }
            else //dataTypeSize : Struct
            {
                L_Format.Text = _msgFormat.ToString() + "(Struct)";
            }
        }



        #endregion

        #region //////////////////////NetworkEvents/////////////////////////////////////
        delegate void ButtonEvent(object sender, EventArgs e);
        ButtonEvent _btnEvent;
        void _tcp_NetError(object sender, NetworkErrorEventArgs e)
        {
            switch (e.error)
            {
                case NetworkErrorEventArgs.NetErrorMsg.CONNECTION_FAIL:
                case NetworkErrorEventArgs.NetErrorMsg.DISCONNECTED:
                    Led_Connect.Off();
                    if (_connPos == NetPosition.Client)
                    {
                        if (this.InvokeRequired)
                        {
                            _btnEvent = B_Stop_Click;

                            this.Invoke(_btnEvent, B_Stop, null);
                        }
                        //B_Stop_Click(B_Stop, null);
                    }
                    break;
            }
        }


        delegate void OnTcpConnEvent(object sender, ConnectionEventArgs e);
        OnTcpConnEvent connEventOccured;
        void _tcp_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            if (this.InvokeRequired)
            {
                connEventOccured = new OnTcpConnEvent(_tcp_ConnectionEvent);
                try
                {
                    this.Invoke(connEventOccured, sender, e);
                }
                catch { }
            }
            else
            {
                if (e.connType == ConnType.Connected)
                {
                    Led_Connect.On();
                }
                else if (e.connType ==ConnType.Connecting)
                {
                    Led_Connect.SetState(ConnType.Connecting);
                }
                else if (e.connType == ConnType.Disconnected)
                {
                    Led_Connect.Off();
                }
            }
        }


        int _recvCount = 0;
        int _totalRecvBytes = 0;
        Boolean _isHeaderReceived = false;

        delegate void onRead(object sender, TransferEventArgs e);
        onRead _onReadData; ///필요없음.

        void onReceiveData(object sender, TransferEventArgs e)
        {
            if (this.InvokeRequired)
            {
                _onReadData = new onRead(onReceiveData);
                this.Invoke(_onReadData, sender, e);
            }
            else
            {
                _recvCount++;
                Pr_RecvProgress.Value = _recvCount % 100;
                sendIndex[0] = _recvCount.ToString();
                L_Rx.Text = _recvCount + "(" + _totalRecvBytes + ")";

                int dataSize, size;
                if (_headerSize > 0)
                {
                    if (_isHeaderReceived == false)
                    {
                        if (_conn.Available < _headerSize) return;
                        size = _conn.Interface.U_Read(_recvHeader, _headerSize);
                        
                        if (size != _headerSize)
                        {
                            //이런 경우는 값을 받았지만 버퍼에서 읽어올 때 에러가 나는 경우이다. 연결을 끊는 것이 낫다.
                            stopConnection();
                            return;
                        }
                        _isHeaderReceived = true;
                    }

                    dataSize = dataSizeFromHeader();
                    if (_conn.Available < dataSize)
                    {
                        return;
                    }
                    Buffer.BlockCopy(_recvHeader, 0, _recvBuff, 0, _headerSize);
                    size = _conn.Interface.U_Read(_recvBuff, _headerSize, dataSize);
                    if (size < 0) return;

                    _totalRecvSize = _headerSize + size; //헤더와 데이터를 다 받기 전에는 _totalRecvSize를 셋팅하지 않는다.
                    _isHeaderReceived = false;


                }
                else
                {
                    int available = _conn.Available;
                    int dts = _dataTypeSize;

                    if (_msgFormat == MsgFormat.String)
                    {
                        dataSize = available;
                    }
                    else
                    {
                        int num = available / dts;
                        dataSize = num * dts;
                    }
                    //dataSize = (_msgFormat == MsgFormat.String) ? _conn.Available : (_conn.Available / _dataTypeSize) * _dataTypeSize;
                    //스트링이 아니라면 타입크기로 한번 나누었다 곱해줌으로서 타입보다 작은 데이터를 가져오는 것을 방지한다.
                    //예> int형은 4byte이지만, 6byte가 버퍼에 있다면, 6/4*4 = 4가 되어 2가 잘린다.
                    size = _conn.Interface.U_Read(_recvBuff, 0, dataSize);
                    _totalRecvSize = size;
                }

                if (_totalRecvSize > 0)
                {
                    if (_totalRecvSize > 0)
                    {
                        _totalRecvBytes += _totalRecvSize;

                        L_Rx.Text = _recvCount + "(" + _totalRecvBytes + ")";
                    }

                    runAfterRecv();
                    D_History.Refresh();
                }
            }
            
        }


        delegate bool onAfterRecv();
        onAfterRecv _onRunAfterRecv;
        int _tempRecvSize = 0;
        String _sentIp = "";
        bool _isEchoEnabled = true;
        private bool runAfterRecv()
        {
            if (_isEchoEnabled == false) return true;
            bool isSuccess = true;
            if (this.InvokeRequired)
            {
                _onRunAfterRecv = new onAfterRecv(runAfterRecv);
                try
                {
                    this.Invoke(_onRunAfterRecv);
                }
                catch { }
            }
            else
            {
                String ip = _conn.getRemoteIp();
                _sentIp = ip;
                if (_totalRecvSize > 0)
                {
                    String text = getDataString(_nsReceived, _recvBuff, 0, _totalRecvSize);
                    if (Ch_CutData.Checked)
                    {
                        String tooltip = text;
                        if (text.Length > 100) text = text.Substring(0, 100) + "...";
                        EasyGridRow row = D_History.AddARow(new object[] { _totalRecvSize.ToString(), ip, text });
                        row[Titles.data].ToolTipText = tooltip;
                    }
                    else
                    {
                        D_History.AddARow(new object[] { _totalRecvSize.ToString(), ip, text });
                    }
                    //_history.Add(new dataHistory(_totalRecvSize, getDataString(), ip));
                }

                if (_responseMode == ResponseMode.EchoRecved)
                {
                    int size = 0;
                    if (_connMode == ConnModes.Tcp)
                    {
                        size = _conn.Interface.U_Write(_recvBuff, _totalRecvSize);
                        if (size < 0) Console.WriteLine("에러");
                    }
                    else
                    {
                        size = _conn.Interface.U_Write(_recvBuff, _totalRecvSize);
                        if (size < 0) Console.WriteLine("에러");
                    }
                    if (size > 0)
                    {
                        _sendCount++;
                        Pr_SendProgress.Value = _sendCount % 100;
                        _totalSendBytes += size;
                        L_Tx.Text = _sendCount + "(" + _totalSendBytes + ")";
                    }
                    else
                    {
                        L_Tx.Text = "보내는 중에 에러";
                    }
                }
                else if (_responseMode == ResponseMode.EchoNext)
                {
                    if (_savedList != null) isSuccess = _savedList.RunNext();
                }
                else if (_responseMode == ResponseMode.EchoSelected)
                {
                    if (_savedList != null) _savedList.RunSelected();
                }
                else //NoEcho
                {
                }
                _tempRecvSize = _totalRecvSize;
                _totalRecvSize = 0;
            }
            return isSuccess;
        }

        private void TimerRun()
        {
            bool isSuccess = true;
           
                String ip = _conn.getRemoteIp();
                _sentIp = ip;
                if (_totalRecvSize > 0)
                {
                    String text = getDataString(_nsReceived, _recvBuff, 0, _totalRecvSize);
                    if (Ch_CutData.Checked)
                    {
                        String tooltip = text;
                        if (text.Length > 100) text = text.Substring(0, 100) + "...";
                        EasyGridRow row = D_History.AddARow(new object[] { _totalRecvSize.ToString(), ip, text });
                        row[Titles.data].ToolTipText = tooltip;
                    }
                    else
                    {
                        D_History.AddARow(new object[] { _totalRecvSize.ToString(), ip, text });
                    }
                    //_history.Add(new dataHistory(_totalRecvSize, getDataString(), ip));
                }

                if (_responseMode == ResponseMode.EchoRecved)
                {
                    int size = 0;
                    if (_connMode == ConnModes.Tcp)
                    {
                        size = _conn.Interface.U_Write(_recvBuff, _totalRecvSize);
                        if (size < 0) Console.WriteLine("에러");
                    }
                    else
                    {
                        size = _conn.Interface.U_Write(_recvBuff, _totalRecvSize);
                        if (size < 0) Console.WriteLine("에러");
                    }
                    if (size > 0)
                    {
                        _sendCount++;
                        Pr_SendProgress.Value = _sendCount % 100;
                        _totalSendBytes += size;
                        L_Tx.Text = _sendCount + "(" + _totalSendBytes + ")";
                    }
                    else
                    {
                        L_Tx.Text = "보내는 중에 에러";
                    }
                }
                else if (_responseMode == ResponseMode.EchoNext)
                {
                    if (_savedList != null) isSuccess = _savedList.RunNext();
                }
                else if (_responseMode == ResponseMode.EchoSelected)
                {
                    if (_savedList != null) _savedList.RunSelected();
                }
                else //NoEcho
                {
                }
                _tempRecvSize = _totalRecvSize;
                _totalRecvSize = 0;
            
        }


        #endregion

        #region //////////////////////button events/////////////////////////////

        CPacketStruct _nsSend = new CPacketStruct();
        /*
        void B_MakeMsg_Click(object sender, EventArgs e)
        {
            DlgMsgMaker dlgMakeMsg = new DlgMsgMaker(_nsSend, "./msg.txt");
            dlgMakeMsg.StartPosition = FormStartPosition.CenterParent;
            dlgMakeMsg.ShowDialog(this);
            MakeMsg();
            try
            {
                if (_savedList != null)
                {
                    _savedList.setEnv(SendMsgToNet, _endian, Ch_IsStringWithNull.Checked, (StringEncodings)_encoding);
                    _savedList.AddNewItem(_nsSend.nativeText);

                    //_savedList.Show();
                    //_savedList.SetBounds(this.Location.X - _savedList.Width, this.Location.Y, 0, 0, BoundsSpecified.Location);
                }
            }
            catch
            {
            }
        }
        */
        /*
        void MakeMsg()
        {
            String msg = "";
            int openBracket = -1;
            int closeBracket = -1;

            String numStr;
            String[] token;

            for (int i = 0; i < _nsSend.Count; i++)
            {
                int size = _nsSend.Items[i].size;
                if (i != 0) msg += "/";
                msg += _nsSend.Items[i].type;
                openBracket = _nsSend.Items[i].InitString.IndexOf("{");
                closeBracket = _nsSend.Items[i].InitString.IndexOf("}");

                if (openBracket >= 0)
                {
                    numStr = _nsSend.Items[i].InitString.Substring(openBracket + 1, closeBracket - openBracket - 1); //브래킷 내부의 내용 가져옴
                }
                else
                {
                    numStr = _nsSend.Items[i].InitString;
                }
                token = numStr.Split(",".ToCharArray());


                for (int j = 0; j < size; j++)
                {
                    if (token.Length > j) numStr = token[j];
                    msg += ",";
                    msg += numStr;
                }
            }
            T_Msg.Text = msg;
            BuildMsg();
            L_MsgSize.Text = _sendSize.ToString();
        }
        */

        Boolean _isStarted = false;
        private void B_Start_Click(object sender, EventArgs e)
        {
            if (_isStarted == true)
            {
                MessageBox.Show("이미 동작중입니다!");
                return;
            }
            B_Start.Enabled = false;
            B_Stop.Enabled = true;
            //B_Send.Enabled = true;
            G_ConnModePos.Enabled = false;
            try
            {
                if (_conn is TcpClientBase)
                {
                    TcpClientBase conn = _conn as TcpClientBase;
                    conn.setServerInfo(T_IP.IpAddress, T_Port.IntValue,0,0);//, true, null, T_Timeout.IntValue);
                    conn.Connect(true);
                }
                else if (_conn is TcpServerBase)
                {
                    TcpServerBase conn = _conn as TcpServerBase;
                    
                    conn.ServerReady(T_IP.IpAddress, T_Port.IntValue, 0, 0);//, true, null, T_Timeout.IntValue);
                    conn.ReadyForClients();
                    
                }
                else if (_conn is UdpClientBase)
                {
                    UdpClientBase conn = _conn as UdpClientBase;
                    conn.setServerInfo(T_IP.IpAddress, T_Port.IntValue, 0, 0);//, true, null, T_Timeout.IntValue);
                    conn.Connect(true);
                }
                else//UdpServerBase
                {
                    UdpServerBase conn = _conn as UdpServerBase;
                    conn.setServerInfo(T_IP.IpAddress, T_Port.IntValue, 0, 0);//, true, null, T_Timeout.IntValue);
                    conn.ReadyForClient();
                }


                _isStarted = true;
                Led_This.On();

                SaveEnvs();
                #region old
                /*
                StreamWriter file = File.CreateText(_saveEnvPath);
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.IndentChars = "    ";
                setting.NewLineChars = "\r\n";
                XmlWriter xWriter = XmlWriter.Create(file, setting);
                
                xWriter.WriteStartDocument(true);
                xWriter.WriteStartElement("TestServerForm");
                for (int i = 0; i < _env.Count; i++)
                {
                    xWriter.WriteElementString(_env.Keys.ElementAt(i), _env.Values.ElementAt(i));
                }
                / *
                xWriter.WriteElementString("IP", T_IP.IpAddress);
                xWriter.WriteElementString("Port", T_Port.Text);
                xWriter.WriteElementString("Endian", (M_EndianBig.Checked) ? "Big" : "Little");
                xWriter.WriteElementString("ConnPos", (R_ConnPosServer.Checked) ? "Server" : "Client");
                xWriter.WriteElementString("ConnMode", (R_ConnModeTcp.Checked) ? "Tcp" : "Udp");
                * /
                xWriter.WriteEndElement();
                xWriter.WriteEndDocument();

                xWriter.Close();
                file.Close();
                */
                #endregion
                EnableMenues(_connTypes, false);
                EnableMenues(_conPosList, false);
                //G_ConnMode.Enabled = false;
                //G_connPos.Enabled = false;
                G_HeaderDefine.Enabled = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show("어떤 이유로 인해 서버를 접속하는 것을 실패했습니다." + ex.Message + ex.StackTrace);
                _isStarted = false;
            }

        }

        public void SaveEnvs()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement xRoot = xDoc.CreateElement("Peer");
            xDoc.AppendChild(xRoot);
            XmlElement xEnvs = xDoc.CreateElement("Envs");
            xRoot.AppendChild(xEnvs);

            for (int i = 0; i < _env.Count; i++)
            {
                String name = _env.Keys.ElementAt(i);
                String value = _env.Values.ElementAt(i);
                XmlElement xEnv = xDoc.CreateElement("Env");
                XmlAttribute attr = xDoc.CreateAttribute("Name");
                attr.Value = name;
                xEnv.Attributes.Append(attr);
                xEnv.InnerText = value;
                xEnvs.AppendChild(xEnv);
            }
            xDoc.Save(_saveEnvPath);
        }

        void EnableMenues(ToolStripMenuItem[] arr, Boolean enable)
        {
            foreach (ToolStripMenuItem c in arr)
            {
                c.Enabled = enable;
            }
        }

        private void B_Stop_Click(object sender, EventArgs e)
        {
            stopConnection();
            B_Start.Enabled = true;
            B_Stop.Enabled = false;
            //B_Send.Enabled = false;
            G_ConnModePos.Enabled = true;
        }

        delegate void OnStopConnection();
        OnStopConnection _onStopConnection;
        public void stopConnection()
        {
            if (this.InvokeRequired)
            {
                _onStopConnection = stopConnection;
                try
                {
                    this.Invoke(_onStopConnection);
                }
                catch { }
            }
            else
            {
                // _tcp.endThisClient();
                if (_conn != null) _conn.Close();

                _isStarted = false;
                Led_This.Off();
                Led_Connect.Off();

                EnableMenues(_connTypes, true);
                EnableMenues(_conPosList, true);

                //G_ConnMode.Enabled = true;
                //G_connPos.Enabled = true;
                if (_connMode == ConnModes.Tcp) G_HeaderDefine.Enabled = true;
            }
        }

        int _headerSize = 0;

        int _headerDataItemSize = 0;
        int _dataItemOffset = 0;
        Array _headerSizeItem;

        private void B_ApplyHeader_Click(object sender, EventArgs e)
        {
            _headerSize = T_HeaderSize.IntValue;
            L_TotalHeaderSize.Text = T_HeaderSize.Text;
            _dataItemOffset = T_SizeItemOffset.IntValue;
            L_SizeItemOffset.Text = T_SizeItemOffset.Text;
            int typeItem = Combo_SizeItemType.SelectedIndex;
            switch (typeItem)
            {
                case 1://short
                    _headerDataItemSize = 2;
                    _headerSizeItem = new short[1];
                    break;
                case 2://byte
                    _headerDataItemSize = 1;
                    _headerSizeItem = new byte[1];
                    break;
                case 0://int
                default:
                    _headerDataItemSize = 4;
                    _headerSizeItem = new int[1];
                    break;
            }
            _recvHeader = new Byte[_headerSize];
            L_SizItemType.Text = Combo_SizeItemType.Text;
        }


        /*
        private void B_Send_Click(object sender, EventArgs e)
        {
            if (_responseMode == ResponseMode.EchoWithMsg)
            {
                BuildMsg();
                
            }
            else
            {
                //SendMsg();
            }

        }
        */

        private void B_StopRecv_Click(object sender, EventArgs e)
        {
            if (_isHeaderReceived)
            {
                int available = _conn.Available;
                int data = _conn.Interface.U_Read(_recvBuff, _headerSize, available);
                if (data == 0) return;
                _totalRecvSize = _headerSize + data;
                _isHeaderReceived = false;
            }
            else
            {
                int available = _conn.Available;
                int data = _conn.Interface.U_Read(_recvBuff, 0, available);
                _totalRecvSize = data;
                _isHeaderReceived = false;
            }

            //_recvTimeoutTimer.Tick -= _timeout;
            runAfterRecv();
            //D_History.Refresh();
        }

        Timer _responseTimer = new Timer();
        Boolean _responseTimerOn = false;
        private void B_ResponseTimer_Click(object sender, EventArgs e)
        {
            /*
            if (_sendSize <= 0)
            {
                MessageBox.Show("Msg 창에 Response할 메시지를 입력하십시오. 메시지포멧은 형식,값1,값2,.../형식,값a,값b.../형식...입니다.");
                return;
            }
             */
            if (_responseTimerOn == false)
            {
                _responseTimer.Interval = T_ResponseTimer.IntValue;
                _responseTimer.Enabled = false;
                _responseTimer.Start();
                Led_ResponseTimer.On();
                _responseTimerOn = true;
                _isEchoEnabled = false;//timer동작중에는 echo를 하지않아야 한다..
            }
            else
            {
                _responseTimer.Stop();
                Led_ResponseTimer.Off();
                _responseTimerOn = false;
                _isEchoEnabled = true;
            }

        }

        #endregion

        #region ///////////////// other events (timer, textbox, form) //////////////////////
        void _responseTimer_Tick(object sender, EventArgs e)
        {
            //SendMsg();
            TimerRun();
        }
        /*
        void T_Msg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                T_Msg.Text = T_Msg.Text.Replace("\r", "");
                T_Msg.Text = T_Msg.Text.Replace("\n", "");
                B_Send_Click(B_Send, new EventArgs());
            }
        }
        */

        void TestServerForm_Resize(object sender, EventArgs e)
        {
            int size = 0;
            int maxSize = 0;
            MenuBar.Width = this.Width;
            foreach (Control c in P_Menues.Controls)
            {
                if (SP_Spliter.Orientation == Orientation.Horizontal)
                {
                    size = c.Location.Y + c.Height;
                }
                else
                {
                    size = c.Width;
                    c.Location = new Point(0, c.Location.Y + 10);
                }
                if (size > maxSize)
                {
                    maxSize = size;
                    //PosLabel.Text = c.Name+ maxSize.ToString();
                }
            }
            try
            {
                SP_Spliter.SplitterDistance = maxSize+MenuBar.Height;
                SP_Spliter.Refresh();
            }
            catch
            {
                maxSize = size;
            }
        }
        
        public void Close()
        {
            if (_conn != null)
            {
                _conn.Close();
            }
        }
        void _dlgViewStruct_U_PopupClosed(object sender, PopupClosedEventArgs args)
        {
            _dlgViewStruct = null;
        }

        #endregion

    }
}
