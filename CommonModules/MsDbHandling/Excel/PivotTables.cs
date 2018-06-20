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
using Microsoft.Office.Interop.Excel;
#endregion  // USING_STATEMENTS

namespace MsDbHandler.Excel
{
    /// <summary>
    /// Provides a functions about the pivot tables manipulations.
    /// </summary>
    public class PivotTables
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

        public PivotTables(_Worksheet xlSheet)
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
        /// Refreshes the All PivotTable reports from the source data.
        /// </summary>
        public void Refresh()
        {
            foreach (Microsoft.Office.Interop.Excel.PivotTable pvtbl in (Microsoft.Office.Interop.Excel.PivotTables)_xlSheet.PivotTables(Type.Missing))
            {
                pvtbl.RefreshTable();
            }
        }


        /// <summary>
        /// Changes data source for the PivotTable report.
        /// </summary>
        /// <param name="sourceAddress">The range address of data source.</param>
        public void ChangeDataSource(RangeAddress sourceAddress)
        {
            foreach (Microsoft.Office.Interop.Excel.PivotTable pvtbl in (Microsoft.Office.Interop.Excel.PivotTables)_xlSheet.PivotTables(Type.Missing))
            {
                pvtbl.SourceData = sourceAddress.AddressLong;
            }
        }

        #endregion  // METHODS
    }
}
