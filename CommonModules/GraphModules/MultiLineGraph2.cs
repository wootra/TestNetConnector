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
    public partial class MultiLineGraph2 : UserControl
    {
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler Click;

        public MultiLineGraph2()
        {
            InitializeComponent();

            U_IsBufferedGraph = true;

            U_Subsets = 1;
            U_BufferSize = 100;
            U_RefreshCount = 20;
           
            U_LineColor = _lineColor;

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

        Queue<float>[] _graphData=new Queue<float>[1];


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

        public Boolean _isBufferedGraph = false;
        [DefaultValue(Style.BlueBackWhiteLine)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Boolean U_IsBufferedGraph
        {
            get { return _isBufferedGraph; }
            set
            {
                _isBufferedGraph = value;
                if (_graphData == null)
                {
                    for (int i = 0; i < _graphData.Length; i++)
                    {
                        _graphData[i] = new Queue<float>(_bufferSize);
                    }
                }
            }
        }

        int _subsets = 1;
        [DefaultValue(1)]
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public int U_Subsets
        {
            get { return _subsets; }
            set
            {
                _subsets = value;
                _graphData = new Queue<float>[_subsets];
                G_Science.PeData.Subsets = value;
                for (int i = 0; i < _subsets; i++)
                {
                    _graphData[i] = new Queue<float>(_bufferSize);
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
                G_Science.PeSpecial.DpiX = value;


                if (_isBufferedGraph)
                {
                    for (int i = 0; i < _subsets; i++)
                    {
                        _graphData[i] = new Queue<float>(value);
                    }
                }
            }
        }

        Color[] _lineColor = new Color[]{Color.Red,Color.Blue,Color.Green,Color.Yellow};
        [BrowsableAttribute(true)]
        [Bindable(true)]
        public Color[] U_LineColor
        {
            get { return _lineColor; }
            set
            {
                _lineColor = value;
                setLineColorArray(value);
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

        private void SetBlueBackWhiteLine()
        {

            G_Science.BackColor = Color.FromArgb(0, 0, 40); //그래프의 뒷면 색깔.
            Gigasoft.ProEssentials.SGraphColorItems color = G_Science.PeColor;

            color.AxisBackColor = Color.FromArgb(0, 0, 0x40); //그리드 나타는 그래프영역의 뒷면색깔
            color.ViewingStyle = Gigasoft.ProEssentials.Enums.ViewingStyle.Color;
            color.XAxis = Color.White; //기준선(x선)과 기준선에 표시되는 라벨의 색
            color.YAxis = Color.White;
            color.GraphForeground = Color.FromArgb(0x55, 0x55, 0x55); //그래프 gird Line

            Gigasoft.ProEssentials.SGraphConfigureItems conf = G_Science.PeConfigure;
            conf.RenderEngine = Gigasoft.ProEssentials.Enums.RenderEngine.Hybrid;

            G_Science.Refresh();
        }

        public void setData(int x, params double[] y)
        {
            // New y value and x value //
            int lineNum = y.Length;
            float[] ys = new float[lineNum];
            float[] xs = new float[lineNum];

            for (int i = 0; i < lineNum; i++)
            {
                ys[i] = (float)y[i];
                xs[i] = x;
                // G_Science.PeData.X[i,(int)x] = x;
                // G_Science.PeData.Y[i,(int)x]= ys[i];
            }
            if (_graphData == null || _graphData[0] == null) return;
            if (_graphData[0].Count == _bufferSize) //queue가 가득 찼다면
            {
                
                for (int loop = 0; loop < _refreshCount; loop++)
                {
                    for (int i = 0; i < _subsets; i++)
                    {
                        _graphData[i].Dequeue();
                        G_Science.PeData.X[i, _bufferSize - loop] = _bufferSize - _refreshCount;
                        G_Science.PeData.Y[i, _bufferSize - loop] = 0;
                    }
                }
            }
            for (int i = 0; i < y.Length; i++)
            {
                _graphData[i].Enqueue((float)y[i]);
            }

            //BindingSource source = new BindingSource();
            for (int subset = 0; subset < _subsets; subset++)
            {
                float[] a = _graphData[subset].ToArray();
                for (int j = 0; j < a.Length; j++)
                {
                    G_Science.PeData.X[subset, j] = j;
                    G_Science.PeData.Y[subset, j] = a[j];
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
            G_Science.PeGrid.Configure.ManualScaleControlY = ManualScaleControl.None;
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
        public void setLineColorArray(Color[] lineColors)
        {
            G_Science.PeColor.SubsetColors.CopyFrom(lineColors);
            G_Science.Refresh();
        }
        public void ClearBuffer()
        {
            for (int i = 0; i < _subsets; i++)
            {
                _graphData[i].Clear();
            }
        }
    }


}
