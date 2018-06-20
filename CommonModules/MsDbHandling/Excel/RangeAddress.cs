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
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    /// <summary>
    /// Excel Worksheet 셀 영역주소를 표현하기 위한 개체입니다.
    /// </summary>
    public class RangeAddress : IComparable<RangeAddress>, IComparable
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 RangeAddress 개체 인스턴스를, 시작 주소와 끝 주소를 사용해 초기화 합니다.
        /// </summary>
        /// <param name="fromAddress">영역 시작 주소.</param>
        /// <param name="toAddress">영역 끝 주소.</param>
        public RangeAddress(CellAddress fromAddress, CellAddress toAddress)
        {
            this.From = new CellAddress(fromAddress);
            this.To = new CellAddress(toAddress);
        }

        /// <summary>
        /// 새로운 RangeAddress 개체 인스턴스를, 영역주소 표현식 문자열을 사용해 초기화 합니다.
        /// </summary>
        /// <param name="rangeExp">
        /// 영역주소 표현식 문자열.
        /// <example>&quot;$A$1:$XFD$1048576&quot;</example>
        /// </param>
        public RangeAddress(string rangeExp)
        {
            Address = rangeExp;
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// 영역 시작 주소.
        /// </summary>
        public CellAddress From { get; private set; }

        /// <summary>
        /// 영역 끝 주소.
        /// </summary>
        public CellAddress To { get; private set; }

        /// <summary>
        /// 현재 RangeAddress의 영역주소 표현식 문자열.
        /// <example>&quot;$A$1:XFD$1048576&quot;</example>
        /// </summary>
        public string Address
        {
            get
            {
                return string.Format("{0}:{1}", From.AddressRangeExpression, To.AddressRangeExpression);
            }
            set
            {
                string[] addrs = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (addrs.Length == 1)
                {
                    From = new CellAddress(addrs[0]);
                    To = new CellAddress(addrs[0]);
                }
                else if (addrs.Length == 2)
                {
                    From = new CellAddress(addrs[0]);
                    To = new CellAddress(addrs[1]);
                }
                else
                {
                    throw new CellAddressException(string.Format("\"{0}\" is invalid address.", value));
                }
            }
        }

        /// <summary>
        /// Sheet 이름을 포함한 영역의 주소입니다.
        /// </summary>
        public string AddressLong
        {
            get
            {
                return string.Format("{0}!{1}:{2}", From.SheetName, From.Address, To.Address);
            }
        }

        /// <summary>
        /// 지정된 영역에 포함된 열 갯수.
        /// </summary>
        public int ColSpan
        {
            get { return (To.ColumnIndex - From.ColumnIndex) + 1; }
        }

        /// <summary>
        /// 지정된 영역에 포함된 행 갯수.
        /// </summary>
        public int RowSpan
        {
            get { return (To.Row - From.Row) + 1; }
        }

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
                return this.Address == (string)obj;
            }
            else if (obj is CellAddress)
            {
                RangeAddress other = obj as RangeAddress;
                return (other.Address == Address);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a System.String that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Address;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Address.GetHashCode();
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
        /// Compares the current object with another RangeAddress and returns
        /// an integer that indicates their relative position in the sort order.
        /// </summary>
        /// <param name="other">The RangeAddress to compare.</param>
        public int CompareTo(RangeAddress other)
        {
            return string.Compare(Address, other.Address);
        }


        ///////////////////////////////////////////////////////////////////////////
        //  IComparable
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compares the current object with another object and returns
        /// an integer that indicates their relative position in the sort order.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is string)
            {
                return string.Compare(Address, (string)obj);
            }
            else if (obj is CellAddress)
            {
                return ((IComparable<RangeAddress>)this).CompareTo((RangeAddress)obj);
            }
            else
            {
                return -1;
            }
        }

        #endregion  // IMPLEMANTATION
    }
}
