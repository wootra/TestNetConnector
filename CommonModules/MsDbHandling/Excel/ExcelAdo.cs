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


namespace MsDbHandler.Excel
{
    /// <summary>
    /// OleDb를 이용해 Excel Workbook을 읽기위한 개체.
    /// ADO구현상의 제한으로, DELETE는 사용할 수 없습니다.
    /// </summary>
    public class ExcelAdo : IOfficeAdo
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
        private static readonly string _ConnectionStringFormat = "Provider=Microsoft.{0}.OLEDB.{1};Data Source=\"{2}\";Mode=ReadWrite|Share Deny None;Extended Properties=\"Excel {3};HDR={4}\"";

        /// <summary>
        /// Excel Workbook과의 연결 개체.
        /// </summary>
        private OleDbConnection _conn;

        /// <summary>
        /// Excel Workbook파일 이름.
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
        /// 새로운 ExcelAdo개체 인스턴스를 초기화 합니다.
        /// </summary>
        public ExcelAdo()
        {
            // do nothing.
        }


        /// <summary>
        /// 새로운 AccessAdo개체 인스턴스를, 주어진 Excel Workbook 파일 이름과 해더 포함 여부를 사용해 초기화 합니다.
        /// </summary>
        /// <param name="xlFileName">Excel Workbook 파일 이름.</param>
        /// <param name="headerIncluded">Excel Worksheet의 첫번째 행을 헤더로 인식할 것인지 여부.</param>
        public ExcelAdo(string xlFileName, bool headerIncluded)
        {
            Open(xlFileName, headerIncluded);
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// Excel Workbook와의 연결을 위한 연결문자열. 읽기전용.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                string connstr = string.Empty;

                if (!string.IsNullOrEmpty(FileName))
                {
                    if (string.Compare(Path.GetExtension(FileName), ".xls", true) == 0)
                    {
                        connstr = string.Format(_ConnectionStringFormat, "Jet", "4.0", FileName, "8.0", IsHeaderIncluded ? "YES" : "NO");
                    }
                    else
                    {
                        connstr = string.Format(_ConnectionStringFormat, "Ace", "12.0", FileName, "12.0", IsHeaderIncluded ? "YES" : "NO");
                    }
                }

                return connstr;
            }
        }

        /// <summary>
        /// Excel Workbook 파일 이름.
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
        /// Excel Worksheet의 첫번째 행을 데이터가 아닌 헤더로 인식할 것인지 여부.
        /// </summary>
        public bool IsHeaderIncluded { get; set; }

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
        /// 현재 Excel Workbook와 연결된 상태인지를 나타냅니다. 읽기전용.
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
        /// Excel Workbook와 연결합니다.
        /// </summary>
        public void Open()
        {
            Open(this.FileName, this.IsHeaderIncluded);
        }

        /// <summary>
        /// 주어진 경로의 Excel Workbook와 연결합니다.
        /// </summary>
        /// <param name="xlFileName">Excel Workbook 파일 이름.</param>
        /// <param name="headerIncluded">Excel Worksheet의 첫번째 행을 헤더로 인식할 것인지 여부.</param>
        public void Open(string xlFileName, bool headerIncluded)
        {
            Close();
            this.FileName = FileName;
            this.IsHeaderIncluded = headerIncluded;

            _conn = new OleDbConnection(this.ConnectionString);
            _conn.Open();
        }

        /// <summary>
        /// Excel Workbook와의 연결을 닫습니다.
        /// </summary>
        public void Close()
        {
            if (_conn != null)
            {
                if (IsOpened)
                {
                    _conn.Close();
                }
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
        /// <remarks>ADO구현상의 제한으로, DELETE는 사용할 수 없습니다.</remarks>
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
        /// ExcelAdo 개체가 사용중인 자원을 해제합니다.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion  // METHODS
    }
}
