using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlHandlers;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Collections;


namespace HtmlReportMaker
{
    public class HtmlTable : HtmlElement, IHtmlReportElement
    {
        XmlElement _tableElement;
        XmlDocument _xDoc;
        public List<int> Widths = new List<int>();
        public List<HtmlTableRow> Rows = new List<HtmlTableRow>();
        
        /// <summary>
        /// 앞으로 추가되는 Row의 기본 Height를 정한다. 만일 다른 Height를 정하고 싶다면 각각의 RowHeight를 넣어주면 된다.
        /// </summary>
        public int BaseHeightForRows { get; set; }
        int _minHeight = 10;
        internal HtmlTable(ICollection<int> widths, ICollection<string> colNames, int baseHeightForRows = 30)
        {
            foreach (int wid in widths)
            {
                Widths.Add(wid);
            }
            if (baseHeightForRows < _minHeight) throw new Exception("baseHeightForRows for HtmlTable must be bigger than " + _minHeight);
            BaseHeightForRows = baseHeightForRows;
            if (colNames != null)
            {
                foreach (string name in colNames)
                {
                    _fieldTitles.Add(name);
                }
            }
            else
            {
                for(int i=0; i<widths.Count; i++)
                {
                    _fieldTitles.Add("col"+i);
                }
            }
            
        }

        string _tableName="";
        /// <summary>
        /// 스타일을 적용할 id를 table에서 사용할 것인지 나타낸다.
        /// </summary>
        public void UseDynamicEffects(String tableName)
        {
            if (tableName == null) _tableName = "";
            else _tableName = tableName;
        }

        public string TableName { get { return _tableName; } }

        public string getCellID(int row, int col)
        {
            return Rows[row].Cells[col].TableCellName;
        }
        
        public void GetXml(XmlDocument xDoc, XmlNode body)
        {
            XmlElement table = XmlAdder.Element(xDoc, "Table", body);
            if (_tableName.Length > 0) XmlAdder.Attribute(xDoc, "id", _tableName, table);
            if (Style != null && Style.Length > 0) XmlAdder.Attribute(xDoc, "Style", Style+"border-collapse:collapse;border:none;", table);
             XmlAdder.Attribute(xDoc, "border", "0", table);
             XmlAdder.Attribute(xDoc, "cellspacing", "0", table);
             XmlAdder.Attribute(xDoc, "cellpadding", "0", table);
            
            base.GetXmlForThis(xDoc, table);

            #region width의 기준이 되는 row를 그려준다.
            XmlElement widRow = XmlAdder.Element(xDoc, "TR", table);
            XmlAdder.Attribute(xDoc, "Height", "1", widRow);

            //for (int i = 0; i < Widths.Count; i++)
            int totalWid = 0;
            foreach(int wid in Widths)
            {
                XmlElement td = XmlAdder.Element(xDoc, "TD", widRow);
                td.InnerText = "";
                if (wid >= 0)
                {
                    XmlAdder.Attribute(xDoc, "Width", wid.ToString(), td);
                    totalWid += wid;
                }
            }
            #endregion
            if (Width >= 0 && Width > totalWid)
            {
                XmlAdder.Attribute(xDoc, "Width", Width.ToString(), table);
            }

            int rowCount = 0;
            foreach (HtmlTableRow row in Rows)
            {
                
                if (row.BorderWidths == null) row.BorderWidths = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                if (row.BorderColors == null) row.BorderColor = Color.Empty;
                
                if (BorderWidths != null)
                {
                    if (rowCount == 0)//처음
                    {
                        row.BorderWidths.Top = BorderWidths.Top;
                        if (BorderColors != null) row.BorderColors[2] = BorderColors[2];
                    }
                    else if (rowCount == Rows.Count - 1) //마지막
                    {
                        row.BorderWidths.Bottom = BorderWidths.Bottom;
                        if (BorderColors != null) row.BorderColors[3] = BorderColors[3];
                    }
                    
                    row.BorderWidths.Left = BorderWidths.Left;
                    if (BorderColors != null) row.BorderColors[0] = BorderColors[0];
                    row.BorderWidths.Right = BorderWidths.Right;
                    if (BorderColors != null) row.BorderColors[1] = BorderColors[1];
                }
                row.GetXml(xDoc, table);
                rowCount++;
            }
        }

        List<string> _fieldTitles = new List<string>();

        public string FieldTitle(int index)
        {
            return _fieldTitles[index];
        }

        public List<String> FieldTitles
        {
            get { return _fieldTitles; }
        }

        public List<object> FieldTitlesObjs
        {
            get {
                List<Object> obj = new List<object>();
                foreach (string title in _fieldTitles)
                {
                    obj.Add(title);
                }
                return obj;
            }
        }
        
        /// <summary>
        /// 처음 정한 Row를 header로 그 content를 header의 title로 해서 몇번째 title인지를 가져온다.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public int GetFieldIndex(string headerName)
        {
            return _fieldTitles.IndexOf(headerName);
        }

        public void SetFieldWid(int index, int size)
        {
            Widths[index] = size;
        }

        public void SetFieldWid(string headerName, int size)
        {
            int index = _fieldTitles.IndexOf(headerName);
            if (index >= 0)
            {
                Widths[index] = size;
            }
        }

        


