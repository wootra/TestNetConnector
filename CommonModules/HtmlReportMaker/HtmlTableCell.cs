using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlHandlers;
using System.Drawing;
using System.Data;
using System.Drawing.Printing;

namespace HtmlReportMaker
{
    public class HtmlTableCell : HtmlElement
    {
        HtmlTableRow _ownerRow;
        int _colIndex;

        /// <summary>
        /// headercell인지 나타낸다.
        /// </summary>
        public bool IsHeader {
            get; 
            internal set; 
        }

        internal HtmlTableCell(HtmlTableRow ownerRow, Color backColor, Color foreColor, int index, int span)
        {
            _ownerRow = ownerRow;
            Content = null;
            Span = span;
            BackColor = backColor;
            ForeColor = foreColor;
            CellAlign = CellAligns.Center;
            _colIndex = index;
            IsHeader = false;
        }
        internal HtmlTableCell(HtmlTableRow ownerRow, int index, int span)
        {
            _ownerRow = ownerRow;
            Content = null;
            Span = span;
            BackColor = Color.Empty;
            ForeColor = Color.Empty;
            CellAlign = CellAligns.Center;
            _colIndex = index;
            IsHeader = false;
        }

        public int Index { get { return _colIndex; } }

        object _content;
        public object Content {
            get{ return _content;}
            set
            {
                _content = value;
                /*
                if (value is String)
                {
                    _content = (value as String).Replace("&", "&&").Replace("<", "&lt;").Replace(">", "&gt;");
                }
                else
                {
                    _content = value;
                }
                 */
            }

        }
        public int Span { get; set; }

        public CellAligns CellAlign { get; set; }



        public override string MoreStyle
        {
            get
            {
                string more = "";
                if (CellAlign != CellAligns.Left)
                {
                    //more += "text-align:";
                    //if (CellAlign == CellAligns.Center) more += "center;";
                    //else if (CellAlign == CellAligns.Right) more += "right;";
                    more += "padding:0px;margin:0px;";
                }

                return more;
            }
            
        }

        public string TableCellName
        {
            get
            {
                if (_ownerRow.TableRowName.Length == 0) return "";
                else return _ownerRow.TableRowName + "_" + _ownerRow.Cells.IndexOf(this);
            }
        }

        internal void GetXml(XmlDocument xDoc, XmlNode tr)
        {
            
            XmlElement td = XmlAdder.Element(xDoc, "TD", tr);
            if (_ownerRow.TableRowName.Length > 0)
            {
                XmlAdder.Attribute(xDoc, "id", TableCellName, td);
                XmlAdder.Attribute(xDoc, "onmouseover", "if ( typeof( window[ 'cell_mouse_over' ] ) != 'undefined' ) cell_mouse_over('" + TableCellName + "');", td);
                XmlAdder.Attribute(xDoc, "onmouseout", "if ( typeof( window[ 'cell_mouse_out' ] ) != 'undefined' ) cell_mouse_out('" + TableCellName + "');", td);
            }
            XmlAdder.Attribute(xDoc, "colSpan", Span.ToString(), td);
            if (Style.Length > 0) XmlAdder.Attribute(xDoc, "Style", Style, td);
            base.GetXmlForThis(xDoc, td);

            if (CellAlign == CellAligns.Center) td = XmlAdder.Element(xDoc, "CENTER", td);
            if (IsHeader) td = XmlAdder.Element(xDoc, "B", td);//header면 두꺼운 글자로..
            if (Content == null) td.InnerText = " ";
            else
            {
                if (Content is String)
                {
                    if ((Content as String).Length == 0 ) td.InnerXml = "<P> </P>";
                    else td.InnerText = Content.ToString();
                }
                else if (Content == null)
                {
                    td.InnerXml = "<P> </P>";
                }
            }
        }

        
    }
    public enum CellAligns { Left, Right, Center };
}
