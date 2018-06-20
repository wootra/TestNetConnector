using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.Sql;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace NewDBHandler
{
    public class XlsHandler:DBHandlerParent
    {

        string _connectionString;

        public XlsHandler(string file_name, string path)
        {
            _connectionString = XlsxConnStr(file_name, path);
        }

        protected override string DBConnectionString
        {
            get { return _connectionString; }
        }

        public override DbTransaction OpenConnection(bool makeTransaction = false)
        {
            Con =  new OleDbConnection(_connectionString);
            Cmd = new OleDbCommand();
            return base.OpenConnection(makeTransaction);
        }

        string XlsxConnStr(string file_name, String path = null)//, String password = "000000")
        {
            if (path == null || path.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
                if (file_name.IndexOf("\\") >= 0) path = "";
                //path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            else { path += (path.LastIndexOf("\\") == path.Length - 1) ? "" : "\\"; }
            string xlsxFilepath = path + file_name + ((file_name.IndexOf(".xls") < 0) ? ".xlsx" : "");

            //string xlsxConnText = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                              //@"Data Source=C:\Test.xls;" +
                              //"Extended Properties=\"Excel 8.0;HDR=YES\"";//"Provider=Microsoft.ACE.OLEDB.12.0;data source=" + xlsxFilepath + ";Mode=ReadWrite;Extended Properties=\"Excel 12.0 Xml; HDR=YES\"";
            //if(password!=null && password.Length>0) mdbConnText+= "Jet OLEDB:Database Password="+password+";";

            string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" +
	"Data Source=" + xlsxFilepath + ";" +
	"Extended Properties=Excel 8.0;";
            return (string)conStr;
        }

        protected override String GetTableName(String sheetName)
        {
            var worksheets = Con.GetSchema("Tables");

            String table = "";
            try
            {
                table = worksheets.Rows[0]["TABLE_NAME"].ToString();
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception("테이블에 내용이 없습니다." + e.Message);
            }

            String tableName;

            tableName = sheetName.ToString();
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

            return table;
        }

        protected override DbDataAdapter GetAdapter()
        {
            return new OleDbDataAdapter();
        }

        protected override DbCommand GetCommand(string query, DbConnection con)
        {
            return new OleDbCommand(query, con as OleDbConnection);
        }

 

    }
}
