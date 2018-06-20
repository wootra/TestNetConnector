#region [[[ COPYRIGHT_NOTICE ]]]
/*
 * ============================================================================
 * 
 * Copyright(c) 2011 Allright reserved.
 * 
 * 이 소스코드는, 상업적 용도를 포함하여 어떠한 목적으로든 자유롭게 사용 및
 * 수정, 배포 할 수 있습니다.
 * 
 * 이 소스코드는 "있는 그대로(AS-IS)"제공되며, 저작자는 어떠한 보증도 하지
 * 않습니다. 사용으로 인한 책임은 전적으로 사용자에게 있습니다.
 *
 * ----------------------------------------------------------------------------
 *
 * Author: GreenB
 * http://blog.greenmaru.com
 *
 * Latest update: 2011-10-17
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Access
{
    /// <summary>
    /// OleDb를 이용해 Access Database를 조작하기 위한 개체.
    /// </summary>
    public class AccessAdo : IOfficeAdo
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER VARIABLES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_VARIABLES

        /// <summary>
        /// 연결문자열 형식.
        /// </summary>
        private static readonly string _ConnectionStringFormat = "Provider=Microsoft.{0}.OLEDB.{1};Data Source=\"{2}\";";

        /// <summary>
        /// Access Database와의 연결 개체.
        /// </summary>
        private OleDbConnection _conn;

        /// <summary>
        /// Access Database파일 이름.
        /// </summary>
        private string _fileName = string.Empty;

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 AccessAdo개체 인스턴스를 초기화 합니다.
        /// </summary>
        public AccessAdo()
        {
            // do nothing.
        }


        /// <summary>
        /// 새로운 AccessAdo개체 인스턴스를, 주어진 Access Database 파일 이름을 사용해 초기화 합니다.
        /// </summary>
        /// <param name="dbFileName">파일 이름.</param>
        public AccessAdo(string dbFileName)
        {
            Open(dbFileName);
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// Access Database와의 연결을 위한 연결문자열. 읽기전용.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return AccessAdo.GetConnectionString(FileName);
            }
        }

        /// <summary>
        /// Access Database 파일 이름.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (this.IsOpened)
                    throw new InvalidOperationException("Connection already opened.");

                _fileName = value;
            }
        }

        
        /// <summary>
        /// 현재 연결 상태. 읽기전용.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                if (_conn == null)
                    return ConnectionState.Closed;

                return _conn.State;
            }
        }

        /// <summary>
        /// 현재 Access Database와 연결된 상태인지를 나타냅니다. 읽기전용.
        /// </summary>
        public bool IsOpened
        {
            get
            {
                return this.State == ConnectionState.Open;
            }
        }

        #endregion  // PROPERTIES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  METHODS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region METHODS

        /// <summary>
        /// Access Database와 연결합니다.
        /// </summary>
        public void Open()
        {
            Open(this.FileName);
        }

        /// <summary>
        /// 주어진 경로의 Access Database와 연결합니다.
        /// </summary>
        /// <param name="dbFileName">Access Database 파일 이름.</param>
        public void Open(string dbFileName)
        {
            Close();
            this.FileName = dbFileName;

            _conn = new OleDbConnection(this.ConnectionString);
            _conn.Open();
        }

        /// <summary>
        /// Access Database와의 연결을 닫습니다.
        /// </summary>
        public void Close()
        {
            if (_conn != null)
            {
                if (IsOpened)
                    _conn.Close();
                
                _conn.Dispose();
                _conn = null;
            }
        }

        /// <summary>
        /// 주어진 명령문을 실행하고, 그 결과를 DataSet으로 반환합니다.
        /// </summary>
        /// <param name="commandText">SQL 명령문</param>
        /// <returns></returns>
        public DataSet Execute(string commandText)
        {
            DataSet ds = null;

            using (OleDbCommand cmd = new OleDbCommand(commandText, _conn))
            {
                using (OleDbDataAdapter adpt = new OleDbDataAdapter(cmd))
                {
                    ds = new DataSet();
                    adpt.Fill(ds);
                }
            }

            return ds;
        }

        /// <summary>
        /// 주어진 명령문을 실행하고, 명령에 영향받은 레코드의 수를 반환합니다.
        /// </summary>
        /// <param name="commandText">SQL 명령문</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            int count = 0;

            using (OleDbCommand cmd = new OleDbCommand(commandText, _conn))
            {
                count = cmd.ExecuteNonQuery();
            }

            return count;
        }

        /// <summary>
        /// AccessAdo 개체가 사용중인 자원을 해제합니다.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion  // METHODS


        ///////////////////////////////////////////////////////////////////////////
        //
        //  STATIC_FUNCTIONS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region STATIC_FUNCTIONS

        /// <summary>
        /// 연결 문자열을 생성합니다.
        /// </summary>
        /// <param name="filePath">Access Database파일 경로.</param>
        /// <returns>연결문자열.</returns>
        /// <remarks>CompactAndRepair함수를 사용할 경우를 위해 별도의 함수로 구현.</remarks>
        private static string GetConnectionString(string filePath)
        {
            string connstr = string.Empty;

            if (!string.IsNullOrEmpty(filePath))
            {
                if (string.Compare(Path.GetExtension(filePath), ".mdb", true) == 0)
                {
                    connstr = string.Format(_ConnectionStringFormat, "Jet", "4.0", filePath, "8.0");
                }
                else
                {
                    connstr = string.Format(_ConnectionStringFormat, "Ace", "12.0", filePath, "12.0");
                }
            }

            return connstr;
        }

        /// <summary>
        /// Access Database 파일의 크기를 최소화 합니다.
        /// </summary>
        /// <param name="filePath">Access Database 파일 경로.</param>
        /// <remarks>
        /// Access Database는 "TRUNCATE"기능이 없으며, DELETE명령문은 레코드의 삭제만을 수행할 뿐, 파일크기에는 영향을 미치지 않습니다.
        /// <list type="number">
        ///     <listheader>
        ///         <term>이 함수를 사용하기 위해서는...</term>
        ///         <description>Greenmaru.MsOffice.Excel.ExcelFile과는 관련없는 기능이므로 주석처리 했습니다.</description>
        ///     <item>
        ///         <term>참조추가</term>
        ///         <description>Add References - "COM - Microsoft Jet and Replication Objects 2.x Library"</description>
        ///     </item>
        ///     <item>
        ///         <term>주석해제</term>
        ///         <description>이 함수의 구현부 주석을 해제하고, Obsolete속성을 제거합니다.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [Obsolete("This function was not implemented. Please see remarks.", true)]
        public static void CompactAndRepair(string filePath)
        {
            /*
            FileInfo fiSource = new FileInfo(filePath);

            string tempFilePath = Path.Combine(fiSource.Directory.FullName, Path.GetRandomFileName() + fiSource.Extension);

            FileInfo fiTemp = new FileInfo(tempFilePath);

            JRO.JetEngine jeteng = new JRO.JetEngine();

            string srcfile = GetConnectionString(fiSource.FullName);
            string tempfile = GetConnectionString(fiTemp.FullName);

            jeteng.CompactDatabase(srcfile, tempfile);

            fiTemp.CopyTo(fiSource.FullName, true);
            fiTemp.Delete();
            */
        }

        #endregion  // STATIC_FUNCTIONS
    }
}
