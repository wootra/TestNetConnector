using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using FormAdders.EasyGridViewCollections;

namespace UserDialogs
{
    public partial class RawDataViewer : Form
    {
        public RawDataViewer()
        {
            InitializeComponent();
            V_Data.U_ShowIndex = true;
            V_Data.U_IndexWidth = 70;
        }

        public RawDataViewer(String initDir)
        {
            InitializeComponent();
            _initDir = initDir;
            V_Data.U_ShowIndex = true;
            V_Data.U_IndexWidth = 70;
            L_Log.SelectedIndexChanged += new EventHandler(L_Log_SelectedIndexChanged);
            V_Data.E_CellDoubleClicked += new CellClickEventHandler(V_Data_E_CellDoubleClicked);
            T_From.Leave += new EventHandler(T_From_Leave);
        }

        void DataViewer_Load(object sender, EventArgs e)
        {
            B_FileOpen_Click(this, null);
        }

        public void SetOpenFileDialog()
        {
            this.Load += new EventHandler(DataViewer_Load);
            
        }

        void T_From_Leave(object sender, EventArgs e)
        {
            int num;
            if(int.TryParse(T_From.Text, out num)){
                Track_From.Value = num;
            }
        }

        void L_Log_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index = L_Log.SelectedIndex;
            
            //if (index < 0) return;
            
            String line = L_Log.SelectedItem as String;
            if (line == null) return;

            String numStr = line.Split(":(".ToCharArray())[1];
            int num;
            if (int.TryParse(numStr, out num))
            {

                T_From.Text = (num-1).ToString();
                
                Track_From.Value = num-1;
                
                
                V_Data.ReleaseSelection();

                List<object> list = V_Data.GetAColumnData(1);//get line col
                int index = list.IndexOf(num.ToString());
                if (index >0)
                {
                    V_Data.SelectRow(index-1, true);
                    V_Data.SelectRow(index, true);
                    V_Data.FirstDisplayedScrollingRowIndex = index-1;
                }
            }
            
            
            

        }

        void V_Data_E_CellDoubleClicked(object sender, CellClickEventArgs e)
        {
            

        }

        protected override void OnClosing(CancelEventArgs e)
        {

            _checking = false;
            if (_fileStream!=null)
            {
                _fileStream.Close();
            }
            if(_checkThread!=null) _checkThread.Abort();
            if (_loadFileThread != null) _loadFileThread.Abort();
            base.OnClosing(e);
        }


        List<EasyGridTextBoxColumn> columns = new List<EasyGridTextBoxColumn>();
        String _initDir = null;

        public void SetTitles(params String[] titles){

            V_Data.ClearData();
            C_Title.Items.Clear();
            V_Data.AddTitleImageCheckColumn(20, "isCheck");
            V_Data.AddTitleTextBoxColumn(50, "line","line",false);
            for(int i=0; i<titles.Length; i++){
                EasyGridTextBoxColumn col = new EasyGridTextBoxColumn(V_Data.ListView as EasyGridViewParent);
                col.ValueType = typeof(String);
                col.HeaderText = titles[i];
                col.Name = titles[i];
                columns.Add(col);
                C_Title.Items.Add(titles[i]);
                int wid = 30;// (i == 0 || i == 1) ? 100 : 50;
                V_Data.AddTitleTextBoxColumn(wid, titles[i], titles[i], false);
            }
        }

        public void SuspendRefresh()
        {
            this.SuspendLayout();
            V_Data.SuspendLayout();
        }
        public void ResumeRefresh()
        {
            this.ResumeLayout();
            this.V_Data.ResumeLayout();
            
            this.Refresh();
        }

       public void clear()
        {
            this.V_Data.Rows.Clear();
        }

