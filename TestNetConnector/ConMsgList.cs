using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CustomParser;
using RtwEnums.Network;
using FormAdders.EasyGridViewCollections;
using FormAdders;

namespace TestNetConnector
{
    public partial class ConMsgList : UserControl
    {
        String _groupsDir = "SavedPacketGroups";
        public ConMsgList(String packetDir)
        {
            InitializeComponent();
            _packetDir = _groupsDir+"\\"+packetDir;
            init();
            this.Name = packetDir;
        }

        // String _baseDir;
        String _packetDir=null;
        
        enum Titles { check = 0, name, edit_button, send, close, comment};
        
        public ConMsgList(String packetDir, EasyGridView view)
        {
            //InitializeComponent();
            V_Data = view;
            _packetDir = _groupsDir + "\\" + packetDir;
            init();
            this.Name = packetDir;
        }
        enum contentTitles { name, type, value, swap };
        String[] _types = new string[]{
                    "sbyte","byte","short","ushort","int","uint","long","ulong","string"};
        string[] _swapYn = new string[]{"y","n"};
        Dictionary<String, CPacketStruct> _modifiedItems = new Dictionary<String, CPacketStruct>();
        void init()
        {
            V_Data.AddTitleImageCheckColumn(15, Titles.check.ToString());
            V_Data.AddTitleTextBoxColumn(100, Titles.name.ToString(), "Name", true);
            V_Data.AddTitleButtonColumn(30, Titles.edit_button.ToString(), "edit", "edit");
            V_Data.AddTitleButtonColumn(30, Titles.send.ToString(), "send", "send");
            V_Data.AddTitleCloseButtonColumn(15, Titles.close.ToString(), "X", "X");
            V_Data.AddTitleTextBoxColumn(-1, Titles.comment.ToString(), "Comment", true);

            V_Data.E_CellClicked += new FormAdders.EasyGridViewCollections.CellClickEventHandler(V_Data_E_CellClicked);
            V_Data.E_TextChanged += new FormAdders.CellTextChangedEventHandler(V_Data_E_TextChanged);
            V_Data.E_TextEditFinished += new FormAdders.CellTextChangedEventHandler(V_Data_E_TextEditFinished);
            V_Data.E_ListRowRemoving += new CellClickEventHandler(V_Data_E_ListRowRemoving);
            V_Data.E_ListRowRemoved += new CellClickEventHandler(V_Data_E_ListRowRemoved);
            V_Data.E_ContextMenuClicked += new FormAdders.EasyGridViewCollections.EasyGridMenuClickHandler(V_Data_E_ContextMenuClicked);
            V_Data.ActionOnRightClicked = FormAdders.EasyGridViewCollections.Actions.ContextMenu;
            V_Data.ActionOnClicked = Actions.Nothing;
            
            V_Data.AddContextMenuItem(new String[] { "SetAsStartPoint", "Copy This", "Delete Lines", "Make New Msg" });
            V_Data.E_CheckBoxChanged += new FormAdders.CellCheckedEventHandler(V_Data_E_CheckBoxChanged);
            V_Data.E_RowPositionChanged += new RowPositionChangedHandler(V_Data_E_RowPositionChanged);
            //_baseDir = Directory.GetCurrentDirectory();

            V_Contents.AddTitleTextBoxColumn(40, contentTitles.name.ToString(), "Name", true);
            V_Contents.AddTitleComboBoxColumn(50, contentTitles.type.ToString(), "Type", _types, 1);
            V_Contents.AddTitleTextBoxColumn(-1, contentTitles.value.ToString(), "Value", true);
            V_Contents.AddTitleComboBoxColumn(40, contentTitles.swap.ToString(), "Swap", _swapYn, 1);

            V_Contents.E_TextChanged += new CellTextChangedEventHandler(V_Contents_E_TextChanged);
            V_Contents.E_ComboBoxChanged += new CellComboBoxEventHandler(V_Contents_E_ComboBoxChanged);

            B_HideButton.Click += new EventHandler(B_HideButton_Click);
            B_Save.Click += new EventHandler(B_Save_Click);
            initList();
            
        }

