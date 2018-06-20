namespace TestNetConnector
{
    partial class ConControllers
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.P_Controllers = new System.Windows.Forms.FlowLayoutPanel();
            this.G_IpPortSettings = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.T_IP = new FormAdders.IPAddressInputBox();
            this.label2 = new System.Windows.Forms.Label();
            this.T_Port = new FormAdders.NumberTextBox();
            this.B_Start = new System.Windows.Forms.Button();
            this.G_ConnModePos = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.R_ConnPosClient = new System.Windows.Forms.RadioButton();
            this.R_ConnPosServer = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.R_ConnModeUdp = new System.Windows.Forms.RadioButton();
            this.R_ConnModeTcp = new System.Windows.Forms.RadioButton();
            this.G_NetInfo = new System.Windows.Forms.GroupBox();
            this.Ch_CutData = new System.Windows.Forms.CheckBox();
            this.Ch_IsStringWithNull = new System.Windows.Forms.CheckBox();
            this.L_EchoMode = new System.Windows.Forms.Label();
            this.L_ConnSite = new System.Windows.Forms.Label();
            this.L_Format = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.L_MyIp = new System.Windows.Forms.Label();
            this.L_MsgSize = new System.Windows.Forms.Label();
            this.L_Endian = new System.Windows.Forms.Label();
            this.L_ConnType = new System.Windows.Forms.Label();
            this.G_EchoMode = new System.Windows.Forms.GroupBox();
            this.radioGroup1 = new FormAdders.RadioGroup();
            this.G_TimerSettings = new System.Windows.Forms.Panel();
            this.G_Timeout = new System.Windows.Forms.GroupBox();
            this.T_Timeout = new FormAdders.NumberTextBox();
            this.B_StopRecv = new System.Windows.Forms.Button();
            this.G_ResponseTimer = new System.Windows.Forms.GroupBox();
            this.T_ResponseTimer = new FormAdders.NumberTextBox();
            this.B_ResponseTimer = new System.Windows.Forms.Button();
            this.Led_ResponseTimer = new FormAdders.OnOffLed();
            this.label9 = new System.Windows.Forms.Label();
            this.G_Analysis = new System.Windows.Forms.GroupBox();
            this.R_MsgFormatELog = new System.Windows.Forms.RadioButton();
            this.R_MsgFormatFloat = new System.Windows.Forms.RadioButton();
            this.R_MsgFormatStr = new System.Windows.Forms.RadioButton();
            this.R_MsgFormatBin = new System.Windows.Forms.RadioButton();
            this.R_MsgFormatDec = new System.Windows.Forms.RadioButton();
            this.R_MsgFormatHex = new System.Windows.Forms.RadioButton();
            this.G_Progress = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.L_Tx = new System.Windows.Forms.Label();
            this.L_Rx = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Pr_SendProgress = new System.Windows.Forms.ProgressBar();
            this.Pr_RecvProgress = new System.Windows.Forms.ProgressBar();
            this.G_ConnLeds = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Led_This = new FormAdders.OnOffLed();
            this.Led_Connect = new FormAdders.OnOffLed();
            this.G_ListControlButtons = new System.Windows.Forms.Panel();
            this.B_RunNext = new System.Windows.Forms.Button();
            this.B_ShowList = new System.Windows.Forms.Button();
            this.G_MsgBox = new System.Windows.Forms.Panel();
            this.T_Msg = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.B_MakeMsg = new System.Windows.Forms.Button();
            this.B_Send = new System.Windows.Forms.Button();
            this.G_HeaderDefine = new System.Windows.Forms.GroupBox();
            this.L_SizeItemOffset = new System.Windows.Forms.Label();
            this.L_SizItemType = new System.Windows.Forms.Label();
            this.L_TotalHeaderSize = new System.Windows.Forms.Label();
            this.Combo_SizeItemType = new System.Windows.Forms.ComboBox();
            this.T_SizeItemOffset = new FormAdders.NumberTextBox();
            this.T_HeaderSize = new FormAdders.NumberTextBox();
            this.B_ApplyHeader = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.P_Controllers.SuspendLayout();
            this.G_IpPortSettings.SuspendLayout();
            this.G_ConnModePos.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.G_NetInfo.SuspendLayout();
            this.G_EchoMode.SuspendLayout();
            this.G_TimerSettings.SuspendLayout();
            this.G_Timeout.SuspendLayout();
            this.G_ResponseTimer.SuspendLayout();
            this.G_Analysis.SuspendLayout();
            this.G_Progress.SuspendLayout();
            this.G_ConnLeds.SuspendLayout();
            this.G_ListControlButtons.SuspendLayout();
            this.G_MsgBox.SuspendLayout();
            this.G_HeaderDefine.SuspendLayout();
            this.SuspendLayout();
            // 
            // P_Controllers
            // 
            this.P_Controllers.Controls.Add(this.G_IpPortSettings);
            this.P_Controllers.Controls.Add(this.G_ConnModePos);
            this.P_Controllers.Controls.Add(this.G_NetInfo);
            this.P_Controllers.Controls.Add(this.G_EchoMode);
            this.P_Controllers.Controls.Add(this.G_TimerSettings);
            this.P_Controllers.Controls.Add(this.G_Analysis);
            this.P_Controllers.Controls.Add(this.G_Progress);
            this.P_Controllers.Controls.Add(this.G_ConnLeds);
            this.P_Controllers.Controls.Add(this.G_ListControlButtons);
            this.P_Controllers.Controls.Add(this.G_MsgBox);
            this.P_Controllers.Controls.Add(this.G_HeaderDefine);
            this.P_Controllers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Controllers.Location = new System.Drawing.Point(0, 0);
            this.P_Controllers.Name = "P_Controllers";
            this.P_Controllers.Size = new System.Drawing.Size(248, 646);
            this.P_Controllers.TabIndex = 1;
            // 
            // G_IpPortSettings
            // 
            this.G_IpPortSettings.Controls.Add(this.label1);
            this.G_IpPortSettings.Controls.Add(this.T_IP);
            this.G_IpPortSettings.Controls.Add(this.label2);
            this.G_IpPortSettings.Controls.Add(this.T_Port);
            this.G_IpPortSettings.Controls.Add(this.B_Start);
            this.G_IpPortSettings.Location = new System.Drawing.Point(3, 3);
            this.G_IpPortSettings.Name = "G_IpPortSettings";
            this.G_IpPortSettings.Size = new System.Drawing.Size(238, 54);
            this.G_IpPortSettings.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Server IP";
            // 
            // T_IP
            // 
            this.T_IP.AccessibleDescription = "IPAddressInputBox";
            this.T_IP.AccessibleName = "IPAddressInputBox";
            this.T_IP.AroundBackColor = System.Drawing.SystemColors.Control;
            this.T_IP.AutoSize = true;
            this.T_IP.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.T_IP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.T_IP.IpAddress = "0.0.0.0";
            this.T_IP.Location = new System.Drawing.Point(69, 6);
            this.T_IP.Margin = new System.Windows.Forms.Padding(0);
            this.T_IP.MinimumSize = new System.Drawing.Size(120, 20);
            this.T_IP.Name = "T_IP";
            this.T_IP.Size = new System.Drawing.Size(120, 20);
            this.T_IP.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server Port";
            // 
            // T_Port
            // 
            this.T_Port.HideSelection = false;
            this.T_Port.Location = new System.Drawing.Point(76, 26);
            this.T_Port.Name = "T_Port";
            this.T_Port.Size = new System.Drawing.Size(97, 21);
            this.T_Port.TabIndex = 8;
            this.T_Port.Text = "3300";
            this.T_Port.U_Text = "3300";
            // 
            // B_Start
            // 
            this.B_Start.Location = new System.Drawing.Point(188, 3);
            this.B_Start.Name = "B_Start";
            this.B_Start.Size = new System.Drawing.Size(47, 44);
            this.B_Start.TabIndex = 10;
            this.B_Start.Text = "Start";
            this.B_Start.UseVisualStyleBackColor = true;
            // 
            // G_ConnModePos
            // 
            this.G_ConnModePos.Controls.Add(this.groupBox7);
            this.G_ConnModePos.Controls.Add(this.groupBox6);
            this.G_ConnModePos.Location = new System.Drawing.Point(3, 63);
            this.G_ConnModePos.Name = "G_ConnModePos";
            this.G_ConnModePos.Size = new System.Drawing.Size(241, 46);
            this.G_ConnModePos.TabIndex = 50;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.R_ConnPosClient);
            this.groupBox7.Controls.Add(this.R_ConnPosServer);
            this.groupBox7.Location = new System.Drawing.Point(123, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(114, 38);
            this.groupBox7.TabIndex = 16;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "ConnPos";
            // 
            // R_ConnPosClient
            // 
            this.R_ConnPosClient.AutoSize = true;
            this.R_ConnPosClient.Location = new System.Drawing.Point(58, 19);
            this.R_ConnPosClient.Name = "R_ConnPosClient";
            this.R_ConnPosClient.Size = new System.Drawing.Size(55, 16);
            this.R_ConnPosClient.TabIndex = 0;
            this.R_ConnPosClient.Tag = "1";
            this.R_ConnPosClient.Text = "Client";
            this.R_ConnPosClient.UseVisualStyleBackColor = true;
            // 
            // R_ConnPosServer
            // 
            this.R_ConnPosServer.AutoSize = true;
            this.R_ConnPosServer.Checked = true;
            this.R_ConnPosServer.Location = new System.Drawing.Point(10, 19);
            this.R_ConnPosServer.Name = "R_ConnPosServer";
            this.R_ConnPosServer.Size = new System.Drawing.Size(48, 16);
            this.R_ConnPosServer.TabIndex = 0;
            this.R_ConnPosServer.TabStop = true;
            this.R_ConnPosServer.Tag = "0";
            this.R_ConnPosServer.Text = "Serv";
            this.R_ConnPosServer.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.R_ConnModeUdp);
            this.groupBox6.Controls.Add(this.R_ConnModeTcp);
            this.groupBox6.Location = new System.Drawing.Point(3, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(114, 38);
            this.groupBox6.TabIndex = 16;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "ConnMode";
            // 
            // R_ConnModeUdp
            // 
            this.R_ConnModeUdp.AutoSize = true;
            this.R_ConnModeUdp.Location = new System.Drawing.Point(58, 16);
            this.R_ConnModeUdp.Name = "R_ConnModeUdp";
            this.R_ConnModeUdp.Size = new System.Drawing.Size(45, 16);
            this.R_ConnModeUdp.TabIndex = 0;
            this.R_ConnModeUdp.Tag = "1";
            this.R_ConnModeUdp.Text = "Udp";
            this.R_ConnModeUdp.UseVisualStyleBackColor = true;
            // 
            // R_ConnModeTcp
            // 
            this.R_ConnModeTcp.AutoSize = true;
            this.R_ConnModeTcp.Checked = true;
            this.R_ConnModeTcp.Location = new System.Drawing.Point(10, 16);
            this.R_ConnModeTcp.Name = "R_ConnModeTcp";
            this.R_ConnModeTcp.Size = new System.Drawing.Size(45, 16);
            this.R_ConnModeTcp.TabIndex = 0;
            this.R_ConnModeTcp.TabStop = true;
            this.R_ConnModeTcp.Tag = "0";
            this.R_ConnModeTcp.Text = "Tcp";
            this.R_ConnModeTcp.UseVisualStyleBackColor = true;
            // 
            // G_NetInfo
            // 
            this.G_NetInfo.Controls.Add(this.Ch_CutData);
            this.G_NetInfo.Controls.Add(this.Ch_IsStringWithNull);
            this.G_NetInfo.Controls.Add(this.L_EchoMode);
            this.G_NetInfo.Controls.Add(this.L_ConnSite);
            this.G_NetInfo.Controls.Add(this.L_Format);
            this.G_NetInfo.Controls.Add(this.label13);
            this.G_NetInfo.Controls.Add(this.label12);
            this.G_NetInfo.Controls.Add(this.L_MyIp);
            this.G_NetInfo.Controls.Add(this.L_MsgSize);
            this.G_NetInfo.Controls.Add(this.L_Endian);
            this.G_NetInfo.Controls.Add(this.L_ConnType);
            this.G_NetInfo.Location = new System.Drawing.Point(3, 115);
            this.G_NetInfo.Name = "G_NetInfo";
            this.G_NetInfo.Size = new System.Drawing.Size(238, 100);
            this.G_NetInfo.TabIndex = 47;
            this.G_NetInfo.TabStop = false;
            this.G_NetInfo.Text = "Info";
            // 
            // Ch_CutData
            // 
            this.Ch_CutData.AutoSize = true;
            this.Ch_CutData.Checked = true;
            this.Ch_CutData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ch_CutData.Location = new System.Drawing.Point(113, 81);
            this.Ch_CutData.Name = "Ch_CutData";
            this.Ch_CutData.Size = new System.Drawing.Size(72, 16);
            this.Ch_CutData.TabIndex = 1;
            this.Ch_CutData.Text = "Cut Data";
            this.Ch_CutData.UseVisualStyleBackColor = true;
            // 
            // Ch_IsStringWithNull
            // 
            this.Ch_IsStringWithNull.AutoSize = true;
            this.Ch_IsStringWithNull.Location = new System.Drawing.Point(0, 78);
            this.Ch_IsStringWithNull.Name = "Ch_IsStringWithNull";
            this.Ch_IsStringWithNull.Size = new System.Drawing.Size(109, 16);
            this.Ch_IsStringWithNull.TabIndex = 1;
            this.Ch_IsStringWithNull.Text = "String with Null";
            this.Ch_IsStringWithNull.UseVisualStyleBackColor = true;
            // 
            // L_EchoMode
            // 
            this.L_EchoMode.AutoSize = true;
            this.L_EchoMode.Location = new System.Drawing.Point(87, 13);
            this.L_EchoMode.Name = "L_EchoMode";
            this.L_EchoMode.Size = new System.Drawing.Size(50, 12);
            this.L_EchoMode.TabIndex = 0;
            this.L_EchoMode.Text = "NoEcho";
            // 
            // L_ConnSite
            // 
            this.L_ConnSite.AutoSize = true;
            this.L_ConnSite.Location = new System.Drawing.Point(40, 13);
            this.L_ConnSite.Name = "L_ConnSite";
            this.L_ConnSite.Size = new System.Drawing.Size(41, 12);
            this.L_ConnSite.TabIndex = 0;
            this.L_ConnSite.Text = "Server";
            // 
            // L_Format
            // 
            this.L_Format.AutoSize = true;
            this.L_Format.Location = new System.Drawing.Point(87, 30);
            this.L_Format.Name = "L_Format";
            this.L_Format.Size = new System.Drawing.Size(75, 12);
            this.L_Format.TabIndex = 0;
            this.L_Format.Text = "Hex(4Bytes)";
            this.L_Format.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "MyIp";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(174, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "MsgSize";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_MyIp
            // 
            this.L_MyIp.AutoSize = true;
            this.L_MyIp.Location = new System.Drawing.Point(46, 50);
            this.L_MyIp.Name = "L_MyIp";
            this.L_MyIp.Size = new System.Drawing.Size(40, 12);
            this.L_MyIp.TabIndex = 0;
            this.L_MyIp.Text = "Status";
            this.L_MyIp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_MsgSize
            // 
            this.L_MsgSize.AutoSize = true;
            this.L_MsgSize.Location = new System.Drawing.Point(186, 30);
            this.L_MsgSize.Name = "L_MsgSize";
            this.L_MsgSize.Size = new System.Drawing.Size(40, 12);
            this.L_MsgSize.TabIndex = 0;
            this.L_MsgSize.Text = "Status";
            this.L_MsgSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Endian
            // 
            this.L_Endian.AutoSize = true;
            this.L_Endian.Location = new System.Drawing.Point(9, 30);
            this.L_Endian.Name = "L_Endian";
            this.L_Endian.Size = new System.Drawing.Size(62, 12);
            this.L_Endian.TabIndex = 0;
            this.L_Endian.Text = "BigEndian";
            this.L_Endian.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_ConnType
            // 
            this.L_ConnType.AutoSize = true;
            this.L_ConnType.Location = new System.Drawing.Point(7, 13);
            this.L_ConnType.Name = "L_ConnType";
            this.L_ConnType.Size = new System.Drawing.Size(27, 12);
            this.L_ConnType.TabIndex = 0;
            this.L_ConnType.Text = "Tcp";
            // 
            // G_EchoMode
            // 
            this.G_EchoMode.Controls.Add(this.radioGroup1);
            this.G_EchoMode.Location = new System.Drawing.Point(3, 221);
            this.G_EchoMode.Name = "G_EchoMode";
            this.G_EchoMode.Size = new System.Drawing.Size(235, 98);
            this.G_EchoMode.TabIndex = 53;
            this.G_EchoMode.TabStop = false;
            this.G_EchoMode.Text = "응답모드";
            // 
            // radioGroup1
            // 
            this.radioGroup1.AutoSize = true;
            this.radioGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioGroup1.Items = new string[] {
        "No Echo",
        "Echo MyMsg",
        "Echo Received",
        "Echo Selected",
        "Echo Next"};
            this.radioGroup1.Location = new System.Drawing.Point(3, 17);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.SelectedIndex = 0;
            this.radioGroup1.Size = new System.Drawing.Size(229, 78);
            this.radioGroup1.TabIndex = 0;
            this.radioGroup1.UnitSize = new System.Drawing.Size(115, 20);
            // 
            // G_TimerSettings
            // 
            this.G_TimerSettings.Controls.Add(this.G_Timeout);
            this.G_TimerSettings.Controls.Add(this.G_ResponseTimer);
            this.G_TimerSettings.Location = new System.Drawing.Point(3, 325);
            this.G_TimerSettings.Name = "G_TimerSettings";
            this.G_TimerSettings.Size = new System.Drawing.Size(238, 53);
            this.G_TimerSettings.TabIndex = 48;
            // 
            // G_Timeout
            // 
            this.G_Timeout.Controls.Add(this.T_Timeout);
            this.G_Timeout.Controls.Add(this.B_StopRecv);
            this.G_Timeout.Location = new System.Drawing.Point(3, 52);
            this.G_Timeout.Name = "G_Timeout";
            this.G_Timeout.Size = new System.Drawing.Size(165, 45);
            this.G_Timeout.TabIndex = 29;
            this.G_Timeout.TabStop = false;
            this.G_Timeout.Text = "Timeout";
            // 
            // T_Timeout
            // 
            this.T_Timeout.HideSelection = false;
            this.T_Timeout.Location = new System.Drawing.Point(4, 16);
            this.T_Timeout.Name = "T_Timeout";
            this.T_Timeout.Size = new System.Drawing.Size(45, 21);
            this.T_Timeout.TabIndex = 22;
            this.T_Timeout.Text = "200";
            this.T_Timeout.U_Text = "200";
            // 
            // B_StopRecv
            // 
            this.B_StopRecv.Location = new System.Drawing.Point(59, 8);
            this.B_StopRecv.Name = "B_StopRecv";
            this.B_StopRecv.Size = new System.Drawing.Size(99, 34);
            this.B_StopRecv.TabIndex = 19;
            this.B_StopRecv.Text = "StopRecv\r\n(show directly)";
            this.B_StopRecv.UseVisualStyleBackColor = true;
            // 
            // G_ResponseTimer
            // 
            this.G_ResponseTimer.Controls.Add(this.T_ResponseTimer);
            this.G_ResponseTimer.Controls.Add(this.B_ResponseTimer);
            this.G_ResponseTimer.Controls.Add(this.Led_ResponseTimer);
            this.G_ResponseTimer.Controls.Add(this.label9);
            this.G_ResponseTimer.Location = new System.Drawing.Point(4, 3);
            this.G_ResponseTimer.Name = "G_ResponseTimer";
            this.G_ResponseTimer.Size = new System.Drawing.Size(164, 47);
            this.G_ResponseTimer.TabIndex = 20;
            this.G_ResponseTimer.TabStop = false;
            this.G_ResponseTimer.Text = "ResponseTimer";
            // 
            // T_ResponseTimer
            // 
            this.T_ResponseTimer.HideSelection = false;
            this.T_ResponseTimer.Location = new System.Drawing.Point(28, 18);
            this.T_ResponseTimer.Name = "T_ResponseTimer";
            this.T_ResponseTimer.Size = new System.Drawing.Size(51, 21);
            this.T_ResponseTimer.TabIndex = 22;
            this.T_ResponseTimer.Text = "200";
            this.T_ResponseTimer.U_Text = "200";
            // 
            // B_ResponseTimer
            // 
            this.B_ResponseTimer.Location = new System.Drawing.Point(104, 15);
            this.B_ResponseTimer.Name = "B_ResponseTimer";
            this.B_ResponseTimer.Size = new System.Drawing.Size(57, 30);
            this.B_ResponseTimer.TabIndex = 24;
            this.B_ResponseTimer.Text = "On/Off";
            this.B_ResponseTimer.UseVisualStyleBackColor = true;
            // 
            // Led_ResponseTimer
            // 
            this.Led_ResponseTimer.BackColor = System.Drawing.Color.Transparent;
            this.Led_ResponseTimer.Location = new System.Drawing.Point(8, 20);
            this.Led_ResponseTimer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Led_ResponseTimer.Name = "Led_ResponseTimer";
            this.Led_ResponseTimer.Size = new System.Drawing.Size(19, 20);
            this.Led_ResponseTimer.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(77, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "ms";
            // 
            // G_Analysis
            // 
            this.G_Analysis.Controls.Add(this.R_MsgFormatELog);
            this.G_Analysis.Controls.Add(this.R_MsgFormatFloat);
            this.G_Analysis.Controls.Add(this.R_MsgFormatStr);
            this.G_Analysis.Controls.Add(this.R_MsgFormatBin);
            this.G_Analysis.Controls.Add(this.R_MsgFormatDec);
            this.G_Analysis.Controls.Add(this.R_MsgFormatHex);
            this.G_Analysis.Location = new System.Drawing.Point(3, 384);
            this.G_Analysis.Name = "G_Analysis";
            this.G_Analysis.Size = new System.Drawing.Size(236, 64);
            this.G_Analysis.TabIndex = 46;
            this.G_Analysis.TabStop = false;
            this.G_Analysis.Text = "분석도구";
            this.G_Analysis.Visible = false;
            // 
            // R_MsgFormatELog
            // 
            this.R_MsgFormatELog.AutoSize = true;
            this.R_MsgFormatELog.Location = new System.Drawing.Point(57, 43);
            this.R_MsgFormatELog.Name = "R_MsgFormatELog";
            this.R_MsgFormatELog.Size = new System.Drawing.Size(52, 16);
            this.R_MsgFormatELog.TabIndex = 0;
            this.R_MsgFormatELog.TabStop = true;
            this.R_MsgFormatELog.Tag = "5";
            this.R_MsgFormatELog.Text = "ELog";
            this.R_MsgFormatELog.UseVisualStyleBackColor = true;
            // 
            // R_MsgFormatFloat
            // 
            this.R_MsgFormatFloat.AutoSize = true;
            this.R_MsgFormatFloat.Location = new System.Drawing.Point(6, 43);
            this.R_MsgFormatFloat.Name = "R_MsgFormatFloat";
            this.R_MsgFormatFloat.Size = new System.Drawing.Size(50, 16);
            this.R_MsgFormatFloat.TabIndex = 0;
            this.R_MsgFormatFloat.TabStop = true;
            this.R_MsgFormatFloat.Tag = "4";
            this.R_MsgFormatFloat.Text = "Float";
            this.R_MsgFormatFloat.UseVisualStyleBackColor = true;
            // 
            // R_MsgFormatStr
            // 
            this.R_MsgFormatStr.AutoSize = true;
            this.R_MsgFormatStr.Location = new System.Drawing.Point(164, 21);
            this.R_MsgFormatStr.Name = "R_MsgFormatStr";
            this.R_MsgFormatStr.Size = new System.Drawing.Size(38, 16);
            this.R_MsgFormatStr.TabIndex = 0;
            this.R_MsgFormatStr.TabStop = true;
            this.R_MsgFormatStr.Tag = "3";
            this.R_MsgFormatStr.Text = "Str";
            this.R_MsgFormatStr.UseVisualStyleBackColor = true;
            // 
            // R_MsgFormatBin
            // 
            this.R_MsgFormatBin.AutoSize = true;
            this.R_MsgFormatBin.Location = new System.Drawing.Point(113, 21);
            this.R_MsgFormatBin.Name = "R_MsgFormatBin";
            this.R_MsgFormatBin.Size = new System.Drawing.Size(41, 16);
            this.R_MsgFormatBin.TabIndex = 0;
            this.R_MsgFormatBin.TabStop = true;
            this.R_MsgFormatBin.Tag = "2";
            this.R_MsgFormatBin.Text = "Bin";
            this.R_MsgFormatBin.UseVisualStyleBackColor = true;
            // 
            // R_MsgFormatDec
            // 
            this.R_MsgFormatDec.AutoSize = true;
            this.R_MsgFormatDec.Location = new System.Drawing.Point(57, 21);
            this.R_MsgFormatDec.Name = "R_MsgFormatDec";
            this.R_MsgFormatDec.Size = new System.Drawing.Size(45, 16);
            this.R_MsgFormatDec.TabIndex = 0;
            this.R_MsgFormatDec.TabStop = true;
            this.R_MsgFormatDec.Tag = "1";
            this.R_MsgFormatDec.Text = "Dec";
            this.R_MsgFormatDec.UseVisualStyleBackColor = true;
            // 
            // R_MsgFormatHex
            // 
            this.R_MsgFormatHex.AutoSize = true;
            this.R_MsgFormatHex.Location = new System.Drawing.Point(6, 21);
            this.R_MsgFormatHex.Name = "R_MsgFormatHex";
            this.R_MsgFormatHex.Size = new System.Drawing.Size(45, 16);
            this.R_MsgFormatHex.TabIndex = 0;
            this.R_MsgFormatHex.TabStop = true;
            this.R_MsgFormatHex.Tag = "0";
            this.R_MsgFormatHex.Text = "Hex";
            this.R_MsgFormatHex.UseVisualStyleBackColor = true;
            // 
            // G_Progress
            // 
            this.G_Progress.Controls.Add(this.label11);
            this.G_Progress.Controls.Add(this.L_Tx);
            this.G_Progress.Controls.Add(this.L_Rx);
            this.G_Progress.Controls.Add(this.label10);
            this.G_Progress.Controls.Add(this.Pr_SendProgress);
            this.G_Progress.Controls.Add(this.Pr_RecvProgress);
            this.G_Progress.Location = new System.Drawing.Point(3, 454);
            this.G_Progress.Name = "G_Progress";
            this.G_Progress.Size = new System.Drawing.Size(237, 45);
            this.G_Progress.TabIndex = 49;
            this.G_Progress.TabStop = false;
            this.G_Progress.Text = "진행상황";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(126, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 12);
            this.label11.TabIndex = 19;
            this.label11.Text = "Tx";
            // 
            // L_Tx
            // 
            this.L_Tx.Location = new System.Drawing.Point(149, 33);
            this.L_Tx.Name = "L_Tx";
            this.L_Tx.Size = new System.Drawing.Size(80, 14);
            this.L_Tx.TabIndex = 19;
            this.L_Tx.Text = "Tx";
            this.L_Tx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Rx
            // 
            this.L_Rx.Location = new System.Drawing.Point(30, 32);
            this.L_Rx.Name = "L_Rx";
            this.L_Rx.Size = new System.Drawing.Size(80, 14);
            this.L_Rx.TabIndex = 19;
            this.L_Rx.Text = "Rx";
            this.L_Rx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 12);
            this.label10.TabIndex = 19;
            this.label10.Text = "Rx";
            // 
            // Pr_SendProgress
            // 
            this.Pr_SendProgress.Location = new System.Drawing.Point(149, 15);
            this.Pr_SendProgress.Name = "Pr_SendProgress";
            this.Pr_SendProgress.Size = new System.Drawing.Size(80, 15);
            this.Pr_SendProgress.TabIndex = 18;
            // 
            // Pr_RecvProgress
            // 
            this.Pr_RecvProgress.Location = new System.Drawing.Point(30, 15);
            this.Pr_RecvProgress.Name = "Pr_RecvProgress";
            this.Pr_RecvProgress.Size = new System.Drawing.Size(80, 15);
            this.Pr_RecvProgress.TabIndex = 18;
            // 
            // G_ConnLeds
            // 
            this.G_ConnLeds.Controls.Add(this.label8);
            this.G_ConnLeds.Controls.Add(this.label7);
            this.G_ConnLeds.Controls.Add(this.Led_This);
            this.G_ConnLeds.Controls.Add(this.Led_Connect);
            this.G_ConnLeds.Location = new System.Drawing.Point(3, 505);
            this.G_ConnLeds.Name = "G_ConnLeds";
            this.G_ConnLeds.Size = new System.Drawing.Size(238, 34);
            this.G_ConnLeds.TabIndex = 43;
            this.G_ConnLeds.TabStop = false;
            this.G_ConnLeds.Text = "Connected";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(159, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "This";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "Remote";
            // 
            // Led_This
            // 
            this.Led_This.BackColor = System.Drawing.Color.Transparent;
            this.Led_This.Location = new System.Drawing.Point(143, 12);
            this.Led_This.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Led_This.Name = "Led_This";
            this.Led_This.Size = new System.Drawing.Size(19, 20);
            this.Led_This.TabIndex = 13;
            // 
            // Led_Connect
            // 
            this.Led_Connect.BackColor = System.Drawing.Color.Transparent;
            this.Led_Connect.Location = new System.Drawing.Point(7, 14);
            this.Led_Connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Led_Connect.Name = "Led_Connect";
            this.Led_Connect.Size = new System.Drawing.Size(19, 20);
            this.Led_Connect.TabIndex = 13;
            // 
            // G_ListControlButtons
            // 
            this.G_ListControlButtons.Controls.Add(this.B_RunNext);
            this.G_ListControlButtons.Controls.Add(this.B_ShowList);
            this.G_ListControlButtons.Location = new System.Drawing.Point(3, 545);
            this.G_ListControlButtons.Name = "G_ListControlButtons";
            this.G_ListControlButtons.Size = new System.Drawing.Size(238, 32);
            this.G_ListControlButtons.TabIndex = 51;
            // 
            // B_RunNext
            // 
            this.B_RunNext.Location = new System.Drawing.Point(78, 4);
            this.B_RunNext.Name = "B_RunNext";
            this.B_RunNext.Size = new System.Drawing.Size(71, 23);
            this.B_RunNext.TabIndex = 0;
            this.B_RunNext.Text = "RunNext";
            this.B_RunNext.UseVisualStyleBackColor = true;
            // 
            // B_ShowList
            // 
            this.B_ShowList.Location = new System.Drawing.Point(4, 4);
            this.B_ShowList.Name = "B_ShowList";
            this.B_ShowList.Size = new System.Drawing.Size(71, 23);
            this.B_ShowList.TabIndex = 0;
            this.B_ShowList.Text = "ShowList";
            this.B_ShowList.UseVisualStyleBackColor = true;
            // 
            // G_MsgBox
            // 
            this.G_MsgBox.Controls.Add(this.T_Msg);
            this.G_MsgBox.Controls.Add(this.label3);
            this.G_MsgBox.Controls.Add(this.B_MakeMsg);
            this.G_MsgBox.Controls.Add(this.B_Send);
            this.G_MsgBox.Location = new System.Drawing.Point(3, 583);
            this.G_MsgBox.Name = "G_MsgBox";
            this.G_MsgBox.Size = new System.Drawing.Size(238, 57);
            this.G_MsgBox.TabIndex = 52;
            // 
            // T_Msg
            // 
            this.T_Msg.Location = new System.Drawing.Point(31, 4);
            this.T_Msg.Name = "T_Msg";
            this.T_Msg.Size = new System.Drawing.Size(204, 24);
            this.T_Msg.TabIndex = 14;
            this.T_Msg.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Msg\r";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // B_MakeMsg
            // 
            this.B_MakeMsg.Location = new System.Drawing.Point(127, 34);
            this.B_MakeMsg.Name = "B_MakeMsg";
            this.B_MakeMsg.Size = new System.Drawing.Size(105, 20);
            this.B_MakeMsg.TabIndex = 13;
            this.B_MakeMsg.Text = "Make Msg";
            this.B_MakeMsg.UseVisualStyleBackColor = true;
            // 
            // B_Send
            // 
            this.B_Send.Enabled = false;
            this.B_Send.Location = new System.Drawing.Point(5, 34);
            this.B_Send.Name = "B_Send";
            this.B_Send.Size = new System.Drawing.Size(110, 23);
            this.B_Send.TabIndex = 13;
            this.B_Send.Text = "Save Send";
            this.B_Send.UseVisualStyleBackColor = true;
            // 
            // G_HeaderDefine
            // 
            this.G_HeaderDefine.Controls.Add(this.L_SizeItemOffset);
            this.G_HeaderDefine.Controls.Add(this.L_SizItemType);
            this.G_HeaderDefine.Controls.Add(this.L_TotalHeaderSize);
            this.G_HeaderDefine.Controls.Add(this.Combo_SizeItemType);
            this.G_HeaderDefine.Controls.Add(this.T_SizeItemOffset);
            this.G_HeaderDefine.Controls.Add(this.T_HeaderSize);
            this.G_HeaderDefine.Controls.Add(this.B_ApplyHeader);
            this.G_HeaderDefine.Controls.Add(this.label6);
            this.G_HeaderDefine.Controls.Add(this.label5);
            this.G_HeaderDefine.Controls.Add(this.label4);
            this.G_HeaderDefine.Location = new System.Drawing.Point(3, 646);
            this.G_HeaderDefine.Name = "G_HeaderDefine";
            this.G_HeaderDefine.Size = new System.Drawing.Size(185, 100);
            this.G_HeaderDefine.TabIndex = 44;
            this.G_HeaderDefine.TabStop = false;
            this.G_HeaderDefine.Text = "Header Define";
            this.G_HeaderDefine.Visible = false;
            // 
            // L_SizeItemOffset
            // 
            this.L_SizeItemOffset.AutoSize = true;
            this.L_SizeItemOffset.Location = new System.Drawing.Point(155, 78);
            this.L_SizeItemOffset.Name = "L_SizeItemOffset";
            this.L_SizeItemOffset.Size = new System.Drawing.Size(11, 12);
            this.L_SizeItemOffset.TabIndex = 24;
            this.L_SizeItemOffset.Text = "0";
            // 
            // L_SizItemType
            // 
            this.L_SizItemType.AutoSize = true;
            this.L_SizItemType.Location = new System.Drawing.Point(155, 59);
            this.L_SizItemType.Name = "L_SizItemType";
            this.L_SizItemType.Size = new System.Drawing.Size(18, 12);
            this.L_SizItemType.TabIndex = 24;
            this.L_SizItemType.Text = "int";
            // 
            // L_TotalHeaderSize
            // 
            this.L_TotalHeaderSize.AutoSize = true;
            this.L_TotalHeaderSize.Location = new System.Drawing.Point(155, 37);
            this.L_TotalHeaderSize.Name = "L_TotalHeaderSize";
            this.L_TotalHeaderSize.Size = new System.Drawing.Size(11, 12);
            this.L_TotalHeaderSize.TabIndex = 24;
            this.L_TotalHeaderSize.Text = "0";
            // 
            // Combo_SizeItemType
            // 
            this.Combo_SizeItemType.AutoCompleteCustomSource.AddRange(new string[] {
            "Int",
            "Short",
            "Byte"});
            this.Combo_SizeItemType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Combo_SizeItemType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Combo_SizeItemType.FormattingEnabled = true;
            this.Combo_SizeItemType.Items.AddRange(new object[] {
            "int",
            "short",
            "byte"});
            this.Combo_SizeItemType.Location = new System.Drawing.Point(96, 55);
            this.Combo_SizeItemType.Name = "Combo_SizeItemType";
            this.Combo_SizeItemType.Size = new System.Drawing.Size(53, 20);
            this.Combo_SizeItemType.TabIndex = 21;
            this.Combo_SizeItemType.Text = "int";
            // 
            // T_SizeItemOffset
            // 
            this.T_SizeItemOffset.HideSelection = false;
            this.T_SizeItemOffset.Location = new System.Drawing.Point(96, 75);
            this.T_SizeItemOffset.Name = "T_SizeItemOffset";
            this.T_SizeItemOffset.Size = new System.Drawing.Size(53, 21);
            this.T_SizeItemOffset.TabIndex = 22;
            this.T_SizeItemOffset.Text = "0";
            this.T_SizeItemOffset.U_Text = "0";
            // 
            // T_HeaderSize
            // 
            this.T_HeaderSize.HideSelection = false;
            this.T_HeaderSize.Location = new System.Drawing.Point(96, 34);
            this.T_HeaderSize.Name = "T_HeaderSize";
            this.T_HeaderSize.Size = new System.Drawing.Size(53, 21);
            this.T_HeaderSize.TabIndex = 23;
            this.T_HeaderSize.Text = "0";
            this.T_HeaderSize.U_Text = "0";
            // 
            // B_ApplyHeader
            // 
            this.B_ApplyHeader.Location = new System.Drawing.Point(95, 3);
            this.B_ApplyHeader.Name = "B_ApplyHeader";
            this.B_ApplyHeader.Size = new System.Drawing.Size(78, 25);
            this.B_ApplyHeader.TabIndex = 13;
            this.B_ApplyHeader.Text = "Apply▼";
            this.B_ApplyHeader.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "SizeItemOffset";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "SizeItemsType";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "TotalSize";
            // 
            // ConControllers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_Controllers);
            this.Name = "ConControllers";
            this.Size = new System.Drawing.Size(248, 646);
            this.P_Controllers.ResumeLayout(false);
            this.G_IpPortSettings.ResumeLayout(false);
            this.G_IpPortSettings.PerformLayout();
            this.G_ConnModePos.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.G_NetInfo.ResumeLayout(false);
            this.G_NetInfo.PerformLayout();
            this.G_EchoMode.ResumeLayout(false);
            this.G_TimerSettings.ResumeLayout(false);
            this.G_Timeout.ResumeLayout(false);
            this.G_Timeout.PerformLayout();
            this.G_ResponseTimer.ResumeLayout(false);
            this.G_ResponseTimer.PerformLayout();
            this.G_Analysis.ResumeLayout(false);
            this.G_Analysis.PerformLayout();
            this.G_Progress.ResumeLayout(false);
            this.G_Progress.PerformLayout();
            this.G_ConnLeds.ResumeLayout(false);
            this.G_ConnLeds.PerformLayout();
            this.G_ListControlButtons.ResumeLayout(false);
            this.G_MsgBox.ResumeLayout(false);
            this.G_MsgBox.PerformLayout();
            this.G_HeaderDefine.ResumeLayout(false);
            this.G_HeaderDefine.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel P_Controllers;
        private System.Windows.Forms.Panel G_IpPortSettings;
        private System.Windows.Forms.Label label1;
        private FormAdders.IPAddressInputBox T_IP;
        private System.Windows.Forms.Label label2;
        private FormAdders.NumberTextBox T_Port;
        private System.Windows.Forms.Button B_Start;
        private System.Windows.Forms.Panel G_ConnModePos;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton R_ConnPosClient;
        private System.Windows.Forms.RadioButton R_ConnPosServer;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton R_ConnModeUdp;
        private System.Windows.Forms.RadioButton R_ConnModeTcp;
        private System.Windows.Forms.GroupBox G_NetInfo;
        private System.Windows.Forms.CheckBox Ch_CutData;
        private System.Windows.Forms.CheckBox Ch_IsStringWithNull;
        private System.Windows.Forms.Label L_EchoMode;
        private System.Windows.Forms.Label L_ConnSite;
        private System.Windows.Forms.Label L_Format;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label L_MyIp;
        private System.Windows.Forms.Label L_MsgSize;
        private System.Windows.Forms.Label L_Endian;
        private System.Windows.Forms.Label L_ConnType;
        private System.Windows.Forms.GroupBox G_EchoMode;
        private FormAdders.RadioGroup radioGroup1;
        private System.Windows.Forms.Panel G_TimerSettings;
        private System.Windows.Forms.GroupBox G_Timeout;
        private FormAdders.NumberTextBox T_Timeout;
        private System.Windows.Forms.Button B_StopRecv;
        private System.Windows.Forms.GroupBox G_ResponseTimer;
        private FormAdders.NumberTextBox T_ResponseTimer;
        private System.Windows.Forms.Button B_ResponseTimer;
        private FormAdders.OnOffLed Led_ResponseTimer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox G_Analysis;
        private System.Windows.Forms.RadioButton R_MsgFormatELog;
        private System.Windows.Forms.RadioButton R_MsgFormatFloat;
        private System.Windows.Forms.RadioButton R_MsgFormatStr;
        private System.Windows.Forms.RadioButton R_MsgFormatBin;
        private System.Windows.Forms.RadioButton R_MsgFormatDec;
        private System.Windows.Forms.RadioButton R_MsgFormatHex;
        private System.Windows.Forms.GroupBox G_Progress;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label L_Tx;
        private System.Windows.Forms.Label L_Rx;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ProgressBar Pr_SendProgress;
        private System.Windows.Forms.ProgressBar Pr_RecvProgress;
        private System.Windows.Forms.GroupBox G_ConnLeds;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private FormAdders.OnOffLed Led_This;
        private FormAdders.OnOffLed Led_Connect;
        private System.Windows.Forms.Panel G_ListControlButtons;
        private System.Windows.Forms.Button B_RunNext;
        private System.Windows.Forms.Button B_ShowList;
        private System.Windows.Forms.Panel G_MsgBox;
        private System.Windows.Forms.RichTextBox T_Msg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button B_MakeMsg;
        private System.Windows.Forms.Button B_Send;
        private System.Windows.Forms.GroupBox G_HeaderDefine;
        private System.Windows.Forms.Label L_SizeItemOffset;
        private System.Windows.Forms.Label L_SizItemType;
        private System.Windows.Forms.Label L_TotalHeaderSize;
        private System.Windows.Forms.ComboBox Combo_SizeItemType;
        private FormAdders.NumberTextBox T_SizeItemOffset;
        private FormAdders.NumberTextBox T_HeaderSize;
        private System.Windows.Forms.Button B_ApplyHeader;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}