        Thread _checkThread;
        private void B_PatternCheck_Click(object sender, EventArgs e)
        {
            L_Log.Items.Clear();
            _selectedCol = C_Title.SelectedIndex;
            if(_selectedCol<0){
                MessageBox.Show("타이틀을 선택하세요!");
                return;
            }
            iDiff = 1;
            
            if(long.TryParse(T_Diff.Text,out iDiff)==false)
            {
                MessageBox.Show("차이는 숫자로 써 주세요");
                return;
            }
            V_Data.ClearData();
            
            _checkThread = new Thread(new ThreadStart(CheckInteger));
            _checkThread.Start();

        }
        int _selectedCol;
        long iDiff;
        private void CheckInteger()
        {
                    
            byte iVal;
            
            byte iBefore = 0;
            //try
            {
                Stream reader = File.OpenRead(_fileName);

                int lineIndex = 1;
                object[] arr = new object[_cols];
                byte[] unit = new byte[_cols];
                object[] beforeLine = new object[_cols];
                
                _checking = true;
                int read =0;
                if ((read = reader.Read(unit, 0, _cols)) != _cols)
                {
                    return;
                }
                else
                {
                    iBefore = unit[_selectedCol];

                    for (int i = 0; i < _cols; i++)
                    {
                        arr[i] = String.Format("{0:X2}", unit[i]);
                    }
                    beforeLine = arr;
                    //this.V_Data.AddARow(arr);
                }
                
                _checking = true;
                int errorIndex = 0;
                //for (int i = 2; (_checking == true && i < V_Data.Rows.Count); i++)

                while ((read = reader.Read(unit, 0, _cols)) == _cols && _checking == true)
                {
                    arr = new object[_cols];
                    for (int i = 0; i < _cols; i++)
                    {
                        arr[i] = String.Format("{0:X2}", unit[i]);
                    }
                    //this.V_Data.AddARow(arr);


                    //while ((line = reader.ReadLine()) != null && _checking == true)
                    //{
                    lineIndex++;//2부터 시작

                    iVal = unit[_selectedCol];// long.Parse(value);

                    if ((iVal - iBefore) != iDiff)
                    {
                        if (this.InvokeRequired)
                        {
                            addalinedele aal = addALine;
                            this.Invoke(aal, false, lineIndex - 1, beforeLine);
                            this.Invoke(aal, true, lineIndex, arr);
                        }
                        else
                        {
                            addALine(false, lineIndex - 1, beforeLine);
                            addALine(true, lineIndex, arr);
                        }
                        errorIndex++;
                        AddLog("" + errorIndex + "/line:" + (lineIndex + 1) + "(" + (iVal - iBefore) + ")");
                    }
                    L_Line.Text = lineIndex.ToString();
                    iBefore = iVal;

                    beforeLine = arr;
                }
                    
            }
            //catch (Exception ex)
            {
            //    MessageBox.Show(ex.Message);
            }
            //finally
            {
            //    _checkThread = null;
            }
        }

        
        private void AddLog(String log)
        {

            if(this.InvokeRequired){
                func addLogFunc = AddLog;
                this.Invoke(addLogFunc, log);
            }else{
                L_Log.Items.Add(log);
            }
            L_Log.AutoScrollOffset = new Point(0, L_Log.Items.Count - 1);
            
        }

        delegate void func(String log);

        long getTime(String time)
        {
            long h,m,s,ms;
            try
            {
                String[] tokens = time.Split(":.".ToCharArray());
                h = long.Parse(tokens[0]);
                m = long.Parse(tokens[1]);
                s = long.Parse(tokens[2]);
                ms = long.Parse(tokens[3]);
                return h * 360000000 + m * 6000000 + s * 100000 + ms;
            }
            catch
            {
                throw new Exception("시간포멧이 아닙니다.");
            }
        }


