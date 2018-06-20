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
 * Latest update: 2011-10-12
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
using System.Data;
#endregion  // USING_STATEMENTS


namespace MsDbHandler
{
    /// <summary>
    /// ADO에 기반하여 MS Office파일을 조작하기 위한 개체입니다.
    /// </summary>
    public interface IOfficeAdo : IDisposable
    {
        /// <summary>
        /// 파일을 열기위한 연결문자열.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 주어진 연결문자열을 사용해, 파일과 연결합니다.
        /// </summary>
        void Open();

        /// <summary>
        /// 파일과의 연결을 닫습니다.
        /// </summary>
        void Close();

        /// <summary>
        /// 주어진 명령문을 실행합니다.
        /// </summary>
        /// <param name="commandText">실행할 SQL. 혹은, MS Office 종속적 명령문.</param>
        /// <returns>명령문을 실행한 결과값이 포함된 DataSet 개체.</returns>
        DataSet Execute(string commandText);

        /// <summary>
        /// 주어진 명령문을 실행합니다.
        /// </summary>
        /// <param name="commandText">실행할 SQL. 혹은, MS Office 종속적 명령문.</param>
        /// <returns>
        /// UPDATE, INSERT, 또는 DELETE의 경우 명령문에 영향을 받은 레코드의 갯수.
        /// 그 이외의 명령문인 경우는 -1.
        /// </returns>
        int ExecuteNonQuery(string commandText);
    }
}
