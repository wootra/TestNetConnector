using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlHandlers;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Collections;
using System.Drawing.Printing;

namespace HtmlReportMaker
{
    public class HtmlTableRow:HtmlElement
    {
        HtmlTable _ownerTable;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTable"></param>
        /// <param name="height"></param>
        /// <param name="contents"></param>
        /// <param name="cellSpans"></param>
        /// <param name="isHeader"></param>
        internal HtmlTableRow(HtmlTable ownerTable, int height, ICollection<object> contents, ICollection<int> cellSpans, bool isHeader = false)
        {
            _ownerTable = ownerTable;
            Height = height;
            _isHeader = isHeader;
            BackColor = Color.Empty;
            for (int i = 0; i < ownerTable.Widths.Count; i++)
            {
                HtmlTableCell cell = new HtmlTableCell(this, i, 1) { IsHeader = isHeader };
                
                if (contents != null && contents.Count > i)
                {
                    if (contents.ElementAt(i) is String)
                    {
                        string content = (contents.ElementAt(i) != null) ? contents.ElementAt(i).ToString() : "";
                        cell.Content = content;// content.Replace("&", "&&").Replace("<", "&lt;").Replace(">", "&gt;");
                    }
                    else
                    {
                        cell.Content = " ";
                    }
                }
                _cells.Add(cell);
            }

            if (cellSpans != null)
            {
                int colIndex = 0;
                for (int i = 0; i < cellSpans.Count; i++)
                {
                    int cellSpan = cellSpans.ElementAt(i);
                    if (cellSpan > 1)
                    {
                        _cells[colIndex].Span = cellSpan;
                        colIndex += cellSpan;
                    }else colIndex++;
                }
                
            }
        }

       

       
        bool _isHeader = false;
        public bool IsHeader { get { return _isHeader; } }

        Margins _borderWidths = null;
        public override Margins BorderWidths
        {
            get
            {
                return _borderWidths;
            }
            set
            {
                _borderWidths = value;
            }
        }

        List<HtmlTableCell> _cells = new List<HtmlTableCell>();

        /// <summary>
        /// Cell을 가져온다. isHeadCell은 해당 cell이 span header에 해당하면 true, 아니면 false이다.
        /// 만일 index가 범위를 넘으면 IndexOutOfRangeException을 발생시킨다.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="isHeadCell">spanHeader</param>
        /// <returns></returns>
        public HtmlTableCell Cell(int cellIndex, out bool isHeadCell)
        {
            int tableColIndex = 0;
            foreach (HtmlTableCell cell in _cells)
            {
                if (cellIndex <= tableColIndex)
                {
                    if (cellIndex == tableColIndex) isHeadCell = true;
                    else isHeadCell = false;
                    return cell;
                }
                tableColIndex += cell.Span;
            }
            throw new IndexOutOfRangeException("Cell Index is out of range in HtmlTableRow");
        }


        /// <summary>
        /// cell을 가져온다.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public HtmlTableCell Cell(int cellIndex)
        {
            int tableColIndex = 0;
            foreach (HtmlTableCell cell in _cells)
            {
                if (cellIndex == tableColIndex)
                {
                    return cell;
                }
                tableColIndex += cell.Span;
            }
            throw new IndexOutOfRangeException("Cell Index is out of range in HtmlTableRow");
        }

        public List<HtmlTableCell> Cells
        {
            get { return _cells; }
        }

        public string TableRowName
        {
            get
            {
                if (_ownerTable.TableName.Length == 0) return "";
                else return _ownerTable.TableName + "_"+_ownerTable.Rows.IndexOf(this);
            }
        }
       
        internal XmlElement GetXml(XmlDocument xDoc, XmlNode table)
        {
            XmlElement tr;
            
            tr = XmlAdder.Element(xDoc, "TR", table);
            if (_ownerTable.TableName.Length > 0)
            {
                XmlAdder.Attribute(xDoc, "id", TableRowName, tr);
                XmlAdder.Attribute(xDoc, "onmouseover", "if ( typeof( window[ 'row_mouse_over' ] ) != 'undefined' ) row_mouse_over('" + TableRowName + "');", tr);
                XmlAdder.Attribute(xDoc, "onmouseout", "if ( typeof( window[ 'row_mouse_out' ] ) != 'undefined' ) row_mouse_out('" + TableRowName + "');", tr);
            }
            base.GetXmlForThis(xDoc, tr);

            if (Style != null && Style.Length > 0) XmlAdder.Attribute(xDoc, "Style", Style, tr);

            
            int col = 0;
            foreach (HtmlTableCell cell in _cells)
            {
                if (BackColor != Color.Empty)
                {
                    if (cell.BackColor == Color.Empty)
                    {
                        cell.BackColor = BackColor;
                    }
                }
                if(BorderWidths!=null){
                    if (col == 0)//first col
                    {
                        cell.BorderWidths = new Margins(BorderWidths.Left, 0, BorderWidths.Top, BorderWidths.Bottom);
                    }
                    else if (col == _cells.Count - 1)//last col
                    {
                        cell.BorderWidths = new Margins(0, BorderWidths.Right, BorderWidths.Top, BorderWidths.Bottom);
                    }
                    else
                    {
                        cell.BorderWidths = new Margins(0, 0, BorderWidths.Top, BorderWidths.Bottom);
                    }
                }
                cell.GetXml(xDoc, tr);
                col++;
            }
            return tr;
        }
    }
}
