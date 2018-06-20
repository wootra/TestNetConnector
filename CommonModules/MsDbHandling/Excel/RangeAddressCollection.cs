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
using System.Collections.Generic;
using System.Text;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    /// <summary>
    /// RangeAddress 컬랙션 개체.
    /// </summary>
    public class RangeAddressCollection : List<RangeAddress>
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 RangeAddressCollection 개체 인스턴스를, 주소 표현식 문자열을 사용해 초기화 합니다.
        /// </summary>
        /// <param name="addressExp">
        /// 영역 주소 집합표현식 문자열. ','문자로 구분됩니다.
        /// <example>
        /// A1:B1,X1:X2....
        /// </example>
        /// </param>
        public RangeAddressCollection(string addressExp)
        {
            string[] ranges = addressExp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string addr in ranges)
            {
                Add(new RangeAddress(addr));
            }
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  OVERRIDINGS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region OVERRIDINGS

        /// <summary>
        /// Returns a System.String that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RangeAddress item in this)
            {
                sb.AppendFormat("{0},", item);
            }

            return sb.ToString().TrimEnd(new char[] { ',' });
        }

        #endregion  // OVERRIDINGS
    }
}
