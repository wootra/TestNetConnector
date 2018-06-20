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

namespace DBHandling
{
    public class MDBHandler
    {

        OleDbConnection _conn;
        OleDbCommand _cmd;
        OleDbDataAdapter _adapter;
        String _query;
        BindingSource _src = new BindingSource();

        //===============================================================================
        // FnMDB MDB 연결정보
        //===============================================================================

        public string MdbConnStr(string db_name, String path=null, String password = "000000")
        {
            if (path == null || path.Length == 0)
            {
                path = Directory.GetCurrentDirectory();
                //path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            string mdbFilePath = path + "\\" + db_name;
            string mdbConnText = @"Provider=Microsoft.JET.OLEDB.4.0;data source=" + mdbFilePath + ".mdb;";
            if(password!=null && password.Length>0) mdbConnText+= "Jet OLEDB:Database Password="+password+";";
            return (string)mdbConnText;
        }

        //===============================================================================
        // New Access MDB Create
        //===============================================================================
        public void CreateDb(string db_name,String path = null, String password = "000000")
        {
            string newMDB = MdbConnStr(db_name, path, password);
            Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
            try
            {
                if (objClassType != null)
                {
                    object obj = Activator.CreateInstance(objClassType);
                    obj.GetType().InvokeMember(
                        "Create", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { newMDB }
                    );

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception E) { MessageBox.Show("DB 생성에 문제가 생겼습니다.\n\n" + E.Message); }
        }

        public void ConnectToDb(String db_name, String dbPath=null)
        {
            _conn = new OleDbConnection(MdbConnStr(db_name, dbPath));
            _conn.Open();
            
        }

        public void DisconnectFromDb()
        {
            _conn.Close();
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

        public int ExecuteNonQuery(String query)
        {
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
        public DataTable getQueryResult(String sqlQuery, String tableName=null)
        {
            _cmd = new OleDbCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = CommandType.Text;

            _query = sqlQuery;// "SELECT * FROM [TestTable]";
            _cmd.CommandText = _query;
            //_conn.Open();

            OleDbDataAdapter dataAddapter = new OleDbDataAdapter(_cmd);
            OleDbCommandBuilder commandBuider = new OleDbCommandBuilder(dataAddapter);

            if (tableName == null || tableName.Length == 0)
            {
                tableName = _query.ToLower().Split(new String[] { "from" }, 10, StringSplitOptions.RemoveEmptyEntries)[1].Trim().Split(" ".ToCharArray())[0] ;
                //From으로 나누고, 앞뒤의 공백을 제거한 다음 공백으로 나눈 첫번째 항은 값을 가져올 Table명이다.
            }
            DataTable dt = new DataTable(tableName);
            dataAddapter.Fill(dt);
            /*
            OleDbDataReader reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

            Dictionary<String, String> obj = new Dictionary<string, string>();
            while (reader.Read())
            {
                obj.Clear();
                for(int i=0; i<reader.FieldCount; i++){
                    obj.Add(reader.GetName(i), reader.GetString(i));
                }
                
                _src.Add(obj);
            }
            
            return _src;
             */
            return dt;
        }
          private DataTable GetDetail(String strFileName, String tableName)
        {
            string con = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path.GetFileName(strFileName);
 
            string sql = "select * from student";
            OleDbConnection connection = new OleDbConnection(con);
            OleDbCommand cmd = new OleDbCommand(sql, connection);
            OleDbDataAdapter dataAddapter = new OleDbDataAdapter(cmd);
            OleDbCommandBuilder commandBuider = new OleDbCommandBuilder(dataAddapter);
 
            
            DataTable dt = new DataTable("student");
 
            // mdb파일에 들어있는 테이블 명이 "student"임.
            dataAddapter.Fill(dt);
            return dt;
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
