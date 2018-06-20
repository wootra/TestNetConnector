using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Microsoft.Reporting;
using System.Drawing.Printing;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace ReportTools.NotUsing
{
    public partial class ReportViewerHandler
    {
        private ReportPrintDocument _reportPrintDoc;
        public enum ExportFormat { PDF = 0, Excel = 1, Word =2, JPG = 3 };
        public String Title { get; set; }
        public String Name { get; set; }
        public ReportViewer _rv;
        public ReportDataSource _rSource = new ReportDataSource();
        public BindingSource _bSource = new BindingSource() ;

        /// <summary>
        /// 레포트뷰어를 셋팅한다.
        /// </summary>
        /// <param name="reportName">uniq name of reportviewer</param>
        /// <param name="sourceName"> EX> "TestRow" this is object name to be used as table row. If you have DataSet, you should enter the name of dataset.</param>
        /// <param name="parent"> parent control to add the report to.</param>
        /// <param name="dock">use DockStyle.Fill normally</param>
        /// <param name="rdlcNamespace"> ex> "WindowsFormsApplication1.Report1.rdlc" </param>
        public ReportViewerHandler(IContainer components, Control parent, DockStyle dock, String reportName, String sourceName, Type sourceType,String rdlcPath)
        {
            if (components != null) _bSource = new BindingSource(components);
            else _bSource = new BindingSource();
            ((System.ComponentModel.ISupportInitialize)(_bSource)).BeginInit();
            _rv = new ReportViewer();
            _rv.Dock = System.Windows.Forms.DockStyle.Fill;
            _rSource.Name = sourceName;
            _rSource.Value = _bSource;
            _bSource.DataSource = sourceType;
            _rv.LocalReport.DataSources.Add(_rSource);
            _rv.LocalReport.ReportPath = rdlcPath;
            _rv.Location = new System.Drawing.Point(0, 0);
            _rv.Name = reportName;
            ((System.ComponentModel.ISupportInitialize)(_bSource)).EndInit();
            parent.Controls.Add(_rv);
        }

        public ReportViewer getReportVeiwer() { return _rv; }
        public void Easy_howToUse()
        {
            Console.WriteLine("First, make .rdlc file. this is report design style file." +
                "you can make it from the tool, 'New Report Design (새 보고서 디자인) of ReportViewer" +
                "after you drop down on Designer Environment of Visual Studio");
            Console.WriteLine();
            Console.WriteLine("Second, bind the report design(ReportX.rdlc) file to this EasyReportViewer instance. use designer's menu.");
            Console.WriteLine("the menu is visible up right of ReportViewer instance on Designer.");
            Console.WriteLine("If you don't want to show this ReportViewer, use Hide() function of it.");
            Console.WriteLine();
            Console.WriteLine("Third, you can add real BindingSource instance with Easy_addBindingSource. Then you can See the Report.");
            Console.WriteLine("ENJOY!");
        }
        public void Easy_addBindingSource(BindingSource source){
            if (_rv.LocalReport.DataSources.Count == 0) _rv.LocalReport.DataSources.Add(new ReportDataSource());
            _rv.LocalReport.DataSources[0].Value = new BindingSource(source, "");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="path">default:null</param>
        /// <param name="isShowDialog">default=false</param>
        /// <param name="isShowFileAfterSaving">default=false</param>
        public void Easy_exportFile(ExportFormat fmt, String path, Boolean isShowDialog, Boolean isShowFileAfterSaving)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;
            // string deviceInfo;

            String expFormat = (fmt == ExportFormat.PDF) ? "PDF" : (fmt == ExportFormat.Excel) ? "Excel" : "JPG";

            byte[] bytes = _rv.LocalReport.Render(
            expFormat, null, out mimeType, out encoding, out extension,
            out streamids, out warnings);

            if (isShowDialog == true)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = extension;
                sfd.InitialDirectory = Directory.GetCurrentDirectory();
                sfd.ShowDialog();
            }



            if (path == null) path = Directory.GetCurrentDirectory() + "\\test_" + DateTime.Now.ToShortDateString() + DateTime.Now.ToString(" HH_mm_ss").Replace(":", "_") + "." + extension;
            else if (path.Split(".".ToCharArray()).Last().Equals(extension) == false) path += "." + extension;
            FileStream file = File.OpenWrite(path);



            file.Write(bytes, 0, bytes.Length);
            file.Close();

            if (isShowFileAfterSaving) Process.Start(path);
        }

        public void Easy_Print(String printName, PaperKind paperKind, Boolean isHorizon,Boolean isShowPrintDlg){

            PrinterSettings printerSettings = new PrinterSettings();
            PageSettings pageSetting;
            if (printName != null && printName.Equals("") == false)
            {
                printerSettings.PrinterName = printName;
                pageSetting = new PageSettings(printerSettings);
            }
            else
            {
                pageSetting = new PageSettings();
            }
            
            PaperSize paperSize = new PaperSize();
                paperSize.PaperName = paperKind.ToString();
                paperSize.RawKind = (int)paperKind;
            
            pageSetting.PaperSize = paperSize;
            pageSetting.Landscape = isHorizon;
            
            if (isShowPrintDlg)
            {
                /*
                SetPageSettings(pageSetting);
                if (printName != null && printName.Equals("") == false) PrintDialog(PrinterSettings);
                else
                 */
                _rv.PrintDialog();
            }
            else
            {
                try
                {
                    _reportPrintDoc = new ReportPrintDocument(_rv.LocalReport);
                    _reportPrintDoc.DefaultPageSettings = pageSetting;
                    _reportPrintDoc.Print();
                }
                catch
                {
                    try
                    {
                        _rv.PrintDialog();
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("프린트시 에러발생.."+e.ToString());
                    }
                }
            }
        }

        private void paramsSplitContainer_Load(object sender, EventArgs e)
        {

        }
    }
}
