using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gigasoft.ProEssentials;
using Gigasoft.ProEssentials.Enums;
using Gigasoft.ProEssentials.EventArg;
using GigsStructs = Gigasoft.ProEssentials.Structs;

using System.Runtime.InteropServices;
using System.IO;

namespace GraphModules
{

    public partial class MultiLineGraphWinRealTime : UserControl
    {
        public event MouseEventHandler E_MouseDown;
        public event MouseEventHandler E_MouseUp;
        public event MouseEventHandler E_MouseMove;
        public event MouseEventHandler E_OverXMax;
        public event MouseEventHandler E_OverXMin;
        public event GraphClickEventHandler E_HotSpotClicked;
        public event GraphClickEventHandler E_HotSpotMovedByKey;
        public event EventHandler E_Click;
        public event EventHandler E_CloseClicked;
        public event EventHandler E_CheckStateChanged;
        
        public event SnapShotEventHandler E_SnapShotClicked;
        
        

        public MultiLineGraphWinRealTime()
        {
            init(4);
        }

        public MultiLineGraphWinRealTime(int subsets, int bufferSize = 600, ICollection<String> subsetTitles = null)
        {
            init(subsets, bufferSize, subsetTitles);
        }

        void init(int subsets, int bufferSize = 600, ICollection<String> subsetTitles = null)
        {
            InitializeComponent();
            _lazyRefresh = true;
            if (subsetTitles != null) U_SubsetTitles = subsetTitles.ToArray();

            U_Subsets = subsets;
            U_BufferSize = bufferSize;
            //U_RefreshCount = refreshCount;

            U_LineColors = _lineColors;
            U_LineStyles = _lineStyles;

            U_BackStyle = Style.BlueBackWhiteLine;

            

            G_Science.MouseDown += new MouseEventHandler(G_Science_MouseDown);
            G_Science.MouseUp += new MouseEventHandler(G_Science_MouseUp);
            G_Science.MouseMove += new MouseEventHandler(G_Science_MouseMove);
            G_Science.Click+=new EventHandler(G_Science_Click);
            //G_Science.PreviewKeyDown += new PreviewKeyDownEventHandler(G_Science_PreviewKeyDown);
            G_Science.PeCursorMoved += new Pesgo.CursorMovedEventHandler(G_Science_PeCursorMoved);

            //G_Science.PeGraphHotSpot += new Pesgo.GraphHotSpotEventHandler(G_Science_PeGraphHotSpot);
            B_SnapShot.Click +=new EventHandler(B_SnapShot_Click);
            B_Close.Click +=new EventHandler(B_Close_Click);

            
            
            C_GraphName.CheckStateChanged += new EventHandler(C_GraphName_CheckStateChanged);
            setGraphInit();
            _lazyRefresh = false;
        }

        void G_Science_PeCursorMoved(object sender, EventArgs e)
        {
            if (_graphXValue.Count == 0) return;
            // get last mouse location within control //
            int x1 = G_Science.PeUserInterface.Cursor.Point;
            int subset = G_Science.PeUserInterface.Cursor.Subset;
            double fX = _graphXValue[x1];
            double fY = U_GraphYValue(subset, x1 + _minSize + U_OffsetX);


            if (E_HotSpotMovedByKey != null) E_HotSpotMovedByKey(this, new GraphClickEventArgs(subset, (int)x1, false, fX, fY));
        }

        
        public void SelectPoint(int subset, int index)
        {
            G_Science.PeUserInterface.Cursor.Subset = subset;
            G_Science.PeUserInterface.Cursor.Point = index;
            
            G_Science.Refresh();
        }

        void G_Science_Click(object sender, EventArgs e)
        {
            if (E_Click != null) E_Click(this, e);
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
                return G_Science.PeFunction.GetRectAxis();
            }
        }

        public Rectangle U_GraphRectGraph
        {
            get
            {
                return G_Science.PeFunction.GetRectGraph();
            }
        }


