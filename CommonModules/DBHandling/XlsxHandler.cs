using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.Sql;
using System.Collections;
using System.Data;
using System.Diagnostics;
//using Excel = Microsoft.Office.Interop.Excel;

namespace DBHandling
{
    public class XlsxHandler:IDisposable
    {

        OleDbConnection _conn;
        OleDbCommand _cmd;
        OleDbDataAdapter _adapter;
        String _query;
        BindingSource _src = new BindingSource();

        public XlsxHandler()
        {

        }

        public string XlsxConnStr(string file_name, String path = null)//, String password = "000000")
        {
            if (path == null || path.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
                if (file_name.IndexOf("\\") >= 0) path = "";
                //path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            else { path += (path.LastIndexOf("\\")==path.Length-1)? "" : "\\"; }
            string xlsxFilepath = path + file_name + ((file_name.IndexOf(".xls")<0)? ".xlsx" : "");
            
            string xlsxConnText = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + xlsxFilepath + ";Mode=ReadWrite;Extended Properties=\"Excel 12.0 Xml; HDR=YES\"";
            //if(password!=null && password.Length>0) mdbConnText+= "Jet OLEDB:Database Password="+password+";";
            return (string)xlsxConnText;
        }

        String getProviderVersion(String providerName)
        {
            try
            {
                
            }
            catch { }
            return "";
        }
        //===============================================================================
        // New Access MDB Create
        //===============================================================================
        /*
        public void CreateNewXlsx(String fileName, params String[] SheetNames)
        {
            Excel.Application _app;
            Excel.Workbook _book;
            Excel.Worksheet _sheet=null;

            if (fileName.IndexOf("\\") < 0) fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
            fileName = fileName + ((fileName.IndexOf(".xlsx") < 0) ? ".xlsx" : "");
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

            _book.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, false, false,
                Excel.XlSaveAsAccessMode.xlShared, false, false, Type.Missing, Type.Missing, Type.Missing);
            _book.Close();
            _app.Quit();
        }
        */
        ~XlsxHandler()
        {
            if (_conn != null) Close();
        }
        void IDisposable.Dispose()
        {
            if (_conn != null) Close();
        }

        public void ConnectToDb(String filePath)
        {
            _conn = new OleDbConnection(XlsxConnStr(filePath));
            _conn.Open();
            if (_conn.State != ConnectionState.Open)
            {
                throw new Exception("엑셀 파일 " + filePath + "에 연결할 수 없습니다.");
            }
            
        }
        public void Close()
        {
            DisconnectFromDb();
        }

        public void DisconnectFromDb()
        {
            try
            {
                _conn.Close();
            }
            catch { }
            _conn = null;
           // Marshal.FinalReleaseComObject(_conn);
        }


        
        public String getTextFieldAndOption(String fieldName, TextType type = TextType.TEXT, int size=255, Boolean isBinary=false, Boolean notNull = false, String Default = "", Boolean primaryKey = false, params Object[] numOrOptions)
        {
            String field = fieldName;
            
            if (type == TextType.CHAR || type == TextType.VARCHAR) field += " "+ type.ToString() + "("+ size.ToString()+") ";
            else if (type == TextType.ENUM || type == TextType.SET)
            {
                if (type == TextType.SET && numOrOptions.Length > 64) throw new Exception("SET형은 64개까지만 저장 가능합니다.");
                else
                {
                    field += " " + type.ToString();
                    if (numOrOptions.Length > 0)
                    {
                        field += "(";
                        for (int i = 0; i < numOrOptions.Length; i++)
                        {
                            if (i != 0) field += ", ";
                            field += numOrOptions[i] as String;
                        }
                        field += ")";
                    }
                }
            }
            else
            {
                field += " "+type.ToString();
                if (isBinary) field += " " + "BINARY";
            }
            if (notNull) field += " NOT NULL";
            if (notNull && Default != null && Default.Length > 0) field += " DEFAULT " + Default;

            if (primaryKey) field += " PRIMARY KEY";

            return field;
        }

        public String getNumFieldAndOption(String fieldName, NumType type = NumType.INT,  Boolean unsigned = false, Boolean zeroFill = false, Double Default = 0, Boolean autoIncrement=false, Boolean primaryKey = false,int size=1)
        {
            String field = fieldName;

            if (type == NumType.INTEGER) field += " " + type.ToString();
            else  field += " " + type.ToString() + "[" + size.ToString() + "]";

            if (unsigned) field += " UNSIGNED";
            if (zeroFill) field += " ZEROFILL";
            else
            {
                field += " NOT NULL";//숫자형은 무조건 not null이다.
                if (type == NumType.REAL || type == NumType.FLOAT || type == NumType.DOUBLE)
                {
                    field += " DEFAULT " + Default;
                }
                else
                {
                    field += " DEFAULT " + ((int)Default).ToString();
                }
            }

            if (primaryKey) field += " PRIMARY KEY";

            return field;
        }

        public String GetTimeFieldAndOption(String fieldName, TimeType type = TimeType.DATETIME, Boolean isNotNull=false, String Default="")
        {
            String field = fieldName;

            field += " " + type.ToString();
            if(isNotNull) field += " NOT NULL";//숫자형은 무조건 not null이다.
            
            if(isNotNull && Default!=null && Default.Length>0) field += " DEFAULT '" + Default+"'";
            

            return field;
        }

        public void CreateTable(String TableName, params string[] fieldsAndOptions)
        {
            string query = "CREATE TABLE " + TableName + "(";
            for (int i = 0; i < fieldsAndOptions.Length; i++)
            {
                if (i != 0) query += ", ";
                query += fieldsAndOptions[i];
            }
            query += ");";
            _cmd = new OleDbCommand(query, _conn);
            _cmd.ExecuteNonQuery();

        }
        /*
        public int INSERT_QUERY(String TableName, String Values="", String Fields="")
        {
            if(TableName.IndexOf("$")<0) TableName="["+TableName+"$]";
            if (Fields.Length > 0) Fields = "(" + Fields + ")";
            string query = "INSERT INTO "+TableName + Fields+ " VALUES("+Values+")";
            return ExecuteNonQuery(query);
        }
        */

        public int INSERT_QUERY(String TableName, Dictionary<String,Object> data)
        {
            if (TableName.IndexOf("$") < 0) TableName = "[" + TableName + "$]";
            string Fields = "";
            String Values = "";
            int count = 0;
            foreach (string key in data.Keys)
            {
                if (count > 0) Fields += ", ";
                if (count++ > 0) Values += ", ";

                Fields += key;
                Values += (data[key] is String) ? "'" + data[key] + "'" : data[key].ToString();
            }

            if (Fields.Length > 0) Fields = "(" + Fields + ")";
            string query = "INSERT INTO " + TableName + Fields + " VALUES(" + Values + ")";
            return ExecuteNonQuery(query);
        }

        public int INSERT_QUERY(String TableName, params Object[] data)
        {
            if (TableName.IndexOf("$") < 0) TableName = "[" + TableName + "$]";
            
            String Values = "";
            for(int i=0; i<data.Length; i++)
            {
                if (i > 0) Values += ", ";

                Values += (data[i] is String) ? "'" + data[i] + "'" : data[i].ToString();
            }
            string query = "INSERT INTO " + TableName + " VALUES(" + Values + ")";
            return ExecuteNonQuery(query);
        }

        public int UPDATE_QUERY(String TableName, Dictionary<String,Object> updateData, Dictionary<String,Object> whereCondition=null)
        {
            String set = "";
            
            int count = 0;
            foreach (string key in updateData.Keys)
            {
                if (count++ > 0) set += ", ";

                set += key + "=";
                set += (updateData[key] is String)? "'"+updateData[key]+"'" : updateData[key].ToString();
            }
            String where = "";
            count = 0;
            foreach (string key in whereCondition.Keys)
            {
                if (count++ > 0) where += ", ";

                where += key + "=";
                where += (whereCondition[key] is String) ? "'" + whereCondition[key] + "'" : whereCondition[key].ToString();
            }

            if (TableName.IndexOf("$") < 0) TableName = "[" + TableName + "$]";
            else TableName = TableName = "[" + TableName + "]";

            string query = "UPDATE " + TableName + " SET " + set;
            query += (whereCondition!=null && whereCondition.Count > 0) ? " WHERE " + where : "";
            if (set.Length == 0) return 0;
            try
            {
                return ExecuteNonQuery(query);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message +" QUERY:"+ query);
            }
        }


