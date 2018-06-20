using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;

namespace GraphModules
{

    public partial class MultiLineChartWin : UserControl
    {
        public event MouseEventHandler E_MouseDown;
        public event MouseEventHandler E_MouseUp;
        public event MouseEventHandler E_MouseMove;
        public event MouseEventHandler E_OverXMax;
        public event MouseEventHandler E_OverXMin;
        public event GraphClickEventHandler E_HotSpotClicked;
        public event GraphClickEventHandler E_HotSpotMovedByKey;
        public event EventHandler E_Click;
        public event EventHandler E_DoubleClick;
        public event EventHandler E_CloseClicked;
        public event EventHandler E_CheckStateChanged;
        
        public event SnapShotEventHandler E_SnapShotClicked;

        Bitmap _imageBuffer;


        bool _isEnabled = true;
        public new bool Enabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public MultiLineChartWin()
        {
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            init(_subsets, _bufferSize);
        }

        void dispose()
        {
            if (_savingFile != null)
            {
                try
                {
                    _savingFile.Close();
                }
                catch { }
            }
        }

        public MultiLineChartWin(int subsets, int bufferSize = 600)
        {
            
            _subsets = subsets;
            _bufferSize = bufferSize;

            
        }

        void init(int subsets, int bufferSize = 600, ICollection<String> subsetTitles = null)
        {
            InitializeComponent();
            _lazyRefresh = true;
            if (subsetTitles != null) U_SubsetTitles = subsetTitles.ToArray();

            U_Subsets = subsets;
            U_BufferSize = bufferSize;
            
            ClearBuffer();
            
            //U_RefreshCount = refreshCount;

            U_LineColors = _lineColors;
            U_LineStyles = _lineThickStyle;

            U_BackStyle = Style.BlueBackWhiteLine;

            GraphArea.MouseDown += new MouseEventHandler(G_Science_MouseDown);
            GraphArea.MouseUp += new MouseEventHandler(G_Science_MouseUp);
            GraphArea.MouseMove += new MouseEventHandler(G_Science_MouseMove);
            GraphArea.Click+=new EventHandler(G_Science_Click);
            GraphArea.DoubleClick += new EventHandler(G_Science_DoubleClick);
            //G_Science.PreviewKeyDown += new PreviewKeyDownEventHandler(G_Science_PreviewKeyDown);
            
            //G_Science.PeGraphHotSpot += new Pesgo.GraphHotSpotEventHandler(G_Science_PeGraphHotSpot);
            B_SnapShot.Click +=new EventHandler(B_SnapShot_Click);
            B_Close.Click +=new EventHandler(B_Close_Click);

            C_GraphName.CheckStateChanged += new EventHandler(C_GraphName_CheckStateChanged);
            _lazyRefresh = false;

            DrawGrid();
            Refresh();
        }

        

        int _cursorPoint = 0;
        public int CursorPoint
        {
            get { return _cursorPoint; }
            set { _cursorPoint = value; }
        }

        int _cursorSubset = 0;
        public int CursorSubset
        {
            get { return _cursorSubset; }
            set { _cursorSubset = value; }
        }

        public void SelectPoint(int subset, int index)
        {
            CursorSubset = subset;
            CursorPoint = index;
            
            if(_lazyRefresh==false)  GraphArea.Refresh();
        }

        void G_Science_Click(object sender, EventArgs e)
        {
            if (E_Click != null) E_Click(this, e);
        }
        void G_Science_DoubleClick(object sender, EventArgs e)
        {
            if (E_DoubleClick != null) E_DoubleClick(this, e);
        }

        void G_Science_MouseMove(object sender, MouseEventArgs e)
        {
            if (E_MouseMove != null) E_MouseMove(this, e);
        }

        void C_GraphName_CheckStateChanged(object sender, EventArgs e)
        {
            if (E_CheckStateChanged != null) E_CheckStateChanged(this, e);
        }

        [Browsable(true)]
        public String Title
        {
            get
            {
                return C_GraphName.Text;
            }
            set
            {
                C_GraphName.Text = value;
            }
        }