        [Browsable(true)]
        public Boolean U_Checked
        {
            get { return C_GraphName.Checked; }
            set { C_GraphName.Checked = value; }
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
            if (_graphXValue.Count == 0) return;
            // get last mouse location within control //
            Int32 pX, pY, nA, nX, nY;
            Double fX = 0, fY = 0;
            System.Drawing.Point pt;

            // get last mouse location within control //'
            pt = G_Science.PeUserInterface.Cursor.LastMouseMove;
            pX = pt.X;
            pY = pt.Y;

            nA = 0;      // Initialize axis, non-zero only if using MultiAxesSubsets
            nX = pX;     // Initialize nX and nY with mouse location
            nY = pY;
            G_Science.PeFunction.ConvPixelToGraph(ref nA, ref nX, ref nY, ref fX, ref fY, false, false, false);

            // We now know data coordinates for mouse location //

            // Use fX to interpolate each subset to find approximate Y value //
            float x1;
            int min = _minSize + U_OffsetX;
            
            int max = _graphXValue.Count - 1 +U_OffsetX;
            if (min < 0)
            {
                min = 0;
                max = U_PointsOfGraph;
            }
            else if (max > U_BufferSize-1)
            {
                min = U_BufferSize - U_PointsOfGraph;
                max = U_BufferSize - 1;
            }
            int minOrg = min;

            nX = Convert.ToInt32(fX);   // nX is floor of fX
            //x2 = nX + 1;   // x2 is right most point index
            x1 = _graphXValue.IndexOf(nX, min);

            if (x1 < 0)
            {

                int mid = -1;
                int beforeMid;
                //if (_graphXValue[max] <= fX) mid = max;
                //else if (_graphXValue[min] >= fX) mid = min;
                //else
                {
                    while (max != min)
                    {
                        beforeMid = mid;
                        mid = (max + min) / 2;
                        if (mid == beforeMid) break;
                        if (_graphXValue[mid] < fX) min = mid;
                        else if (_graphXValue[mid] > fX) max = mid;
                        else
                        {
                            break;
                        }
                    }
                }
                x1 = mid-minOrg;
            }
            else
            {
                x1 = x1 - minOrg;
            }
            
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
            if (E_MouseUp != null) E_MouseUp(this,e);
            if( E_HotSpotClicked!=null)  E_HotSpotClicked(this, new GraphClickEventArgs((int)x1, e.Clicks>1, fX, fY));
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
        int _selectedSubset=0;
        public int U_SelectedSubset
        {
            get
            {
                return G_Science.PeUserInterface.Cursor.Subset;
            }
            set
            {
                G_Science.PeUserInterface.Cursor.Subset = value;
                _selectedSubset = value;
                //G_Science.pe
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

        public List<double> U_GraphXValue
        {
            get { return _graphXValue; }
        }
        public double U_GraphYValue(int subset, int index)
        {
            return G_Science.PeData.Y[subset, index]; 
        }

        public double U_SelectedX{
            get
            {
                try
                {
                    return _graphXValue[G_Science.PeUserInterface.Cursor.Point+_minSize+U_OffsetX];
                }
                catch { throw; }
            }
        }

        public double U_SelectedY
        {
            get
            {
                try
                {
                    //return G_Science.PeData.Y[U_SelectedSubset, G_Science.PeUserInterface.Cursor.Point+_minSize+U_OffsetX];
                    return G_Science.PeData.Y[U_SelectedSubset, G_Science.PeUserInterface.Cursor.Point];
                }
                catch { throw; }
            }
        }


        /*
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        */
        List<float>[] _graphData = new List<float>[1];//1 is just for initial value
        List<float>[] _graphDataX = new List<float>[1];
        List<double> _graphXValue = new List<double>();

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
                _graphData = new List<float>[_subsets];
                _graphDataX = new List<float>[_subsets];

                G_Science.PeData.Subsets = value;
                
                for (int i = 0; i < _subsets; i++)
                {
                    _graphData[i] = new List<float>();
                    _graphDataX[i] = new List<float>();

                    G_Science.PeData.X[i, 0] = 0;
                    G_Science.PeData.X[i, 1] = 0;
                    G_Science.PeData.X[i, 2] = 0;
                    G_Science.PeData.X[i, 3] = 0;
                    
                    G_Science.PeData.Y[i, 0] = 0;
                    G_Science.PeData.Y[i, 1] = 0;
                    G_Science.PeData.Y[i, 2] = 0;
                    G_Science.PeData.Y[i, 3] = 0;
                    
                    //G_Science.PeLegend.SubsetsToLegend[i] = i;
                    try
                    {
                        G_Science.PeString.SubsetLabels[i] = (U_SubsetTitles.Count()>i)? U_SubsetTitles[i]:"line"+i;
                        G_Science.PeLegend.SubsetLineTypes[i] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
                        G_Science.PeAnnotation.Line.XAxisText[i] = "test";
                        G_Science.PeLegend.SubsetColors[i] = U_LineColors[i];
                        G_Science.PeLegend.SubsetsToLegend[i] = i;
                    }
                    catch { }
                }
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

        [SettingsBindable(true)]
        [Browsable(true)]
        public double U_ManualMinY
        {
            set
            {
                G_Science.PeGrid.Configure.ManualMinY = value;
                if(_lazyRefresh==false) RefreshNow();
            }
            get
            {
                return G_Science.PeGrid.Configure.ManualMinY;
            }
        }

        [SettingsBindable(true)]
        [Browsable(true)]
        public double U_ManualMaxY
        {
            set
            {
                G_Science.PeGrid.Configure.ManualMaxY = value;
                if (_lazyRefresh == false) RefreshNow();
            }
            get
            {
                return G_Science.PeGrid.Configure.ManualMaxY;
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
                switch (value)
                {
                    case XAxisMode.SecondMode:
                           G_Science.PeData.UsingXDataii = true;
                           G_Science.PeData.DateTimeMode = true;
                           G_Science.PeData.DateTimeShowSeconds = true;
                           G_Science.PeData.DateTimeMilliSeconds = true;
                        break;
                    case XAxisMode.IndexMode:
                           G_Science.PeData.UsingXDataii = true;
                           
                           G_Science.PeData.DateTimeMode = false;
                           G_Science.PeData.DateTimeShowSeconds = false;
                           G_Science.PeData.DateTimeMilliSeconds = false;
                        break;
                }
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
                G_Science.PeData.Points = _pointsOfGraph;
                
            }
        }
        int _bufferSize = 601;
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_BufferSize
        {
            get { return _bufferSize-1; }
            set
            {
                _bufferSize = value+1;

                    for (int i = 0; i < _subsets; i++)
                    {
                        _graphData[i] = new List<float>(value+1);
                        _graphDataX[i] = new List<float>(value+1);
                    }

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
                G_Science.PeColor.SubsetColors.CopyFrom(value);
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
        LineType[] _lineStyles = new LineType[] {
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
            get { return _lineStyles; }
            set
            {
                
                _lineStyles = value;
                Gigasoft.ProEssentials.Enums.LineType[] arr = new Gigasoft.ProEssentials.Enums.LineType[_lineStyles.Length];
                for (int i = 0; i < _lineStyles.Length; i++)
                {
                    arr[i] = (Gigasoft.ProEssentials.Enums.LineType)_lineStyles[i];
                }
                
                G_Science.PePlot.SubsetLineTypes.CopyFrom(arr);
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
                G_Science.PeString.XAxisLabel = value[0];
                G_Science.PeString.YAxisLabel = value[1];

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
                
                for (int i = 0; i < value.Count(); i++)
                {
                    try
                    {
                        G_Science.PeString.SubsetLabels[i] = value[i];
                        G_Science.PeLegend.SubsetsToLegend[i] = i;
                        //G_Science.PeLegend.AnnotationText[i] = value[i];
                    }
                    catch
                    {
                        Console.WriteLine("graph legend" + i + " has not handled...");
                    }
                }

                
                if (_lazyRefresh == false) RefreshNow();
            }
        }                        

        int _refreshCount = 50;
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_RefreshCount
        {
            get { return _refreshCount; }
            set
            {
                if (value < 1) throw new Exception("RefreshCount는 1보다 작을 수 없습니다");
                _refreshCount = value;
            }
        }

        [BrowsableAttribute(true)]
        [Bindable(true)]
        public enum GridLine {Both=GridLineControl.Both,None=GridLineControl.None,XAxis=GridLineControl.XAxis,YAxis=GridLineControl.YAxis };
        public GridLine U_GridLine
        {
            get { return (GridLine)G_Science.PeGrid.LineControl; }
            set
            {
                G_Science.PeGrid.LineControl = (GridLineControl)value;
                if (_lazyRefresh == false) RefreshNow();
            }
        }


        public Pesgo U_GraphArea
        {
            get { return G_Science; }
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
                G_Science.PeGrid.Configure.ManualScaleControlY = (Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax);//무조건 MinMax모드로 하고 수동으로 움직인다.
                if (value == ManualScaleControl.None)
                {
                    U_AutoMaxYIndex = -1;
                    U_AutoMinYIndex = -1;
                }
                
                if (_lazyRefresh==false) RefreshNow();
            }
        }

        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Boolean U_LegendShow
        {
            get { return G_Science.PeLegend.Show; }
            set
            {
                G_Science.PeLegend.Show = value;
                G_Science.PeLegend.SimpleLine = true;
                G_Science.PeLegend.OneLinePerLegend = true;

                for (int i = 0; i < U_Subsets; i++)
                {
                    try
                    {
                        G_Science.PeLegend.SubsetColors[i] = U_LineColors[i];
                        G_Science.PeLegend.SubsetsToLegend[i] = i;
                        G_Science.PeLegend.SubsetLineTypes[i] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
                        G_Science.PeLegend.AnnotationText[i] = U_SubsetTitles[i];
                    }
                    catch {
                        Console.WriteLine("graph legend" + i + " has not handled...");
                    }
                }
                if (_lazyRefresh == false) RefreshNow();
            }
        }

        public enum GraphMethods { Line, Step, Curve, Point, PointAndLine, PointAndCurve };
        public GraphMethods U_GraphMethod
        {
            get
            {
                switch (G_Science.PePlot.Method)
                {
                    case SGraphPlottingMethod.Line:
                        return GraphMethods.Line;
                    case SGraphPlottingMethod.Point:
                        return GraphMethods.Point;
                    case SGraphPlottingMethod.PointsPlusLine:
                        return GraphMethods.PointAndLine;
                    case SGraphPlottingMethod.PointsPlusSpline:
                        return GraphMethods.PointAndCurve;
                    case SGraphPlottingMethod.Step:
                        return GraphMethods.Step;
                    default:
                        return GraphMethods.Line;
                }
            }
            set
            {
                switch (value)
                {
                    case GraphMethods.Line:
                        G_Science.PePlot.Method = SGraphPlottingMethod.Line;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.Line;
                        break;
                    case GraphMethods.Point:
                        G_Science.PePlot.Method = SGraphPlottingMethod.Point;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.Point;
                        break;
                    case GraphMethods.PointAndLine:
                        G_Science.PePlot.Method = SGraphPlottingMethod.PointsPlusLine;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.PointsPlusLine;
                        break;
                    case GraphMethods.PointAndCurve:
                        G_Science.PePlot.Method = SGraphPlottingMethod.PointsPlusSpline;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.PointsPlusSpline;
                        break;
                    case GraphMethods.Step:
                        G_Science.PePlot.Method = SGraphPlottingMethod.Step;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.Step;
                        break;
                    default:
                        G_Science.PePlot.Method = SGraphPlottingMethod.Line;
                        G_Science.PePlot.MethodII = SGraphPlottingMethodII.Line;
                        break;
                }

                //G_Science.PePlot.Method = SGraphPlottingMethod.Line;
            }
        }
        public Dictionary<int, GraphMethods> SubsetLineTypes = new Dictionary<int, GraphMethods>();

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

        double _nullDataValue = float.MinValue;
        public double U_NullDataValue{
            get{
                return _nullDataValue;
            }
            set{
                _nullDataValue = value;
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

        public Boolean U_ShowMarkingPoints
        {
            get
            {
                return G_Science.PePlot.MarkDataPoints;
            }
            set
            {
                G_Science.PePlot.MarkDataPoints = value;
            }
        }



        
        String _tempLine = "";
        public void setData2D(double x, double y, int subsetIndex = 0)
        {
            if (_pausing) return;
            // New y value and x value //
            _writingData = true;
            G_Science.PePlot.MarkDataPoints = true;

            if (_graphData == null || _graphData[0] == null) return;

            if (_graphData[0].Count == _bufferSize) //queue가 가득 찼다면
            {
                if (_graphData[subsetIndex].Count == _relatedData.Count) _relatedData.RemoveRange(0, _refreshCount);//1회만 적용됨.
                _graphData[subsetIndex].RemoveRange(0, _refreshCount);
                _graphDataX[subsetIndex].RemoveRange(0, _refreshCount);
                
                G_Science.PeData.X.Clear();
                G_Science.PeData.Y.Clear();
            }

            _graphDataX[subsetIndex].Add((float)x);
            _graphData[subsetIndex].Add((float)y);
            //BindingSource source = new BindingSource();

            if (_savingFile != null)
            {
                
                Boolean isFirstElement = true;
                if (subsetIndex == 0)
                {

                    if (_saveOptions.Contains(SaveOptions.RelatedData))
                    {

                        _tempLine = (_relatedData != null) ? _relatedData.ToString() : "";
                        isFirstElement = false;
                    }
                    
                }
                if (_tempLine.Equals("")==false) isFirstElement = false;
                
                for (int i = 0; i < _saveOptions.Length; i++)
                {
                    if(isFirstElement==false) _tempLine += ",";
                    if (_saveOptions[i] == SaveOptions.X) _tempLine += x.ToString();
                    else if (_saveOptions[i] == SaveOptions.Y)
                    {
                       _tempLine += y.ToString();
                    }
                }
                _savingFile.Write(_tempLine);
                if (subsetIndex == U_Subsets - 1) _savingFile.Write("\n");
            }

            _writingData = false;
            if (_lazyRefresh == false) DrawGraphFor2D();
            
        }

        List<object> _relatedData = new List<object>();
        /// <summary>
        /// You must call it before setData or set2DData...
        /// </summary>
        /// <param name="relatedData">relatedData is saving on the same line with data for setData that is called after it.</param>
        public void setRelatedData(object relatedData)
        {
            if (_pausing) return;
            _relatedData.Add(relatedData);
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

        public Object U_SelectedRelatedData
        {
            get {
                if (_relatedData.Count > 0)
                {
                    try
                    {
                        return _relatedData[G_Science.PeUserInterface.Cursor.Point + _minSize + _offsetX];
                    }
                    catch { return ""; }
                }
                else return "";
            }
        }

        public List<Object> U_RelatedData
        {
            get { return _relatedData; }
        }

        public void pause()
        {
            _pausing = true;
        }
        public void resume()
        {
            _pausing = false;
        }
        
        public void setData(object relatedData, double x, params double[] y)
        {
            if (_pausing) return;
            setRelatedData(relatedData);
            setData(x, y);
        }


        int _minSize = 0;
        bool _pausing = false;
        Boolean _writingData = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">U_X_AxisMode가 SecondMode일때는 new DateTime(year, month, day, hour, min, sec, ms).ToOADate();
        ///  IndexMode일때는 float tick을 넘겨주면 된다.
        /// </param>
        /// <param name="y">각 subset에 해당하는 값</param>
        public void setData(double x, params double[] y)
        {
            // New y value and x value //
            if (_pausing) return;
            if (_graphData == null || _graphData[0] == null) return;
            _writingData = true;
            if (_graphData[0].Count == _bufferSize) //queue가 가득 찼다면
            {
                if (U_X_AxisMode == XAxisMode.IndexMode)
                {
                    G_Science.PeGrid.Configure.ManualScaleControlX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;
                    
                    //_minSize += _refreshCount;
                    G_Science.PeGrid.Configure.ManualMinX = _graphXValue[_minSize];
                    G_Science.PeGrid.Configure.ManualMaxX = _graphXValue[_minSize+_pointsOfGraph-1];
                }
                else
                {
                    G_Science.PeGrid.Configure.ManualScaleControlX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;
                    
                    G_Science.PeGrid.Configure.ManualMinX = _graphXValue[_refreshCount];
                    G_Science.PeGrid.Configure.ManualMaxX = _graphXValue[_graphXValue.Count - 1]+(_graphXValue[_refreshCount]-_graphXValue[0]);
                }

                for (int subsetIndex = 0; subsetIndex < _subsets; subsetIndex++)
                {
                    _graphData[subsetIndex].RemoveRange(0, _refreshCount);

                }
                _graphXValue.RemoveRange(0, _refreshCount);
                if (_relatedData.Count > _refreshCount) _relatedData.RemoveRange(0, _refreshCount);
                   
                /*
                G_Science.PeGrid.Zoom.MinX = minSize;
                G_Science.PeGrid.Zoom.MaxX = minSize + U_PointsOfGraph;
                 */
                
            }
            if (_savingFile != null)
            {
                String line = "";
                for (int i = 0; i < _saveOptions.Length; i++)
                {
                    if (i != 0) line += ",";
                    if (_saveOptions[i] == SaveOptions.X) line += x.ToString();
                    else if (_saveOptions[i] == SaveOptions.RelatedData && _relatedData != null) line += _relatedData[_relatedData.Count-1].ToString(); //마지막추가된 relativeData를 추가.
                    else if (_saveOptions[i] == SaveOptions.Y)
                    {

                        for (int j = 0; j < y.Length; j++)
                        {
                            if (j != 0) line += ",";
                            line += String.Format("{0:f12}", y[j]);
                        }
                    }
                }
                _savingFile.WriteLine(line);
            }
            for (int subset = 0; subset < y.Length; subset++)
            {
                _graphData[subset].Add((float)y[subset]);
            }
            _graphXValue.Add(x);
            //BindingSource source = new BindingSource();
            _writingData = false;
            if (_lazyRefresh == false)
            {
                RefreshNow();
                //DrawGraphFor1D();
            }

        }

        public enum SaveOptions { X = 0, Y, RelatedData };
        StreamWriter _savingFile = null;
        SaveOptions[] _saveOptions = null;
        List<int> _savingIndexes = new List<int>();


        /// <summary>
        /// Data를 지정된 형식으로 저장한다.
        /// </summary>
        /// <param name="file">저장할 파일</param>
        /// <param name="savingTitles">그래프에 있는 subset중에서 저장할 항목. null이면 모두 저장</param>
        /// <param name="saveOptions">저장할 순서를 정한다. 아무것도 적지 않으면 RelatedData,X,Y순이다.</param>
        public void StartSaveData(FileStream file, List<String> savingTitles=null, params SaveOptions[] saveOptions)
        {
            if (_savingFile == null)
            {
                _savingFile = new StreamWriter(file);
                _saveOptions = (saveOptions==null || saveOptions.Length==0)? new SaveOptions[]{ SaveOptions.RelatedData, SaveOptions.X, SaveOptions.Y}: saveOptions;
                if (savingTitles == null) savingTitles = _subSetTitles.ToList();
                int count = U_SubsetTitles.Count();
                _savingIndexes.Clear();
                for(int i=0; i<count; i++)
                {
                    if (savingTitles.Contains(U_SubsetTitles[i])) _savingIndexes.Add(i);
                }
                writeTitleLine();
            }
            else
            {
                throw new Exception("이미 저장중인 파일이 있습니다.");
            }
        }
        
        public void StartSaveData(StreamWriter file, List<int> savingSubsets, params SaveOptions[] saveOptions)
        {
            if (_savingFile == null)
            {
                _savingFile = file;
                _saveOptions = saveOptions;
                _savingIndexes = new List<int>(savingSubsets);

                writeTitleLine();
            }
            else
            {
                throw new Exception("이미 저장중인 파일이 있습니다.");
            }
        }

        /// <summary>
        /// you have to call startSaveData instead of calling it
        /// </summary>
        void writeTitleLine()
        {
            if (U_GraphDimension == GraphDimension.One)
            {
                String line = "";
                for (int i = 0; i < _saveOptions.Length; i++)
                {
                    if (i != 0) line += ",";
                    if (_saveOptions[i] == SaveOptions.X) line += (U_X_AxisMode == XAxisMode.IndexMode) ? "Index" : "Time";
                    else if (_saveOptions[i] == SaveOptions.RelatedData) line += _relatedDataName.ToString();
                    else if (_saveOptions[i] == SaveOptions.Y)
                    {
                        for (int j = 0; j < _savingIndexes.Count(); j++)
                        {
                            if (j != 0) line += ",";
                            line += U_SubsetTitles[_savingIndexes[j]].ToString();
                        }
                    }
                }
                _savingFile.WriteLine(line);
            }
            else
            {
                String line = "";
                if (_saveOptions.Contains(SaveOptions.RelatedData)) line += _relatedDataName.ToString() + ",";
                int added = 0;
                for (int i = 0; i < _saveOptions.Length; i++)
                {

                    if (_saveOptions[i] == SaveOptions.X)
                    {
                        if (added != 0) line += ",";
                        line += (U_X_AxisMode == XAxisMode.IndexMode) ? "Index" : "Time";
                        added++;
                    }

                    else if (_saveOptions[i] == SaveOptions.Y)
                    {
                        for (int j = 0; j < _savingIndexes.Count(); j++)
                        {
                            if (added != 0) line += ",";
                            line += U_SubsetTitles[_savingIndexes[j]].ToString();
                        }
                        added++;
                    }


                }
                _savingFile.WriteLine(line);
            }
        }

        public void StopSaveData()
        {
            if (_savingFile != null)
            {
                try
                {
                    _savingFile.Close();
                }
                catch { }
            }
            _savingFile = null;
        }

        bool _refreshEnabled = true;
        public void SuspendRefresh()
        {
            _refreshEnabled = false;
        }
        public void ResumeRefresh()
        {
            _refreshEnabled = true;
        }

        public void RefreshNow()
        {
            if (_refreshEnabled == false) return;
            if (U_GraphDimension == GraphDimension.One) DrawGraphFor1D();
            else DrawGraphFor2D();

            /*
            if (_graphXValue.Count > 0)
            {
                try
                {
                    G_Science.PeUserInterface.Allow.Zooming = AllowZooming.Horizontal;
                    G_Science.PeGrid.Zoom.MinX = G_Science.PeData.Xii[0, 0];
                    G_Science.PeGrid.Zoom.MaxX = G_Science.PeData.Xii[0, _pointsOfGraph - 1];
                    

                    //G_Science.PeGrid.Configure.ManualMinTX = _graphXValue[startIndex];
                    //G_Science.PeGrid.Configure.ManualMaxTX = _graphXValue[endCondition];
                    //G_Science.PeGrid.Configure.ManualMinX = _graphXValue[startIndex];
                    //G_Science.PeGrid.Configure.ManualMaxX = _graphXValue[endCondition];

                }
                catch { }
            }
             */
        }
        void DrawGraphFor2D()
        {
            if (_writingData) return;
            int samplingSize;
            for (int subsetIndex = 0; subsetIndex < U_Subsets; subsetIndex++)
            {
                int sizeOfData = _graphData[subsetIndex].Count;
                samplingSize = sizeOfData / _pointsOfGraph;
                if (samplingSize == 0) samplingSize = 1;
                if (sizeOfData % _pointsOfGraph == 0)
                {
                    G_Science.PeData.X.Clear();
                    G_Science.PeData.Y.Clear();
                }
                int sampledIndex = 0;
                for (int j = 0; j < sizeOfData; j = j + samplingSize) //samplingSize만큼 이동한다.
                {

                    G_Science.PeData.X[subsetIndex, sampledIndex] = _graphDataX[subsetIndex][j];
                    G_Science.PeData.Y[subsetIndex, sampledIndex] = _graphData[subsetIndex][j];
                    sampledIndex++;
                }

            }
            // Append new values  //
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, ys[0], 1);
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, xs[0], 1);

            //}
            // Update image and force paint //
            //G_Science.PeFunction.ReinitializeResetImage();

            G_Science.Refresh();
        }
        enum OffsetOverStatus { NotOver=0, MinOver, MaxOver };
        OffsetOverStatus _offsetOverStatus = OffsetOverStatus.NotOver;
        void DrawGraphFor1D()
        {
            if (_writingData) return;
            float minY = float.MaxValue, maxY = float.MinValue;

            for (int subset = 0; subset < _subsets; subset++)
            {

                
                /*
                for (int i = 1; i < sizeOfData; i++) //sampling할 size를 가져온다.
                {
                    if ((sizeOfData / i) < _pointsOfGraph) 
                    {
                        samplingSize = i;
                        break;
                    }
                }
                 */

                DrawGraph(subset, ref minY, ref maxY);
                
            }
            /*
            DrawGraph(_selectedSubset, ref minY, ref maxY); //선택된 것은 한번 더 그려줌..

            if (U_ManualScaleY != ManualScaleControl.None)
            {
                if (U_ManualScaleY != ManualScaleControl.Min && U_AutoMaxYIndex >= 0) U_ManualMinY = minY - ((maxY - minY) * U_Y_MinMaxMarginRate);
                if (U_ManualScaleY != ManualScaleControl.Max && U_AutoMinYIndex >= 0) U_ManualMaxY = maxY + ((maxY - minY) * U_Y_MinMaxMarginRate);

            }
            else
            {
                U_ManualMinY = minY - ((maxY - minY) * U_Y_MinMaxMarginRate);
                U_ManualMaxY = maxY + ((maxY - minY) * U_Y_MinMaxMarginRate);
            }
             */
            float[] ys = new float[_subsets];
            float[] xs = new float[_subsets];
                if (_graphXValue.Count > 0)
                {
                    
                    for (int i = 0; i < _subsets; i++)
                    {
                        xs[i] = (float)_graphXValue[_graphXValue.Count - 1];    
                        ys[i] = (float)_graphData[i][_graphXValue.Count - 1];//맨 마지막 값을 가져옴.
                    }
                    Gigasoft.ProEssentials.Api.PEvsetW(G_Science.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, ys, _subsets);
                    Gigasoft.ProEssentials.Api.PEvsetW(G_Science.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, ++_count, _subsets);
                }
            
            // Append new values  //
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, ys[0], 1);
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, xs[0], 1);

            //}
            // Update image and force paint //
            if(_count>100) G_Science.PeGrid.Configure.ManualScaleControlX = Gigasoft.ProEssentials.Enums.ManualScaleControl.None;
            G_Science.PeFunction.ReinitializeResetImage();
            G_Science.Refresh();
        }
        float _count = 0;
        void DrawGraph(int subset, ref float minY, ref float maxY)
        {
            int sizeOfData = _graphData[subset].Count;
            int samplingSize = sizeOfData / _pointsOfGraph;
            if (samplingSize == 0) samplingSize = 1;

            int startIndex;
            int endCondition;
            int increment;
            if (U_GraphShowMode == GraphShowMode.SampledGraph)
            {
                startIndex = 0;
                endCondition = sizeOfData;
                increment = samplingSize;
            }
            else //GraphShowMode.ShowLastData 
            {

                startIndex = (sizeOfData < U_PointsOfGraph) ? 0 : sizeOfData - U_PointsOfGraph;
                _minSize = startIndex;
                endCondition = sizeOfData;
                increment = 1;

                if (startIndex + _offsetX >= 0 && endCondition + _offsetX <= sizeOfData)
                {
                    startIndex += _offsetX;
                    endCondition += _offsetX;
                    //_offsetOverStatus = OffsetOverStatus.NotOver;
                }
                else if (startIndex + _offsetX < 0)
                {
                    _overOffsetSize = -(startIndex + _offsetX);
                    _offsetX = -startIndex;
                    startIndex = 0;
                    _offsetOverStatus = OffsetOverStatus.MinOver;

                }
                else //endCondition+_offsetX > sizeOfData
                {
                    _overOffsetSize = (endCondition + _offsetX) - sizeOfData;
                    _offsetX = 0;
                    endCondition = sizeOfData;
                    _offsetOverStatus = OffsetOverStatus.MaxOver;

                }

            }



            if (U_X_AxisMode == XAxisMode.IndexMode)
            {

                ShowDataOnGraphForIndexMode(startIndex, endCondition, increment, subset, ref minY, ref maxY);

            }
            else
            {
                ShowDataOnGraphForSecondMode(startIndex, endCondition, increment, subset, ref minY, ref maxY);

            }
        }
        void ShowDataOnGraphForIndexMode(int startIndex, int endCondition, int increment, int subset, ref float minY, ref float maxY)
        {
            float currY=0;
            int sampledIndex = 0;
            double dist = 1;
            double before = 0;
            double nowX=0;
            for (int j = startIndex; j < endCondition; j = j + increment) //samplingSize만큼 이동한다.
            {
                try
                {
                    nowX = _graphXValue[j];
                    dist = nowX - before;
                    before = nowX;
                    //G_Science.PeData.Xii[subset, sampledIndex] = _graphXValue[j];
                    currY = _graphData[subset][j];
                    //G_Science.PeData.Y[subset, sampledIndex] = currY;
                    if (U_ManualScaleY != ManualScaleControl.None)
                    {
                        if (subset == U_AutoMaxYIndex && maxY < currY) maxY = currY;
                        if (subset == U_AutoMinYIndex && minY > currY) minY = currY;
                        if (U_AutoMaxYIndex < 0 && U_AutoMinYIndex < 0)
                        {
                            if (maxY < currY) maxY = currY;
                            if (minY > currY) minY = currY;
                        }
                    }
                    else
                    {
                        if (maxY < currY) maxY = currY;
                        if (minY > currY) minY = currY;
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    break; //데이터를 그리는중에 메모리 관리가 일어나면 발생..
                }
                catch (IndexOutOfRangeException e)
                {
                    break; //데이터를 그리는중에 메모리 관리가 일어나면 발생..
                }
                sampledIndex++;
            }
            /*
            for (int rest = sampledIndex; rest < _pointsOfGraph; rest++) //값을 넣고 나머지 값들을 초기화시켜줌.
            {

                G_Science.PeData.Xii[subset, rest] = dist*rest;
                G_Science.PeData.Y[subset, rest] = (float)U_NullDataValue;
            }
             */
           

            //G_Science.PeGrid.Configure.AutoMinMaxPaddingX = 1;



        }
        void ShowDataOnGraphForSecondMode(int startIndex, int endCondition, int increment, int subset, ref float minY, ref float maxY)
        {
            float currY;
            int sampledIndex = 0;
            double timeSpan = 0.02;
            sampledIndex = 0;
            if (_graphXValue.Count > 1) timeSpan = (_graphXValue[1] - _graphXValue[0]);
            for (int j = startIndex; j < endCondition; j = j + increment) //samplingSize만큼 이동한다.
            {
                try
                {
                    G_Science.PeData.Xii[subset, sampledIndex] = _graphXValue[j];
                    currY = _graphData[subset][j];
                    G_Science.PeData.Y[subset, sampledIndex] = currY;
                    if (U_ManualScaleY != ManualScaleControl.None)
                    {
                        if (subset == U_AutoMaxYIndex && maxY < currY) maxY = currY;
                        if (subset == U_AutoMinYIndex && minY > currY) minY = currY;
                    }
                    else
                    {
                        if (maxY < currY) maxY = currY;
                        if (minY > currY) minY = currY;
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    break;//데이터를 그리는중에 메모리 관리가 일어나면 발생..
                }
                sampledIndex++;
            }

            for (int rest = sampledIndex; sampledIndex > 0 && rest < _pointsOfGraph; rest++) //값을 넣고 나머지 값들을 초기화시켜줌.
            {
                G_Science.PeData.Xii[subset, rest] = _graphXValue[sampledIndex - 1] + (timeSpan * (rest - sampledIndex + 1));
                G_Science.PeData.Y[subset, rest] =  (float)U_NullDataValue;
            }
        }
        public void setGraphInit(String title = "", double min = -10, double max = 50, ManualScaleControl manualScale= ManualScaleControl.None)
        {
            //! Chart fills 100 points autoscaling x axis is it
            //! is filled.  Once 100 point have been passed, the
            //! chart then acts as a strip chart.

            G_Science.PeData.Subsets = U_Subsets;
            //G_Science.PeData.AppendToEnd = true;
            G_Science.PeAnnotation.Graph.TextSize = 11;
            G_Science.PeData.NullDataValue = U_NullDataValue;
            G_Science.PeData.Points = _bufferSize;

            //G_Science.PeUserInterface.Menu.GridLine = MenuControl.Show;
            //G_Science.PeData.SubsetsToShow[0] = 1;
            //G_Science.PeData.UsingYDataii = false;

            //G_Science.PeData.SubsetsToShow[1] = 1;
            //G_Science.PeData.SubsetsToShow[2] = 2;

            // Set Manual Y scale //
            //G_Science.PeGrid.Configure.ManualScaleControlY = ManualScaleControl.MinMax;
            G_Science.PeGrid.Configure.ManualScaleControlY = (Gigasoft.ProEssentials.Enums.ManualScaleControl)manualScale;// (Gigasoft.ProEssentials.Enums.ManualScaleControl)ManualScaleControl.None;
            // G_Science.PeGrid.Configure.ManualScaleControlX = ManualScaleControl.None;

            G_Science.PeGrid.Configure.ManualMinY = min;
            G_Science.PeGrid.Configure.ManualMaxY = max;

            // Clear out default data // x[0, 0] 은 x[subset, index]를 말한다.
            G_Science.PeData.X[0, 0] = 0;
            G_Science.PeData.X[0, 1] = 0;
            G_Science.PeData.X[0, 2] = 0;
            G_Science.PeData.X[0, 3] = 0;
            G_Science.PeData.Y[0, 0] = 0;
            G_Science.PeData.Y[0, 1] = 0;
            G_Science.PeData.Y[0, 2] = 0;
            G_Science.PeData.Y[0, 3] = 0;

            // Set Various Other Properties ///
            G_Science.PeColor.BitmapGradientMode = false;
            //G_Science.PeColor.QuickStyle = QuickStyle.LightShadow;
            //G_Science.PeGrid.Option.CustomGridNumbersX = true;

            //Zoom...
            
            //G_Science.PeUserInterface.Allow.Zooming = AllowZooming.Horizontal;
            //G_Science.PeUserInterface.Scrollbar.ScrollingHorzZoom = true;
            //G_Science.PeGrid.Zoom.Mode = true;
            //G_Science.PeGrid.Configure.ManualScaleControlTX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;
            //G_Science.PeGrid.Configure.ManualScaleControlX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;


            // Set various properties //
            G_Science.PeString.MainTitle = title;
            G_Science.PeString.SubTitle = "";
            G_Science.PeUserInterface.Dialog.RandomPointsToExport = false;
            G_Science.PeUserInterface.Allow.FocalRect = false;
            G_Science.PePlot.Allow.Bar = false;
            G_Science.PeUserInterface.Allow.Popup = false;
            G_Science.PeConfigure.PrepareImages = true;
            G_Science.PeConfigure.CacheBmp = true;
            G_Science.PeFont.Fixed = true;
            //G_Science.PeColor.SubsetColors[0] = Color.FromArgb(0, 0, 198);

            G_Science.PePlot.Option.GradientBars = 8;
            G_Science.PeConfigure.TextShadows = TextShadows.BoldText;
            G_Science.PeFont.MainTitle.Bold = true;
            G_Science.PeFont.SubTitle.Bold = true;
            G_Science.PeFont.Label.Bold = true;
            G_Science.PePlot.Option.LineShadows = false;
            G_Science.PeFont.FontSize = FontSize.Medium;

            // Improves Metafile Export //
            G_Science.PeSpecial.DpiX = _bufferSize;
            G_Science.PeSpecial.DpiY = 600;

            G_Science.PeConfigure.RenderEngine = RenderEngine.Hybrid;
            G_Science.PeConfigure.AntiAliasText = false;

            G_Science.PeFunction.ReinitializeResetImage();

            G_Science.PePlot.PointSize =  PointSize.Medium;
            

            if (_lazyRefresh==false) RefreshNow();
        }
        private void SetBlueBackWhiteLine()
        {
            G_Science.BackColor = Color.FromArgb(0, 0, 40); //그래프의 뒷면 색깔.

            G_Science.PeColor.AxisBackColor = Color.FromArgb(0, 0, 0x40); //그리드 나타는 그래프영역의 뒷면색깔
            G_Science.PeColor.ViewingStyle = Gigasoft.ProEssentials.Enums.ViewingStyle.Color;
            G_Science.PeColor.XAxis = Color.White; //기준선(x선)과 기준선에 표시되는 라벨의 색
            G_Science.PeColor.YAxis = Color.White;

            G_Science.PeColor.GraphForeground = Color.FromArgb(0x55, 0x55, 0x55); //그래프 gird Line

            //G_Science.ForeColor = Color.Tomato;
            G_Science.PeFont.SizeLegendCntl = 1.2F;
            G_Science.PeConfigure.BorderTypes = TABorder.SingleLine;
            G_Science.ForeColor = Color.White;
            G_Science.PeLegend.AnnotationColor[0] = Color.White; //하나만 정해도 나머지 모두 지정된다.
            //G_Science.PeFont.DpiY = 6;
            //G_Science.PeFont.DpiX = 3;
            String[] strArr = new String[] { "test1", "test2", "test3", "test4" };

            G_Science.PeConfigure.AntiAliasGraphics = true;
            //U_X_AxisMode = XAxisMode.SecondMode;

            /*
            for (int i = 0; i < U_Subsets; i++)
            {
                //G_Science.PeAnnotation.Axis.XText[0] = "test1";//모름
                
               // G_Science.PeString.TruncateXAxisLabels = true;
               // G_Science.PeString.TXAxisLabel = "test1";

                G_Science.PeString.MultiSubTitles[i] = i.ToString();// strArr[i];   
                if (U_SubsetTitles.Count() == U_Subsets) G_Science.PeString.SubsetLabels[i] = U_SubsetTitles[i];// strArr[i]; //범례에서 나타나는 각 라인의 타이틀.
                //G_Science.PeAnnotation.Graph.Text[i] = strArr[i];
                //G_Science.PeLegend.AnnotationText[i] = strArr[i];
                //G_Science.PeLegend.SubsetColors[i] = Color.White;
                G_Science.PeLegend.AnnotationText[i] = i.ToString();
                G_Science.PeLegend.SubsetsToLegend[i] = i;
                if(_lineStyles.Count()==U_Subsets) G_Science.PeLegend.SubsetLineTypes[i] = (Gigasoft.ProEssentials.Enums.LineType)_lineStyles[i]; //범례의 라인 타입.
                //G_Science.PeGrid.AxisBorderType = AxisBorderType.NoBorder;
            }
            */
            G_Science.PeUserInterface.Menu.ExportDialog = MenuControl.Hide;

            #region DataCross
            G_Science.PeUserInterface.Cursor.Mode = CursorMode.DataSquare;

            //ManualXScale
            //G_Science.PeGrid.Zoom.Mode = true;
            //G_Science.PeGrid.Configure.ManualScaleControlTX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;
            //G_Science.PeGrid.Configure.ManualScaleControlX = Gigasoft.ProEssentials.Enums.ManualScaleControl.MinMax;
            //G_Science.PeGrid.Zoom.Mode = true;


            // Help see data points //
            G_Science.PePlot.MarkDataPoints = true;

            // This will allow you to move cursor by clicking data point //
            G_Science.PeUserInterface.Cursor.MouseCursorControl = true;
            G_Science.PeUserInterface.Allow.Popup = false;
            //G_Science.PeUserInterface.HotSpot.AxisLabel = true;
            G_Science.PeUserInterface.HotSpot.Data = true;
            G_Science.PeUserInterface.HotSpot.Graph = true;
            
            // Cursor prompting in top left corner //
            G_Science.PeUserInterface.Cursor.PromptTracking = false;
            G_Science.PeUserInterface.Cursor.PromptStyle = CursorPromptStyle.None;
            G_Science.PeUserInterface.Cursor.PromptLocation = CursorPromptLocation.Right;
            G_Science.PeUserInterface.Cursor.Point = 0;
            #endregion
            
            G_Science.PeUserInterface.Allow.CoordinatePrompting = false;
            

            G_Science.PeLegend.SimpleLine = true; //이렇게 하지 않으면 범례의 라인 주변에 border가 생김.

            G_Science.PeColor.Text = Color.White; //범례등 바깥에 표시되는 글자의 색깔.

            G_Science.PeConfigure.RenderEngine = Gigasoft.ProEssentials.Enums.RenderEngine.Hybrid;

            if (_lazyRefresh == false) RefreshNow();
        }
        /*
        void G_Science_PeGraphHotSpot(object sender, Gigasoft.ProEssentials.EventArg.GraphHotSpotEventArgs e)
        {
            //if (E_Click != null) E_Click(this, new GraphClickEventArgs(e.AxisIndex, e.DoubleClick, e.X, e.Y));
        }
         */
        public void setLineColors(params Color[] lineColors)
        {
            G_Science.PeColor.SubsetColors.CopyFrom(lineColors);
            if (_lazyRefresh == false) RefreshNow();
        }

        public void setLineStyles(params LineType[] lineStyles)
        {
            Gigasoft.ProEssentials.Enums.LineType[] arr = new Gigasoft.ProEssentials.Enums.LineType[lineStyles.Length];
            for (int i = 0; i < _lineStyles.Length; i++)
            {
                arr[i] = (Gigasoft.ProEssentials.Enums.LineType)_lineStyles[i];
            }

            G_Science.PePlot.SubsetLineTypes.CopyFrom(arr);
        }

        public void ClearBuffer()
        {
            for (int i = 0; i < _subsets; i++)
            {
                _graphData[i].Clear();
                
                //if (_graphDataX != null && _graphDataX.Length < i && _graphDataX[i].Count > 0) _graphDataX[i].Clear();
                if (_graphDataX != null && _graphDataX.Length > i && _graphDataX[i].Count > 0) _graphDataX[i].Clear();
                

            }

            _relatedData.Clear();
            _graphXValue.Clear();
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
            if (E_SnapShotClicked != null) E_SnapShotClicked(this, new SnapShotEventArgs(G_Science,format));
        }
    }



}
