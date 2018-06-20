using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FormAdders;


namespace FormAdders
{
    public partial class NameValuePairList : UserControl
    {
        private int valueX = 30;
        List<Label> names = new List<Label>();
        List<String> nameString = new List<string>();
        List<Label> values = new List<Label>();
        Dictionary<String, Label> valueDic = new Dictionary<string,Label>();
        Boolean isInit = true;
        Timer scrollTimer = null;
        EventHandler ScrolltimerTickEventHandler;
        public void makeInit()
        {
            isInit = false;

            AddRow("testName", "testValue");
            isInit = true;
        }

        public NameValuePairList():base()
        {
            InitializeComponent();

            //ProcessKeyPreview(Message.Create(this.Handle, 
            makeInit();


        }

        public void Scrolls(int scrollNum)
        {
            int initPt = BackPanel.VerticalScroll.Value;
            int pt =  initPt + scrollNum;
            if (BackPanel.VerticalScroll.Minimum > pt) pt = BackPanel.VerticalScroll.Minimum;
            else if (BackPanel.VerticalScroll.Maximum < pt) pt = BackPanel.VerticalScroll.Maximum;
            BackPanel.VerticalScroll.Value = pt;
            if (pt == initPt) BackPanel.Refresh();
        }
        
        void BackPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("wheel");
            scrollTimer.Start();
        }

        void scrollTimer_Tick(object sender, EventArgs e)
        {
            BackPanel.Refresh();
            scrollTimer.Stop();
        }

        void BackPanel_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type != ScrollEventType.ThumbTrack)
            {
                BackPanel.Refresh();
            }
        }

        #region U_IsShowUnit
        Boolean _isShowUnit = false;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Boolean U_IsShowUnit { set { _isShowUnit = true; } get { return _isShowUnit; } }
        #endregion

        #region U_ValuePosition
        public enum ValuePositionMode { HalfPosition, FixedPosition, PercentPosition, AttatchedPosition };

        ValuePositionMode _valuePosition = ValuePositionMode.HalfPosition;
        [SettingsBindable(true)]
        [Browsable(true)]
        public ValuePositionMode U_ValuePosition
        {
            get { return _valuePosition; }
            set { 
                _valuePosition = value;
                readyToRedraw();
                reDrawAll();
            }
        }
        #endregion

        #region U_RowPadding
        int _rowPadding = 5;
        [SettingsBindable(true)]
        [Browsable(true)]
        public int U_RowPadding
        {
            get { return _rowPadding; }
            set 
            { 
                this._rowPadding = value;
                readyToRedraw();
                reDrawAll();
            }
        }
        #endregion

        #region U_InitValuesPair
        String[] initValues;
        [SettingsBindable(true)]
        [Browsable(true)]
        public String[] U_InitValuesPair
        {
            get { return initValues; }
            set
            {
                initValues = value;
                if (value == null) return;
                this.clear();
                String[] pair;
                String strName, strValue; ;
                for (int i = 0; i < value.Length; i++)
                {
                    pair = value[i].Split(",".ToCharArray());
                    strName = pair[0];
                    if (pair.Length > 1) strValue = pair[1];
                    else strValue = "null";
                    AddRow(strName, strValue);
                }
            }
        }
