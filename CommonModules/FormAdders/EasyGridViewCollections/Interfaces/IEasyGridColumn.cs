using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Drawing;
using System.Windows.Forms;
using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public interface IEasyGridColumn
    {
        /// <summary>
        /// Cell을 표현할 때 변화가있으면 이 내용을 갱신한다.
        /// </summary>
        /// <param name="e">
        /// CellPainting 이벤트를 구현할 때 나오는 argument.
        /// </param>
        void OnPaint(DataGridViewCellPaintingEventArgs e);

        ItemTypes ItemType { get; }

        int Width { get; set; }

        String Name { get; set; }

        String HeaderText { get; set; }

        CustomDictionary<String,Object> RelativeObject { get; }

        int Index { get; }

        EasyGridHeaderCell HeaderCell { get;}

        ColumnSpan Span { get; }

        EasyGridViewParent DataGridView { get; }
    }

    public class ColumnSpan
    {
        List<ColumnSpanInfo> _spans = new List<ColumnSpanInfo>();
        public List<ColumnSpanInfo> Spans { get { return _spans; } }
        IEasyGridColumn _targetColumn;

        public ColumnSpan(IEasyGridColumn column)
        {
            _targetColumn = column;
        }

        public void RemoveSpans()
        {
            foreach (ColumnSpanInfo info in _spans)
            {
                for (int i = info.SpanStart; i < info.SpanStart + info.SpanSize; i++)
                {
                    IEasyGridCell cell = _targetColumn.DataGridView.Rows[i].EasyCells[_targetColumn.Index];
                    cell.Span.SetSpanBaseInColumn(cell, 1);
                }
            }
        }

        public void AddSpans(IEasyGridCell baseCell, int size)
        {
            Spans.Add(new ColumnSpanInfo(_targetColumn, baseCell, size));
            //IEasyGridCell baseCell = baseCell;// _targetColumn.DataGridView.Rows[startRow].EasyCells[_targetColumn.Index];
            
            for (int i = baseCell.RowIndex; i < baseCell.RowIndex+size; i++)
            {
                _targetColumn.DataGridView.Rows[i].EasyCells[_targetColumn.Index].Span.SetSpanBaseInColumn(baseCell, size);
            }
        }
    }

    public class ColumnSpanInfo
    {
        public int SpanStart;
        public int SpanSize;
        public IEasyGridCell BaseCell;

        IEasyGridColumn _targetColumn;
        public ColumnSpanInfo(IEasyGridColumn targetColumn, IEasyGridCell baseCell,int spanSize)
        {
            _targetColumn = targetColumn;
            BaseCell = baseCell;
            SpanStart = baseCell.RowIndex;
            SpanSize = spanSize;
        }
        public IEasyGridColumn TargetColumn { get { return _targetColumn; } }

    }
}