        Boolean _titleBarVisible = true;
        [Browsable(true)]
        public Boolean U_TitleBarVisible
        {
            get { return _titleBarVisible; }
            set
            {
                _titleBarVisible = value;
                P_TitleBar.Visible = value;
                if (value)
                {
                    T_TopDown.RowStyles[0].Height = 20;
                }
                else
                {
                    T_TopDown.RowStyles[0].Height = 0;
                }
            }
        }
        public Boolean U_IsSaving
        {
            get { return _savingFile != null; }
        }
        public Boolean U_IsPausing
        {
            get { return _pausing; }
        }

        public override string Text
        {
            get
            {
                return C_GraphName.Text;
            }
            set
            {
                C_GraphName.Text = value;

            }
        }

        public Rectangle U_GraphRectAxis
        {
            get
            {
                return GraphArea.ClientRectangle;
            }
        }

        public Rectangle U_GraphRectGraph
        {
            get
            {
                return GraphArea.ClientRectangle;
            }
        }


        [Browsable(true)]
        public Boolean U_Checked
        {
            get { return C_GraphName.Checked; }
            set { C_GraphName.Checked = value; }
        }

        double ConvRateXPixcelToGraph()
        {
            if (_dataSize < 2) return 1;

            double minX = _graphXValue[_showStart + _offsetX];
            double maxX;
            if (_dataSize > _pointsOfGraph)
            {
                int lastIndex = _pointsOfGraph;
                maxX = _graphXValue[lastIndex + _offsetX];
            }
            else
            {
                double unit = _graphXValue[1] - _graphXValue[0];
                maxX = unit * _pointsOfGraph;
            }

            return (maxX - minX) / GraphArea.Width;
        }

        double ConvRateYPixcelToGraph()
        {
            if (_dataSize < 2) return 1;

            double max = U_ManualMaxY;
            double min = U_ManualMinY;

            return (max - min) / GraphArea.Height;
        }

        bool ConvPixelToGraph(int x, int y, ref double gX, ref double gY)
        {
            if (_dataSize< 2) return false;
            
            double rateX = ConvRateXPixcelToGraph();
            double rateY = ConvRateYPixcelToGraph();
            gX = x * rateX;
            gY = y * rateY;
            
            return true;
        }

        double ConvRateXGraphToPixel()
        {
            if (_dataSize < 2) return 1;

            double minX = _graphXValue[_showStart + _offsetX];
            double maxX;
            if (_dataSize > _pointsOfGraph)
            {
                int lastIndex = _pointsOfGraph;
                maxX = _graphXValue[lastIndex + _offsetX];
            }
            else
            {
                double unit = _graphXValue[1] - _graphXValue[0];
                maxX = unit * _pointsOfGraph;
            }

            return GraphArea.Width / (maxX - minX);
        }

        double ConvRateYGraphToPixel()
        {
            if (_dataSize < 2) return 1;

            double max = _maxY;
            double min = _minY;
            
            return GraphArea.Height / (max - min);
        }

        bool ConvGraphToPixel(double gX, double gY, ref int x, ref int y)
        {
            if (_dataSize < 2) return false;
         
            double rateX = ConvRateXGraphToPixel();
            double rateY = ConvRateYGraphToPixel();

            x = (int)(gX * rateX);
            y = (int)(gY * rateY);

            return true;
        }

        int _overOffsetSize = 0;
        
        void G_Science_MouseUp(object sender, MouseEventArgs e)
        {
            if (_offsetOverStatus == OffsetOverStatus.MinOver && E_OverXMin != null)
            {
                E_OverXMin(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, _overOffsetSize, 0, 0));
                if (E_MouseUp != null) E_MouseUp(this, e);
                _offsetOverStatus = OffsetOverStatus.NotOver;//MOuseUp할때마다 초기화
                return;
            }
            else if (_offsetOverStatus == OffsetOverStatus.MaxOver && E_OverXMax != null)
            {
                E_OverXMax(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, _overOffsetSize, 0, 0));
                _offsetOverStatus = OffsetOverStatus.NotOver;//MOuseUp할때마다 초기화
                if (E_MouseUp != null) E_MouseUp(this, e);
                return;
            }
            _offsetOverStatus = OffsetOverStatus.NotOver;//MOuseUp할때마다 초기화

            if (E_MouseUp != null) E_MouseUp(this, e);

            // get last mouse location within control //
            Int32 pX, pY, nA, nX, nY;
            Double fX = 0, fY = 0;
            System.Drawing.Point pt;

