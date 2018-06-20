using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using DataHandling;
using System.Windows.Forms;
using DataHandling;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FormAdders.EasyGridViewCollections
{
    public interface IEasyGridCell
    {
        bool Enabled { get; set; }
        ItemTypes ItemType { get;}
        void SetValue(EasyGridCellInfo info);
        object Value { get; set; }
        String ToolTipText { get; set; }
        void OnPaint(DataGridViewCellPaintingEventArgs e);
        CellSpanInfo Span { get; }
        int RowIndex { get; }
        int ColumnIndex { get; }
        void RePaint();
        CustomDictionary<String, Object> RelativeObject { get; }
    }

    public class CellSpanInfo
    {
        public static bool RedrawSpanCell = false;
        public SpanPosition SpanPos = SpanPosition.NoSpan;
        public IEasyGridCell SpanBaseCell;
        IEasyGridCell _targetCell;
        DataGridView _targetView{
            get { return (_targetCell as DataGridViewCell).DataGridView; }
        }
        DataGridViewElementStates _cellOriginalState = DataGridViewElementStates.Selected;
        internal CellSpanInfo(IEasyGridCell targetCell)
        {
            _targetCell = targetCell;
            SpanBaseCell = targetCell;//default
            
        }
        public delegate void PaintFunc(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts);

        public void Paint(PaintFunc paintFunc,
            System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds,
            System.Drawing.Rectangle cellBounds, int rowIndex, 
            DataGridViewElementStates cellState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            //Rectangle spanBaseRect = cellBounds;
            /*
            bool drawContentInSpanedCell = false;
            int spanBaseXOffset = 0;
            int spanBaseYOffset = 0;
            if (SpanPos != SpanPosition.NoSpan)
            {
                int horScrollOffset=_targetView.HorizontalScrollingOffset;
                int x = 0;
                for(int i=0; i<SpanBaseCell.ColumnIndex; i++){
                    x+= _targetView.Columns[i].Width;
                }
                spanBaseXOffset = x - horScrollOffset;

                int vertScrollOffset = _targetView.VerticalScrollingOffset;
                int y = 0;
                for (int i = 0; i < SpanBaseCell.RowIndex; i++)
                {
                    y += _targetView.Rows[i].Height;
                }
                spanBaseYOffset = y - vertScrollOffset;

                
                spanBaseRect = GetSpanBaseRect();
                if (clipBounds.Width <= (spanBaseRect.Width+1))
                {
                    clipBounds.Width = spanBaseRect.Width;// _targetView.Width - _targetView.RowHeadersWidth;//spanBaseRect.Width;// 
                    clipBounds.X = spanBaseRect.X;// _targetView.RowHeadersWidth;//spanBaseRect.X;// 
                    //clipBounds.Height = spanBaseRect.Height;
                    //clipBounds.Y = spanBaseRect.Y;
                    
                }

                if (clipBounds.Height <= (spanBaseRect.Height+1))
                {
                    clipBounds.Height = spanBaseRect.Height-1;// _targetView.Height - _targetView.ColumnHeadersHeight;//spanBaseRect.Height;//
                    clipBounds.Y = spanBaseRect.Y;// _targetView.ColumnHeadersHeight;//spanBaseRect.Y;// 
                    drawContentInSpanedCell = true;
                    //clipBounds.X = spanBaseRect.X;
                    //clipBounds.Width = spanBaseRect.Width;
                }
                
            }
            */
            //PaintSpanedCell(g);
            if (SpanPos == SpanPosition.NoSpan)
            {
                paintParts = DataGridViewPaintParts.All;
                paintFunc
                    (
                    g,
                clipBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                formattedValue,
                _targetCell.ToolTipText,
                cellStyle,
                advancedBorderStyle,
                paintParts);
            }
            else if (SpanPos == SpanPosition.Spanned)
            {
                try
                {
                    paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground;// DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.ContentBackground;
                    DataGridViewCellStyle cStyle = cellStyle;
                    
                    
                    paintFunc
                       (
                       g,
                   clipBounds,
                   cellBounds,
                   rowIndex,
                   cellState,//(SpanBaseCell as DataGridViewCell).State,
                   (SpanBaseCell as DataGridViewCell).Value,
                   (SpanBaseCell as DataGridViewCell).FormattedValue,
                   SpanBaseCell.ToolTipText,
                   cellStyle,
                   advancedBorderStyle,
                   paintParts);

                    DataGridViewCell cell = _targetCell as DataGridViewCell;
                    int m_nLeftColumn = SpanBaseCell.ColumnIndex;
                    int m_nRightColumn = SpanBaseCell.ColumnIndex + SpanBaseCell.Span.ColSpanSize - 1;
                    int m_nTopRow = SpanBaseCell.RowIndex;
                    int m_nBottomRow = SpanBaseCell.RowIndex + SpanBaseCell.Span.RowSpanSize - 1;

                    int widMergeIndex = cell.ColumnIndex - m_nLeftColumn;
                    int higMergeIndex = cell.RowIndex - m_nTopRow;
                    int i = 0;
                    int nWidth = 0;
                    int nWidthLeft = 0;
                    int nHeight = 0;
                    int nHeightTop = 0;
                    string strText = null;

                    Pen pen = new Pen(Brushes.Black);

                    // Draw the background
                    //g.FillRectangle(new SolidBrush(SystemColors.Control), cellBounds);

                    // Draw the separator for rows
                    Color backColor = (cell.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor;
                    if (cell.RowIndex == m_nBottomRow)
                    {
                        g.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDark)), cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right, cellBounds.Bottom - 1);
                    }
                    else
                    {
                        g.DrawLine(new Pen(new SolidBrush(backColor)), cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right, cellBounds.Bottom - 1);
                    }

                    // Draw the right vertical line for the cell
                    if (cell.ColumnIndex == m_nRightColumn)
                    {
                        g.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDark)), cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom);
                    }
                    else
                    {
                        g.DrawLine(new Pen(new SolidBrush(backColor)), cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom);
                    }

                    // Draw the text
                   
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    // Determine the total width of the merged cell
                    nWidth = 0;
                    for (i = m_nLeftColumn; i <= m_nRightColumn; i++)
                    {
                        nWidth += cell.OwningRow.Cells[i].Size.Width;
                    }

                    // Determine the width before the current cell.
                    nWidthLeft = 0;
                    for (i = m_nLeftColumn; i <= cell.ColumnIndex - 1; i++)
                    {
                        nWidthLeft += cell.OwningRow.Cells[i].Size.Width;
                    }

                    nHeight = 0;
                    for (i = m_nTopRow; i <= m_nBottomRow; i++)
                    {
                        nHeight += cell.DataGridView.Rows[i].Cells[_targetCell.ColumnIndex].Size.Height;
                    }

                    // Determine the width before the current cell.
                    nHeightTop = 0;
                    for (i = m_nTopRow; i <= cell.RowIndex - 1; i++)
                    {
                        nHeightTop += cell.DataGridView.Rows[i].Cells[_targetCell.ColumnIndex].Size.Height;
                    }
                    
                    if (cell is EasyGridTextBoxCell)
                    {
                        RectangleF rectDest = RectangleF.Empty;
                        // Retrieve the text to be displayed
                        strText = SpanBaseCell.Value.ToString();

                        rectDest = new RectangleF(cellBounds.Left - nWidthLeft, cellBounds.Top - nHeightTop, nWidth, nHeight);
                        g.DrawString(strText, cell.DataGridView.DefaultCellStyle.Font,
                            new SolidBrush(cell.DataGridView.DefaultCellStyle.ForeColor), rectDest, sf);
                    }
                    else
                    
                    {
                     
                        paintParts = DataGridViewPaintParts.ContentForeground;
                        Rectangle rectDest = new Rectangle(cellBounds.Left - nWidthLeft, cellBounds.Top - nHeightTop, nWidth, nHeight);
                        
                        paintFunc
                       (
                       g,
                   clipBounds,
                   rectDest,
                   rowIndex,
                   cellState,// GetSpanedCellState(),
                   (SpanBaseCell as DataGridViewCell).Value,
                   (SpanBaseCell as DataGridViewCell).FormattedValue,
                   SpanBaseCell.ToolTipText,
                   cellStyle,
                   advancedBorderStyle,
                   paintParts);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
                /*
                //if (drawContentInSpanedCell)
                {
                    (_targetCell as DataGridViewCell).Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    paintParts = DataGridViewPaintParts.Background| DataGridViewPaintParts.SelectionBackground;// DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.ContentBackground;
                    
                    paintFunc
                       (
                       g,
                   clipBounds,
                   spanBaseRect,
                   rowIndex,
                   cellState,// GetSpanedCellState(),
                   (SpanBaseCell as DataGridViewCell).Value,
                   (SpanBaseCell as DataGridViewCell).Value,
                   SpanBaseCell.ToolTipText,
                   cellStyle,
                   advancedBorderStyle,
                   paintParts);
                    
                    //if (drawContentInSpanedCell) paintParts = paintParts | DataGridViewPaintParts.ContentForeground;
                    //(SpanBaseCell as DataGridViewCell).Style.State |= cellState;
                    EasyGridTextBoxCell cell = _targetCell as EasyGridTextBoxCell;
                    
                    int stringWid = 0;
                    EasyGridViewParent view = (_targetView as EasyGridViewParent);
                    for (int i = SpanBaseCell.ColumnIndex; i < _targetView.Columns.Count; i++)
                    {
                        if (view.Rows[SpanBaseCell.RowIndex].EasyCells[i].Span.SpanBaseCell.Equals(SpanBaseCell))
                        {
                            stringWid += _targetView.Columns[i].Width;
                        }
                        else
                        {
                            break;
                        }
                    }
                    int stringHeight = 0;
                    for (int i = SpanBaseCell.RowIndex; i < _targetView.Rows.Count; i++)
                    {
                        if (view.Rows[i].EasyCells[SpanBaseCell.ColumnIndex].Span.SpanBaseCell.Equals(SpanBaseCell))
                        {
                            stringHeight += _targetView.Rows[i].Height;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    g.DrawString(SpanBaseCell.Value.ToString(),
                        cell.Font,
                        new SolidBrush(cell.DataGridView.DefaultCellStyle.ForeColor),
                        new PointF(spanBaseXOffset+cell.Style.Padding.Left,
                            (float)CellFunctions.CenterTextY(
                                cell.Size.Height,
                                spanBaseRect.Y, g, cell.Font)
                        )
                    );
                    //if (clipBounds.Width > 0) RedrawSpanCell = false;//한번 그리고 나면 다시 초기화..
                  }
                    */



            }
            else if (SpanPos == SpanPosition.SpanBase)
            {
                paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground;// DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.ContentBackground;

                paintFunc
                   (
                   g,
               clipBounds,
               cellBounds,
               rowIndex,
               cellState,// GetSpanedCellState(),
               (SpanBaseCell as DataGridViewCell).Value,
               (SpanBaseCell as DataGridViewCell).Value,
               SpanBaseCell.ToolTipText,
               cellStyle,
               advancedBorderStyle,
               paintParts);

                DataGridViewCell cell = _targetCell as DataGridViewCell;
                int m_nLeftColumn = SpanBaseCell.ColumnIndex;
                int m_nRightColumn = SpanBaseCell.ColumnIndex + SpanBaseCell.Span.ColSpanSize - 1;
                int m_nTopRow = SpanBaseCell.RowIndex;
                int m_nBottomRow = SpanBaseCell.RowIndex + SpanBaseCell.Span.RowSpanSize - 1;

                int widMergeIndex = cell.ColumnIndex - m_nLeftColumn;
                int higMergeIndex = cell.RowIndex - m_nTopRow;
                int i = 0;
                int nWidth = 0;
                int nWidthLeft = 0;
                int nHeight = 0;
                int nHeightTop = 0;
                string strText = null;

                Pen pen = new Pen(Brushes.Black);

                // Draw the background
                //g.FillRectangle(new SolidBrush(SystemColors.Control), cellBounds);

                // Draw the separator for rows
                Color backColor = (cell.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor;
                    
                if (cell.RowIndex == m_nBottomRow)
                {
                    g.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDark)), cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right, cellBounds.Bottom - 1);
                }
                else
                {
                    g.DrawLine(new Pen(new SolidBrush(backColor)), cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right, cellBounds.Bottom - 1);
                }

                // Draw the right vertical line for the cell
                if (cell.ColumnIndex == m_nRightColumn)
                {
                    g.DrawLine(new Pen(new SolidBrush(SystemColors.ControlDark)), cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom);
                }
                else
                {
                    g.DrawLine(new Pen(new SolidBrush(backColor)), cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom);
                }

                // Draw the text
                RectangleF rectDest = RectangleF.Empty;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                // Determine the total width of the merged cell
                nWidth = 0;
                for (i = m_nLeftColumn; i <= m_nRightColumn; i++)
                {
                    nWidth += cell.OwningRow.Cells[i].Size.Width;
                }

                // Determine the width before the current cell.
                nWidthLeft = 0;
                for (i = m_nLeftColumn; i <= cell.ColumnIndex - 1; i++)
                {
                    nWidthLeft += cell.OwningRow.Cells[i].Size.Width;
                }

                nHeight = 0;
                for (i = m_nTopRow; i <= m_nBottomRow; i++)
                {
                    nHeight += cell.DataGridView.Rows[i].Cells[_targetCell.ColumnIndex].Size.Height;
                }

                // Determine the width before the current cell.
                nHeightTop = 0;
                for (i = m_nTopRow; i <= cell.RowIndex - 1; i++)
                {
                    nHeightTop += cell.DataGridView.Rows[i].Cells[_targetCell.ColumnIndex].Size.Height;
                }
                
                if (cell is EasyGridTextBoxCell)
                {
                    // Retrieve the text to be displayed
                    strText = SpanBaseCell.Value.ToString();

                    rectDest = new RectangleF(cellBounds.Left - nWidthLeft, cellBounds.Top - nHeightTop, nWidth, nHeight);
                    g.DrawString(strText, cellStyle.Font, new SolidBrush(cellStyle.ForeColor), rectDest, sf);
                }
                else
                
                {
                    Rectangle rect = new Rectangle(cellBounds.Left - nWidthLeft, cellBounds.Top - nHeightTop, nWidth, nHeight);
                   
                    paintParts = DataGridViewPaintParts.ContentForeground;
                    paintFunc
                   (
                   g,
               clipBounds,
               rect,
               rowIndex,
               cellState,// GetSpanedCellState(),
               (SpanBaseCell as DataGridViewCell).Value,
               (SpanBaseCell as DataGridViewCell).Value,
               SpanBaseCell.ToolTipText,
               cellStyle,
               advancedBorderStyle,
               paintParts);
                }
            }
        }

        public DataGridViewElementStates GetSpanedCellState()
        {
            if (SpanPos == SpanPosition.NoSpan)
            {
                return (_targetCell as DataGridViewCell).State;
            }
            else if (SpanPos == SpanPosition.SpanBase || SpanPos == SpanPosition.Spanned)
            {
                return (SpanBaseCell as DataGridViewCell).State;//.Span._cellOriginalState;
                /*
                DataGridViewElementStates state = DataGridViewElementStates.None;
                for (int c = SpanBaseCell.ColumnIndex; c < SpanBaseCell.ColumnIndex + SpanBaseCell.Span.ColSpanSize; c++)
                {
                    for (int r = SpanBaseCell.RowIndex; r < SpanBaseCell.RowIndex + SpanBaseCell.Span.RowSpanSize; r++)
                    {
                        state = state | ((IEasyGridCell)( _targetView.Rows[r].Cells[c])).Span._cellOriginalState;
                    }
                }
                 */
                //return state;
            }
            else
            {
                return DataGridViewElementStates.None;
            }
        }

        public Rectangle GetSpanBaseRect()
        {
            int startRowIndex = SpanBaseCell.RowIndex;
            int startColIndex = SpanBaseCell.ColumnIndex;
            if (startRowIndex < (_targetView.FirstDisplayedScrollingRowIndex))
            {
                startRowIndex = _targetView.FirstDisplayedScrollingRowIndex;
            }
            if (startColIndex < _targetView.FirstDisplayedScrollingColumnIndex)
            {
                startColIndex = _targetView.FirstDisplayedScrollingColumnIndex;
            }

            Rectangle startRect = _targetView.GetCellDisplayRectangle(startColIndex, startRowIndex, false);
            int endColIndex = SpanBaseCell.ColumnIndex + SpanBaseCell.Span.ColSpanSize - 1;
            if (endColIndex >= (_targetView.FirstDisplayedScrollingColumnIndex + _targetView.DisplayedColumnCount(true)))
            {
                endColIndex = (_targetView.FirstDisplayedScrollingColumnIndex + _targetView.DisplayedColumnCount(true)) - 1;
            }
            int endRowIndex = SpanBaseCell.RowIndex + SpanBaseCell.Span.RowSpanSize - 1;
            if (endRowIndex >= (_targetView.FirstDisplayedScrollingRowIndex + _targetView.DisplayedRowCount(true)))
            {
                endRowIndex = (_targetView.FirstDisplayedScrollingRowIndex + _targetView.DisplayedRowCount(true)) - 1;
            }
            Rectangle endRect = _targetView.GetCellDisplayRectangle(endColIndex, endRowIndex, false);

            Rectangle cellRect = new Rectangle(startRect.Left, startRect.Top, endRect.Right - startRect.Left, endRect.Bottom - startRect.Top);
            return cellRect;
        }

        
        /// <summary>
        /// cell에서 부터 시작해서 Row 안에서 몇 개의 column을 span할 것인지 지정한다.
        /// 대상 cell이 이 셋팅으로 인하여 어느 위치에 어떻게 들어가는지를 나타낸다.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="colSpan">1이면 span하지 않는다는 뜻이다.</param>
        internal void SetSpanBaseInRow(IEasyGridCell cell, int colSpan=1)
        {
            if(colSpan<=1){
                if (cell.Equals(_targetCell))
                {
                    SpanPos = SpanPosition.NoSpan;
                    _colSpanSize = 1;
                }
                return;
            }
            
            if (cell.Equals(_targetCell))
            {
                SpanBaseCell = cell;
                SpanPos = SpanPosition.SpanBase;
                _colSpanSize = colSpan;
                _rowSpanSize = 1;
            }
            else if (_targetCell.ColumnIndex > cell.ColumnIndex &&
               _targetCell.ColumnIndex < cell.ColumnIndex + colSpan)
            {
                SpanBaseCell = cell;
                SpanPos = SpanPosition.Spanned;
                _colSpanSize = 0;
            }
            else
            {
                //이 영역에 들어가지 않으므로 상태를 유지한다.
            }

        }

        internal void SetSpanBaseInColumn(IEasyGridCell baseCell, int rowSpan = 1)
        {
            if (rowSpan <= 1)
            {
                if (baseCell.Equals(_targetCell))
                {
                    SpanPos = SpanPosition.NoSpan;
                    _rowSpanSize = 1;
                    _colSpanSize = 1;
                }
                return;
                
            }

            if (baseCell.Equals(_targetCell))
            {
                SpanBaseCell = baseCell;
                SpanPos = SpanPosition.SpanBase;
                _rowSpanSize = rowSpan;
                _colSpanSize = 1;
            }
            else if (_targetCell.RowIndex > baseCell.RowIndex &&
               _targetCell.RowIndex <= baseCell.RowIndex + rowSpan)
            {
                SpanBaseCell = baseCell;
                SpanPos = SpanPosition.Spanned;
                _rowSpanSize = 0;
            }
            else
            {
                //이 영역에 들어가지 않으므로 상태를 유지한다.
            }

        }

        int _rowSpanSize = 1;
        int _colSpanSize = 1;
        public int RowSpanSize { get { return _rowSpanSize; } }
        public int ColSpanSize { get { return _colSpanSize; } }
        
        public IEasyGridCell TargetCell { get { return _targetCell; } }
        public DataGridView TargetView { 
            get {
                if (_targetView != null) return _targetView;
                else
                {
                    return (_targetCell as DataGridViewCell).DataGridView;
                }
            } 
        }

    }
}
