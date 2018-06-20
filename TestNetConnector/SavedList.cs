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

namespace TestNetConnector
{
    public partial class SavedList : Form
    {
        String _baseDir;
        String _packetDir;
        enum Titles { check = 0, name, edit_button, send, close, comment};
        public SavedList()
        {
            InitializeComponent();
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
            V_Data.AddContextMenuItem(new String[] { "SetAsStartPoint", "Copy This", "Delete Lines" });
            V_Data.E_CheckBoxChanged += new FormAdders.CellCheckedEventHandler(V_Data_E_CheckBoxChanged);
            V_Data.E_RowPositionChanged += new RowPositionChangedHandler(V_Data_E_RowPositionChanged);
            _baseDir = Directory.GetCurrentDirectory();
            initList();
            C_OnTop.CheckedChanged += new EventHandler(C_OnTop_CheckedChanged);
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
                    MessageBox.Show("디렉토리 개수와 목록의 개수가 달라서 목록을 다시 만듭니다.");
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
                    MessageBox.Show("디렉토리 개수와 목록의 개수가 달라서 목록을 다시 만듭니다.");
                    //File.Delete(orderFile);
                    makeNewOrderIfNotExists();
                    initList();
                    return;
                }
            }
            StreamWriter sw = new StreamWriter(File.OpenWrite(orderFile));