#endregion

        #region U_ForeColors
        Color[] _foreColors = new Color[]{Color.Black, Color.Black};
        [SettingsBindable(true)]
        [Browsable(true)]
        public Color[] U_ForeColors
        {
            get { return _foreColors; }
            set
            {
                this._foreColors = value;
                rePaintColor();
            }
        }
        #endregion

        #region U_FixedValuePosition
        int _fixedValuePosition=100;
        [SettingsBindable(true)]
        [Browsable(true)]
        [Bindable(true)]
        public int U_FixedValuePosition { 
            get { return _fixedValuePosition; } 
            set {
                _fixedValuePosition = value;
                if (_valuePosition == ValuePositionMode.FixedPosition)
                {
                    readyToRedraw();
                    reDrawAll();
                }
            } 
        }
        #endregion

        #region ForeColor
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                _foreColors[0] = value;
                _foreColors[1] = value;
                rePaintColor();
            }
        }
        #endregion

        #region U_BackColor
        Color _backColor = Color.DarkBlue;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Color U_BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                //base.BackColor = value;
                rePaintColor();
            }
        }
        #endregion

        #region U_TextBackColorForTransparent
        Color _textBackColor = Color.DarkBlue;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Color U_TextBackColorForTransparent
        {
            get
            {
                return _textBackColor;
            }
            set
            {
                _textBackColor = value;
                //base.BackColor = value;
                rePaintColor();
            }
        }
        #endregion

        #region U_PercentValuePosition
        float _percentValuePosition=50.0F;
        [SettingsBindable(true)]
        [Browsable(true)]
        public float U_PercentValuePosition { 
            get { return _percentValuePosition; } 
            set { 
                _percentValuePosition = value;
                if (U_ValuePosition == ValuePositionMode.PercentPosition)
                {
                    readyToRedraw();
                    reDrawAll();
                }
            } 
        }
        #endregion

        #region U_IsUnderLine
        Boolean _isUnderLine = false;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Boolean U_IsUnderLine { set { _isUnderLine = true; } get { return _isUnderLine; } }
        #endregion

        #region Count
        public int Count { get { return names.Count; } }
        #endregion

        #region U_UnderLineColor
        Color _underLineColor = Color.Gray;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Color U_UnderLineColor { get { return _underLineColor; } set { _underLineColor = value; } }
        #endregion

        #region U_FixedColWidth
        int _fixedColWidth = 250;
        [SettingsBindable(true)]
        [Browsable(true)]
        public int U_FixedColWidth { get { return _fixedColWidth; } set { _fixedColWidth = value; } }
        #endregion

        #region U_IsColWidthFixed
        Boolean _isColWidthFixed = false;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Boolean U_IsColWidthFixed { get { return _isColWidthFixed; }
            set {
                _isColWidthFixed = value;
                readyToRedraw();
                reDrawAll();
            } 
        }
        #endregion
        
        #region U_IsScrollEnabled
        public Boolean _isScrollEnabled = false;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Boolean U_IsScrollEnabled { get { return _isScrollEnabled; } 
            set {
                _isScrollEnabled = value;
                readyToRedraw();
                reDrawAll();
            } 
        }
        #endregion
        
        #region U_IsAutoRedraw
        public Boolean _isAutoRedraw = false;
        [SettingsBindable(true)]
        [Browsable(true)]
        public Boolean U_IsAutoRedraw { get { return _isAutoRedraw; } set { _isAutoRedraw = value; } }
        #endregion

        public Label this[int index]
        {
            get { try { return values[index]; } catch { return null; } }
        }

        public Label this[String name]
        {
            get { try { return valueDic[name]; } catch { return null; } }
        }

        public void setValueArray(int start, Array data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                values[i+start].Text = data.GetValue(i).ToString();
            }
        }

        public void setValueArray(int start, List<Object> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                values[i+start].Text = data[i].ToString();
            }
        }

        public Label getValueLabel(String name)
        {
            return valueDic[name];
        }

        public List<Label> getValueLabelList()
        {
            return values;
        }

        public Label makeLabel(int x, int y, String Text, String name = "noName")
        {
            Label label = new Label();
            label.AutoSize = true;
            //label.BackColor = this._backColor ;// .FromArgb(0x051b2d);// System.Drawing.Color.Transparent;
            label.Location = new System.Drawing.Point(x, y);
            label.Name = name;
            label.Text = Text;
            
            return label;
        }


        public void clear()
        {
            values.Clear();
            names.Clear();
            valueDic.Clear();
            nameString.Clear();
            BackPanel.Controls.Clear();
        }

        #region AddRow
        int _numOfCols = 1;
        Boolean _isNeedReposition = false;
        int _initWidth=100;
        Boolean _isNeedScroll = false;

        public void AddRow(String name, String value="", String Unit="")
        {
            
            int y = 0;
            int x = 0;
            setValueX();
            if (isInit == true)
            {//디자이너에서 보여주기 위해 초기에 라벨이 추가되어 있다. 
             //하지만 사용자가 addRow를 호출하는 순간 기존의 것은 없어져야 한다.
                isInit = false;
                this.clear();
                isInit = false;
            }
            
            int count = values.Count;
            if (count > 0)
            {//이전에 addRow를 호출하여 추가된 것이 있다면 그 추가된 것의 위치를 차용해 온다.
                x = names[count - 1].Location.X;
                y = values[count - 1].Location.Y;
                y += values[count - 1].Height;
                y += _rowPadding;
            }
            if (y+30 > this.Height)
            {//만일 초기 높이를 넘어갔다면
                if (_isScrollEnabled == false)
                {//스크롤 불가시 옆(오른쪽)으로 확장해 주어야 한다.
                    if (_isColWidthFixed) //한 열의 크기가 고정되어 있다면
                    {//고정된 만큼만 우측으로 이동하여 그려준다.
                        x += _fixedColWidth;
                        y = 0;
                        _numOfCols++;
                    }
                    else //고정이 아니라면, 비율에 따라서 이동하여야 할 것이다.
                    {//여기서 비율은 실시간으로 계산되어, col의 개수로 나뉜다.
                        _numOfCols++;
                        _isNeedReposition = true;
                        if (_isAutoRedraw) reDrawAll(); //2개 이상의 col이라면 이전에 그린 줄의 위치를 재설정 해야 한다.
                        //그러나 autoRedraw 설정이 되어 있어야만 그리고, 그렇지 않다면 사용자가 직접 reDrawAll명령을 
                        //호출해야 할 것이다.

                        x = (int)((_initWidth) * ((double)(_numOfCols - 1) / (double)_numOfCols));
                        y = 0;
                    }
                }
                else
                {//스크롤 가능 시에는 스크롤 바의 움직임에 따라 백그라운드를 리프레시 해 주는 루틴을 넣는다.
                    if (_isNeedScroll == false) //두 번 루틴을 넣는 것을 방지하기 위해 _isNeedScroll변수를 셋팅한다.
                    {
                        _isNeedScroll = true;
                        //scroll이 필요할 때 정해 줄 것
                        BackPanel.Scroll += new ScrollEventHandler(BackPanel_Scroll);
                        ScrolltimerTickEventHandler = new EventHandler(scrollTimer_Tick);
                        BackPanel.MouseWheel += new MouseEventHandler(BackPanel_MouseWheel);
                        scrollTimer = new Timer();
                        scrollTimer.Interval = 1000;
                        scrollTimer.Tick += ScrolltimerTickEventHandler;
                    }
                }


            }
            
            setValueX();
            Label nameLabel = makeLabel(x, y, name);
           
            int unitWidth = 0;
            
            if (_isShowUnit)
            {

            }
            int valuePos = (valueX > (nameLabel.Width + unitWidth)) ? valueX : nameLabel.Width + unitWidth;
            Label valueLabel = makeLabel(x+valuePos, y, value, name);
            valueLabel.MouseHover += new EventHandler(valueLabel_MouseHover);
            valueLabel.MouseLeave += new EventHandler(valueLabel_MouseLeave);
            nameLabel.ForeColor = _foreColors[0];
            valueLabel.ForeColor = _foreColors[1];
            nameLabel.BackColor = _backColor;
            valueLabel.BackColor = _textBackColor;


            names.Add(nameLabel);
            nameString.Add(name);
            values.Add(valueLabel);
            valueDic.Add(name, valueLabel);
            
            //BackPanel.SuspendLayout();
            BackPanel.Controls.Add(nameLabel);
            BackPanel.Controls.Add(valueLabel);
            if (_isUnderLine)
            {
                UserControl underLine = new UserControl();
                underLine.Location = new Point(0, y + 12);
                underLine.Width = (_isScrollEnabled)? _initWidth-20:_initWidth;//스크롤바가 생겼을 때를 대비
                underLine.Height = 1;
                underLine.BackColor = _underLineColor;
                BackPanel.Controls.Add(underLine);
                /*
                Graphics g = BackPanel.CreateGraphics();
                
                Pen pen = new Pen(_underLineBrushes[(int)_underLineBrush]);

                g.DrawLine(pen, new Point(0, y), new Point(BackPanel.Width, y));
                g.Flush();
                 */
            }
            //BackPanel.ResumeLayout();

        }
        #endregion

        void valueLabel_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this.BackPanel);
        }

        void valueLabel_MouseHover(object sender, EventArgs e)
        {
            Label label = (Label)sender;

            toolTip.Show(label.Text, this.BackPanel,label.Location.X+label.Width/2, label.Location.Y+label.Height+5); 
        }
        public void readyToRedraw()
        {
            if(_isAutoRedraw)
                _isNeedReposition = true;
        }

        public void rePaintColor()
        {
            for (int i = 0; i < names.Count; i++)
            {
                //Color backColor;
                if (_backColor == Color.Transparent)
                {
                    //backColor = ControlBitmap.GetPixel(values[i].Location.X, values[i].Location.Y);
                    names[i].BackColor = _backColor;
                    values[i].BackColor = _textBackColor;
                }
                else
                {
                    names[i].BackColor = _backColor;
                    values[i].BackColor = _backColor;
                }

                names[i].ForeColor = _foreColors[0];

                values[i].ForeColor = _foreColors[1];


            }
        }

        public void reDrawAll()
        {
            if (_isNeedReposition)
            {
                setValueX();
                int y = 0; int x = 0;
                int col = 1;
                int valuePos;
                //BackPanel.SuspendLayout();
                for (int i = 0; i < names.Count; i++)
                {

                    if (y + 30 > this.Height && _isScrollEnabled == false)
                    {

                        if (_isColWidthFixed)
                        {
                            x += _fixedColWidth;
                            y = 0;
                        }
                        else
                        {
                            x = (int)(_initWidth * ((double)(col) / (double)_numOfCols));
                            y = 0;
                            col++;
                        }
                        names[i].Location = new Point(x, y);
                        valuePos = (names[i].Width > valueX) ? names[i].Width : valueX;
                        values[i].Location = new Point(x + valuePos, y);
                        y += values[i].Height;
                        y += _rowPadding;
                        continue;
                    }
                    else
                    {
                        names[i].Location = new Point(x, y);
                        valuePos = (names[i].Width > valueX) ? names[i].Width : valueX;
                        values[i].Location = new Point(x + valuePos, y);

                        y = values[i].Location.Y;
                        y += values[i].Height;
                        y += _rowPadding;
                    }
                }
                //BackPanel.ResumeLayout();
                _isNeedReposition = false;
            }
        }


        void setValueX()
        {
            switch (this.U_ValuePosition)
            {
                case ValuePositionMode.FixedPosition:
                    valueX = this.U_FixedValuePosition;
                    break;
                case ValuePositionMode.HalfPosition:
                    valueX = _initWidth / _numOfCols/2;
                    
                    break;
                case ValuePositionMode.PercentPosition:
                    valueX = (int)((double)this.Width/(double)_numOfCols * (U_PercentValuePosition / 100.0));
                    break;
                case ValuePositionMode.AttatchedPosition:
                    valueX = -1;
                    break;

            }
        }


        protected override void OnResize(EventArgs e)
        {
            _initWidth = this.Width;
            readyToRedraw();
            reDrawAll();

            //base.OnResize(e);
        }

    }
}
