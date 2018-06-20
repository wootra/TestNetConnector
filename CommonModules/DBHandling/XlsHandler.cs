using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;

namespace DBHandling
{
    public class XlsHandler:IDisposable
    {
        Excel.Application _app;
        Excel.Workbook _book;
        Excel.Worksheet _sheet;
        Excel.Range _range;
        String _fileName;
        DataTable _table;
        Boolean _isFirstRowHeader = false;
        Boolean _isOpened = false;
        public XlsHandler(){}


        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 연속적으로 읽기를 하고 싶을 때에 사용합니다.
        /// </summary>
        /// <param name="xlsFile"></param>
        /// <param name="Sheet"></param>
        /// <param name="ReadOnly"></param>
        /// <param name="Editable"></param>
        /// <param name="isFirstRowHeader"></param>
        public void OpenExcel(String xlsFile, object Sheet, Boolean ReadOnly=true, Boolean Editable=false, Boolean isFirstRowHeader=false)
        {
            if (xlsFile.IndexOf("\\") < 0) xlsFile = Directory.GetCurrentDirectory() + "\\" + xlsFile;
            _fileName = xlsFile + ((xlsFile.IndexOf(".xls") < 0) ? ".xls" : "");
            _fileName = xlsFile;
            _app = new Excel.ApplicationClass();
            try
            {
                _book = _app.Workbooks.Open(xlsFile, 0, ReadOnly, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", Editable, false, 0, true, 1, 0);
            }
            catch(Exception ex) {
                throw ex;
            }
            _sheet = _book.Worksheets.get_Item(Sheet) as Excel.Worksheet;
            _range = _sheet.UsedRange;
            _table = new DataTable();
            DataTable table = new DataTable();
            for (int i = 0; i < _range.Columns.Count; i++)
            {
                if (isFirstRowHeader)
                    table.Columns.Add((_range.Cells[0, i] as Excel.Range).Value2.ToString());//(_range.Cells[0,i] as Excel.Range).Value2.ToString());
                else table.Columns.Add();
            }
            _table = table;
            _isOpened = true;
        }

        public Boolean IsOpened { get { return _isOpened; } }
        public void CreateNewXls(String fileName, params String[] SheetNames)
        {
            if (fileName.IndexOf("\\") < 0) fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
            _fileName = fileName + ((fileName.IndexOf(".xls")<0)? ".xls" : "");
            _app = new Excel.ApplicationClass();
            _book = _app.Workbooks.Add();//.Open(ExcelFile, 0, ReadOnly, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", Editable, false, 0, true, 1, 0);
            int count = 0;
            foreach (String sheetName in SheetNames)
            {
                object beforeSheet = (_sheet != null) ? _sheet : Type.Missing;
                if (count != 0) _sheet = _book.Worksheets.Add(Type.Missing,_sheet) as Excel.Worksheet;
                else _sheet = _book.ActiveSheet as Excel.Worksheet;
                _sheet.Name = sheetName;
                count++;
            }
            _app.Visible = false;
            _app.UserControl = false;
            //_sheet = _book.ActiveSheet as Excel.Worksheet;//.Worksheets.get_Item(Sheet) as Excel.Worksheet;
            
            //_sheet.Name = SheetName;
            
            //_range = _sheet.UsedRange;
            
            _book.SaveAs(_fileName, Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            _book.Close();
            _app.Quit();
        }

        public Excel.Workbook WorkBook { get { return _book; } }
        public Excel.Worksheet WorkSheet { get { return _sheet; } }

        public void CreateNewXlsx(String fileName, params String[] SheetNames)
        {
            if (fileName.IndexOf("\\") < 0) fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
            _fileName = fileName + ((fileName.IndexOf(".xls") < 0) ? ".xls" : "");
            _app = new Excel.ApplicationClass();
            _book = _app.Workbooks.Add();//.Open(ExcelFile, 0, ReadOnly, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", Editable, false, 0, true, 1, 0);
            int count = 0;
            foreach (String sheetName in SheetNames)
            {
                object beforeSheet = (_sheet != null) ? _sheet : Type.Missing;
                if (count != 0) _sheet = _book.Worksheets.Add(Type.Missing, _sheet) as Excel.Worksheet;
                else _sheet = _book.ActiveSheet as Excel.Worksheet;
                _sheet.Name = sheetName;
                count++;
            }
            _app.Visible = false;
            _app.UserControl = false;
            //_sheet = _book.ActiveSheet as Excel.Worksheet;//.Worksheets.get_Item(Sheet) as Excel.Worksheet;

            //_sheet.Name = SheetName;

            //_range = _sheet.UsedRange;

            _book.SaveAs(_fileName, Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, false, false,
                Excel.XlSaveAsAccessMode.xlShared, false, false, Type.Missing, Type.Missing, Type.Missing);
            _book.Close();
            _app.Quit();
        }

        public void CreateNewXlsUnder2002(String fileName, params String[] SheetNames)
        {
            if (fileName.IndexOf("\\") < 0) fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
            _fileName = fileName + ((fileName.IndexOf(".xls") < 0) ? ".xls" : "");

            Type objClassType = Type.GetTypeFromProgID("Excel.Application");
            _app = (Excel.Application)Activator.CreateInstance(objClassType, true);
            _book = _app.Workbooks.Add(true);//.Open(ExcelFile, 0, ReadOnly, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", Editable, false, 0, true, 1, 0);
            
            int count = 0;
            foreach (String sheetName in SheetNames)
            {
                object beforeSheet = (_sheet != null) ? _sheet : Type.Missing;
                if (count != 0) _sheet = _book.Worksheets.Add(Type.Missing, _sheet) as Excel.Worksheet;
                else _sheet = _book.ActiveSheet as Excel.Worksheet;
                _sheet.Name = sheetName;
                count++;
            }
            //_sheet = _book.ActiveSheet as Excel.Worksheet;//.Worksheets.get_Item(Sheet) as Excel.Worksheet;
            //_range = _sheet.UsedRange;
            _book.SaveAs(_fileName, Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            _book.Close();
            _app.Quit();

        }
        
        /// <summary>
        /// 한번만 데이터를 읽어올 때 사용합니다. 엑셀파일을 열고, 닫기를 자동으로 합니다.
        /// </summary>
        /// <param name="ExcelFile"></param>
        /// <param name="Sheet"></param>
        /// <param name="isFirstRowHeader">첫 행이 제목열일 때 사용합니다. 첫행을 빼고 가져올 때 true</param>
        /// <returns></returns>
        public static DataTable getTable(String ExcelFile, object Sheet, Boolean isFirstRowHeader=false){
            XlsHandler xls = new XlsHandler();
            xls.OpenExcel(ExcelFile,Sheet, true, false, isFirstRowHeader);
            DataTable table = xls.getTable();
            xls.Close();
            return table;
        }

        /// <summary>
        /// 한번만 쓰는 작업을 할 때 사용합니다. 엑셀파일을 열고, 닫기를 자동으로 합니다.
        /// </summary>
        /// <param name="ExcelFile"></param>
        /// <param name="Sheet"></param>
        /// <param name="table"></param>
        /// <param name="isFirstRowHeader">첫 행을 무시하고 다음행부터 씁니다.</param>
        public static void setTable(String ExcelFile, object Sheet, DataTable table, Boolean isFirstRowHeader=false){
            XlsHandler xls = new XlsHandler();
            xls.OpenExcel(ExcelFile, Sheet, false, true, isFirstRowHeader);
            xls.setTable(table);
            xls.Close();
        }

        /// <summary>
        /// Microsoft.Office.Interop.Excel형으로 데이터를 가져옴. 제목열까지 한꺼번에 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public Excel.Range getRange() { return _range; }

        /// <summary>
        /// Open을 한 이후에 쓸 수 있는 명령입니다. 읽고 나서도 Close하지 않습니다.
        /// </summary>
        /// <returns></returns>
        public DataTable getTable()
        {
            int firstRow = (_isFirstRowHeader) ? 2 : 1;
            _table.Clear();
            for (int row = firstRow; row <= _range.Rows.Count; row++)
            {
                List<Object> objList = new List<object>();
                for (int col = 1; col <= _range.Columns.Count; col++)
                {
                    try
                    {
                        objList.Add((_range.Cells[row, col] as Excel.Range).Value2.ToString());
                    }
                    catch { }
                }
                _table.Rows.Add(objList.ToArray());
            }
            return _table;
        }

        /// <summary>
        /// Open한 이후에 쓸 수 있는 명령입니다. 쓰고 나서도 Close하지 않습니다.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="WriteAfterClear"></param>
        public void setTable(DataTable table, Boolean WriteAfterClear=false){
            
            int firstRow = (_isFirstRowHeader) ? 2 : 1;
            if(WriteAfterClear) _sheet.Cells.Clear();
            for (int row = firstRow; row <= _range.Rows.Count; row++)
            {
                //List<Object> objList = new List<object>();
                for (int col = 1; col <= _range.Columns.Count; col++)
                {
                    try
                    {
                        (_sheet.Cells[row, col] as Excel.Range).Value2 = table.Rows[row - firstRow][col - 1];
                        //(_range.Cells[row, col] as Excel.Range).Value2 = table.Rows[row - 1][col - 1];
                        //objList.Add((_range.Cells[row, col] as Excel.Range).Value2.ToString());
                    }
                    catch { }
                }
                //table.Rows.Add(objList.ToArray());
            }
            _book.Save();
        }

        /// <summary>
        /// 열려있던 엑셀파일을 닫습니다.
        /// </summary>
        /// <param name="isSaveChanged"></param>
        public void Close(Boolean isSaveChanged=false)
        {
            try{
                _book.Close(isSaveChanged, (isSaveChanged) ? _fileName : null, null);
            }
            catch { }
            //try { _app.Quit(); }
            //catch { }
            try { releaseObject(_sheet); }
            catch { }
            try { releaseObject(_book); }
            catch { }
            try { releaseObject(_app); }
            catch { }
            try{_app.Quit();}
            catch { }
            _isOpened = false;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        } 



    }
}
