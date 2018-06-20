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
using HtmlReportMaker;

namespace ReportTools
{
    public partial class ReportDialog : Form
    {
        //public ReportTools.ReportViewerHandler_new ReportViewer1;
        //public ReportTools.RdlcCreator ReportForm;
        HtmlReport _report;
        HtmlTable _table;
        HtmlTable _headerTable;
        HtmlTable _titleTable;

        public ReportDialog()
        {
            InitializeComponent();
            _report = new HtmlReport();
            _titleTable = _report.AddTable(new int[] { 500 });
            
            _headerTable = _report.AddTable(new int[] { 500,100,100 });
            
          //  ReportForm = new RdlcCreator();
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
        public void AddFields(int pageWidth, ICollection<object> Names, ICollection<int> columnWidths, ICollection<String> TitleTexts=null, ICollection<Type> types=null)
        {
            this.Width = 500;
            //float rate = 40.0f;
            //float wid = pageWidth / rate;
            _table = _report.AddTable(columnWidths);
            int totalWidth = 0;
            foreach (int wid in columnWidths)
            {
                totalWidth += wid;

            }
            _titleTable.Widths = new int[] { totalWidth };
            _headerTable.Widths = new int[] { totalWidth - 200, 100, 100 };
            _table.BorderWidth = 1;
            _table.BorderColor = Color.Black;

            HtmlTableRow header = _table.AddRow(Names);
            header.BackColor = Color.Green;
            header.Height = 50;
        }

        public void AddARow(object[] items, Dictionary<int, String> colors = null)
        {
            HtmlTableRow row = _table.AddRow(items);
            

        }
        public void AddARow(List<object> items, Dictionary<int,String> colors=null)
        {
            HtmlTableRow row = _table.AddRow(items);
            
            //ReportForm.addValues(items, colors);
        }
        public void AddARow(Dictionary<String, object> items, Dictionary<int, String> colors = null)
        {
            
            HtmlTableRow row = _table.AddRow();
            foreach (string key in items.Keys)
            {
                int index = _table.GetField(key);
                if (index >= 0)
                {
                    bool isHeaderCell;
                    HtmlTableCell cell = row.Cell(index, out isHeaderCell);
                    cell.Content = items[key];
                    cell.BackColor = Color.FromName(colors[index]);
                }
            }
            //ReportForm.addValues(items, colors);
        }
        
        public new void Show(String Title, String name, String StartDate, String EndDate, String subTitle="...")
        {
            ReportView.DocumentText = _report.GetHtml();
            //ReportForm.InitRdlc();
            
            Show();
        }

        
        public void AddTitle(String title, int fontSize)
        {
            HtmlTableRow row = _titleTable.AddRow(new object[] { title },null, fontSize * 2);
            bool headerCell;
            HtmlTableCell cell = row.Cell(0, out headerCell);
            cell.FontSize = fontSize;
            //ReportForm.AddTitles(title, fontSize);
            
        }
        public void AddInfo(String name, String info)
        {
            _headerTable.AddRow(new string[] { "", name, info }, null, 40);

        }
        public void SetTitle(IDictionary<String,int> titles)
        {
            _titleTable.Rows.Clear();
            foreach (string title in titles.Keys)
            {
                AddTitle(title, titles[title]);
            }

        }
        public void SetInfo(IDictionary<String,string> infos)
        {
            _headerTable.Rows.Clear();
            foreach (string title in infos.Keys)
            {
                AddInfo(title, infos[title]);
            }

        }


    }
}
