using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders; using DataHandling;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridFileOpenBoxColumn:DataGridViewTextBoxColumn, IEasyGridColumn
    {
        public EasyGridFileOpenBoxColumn(EasyGridViewParent parent)
            : base()
        {
            this.CellTextAlignMode = _cellTextAlignMode;
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            this.CellTemplate = new EasyGridFileOpenBoxCell(parent);
            _columnSpan = new ColumnSpan(this);
            _dataGridView = parent;
        }

        EasyGridViewParent _dataGridView;
        public EasyGridViewParent DataGridView
        {
            get { return _dataGridView; }
        }
        ColumnSpan _columnSpan;
        public ColumnSpan Span { get { return _columnSpan; } }

        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }


        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }

        bool _isEditable = true;
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
            }
        }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            if (_columnTextAlignMode == TextAlignModes.Center)
            {
                Graphics g = e.Graphics;

                Point pt = Control.MousePosition;
                DataGridView _parent = base.DataGridView;
                if (_parent == null) return;

                pt = _parent.PointToClient(pt);
                Brush textColor;

                if (e.CellBounds.Contains(pt)) textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, _parent.GridColor, true);
                else textColor = CellFunctions.DrawHeaderBack(e.CellBounds, g, Color.WhiteSmoke, this.Selected);

                //Brush textColor = CellFunctions.DrawHeaderBack(e.CellBounds,g, _parent.GridColor, this.Selected);
                pt = CellFunctions.TextCenterInRact(e.CellBounds, g, _parent.Font, base.HeaderText);
                g.DrawString(base.HeaderText as String, _parent.Font, textColor, pt.X, pt.Y + 2);
                /*
                g.FillPath(Brushes.Gainsboro, new GraphicsPath(
                    new PointF[]{
                        new PointF(e.CellBounds.Right-10, e.CellBounds.Top+4),
                        new PointF(e.CellBounds.Right-10, e.CellBounds.Bottom -4),
                        new PointF(e.CellBounds.Right-3, (e.CellBounds.Top + e.CellBounds.Bottom)/2)},
                    new byte[]{
                        (byte)PathPointType.Start,(byte)PathPointType.Line,(byte)PathPointType.Line}, FillMode.Winding));
                 */
                e.Handled = true;
            }
        }

        public int Width
        {
            get { return base.Width; }
            set {
                base.Width = value;
                if (value == 0) this.Visible = false;
                else this.Visible = true;
            }
        }
        
        TextViewModes _textViewMode = TextViewModes.Default;
        public TextViewModes TextViewMode
        {
            get
            {
                if (_textViewMode == TextViewModes.Default)
                {
                    return (DataGridView as EasyGridViewParent).TextViewMode;
                }
                else return _textViewMode;
            }
            set
            {
                _textViewMode = value;
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.FileOpenBox; }
        }

        TextAlignModes _cellTextAlignMode = TextAlignModes.None;
        public TextAlignModes CellTextAlignMode
        {
            get { return _cellTextAlignMode; }
            set {
                _cellTextAlignMode = value;
                
            }
        }

        TextAlignModes _columnTextAlignMode = TextAlignModes.Center;
        public TextAlignModes ColumnTextAlignMode
        {
            get { return _columnTextAlignMode; }
            set
            {
                _columnTextAlignMode = value;

            }
        }

    }
}
