using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using HtmlReportMaker;

namespace HtmlReportMaker
{
    public partial class ReportDialog : Form
    {
        //public ReportTools.ReportViewerHandler_new ReportViewer1;
        //public ReportTools.RdlcCreator ReportForm;
        HtmlReport _report;
        HtmlTable _table;
        HtmlTable _headerTable;
        HtmlTable _titleTable;
        string _initDir;
        public ReportDialog(string initDirToSave=null)
        {
            InitializeComponent();
            _report = new HtmlReport();
            _titleTable = _report.AddTable(new int[] { -1 });


            _headerTable = _report.AddTable(new int[] { -1,200,300 });
            _headerTable.FontSize = 10;

            HtmlTable marginTable = _report.AddTable(new int[] { -1 });//margin between title and data table
            HtmlTableRow row = marginTable.AddRow(new object[]{ ""},null,20);
            row.FontSize = 10;
            row.Cell(0).ForeColor = Color.White;


            if (initDirToSave == null) _initDir = Directory.GetCurrentDirectory() + "\\SavedReports";
            else _initDir = initDirToSave;
            if (Directory.Exists(_initDir) == false) Directory.CreateDirectory(_initDir);
            
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Names"></param>
        /// <param name="columnWidths"></param>
        /// /// <param name="valueFontSize"></param>
        /// <param name="rowFontSize">-1이면 valueFontSize와 같다.</param>
        public void AddFields(ICollection<object> Names, ICollection<int> columnWidths, ICollection<object> Texts=null, int valueFontSize=10, int headerFontSize = -1)
        {
            
            //float rate = 40.0f;
            //float wid = pageWidth / rate;
            _table = _report.AddTable(columnWidths);
            
            if (headerFontSize < 0) headerFontSize = valueFontSize;
            int rate = 1;
            int totalWidth = 0;
            int colIndex = 0;
            foreach (int wid in columnWidths)
            {
                totalWidth += wid/rate;
                _table.Widths[colIndex] = wid / rate;
                colIndex++;
            }

            this.Width = totalWidth+20;

            _titleTable.Widths[0] = totalWidth / rate;
            _titleTable.Width = totalWidth / rate;
            _headerTable.Widths[0] = totalWidth / rate - 300;
            _headerTable.Widths[1] = 100;
            _headerTable.Widths[2] = 200;
            _titleTable.Width = totalWidth / rate;
            _headerTable.Width = totalWidth / rate;
            _table.BorderWidths = new Margins(1, 1, 1, 1);
            _table.BorderColor = Color.Black;
            HtmlTableRow header;
            if (Texts == null)
            {
                header = _table.AddHeaderRow(Names);
            }
            else
            {
                header = _table.AddHeaderRow(Texts);
            }
             
            header.FontSize = headerFontSize;
            header.BorderWidths = new Margins(0, 0, 0, 1);
            header.BorderColor = Color.Gray;
            _table.FontSize = valueFontSize;
            header.BackColor = Color.LightGreen;
            header.Height = 50;
        }

        public void AddARow(ICollection<object> items, Dictionary<int, Color> colors = null)
        {
            HtmlTableRow row = _table.AddRow(items);
            if (colors != null)
            {
                foreach (int i in colors.Keys)
                {
                    row.Cell(i).ForeColor = colors[i];
                }
            }
            if (_table.Rows.Count > 1)
            {
                row.BackColor = (_table.Rows.Count % 2 == 0) ? Color.LightGray : Color.Empty;
                row.BorderWidths = new Margins(0, 0, 0, 1);
                row.BorderColor = Color.LightGray;
            }
        }

        public void AddACommentRow(string comment)
        {
            HtmlTableRow row = _table.AddRow();
            
            row.Cell(0).Span = _table.Widths.Count;
            row.Cell(0).Content = comment;
            row.Cell(0).CellAlign = CellAligns.Left;
            if (_table.Rows.Count > 1)
            {
                row.BackColor = (_table.Rows.Count % 2 == 0) ? Color.LightGray : Color.Empty;
                row.BorderWidths = new Margins(0, 0, 0, 1);
                row.BorderColor = Color.LightGray;
            }
        }
        
        public void AddARow(Dictionary<String, object> items, Dictionary<int, String> colors = null)
        {
            
            HtmlTableRow row = _table.AddRow();
            foreach (string key in items.Keys)
            {
                int index = _table.GetFieldIndex(key);
                if (index >= 0)
                {
                    bool isHeaderCell;
                    HtmlTableCell cell = row.Cell(index, out isHeaderCell);
                    cell.Content = items[key];
                    cell.ForeColor = Color.FromName(colors[index]);
                }
            }
            if (_table.Rows.Count > 1)
            {
                row.BackColor = (_table.Rows.Count % 2 == 0) ? Color.LightGray : Color.Empty;
                row.BorderWidths = new Margins(0, 0, 0, 1);
                row.BorderColor = Color.LightGray;
            }
            //ReportForm.addValues(items, colors);
        }
        
        public new void Show()
        {
            ReportView.Navigate("about:blank");
            ReportView.Document.Write(_report.GetHtml());
            //ReportForm.InitRdlc();

            base.Show();
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
            HtmlTableRow row = _headerTable.AddRow(new string[] { "", name, info }, null, 40);

            row.Cell(1).BorderWidths = new Margins(0, 0, 0, 1);
            row.Cell(2).BorderWidths = new Margins(0, 0, 0, 1);

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

        private void B_ToolPrint_Click(object sender, EventArgs e)
        {
            ReportView.ShowPageSetupDialog();
            ReportView.ShowPrintDialog();
        }

        private void B_ToolHtml_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = _initDir;
            sf.Filter = "ReportHtml (.html)|*.html|All Files (*.*)|*.*";
            sf.FilterIndex = 0;
            DialogResult result = sf.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Abort || result == System.Windows.Forms.DialogResult.Cancel) return;

            string file = sf.FileName;
            File.WriteAllText(file, _report.GetHtml());
        }


    }
}
