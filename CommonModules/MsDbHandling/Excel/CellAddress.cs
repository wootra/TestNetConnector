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
    /// Excel Worksheet 셀 주소를 표현하기 위한 개체입니다.
    /// </summary>
    public class CellAddress : IComparable<CellAddress>, IComparable
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER VARIABLES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_VARIABLES

        /// <summary>
        /// 유효하지 않은 Worksheet 이름 값.
        /// </summary>
        protected static readonly string _InvalidSheetName = string.Empty;

        /// <summary>
        /// 유효하지 않은 열 이름 값.
        /// </summary>
        protected static readonly string _InvalidColumn = string.Empty;

        /// <summary>
        /// 유효하지 않은 행 값.
        /// </summary>
        /// <remarks>Excel의 행은 1부터 시작하는 배열입니다.</remarks>
        protected static readonly int _InvalidRow = 0;

        /// <summary>
        /// 유효하지 않은 열 순번 값.
        /// </summary>
        /// <remarks>Excel의 열은 1부터 시작하는 배열입니다.</remarks>
        protected static readonly int _InvalidColumnIndex = 0;

        /// <summary>
        /// Excel Workbook 최대 행. Excel 2007 기준입니다.
        /// </summary>
        public static readonly int MaxRow = 1048576;

        /// <summary>
        /// Excel Workbook 최대 열. Excel 2007 기준입니다.
        /// </summary>
        public static readonly string MaxColumn = "XFD";

        /// <summary>
        /// Excel Workbook 최대 열 순번 값. Excel 2007 기준입니다.
        /// </summary>
        public static readonly int MaxColumnIndex = 16384;

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를 초기화 합니다.
        /// </summary>
        private CellAddress()
            :this(_InvalidSheetName, _InvalidColumn, _InvalidRow)
        {
            // do nothing.
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, 또 다른 CellAddress개체를 통해 초기화 합니다.
        /// </summary>
        /// <param name="other">복사할 CellAddress 개체.</param>
        public CellAddress(CellAddress other)
            : this(other.SheetName, other.Column, other.Row)
        {
            // do nothing.
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, 셀 주소 문자열을 통해 초기화 합니다.
        /// </summary>
        /// <param name="address">셀 주소 문자열. <example>&quot;Sheet1!A1&quot;...&quot;Sheet1!XFD1048576&quot;</example></param>
        public CellAddress(string address)
        {
            InitializeFromAddressString(address);
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, 지정된 열과 행을 통해 초기화 합니다.
        /// </summary>
        /// <param name="column">A, B, C...로 지정된 열 이름 값.</param>
        /// <param name="row">1부터 시작하는 행 값.</param>
        public CellAddress(string column, int row)
            : this(CellAddress._InvalidSheetName, column, row)
        {
            // do nothing.
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, 지정된 열과 행을 통해 초기화 합니다.
        /// </summary>
        /// <param name="columnIndex">1부터 시작하는 열 순번 값.</param>
        /// <param name="row">1부터 시작하는 행 값.</param>
        public CellAddress(int columnIndex, int row)
            : this(CellAddress._InvalidSheetName, columnIndex, row)
        {
            // do nothing.
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, Worksheet 이름 및 열, 행 값을 통해 초기화 합니다.
        /// </summary>
        /// <param name="sheetName">Worksheet 이름.</param>
        /// <param name="column">A, B, C...로 지정된 열 이름 값.</param>
        /// <param name="row">1부터 시작하는 행 값.</param>
        public CellAddress(string sheetName, string column, int row)
        {
            SheetName = sheetName;
            Column = column;
            Row = row;
        }

        /// <summary>
        /// 새로운 CellAddress 개체 인스턴스를, Worksheet 이름 및 열, 행 값을 통해 초기화 합니다.
        /// </summary>
        /// <param name="sheetName">Worksheet 이름.</param>
        /// <param name="columnIndex">1부터 시작하는 열 순번 값.</param>
        /// <param name="row">1부터 시작하는 행 값.</param>
        public CellAddress(string sheetName, int columnIndex, int row)
        {
            SheetName = sheetName;
            ColumnIndex = columnIndex;
            Row = row;
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// 셀 주소 문자열.
        /// <example>&quot;A1&quot;...&quot;XFD1048576&quot;</example>
        /// </summary>
        public string Address
        {
            get
            {
                string shortAddress = string.Empty;

                if (Column != CellAddress._InvalidColumn && Row != CellAddress._InvalidRow)
                {
                    shortAddress = string.Format("{0}{1}", Column, Row);
                }

                return shortAddress;
            }
            set
            {
                InitializeFromAddressString(value);
            }
        }

        /// <summary>
        /// 시트 이름을 포함한 셀 주소 문자열.
        /// <example>&quot;Sheet1!A1&quot;...&quot;Sheet1!XFD1048576&quot;</example>
        /// </summary>
        public string AddressLong
        {
            get
            {
                string longAddress = string.Empty;
                string shortAddress = Address;

                if (string.IsNullOrEmpty(SheetName))
                {
                    longAddress = shortAddress;
                }
                else if (!string.IsNullOrEmpty(shortAddress))
                {
                    longAddress = string.Format("{0}!{1}", SheetName, shortAddress);
                }

                return longAddress;
            }
            set
            {
                InitializeFromAddressString(value);
            }
        }

        /// <summary>
        /// &quot;R1C1&quot; 형식의 셀 주소 문자열.
        /// <example>A1 = R1C1, B2 = R2C2</example>
        /// </summary>
        public string AddressR1C1
        {
            get
            {
                if (Column == CellAddress._InvalidColumn || Row <= CellAddress._InvalidRow)
                    return string.Empty;

                return string.Format("R{0}C{1}", Row, ColumnIndex);
            }
        }

        /// <summary>
        /// 영역 주소 형식의 셀 주소 문자열.
        /// Gets or sets the string that represent a according to expression of cell range address.
        /// <example>&quot;$A$1&quot;...&quot;$XFD$1048576&quot;</example>
        /// </summary>
        public string AddressRangeExpression
        {
            get
            {
                if (Column == CellAddress._InvalidColumn || Row <= CellAddress._InvalidRow)
                    return string.Empty;

                return string.Format("${0}${1}", Column, Row);
            }

            set
            {
                InitializeFromAddressString(value);
            }
        }

        /// <summary>
        /// 1부터 시작하는 행 값.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// A, B, C...로 지정된 열 이름 값.
        /// <example>&quot;A&quot;...&quot;XFD&quot;</example>
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 1부터 시작하는 열 값.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                int columnIndex = CellAddress._InvalidColumnIndex;
                string columnKey = Column;

                if (string.IsNullOrEmpty(columnKey))
                    return columnIndex;

                int x = columnKey.Length - 1;

                // Converts the specified string to integer that represents a index of column.
                for (int i = 0; i < columnKey.Length; i++)
                {
                    columnIndex += (Convert.ToInt32(columnKey[i]) - 64) * Convert.ToInt32(Math.Pow(26d, Convert.ToDouble(x--)));
                }

                return columnIndex;
            }

            set
            {
                string resultKey = CellAddress._InvalidColumn;

                if (value > CellAddress._InvalidColumnIndex)
                {
                    // Converts the specified integer to string that represents a key of column.
                    for (int i = Convert.ToInt32(Math.Log(Convert.ToDouble(25d * (Convert.ToDouble(value) + 1d))) / Math.Log(26d)) - 1; i >= 0; i--)
                    {
                        int x = Convert.ToInt32(Math.Pow(26d, i + 1d) - 1d) / 25 - 1;
                        if (value > x)
                        {
                            resultKey += (char)(((value - x - 1) / Convert.ToInt32(Math.Pow(26d, i))) % 26d + 65d);
                        }
                    }
                }

                Column = resultKey;
            }
        }

        /// <summary>
        /// Worksheet 이름.
        /// </summary>
        public string SheetName { get; set; }

        #endregion  // PROPERTIES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  OVERRIDINGS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region OVERRIDINGS

        /// <summary>
        /// Determines whether this instance and another specified object have the same value.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if the value of the obj parameter is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is string)
            {
                return AddressLong == (string)obj;
            }
            else if (obj is CellAddress)
            {
                return (obj as CellAddress).AddressLong == AddressLong;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a System.String that represents the current CellAddress.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AddressLong;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return AddressLong.GetHashCode();
        }

        #endregion  // OVERRIDINGS


        ///////////////////////////////////////////////////////////////////////////
        //
        //  IMPLEMANTATION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region IMPLEMANTATION

        ///////////////////////////////////////////////////////////////////////////
        //  IComparable<CellAddress>
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compares the current CellAddress with another CellAddress and returns
        /// an integer that indicates their relative position in the sort order.
        /// </summary>
        /// <param name="other">The CellAddress to compare.</param>
        /// <returns></returns>
        int IComparable<CellAddress>.CompareTo(CellAddress other)
        {
            return string.Compare(AddressLong, other.AddressLong);
        }

        ///////////////////////////////////////////////////////////////////////////
        //  IComparable
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compares the current CellAddress with another object and returns
        /// an integer that indicates their relative position in the sort order.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj is string)
            {
                return string.Compare(AddressLong, (string)obj);
            }
            else if (obj is CellAddress)
            {
                return ((IComparable<CellAddress>)this).CompareTo((CellAddress)obj);
            }
            else
            {
                return -1;
            }
        }

        #endregion  // IMPLEMANTATION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER FUNCTIONS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_FUNCTIONS

        /// <summary>
        /// 셀 주소 문자열을 분석하여 현재 개체의 값을 초기화 합니다.
        /// </summary>
        /// <param name="addrString">셀 주소 문자열.</param>
        private void InitializeFromAddressString(string addrString)
        {
            if (addrString.IndexOf('$') > -1)
            {
                // ex) "$XFD$1048576": SheetName = Same as previous value, Column = "XFD", Row = "1048576"
                string[] addrParts = addrString.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);

                if (addrParts.Length != 2)
                    throw new CellAddressException(string.Format("\"{0}\" is invalid address.", addrString));

                Column = addrParts[0];
                Row = int.Parse(addrParts[1]);
            }
            else
            {
                // ex) "Sheet1!A10": SheetName = "Sheet1", Column = "A", Row = "10"
                int sheetNameIndex = addrString.IndexOf('!');

                if (sheetNameIndex > -1)
                {
                    SheetName = addrString.Substring(0, sheetNameIndex);
                    sheetNameIndex++;
                }
                else
                {
                    sheetNameIndex = 0;
                }

                for (int i = sheetNameIndex; i < addrString.Length; i++)
                {
                    if (char.IsNumber(addrString[i]))
                    {
                        Column = addrString.Substring(sheetNameIndex, i - sheetNameIndex);
                        Row = int.Parse(addrString.Substring(i));
                        break;
                    }
                }
            }
        }

        #endregion  // MEMBER_FUNCTIONS
    }
}