        /// <summary>
        /// 줄을 한 줄 추가한다. span을 정해주면 바로 cell을 merge한다.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="cellsSpan">merge할 cell 크기를 나타낸다. 총 column이 5개에서 3개, 2개로 merge하고 싶다면, int[]{3,2}를 넣으면 된다.</param>
        /// <param name="height"></param>
        /// <returns></returns>
        public HtmlTableRow AddRow(ICollection<object> contents, ICollection<int> cellsSpan=null, int height = -1)
        {
            HtmlTableRow row;
            if (height < 0) height = BaseHeightForRows;
            else if (height < _minHeight) throw new Exception("height for HtmlTable must be bigger than " + _minHeight);

            row = new HtmlTableRow(this, (height < 0) ? BaseHeightForRows : height, contents, cellsSpan);
            
            Rows.Add(row);
            return row;
        }

        /// <summary>
        /// 줄을 한 줄 추가한다. span을 정해주면 바로 cell을 merge한다.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="cellsSpan">merge할 cell 크기를 나타낸다. 총 column이 5개에서 3개, 2개로 merge하고 싶다면, int[]{3,2}를 넣으면 된다.</param>
        /// <param name="height"></param>
        /// <returns></returns>
        public HtmlTableRow AddHeaderRow(ICollection<object> contents, ICollection<int> cellsSpan = null, int height = -1)
        {
            HtmlTableRow row;
            if (height < 0) height = BaseHeightForRows;
            else if (height < _minHeight) throw new Exception("height for HtmlTable must be bigger than " + _minHeight);

            row = new HtmlTableRow(this, (height < 0) ? BaseHeightForRows : height, contents, cellsSpan, true);

            Rows.Add(row);
            return row;
        }

       

        /// <summary>
        /// 줄을 한 줄 추가한다.
        /// </summary>
        /// <param name="height"></param>
        public HtmlTableRow AddRow(ICollection<int> cellsSpan, int height = -1)
        {
            HtmlTableRow row;
            if (height < 0) height = BaseHeightForRows;
            else if (height < _minHeight) throw new Exception("height for HtmlTable must be bigger than " + _minHeight);

            row = new HtmlTableRow(this, (height < 0) ? BaseHeightForRows : height, null, cellsSpan);
            Rows.Add(row);
            return row;
        }

        public HtmlTableRow AddRow(int height = -1)
        {
            HtmlTableRow row;
            if (height < 0) height = BaseHeightForRows;
            else if (height < _minHeight) throw new Exception("height for HtmlTable must be bigger than " + _minHeight);

            row = new HtmlTableRow(this, (height < 0) ? BaseHeightForRows : height, null, null);
            Rows.Add(row);
            return row;
        }

        /// <summary>
        /// 테이블이 모두 만들어진 이후에 값만 갱신할 때 필요하다.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="visibleRows">보여야 할 Row만 따로 셋팅해야 할 때..</param>
        public void SetDataSource(DataTable data, bool[] visibleRows = null)
        {
            int count = 0;
            int dataRowCount = 0;
            foreach (DataRow row in data.Rows)
            {
                if (visibleRows != null)
                {
                    if (visibleRows[dataRowCount] == false)
                    {
                        dataRowCount++;
                        continue;//다음검색..
                    }
                }
                if (dataRowCount >= Rows.Count) break;//테이블보다 데이터가 크면 끝냄..
                HtmlTableRow tableRow = Rows[count++];
                int dataColSize = data.Columns.Count;

                for (int col = 0; col < dataColSize; col++)
                {
                    bool isHeadCell;
                    try
                    {
                        HtmlTableCell cell = tableRow.Cell(col, out isHeadCell);
                        if (isHeadCell) cell.Content = row[col];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        break;
                    }
                }
                dataRowCount++;
            }
        }

        /// <summary>
        /// 테이블이 모두 만들어진 이후에 값만 갱신할 때 필요하다.
        /// </summary>
        /// <param name="data"></param>
        /// /// <param name="rowIndexFirst">배열의 내용이 [row,col]형식이면 true이다.</param>
        /// <param name="visibleRows">데이터 소스 중에서 보여야 할 row만 보인다.</param>
        public void SetDataSource(string[,] data, bool rowIndexFirst = true, bool[] visibleRows = null)
        {
            int tableRowIndex = 0;
            //int dataRowCount = 0;
            for (int dataRowIndex = 0; dataRowIndex < data.GetLength((rowIndexFirst) ? 0 : 1); dataRowIndex++)
            {
                if (visibleRows != null)
                {
                    if (visibleRows[dataRowIndex] == false)
                    {
                        //dataRowCount++;
                        continue;//다음검색..
                    }
                }
                if (dataRowIndex >= Rows.Count) break;//테이블보다 데이터가 크면 끝냄..
                HtmlTableRow tableRow = Rows[tableRowIndex++];
                int dataColSize = data.GetLength((rowIndexFirst) ? 1 : 0);

                for (int dataColInex = 0; dataColInex < dataColSize; dataColInex++)
                {
                    bool isHeadCell;
                    try
                    {
                        HtmlTableCell cell = tableRow.Cell(dataColInex, out isHeadCell);
                        if (isHeadCell) cell.Content = (rowIndexFirst) ? data[dataRowIndex, dataColInex] : data[dataColInex, dataRowIndex];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        break;
                    }
                }
            }

        }

        int _width = -1;
        public int Width { get { return _width; } set { _width = value; } }
    }

}