        public int DELETE_QUERY(String TableName, Dictionary<String, Object> whereCondition = null)
        {
            int count = 0;

            String where = "";
            count = 0;
            foreach (string key in whereCondition.Keys)
            {
                if (count++ > 0) where += ", ";

                where += key + "=";
                where += (whereCondition[key] is String) ? "'" + whereCondition[key] + "'" : whereCondition[key].ToString();
            }

            if (TableName.IndexOf("$") < 0) TableName = "[" + TableName + "$]";
            else TableName = TableName = "[" + TableName + "]";

            string query = "DELETE " + TableName;
            query += (whereCondition != null && whereCondition.Count > 0) ? " WHERE " + where : "";

            return ExecuteNonQuery(query);
        }

         

        public int ExecuteNonQuery(String query)
        {
            String[] items = query.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            String newQuery = "";
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].ToLower().Equals("from"))
                {
                    items[i + 1].Replace("[", "");
                    items[i + 1].Replace("]", "");
                    
                    if (items[i + 1].IndexOf("$") < 0)
                    {
                        items[i + 1] += "$";
                    }
                    items[i + 1] = "[" + items[i + 1] + "]";
                }
                newQuery += " " + items[i];
            }
            query = newQuery;
            _cmd = new OleDbCommand(query, _conn);
            return _cmd.ExecuteNonQuery();
        }
        public void getQueryResultSource(DataGridView src, String sqlQuery, String tableName = null)
        {
            _cmd = new OleDbCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = CommandType.Text;

            _query = sqlQuery;// "SELECT * FROM [TestTable]";
            _cmd.CommandText = _query;
            //_conn.Open();
            
            OleDbDataReader reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //List<List<String>> src = new List<List<string>>();
            //List<String> list;
            //DataGridViewRow row;
            
            while (reader.Read())
            {
                //row = new DataGridViewRow();
                //src.AddNew();
                List<Object> objList = new List<object>();
                //list = new List<string>();

                if (src.Columns.Count == 0)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        src.Columns.Add(reader.GetName(i), reader.GetName(i));

                    }
                }
                for(int i=0; i<reader.FieldCount; i++){
                    
                    //list.Add(reader.GetString(i));
                    objList.Add(reader.GetString(i));
                    
                    //src.Insert(i, reader.GetString(i));
                    //obj.Add(reader.GetName(i), reader.GetString(i));
                }
                //row.CreateCells(src, objList.ToArray());
                
                src.Rows.Add(objList.ToArray());
                //src.Add(list);
                //_src.Add(obj.Values);
            }
            
            //return src;

        }
        public DataTable getQueryResultSource(String tableName, String where="", params string[] fieldNames)
        {
            _cmd = new OleDbCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = CommandType.Text;
            if (tableName.IndexOf("$") < 0) tableName += "$";
            if (fieldNames == null || fieldNames.Length == 0)
            {
                _query = "SELECT * from [" + tableName + "]";
                
            }
            else
            {
                _query = "SELECT " + fieldNames[0];
                for (int i = 1; i < fieldNames.Length; i++)
                {
                    _query += "," + fieldNames[i];
                }
                _query += " from [" + tableName + "]";
            }
            if (where != null && where.Length > 0) _query += " where " + where;

            //_query = sqlQuery;// "SELECT * FROM [TestTable]";
            _cmd.CommandText = _query;
            //_conn.Open();
            DataTable table = new DataTable(tableName);

            OleDbDataReader reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //List<List<String>> src = new List<List<string>>();
            //List<String> list;
            //DataGridViewRow row;
            if (table.Columns.Count == 0)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i));
                }
            }

            while (reader.Read())
            {
                //row = new DataGridViewRow();
                //src.AddNew();
                //list = new List<string>();

                //List<Object> objList = new List<object>();
                object[] objs = new object[reader.FieldCount];
                reader.GetValues(objs);
                if (objs[0] == null || objs[0].ToString().Length==0)
                {
                    
                    continue;
                }
                table.Rows.Add(objs);
                /*
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetFieldType(i) == typeof(String)) objList.Add(reader.GetValues reader.GetString(i));
                    else if (reader.GetFieldType(i) == typeof(int)) objList.Add(reader.GetInt32(i));
                    else
                    {
                        
                    }
                    //list.Add(reader.GetString(i));
                    

                    //src.Insert(i, reader.GetString(i));
                    //obj.Add(reader.GetName(i), reader.GetString(i));
                }
                 */
                //row.CreateCells(src, objList.ToArray());
                
                
                //src.Add(list);
                //_src.Add(obj.Values);
            }
            return table;

            //return src;

        }
        
        public DataTable getDataTable(Object sheetNameOrNumber, String range="", params String[] fieldNames)
        {
            var worksheets = _conn.GetSchema("Tables");

            String table="";
            try
            {
                table = worksheets.Rows[0]["TABLE_NAME"].ToString();
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception("테이블에 내용이 없습니다." + e.Message);
            }

            String tableName;
            if (sheetNameOrNumber is int && (int)sheetNameOrNumber > 0)
            {
                int count = 0;
                foreach (DataRow row in worksheets.Rows)
                {
                    if(count++==(int)sheetNameOrNumber)
                    {
                        table = row["TABLE_NAME"].ToString();
                        break;
                    }
                }
            }
            else if(sheetNameOrNumber is String)//string
            {
                tableName = sheetNameOrNumber.ToString();
                if (tableName.Length > 0)
                {
                    foreach (DataRow row in worksheets.Rows)
                    {
                        if (row["TABLE_NAME"].ToString().IndexOf(tableName) >= 0)
                        {
                            table = row["TABLE_NAME"].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception("엑셀의 시트이름이 빠졌습니다.");
                }
            }
            if (range.Length > 0)
            {
                table = table + range;
            }
            
            
            string field = "*";
            if (fieldNames.Length > 0)
            {
                field = "";
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (i != 0) field += ",";
                    field += "A." + fieldNames[i];
                }

            }
            String query = String.Format(" select A.* from [{0}] as A", table);// worksheets.Rows[0]["TABLE_NAME"]);
            _cmd = new OleDbCommand(query, _conn);

            OleDbDataAdapter dataAddapter = new OleDbDataAdapter(_cmd);
            DataSet ds = new DataSet();
            dataAddapter.Fill(ds);
            DataTable tbl = new DataTable();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                tbl.Columns.Add();
            }
            /*
            DataColumn[] cols = new DataColumn[ds.Tables[0].Columns.Count];
            ds.Tables[0].Columns.CopyTo(cols,0);
            tbl.Columns.AddRange(cols);
             */
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow row =  ds.Tables[0].Rows[i];
                //row.ItemArray = ds.Tables[0].Rows[i].ItemArray;
                tbl.Rows.Add(row.ItemArray);
            }
            return tbl;
 
        }

       


        //HELP SAMPLE//
        /*
         
        public void CreateTable()
        {
            _db.ConnectToDb("test_db");
            
            try
            {

                _db.CreateTable("testTable",
                    _db.getTextFieldAndOption("test1"),
                    _db.getTextFieldAndOption("test2"));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            try
            {

                _db.CreateTable("testTable1",
                    _db.getTextFieldAndOption("test1"),
                    _db.getTextFieldAndOption("test2"));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //_db.DisconnectFromDb();

            //_db.ConnectToDb("test_db");

            int size = _db.ExecuteNonQuery("INSERT INTO testTable VALUES('test','test2');");
            size = _db.ExecuteNonQuery("INSERT INTO testTable1 VALUES('test','test2');");
            //_db.DisconnectFromDb();

            //_db.ConnectToDb("test_db");
            
            DataTable source = _db.getQueryResult("SELECT * FROM testTable","testTable");
           // source.Rows.Add("test11", "test21");
            
            V_Data.DataSource = source;
            _db.DisconnectFromDb();
            V_Data.Refresh();

        }
 
         */

    }

}
