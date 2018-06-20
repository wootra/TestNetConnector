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
using System.Runtime.InteropServices;

namespace GraphModules
{
    public partial class MultiLineGraph : UserControl
    {
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler Click;

        public MultiLineGraph()
        {
            InitializeComponent();

            U_Subsets = _subsets;
            U_BufferSize = _bufferSize;
            U_RefreshCount = _refreshCount;

            U_LineColors = _lineColors;
            U_LineStyles = _lineStyles;
            U_SubsetTitles = _subSetTitles;

            U_BackStyle = Style.BlueBackWhiteLine;

            G_Science.MouseDown += new MouseEventHandler(G_Science_MouseDown);
            G_Science.MouseUp += new MouseEventHandler(G_Science_MouseUp);
            G_Science.Click += new EventHandler(G_Science_Click);
            
            setGraphInit();
        }

        
        void G_Science_Click(object sender, EventArgs e)
        {
            if (Click != null) Click(this, e);
        }

        void G_Science_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseDown != null) MouseDown(this, e);
        }

        void G_Science_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseUp != null) MouseUp(this, e);
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

        
        /*
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        */
        List<float>[] _graphData = new List<float>[1];
        List<float>[] _graphDataX = new List<float>[1];
        

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
                }
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

        int _pointsOfGraph = 600;
        public int U_PointsOfGraph
        {
            get { return _pointsOfGraph; }
            set
            {
                _pointsOfGraph = value;
                G_Science.PeData.Points = _pointsOfGraph;

            }
        }
        int _bufferSize = 600;
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_BufferSize
        {
            get { return _bufferSize; }
            set
            {
                _bufferSize = value;

                


                    for (int i = 0; i < _subsets; i++)
                    {
                        _graphData[i] = new List<float>(value);
                        _graphDataX[i] = new List<float>(value);
                    }

            }
        }

        Color[] _lineColors = new Color[]{Color.Red,Color.Blue,Color.Green,Color.Yellow};
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Color[] U_LineColors
        {
            get { return _lineColors; }
            set
            {
                _lineColors = value;
                G_Science.PeColor.SubsetColors.CopyFrom(value);
                G_Science.Refresh();
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

        LineType[] _lineStyles = new LineType[] { LineType.ThinSolid, LineType.ThinSolid, LineType.ThinSolid, LineType.ThinSolid };
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
                G_Science.Refresh();
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

                G_Science.Refresh();
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
                for (int i = 0; i < value.Length; i++)
                {
                    G_Science.PeString.SubsetLabels[i] = value[i];
                }
                G_Science.Refresh();
            }
        }
        double _nullDataValue = float.MinValue;
        public double U_NullDataValue
        {
            get
            {
                return _nullDataValue;
            }
            set
            {
                _nullDataValue = value;
                G_Science.PeData.NullDataValue = value;
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
                G_Science.Refresh();
            }
        }

        Boolean _showLegend = false;
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Boolean U_LegendShow
        {
            get { return G_Science.PeLegend.Show; }
            set
            {
                G_Science.PeLegend.Show = value;
                G_Science.Refresh();
            }
        }
        bool _isStep = false;
        public Boolean U_IsStep
        {
            get
            {
                return _isStep;
            }
            set
            {
                _isStep = value;
                if (value) G_Science.PePlot.Method = SGraphPlottingMethod.Step;
                else G_Science.PePlot.Method = SGraphPlottingMethod.Line;
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
        public ManualScaleControl U_ManualScaleY
        {
            get { return (ManualScaleControl)G_Science.PeGrid.Configure.ManualScaleControlY; }
            set
            {
                G_Science.PeGrid.Configure.ManualScaleControlY = (Gigasoft.ProEssentials.Enums.ManualScaleControl)value;
                G_Science.Refresh();
            }
        }


        private void SetBlueBackWhiteLine()
        {
            G_Science.PeLegend.Show = _showLegend;
            G_Science.BackColor = Color.FromArgb(0, 0, 40); //그래프의 뒷면 색깔.

            G_Science.PeColor.AxisBackColor = Color.FromArgb(0, 0, 0x40); //그리드 나타는 그래프영역의 뒷면색깔
            G_Science.PeColor.ViewingStyle = Gigasoft.ProEssentials.Enums.ViewingStyle.Color;
            G_Science.PeColor.XAxis = Color.White; //기준선(x선)과 기준선에 표시되는 라벨의 색
            G_Science.PeColor.YAxis = Color.White;

            G_Science.PeColor.GraphForeground = Color.FromArgb(0x55, 0x55, 0x55); //그래프 gird Line

            //G_Science.ForeColor = Color.Tomato;
            G_Science.PeFont.SizeLegendCntl = 1.2F;
            G_Science.PeConfigure.BorderTypes = TABorder.SingleLine;
            //G_Science.PeFont.DpiY = 6;
            //G_Science.PeFont.DpiX = 3;
            String[] strArr = new String[]{"test1","test2","test3","test4"};
            
            G_Science.PeConfigure.AntiAliasGraphics = true;

            for (int i = 0; i < 4; i++)
            {
                //G_Science.PeAnnotation.Axis.XText[0] = "test1";//모름
                
               // G_Science.PeString.TruncateXAxisLabels = true;
               // G_Science.PeString.TXAxisLabel = "test1";
                
                //G_Science.PeString.MultiSubTitles[i] = strArr[i];   
                //G_Science.PeString.SubsetLabels[i] = strArr[i]; //범례에서 나타나는 각 라인의 타이틀.
                //G_Science.PeAnnotation.Graph.Text[i] = strArr[i];
                //G_Science.PeLegend.AnnotationText[i] = strArr[i];
                //G_Science.PeLegend.SubsetColors[i] = Color.White;
                G_Science.PeLegend.SubsetsToLegend[i] = i;
                G_Science.PeLegend.SubsetLineTypes[i] = (Gigasoft.ProEssentials.Enums.LineType)_lineStyles[i]; //범례의 라인 타입.
                //G_Science.PeGrid.AxisBorderType = AxisBorderType.NoBorder;
            }
            G_Science.PeLegend.SimpleLine = true; //이렇게 하지 않으면 범례의 라인 주변에 border가 생김.
            G_Science.PeColor.Text = Color.White; //범례등 바깥에 표시되는 글자의 색깔.

            G_Science.PeConfigure.RenderEngine = Gigasoft.ProEssentials.Enums.RenderEngine.Hybrid;

            G_Science.Refresh();
        }
        public void setData2D(double x, double y, int subsetIndex = 0)
        {
            // New y value and x value //
            G_Science.PePlot.MarkDataPoints = true;

            if (_graphData == null || _graphData[0] == null) return;

            if (_graphData[0].Count == _bufferSize) //queue가 가득 찼다면
            {

                _graphData[subsetIndex].RemoveRange(0, _refreshCount);
                _graphDataX[subsetIndex].RemoveRange(0, _refreshCount);
                G_Science.PeData.X.Clear();
                G_Science.PeData.Y.Clear();
            }
            _graphDataX[subsetIndex].Add((float)x);
            _graphData[subsetIndex].Add((float)y);
            //BindingSource source = new BindingSource();
            int samplingSize = 1;

            int sizeOfData = _graphData[subsetIndex].Count;
            samplingSize = 1;
            for (int i = 1; i < sizeOfData; i++) //sampling할 size를 가져온다.
            {
                if ((sizeOfData / i) < _pointsOfGraph) //일단 전체를 200으로 max를 잡았다.
                {
                    samplingSize = i;
                    break;
                }
                else if((sizeOfData/i) == _pointsOfGraph)
                {
                    int test = 0;
                }
            }
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


            // Append new values  //
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, ys[0], 1);
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, xs[0], 1);

            //}
            // Update image and force paint //
            //G_Science.PeFunction.ReinitializeResetImage();

            if (sizeOfData % samplingSize == 0)
            {
                G_Science.PeFunction.ReinitializeResetImage();
                G_Science.Refresh();
            }

            
        }
        public void setData(ulong x, params double[] y)
        {
            // New y value and x value //

            if (_graphData == null || _graphData[0] == null) return;
            if (_graphData[0].Count == _bufferSize) //queue가 가득 찼다면
            {
                    for (int subsetIndex = 0; subsetIndex < _subsets; subsetIndex++)
                    {
                        _graphData[subsetIndex].RemoveRange(0, _refreshCount);
                    }
            }
            for (int subset = 0; subset < y.Length; subset++)
            {
                _graphData[subset].Add((float)y[subset]);
            }

            //BindingSource source = new BindingSource();
            int sampledIndex = 0;
            for (int subset = 0; subset < _subsets; subset++)
            {

                int sizeOfData = _graphData[subset].Count;
                int samplingSize = 1;
                for (int i = 1; i < sizeOfData; i++) //sampling할 size를 가져온다.
                {
                    if ((sizeOfData / i) < _pointsOfGraph) 
                    {
                        samplingSize = i;
                        break;
                    }
                }
                sampledIndex = 0;
                for (int j = 0; j < sizeOfData; j = j + samplingSize) //samplingSize만큼 이동한다.
                {

                    G_Science.PeData.X[subset, sampledIndex] = (int)j;
                    G_Science.PeData.Y[subset, sampledIndex] = _graphData[subset][j];
                    sampledIndex++;

                }
                for (int loop = sampledIndex; loop < _pointsOfGraph; loop++)
                {

                    G_Science.PeData.X[subset, loop] = loop;
                    G_Science.PeData.Y[subset, loop] = 0;
                }
            }


            // Append new values  //
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, ys[0], 1);
            //Gigasoft.ProEssentials.Api.PEvsetW(graph.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, xs[0], 1);

            //}
            // Update image and force paint //
            //G_Science.PeFunction.ReinitializeResetImage();
            //G_Science.Refresh();
        }

        public void setGraphInit(String title = "", double min = -10, double max = 50)
        {
            //! Chart fills 100 points autoscaling x axis is it
            //! is filled.  Once 100 point have been passed, the
            //! chart then acts as a strip chart.

            G_Science.PeData.Subsets = U_Subsets;
            //G_Science.PeData.AppendToEnd = true;
            G_Science.PeAnnotation.Graph.TextSize = 11;
            G_Science.PeData.NullDataValue = 0;
            G_Science.PeData.Points = _bufferSize;

            //G_Science.PeUserInterface.Menu.GridLine = MenuControl.Show;
            //G_Science.PeData.SubsetsToShow[0] = 1;
            //G_Science.PeData.UsingYDataii = false;

            //G_Science.PeData.SubsetsToShow[1] = 1;
            //G_Science.PeData.SubsetsToShow[2] = 2;

            // Set Manual Y scale //
            //G_Science.PeGrid.Configure.ManualScaleControlY = ManualScaleControl.MinMax;
            G_Science.PeGrid.Configure.ManualScaleControlY = (Gigasoft.ProEssentials.Enums.ManualScaleControl) ManualScaleControl.None;
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

            // Set various properties //
            G_Science.PeString.MainTitle = title;
            G_Science.PeString.SubTitle = "";
            G_Science.PeUserInterface.Dialog.RandomPointsToExport = true;
            G_Science.PeUserInterface.Allow.FocalRect = false;
            G_Science.PePlot.Allow.Bar = false;
            G_Science.PeUserInterface.Allow.Popup = false;
            G_Science.PeConfigure.PrepareImages = true;
            G_Science.PeConfigure.CacheBmp = true;
            G_Science.PeFont.Fixed = false;
            //G_Science.PeColor.SubsetColors[0] = Color.FromArgb(0, 0, 198);

            G_Science.PePlot.Option.GradientBars = 8;
            G_Science.PeConfigure.TextShadows = TextShadows.BoldText;
            G_Science.PeFont.MainTitle.Bold = true;
            G_Science.PeFont.SubTitle.Bold = true;
            G_Science.PeFont.Label.Bold = true;
            G_Science.PePlot.Option.LineShadows = true;
            G_Science.PeFont.FontSize = FontSize.Medium;

            // Improves Metafile Export //
            G_Science.PeSpecial.DpiX = _bufferSize;
            G_Science.PeSpecial.DpiY = 600;

            G_Science.PeConfigure.RenderEngine = RenderEngine.Hybrid;
            G_Science.PeConfigure.AntiAliasText = true;

            G_Science.PeFunction.ReinitializeResetImage();
            G_Science.Refresh();

            
        }

        public void setLineColors(params Color[] lineColors)
        {
            G_Science.PeColor.SubsetColors.CopyFrom(lineColors);
            G_Science.Refresh();
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
                if (_graphDataX != null && _graphDataX.Length < i && _graphDataX[i].Count > 0) _graphDataX[i].Clear();
            }
        }
    }


}