            // get last mouse location within control //'
            pt = GraphArea.PointToClient(Control.MousePosition);
            pX = pt.X;
            pY = pt.Y;

            nA = 0;      // Initialize axis, non-zero only if using MultiAxesSubsets
            nX = pX;     // Initialize nX and nY with mouse location
            nY = pY;
            ConvPixelToGraph(nX, nY, ref fX, ref fY);

            // We now know data coordinates for mouse location //

            // Use fX to interpolate each subset to find approximate Y value //
            int index = 0;
            for (int i = 0; i < _pointsOfGraph; i++)
            {
                index = i + _showStart + _offsetX;
                double x = _graphXValue[index];
                if (x >= fX) break;
            }
            int x1 = index;
            /*
            //GigsStructs.GraphLoc gl = G_Science.PeFunction.GetGraphLoc();
            int axis=0;
            int pixelY = e.Y;
            int pixelX = e.X;
            double graphX = 0;
            double graphY = 0;
            G_Science.PeFunction.ConvPixelToGraph(ref axis, ref pixelX, ref pixelY, ref graphX, ref graphY, false, true, false);
            Rectangle rect1=G_Science.PeFunction.GetRectAxis();
            
            int posX = pixelX - rect1.X;
            if (posX < 0) posX = 0;
            if (posX > rect1.Width) posX = rect1.Width;
             */
            /*
            int posY = pixelY - rect1.Y;
            if (posY < 0) posY = 0;
            if (posY > rect1.Height) posY = rect1.Height;
            */
            /*
            double xRate = (double)U_PointsOfGraph / (double)rect1.Width;
            
            int clickXIndex = (int)(posX*xRate);
            */
            if (E_MouseUp != null) E_MouseUp(this, e);
            if (E_HotSpotClicked != null) E_HotSpotClicked(this, new GraphClickEventArgs((int)x1, e.Clicks > 1, fX, fY));
        }

        void G_Science_MouseDown(object sender, MouseEventArgs e)
        { 
            if (E_MouseDown != null) E_MouseDown(this, e);
        }

        

        private Style _style = Style.BlueBackWhiteLine;
        public enum Style { BlueBackWhiteLine }

        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Style U_BackStyle{ get{ return _style;} 
            set{
                _style = value;
                switch(_style){
                    case Style.BlueBackWhiteLine:
                    default:
                        SetBlueBackWhiteLine();
                        break;
                }
            }
        }

        void SetBlueBackWhiteLine()
        {
            
            this.BackColor = Color.FromArgb(0, 0, 40); //그래프의 뒷면 색깔.
            DrawGrid();
        }

        void DrawGrid()
        {
            DrawXGrid();
            DrawYGrid();
            
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawGrid();
            Refresh();
        }
        double getXRange()
        {
            double minX = _graphXValue[_showStart + _offsetX];
            double maxX;
            if (_dataSize == 0)
            {
                return GraphArea.Width;
            }
            if (_dataSize > _pointsOfGraph)
            {
                int lastIndex = _pointsOfGraph;
                maxX = _graphXValue[lastIndex + _offsetX];
            }
            else
            {
                double unit = _graphXValue[1] - _graphXValue[0];
                maxX = unit * _pointsOfGraph;
            }
            if ((maxX - minX) == 0) return 1;
            else return maxX - minX;
        }

        void DrawXGrid()
        {
            double xRange = getXRange();

            double toPixel = ConvRateYGraphToPixel();
            int dev = 10;
            int unitSize = (int)(toPixel * xRange / dev);

            if (unitSize >= 30 && unitSize <= 40)
            {
            }
            else if (unitSize < 30)
            {
                int mult = 2; //2/10, 3/10, ...
                while ((unitSize = ((int)(toPixel * xRange * mult / dev))) < 30)
                {
                    mult++;
                }
            }
            else //initSize>40
            {
                int mult = 2; //1/20, 1/30,...
                while ((unitSize = ((int)(toPixel * xRange / (dev * mult)))) < 30)
                {
                    mult++;
                }
            }
            Graphics g = GraphArea.CreateGraphics();
            int xPos = unitSize;
            Pen pen = new Pen(Brushes.White, 1.0f);
            while (xPos < GraphArea.Width)
            {
                g.DrawLine(pen, new Point(xPos,0), new Point(xPos, GraphArea.Height));
                xPos += unitSize;
            }
        }
        void DrawYGrid()
        {
            double yRange = _maxY - _minY;
            
            double toPixel = ConvRateYGraphToPixel();
            int dev = 10;
            int unitSize = (int)(toPixel * yRange / dev);
            
            if (unitSize >= 30 && unitSize <= 40)
            {
            }
            else if (unitSize < 30)
            {
                int mult = 2; //2/10, 3/10, ...
                while ((unitSize = ((int)(toPixel * yRange * mult / dev))) < 30)
                {
                    mult++;
                }
            }
            else //initSize>40
            {
                int mult = 2; //1/20, 1/30,...
                while ((unitSize = ((int)(toPixel * yRange / (dev * mult)))) < 30)
                {
                    mult++;
                }
            }
            Graphics g = GraphArea.CreateGraphics();
            int yPos = unitSize;
            Pen pen = new Pen(Brushes.White, 1.0f);
            while (yPos < GraphArea.Height)
            {
                g.DrawLine(pen, new Point(0, yPos), new Point(GraphArea.Width, yPos));
                yPos += unitSize;
            }
        }

        int _selectedSubset=0;
        public int U_SelectedSubset
        {
            get
            {
                return CursorSubset;
            }
            set
            {
                _selectedSubset = value;
                Refresh();
            }
        }

        public String U_SelectedSubsetTitle
        {
            get
            {
                if (U_SelectedSubset > 0) return U_SubsetTitles[U_SelectedSubset];
                else return "";
            }
        }

        public double[] U_GraphXValue
        {
            get { return _graphXValue; }
        }
        
        public double U_GraphYValue(int subset, int index)
        {
            return _graphData[subset, index];
        }

        public double U_SelectedX{
            get
            {
                try
                {
                    return _graphXValue[(CursorPoint + _showStart + _offsetX) % _bufferSize];
                }
                catch { throw; }
            }
        }

        public enum CursorPositions { Left = 0, Right, UserDefined };
        CursorPositions _cursorPos = CursorPositions.UserDefined;
        public CursorPositions U_CursorPosition
        {
            get { return _cursorPos; }
            set
            {
                _cursorPos = value;
            }
        }

        public double U_SelectedY
        {
            get
            {
                try
                {
                    return _graphData[CursorSubset,CursorPoint];
                }
                catch { throw; }
            }
        }


        /*
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        */
        double[,] _graphData = new double[4,1];//1 is just for initial value
        double[] _graphXValue = new double[1];
        int _dataSize = 0;
        int _showStart = 0;
        int _subsets = 1;
        [DefaultValue(1)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_Subsets
        {
            get { return _subsets; }
            set
            {
                if (_subsets != value)
                {
                    _subsets = value;
                }
                _graphData = new double[_subsets, _bufferSize];// new List<float>[_subsets];
                _dataSize = 0;//초기화
                _showStart = 0;
                _offsetX = 0;
                _writePoint = 0;
                
                List<Color> lineColors = _lineColors.ToList();

                for (int i = 0; i < _subsets; i++)
                {
                    if (i >= 22) lineColors.Add(_lineColors[i % 22]);
                }
                if (_lineColors.Length < lineColors.Count) _lineColors = lineColors.ToArray();
                if (_lazyRefresh == false) RefreshNow();
            }

        }

        Boolean _lazyRefresh = false;
        public Boolean U_LazyRefresh
        {
            set { _lazyRefresh = value; }
            get { return _lazyRefresh; }
        }

        int _offsetX = 0;
        public int U_OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        double _minY=0;
        [SettingsBindable(true)]
        [Browsable(true)]
        public double U_ManualMinY
        {
            set
            {
                _minY = value;
                if(_lazyRefresh==false) RefreshNow();
            }
            get
            {
                return _minY;
            }
        }

        double _maxY=1;
        [SettingsBindable(true)]
        [Browsable(true)]
        public double U_ManualMaxY
        {
            set
            {
                _maxY = value;
                if (_lazyRefresh == false) RefreshNow();
            }
            get
            {
                return _maxY;
            }
        }


        GraphDimension _graphDimension = GraphDimension.One;
        public enum GraphDimension{One=1,Two=2};
        [Bindable(true)]
        [SettingsBindable(true)]
        [Browsable(true)]
        public GraphDimension U_GraphDimension
        {
            get { return _graphDimension; }
            set { _graphDimension = value; }
        }

        public enum XAxisMode { SecondMode, IndexMode }
        XAxisMode _xAxisMode = XAxisMode.IndexMode;
        [Bindable(true)]
        [SettingsBindable(true)]
        [Browsable(true)]
        public XAxisMode U_X_AxisMode
        {
            get { return _xAxisMode; }
            set
            {
                _xAxisMode = value;
            }
        }


        int _pointsOfGraph = 600;
        [Bindable(true)]
        [SettingsBindable(true)]
        [Browsable(true)]
        public int U_PointsOfGraph
        {
            get { return _pointsOfGraph; }
            set
            {
                _pointsOfGraph = value;
            }
        }
        
        int _bufferSize = 20000;
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_BufferSize
        {
            get { return _bufferSize; }
            set
            {
                _bufferSize = value;

                _graphData = new double[_subsets, _bufferSize];//[i] = new List<float>(value+1);
                _graphXValue = new double[_bufferSize];
                
                _dataSize = 0;//초기화
                _showStart = 0;
                _offsetX = 0;
                _writePoint = 0;
            }
        }
        #region lineColors
        Color[] _lineColors = new Color[]{
            Color.Red,
            Color.Cyan,
            Color.Green,
            Color.Yellow,
            Color.YellowGreen,
            Color.White,
            Color.Gray,
            Color.Violet,
            Color.Tan,
            Color.ForestGreen,
            Color.Brown,
            Color.BlanchedAlmond,
            Color.DarkCyan,
            Color.Orange,
            Color.PaleVioletRed,
            Color.BlueViolet,
            Color.Fuchsia,
            Color.LightYellow,
            Color.OrangeRed,
            Color.FloralWhite,
            Color.DarkSeaGreen,
            Color.MintCream,//다음부터 반복
        };
        #endregion

        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Color[] U_LineColors
        {
            get { return _lineColors; }
            set
            {
                _lineColors = value;
                if (_lazyRefresh == false) RefreshNow();
            }
        }
        public enum LineType
        {
            ThinSolid = 0,
            Dash = 1,
            Dot = 2,
            DashDot = 3,
            DashDotDot = 4,
            MediumSolid = 5,
            ThickSolid = 6,
            MediumThinSolid = 9,
            MediumThickSolid = 10,
            ExtraThickSolid = 11,
            ExtraThinSolid = 12,
            MediumThinDash = 16,
            MediumThinDot = 17,
            MediumThinDashDot = 18,
            MediumThinDashDotDot = 19,
            MediumDash = 20,
            MediumDot = 21,
            MediumDashDot = 22,
            MediumDashDotDot = 23,
            MediumThickDash = 24,
            MediumThickDot = 25,
            MediumThickDashDot = 26,
            MediumThickDashDotDot = 27,
            ThickDash = 28,
            ThickDot = 29,
            ThickDashDot = 30,
            ThickDashDotDot = 31,
            ExtraThickDash = 32,
            ExtraThickDot = 33,
            ExtraThickDashDot = 34,
            ExtraThickDashDotDot = 35,
        }
        #region lineStyles
        LineType[] _lineThickStyle = new LineType[] {
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.ThinSolid,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dot,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.Dash,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
            LineType.DashDotDot,
        };
        #endregion

        [BrowsableAttribute(true)]
        [Bindable(true)]
        public LineType[] U_LineStyles
        {
            get { return _lineThickStyle; }
            set
            {
                
                _lineThickStyle = value;
                LineType[] arr = new LineType[_lineThickStyle.Length];
                for (int i = 0; i < _lineThickStyle.Length; i++)
                {
                    arr[i] = (LineType)_lineThickStyle[i];
                }
                
                if (_lazyRefresh == false) RefreshNow();
            }
        }



        String[] _axisTitle = new String[]{ "X_Axis", "Y_Axis" };
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public String[] U_AxisTitles
        {
            get { return _axisTitle; }
            set
            {
                _axisTitle = value;

                if (_lazyRefresh == false) RefreshNow();
            }
        }                        
        
        String[] _subSetTitles = new String[] {"1","2","3","4"};
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public String[] U_SubsetTitles
        {
            get { return _subSetTitles; }
            set
            {
                _subSetTitles = value;
                if(U_Subsets != _subSetTitles.Length) U_Subsets = _subSetTitles.Length;

                List<Color> lineColors = _lineColors.ToList();
                if (value.Length >= 22)
                {
                    for (int i = 22; i < value.Count(); i++)
                    {
                        try
                        {
                            lineColors.Add(lineColors[i % 22]);
                            //G_Science.PeLegend.AnnotationText[i] = value[i];
                        }
                        catch
                        {
                            Console.WriteLine("graph legend" + i + " has not handled...");
                        }
                    }
                }
                if(_lineColors.Length< lineColors.Count) _lineColors = lineColors.ToArray();
                
                if (_lazyRefresh == false) RefreshNow();
            }
        }                        

        int _xMargin = 100;
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_RefreshCount
        {
            get { return _xMargin; }
            set
            {
                if (value < 1) throw new Exception("RefreshCount는 1보다 작을 수 없습니다");
                _xMargin = value;
            }
        }

        [BrowsableAttribute(true)]
        [Bindable(true)]
        public enum GridLine {Both=0,None,XAxis,YAxis};
        GridLine _gridLineStyle = GridLine.YAxis;
        public GridLine U_GridLine
        {
            get { return _gridLineStyle; }
            set
            {
                _gridLineStyle = value;
                if (_lazyRefresh == false) RefreshNow();
            }
        }

        public enum ManualScaleControl
        {
            None = 0,
            Min = 1,
            Max = 2,
            MinMax = 3,
        }
        [BrowsableAttribute(true)]
        [Bindable(true)]
        ManualScaleControl _manualScaleControl = ManualScaleControl.None;
        public ManualScaleControl U_ManualScaleY
        {
            get {
                return _manualScaleControl;
            //    return (ManualScaleControl)G_Science.PeGrid.Configure.ManualScaleControlY; 
            }
            set
            {
                _manualScaleControl = value;
                
                if (value == ManualScaleControl.None)
                {
                    U_AutoMaxYIndex = -1;
                    U_AutoMinYIndex = -1;
                }
                
                if (_lazyRefresh==false) RefreshNow();
            }
        }

        public enum GraphLineMethods { Line, Step, Curve, Point, PointAndLine, PointAndCurve };
        GraphLineMethods _graphMethod = GraphLineMethods.Line;
        public GraphLineMethods U_GraphLineMethod
        {
            get
            {
                return _graphMethod;
            }
            set
            {
                _graphMethod = value;
             }
        }
        public Dictionary<int, GraphLineMethods> SubsetLineTypes = new Dictionary<int, GraphLineMethods>();

        int _autoMinYIndex = -1;
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_AutoMinYIndex
        {
            get { return _autoMinYIndex; }
            set
            {
                _autoMinYIndex = value;
            }
        }

        int _autoMaxYIndex = -1;
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_AutoMaxYIndex
        {
            get { return _autoMaxYIndex; }
            set
            {
                _autoMaxYIndex = value;
            }
        }

        float _yMinMaxMarginRate = 0.1f;
        public float U_Y_MinMaxMarginRate
        {
            get { return _yMinMaxMarginRate; }
            set { _yMinMaxMarginRate = value; }
        }

        GraphShowMode _graphShowMode = GraphShowMode.SampledGraph;
        public enum GraphShowMode { SampledGraph = 0, ShowLastData }
        
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public GraphShowMode U_GraphShowMode
        {
            get
            {
                return _graphShowMode;
            }
            set
            {
                _graphShowMode = value;
            }
        }

        bool _isShowMarkingPoints = true;
        public Boolean U_ShowMarkingPoints
        {
            get
            {
                return _isShowMarkingPoints;
            }
            set
            {
                _isShowMarkingPoints = value;
            }
        }

        Dictionary<string, List<object>> _relatedData = new Dictionary<string, List<object>>();
        /// <summary>
        /// You must call it before setData or set2DData...
        /// </summary>
        /// <param name="relatedData">relatedData is saving on the same line with data for setData that is called after it.</param>
        public void setRelatedData(String name, object relatedData)
        {
            if (_pausing) return;
            if (_relatedData.ContainsKey(name) == false) _relatedData[name] = new List<object>();
            _relatedData[name].Add(relatedData);
        }

        public List<Object> getRelativeData(String name)
        {
            try
            {
                return _relatedData[name];
            }
            catch { return null; }
        }

        String _relatedDataName = "data";
        /// <summary>
        /// RelatedData's name. it needs for file saving.
        /// </summary>
        public String U_RelatedDataName
        {
            get { return _relatedDataName; }
            set { _relatedDataName = value; }
        }

        public Object U_SelectedRelatedData(String name)
        {
            
            if (_relatedData[name].Count > 0)
            {
                try
                {
                    return _relatedData[name][CursorPoint + _showStart + _offsetX];
                }
                catch { return null; }
            }
            else return "";
            
        }

        public List<Object> U_RelatedData(String name)
        {
            try
            {
                return _relatedData[name];
            }
            catch { return null; }
        }

        public void pause()
        {
            _pausing = true;
        }
        public void resume()
        {
            _pausing = false;
        }
        
        public void setData(String name, object relatedData, double x, params double[] y)
        {
            if (_pausing) return;

            setRelatedData(name, relatedData);
            setData(x, y);
        }


        bool _pausing = false;

        Boolean _writingData = false;
        int _writePoint = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">U_X_AxisMode가 SecondMode일때는 new DateTime(year, month, day, hour, min, sec, ms).ToOADate();
        ///  IndexMode일때는 float tick을 넘겨주면 된다.
        /// </param>
        /// <param name="y">각 subset에 해당하는 값</param>
        public void setData(double x, params double[] y)
        {
            if (_pausing) return;

            _writingData = true;

            for (int subsetIndex = 0; subsetIndex < _subsets; subsetIndex++)
            {
                _graphData[subsetIndex, _writePoint] = (float)y[subsetIndex];
            }
            _graphXValue[_writePoint] = x;

            _writingData = false;
            if (_lazyRefresh == false)
            {
                if (_dataSize > 2) RefreshNow();// DrawGraphFor1D();

            }
            _writePoint = (_writePoint + 1) % _bufferSize;
            if (_dataSize > _pointsOfGraph) _showStart++;
            _dataSize++;
            if (_dataSize > _bufferSize) _dataSize = _bufferSize;

        }

        public enum SaveOptions { X = 0, Y, RelatedData };
        StreamWriter _savingFile = null;
        SaveOptions[] _saveOptions = null;
        List<int> _savingIndexes = new List<int>();

        double minY = double.MaxValue;
        double maxY = double.MinValue;
        bool _isRefreshing = false;
        public void RefreshNow()
        {
            if (_dataSize < 10) return;

            if (_isRefreshing) return;
            _isRefreshing = true;

            minY = float.MaxValue;
            maxY = float.MinValue;

            if (_selectedSubset < 0)
            {
                for (int subset = 0; subset < _subsets; subset++)
                {
                    for (int i = 0; i < _pointsOfGraph; i++)
                    {
                        double y = _graphData[subset, i];
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }
            else
            {
                
                for (int i = 0; i < _pointsOfGraph; i++)
                {
                    double y = _graphData[_selectedSubset, i];
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
            Refresh();
            
            _isRefreshing = false;
        }

        public override void Refresh()
        {
            GraphArea.Invalidate();
            GraphArea.Update();

            base.Refresh();
        }

        int _samplingSize = 1;
        /// <summary>
        /// SamplingSize가 2이면 2개중 1개를 표시한다.
        /// </summary>
        public int SamplingSize
        {
            get { return _samplingSize; }
            set { _samplingSize = value; }
        }

        enum OffsetOverStatus { NotOver=0, MinOver, MaxOver };
        OffsetOverStatus _offsetOverStatus = OffsetOverStatus.NotOver;

        void DrawGraph(int subset, ref float minY, ref float maxY)
        {
            //if (_graphData.Length <= subset) return;
            int sizeOfData = _dataSize;
            int samplingSize = _samplingSize;
        }

        public void ClearBuffer()
        {
            _dataSize = 0;
            _showStart = 0;
            _writePoint = 0;
        }

        private void B_Close_Click(object sender, EventArgs e)
        {
            if (E_CloseClicked != null) E_CloseClicked(this, e);
        }

        private void B_SnapShot_Click(object sender, EventArgs e)
        {
            TakeSnapShot();
        }

        public void TakeSnapShot(System.Drawing.Imaging.ImageFormat format=null)
        {
            if (E_SnapShotClicked != null) E_SnapShotClicked(this, new SnapShotEventArgs(GraphArea,format));
        }
    }


}
