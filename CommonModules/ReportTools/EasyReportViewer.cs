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

namespace ReportTools
{
    public partial class EasyReportViewer : ReportViewer
    {
        private ReportPrintDocument _reportPrintDoc;
        public enum ExportFormat { PDF = 0, Excel = 1, Word =2, JPG = 3 };
        public String Title { get; set; }
        public String Name { get; set; }
        public EasyReportViewer()
        {
            InitializeComponent();
        }

        public EasyReportViewer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
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
            if (this.LocalReport.DataSources.Count == 0) this.LocalReport.DataSources.Add(new ReportDataSource());
            this.LocalReport.DataSources[0].Value = new BindingSource(source,"");
        }
        
        public void Easy_exportFile(ExportFormat fmt, String path=null, Boolean isShowDialog=false, Boolean isShowFileAfterSaving=true)
        {

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension="pdf";
            // string deviceInfo;

            String expFormat = (fmt == ExportFormat.PDF) ? "pdf" : (fmt == ExportFormat.Excel) ? "xls" : (fmt == ExportFormat.Word)? "doc" : "jpeg";
            byte[] bytes = new byte[10];
            try
            {
                bytes = this.LocalReport.Render(
                expFormat, null, out mimeType, out encoding, out extension,
                out streamids, out warnings);
            }catch(Exception e){
                MessageBox.Show("EasyReportViewer::Easy_exportFile - " + e.ToString());
            }
            if (isShowDialog == true || path==null || path.Equals(""))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = extension;
                sfd.InitialDirectory = Directory.GetCurrentDirectory();
                sfd.ShowDialog();
            }



            if (path == null) path = Directory.GetCurrentDirectory() + "\\sample" + DateTime.Now.ToShortDateString() + DateTime.Now.ToString(" HH_mm_ss").Replace(":","_") + "." + extension;
            else if (path.Split(".".ToCharArray()).Last().Equals(extension) == false) path += "." + extension;
            FileStream file = File.OpenWrite(path);
            
            

            file.Write(bytes, 0, bytes.Length);
            file.Close();

            if(isShowFileAfterSaving) Process.Start(path);
        }

        public void Easy_Print(String printName, PaperKind paperKind, Boolean isHorizon=false,Boolean isShowPrintDlg=false){

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
                SetPageSettings(pageSetting);
                if (printName != null && printName.Equals("") == false) PrintDialog(PrinterSettings);
                else PrintDialog();
            }
            else
            {
                _reportPrintDoc = new ReportPrintDocument(this.LocalReport);
                _reportPrintDoc.DefaultPageSettings = pageSetting;
                _reportPrintDoc.Print();
            }
        }

        private void paramsSplitContainer_Load(object sender, EventArgs e)
        {

        }
    }
}
