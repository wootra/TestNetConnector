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
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
#endregion  // USING_STATEMENTS


namespace MsDbHandler.Excel
{
    /// <summary>
    /// MS Excel Workbook 조작을 위한 기능을 제공합니다.
    /// </summary>
    public class ExcelFile : IOfficeFile
    {
        ///////////////////////////////////////////////////////////////////////////
        //
        //  MEMBER VARIABLES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region MEMBER_VARIABLES

        /// <summary>
        /// Microsoft.Office.Interop.Excel.Application 개체 인스턴스.
        /// </summary>
        private _Application _xlApp;

        /// <summary>
        /// Worksheet컬랙션.
        /// </summary>
        private SheetCollection _sheets;

        #endregion  // MEMBER_VARIABLES


        ///////////////////////////////////////////////////////////////////////////
        //
        //  CONSTRUCTION
        //
        ///////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTION

        /// <summary>
        /// 새로운 ExcelFile 개체 인스턴스를 초기화 합니다.
        /// </summary>
        private ExcelFile()
        {
            _xlApp = new Application();

            _xlApp.Visible = false;

            // Do not disturbed by prompts and alert messages while a program is running.
            _xlApp.DisplayAlerts = false;
        }

        #endregion  // CONSTRUCTION


        ///////////////////////////////////////////////////////////////////////////
        //
        //  PROPERTIES
        //
        ///////////////////////////////////////////////////////////////////////////
        #region PROPERTIES

        /// <summary>
        /// 활성 상태의 Workbook개체 참조. 읽기전용.
        /// </summary>
        /// <remarks>20111021: 이 클래스는 여러개의 Workbook으로 구성된 통합 문서에 대한 조작은 고려하지 않고 있습니다.</remarks>
        protected Workbook ActiveWorkbook
        {
            get { return _xlApp.ActiveWorkbook; }
        }


        /// <summary>
        /// Worksheets 컬랙션에 대한 참조. 읽기전용.
        /// </summary>
        public SheetCollection Sheets
        {
            get { return _sheets; }
        }

        /// <summary>
        /// Excel Workbook파일의 경로. 읽기전용.
        /// </summary>
        public string Path
        {
            get
            {
                return ActiveWorkbook.Path;
            }
        }

        /// <summary>
        /// Excel Workbook파일의 경로를 제외한 이름. 읽기전용.
        /// </summary>
        public string Name
        {
            get
            {
                return ActiveWorkbook.Name;
            }
        }

        /// <summary>
        /// Excel Workbook파일의 경로를 포함한 전체 이름. 읽기전용.
        /// </summary>
        public string FullName
        {
            get
            {
                return ActiveWorkbook.FullName;
            }
        }


        /// <summary>
        /// 땡기는 녀석-_-
        /// </summary>
        public bool Saved
        {
            get
            {
                return ActiveWorkbook.Saved;
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
        /// 변경된 모든 사항을 Workbook 파일에 저장합니다.
        /// </summary>
        public void Save()
        {
            ActiveWorkbook.Save();
        }


        /// <summary>
        /// 변경된 모든 사항을 지정된 경로의 Workbook 파일에 저장합니다.
        /// </summary>
        /// <param name="fileName">파일을 저장할 경로.</param>
        public void SaveAs(string fileName)
        {
            // NOTE:
            // If you want save to previous version of Excel 2007 file format,
            // set XlFileFormat.xlWorkbookNormal.
            SaveAs(fileName, XlFileFormat.xlWorkbookDefault);
        }

        /// <summary>
        /// 변경된 모든 사항을 지정된 경로에 지정된 형식으로 저장합니다.
        /// </summary>
        /// <param name="type">저장할 파일의 형식.</param>
        /// <param name="fileName">파일을 저장할 경로.</param>
        public void SaveAs(string fileName, XlFileFormat fileFormat)
        {
            ActiveWorkbook.SaveAs(
                fileName,                       // Filename
                fileFormat,                     // FileFormat
                Type.Missing,                   // Password
                Type.Missing,                   // WriteResPassword
                Type.Missing,                   // ReadOnlyRecommended
                Type.Missing,                   // CreateBackup
                XlSaveAsAccessMode.xlNoChange,  // AccessMode
                Type.Missing,                   // ConflictResolution
                Type.Missing,                   // AddToMru
                Type.Missing,                   // TextCodepage
                Type.Missing,                   // TextVisualLayout
                Type.Missing                    // Local
                );
        }

        /// <summary>
        /// Workbook을 PDF파일 형식으로 저장합니다.
        /// </summary>
        /// <param name="fileName">파일 이름을 포함한 저장 경로.</param>
        public void ExportAsPdf(string fileName)
        {
            ActiveWorkbook.ExportAsFixedFormat(
                XlFixedFormatType.xlTypePDF,            // Type
                fileName,                               // Filename
                XlFixedFormatQuality.xlQualityStandard, // Quality
                Type.Missing,                           // IncludeDocProperties
                Type.Missing,                           // IgnorePrintAreas
                Type.Missing,                           // From
                Type.Missing,                           // To
                false,                                  // OpenAfterPublish
                Type.Missing                            // FixedFormatExtClassPtr
                );
        }

        /// <summary>
        /// System.Data.DataSet에 포함된 모든 데이터를 Workbook에 기록합니다.
        /// <list type="bullet">
        ///     <item>
        ///         <term>DataTable</term>
        ///         <description>DataSet에 포함된 DataTable과 동일한 이름의 Worksheet가 생성됩니다.</description>
        ///     </item>
        ///     <item>
        ///         <term>Records</term>
        ///         <description>DataTable에 포함된 모든 데이터는 동일한 이름의 Worksheet에 복사됩니다.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="sourceDataSet">데이터 원본 DataSet 개체.</param>
        /// <param name="headerInclude">첫번째 행을 헤더로 사용할 것인지 여부. True인 경우, Worksheet의 첫번째 행은 해더로 사용됩니다.</param>
        public void DataSetToExcel(System.Data.DataSet sourceDataSet, bool headerInclude)
        {
            foreach (System.Data.DataTable dt in sourceDataSet.Tables)
            {
                Sheet target = _sheets.IsExist(dt.TableName) ? _sheets[dt.TableName] : _sheets.Add(dt.TableName);

                if (headerInclude == true)
                {
                    object[] headerName = new object[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        headerName[i] = dt.Columns[i].ColumnName;
                    }

                    target.SetValue(new RangeAddress(new CellAddress("A1"), new CellAddress(headerName.Length, 1)), headerName);
                }

                target.SetValue(new CellAddress(dt.TableName, "A", headerInclude ? 2 : 1), dt);
            }
        }

        /// <summary>
        /// 현재 개체에서 사용중인 모든 리소스 점유를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            if (_xlApp != null)
            {
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlApp);
                _xlApp = null;

                // NOTE
                // Invoke the Garbage Collector immediately.
                // Otherwise, process of Excel Application for current object are
                // still alive until next GC's operation.
                GC.Collect();
            }
        }
        #endregion  // METHODS


