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
using ReportTools;

namespace ReportTools.NotUsing
{
    public partial class ReportDialog : Form
    {
        ReportViewerHandler_new _rh;   
        public ReportDialog()
        {
            InitializeComponent();

            
        }

        public void setReportDialog(String rdlcFile, String DataSetName="DataSet1"){
            _rh = new ReportViewerHandler_new(components, this, DockStyle.Fill, "rv", DataSetName, rdlcFile);
            //_rh.getReportVeiwer().Print += new CancelEventHandler(ReportViewer_Print);
            //wootra ..release
            ReportViewer rv = _rh.getReportVeiwer();

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
        public void setReportData(BindingSource source){
            
            //BindingSource testResult = new BindingSource();
            /*
            foreach (TestRow row in source)
            {
                
                 testResult.Add(new ReportRow(row.index, row.Item, row.Channel, (row.IsOK) ? "ok" : "fail"));
            }
            */
            _rh.Easy_addBindingSource(source);
            /*
            this.rviewer.LocalReport.DataSources[0].Value = new BindingSource(testResult,"");
            
            this.rviewer.Refresh();
            */
            _rh.getReportVeiwer().RefreshReport();
            
            //this.ReportViewer.DataBindings.Add("Item", source, "Item", true, DataSourceUpdateMode.OnPropertyChanged, "없음", "{0}");

        }
    }
}