        void B_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _modifiedItems.Count; i++)
            {
                CPacketStruct item = _modifiedItems.Values.ElementAt(i);
                String name = _modifiedItems.Keys.ElementAt(i);
                StructXMLParser.ItemsToXml(item.Items, _packetDir+"\\" + name + "\\Msg.xml", item.Infos);
            }
            _modifiedItems.Clear();
            P_Table.RowStyles[3] = new RowStyle(System.Windows.Forms.SizeType.Absolute, 0);
            P_Table.RowStyles[3] = new RowStyle(System.Windows.Forms.SizeType.Absolute, 25);
            
        }

        void B_HideButton_Click(object sender, EventArgs e)
        {
            P_Table.RowStyles[1] = new RowStyle(System.Windows.Forms.SizeType.Absolute, 0);//hide button
            P_Table.RowStyles[2] = new RowStyle(System.Windows.Forms.SizeType.Absolute, 0);
        }

        CPacketStruct _selectedParser;

        void V_Contents_E_ComboBoxChanged(object sender, CellComboBoxEventArgs e)
        {
            int itemIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.itemIndex.ToString()];
            int valueIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.valueIndex.ToString()];
            CPacketStruct parser = V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.parser.ToString()] as CPacketStruct;

            switch((contentTitles)e.ColIndex){
                case contentTitles.swap:
                    parser.Items[itemIndex].IsSwap = (_swapYn[e.SelectedIndex].Equals("y"));
                    break;
                case contentTitles.type:
                    //parser.Items[itemIndex].TypeString = _types[e.SelectedIndex];
                    parser.Items[itemIndex].SetType(_types[e.SelectedIndex], true);
                    break;
            }
            try
            {
                parser.MakePacket(_endian == Endians.Big);
                parser.MakeMsgText();
                String name = V_Contents.RelativeObject["name"] as String;
                _modifiedItems[name] = parser;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void V_Contents_E_TextChanged(object sender, CellTextChangedEventArgs e)
        {
            int itemIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.itemIndex.ToString()];
            int valueIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.valueIndex.ToString()];
            CPacketStruct parser = V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.parser.ToString()] as CPacketStruct;
            
            switch((contentTitles)e.ColIndex){
                case contentTitles.name:
                    if (e.Text.Length == 0)//아무것도 넣지않으면 자동으로 복귀
                    {
                        e.IsCancel = true;
                        return;
                    }
                    else if (Char.IsDigit(e.Text[0]))
                    {
                        MessageBox.Show("변수명의 처음은 문자로 시작해야 합니다.");
                        e.IsCancel = true;
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < e.Text.Length; i++)
                        {
                            if (Char.IsLetterOrDigit(e.Text, i) == false)
                            {
                                MessageBox.Show("변수명에는 문자와 숫자만 들어갈 수 있습니다.");
                                e.IsCancel = true;
                                return;
                            }
                        }
                        for (int i = 0; i < parser.Items.Count; i++)
                        {
                            if (parser.Items[i].Name.Equals(e.Text))
                            {
                                MessageBox.Show("같은 이름이 존재합니다.");
                                e.IsCancel = true;
                                return;
                            }
                        }
                        
                    }
                    parser.Items[itemIndex].Name = e.Text;

                    if (parser.Items[itemIndex].InitValues.Length > 1)//배열일 때,
                    {
                        viewContent(parser);//목록을 다시 만듬. 
                    }
                    
                    break;
                case contentTitles.value:
                    parser.Items[itemIndex].InitValues[valueIndex] = e.Text;
                    break;
            }
            parser.MakePacket(_endian == Endians.Big);
            parser.MakeMsgText();
            String name = V_Contents.RelativeObject["name"] as String;
            _modifiedItems[name] = parser;
            
        }

        void V_Data_E_TextEditFinished(object sender, FormAdders.CellTextChangedEventArgs e)
        {
            saveNameList();
        }

        void V_Data_E_ListRowRemoved(object sender, CellClickEventArgs e)
        {
            saveNameList();
        }

        void V_Data_E_RowPositionChanged(object sender, RowPositionChangedArgs args)
        {
            saveNameList();
        }

        void saveNameList()
        {
            String orderFile = _packetDir + "\\ListOrder.txt";
            File.Delete(orderFile);
            if (Directory.Exists(_packetDir + "\\Backups"))
            {
                if (V_Data.Rows.Count != Directory.GetDirectories(_packetDir).Length - 1)
                {
                    //MessageBox.Show("디렉토리 개수와 목록의 개수가 달라서 목록을 다시 만듭니다.");
                    //File.Delete(orderFile);
                    makeNewOrderIfNotExists();
                    initList();
                    return;
                }
            }
            else
            {
                if (V_Data.Rows.Count != Directory.GetDirectories(_packetDir).Length)
                {
                    //MessageBox.Show("디렉토리 개수와 목록의 개수가 달라서 목록을 다시 만듭니다.");
                    //File.Delete(orderFile);
                    makeNewOrderIfNotExists();
                    initList();
                    return;
                }
            }
            StreamWriter sw = new StreamWriter(File.OpenWrite(orderFile));

            //try
            {
                string root = Directory.GetCurrentDirectory();
                int rootLen = root.Length + 1;

                for (int i = 0; i < V_Data.RowCount; i++)
                {
                    String name = V_Data.GetValue(i, (int)Titles.name) as String;
                    String dir = _packetDir + "\\" + name;
                    
                    if (Directory.Exists(dir) == false)
                    {
                        MessageBox.Show(dir+"라인이 무결성을 통과 못하여 목록을 다시 만듭니다.");
                        sw.Close();
                        File.Delete(orderFile);
                        makeNewOrderIfNotExists();
                        initList();
                        return;
                    }
                    sw.WriteLine(name);
                    //if (dir.Contains(root)) sw.WriteLine(dir.Substring(rootLen));
                    //else sw.WriteLine(name);
                }
            }
            //catch (Exception e)
            {

              //  MessageBox.Show(e.Message);

            }
            //finally
            {
                sw.Close();
                
            }
        }

        void V_Data_E_CheckBoxChanged(object sender, FormAdders.CellCheckedEventArgs e)
        {
            

            for (int i = e.StartRowIndex; i <= e.EndRowIndex; i++)
            {
                String state = _packetDir + "\\" + (V_Data.Rows[i][Titles.name].Value as String) + "\\States.txt";
                if (e.Checked == true)
                {
                    if (V_Data.Rows[i].RowBackMode == RowBackModes.Red)
                    {
                        _startIndex = -1;
                    }
                    V_Data.Rows[i].RowBackMode = RowBackModes.Gray;
                    File.WriteAllText(state, "check=y");
                }
                else
                {
                    V_Data.Rows[i].RowBackMode = RowBackModes.None;
                    File.WriteAllText(state, "check=n");
                }

                //int itemIndex = (int)V_Contents.Rows[i].RelativeObject[rowInfos.itemIndex.ToString()];
                //int valueIndex = (int)V_Contents.Rows[i].RelativeObject[rowInfos.valueIndex.ToString()];
                CPacketStruct parser = V_Data.Rows[i].RelativeObject[rowInfos.parser.ToString()] as CPacketStruct;
                parser.Infos.Checked = (V_Data.Rows[i].Cells[0].Value.Equals(1));
                //String name = V_Data.RelativeObject["name"] as String;
                //_modifiedItems[name] = parser;
            
            }
        }
        void V_Data_E_TextChanged(object sender, FormAdders.CellTextChangedEventArgs e)
        {
            
            if (e.ColIndex == (int)Titles.name)//name change
            {
                if (Directory.Exists(_packetDir + "\\" + e.Text))
                {
                    MessageBox.Show("이미 같은 이름을 가진 메시지가 존재합니다. 다른이름으로 해 주세요.");
                    e.IsCancel = true;
                    
                    return;
                }
                try
                {
                    Directory.Move(_packetDir + "\\" + e.BeforeText, _packetDir + "\\" + e.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.ToString());
                    e.IsCancel = true;
                    return;
                }
                if (_modifiedItems.ContainsKey(e.BeforeText))
                {
                    _modifiedItems.Add(e.Text, _modifiedItems[e.BeforeText]);
                    _modifiedItems.Remove(e.BeforeText);
                }
            }
            else if (e.ColIndex == (int)Titles.comment)//comment change
            {
                String commentFile = _packetDir + "\\" + (V_Data.GetCell(e.RowIndex, 1).Value as String) + "\\Comment.txt";
                File.WriteAllText(commentFile, e.Text);
                int itemIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.itemIndex.ToString()];
                int valueIndex = (int)V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.valueIndex.ToString()];
                CPacketStruct parser = V_Contents.Rows[e.RowIndex].RelativeObject[rowInfos.parser.ToString()] as CPacketStruct;
                parser.Infos.Comment = e.Text;
                String name = (V_Data.GetCell(e.RowIndex, 1).Value as String);
                _modifiedItems[name] = parser;
            
            }
            //saveNameList();

        }
        void V_Data_E_CellClicked(object sender, FormAdders.EasyGridViewCollections.CellClickEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColIndex < 0) return;
            String packetName =  (V_Data.GetCell(e.RowIndex, 1).Value as String);
            if (e.ColIndex == (int)Titles.edit_button) //edit msg
            {
                CPacketStruct cp = V_Data.RowRelativeObject(e.RowIndex)["parser"] as CPacketStruct;
                cp.MakeMsgText();
                //cp.MakeMsg();
                String msgFile = _packetDir + "\\" + packetName + "\\Msg.txt";
                DlgMsgMaker msgMaker = new DlgMsgMaker(cp, msgFile, _endian == Endians.Big);
                msgMaker.Location = this.Location;
                msgMaker.ShowDialog();
                cp.MakePacket(_endian == Endians.Big);
                //cp.MakeMsg();//다시 메시지를 만들어준다.
            }
            else if (e.ColIndex == (int)Titles.send) //send msg
            {
                CPacketStruct cp = V_Data.RowRelativeObject(e.RowIndex)["parser"] as CPacketStruct;
                cp.MakePacket(_endian == Endians.Big);
                _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp);
                ReleaseSelections();
                V_Data.Rows[e.RowIndex].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.Red;
            }
            else//내용보기
            {
                V_Contents.RelativeObject["name"] = packetName;
                CPacketStruct cp = V_Data.RowRelativeObject(e.RowIndex)["parser"] as CPacketStruct;
                viewContent(cp);
            }
        }

        enum rowInfos{itemIndex, valueIndex, parser};
        void viewContent(CPacketStruct cp)
        {
            _selectedParser = cp;
            V_Contents.ClearData();
            V_Contents.RefreshList();
            for (int i = 0; i < cp.Count; i++)
            {
                for (int v = 0; v < cp.Items[i].InitValues.Length; v++)
                {
                    Dictionary<String, object> infos = new Dictionary<string, object>();
                    infos[rowInfos.itemIndex.ToString()] = i;
                    infos[rowInfos.valueIndex.ToString()] = v;
                    infos[rowInfos.parser.ToString()] = cp;
                    String name = cp.Items[i].Name;
                    if (cp.Items[i].InitValues.Length > 1) name += "[" + v + "]";
                    V_Contents.AddARow(infos, new object[]{
                    name.Trim(),
                    _types.ToList().IndexOf(cp.Items[i].TypeString),
                    cp.Items[i].InitValues[v].Trim(),
                    _swapYn.ToList().IndexOf( (cp.Items[i].IsSwap)?"y" : "n" )});
                }
            }
            float height = V_Contents.Rows.Count * V_Contents.BaseRowHeight+ V_Contents.ColumnHeaderHeight + 5;//
            if (height > 500) height = 500f;
            if (height < 200) height = 200f;
            P_Table.RowStyles[1] = new RowStyle(System.Windows.Forms.SizeType.Absolute, 15);//hide button
            P_Table.RowStyles[2] = new RowStyle(System.Windows.Forms.SizeType.Absolute, height);
        }

        void V_Data_E_ListRowRemoving(object sender, CellClickEventArgs e)
        {
            if (DeleteLine(e.RowIndex,true) == false)
            {
                e.IsCancel = true;
                return;
            }


        }
        bool DeleteLine(int rowIndex, bool isConfirm=true)
        {
            String name = V_Data.GetCell(rowIndex, (int)Titles.name).Value as String;
            if (isConfirm)
            {
                DialogResult result = MessageBox.Show("[" + name + "] 을 진짜 지울까요?", "삭제확인", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.No)
                {

                    return false;
                }
            }


            String dir = _packetDir + "\\" + name;
            String newDir = _packetDir + "\\Backups\\" + name;
            if (Directory.Exists(_packetDir + "\\Backups") == false) Directory.CreateDirectory(_packetDir + "\\Backups");

            if (Directory.Exists(newDir)) newDir += DateTime.Now.ToString("yyyyMMdd_HHmmss");
            try
            {
                Directory.Move(dir, newDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("문제가 있어서 삭제하지 못했습니다. 다음의 메시지를 확인해주세요." + ex.Message);
                
                return false;
            }
            return true;
        }

        int _startIndex = 0;
        void V_Data_E_ContextMenuClicked(object sender, FormAdders.EasyGridViewCollections.EasyGridMenuClickArgs args)
        {
            switch (args.Text)
            {
                case "SetAsStartPoint":
                    if (SetAsStartPoint(args.RowIndex) == false)
                    {
                        MessageBox.Show("비활성된 라인입니다. 체크를 풀어주세요.");
                    }
                    break;
                case "Copy This":
                    {
                        String name = V_Data.GetValue(args.RowIndex, (int)Titles.name) as String;
                        String msg = V_Data.RowRelativeObject(args.RowIndex)["msg"] as String;
                        CPacketStruct parser = V_Data.Rows[args.RowIndex].RelativeObject["parser"] as CPacketStruct;
                        AddNewItem(parser, name);
                        //initList();
                    }
                    break;
                case "Delete Lines":
                    if (V_Data.SelectedRows.Count > 0)
                    {
                        List<EasyGridRow> rows = new List<EasyGridRow>(V_Data.SelectedRows);
                        foreach (EasyGridRow row in rows)
                        {
                            DeleteLine(row.Index);
                        }
                        foreach (EasyGridRow row in rows)
                        {
                            V_Data.RemoveARow(row, false);
                        }
                        V_Data.RefreshList();
                        saveNameList();
                    }
                    break;
                case "Make New Msg":
                    {
                        InputForm input = new InputForm();
                        DialogResult result = input.ShowDialog("새 메시지 이름");
                        if(result== DialogResult.Cancel || result== DialogResult.Abort) return;
                        AddNewItem(null, input.InputText);
                    }
                    break;
            }
        }

        public bool RunNext()
        {
            if (_startIndex < 0)
            {
                MessageBox.Show("StartLine을 지정해 주십시오. 마우스 오른쪽 버튼을 이용하십시오.");
                return false;
            }
            CPacketStruct cp;
            int count=0;
            while ((((cp = GetStructParser(_startIndex)) == null) || (int)V_Data.Rows[_startIndex][Titles.check].Value ==1) 
                && (count++ < V_Data.RowCount))
            {
                _startIndex = (_startIndex + 1) % V_Data.RowCount;//정상적인 것이 나올때까지 돌아간다.
            }
            if (count >= V_Data.RowCount)
            {
                MessageBox.Show("정상적인 라인이 하나도 없어 실행할 수 없습니다.");
                return false;
            }
            if (_sendFunc == null) return false;
            if (cp.IsDynamicPacket)
            {
                cp.MakePacket(_endian == Endians.Big);
            }
            else
            {
                //cp = V_Data.RowRelativeObject(_startIndex)["parser"] as CPacketStruct;
            }
            //cp.MakePacket(_endian == Endians.Big);
            _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp);

            int next = (_startIndex + 1) % V_Data.RowCount;
            count = 0;
            
            while (SetAsStartPoint(next)==false && count++<V_Data.RowCount+1)
            {
                next = (next + 1) % V_Data.RowCount;
            }
            
            if (count > V_Data.RowCount + 1)
            {
                //MessageBox.Show("체크가 풀린 칸이 하나도 없습니다.");
                return false;
            }
            _startIndex = next;
            return true;
        }

        public void RunSelected()
        {
            if (V_Data.SelectedItems.Count == 0)
            {
                MessageBox.Show("StartLine을 지정해 주십시오. 마우스 오른쪽 버튼을 이용하십시오.");
                return;
            }
            
            CPacketStruct cp = GetStructParser(V_Data.SelectedItems[0].Index);
            if (cp.IsDynamicPacket) cp.MakePacket(_endian == Endians.Big);
            if (_sendFunc == null) return;
            _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp);

        }

        bool SetAsStartPoint(int rowIndex)
        {
            if (V_Data.Rows[rowIndex][Titles.check].Value.Equals(1))
            {
                return false;
            }
            ReleaseSelections();
            //V_Data.Rows[_startIndex].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.None;
            V_Data.Rows[rowIndex].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.Red;
            _startIndex = rowIndex;
            return true;
        }

        Endians _endian = Endians.Big;
        bool _isStringWithNullEnd = false;
        //StringEncoding { UTF8 = 0, UTF7 = 1, UTF16, UTF32, ASCII };
        
        Encoding _strEncoding = Encoding.UTF8;
        public delegate int SendFunc(Byte[] buff, int offset, int size, CPacketStruct text);
        SendFunc _sendFunc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendFunc">int func(byte[],int offset, int size). send를 누를 때 buff를 보낼 sendFunction.</param>
        /// <param name="endian"></param>
        /// <param name="isStringWithNullEnd"></param>
        /// <param name="stringEncoding"></param>
        public void setEnv(SendFunc sendFunc, Endians endian= Endians.Big, bool isStringWithNullEnd=false, StringEncodings stringEncoding=StringEncodings.UTF8){
            _endian = endian;
            _isStringWithNullEnd = isStringWithNullEnd;
            switch (stringEncoding)
            {
                case StringEncodings.Unicode:
                    _strEncoding = Encoding.Unicode;
                    break;
                case StringEncodings.UTF8:
                    _strEncoding = Encoding.UTF8;
                    break;
                case StringEncodings.UTF7:
                    _strEncoding = Encoding.UTF7;
                    break;
                case StringEncodings.UTF32:
                    _strEncoding = Encoding.UTF32;
                    break;
                case StringEncodings.ASCII:
                    _strEncoding = Encoding.ASCII;
                    break;
            }
            _sendFunc = sendFunc;
            initList();
        }

        void makeNewOrderIfNotExists()
        {
            String listDir = _packetDir;// = _baseDir + "\\PacketList";
            String orderFile = listDir + "\\ListOrder.txt";
            
            if (Directory.Exists(listDir) == false) 
                Directory.CreateDirectory(listDir);

            #region 순서파일이 없으면 만들어줌...
            if (File.Exists(orderFile) == false)
            {
                StreamWriter sw = new StreamWriter(File.OpenWrite(orderFile));
                String[] dirs = Directory.GetDirectories(listDir);
                foreach (String dir in dirs)
                {
                    String name = dir.Substring(dir.LastIndexOf("\\") + 1);
                    if (name.Equals("Backups")) continue;
                    sw.WriteLine(name);
                }
                sw.Close();
            }
            #endregion

        }

        public void initList()
        {
            
            V_Data.ClearData();
            //if (packetDir != null) _packetDir = packetDir;
            if (_packetDir == null) return;
            String listDir= _packetDir;// = _baseDir+"\\PacketList";
            String orderFile = _packetDir + "\\ListOrder.txt";

            makeNewOrderIfNotExists();

            List<String> listOrder = new List<string>();
            #region 순서를 가져옴
            StreamReader sr = new StreamReader(File.OpenRead(orderFile));
            String line;
            //String rootDir = Directory.GetCurrentDirectory() + "\\";
            while ((line = sr.ReadLine()) != null)
            {

                if (line.Trim().Length > 0)
                {
                    listOrder.Add(line);
                    //if (line.IndexOf(rootDir) == 0) listOrder.Add(line);
                    //else listOrder.Add(rootDir + line);
                }
            }
            sr.Close();

            #endregion
            String[] dirs = Directory.GetDirectories(_packetDir);

            for(int i=0; i<dirs.Length; i++)
            {
                String name = dirs[i].Substring(dirs[i].LastIndexOf("\\")+1);
                if (name.Equals("Backups")) continue;//백업디렉토리는 제외..

                if (listOrder.Contains(name) == false) //만일 리스트에 없으면..
                {
                    File.Delete(orderFile);
                    makeNewOrderIfNotExists();
                    listOrder.Clear();
                    for (int d = 0; d < dirs.Length; d++)//새로만듬..
                    {
                        name = dirs[i].Substring(dirs[i].LastIndexOf("\\")+1);
                        if (name.Equals("Backups")) continue;//백업디렉토리는 제외..
                        listOrder.Add(name);
                    }
                    
                    break;
                }
            }
            //if (Directory.Exists(listDir) == false) Directory.CreateDirectory(listDir);
            //else
            {
                //CStructParser parser;// = new CStructParser();
                
                //foreach (String dir in Directory.GetDirectories(listDir))
                foreach(String name in listOrder)
                {
                    
                    String Msg="";
                    bool integrity=false;
                    String path = _packetDir + "\\" + name;
                    String msgXml = path + "\\Msg.xml";
                    String msgTxt = path + "\\Msg.txt";
                    CPacketStruct parser = new CPacketStruct();
                    
                    IList<CPacketItem> itemsList = null;
                    Dictionary<String, object> rels = new Dictionary<string,object>();
                    //try
                    {
                        if (File.Exists(msgXml))
                        {
                            //try
                            {
                                itemsList = StructXMLParser.XmlToItems(msgXml, parser);
                                Msg = StructXMLParser.ItemsToCode(itemsList);
                                integrity = true;
                            }
                            //catch {
                            /*
                                integrity = false;
                                if (File.Exists(msgTxt))
                                {
                                    Msg = File.ReadAllText(msgTxt);
                                }
                                else
                                {

                                    MessageBox.Show(msgXml + "이나 "+ msgTxt+" 가 없습니다.메시지를 다시 작성하십시오.");//필수파일
                                    File.WriteAllText(msgTxt, "");

                                }
                             */
                            //}
                        }
                        else if (File.Exists(msgTxt))
                        {
                            Msg = File.ReadAllText(msgTxt);
                        }
                        else
                        {

                            //MessageBox.Show(msgXml + "이나 " + msgTxt + " 가 없습니다.메시지를 다시 작성하십시오.");//필수파일
                            if(Directory.Exists(path)) Directory.Delete(path, true);
                            //File.WriteAllText(msgTxt,"");

                        }
                    }
                    //catch
                    {
                    //    continue;
                    }
                    String comment = "";
                    if(File.Exists(path+"\\Comment.txt")){
                        comment = File.ReadAllText(path + "\\Comment.txt");
                    }
                    Dictionary<String, String> states = new Dictionary<string, string>();
                    if (File.Exists(path + "\\States.txt"))
                    {
                        String[] lines = File.ReadAllLines(path + "\\States.txt");
                        for (int i = 0; i < lines.Length; i++)
                        {
                            String[] tokens = lines[i].Split("=;".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length > 1)
                            {
                                states[tokens[0].ToLower().Trim()] = tokens[1].ToLower().Trim();
                            }
                        }
                    }
                    bool isChecked = (states.ContainsKey("check")) ? (states["check"].Equals("y") || states["check"].Equals("true")) : false;

                    parser.IsStringWithNullEnd = _isStringWithNullEnd;
                    parser.Endian = _endian;
                    parser.StringEncoding = _strEncoding;

                    String msgName = name;
                   //try
                    {
                        if (itemsList == null)
                        {
                            parser.MakePacket(Msg, _endian == Endians.Big);
                            integrity = true;
                        }
                        else
                        {
                            //parser.Items.Clear();
                            parser.Items.CopyFrom(itemsList);
                            //parser.Items = itemsList as List<CPacketItem>;

                            parser.MakePacket(_endian == Endians.Big);
                        }
                        
                    }
                    //catch(Exception e)
                    {
                    //    MessageBox.Show(msgName+":"+e.Message);
                        integrity = true;
                    }
                    rels["integrity"] = integrity;
                    rels["parser"] = parser;
                    rels["msg"] = Msg;

                    EasyGridRow row = V_Data.AddARow(rels, new object[]{
                        isChecked,
                        msgName,
                        "edit",
                        "send",
                        "X",
                        comment

                    });
                    if (integrity == false) (row[(int)Titles.name] as EasyGridTextBoxCell).FontColor = Color.Red;
                    if (isChecked) row.RowBackMode = RowBackModes.Gray;
                }
            }
            if (V_Data.Rows.Count > 0)
            {
                if (_startIndex < V_Data.Rows.Count)
                {
                    V_Data.Rows[_startIndex].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.Red;
                }
                //V_Data.Rows[0].RowBackCustomColor = Color.FromArgb(255, 222, 222);
            }
            V_Data.RefreshList();
        }

 

        public CPacketStruct GetStructParser(int row)
        {
            bool integrity = (bool)V_Data.RowRelativeObject(row)["integrity"];
            if (row > V_Data.RowCount - 1 || integrity==false)
            {
                return null;
            }
            else
            {
                return V_Data.RowRelativeObject(row)["parser"] as CPacketStruct;
            }
        }



        public void ReleaseSelections()
        {
            for (int i = 0; i < V_Data.RowCount; i++)
            {
                if(V_Data.Rows[i].RowBackMode == RowBackModes.Red)
                    V_Data.Rows[i].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.None;
            }
        }

        public void AddNewItem(CPacketStruct parserObj=null, String name=null)
        {
            
            #region 기존에 같은 이름이 있는지 검사후 있으면 이름 붙여줌.
            List<object> names = V_Data.GetAColumnData(0);
            name = (name==null)?"Untitled" : name;
            String newName = name;

            int count=0;
            while (Directory.Exists(_packetDir+"\\"+(newName)) == true)
            {
                newName = name + (count++);
            }
            #endregion

            #region 새 디렉토리 만들고 Msg.txt파일 만들어 내용 넣어줌.
            String newDir = _packetDir + "\\" + newName;
            
            if(Directory.Exists(newDir)==false) Directory.CreateDirectory(newDir);


            if (parserObj == null)
            {
                parserObj = new CPacketStruct();
                DlgMsgMaker dlg = new DlgMsgMaker(parserObj, newDir + "\\Msg.txt", _endian == Endians.Big);
                
                DialogResult result = dlg.ShowDialog();
                if (result == DialogResult.Abort || result == DialogResult.Cancel) return;
                //parserObj.Items = 
                StructXMLParser.CodeToItems(parserObj.NativeText, parserObj);
                StructXMLParser.ItemsToXml(parserObj.Items, newDir + "\\Msg.xml", parserObj.Infos);
            }
            else
            {
                parserObj.MakeMsgText();
                CPacketStruct oldObj = parserObj;
                
                parserObj = new CPacketStruct();
                parserObj.NativeText = oldObj.NativeText;
                //parserObj.Items = 
                StructXMLParser.CodeToItems(oldObj.NativeText, parserObj);
                StructXMLParser.ItemsToXml(parserObj.Items, newDir + "\\Msg.xml", parserObj.Infos);
            }
            
            
            #endregion

            #region 표에 줄 삽입
            
            Dictionary<String, object> rels = new Dictionary<string, object>();

            parserObj.IsStringWithNullEnd = _isStringWithNullEnd;
            parserObj.Endian = _endian;
            parserObj.StringEncoding = _strEncoding;

            String msgName = newName;
            bool integrity;
            try
            {
                parserObj.MakePacket(_endian == Endians.Big);
                integrity = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(msgName + ":" + e.Message);
                integrity = true;
            }
            rels["integrity"] = integrity;
            rels["parser"] = parserObj;
            rels["msg"] = parserObj.NativeText;

            EasyGridRow row = V_Data.AddARow(rels, new object[]{
                false,
                msgName,
                "edit",
                "send",
                "X",
                "",
                    });
            if (integrity == false) (row[(int)Titles.name] as EasyGridTextBoxCell).FontColor = Color.Red;

            #endregion
            
            V_Data.RefreshList();
            saveNameList();
        }
    }
}
