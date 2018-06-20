using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.IO;
using System.Data;
using System.Data.Common;


namespace NewDBHandler
{
    public abstract class DBHandlerParent
    {
        public DbDataReader Reader = null;
        public DbConnection Con = null;
        public DbCommand Cmd = null;
        public DbTransaction Trans = null;
        public Dictionary<String, String> dbDict = new Dictionary<String, String>();


        protected abstract String DBConnectionString
        {
            get;
        }
            //= @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\stockhero\StockHeroV2\Stockherov2\app_data\StockHeroDB.mdf;Integrated Security=True";

        /// <summary>
        /// Con과 Cmd를 만들고 Open한다.
        /// </summary>
        /// <param name="makeTransaction"></param>
        /// <returns></returns>
        public virtual DbTransaction OpenConnection(bool makeTransaction = false)
        {

            // 데이터베이스 연결
            //Con = new SqlConnection(DBConnectionString);
            //Cmd = new SqlCommand();
            // 커맨드에 커넥션을 연결
            Cmd.Connection = Con;
            Con.Open();

            if (makeTransaction)
            {
                return Con.BeginTransaction();
            }
            else return null;
        }



        public void CloseConnection()
        {
            Con.Close();
        }

        
        public DataTable Select(String fields, String table, String condition = null)
        {
            
            try
            {
                string cmd;
                cmd = "SELECT " + fields + " FROM [" + GetTableName(table) + "]" + ((condition != null && condition.Length > 0) ? " WHERE " + condition : "");
                /*
                if (condition != null && condition != "")
                    Cmd.CommandText = " SELECT " + data + " FROM [" + table + "] WHERE " + condition + ";";
                else
                    Cmd.CommandText = " SELECT " + data + " FROM [" + table + "];";
                */
                DbDataAdapter adapter = GetAdapter();
                adapter.SelectCommand = GetCommand(cmd, Con);
                DataSet dataSet = new DataSet();
            
                adapter.Fill(dataSet, "XLData");

                return dataSet.Tables[0];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

            }
            return null;
        }

        protected virtual DbCommand GetCommand(string query, DbConnection con)
        {
            throw new NotImplementedException();
        }

        protected virtual DbDataAdapter GetAdapter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 특이한 tableName을 쓸 경우 override해 준다. 아니면 그대로 사용한다.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        protected virtual String GetTableName(String sheetName)
        {
            return sheetName;
        }

        public  int Update(Dictionary<String, String> data, String table, String cond, DbTransaction tx)
        {
            int result = 0;
            Cmd.Parameters.Clear();
            Cmd.Transaction = tx;
            
            try
            {
                if (data != null)
                {
                    List<string> keys = data.Keys.ToList();
                    String Set = "";
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if(i>0) Set+=",";
                        Set += keys[i] + "=";
                        Set += "@" + keys[i];
                        
                        DbParameter pm = Cmd.CreateParameter();
                        pm.ParameterName = keys[i];
                        pm.Value = data[keys[i]];
                        
                        Cmd.Parameters.Add(pm);//.AddWithValue(list[i], data[list[i]]);

                    }
                    //DbDataAdapter adapter = GetAdapter();
                    //adapter.UpdateCommand = GetCommand("UPDATE " + table + " SET " + Set + ((cond!=null && cond.Length>0)? " WHERE " + cond + ";" : ""), Con);

                    Cmd.CommandText = "UPDATE [" + GetTableName(table) + "] SET " + Set + ((cond!=null && cond.Length>0)? " WHERE " + cond + ";" : "");
                }


                result = (int)Cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return result;

        }

        public  int Delete(String table, String cond, DbTransaction tx)
        {
            int result = 0;
            Cmd.Parameters.Clear();
            Cmd.Transaction = tx;
            Cmd.CommandText = "DELETE FROM [" + GetTableName(table) + "]" + ((cond != null && cond.Length > 0) ? " WHERE " + cond + ";" : "");
            result = (int)Cmd.ExecuteNonQuery();
            return result;
        }

        public  int Insert(Dictionary<String, String> data, String table, DbTransaction tx)
        {
            int result = 0;
            Cmd.Parameters.Clear();
            Cmd.Transaction = tx;
            List<string> list = new List<string>(data.Keys);

            String Column = "";
            String Values = "";
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i != 0)
                    {
                        Column += ",";
                        Values += ",";
                    }

                    Column += list[i];
                    Values += "@" + list[i];
                    string str = data[list[i]];
                    
                    DbParameter pm = Cmd.CreateParameter();
                    pm.ParameterName = list[i];
                    pm.Value = str;

                    Cmd.Parameters.Add(pm);//.AddWithValue(list[i], str);
                }
                Cmd.CommandText = "INSERT INTO [" + GetTableName(table) + "] (" + Column + ") Values (" + Values + ");";
                result = (int)Cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }


            return result;

        }
        public  Int64 GetIndex()
        {
            Cmd.CommandText = "SELECT @@IDENTITY";
            var val = Cmd.ExecuteScalar();
            Int64 id = Int64.Parse(val.ToString());

            return id;
        }

        /// <summary>
        /// fieldname=value와 같은 식을 dictionary로 변환하여 줌..
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetDBData(String content)
        {
            dbDict.Clear();
            String[] fieldValues = content.Split('&');
            for (int i = 0; i < fieldValues.Length; i++)
            {
                String[] fieldValue = fieldValues[i].Split('=');

                dbDict.Add(fieldValue[0], fieldValue[1]);
            }

            return dbDict;
        }

        public String getTextFieldAndOption(String fieldName, TextType type = TextType.TEXT, int size = 255, Boolean isBinary = false, Boolean notNull = false, String Default = "", Boolean primaryKey = false, params Object[] numOrOptions)
        {
            String field = fieldName;

            if (type == TextType.CHAR || type == TextType.VARCHAR) field += " " + type.ToString() + "(" + size.ToString() + ") ";
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
                field += " " + type.ToString();
                if (isBinary) field += " " + "BINARY";
            }
            if (notNull) field += " NOT NULL";
            if (notNull && Default != null && Default.Length > 0) field += " DEFAULT " + Default;

            if (primaryKey) field += " PRIMARY KEY";

            return field;
        }

        public String getNumFieldAndOption(String fieldName, NumType type = NumType.INT, Boolean unsigned = false, Boolean zeroFill = false, Double Default = 0, Boolean autoIncrement = false, Boolean primaryKey = false, int size = 1)
        {
            String field = fieldName;

            if (type == NumType.INTEGER) field += " " + type.ToString();
            else field += " " + type.ToString() + "[" + size.ToString() + "]";

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

        public String GetTimeFieldAndOption(String fieldName, TimeType type = TimeType.DATETIME, Boolean isNotNull = false, String Default = "")
        {
            String field = fieldName;

            field += " " + type.ToString();
            if (isNotNull) field += " NOT NULL";//숫자형은 무조건 not null이다.

            if (isNotNull && Default != null && Default.Length > 0) field += " DEFAULT '" + Default + "'";


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
            Cmd.CommandText = query;
            Cmd.ExecuteNonQuery();

        }
    }
}
