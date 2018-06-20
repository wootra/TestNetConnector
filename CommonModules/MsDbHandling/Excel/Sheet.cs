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
 * Latest update: 2011-10-23
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    /// <summary>
    /// Excel Worksheet개체를 표현합니다.
    /// </summary>
    public class Sheet
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

        /// <summary>
        /// The reference of collection of PivotTables.
        /// </summary>
        private PivotTables _pivots;

        /// <summary>
        /// The reference of collection of Validations.
        /// </summary>
        private Validations _valdts;

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        public Sheet(_Worksheet xlSheet)
        {
            _xlSheet = xlSheet;

            _pivots = new PivotTables(xlSheet);

            _valdts = new Validations(xlSheet);
        }
        
        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// Worksheet의 이름.
        /// </summary>
        /// <exception cref="System.ArgumentException">변경하려는 Worksheet의 이름이 다른 Worksheet에서 이미 사용되고 있는 경우 발생합니다.</exception>
        public string Name
        {
            get
            {
                return _xlSheet.Name;
            }

            set
            {
                try
                {
                    _xlSheet.Name = value;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Same names already exists in Worksheets.", ex);
                }
            }
        }


        /// <summary>
        /// Worksheet의 표시 및 숨김여부.
        /// </summary>
        public bool Visible
        {
            get
            {
                return _xlSheet.Visible == XlSheetVisibility.xlSheetVisible;
            }

            set
            {
                _xlSheet.Visible = value ? XlSheetVisibility.xlSheetVisible : XlSheetVisibility.xlSheetHidden;
            }
        }


        /// <summary>
        /// 모든 Pivot table에 대한 참조.
        /// </summary>
        public PivotTables PivotTables
        {
            get { return _pivots; }
        }

        /// <summary>
        /// 모든 유효성규칙에 대한 참조.
        /// </summary>
        public Validations Validations
        {
            get { return _valdts; }
        }
        #endregion PROPERTIES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  METHODS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region METHODS

        /// <summary>
        /// 현재 Sheet를 Wrokbook의 활성 Worksheet로 설정합니다.
        /// </summary>
        public void Activate()
        {
            _xlSheet.Activate();
        }

        /// <summary>
        /// 이 Worksheet를 다른 이름으로 저장합니다.
        /// </summary>
        /// <param name="fileName">저장할 파일 이름.</param>
        /// <param name="fileFormat">파일 형식.</param>
        public void SaveAs(string fileName, XlFileFormat fileFormat)
        {
            _xlSheet.SaveAs(
                fileName,           // Filename
                fileFormat,         // FileFormat
                Type.Missing,       // Password
                Type.Missing,       // WriteResPassword
                Type.Missing,       // ReadOnlyRecommended
                Type.Missing,       // CreateBackup
                Type.Missing,       // AddToMru
                Type.Missing,       // TextCodepage
                Type.Missing,       // TextVisualLayout
                Type.Missing        // Local
                );
        }
        
        /// <summary>
        /// 현재 Worksheet에서 사용중인 영역의 주소를 구합니다.
        /// </summary>
        /// <returns>데이터가 기록된 영역의 범위 주소.</returns>
        public RangeAddress GetUsedRangeAddress()
        {
            return new RangeAddress(_xlSheet.UsedRange.get_Address(Type.Missing, Type.Missing, XlReferenceStyle.xlA1, Type.Missing, Type.Missing));
        }

        /// <summary>
        /// 데이터가 기록된 영역에서 숨겨지지 않은 영역의 주소를 구합니다.
        /// </summary>
        /// <returns>숨겨지지 않은 영역의 범위 주소 컬랙션.</returns>
        public RangeAddressCollection GetVisibleRangeAddress()
        {
            return new RangeAddressCollection(_xlSheet.UsedRange.SpecialCells(XlCellType.xlCellTypeVisible, Type.Missing).get_Address(Type.Missing, Type.Missing, XlReferenceStyle.xlA1, Type.Missing, Type.Missing));
        }


        #region GET_VALUE

        /// <summary>
        /// 지정된 셀의 값을 읽습니다.
        /// </summary>
        /// <param name="address">값을 읽을 셀의 주소.</param>
        /// <returns></returns>
        public object GetValue(CellAddress address)
        {
            return _xlSheet.get_Range(address.Address, Type.Missing).Value2;
        }

        /// <summary>
        /// 지정된 영역의 값을 읽습니다.
        /// </summary>
        /// <param name="startAddress">영역 시작 주소.</param>
        /// <param name="endAddress">영역 끝 주소.</param>
        /// <returns>지정된 범위의 값을 포함한 2차원 배열(행, 열)</returns>
        public object[,] GetValue(CellAddress startAddress, CellAddress endAddress)
        {
            return (object[,])_xlSheet.get_Range(startAddress.Address, endAddress.Address).Value2;
        }

        /// <summary>
        /// 지정된 영역의 값을 읽습니다.
        /// </summary>
        /// <param name="rangeAddress">영역 주소.</param>
        /// <returns>지정된 범위의 값을 포함한 2차원 배열(행, 열)</returns>
        public object[,] GetValue(RangeAddress rangeAddress)
        {
            return GetValue(rangeAddress.From, rangeAddress.To);
        }

        /// <summary>
        /// 지정된 셀의 값을 Excel에서 지정된 문자열 형식으로 읽습니다.
        /// </summary>
        /// <param name="address">값을 읽을 셀의 주소.</param>
        /// <returns></returns>
        public object GetFormattedValue(CellAddress address)
        {
            return _xlSheet.get_Range(address.Address, Type.Missing).Text;
        }

        /// <summary>
        /// 지정된 셀의 값을 숫자 형식으로 읽습니다.
        /// </summary>
        /// <param name="address">값을 읽을 셀의 주소.</param>
        /// <returns></returns>
        public object GetNumericValue(CellAddress address)
        {
            double? val = (double?)GetValue(address);
            return val.HasValue ? (double)val : 0d;
        }

        /// <summary>
        /// 지정된 셀의 값을 날짜 형식으로 읽습니다.
        /// </summary>
        /// <param name="address">값을 읽을 셀의 주소.</param>
        /// <returns></returns>
        public DateTime GetDatetimeValue(CellAddress address)
        {
            DateTime xldate;
            object val = GetValue(address);

            if (val is DateTime)
            {
                xldate = (DateTime)val;
            }
            else if (val is double)
            {
                xldate = DateTime.FromOADate((double)val);
            }
            else
            {
                DateTime.TryParse((string)val, out xldate);
            }

            return xldate;
        }

        #endregion  // GET_VALUE


        #region SET_VALUE

        /// <summary>
        /// 지정된 샐의 값을 설정합니다.
        /// </summary>
        /// <param name="address">셀 주소.</param>
        /// <param name="value">기록할 값.</param>
        public void SetValue(CellAddress address, object value)
        {
            _xlSheet.get_Range(address.Address, Type.Missing).Value2 = value;
        }

        /// <summary>
        /// 지정된 영역의 값을 설정합니다.
        /// </summary>
        /// <param name="rangeAddress">영역 주소.</param>
        /// <param name="sourceArray">기록할 값을 포함한 2차원 배열. 배열의 값은 Worksheet에 [행, 열]로 기록됩니다.</param>
        public void SetValue(RangeAddress rangeAddress, object[,] sourceArray)
        {
            _xlSheet.get_Range(rangeAddress.From.Address, rangeAddress.To.Address).Value2 = sourceArray;
        }

        /// <summary>
        /// 지정된 영역의 값을 설정합니다.
        /// </summary>
        /// <param name="rangeAddress">영역 주소.</param>
        /// <param name="sourceArray">당 범위에 기록할 값이 포함된 컬랙션. 영역 주소를 계산하여, 행 우선 2차원 배열로 재구성 됩니다.</param>
        public void SetValue(RangeAddress rangeAddress, IList sourceCollection)
        {
            int rowCount = rangeAddress.RowSpan;
            int colCount = rangeAddress.ColSpan;

            object[,] valArray = new object[rowCount, colCount];

            int idx = 0;

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                for (int colIndex = 0; colIndex < colCount; colIndex++)
                {
                    valArray[rowIndex, colIndex] = sourceCollection[idx++];
                }
            }

            SetValue(rangeAddress, valArray);
        }

        /// <summary>
        /// 지정된 영역의 값을 설정합니다.
        /// </summary>
        /// <param name="fromAddress">기록을 시작할 지점의 셀 주소.</param>
        /// <param name="sourceTable">기록할 값이 포함된 DataTable.</param>
        public void SetValue(CellAddress fromAddress, System.Data.DataTable sourceTable)
        {
            object[,] valArray = new object[sourceTable.Rows.Count, sourceTable.Columns.Count];

            for (int rowIndex = 0; rowIndex < sourceTable.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < sourceTable.Columns.Count; colIndex++)
                {
                    valArray[rowIndex, colIndex] = sourceTable.Rows[rowIndex][colIndex];
                }
            }

            CellAddress toAddress = new CellAddress(sourceTable.Columns.Count + fromAddress.ColumnIndex - 1, sourceTable.Rows.Count + fromAddress.Row - 1);

            SetValue(new RangeAddress(fromAddress, toAddress), valArray);
        }


        /// <summary>
        /// 지정된 영역의 값을 설정합니다.
        /// </summary>
        /// <param name="fromAddress">기록을 시작할 지점의 셀 주소.</param>
        /// <param name="sourceTable">기록할 값이 포함된 DataTable.</param>
        /// <param name="columnNames">기록할 Column이름. DataTable의 Column중, 이 배열에 포함된 Column만이 Worksheet에 기록됩니다.</param>
        public void SetValue(CellAddress fromAddress, System.Data.DataTable sourceTable, IList<string> columnNames)
        {
            object[,] valArray = new object[sourceTable.Rows.Count, columnNames.Count];

            for (int rowIndex = 0; rowIndex < sourceTable.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < columnNames.Count; colIndex++)
                {
                    valArray[rowIndex, colIndex] = sourceTable.Rows[rowIndex][columnNames[colIndex]];
                }
            }

            CellAddress toAddress = new CellAddress(columnNames.Count + fromAddress.ColumnIndex - 1, sourceTable.Rows.Count + fromAddress.Row - 1);

            SetValue(new RangeAddress(fromAddress, toAddress), valArray);
        }

        #endregion  // SET_VALUE


        #region DELETE

        /// <summary>
        /// 지정된 셀의 값을 삭제합니다.
        /// </summary>
        /// <param name="rangeAddress">삭제할 영역 주소.</param>
        /// <param name="endAddress"></param>
        public void Delete(CellAddress address)
        {
            _xlSheet.get_Range(address.Address, Type.Missing).Delete(Type.Missing);
        }

        /// <summary>
        /// 지정된 영역의 값을 삭제합니다.
        /// </summary>
        /// <param name="rangeAddress">삭제할 영역 주소.</param>
        /// <param name="endAddress"></param>
        public void Delete(RangeAddress rangeAddress)
        {
            _xlSheet.get_Range(rangeAddress.From.Address, rangeAddress.To.Address).Delete(Type.Missing);
        }

        #endregion  // DELETE

        #endregion  // METHODS
    }
}
