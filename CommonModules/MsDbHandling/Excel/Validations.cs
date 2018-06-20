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
 * Latest update: 2011-10-31
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    public class Validations
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER VARIABLES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_VARIABLES

        /// <summary>
        /// Excel interop Worksheet object.
        /// </summary>
        private _Worksheet _xlSheet;

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        public Validations(_Worksheet xlSheet)
        {
            _xlSheet = xlSheet;
        }
        
        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  METHODS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region METHODS


        /// <summary>
        /// 지정된 영역에 설정된 유효성 검사 규칙을 삭제합니다.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="endAddress"></param>
        public void Delete(RangeAddress rangeAddress)
        {
            Validation validation = _xlSheet.get_Range(rangeAddress.From.Address, rangeAddress.To.Address).Validation;
            if (validation != null)
            {
                validation.Delete();
                Marshal.ReleaseComObject(validation);
            }
        }

        /// <summary>
        /// 지정된 영역에 이름을 설정합니다.
        /// </summary>
        /// <param name="namedRangeName">영역 이름.</param>
        /// <param name="rangeAddress">영역 주소.</param>
        public void SetNamedRanges(string namedRangeName, RangeAddress rangeAddress)
        {
            _xlSheet.Names.Add(namedRangeName, _xlSheet.get_Range(rangeAddress.From.Address, rangeAddress.To.Address), true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        /// <summary>
        /// 지정된 영역에 "목록 유효성 검사" 규칙을 추가합니다.
        /// </summary>
        /// <param name="namedRangeName">유효성 검사 규칙에 사용할 명명된 영역 이름.</param>
        /// <param name="rangeAddress">유효성 검사 규칙을 적용할 영역 주소.</param>
        public void SetNamedRangeListValidation(string namedRangeName, RangeAddress rangeAddress)
        {
            string refCellName = namedRangeName.StartsWith("=") ? namedRangeName : "=" + namedRangeName;
            Validation validation = _xlSheet.get_Range(rangeAddress.From.Address, rangeAddress.To.Address).Validation;
            validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertStop, XlFormatConditionOperator.xlBetween, refCellName, Type.Missing);

            Marshal.ReleaseComObject(validation);
        }

        #endregion METHODS
    }
}