        ///////////////////////////////////////////////////////////////////////////
        //
        //  OVERRIDINGS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region OVERRIDINGS


        /// <summary>
        /// Returns a string that represents the current ExcelFile object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }

        #endregion  // OVERRIDINGS


        ///////////////////////////////////////////////////////////////////////////
        //
        //  STATIC_FUNCTIONS
        //
        ///////////////////////////////////////////////////////////////////////////
        #region STATIC_FUNCTIONS

        /// <summary>
        /// 지정된 경로의 Excel workbook파일을 ExcelFile개체를 사용해 엽니다.
        /// </summary>
        /// <param name="fileName">Excel workbook파일 이름.</param>
        /// <returns></returns>
        public static ExcelFile Open(string fileName)
        {
            return ExcelFile.Open(fileName, false, string.Empty);
        }

        /// <summary>
        /// 지정된 경로의 Excel workbook파일을 ExcelFile개체를 사용해 엽니다.
        /// </summary>
        /// <param name="fileName">Excel workbook파일 이름.</param>
        /// <param name="readOnly">파일을 읽기 전용으로 열 것인지 여부.</param>
        /// <returns></returns>
        public static ExcelFile Open(string fileName, bool readOnly)
        {
            return ExcelFile.Open(fileName, readOnly, string.Empty);
        }


        /// <summary>
        /// 지정된 경로의 Excel workbook파일을 ExcelFile개체를 사용해 엽니다.
        /// </summary>
        /// <param name="fileName">Excel workbook파일 이름.</param>
        /// <param name="readOnly">파일을 읽기 전용으로 열 것인지 여부.</param>
        /// <param name="password">비밀번호.</param>
        /// <returns></returns>
        public static ExcelFile Open(string fileName, bool readOnly, string password)
        {
            ExcelFile xlobj = new ExcelFile();

            xlobj._xlApp.Workbooks.Open(
                fileName,           // Filename
                false,              // UpdateLinks
                readOnly,           // ReadOnly
                Type.Missing,       // Format
                password,           // Password
                Type.Missing,       // WriteResPassword
                Type.Missing,       // IgnoreReadOnlyRecommended
                Type.Missing,       // Origin
                Type.Missing,       // Delimiter
                false,              // Editable
                Type.Missing,       // Notify
                Type.Missing,       // Converter
                false,              // AddToMru
                Type.Missing,       // Local
                Type.Missing        // CorruptLoad
                );

            xlobj._sheets = new SheetCollection(xlobj.ActiveWorkbook);
            
            return xlobj;
        }


        /// <summary>
        /// 새로운 Excel workbook파일을 ExcelFile개체를 사용해 생성합니다.
        /// </summary>
        /// <returns>새로운 ExcelFile 개체 인스턴스.</returns>
        public static ExcelFile CreateNew()
        {
            ExcelFile xlobj = new ExcelFile();

            xlobj._xlApp.Workbooks.Add(Type.Missing);

            xlobj._sheets = new SheetCollection(xlobj.ActiveWorkbook);

            return xlobj;
        }

        #endregion  // STATIC_FUNCTIONS
    }
}