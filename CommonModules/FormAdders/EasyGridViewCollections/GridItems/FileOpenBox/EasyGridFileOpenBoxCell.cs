using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridFileOpenBoxCell:DataGridViewTextBoxCell,IEasyGridCell
    {
        EasyGridViewParent _parent;
        int _buttonWid = 40;
        public EasyGridFileOpenBoxCell(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {

            IEasyGridCell baseCell = this.Span.SpanBaseCell;// GetSpanBaseCell();
            
            if (_isEditable)
            {
                _parent.CurrentCell = baseCell as DataGridViewCell;
                _parent.BeginEdit(true);
            }
            
            if (baseCell.Equals(this) == false)
            {
                _parent.InvalidateCell(baseCell as DataGridViewCell);
                base.OnDoubleClick(e);
                return;
            }
            
            base.OnDoubleClick(e);

        }
        
        
        
        bool _enabled = true;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RePaint();
                }
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.FileOpenBox; }
        }
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            /*
            IEasyGridCell baseCell = GetSpanBaseCell();
            DataGridViewCell cell = baseCell as DataGridViewCell;
            
            String text = this.Text;
            if (baseCell.Equals(this) == false)
            {

            }
            else
            {
                PaintCell(e.CellBounds, e.Graphics);
            }
            */
            Rectangle textArea = getTextArea(e.CellBounds);
            PaintCell(textArea, e.Graphics);
            Rectangle buttonArea = getButtonArea(e.CellBounds);
            e.Graphics.FillRectangle(Brushes.Gray, getButtonArea(e.CellBounds));
            //CellFunctions.DrawLensBack(this, buttonArea, e.Graphics, DataGridView.GridColor, this.Selected, this.Enabled);
            e.Graphics.DrawString("...", _font, Brushes.Black, new Point(buttonArea.X + 5, buttonArea.Y + buttonArea.Height / 2 - 3));
            e.Handled = true;
        }

        public Rectangle TextArea
        {
            get
            {
                Rectangle dRect = DataGridView.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, true);
                Rectangle rect = new Rectangle(new Point(0, 0), dRect.Size);
                return getTextArea(rect);
            }
        }

        public Rectangle ButtonArea
        {
            get
            {
                Rectangle dRect = DataGridView.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, true);
                Rectangle rect = new Rectangle(new Point(0, 0), dRect.Size);
                return getButtonArea(rect);
            }
        }

        /// <summary>
        /// 마우스 클릭하여 값이 변했는지를 알려준다.
        /// </summary>
        /// <param name="mousePositionInCell"></param>
        /// <returns>isChanged..</returns>
        public bool MouseClick(Point mousePositionInCell)
        {

            if (IsInButtonArea(mousePositionInCell))
            {
                OpenFileDialog dlg = new OpenFileDialog();

                String path = Text.Replace("/", "\\");
                if (path.Contains("\\"))
                {
                    string dir = (path.LastIndexOf("\\") > 0) ? path.Substring(0, path.LastIndexOf("\\") - 1) : "\\";

                    dlg.InitialDirectory = dir;
                }
                DialogResult result = dlg.ShowDialog();
                if (result != DialogResult.Abort && result != DialogResult.Cancel)
                {
                    Text = dlg.FileName;
                    return true;
                }
            }
            return false;
        }

        public bool IsInTextArea(Point mousePositionInDataView)
        {
            
            return TextArea.Contains(mousePositionInDataView);
        }

        public bool IsInButtonArea(Point mousePositionInDataView)
        {
            return ButtonArea.Contains(mousePositionInDataView);
        }

        Rectangle getTextArea(Rectangle cellBounds)
        {
            return new Rectangle(cellBounds.Location, new Size(cellBounds.Width - _buttonWid, cellBounds.Height));
        }

        Rectangle getButtonArea(Rectangle cellBounds)
        {
            return new Rectangle(new Point(cellBounds.X + cellBounds.Width - _buttonWid, cellBounds.Y), new Size(_buttonWid, cellBounds.Height));
        }

        TextAlignModes _textAlignMode = TextAlignModes.None;
        public TextAlignModes TextAlignMode
        {
            get {
                if (_textAlignMode == TextAlignModes.None)
                {
                    TextAlignModes alignMode;
                    if ((OwningColumn is EasyGridFileOpenBoxColumn) && (OwningColumn as EasyGridFileOpenBoxColumn).CellTextAlignMode != TextAlignModes.None)
                        alignMode = (OwningColumn as EasyGridFileOpenBoxColumn).CellTextAlignMode;
                    else alignMode = _parent.TextAlignMode;
                    return alignMode;
                }
                else return _textAlignMode; 
            }
            set { _textAlignMode = value; }
        }

        bool _isEditable = true;
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(String);
            }
        }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                /*
                IEasyGridCell baseCell = GetSpanBaseCell();
                if (baseCell.Equals(this) == false)
                {
                    DataGridViewCell cell = baseCell as DataGridViewCell;
                    EasyGridFileOpenBoxCell txtCell = cell as EasyGridFileOpenBoxCell;

                    if (value == true)
                    {
                        
                        for (int i = txtCell.RowIndex; i < txtCell.RowIndex + txtCell.RowSpan; i++)
                        {
                            EasyGridFileOpenBoxCell relCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridFileOpenBoxCell);
                            if (txtCell != null) relCell.SetSelection(true);
                        }

                    }
                    else
                    {
                        Point pt = _parent.PointToClient(Control.MousePosition);
                        bool isInArea = false;
                        
                        for (int i = txtCell.RowIndex; i < txtCell.RowIndex + txtCell.RowSpan; i++)
                        {
                            Rectangle cellRect = _parent.GetCellDisplayRectangle(this.ColumnIndex, i, true);
                            if (_parent.SelectionMode == DataGridViewSelectionMode.CellSelect)
                            {
                                if (cellRect.Contains(pt))
                                {
                                    isInArea = true;
                                    break;
                                }
                            }
                            else if (_parent.SelectionMode == DataGridViewSelectionMode.FullRowSelect)
                            {
                                if (pt.Y >= cellRect.Top && pt.Y <= cellRect.Bottom && pt.X>=_parent.ClientRectangle.Left && pt.X<= _parent.ClientRectangle.Right)
                                {
                                    isInArea = true;
                                    break;
                                }
                            }
                        }

                        if (isInArea == false)//영역안에 마우스가 없으면 false..
                        {
                            for (int i = txtCell.RowIndex; i < txtCell.RowIndex + txtCell.RowSpan; i++)
                            {
                                EasyGridFileOpenBoxCell relCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridFileOpenBoxCell);

                                if (txtCell != null) relCell.SetSelection(false);
                            }
                        }
                    }
                    
//                    base.Selected = false;
                    
                    //_parent.InvalidateColumn(this.ColumnIndex);
                    //_parent.Update();
                }
                else
                {
                    if (this.RowSpan > 1)
                    {
                        for (int i = RowIndex; i < RowIndex + RowSpan; i++)
                        {
                            EasyGridFileOpenBoxCell txtCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridFileOpenBoxCell);
                            if (txtCell != null) txtCell.SetSelection(value);
                        }
                        _parent.InvalidateColumn(this.ColumnIndex);
                        _parent.Update();
                    }
                    base.Selected = value;
                }
                 */
            }
        }

        public void SetSelection(bool Selected)
        {
            base.Selected = Selected;
        }

        Font _font = new Font(SystemFonts.DefaultFont, FontStyle.Regular);
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                RePaint();
            }
        }

        Brush _fontColor = Brushes.Black;
        public Brush FontColor
        {
            get { return _fontColor; }
            set
            {
                if (_fontColor != value)
                {
                    _fontColor = value;
                    RePaint();
                }
            }
        }

        public String Text
        {
            get
            {
                
                if (base.Value == null) return "";
                else return base.Value as String;
            }
            set
            {
                if (base.Value==null || base.Value.Equals(value) == false)
                {
                    base.Value = value;
                }
            }
        }

        public new object Value
        {
            get
            {
                if (base.Value == null) return "";
                else return base.Value as String;
            }
            set
            {
                if (value is string)
                {
                    SetValue(value as String);
                    //base.Value = value;
                }
                else if (value is EasyGridCellInfo)
                {
                    SetValue(value as EasyGridCellInfo);
                }
                else
                {
                    if (value == null) SetValue("");
                    else SetValue(value.ToString());
                    /*
                    if (base.Value.Equals(value.ToString()) == false)
                    {
                        
                        //base.Value = value.ToString();
                        //if (OwningRow != null) (OwningRow as EasyGridRow).IsTextChanged = true;
                    }
                     */
                    /*
                    throw new InvalidTypeException(value, new Type[]{
                        typeof(EasyGridCellInfo), typeof(string)});
                     */
                }
                
            }
        }

        
        public void SetValue(EasyGridCellInfo info){
            this.IsEditable = info.IsEditable;
            this.TextAlignMode = info.TextAlignMode;
            SetValue(info.Text);
        }

        
        public void SetValue(String text){
            if (base.Value==null || base.Value.Equals(text) == false)
            {
                base.Value = text;
                RePaint();
            }
        }

        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= _parent.FirstDisplayedScrollingRowIndex && this.ColumnIndex >= _parent.FirstDisplayedScrollingColumnIndex)
            {
                try
                {
                    Rectangle rect = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, true);
                    if(rect.Width>0 && rect.Height>0){
                        PaintCell(rect, _parent.CreateGraphics());
                
                        _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
                    }
                }
                catch { }
                //_parent.UpdateCellValue(this.ColumnIndex, this.RowIndex);
            }
        }
        /*
        protected override void PaintBorder(Graphics g, Rectangle clipBound, Rectangle drawRect, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
          //  base.PaintBorder(g, clipBound, drawRect, cellStyle, advancedBorderStyle);
            int y = drawRect.Y + drawRect.Height-1;
            g.DrawLine(new Pen(_parent.GridColor, 0.1f), drawRect.X, y, drawRect.X + drawRect.Width, y);
                
        }
         */
        void DrawStringResizedForCellWid(SizeF size, Rectangle cellBounds, int textMargin, int textY, Graphics g)
        {
            Font font = new Font(_font, FontStyle.Regular);
            if (size.Width > this.OwningColumn.Width)
            {
                SizeF Letter1 = g.MeasureString(Text[0].ToString(), _font);
                if (this.OwningColumn.Width <= Letter1.Width)//이 경우에는 숨어있는 cell이라고 본다. loop문에 들어가면 무한루프가 된다.
                {
                }
                else
                {
                    while (size.Width > this.OwningColumn.Width)
                    {
                        font = new Font(FontFamily.GenericMonospace, font.Size - 0.5f);// new Font( font.FontFamily, font.Size - 0.5f);// font.Size = font.Size - 0.5f;
                        size = g.MeasureString(Text, font);
                    }

                    g.DrawString(Text, font, _fontColor, cellBounds.X + textMargin, textY);
                }
            }
            else
            {
                DrawSingleLine(cellBounds, g, size, textY, textMargin);
            }
        }


        public int GetLinesOfATextLine(SizeF textSize)
        {
            int size = (int)(textSize.Width + _textMargin);
            int lines =  size / this.OwningColumn.Width; // 5/2 = 2
            if ((size % this.OwningColumn.Width) > 0) lines++; // 5%2 = 1

            if (this.OwningColumn.Width <= 15)//이 경우에는 숨어있는 cell이라고 본다. loop문에 들어가면 무한루프가 된다.
            {
                lines = 1;
            }
            return lines;
        }

        public int GetTextLines(String text)
        {
            Graphics g = _parent.CreateGraphics();
            String[] lines = text.Split("\n".ToCharArray());
            int numLines = 0;
            
            for (int i = 0; i < lines.Length; i++)
            {
                SizeF textSize = g.MeasureString(lines[i], _font);
                numLines += GetLinesOfATextLine(textSize);
            }
            return numLines;
        }

        public SizeF GetTextSize(String text)
        {
            Graphics g = _parent.CreateGraphics();
            return g.MeasureString(text, _font);
        }

        public int GetCellHeight()
        {

            float textHeight =GetTextSize("Tj").Height;
            int numLines = GetTextLines(Text);
            
            if (numLines == 1) return _parent.BaseRowHeight;
            else return (int)((numLines * (textHeight + _textLineSpace)) + _textMargin * 2);
        }

        public float GetAllTextHeight()
        {
            float textHeight = GetTextSize("Tj").Height;
            int numLines = GetTextLines(Text);

            if (numLines == 1) return textHeight;
            else return (float)((numLines * (textHeight + _textLineSpace)) );
        }

        void DrawSingleLine(Rectangle cellBounds,Graphics g, SizeF textSize, int textY, int textMargin)
        {
            double d;
            String text = Text;
            if (cellBounds.Width <= 2)//이 경우에는 숨어있는 cell이라고 본다.
            {
                return;
            }
            else
            {
                
                while (text.Length>1 && textSize.Width > this.OwningColumn.Width) //셀의 크기보다 크다면 자른다.
                {
                    text = text.Substring(0, text.Length - 1);
                    textSize = g.MeasureString(Text, _font);
                }
                if (text.Length < Text.Length) this.ToolTipText = Text;
                if (text.Length == 0) return;
                
            }

            switch (_textAlignMode)
            {
                case TextAlignModes.Center:
                    g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width) / 2), textY);
                    break;
                case TextAlignModes.Right:
                    g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                    break;
                case TextAlignModes.NumberOnlyRight:
                    if (double.TryParse(text, out d))//right
                    {
                        g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                    }
                    else//left
                    {
                        g.DrawString(text, _font, _fontColor, cellBounds.X + textMargin, textY);
                    }
                    break;
                case TextAlignModes.NumberRightTextCenter:
                    if (double.TryParse(text, out d))//right
                    {
                        g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                    }
                    else//center
                    {
                        g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width) / 2), textY);
                    }
                    break;
                case TextAlignModes.Left:
                default:
                    g.DrawString(text, _font, _fontColor, cellBounds.X + textMargin, textY);
                    break;


            }
        }
        int _textLineSpace = 2;
        public int TextLineSpace
        {
            get
            {
                return _textLineSpace;
            }
            set
            {
                _textLineSpace = value;
            }
        }
        void DrawStringMultiLines(Rectangle cellBounds, int textMargin, int textY, Graphics g)
        {
            Font font = new Font(_font, FontStyle.Regular);
            /*
            EasyGridRow row = OwningRow as EasyGridRow;
            
            bool isFirstFileOpenBox = false;
            
            int firstTextCellIndex = -1;
            int lastTextCellIndex = -1;
            
            for (int i = _parent.FirstDisplayedScrollingColumnIndex; i < (_parent.FirstDisplayedScrollingColumnIndex + _parent.DisplayedColumnCount(true)); i++)
            {
                if (row[i].ItemType == ItemTypes.FileOpenBox && row.Cells[i].OwningColumn.Visible == true)
                {
                    if (firstTextCellIndex < 0) firstTextCellIndex = i;
                    lastTextCellIndex = i;
                }
            }
            if (firstTextCellIndex == this.ColumnIndex)
            {
                isFirstTextBox = true;
            }
             */
            int lines = GetTextLines(Text);
            SizeF textSize = GetTextSize(Text);
            float TextHeight = GetTextSize("Tj").Height;
            if (lines>1) //multiLine
            {
                String[] textLines = Text.Split("\n".ToCharArray());
                int count = 0;//line번호
                for (int i = 0; i < textLines.Length; i++)
                {
                    String text = textLines[i];
                    while (text.Length > 0)
                    {
                        string rest = "";
                        int ret = 0;
                        while (text.Length > 0 && ((int)(g.MeasureString(text, font)).Width) > (this.OwningColumn.Width + 1))
                        {
                            rest = text.Substring(text.Length - 1) + rest; //마지막 글자를 붙임..
                            text = text.Substring(0, text.Length - 1);
                        }
                        g.DrawString(text, font, _fontColor, cellBounds.X + textMargin, cellBounds.Top + /*textMargin +*/ ((TextHeight + (float)_textLineSpace) * count));
                        text = rest;
                        count++;//line 번호.
                        
                        if (lines < count) break;//칸을 넘어가면 자른다.
                        
                    }
                    if (lines < count) break;//칸을 넘어가면 자른다.
                }
                /*
                if (count != lines)
                {
                    lines = count;
                }
                */
            }
            else //single line
            {
                DrawSingleLine(cellBounds, g, textSize, textY, textMargin);
            }

            //if(row.IsTextChanged){
/*

            if (lastTextCellIndex == this.ColumnIndex)//마지막 TextBoxCell. 첫째열이자 마지막열일수도 있으므로, else가 아니다.
            {
                int height = (lines == 1) ? 27 : (int)(lines * textSize.Height + 4);
                row.SetRowHeight(height, RowHeightSettingModes.UpdateMaxNow);
            }
            else if (isFirstTextBox)
            {
                int height = (lines == 1) ? 27 : (int)(lines * textSize.Height + 4);
                row.SetRowHeight(height, RowHeightSettingModes.SetWithThis);
            }
            else
            {
                int height = (lines == 1) ? 27 : (int)(lines * textSize.Height + 4);
                row.SetRowHeight(height, RowHeightSettingModes.SetForMax);
            }
 */
        }
        /*
        void DrawStringNormal(SizeF size, Rectangle cellBounds, int textMargin, int textY, Graphics g)
        {
            double d;

            Font font = _font;
            switch (alignMode)
            {
                case TextAlignModes.Center:
                    g.DrawString(Text, font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - size.Width) / 2), textY);
                    break;
                case TextAlignModes.Right:
                    g.DrawString(Text, font, _fontColor, cellBounds.X - textMargin + (int)((OwningColumn.Width - size.Width)), textY);
                    break;
                case TextAlignModes.NumberOnlyRight:
                    if (double.TryParse(Text, out d))//right
                    {
                        g.DrawString(Text, font, _fontColor, cellBounds.X - textMargin + (int)((OwningColumn.Width - size.Width)), textY);
                    }
                    else//left
                    {
                        g.DrawString(Text, font, _fontColor, cellBounds.X + textMargin, textY);
                    }
                    break;
                case TextAlignModes.NumberRightTextCenter:
                    if (double.TryParse(Text, out d))//right
                    {
                        g.DrawString(Text, font, _fontColor, cellBounds.X - textMargin + (int)((OwningColumn.Width - size.Width)), textY);
                    }
                    else//center
                    {
                        g.DrawString(Text, font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - size.Width) / 2), textY);
                    }
                    break;
                case TextAlignModes.Left:
                default:
                    g.DrawString(Text, font, _fontColor, cellBounds.X + textMargin, textY);
                    break;


            }
         
        }
        */
        FontStyle _fontStyle = FontStyle.Regular;
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                _font = new Font(_font, _fontStyle);
            }
        }

        TextViewModes _textViewMode = TextViewModes.Default;
        public TextViewModes TextViewMode
        {
            get
            {
                if (_textViewMode == TextViewModes.Default)
                {
                    if ((OwningColumn as IEasyGridColumn).ItemType == ItemTypes.FileOpenBox)
                    {
                        if ((OwningColumn as EasyGridFileOpenBoxColumn).TextViewMode == TextViewModes.Default)
                        {
                            return _parent.TextViewMode;
                        }
                        else
                        {
                            return (OwningColumn as EasyGridFileOpenBoxColumn).TextViewMode;
                        }
                    }
                    else return _parent.TextViewMode;
                }
                else return _textViewMode;
            }
            set
            {
                _textViewMode = value;
            }
        }

        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle initCellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //IEasyGridCell baseCell = GetSpanBaseCell();
            //DataGridViewCell cell = baseCell as DataGridViewCell;
            cellStyle.BackColor = CellFunctions.BackColor(this);

            String text = this.Text;
            Rectangle textArea = getTextArea(initCellBounds);
            PaintCell(textArea, g);
            Rectangle buttonArea = getButtonArea(initCellBounds);
            //g.FillRectangle(Brushes.Gray, getButtonArea(initCellBounds));
            CellFunctions.DrawLensBack(this, buttonArea, g, DataGridView.GridColor, false, this.Enabled);
            g.DrawString("...", _font, Brushes.Black, new Point(buttonArea.X + 5, buttonArea.Y));
            //PaintCell(initCellBounds, g);
            /*
            if (baseCell.Equals(this) == false)
            {
                //do nothing..
                
            }
            else
            {
                PaintCell(initCellBounds, g);
            }
             */
            paintParts = DataGridViewPaintParts.None;
            
        }
        int _textMargin = 2;
        void PaintCell(Rectangle initCellBounds, Graphics g)
        {
            System.Drawing.Drawing2D.GraphicsContainer con =  g.BeginContainer();
            Rectangle cellBounds =  initCellBounds;
            //Rectangle drawRect = initCellBounds;
            #region test
            /*
            if (ColSpan > 1)
            {
                cellBounds = new Rectangle(cellBounds.X, cellBounds.Y, 0, cellBounds.Height);
                cellBounds.Width = _parent.GetColumnSize(this.ColumnIndex, ColSpan);
                
            }
            if (RowSpan > 1)
            {
                int firstRow = RowIndex;
                int lastRow = RowIndex + RowSpan-1;
                int minus = 0;
                if (RowIndex < _parent.FirstDisplayedCell.RowIndex)
                {
                    firstRow = _parent.FirstDisplayedCell.RowIndex;
                    minus = _parent.GetRowsSize(RowIndex, firstRow - RowIndex);// RowIndex - _parent.FirstDisplayedCell.RowIndex;
                }
                if (lastRow > (_parent.FirstDisplayedCell.RowIndex + _parent.DisplayedRowCount(true)))
                {
                    lastRow = (_parent.FirstDisplayedCell.RowIndex + _parent.DisplayedRowCount(true))-1;
                }
                Rectangle firstRect = _parent.GetCellDisplayRectangle(this.ColumnIndex, firstRow, true);
                Rectangle lastRect = _parent.GetCellDisplayRectangle(this.ColumnIndex, lastRow, true);
                drawRect = new Rectangle(firstRect.X, firstRect.Y, firstRect.Width, lastRect.Y - firstRect.Y + lastRect.Height);

                cellBounds = new Rectangle(drawRect.X, drawRect.Y-minus, drawRect.Width, _parent.GetRowsSize(RowIndex, RowSpan));
                
            }
            
            if (ColSpan > 1 || RowSpan > 1)
            {
                CellFunctions.DrawPlainBackground(this, _enabled, drawRect, g, this.RowIndex, false, _parent.GridColor);
                
             
            }
            else
            {
                CellFunctions.DrawPlainBackground(this, _enabled, drawRect, g, this.RowIndex, this.Selected, _parent.GridColor);
            }
             */
            #endregion
            
            int textY = CellFunctions.TextCenterYInRact(cellBounds, g, _font);// cellBounds.Y + (int)((cellBounds.Height - textSize.Height) / 2.0);
            int textMargin = _textMargin;

            SizeF textSize = g.MeasureString(Text, _font);
            textSize.Width -= textMargin;

            if (textY < cellBounds.Y )//|| (textY + textSize.Height) > (cellBounds.Y + cellBounds.Height))
            {
                //don't draw text..
            }
            else
            {
                

                if (TextViewMode == TextViewModes.ResizeForCellWid)
                {
                    CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);

                    DrawStringResizedForCellWid(textSize, cellBounds, textMargin, textY, g);
                }
                else if (TextViewMode == TextViewModes.MultiLines)
                {
                    int newHeight = (OwningRow as EasyGridRow).SetRowHeight();
                    cellBounds.Height = newHeight;
                    CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);

                    DrawStringMultiLines(cellBounds, textMargin, textY, g);
                }
                else //default.. single line..
                {
                    CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
                    DrawSingleLine(cellBounds, g, textSize, textY, textMargin);
                    //DrawStringNormal(size, cellBounds, textMargin, textY, g);
                    //g.DrawString(Text, _font, _fontColor, cellBounds.X + 2, textY);
                }
                //int y = drawRect.Y + drawRect.Height - 1;
                //g.DrawLine(new Pen(_parent.GridColor, 0.1f), drawRect.X, y, drawRect.X + drawRect.Width, y);
            }

            g.EndContainer(con);
            g.Flush();
        }
    }
}