        private void B_FileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (_initDir != null && _initDir.Length > 0) dlg.InitialDirectory = _initDir;
            DialogResult result = dlg.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.Abort || result != System.Windows.Forms.DialogResult.Cancel)
            {
                LoadFile(dlg.FileName,true);
            }
        }

        int _nowLine = 0;
        bool _isInit = true;
        int _cols = 16;
        //List<long> _seekPoint = new List<long>();
        public void LoadFile(String fileName, bool isInit, int columns=-1)
        {
            if (columns > 0)
            {
                _cols = columns;
                T_Columns.Text = _cols.ToString();
            }
            _isInit = isInit;

            _fileName = fileName;
            this.Text = fileName.Substring(fileName.LastIndexOf('\\')+1)+"["+fileName.Substring(0,(fileName.LastIndexOf('\\'))+1)+"]";
            if (File.Exists(fileName) == false)
            {
                if(fileName.Length>0) MessageBox.Show(fileName + " 그런 파일이 없습니다.");
                return;
            }
            
            _nowLine = 0;
            //_seekPoint.Clear();
            if(isInit) _totalLines = 0;
            if (_fileStream != null)
            {
                try
                {
                    _fileStream.Close();
                }
                catch { }
            }
            _fileStream = File.OpenRead(fileName);

            if (_isInit)
            {
                V_Data.ClearData();
                V_Data.ClearTitles();

                List<String> titles = new List<string>();
                _cols = Convert.ToInt32(T_Columns.Text);
                if(_cols<=0) _cols = 16; //default 16
                for (int i = 0; i < _cols; i++)
                {
                    titles.Add(string.Format("{0:X2}", i));
                }

                SetTitles(titles.ToArray());
            }
            else
            {
                V_Data.ClearData();
            }
            //_seekPoint.Add(_fileReader.BaseStream.Position); //제목을 제외한 제일 첫 줄.
            if (V_Data.ColumnCount > 0) C_Title.SelectedIndex = 0;

            _loadFileThread = new Thread(new ThreadStart(loadFile));
            _loadFileThread.Start();
        }

        Thread _loadFileThread;
        Stream _fileStream;
        String _fileName;

        void loadFile()
        {//thread에서 라인을 읽어 표에 읽어온다.

            
            long startIndex, numLines;
            if (long.TryParse(T_From.Text, out startIndex) == false)
            {
                MessageBox.Show("from에는 시작할 라인의 index가 들어가야 합니다.");
                return;
            }
            else
            {
                if (startIndex < 0)
                {
                    MessageBox.Show("from index는 음수가 될 수 없습니다.");
                    return;
                }
            }
            
            

            if (long.TryParse(T_NumLines.Text, out numLines) == false)
            {
                MessageBox.Show("lines에는 분석할 라인의 개수가 들어가야 합니다. -1을 넣으면 끝까지 검색합니다.");
                return;
            }
            if (this.InvokeRequired)
            {
                addlinesdele addlinefunc = addLines;
                this.Invoke(addlinefunc, startIndex, numLines);
            }
            else
            {
                addLines(startIndex, numLines);
            }


            //_fileReader.Close();
            //L_Total.Text = V_Data.Rows.Count.ToString();

            _loadFileThread = null;
            

        }
        delegate void addlinesdele(long startIndex, long numLines);

        long _totalLines = 0;
        void addLines(long startIndex, long numLines)
        {
            String line;
            long lineIndex = 0;

            
                _checking = true;
                //long backPoint = 0;
                int backLine = 0;
                int read = 0;
                object[] arr = new object[_cols];
                byte[] unit = new byte[_cols];

                while ((read = _fileStream.Read(unit, 0, _cols)) == _cols && _checking == true)
                {
                    arr = new object[_cols];
                    for (int i = 0; i < _cols; i++)
                    {
                        arr[i] = String.Format("{0:X2}", unit[i]);
                    }
                //while ((line = _fileStream.ReadLine()) != null && _checking)
                //{
                    /*
                    if (_totalLines == 0)
                    {
                        if (_seekPoint[_seekPoint.Count - 1] == _fileReader.BaseStream.Position)
                        {
                            _seekPoint.Add(_seekPoint[_seekPoint.Count - 1] + Buffer.ByteLength(line.ToCharArray()));
                        }
                        else
                        {
                            _seekPoint.Add(_fileReader.BaseStream.Position);
                        }
                    }
                     */
                    
                    if (lineIndex >= startIndex && numLines>0)
                    {
                        addALine(false, lineIndex, arr);


                        if ( (--numLines) == 0 && _isInit) //최초 1회, 끝까지 간다.
                        {
                            //backPoint = _fileReader.BaseStream.Position;
                            backLine = (int)lineIndex;
                        }
                        else if (numLines == 0)
                        {
                            break;
                        }
                        
                        
                    }
                    lineIndex++;
                }

                if (_isInit)//포인터를 되돌려놓기 위함..
                {
                    _isInit = false;
                    _totalLines = lineIndex-1;
                    _fileStream.Seek(0, SeekOrigin.Begin);

                    _nowLine = 0;

                    L_Total.Text = _totalLines.ToString();

                    Track_From.Maximum = (int)_totalLines;
                    Track_From.LargeChange = (int)lineIndex / 10;
                    //MessageBox.Show("파일의 일부를 표시합니다.. from:" + T_From.Text + "  to:" + (_nowLine - startIndex).ToString());
                    
                }
                else
                {
                    _nowLine = (int)lineIndex;
                }
                //T_From.Text = _nowLine.ToString();
            
            

        }
        delegate void addalinedele(bool isChecked, long lineIndex, object[] line);
        void addALine(bool isChecked, long lineIndex, object[] aLine)
        {
            
            /*
            object[] objs = new Object[tokens.Length];
            Array.ConstrainedCopy(tokens, 0, objs, 0, tokens.Length);
            
            addLine(objs);
            * */
            //addLine(tokens);
            List<object> list = new List<object>();
            list.Add(isChecked);
            list.Add(lineIndex.ToString());

            list.AddRange(aLine);
            this.V_Data.AddARow(list);

        }



        bool _checking = false;
        private void B_Stop_Click(object sender, EventArgs e)
        {

            _checking = false;
        }

        private void B_Reload_Click(object sender, EventArgs e)
        {
            if (_nowLine >= _totalLines) LoadFile(_fileName, true);
            else LoadFile(_fileName,false);
        }

        private void Track_From_Scroll(object sender, EventArgs e)
        {
            T_From.Text =  (Track_From.Value ).ToString();
            
        }

        private void B_Prev_Click(object sender, EventArgs e)
        {
            int line = V_Data.FirstDisplayedScrollingRowIndex;
            int toLine = line - V_Data.DisplayedRowCount;
            if (toLine < 0) toLine = 0;
            V_Data.FirstDisplayedScrollingRowIndex = toLine;
            /*
            if (_fileReader != null)
            {
                int lineToBack = (V_Data.DisplayedRowCount>0)? V_Data.DisplayedRowCount:18;
                if (_nowLine == 0) return;
                int start = _nowLine - lineToBack*2;
                V_Data.ClearData();

                if (start < 0) start = 0;
                //_fileReader.BaseStream.Position = _fileReader.BaseStream.Seek(_seekPoint[start], SeekOrigin.Current);
                _nowLine = start;
                T_From.Text = _nowLine.ToString();
                
                T_NumLines.Text = lineToBack.ToString();
                //LoadFile(_fileName);
                _fileReader.BaseStream.Position = 0;
                _fileReader.ReadLine(); //title
                
                String    line;
                    
                int lines = start;
                while ((line = _fileReader.ReadLine()) != null && --lines >= 0)
                {
                }
                
                loadFile();
            }
             */
            
        }

        private void B_Next_Click(object sender, EventArgs e)
        {
            if ((V_Data.FirstDisplayedScrollingRowIndex + V_Data.DisplayedRowCount) < V_Data.Rows.Count)
            {
                V_Data.FirstDisplayedScrollingRowIndex += V_Data.DisplayedRowCount;
            }
            else
            {
                if (_fileStream != null)
                {
                    //int lineToForward = (V_Data.DisplayedRowCount > 0) ? V_Data.DisplayedRowCount : 18;
                    //if ((_nowLine + lineToForward) > _totalLines) lineToForward = (int)_totalLines - _nowLine;
                    //int start = _nowLine + lineToForward;
                    //if (start > _totalLines) start = (int)_totalLines-1;
                    //_fileReader.BaseStream.Position = _fileReader.BaseStream.Seek(_seekPoint[start], SeekOrigin.Current);

                    //_nowLine = start;
                    if (_nowLine > _totalLines) return;
                    else
                    {
                        int lineToForward = V_Data.DisplayedRowCount;
                        T_From.Text = _nowLine.ToString();
                        T_NumLines.Text = lineToForward.ToString();
                        Track_From.Value = _nowLine;

                        //LoadFile(_fileName);
                        byte[] unit = new byte[_cols];
                        object[] arr = new object[_cols];
                        while (lineToForward-- > 0)
                        {
                            int read = 0;
                            if ((read = _fileStream.Read(unit, 0, _cols)) == _cols)
                            {

                                arr = new object[_cols];
                                for (int i = 0; i < _cols; i++)


                                {
                                    arr[i] = String.Format("{0:X2}", unit[i]);
                                }

                                addALine(false, _nowLine++, arr);
                            }
                            else break;

                        }
                        L_Line.Text = (_nowLine - 1).ToString();
                    }
                    //if (_nowLine >= _totalLines) _totalLines = 0;//모두 지나고 나서는 다시 읽어야 한다.
                }
            }
        }



    }
}
