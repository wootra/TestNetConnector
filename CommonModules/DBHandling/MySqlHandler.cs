using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MySql.Data.MySqlClient;

namespace DBHandling
{

    public class MySqlHandler
    {

        MySqlConnection _conn;
        MySqlCommand _cmd;
        MySqlDataAdapter _adapter;
        String _query;
        BindingSource _src = new BindingSource();

        //===============================================================================
        // FnMDB MDB 연결정보
        //===============================================================================

        public string MySqlConnStr(string db_name, String url="localhost", String userId="admin", String password = "")
        {
            if (url == null || url.Length == 0)
            {
                url = "localhost";
                //path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            //string mdbFilePath = path + "\\" + db_name;
            //String trusted="true";
            //String a="Data Source=localhost;Database=test;User Id=root;Password=root";
            //if (password != null && password.Length > 0) trusted = "false";
            string mdbConnText = @"Data Source=" + url + ";Database=" + db_name + ";User Id=" + userId;// +mdbFilePath + ".mdb;";
            if(password!=null && password.Length>0) mdbConnText+= ";Password="+password;
            return (string)mdbConnText;
        }


        public void ConnectToDb(String db_name, String url="localhost", String userId="admin", String password="")
        {
            string dbStr = MySqlConnStr(db_name, url, userId, password);
            _conn = new  MySqlConnection(dbStr);

            _conn.Open();
            if (_conn.State == ConnectionState.Open)
            {

            }
            else
            {
                throw new Exception("DB로 연결 실패");
            }
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
            _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();

        }

        public int ExecuteNonQuery(String query)
        {
            _cmd = new MySqlCommand(query, _conn);
            return _cmd.ExecuteNonQuery();
        }
        public void getQueryResultSource(DataGridView src, String MySqlQuery, String tableName = null)
        {
            _cmd = new MySqlCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = CommandType.Text;

            _query = MySqlQuery;// "SELECT * FROM [TestTable]";
            _cmd.CommandText = _query;
            //_conn.Open();
            
            MySqlDataReader reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

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
            reader.Close();
            //return src;

        }
        public DataTable getQueryResult(String MySqlQuery, String tableName=null)
        {
            _cmd = new MySqlCommand();
            _cmd.Connection = _conn;
            _cmd.CommandType = CommandType.Text;

            _query = MySqlQuery;// "SELECT * FROM [TestTable]";
            _cmd.CommandText = _query;
            //_conn.Open();

            MySqlDataAdapter dataAddapter = new MySqlDataAdapter(_cmd);
            MySqlCommandBuilder commandBuider = new MySqlCommandBuilder(dataAddapter);

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
