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

namespace GraphModules
{
    public partial class SimpleRealtimeGraph : UserControl
    {
        public SimpleRealtimeGraph()
        {
            InitializeComponent();
            SetBlueBackWhiteLine(-100);
            setGraph(60, -1, 2);
        }

        public SimpleRealtimeGraph(int points, int min, int max, double nulldata)
        {
            InitializeComponent();
            SetBlueBackWhiteLine(nulldata);
            setGraph(points, min, max);
        }

        void setGraph(int points, int min, int max)
        {
            Pesgo1.PeData.Subsets = 1;
            Pesgo1.PeData.Points = points;

            // Set Manual Y scale //
            Pesgo1.PeGrid.Configure.ManualScaleControlY = ManualScaleControl.MinMax;
            Pesgo1.PeGrid.Configure.ManualMinY = min;
            Pesgo1.PeGrid.Configure.ManualMaxY = max;

            // Set Manual X scale//
            Pesgo1.PeGrid.Configure.ManualScaleControlX = ManualScaleControl.MinMax;
            Pesgo1.PeGrid.Configure.ManualMinX = 0;
            Pesgo1.PeGrid.Configure.ManualMaxX = points;

            // Clear out default data //
            Pesgo1.PeData.X[0, 0] = 0;
            Pesgo1.PeData.X[0, 1] = 0;
            Pesgo1.PeData.X[0, 2] = 0;
            Pesgo1.PeData.X[0, 3] = 0;
            Pesgo1.PeData.Y[0, 0] = 0;
            Pesgo1.PeData.Y[0, 1] = 0;
            Pesgo1.PeData.Y[0, 2] = 0;
            Pesgo1.PeData.Y[0, 3] = 0;

            // Set Various Other Properties ///
            Pesgo1.PeColor.BitmapGradientMode = false;
            //Pesgo1.PeColor.QuickStyle = QuickStyle.MediumInset;

            // Set various properties //
            //Pesgo1.PeString.MainTitle = "Scientific Real-Time Example";
            Pesgo1.PeString.SubTitle = "";
            Pesgo1.PeUserInterface.Dialog.RandomPointsToExport = true;
            Pesgo1.PeUserInterface.Allow.FocalRect = false;
            Pesgo1.PePlot.Allow.Bar = false;
            Pesgo1.PeUserInterface.Allow.Popup = false;
            Pesgo1.PeConfigure.PrepareImages = true;
            Pesgo1.PeConfigure.CacheBmp = true;
            Pesgo1.PeFont.Fixed = true;
            Pesgo1.PeColor.SubsetColors[0] = Color.FromArgb(0, 255, 0);

            Pesgo1.PePlot.Option.GradientBars = 8;
            Pesgo1.PeConfigure.TextShadows = TextShadows.BoldText;
            //Pesgo1.PeFont.MainTitle.Bold = true;
            //Pesgo1.PeFont.SubTitle.Bold = true;
            //Pesgo1.PeFont.Label.Bold = true;
            //Pesgo1.PePlot.Option.LineShadows = true;
            Pesgo1.PeFont.FontSize = FontSize.Medium;

            // Improves Metafile Export //
            Pesgo1.PeSpecial.DpiX = 600;
            Pesgo1.PeSpecial.DpiY = 600;

            Pesgo1.PeConfigure.RenderEngine = RenderEngine.Hybrid;
            Pesgo1.PeConfigure.AntiAliasText = true;



        }
        public new void Dispose(){
            
            Pesgo1.Dispose();

            GC.Collect();
            
        }
        private void SetBlueBackWhiteLine(double nulldata)
        {
            Pesgo1.BackColor = Color.FromArgb(0, 0, 40); //그래프의 뒷면 색깔.
            //Pesgo1.PeData.ScaleForYData = 0;
            Pesgo1.PeColor.AxisBackColor = Color.FromArgb(0, 0, 0x40); //그리드 나타는 그래프영역의 뒷면색깔
            //Pesgo1.PeColor.ViewingStyle = Gigasoft.ProEssentials.Enums.ViewingStyle.Color;
            Pesgo1.PeColor.XAxis = Color.White; //기준선(x선)과 기준선에 표시되는 라벨의 색
            Pesgo1.PePlot.Method = SGraphPlottingMethod.Step;
            Pesgo1.PeColor.GraphForeground = Color.FromArgb(0x55, 0x55, 0x55); //그래프 gird Line

            Pesgo1.ForeColor = Color.White;
            //Pesgo1.PeLegend.AnnotationColor[0] = Color.White; //하나만 정해도 나머지 모두 지정된다.
            //Pesgo1.PeColor.SubsetColors[0] = Color.Green;
            Pesgo1.PeData.NullDataValue = nulldata;

            Pesgo1.PeFunction.ReinitializeResetImage();

        }


        int m_nRealTimeCounter=0;
        public void setData(ulong newx, float newy, bool refreshNow=false)
        {

            // Append new values  //
            Gigasoft.ProEssentials.Api.PEvsetW(Pesgo1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendYData, newy, 1);
            Gigasoft.ProEssentials.Api.PEvsetW(Pesgo1.PeSpecial.HObject, Gigasoft.ProEssentials.DllProperties.AppendXData, newx, 1);

            // Increment counter ///
            m_nRealTimeCounter = m_nRealTimeCounter + 1;

            // Switch to AutoScaling x axis after receiving 100 data points ///				
            if (m_nRealTimeCounter == 1)
                Pesgo1.PeGrid.Configure.ManualScaleControlX = ManualScaleControl.None;


            if (refreshNow)
            {

                // Update image and force paint///
                if (this.InvokeRequired)
                {
                    voidfunc func = Refresh;
                    try
                    {
                        this.Invoke(func);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        Refresh();
                    }
                    catch { }
                }
            }
        }
        delegate void voidfunc();
        public new void Refresh(){
            Pesgo1.PeFunction.ReinitializeResetImage();
            Pesgo1.Refresh();
        }
    }
}