            try
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
                    sw.WriteLine(dir.Substring(rootLen));
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);

            }
            finally
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
            }
            else if (e.ColIndex == (int)Titles.comment)//comment change
            {
                String commentFile = _packetDir + "\\" + (V_Data.GetCell(e.RowIndex, 1).Value as String) + "\\Comment.txt";
                File.WriteAllText(commentFile, e.Text);
            }
            //saveNameList();

        }
        void V_Data_E_CellClicked(object sender, FormAdders.EasyGridViewCollections.CellClickEventArgs e)
        {
            if (e.ColIndex == (int)Titles.edit_button) //edit msg
            {
                CPacketStruct cp = V_Data.RowRelativeObject(e.RowIndex)["parser"] as CPacketStruct;
                cp.MakeMsgText();
                String msgFile = _packetDir + "\\" + (V_Data.GetCell(e.RowIndex, 1).Value as String) + "\\Msg.txt";
                DlgMsgMaker msgMaker = new DlgMsgMaker(cp, msgFile, _endian == Endians.Big);
                msgMaker.Location = this.Location;
                msgMaker.ShowDialog();
                
            }
            else if (e.ColIndex == (int)Titles.send) //send msg
            {
                CPacketStruct cp = V_Data.RowRelativeObject(e.RowIndex)["parser"] as CPacketStruct;
                cp.MakePacket(_endian == Endians.Big);
                _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp.SimpleText);
                ReleaseSelections();
                V_Data.Rows[e.RowIndex].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.Red;
            }
        }
        void C_OnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = C_OnTop.Checked;
        }
        
        bool _isDisposing = false;
        public new void Close(){
            _isDisposing = true;
            base.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isDisposing == false)
            {
                e.Cancel = true;
                this.Hide();
            }
            else e.Cancel = false;
            
            base.OnClosing(e);
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
                    String name = V_Data.GetValue(args.RowIndex, (int)Titles.name) as String;
                    String msg = V_Data.RowRelativeObject(args.RowIndex)["msg"] as String;
                    AddNewItem(msg, name, true);
                    //initList();
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
            _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp.SimpleText);

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
            if (_startIndex < 0)
            {
                MessageBox.Show("StartLine을 지정해 주십시오. 마우스 오른쪽 버튼을 이용하십시오.");
                return;
            }
            CPacketStruct cp = GetStructParser(_startIndex);

            if (_sendFunc == null) return;
            _sendFunc(cp.PacketBuffer, 0, cp.PacketDataSize, cp.SimpleText);

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
        public enum StrEncoding { UTF8, UTF7, Unicode, UTF32, ASCII };
        Encoding _strEncoding = Encoding.UTF8;
        public delegate int SendFunc(Byte[] buff, int offset, int size, String text);
        SendFunc _sendFunc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendFunc">int func(byte[],int offset, int size). send를 누를 때 buff를 보낼 sendFunction.</param>
        /// <param name="endian"></param>
        /// <param name="isStringWithNullEnd"></param>
        /// <param name="stringEncoding"></param>
        public void setEnv(SendFunc sendFunc, Endians endian= Endians.Big, bool isStringWithNullEnd=false, StrEncoding stringEncoding=StrEncoding.UTF8){
            _endian = endian;
            _isStringWithNullEnd = isStringWithNullEnd;
            switch (stringEncoding)
            {
                case StrEncoding.Unicode:
                    _strEncoding = Encoding.Unicode;
                    break;
                case StrEncoding.UTF8:
                    _strEncoding = Encoding.UTF8;
                    break;
                case StrEncoding.UTF7:
                    _strEncoding = Encoding.UTF7;
                    break;
                case StrEncoding.UTF32:
                    _strEncoding = Encoding.UTF32;
                    break;
                case StrEncoding.ASCII:
                    _strEncoding = Encoding.ASCII;
                    break;
            }
            _sendFunc = sendFunc;
            initList();
        }

        void makeNewOrderIfNotExists()
        {
            String orderFile = _packetDir + "\\ListOrder.txt";
            String listDir = _packetDir = _baseDir + "\\PacketList";
            
            if (Directory.Exists(listDir) == false) Directory.CreateDirectory(listDir);

            #region 순서파일이 없으면 만들어줌...
            if (File.Exists(orderFile) == false)
            {
                StreamWriter sw = new StreamWriter(File.OpenWrite(orderFile));
                String root = Directory.GetCurrentDirectory();
                int rootlen = root.Length+1;
                String[] dirs = Directory.GetDirectories(listDir);
                foreach (String dir in dirs)
                {
                    sw.WriteLine(dir.Substring(rootlen));
                }
                sw.Close();
            }
            #endregion

        }

        void initList()
        {
            V_Data.ClearData();
            String listDir= _packetDir = _baseDir+"\\PacketList";
            String orderFile = _packetDir + "\\ListOrder.txt";

            makeNewOrderIfNotExists();

            List<String> listOrder = new List<string>();
            #region 순서를 가져옴
            StreamReader sr = new StreamReader(File.OpenRead(orderFile));
            String line;
            String rootDir = Directory.GetCurrentDirectory() + "\\";
            while ((line = sr.ReadLine()) != null)
            {

                if (line.Trim().Length > 0)
                {
                    if (line.IndexOf(rootDir) == 0) listOrder.Add(line);
                    else listOrder.Add(rootDir + line);
                }
            }
            sr.Close();

            #endregion

            //if (Directory.Exists(listDir) == false) Directory.CreateDirectory(listDir);
            //else
            {
                CPacketStruct parser;// = new CStructParser();
                
                //foreach (String dir in Directory.GetDirectories(listDir))
                foreach(String dir in listOrder)
                {
                    if (dir.Substring(dir.LastIndexOf("\\") + 1).Equals("Backups")) continue;//백업디렉토리는 제외..
                    String Msg="";
                    Dictionary<String, object> rels = new Dictionary<string,object>();
                    try
                    {
                        if (File.Exists(dir + "\\Msg.txt") == false)
                        {
                            MessageBox.Show(dir + "\\Msg.txt 가 없습니다.메시지를 다시 작성하십시오.");//필수파일
                            File.WriteAllText(dir + "\\Msg.txt", "");

                        }
                        else
                        {
                            Msg = File.ReadAllText(dir + "\\Msg.txt");
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    String comment = "";
                    if(File.Exists(dir+"\\Comment.txt")){
                        comment = File.ReadAllText(dir+"\\Comment.txt");
                    }
                    Dictionary<String, String> states = new Dictionary<string, string>();
                    if (File.Exists(dir + "\\States.txt"))
                    {
                        String[] lines = File.ReadAllLines(dir + "\\States.txt");
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
                    
                    parser = new CPacketStruct();
                    parser.IsStringWithNullEnd = _isStringWithNullEnd;
                    parser.Endian = _endian;
                    parser.StringEncoding = _strEncoding;

                    String msgName = dir.Substring(dir.LastIndexOf("\\")+1);
                    bool integrity;
                    try
                    {
                        parser.MakePacket(Msg, _endian == Endians.Big);
                        integrity = true;
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(msgName+":"+e.Message);
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
                V_Data.Rows[0].RowBackMode = FormAdders.EasyGridViewCollections.RowBackModes.Red;
                //V_Data.Rows[0].RowBackCustomColor = Color.FromArgb(255, 222, 222);
            }
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

        public void AddNewItem(String msgText, String name=null, bool allowSameMsg=false)
        {
            if (msgText.Trim().Length == 0)
            {
                MessageBox.Show("메시지가 비었습니다.");
                return;
            }
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
            #region 같은 Text가 있으면 같은 라인을 선택해 줌.. 아니면 추가..
            if (allowSameMsg == false)
            {
                for (int i = 0; i < V_Data.RowCount; i++)
                {
                    String msg = V_Data.RowRelativeObject(i)["msg"] as String;
                    if (msg.CompareTo(msgText) == 0)
                    {
                        V_Data.ReleaseSelection();
                        V_Data.Rows[i].Selected = true;
                        return;///같은 item은 추가하지 않음..
                    }
                }
            }
            #endregion
            #region 새 디렉토리 만들고 Msg.txt파일 만들어 내용 넣어줌.
            String newDir = _packetDir + "\\" + newName;
            try
            {
                Directory.CreateDirectory(newDir);
            }
            catch //이미 있는 경우.. 그냥 지나침..
            {
            }
            if (msgText == null) msgText = "";
            File.WriteAllText(newDir + "\\Msg.txt", msgText);

            #endregion

            #region 표에 줄 삽입
            CPacketStruct parser = new CPacketStruct();
            Dictionary<String, object> rels = new Dictionary<string, object>();

            parser.IsStringWithNullEnd = _isStringWithNullEnd;
            parser.Endian = _endian;
            parser.StringEncoding = _strEncoding;

            String msgName = newName;
            bool integrity;
            try
            {
                parser.MakePacket(msgText, _endian == Endians.Big);
                integrity = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(msgName + ":" + e.Message);
                integrity = true;
            }
            rels["integrity"] = integrity;
            rels["parser"] = parser;
            rels["msg"] = msgText;

            EasyGridRow row = V_Data.AddARow(rels, new object[]{
                false,
                msgName,
                "edit",
                "send",
                "X",
                "",
                    });
            if (integrity == false) (row[(int)Titles.name] as EasyGridTextBoxCell).FontColor = Color.Red;

            if (allowSameMsg == true) row.Selected = true;
            #endregion
            
            V_Data.RefreshList();
            saveNameList();
        }
    }
}
