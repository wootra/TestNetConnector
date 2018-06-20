using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;

namespace ReportTools
{
    public partial class ReportDialog : Form
    {
        public ReportTools.ReportViewerHandler_new ReportViewer1;
        public ReportTools.RdlcCreator ReportForm;

        public ReportDialog()
        {
            InitializeComponent();
            ReportForm = new RdlcCreator();
            //ReportForm.InitRdlc("", "DataSet1", "", "", "");

        }
        //ReportViewerHandler_new _rh;
        /*
        public void setReportDialog(String rdlcFile, String DataSetName = "DataSet1")
        {
            _rh = new ReportViewerHandler_new(components, this, DockStyle.Fill, "rv", DataSetName, rdlcFile);
            _rh.getReportVeiwer().Print += new CancelEventHandler(ReportViewer_Print);
        }


        void ReportViewer_Print(object sender, CancelEventArgs e)
        {
            System.Drawing.Printing.PrintDocument pdoc = new System.Drawing.Printing.PrintDocument();
        }

        private void ReportDialog_Load(object sender, EventArgs e)
        {

            //this.ReportViewer.RefreshReport();
            _rh.getReportVeiwer().RefreshReport();
        }
        */
        public void AddFields(int pageWidth, ICollection<String> Names, ICollection<int> columnWidths, ICollection<String> TitleTexts=null, ICollection<Type> types=null)
        {
            this.Width = 500;
            //float rate = 40.0f;
            //float wid = pageWidth / rate;
            ReportForm.setSize(20,21);
            int colWid = 0;
            int totalWid = 0;
            for (int i = 0; i < Names.Count; i++)
            {
                String txt = ((TitleTexts == null) ? Names.ElementAt(i).Replace(" ", "") /*.ToLower().Replace(" ", "")*/ : TitleTexts.ElementAt(i)).Replace(" ", "") /*.ToLower().Replace(" ", "")*/;
                if (columnWidths.ElementAt(i) <= 2) continue;
                //    txt = ""+i;
                colWid = (columnWidths.ElementAt(i)<0)? 100 : columnWidths.ElementAt(i);// 1:(columnWidths.ElementAt(i))/rate;
                totalWid += colWid;
                ReportForm.addFields(
                    Names.ElementAt(i).Replace(" ","") /*.ToLower().Replace(" ","")*/,
                    (types == null) ? typeof(String) : types.ElementAt(i),
                    txt,
                    colWid);
                
            }

        }

        public void AddARow(object[] items, Dictionary<int, String> colors = null)
        {
            ReportForm.addValues(items, colors);

        }
        public void AddARow(List<object> items, Dictionary<int,String> colors=null)
        {
            ReportForm.addValues(items, colors);
        }
        public void AddARow(Dictionary<String, object> items, Dictionary<int, String> colors = null)
        {
            ReportForm.addValues(items, colors);
        }
        
        public new void Show(String Title, String name, String StartDate, String EndDate, String subTitle="...")
        {
            
            //ReportForm.InitRdlc();
            ReportForm.SetCommonField(StartDate, EndDate, name, Title, subTitle);
            Show();
        }
        public void AddTitle(String title, int fontSize)
        {
            ReportForm.AddTitles(title, fontSize);
            
        }
        public void AddInfo(String name, String info)
        {
            ReportForm.AddInfos(name, info);

        }
        public void SetTitle(IDictionary<String,int> titles)
        {
            ReportForm.SetTitles(titles);

        }
        public void SetInfo(IDictionary<String,string> infos)
        {
            ReportForm.SetInfos(infos);

        }

        /// <summary>
        /// 레포트파일을 만들어 보여준다.
        /// </summary>
        /// <param name="reportFileName"></param>
        /// <param name="makeNew"></param>
        public new void Show(string reportFileName="report.rdlc", bool makeNew=true)
        {

            //ReportForm.InitRdlc();
            if(makeNew) ReportForm.MakeRdlc(reportFileName);

            ReportViewer1 = new ReportViewerHandler_new
                (components,
                this,
                DockStyle.Fill,
                "Report",
                "DataSet1",
                reportFileName);
            


            ReportViewer1.MakeReportViewer();
            BindingSource src = new BindingSource();

            ReportViewer1.Easy_addBindingSource(src);

            base.Show();
            ReportViewer1.getReportVeiwer().RefreshReport();
        }
       

    }
}
