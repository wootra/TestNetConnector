using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DataHandling;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridVariousColumn:DataGridViewColumn, IEasyGridColumn
    {
        //Dictionary<EasyGridRow, EasyGridVariousTypeCellInfo> Info = new Dictionary<EasyGridRow, EasyGridVariousTypeCellInfo>();

        public EasyGridVariousColumn(EasyGridViewParent parent)
            : base()
        {
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
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

        public EasyGridVariousColumn(EasyGridKeyValueCell template, EasyGridViewParent parent)
            : base(template)
        {
            _headerCell = new EasyGridHeaderCell(base.HeaderCell);
            _columnSpan = new ColumnSpan(this);
            _dataGridView = parent;
        }

        
        
        EasyGridHeaderCell _headerCell;
        public new EasyGridHeaderCell HeaderCell
        {
            get { return _headerCell; }
        }



        CustomDictionary<String, object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, object> RelativeObject { get { return _relativeObject; } }

        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            if (_columnTextAlignMode == TextAlignModes.Center){
                this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            }
            /*
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
                
                e.Handled = true;
            } 
             */
        }




        public ItemTypes ItemType
        {
            get { return ItemTypes.Various; }
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
