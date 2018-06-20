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
 * Latest update: 2011-10-21
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
    /// Sheet개체 컬랙션을 표현합니다.
    /// </summary>
    public class SheetCollection : IEnumerable<Sheet>, IEnumerable
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER VARIABLES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_VARIABLES

        /// <summary>
        /// Excel interop Workbook 개체.
        /// </summary>
        private Workbook _xlWorkbook;

        /// <summary>
        /// Sheet 컬랙션.
        /// </summary>
        /// <remarks>
        /// Worksheet의 추가, 삭제, 이름변경 등이 이루어 지므로, Dictionary류의 자료형은 사용할 수 없습니다.
        /// </remarks>
        private List<Sheet> _sheets = new List<Sheet>();

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 SheetCollection개체 인스턴스를 초기화 합니다.
        /// </summary>
        /// <param name="xlWorkbook">Excel Workbook 인스턴스 참조.</param>
        public SheetCollection(Workbook xlWorkbook)
        {
            _xlWorkbook = xlWorkbook;

            // Initialize the collection.
            foreach (_Worksheet xlsheet in _xlWorkbook.Worksheets)
            {
                _sheets.Add(new Sheet(xlsheet));
            }
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// 컬랙션에 포함된 Sheet의 갯수. 읽기전용.
        /// </summary>
        public int Count
        {
            get { return _sheets.Count; }
        }

        /// <summary>
        /// 활성 상태의 Worksheet개체 참조. 읽기전용.
        /// </summary>
        public Sheet ActiveSheet
        {
            get
            {
                return FindSheet((_xlWorkbook.ActiveSheet as Worksheet).Name);
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
        /// 컬랙션에 새로운 Sheet개체를 추가합니다. 추가된 Sheet는 Active상태가 됩니다.
        /// </summary>
        /// <returns>컬랙션에 추가된 Sheet개체 참조.</returns>
        /// <remarks>새로 추가된 Sheet의 이름(Name)은 Excel에서 임의로 지정됩니다.</remarks>
        public Sheet Add()
        {
            Sheet sheetNew = new Sheet(_xlWorkbook.Worksheets.Add(Type.Missing, Type.Missing, 1, XlSheetType.xlWorksheet) as _Worksheet);
            _sheets.Add(sheetNew);
            return sheetNew;
        }

        /// <summary>
        /// 컬랙션에 지정된 이름의 새로운 Sheet개체를 추가합니다. 추가된 Sheet는 Active상태가 됩니다.
        /// </summary>
        /// <param name="sheetName">새롭게 추가될 Sheet의 이름.</param>
        /// <returns>컬랙션에 추가된 Sheet개체 참조.</returns>
        /// <exception cref="System.ArgumentException">새로 추가되는 Worksheet의 이름이 이미 존재하는 경우 발생합니다.</exception>
        public Sheet Add(string sheetName)
        {
            Sheet sheetNew = Add();

            try
            {
                sheetNew.Name = sheetName;
            }
            catch (Exception ex)
            {
                // Same names already exists in Worksheet.
                Remove(sheetNew.Name);

                throw new ArgumentException("Same names already exists in Worksheets.", "sheetName", ex);
            }

            return sheetNew;
        }

        /// <summary>
        /// 지정된 Sheet를 삭제합니다.
        /// </summary>
        /// <param name="sheet">컬랙션에서 삭제할 Sheet 이름.</param>
        /// <exception cref="System.InvalidOperationException">Workbook에서 모든 Sheet를 삭제하려고 할 경우 발생합니다.</exception>
        public void Remove(string sheetName)
        {
            Sheet sheetRemove = FindSheet(sheetName);

            if (sheetRemove != null)
            {
                try
                {
                    (_xlWorkbook.Sheets[sheetName] as Worksheet).Delete();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("You cannot delete all worksheets in a workbook.", ex);
                }

                _sheets.Remove(sheetRemove);
            }
        }

        /// <summary>
        /// 컬랙션에서 지정된 이름의 Sheet를 참조. 읽기전용.
        /// </summary>
        /// <param name="sheetName">참조하고자 하는 Sheet의 이름.</param>
        /// <returns></returns>
        public Sheet this[string sheetName]
        {
            get
            {
                return FindSheet(sheetName);
            }
        }

        /// <summary>
        /// 지정된 이름의 Sheet가 컬랙션에 존재하는지를 확인합니다.
        /// </summary>
        /// <param name="sheetName">Sheet이름.</param>
        /// <returns>Sheet가 컬랙션에 존재하는 경우 true.</returns>
        public bool IsExist(string sheetName)
        {
            return (FindSheet(sheetName) != null);
        }

        #endregion  // METHODS


        ///////////////////////////////////////////////////////////////////////////
        //
        //  IMPLEMANTATION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region IMPLEMANTATION

        ///////////////////////////////////////////////////////////////////////////
        //  IEnumerable<Sheet>
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator<Sheet> IEnumerable<Sheet>.GetEnumerator()
        {
            return _sheets.GetEnumerator();
        }

        ///////////////////////////////////////////////////////////////////////////
        //  IEnumerable
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_sheets as IEnumerable).GetEnumerator();
        }

        #endregion  // IMPLEMANTATION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER FUNCTIONS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_FUNCTIONS

        /// <summary>
        /// 컬랙션에서 지정된 이름의 Sheet개체를 찾습니다.
        /// </summary>
        /// <param name="sheetName">Sheet 이름.</param>
        /// <returns>지정된 이름에 해당하는 Sheet 개체 참조; 존재하지 않을 경우 null</returns>
        private Sheet FindSheet(string sheetName)
        {
            Sheet result = null;
            foreach (Sheet sheet in _sheets)
            {
                if (string.Compare(sheet.Name, sheetName, true) == 0)
                {
                    result = sheet;
                    break;
                }
            }
            return result;
        }

        #endregion  // MEMBER_FUNCTIONS
    }
}