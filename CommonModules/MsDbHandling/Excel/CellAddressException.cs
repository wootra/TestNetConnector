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
 * Latest update: 2011-10-16
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    /// <summary>
    /// 셀 주소와 관련된 오류를 표현합니다.
    /// </summary>
    public class CellAddressException : Exception
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 CellAddressException 개체 인스턴스를 초기화 합니다.
        /// </summary>
        public CellAddressException()
        {
        }

        /// <summary>
        /// 새로운 CellAddressException 개체 인스턴스를, 지정된 오류 메시지를 통해 초기화 합니다.
        /// </summary>
        /// <param name="message">오류 메시지.</param>
        public CellAddressException(string message)
            : base(message)
        {
        }

        #endregion  // CONSTRUCTION
    }
}
