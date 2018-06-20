using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridTextBoxCell:DataGridViewTextBoxCell,IEasyGridCell
    {
        EasyGridViewParent _parent;
        public EasyGridTextBoxCell(EasyGridViewParent parent)
            : base()
        {
            _parent = parent;
            _info = new CellSpanInfo(this);
            Style.SelectionBackColor = Color.LightSkyBlue;
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
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnDoubleClick(e);
            }
            else if (_enabled)
            {

                IEasyGridCell baseCell = this.Span.SpanBaseCell;
                if (baseCell.Equals(this) == false)
                {
                    _parent.InvalidateCell(baseCell as DataGridViewCell);
                    base.OnDoubleClick(e);
                    return;

                }

                if (_isEditable)
                {
                    _parent.CurrentCell = this;
                    _parent.BeginEdit(true);
                }
                base.OnDoubleClick(e);
            }

            
            
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
            get { return ItemTypes.TextBox; }
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

            //PaintSpanedCell(e.Graphics);
            //PaintCell(e.CellBounds, e.Graphics);
            //e.Handled = false;
        }
        TextAlignModes _textAlignMode = TextAlignModes.None;
        public TextAlignModes TextAlignMode
        {
            get {
                if (_textAlignMode == TextAlignModes.None)
                {
                    TextAlignModes alignMode;
                    if ((OwningColumn is EasyGridTextBoxColumn) && (OwningColumn as EasyGridTextBoxColumn).CellTextAlignMode != TextAlignModes.None)
                        alignMode = (OwningColumn as EasyGridTextBoxColumn).CellTextAlignMode;
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
            set {
                _isEditable = value;
                if (value == false)
                {
                    
                    //base.DetachEditingControl();
                }
                else
                {
                    //base.InitializeEditingControl(this.RowIndex, "", this.Style);
                }
            }
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
                    EasyGridTextBoxCell txtCell = cell as EasyGridTextBoxCell;

                    if (value == true)
                    {
                        
                        for (int i = txtCell.RowIndex; i < txtCell.RowIndex + txtCell.RowSpan; i++)
                        {
                            EasyGridTextBoxCell relCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridTextBoxCell);
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
                                EasyGridTextBoxCell relCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridTextBoxCell);

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
                            EasyGridTextBoxCell txtCell = (_parent.Rows[i].Cells[ColumnIndex] as EasyGridTextBoxCell);
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

        public Font Font
        {
            get
            {
                if (this.Style.Font == null) return _parent.DefaultCellStyle.Font;
                else return this.Style.Font;
            }

        }

        //Brush _fontColor = Brushes.Black;
        public Color FontColor
        {
            get { return Style.ForeColor; }
            set
            {
                Style.ForeColor = value;
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
            switch(this.TextAlignMode){
                case TextAlignModes.Left:
                case TextAlignModes.None:
                     base.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    break;
                case TextAlignModes.Center:
                    base.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    break;
                case TextAlignModes.NumberRightTextCenter:
                    double num;
                    if (double.TryParse(info.Text, out num))
                    {
                        base.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else
                    {
                        base.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    break;
                case TextAlignModes.Right:
                    base.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    break;
            }

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
            if(this.DataGridView!=null) _parent.InvalidateCell(this);
            //PaintSpanedCell(_parent.CreateGraphics(), true);
        }
        /*
        protected override void PaintBorder(Graphics g, Rectangle clipBound, Rectangle drawRect, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
          //  base.PaintBorder(g, clipBound, drawRect, cellStyle, advancedBorderStyle);
            int y = drawRect.Y + drawRect.Height-1;
            g.DrawLine(new Pen(_parent.GridColor, 0.1f), drawRect.X, y, drawRect.X + drawRect.Width, y);

                
        }
        */
        /*
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
        */
        /*
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
        */
        public SizeF GetTextSize(String text)
        {
            Graphics g = _parent.CreateGraphics();
            return g.MeasureString(text, Font);
        }
        /*
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
        */

        StringFormatFlags _singleLineFormat = StringFormatFlags.NoWrap;
        public StringFormatFlags SingleLineFormat
        {
            get { return _singleLineFormat; }
            set { _singleLineFormat = value; }
        }

        SizeF _textSize = new SizeF(0, 0);
        /// <summary>
        /// SingleLine모드일때 Text의 실제 크기를 저장하고 있다가 리턴한다.
        /// </summary>
        public SizeF TextSize
        {
            get { return _textSize; }
        }
        /*
        void DrawSingleLine(Rectangle cellBounds,Graphics g, SizeF textSize, int textY, int textMargin)
        {
            double d;
            String text = Text;
            textSize = g.MeasureString(Text, _font);
            _textSize = textSize;
            if (cellBounds.Width <= 2)//이 경우에는 숨어있는 cell이라고 본다.
            {
                
                return;
            }
            else
            {
                
                textY = (int)((cellBounds.Height - textSize.Height) / 2);
                if (textY < 0 && textSize.Height > _parent.BaseRowHeight) textY = 0;
                else if(cellBounds.Height < _parent.BaseRowHeight && textSize.Height<_parent.BaseRowHeight)
                {
                    textY = (int)((_parent.BaseRowHeight - textSize.Height) / 2);
                }

                if (textSize.Width > this.OwningColumn.Width)
                {
                }

                
            }
            float x = cellBounds.X + textMargin;
            float y = cellBounds.Y + textY;
            PointF textPos = new PointF(x, y);

            RectangleF textBounds = new RectangleF(new PointF(cellBounds.X + textMargin, cellBounds.Y + textY), new SizeF(OwningColumn.Width - textMargin, _parent.BaseRowHeight - textY));
            StringFormat format = new StringFormat(_singleLineFormat);
            switch (_textAlignMode)
            {
                case TextAlignModes.Center:
                    //g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width) / 2), textY);
                    if (textSize.Width > OwningColumn.Width)
                    {
                        g.DrawString(text, _font, _fontColor, textPos, format);
                        //g.DrawString(text, _font, _fontColor, textBounds, format);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(textBounds.X + (textBounds.Width - textSize.Width) / 2, cellBounds.Y + textY, textSize.Width, textSize.Height);
                        g.DrawString(text, _font, _fontColor, rect, format);
                    }
                    
                    break;
                case TextAlignModes.Right:
                    {
                        //g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                        if (textSize.Width > OwningColumn.Width)
                        {
                            g.DrawString(text, _font, _fontColor, textPos, format);
                            //g.DrawString(text, _font, _fontColor, textBounds, format);
                        }
                        else
                        {
                            RectangleF rect = new RectangleF(textBounds.X + (textBounds.Width - textSize.Width), cellBounds.Y + textY, textSize.Width, textSize.Height);
                            g.DrawString(text, _font, _fontColor, rect, format);
                        }

                        break;
                    }
                case TextAlignModes.NumberOnlyRight:
                    {

                        RectangleF rect = new RectangleF(textBounds.X + (textBounds.Width - textSize.Width), cellBounds.Y + textY, textSize.Width, textSize.Height);
                        if (double.TryParse(text, out d))//right
                        {
                            //g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                            if (textSize.Width > OwningColumn.Width)
                            {
                                g.DrawString(text, _font, _fontColor, textPos, format);
                                //g.DrawString(text, _font, _fontColor, textBounds, format);
                            }
                            else
                            {
                                g.DrawString(text, _font, _fontColor, rect, format);
                            }
                            
                        }
                        else//left
                        {
                            g.DrawString(text, _font, _fontColor, textPos, format);
                            //g.DrawString(text, _font, _fontColor, textBounds, format);
                                
                           //g.DrawString(text, _font, _fontColor, cellBounds.X + textMargin, textY);

                        }
                        break;
                    }

                    
                case TextAlignModes.NumberRightTextCenter:
                    {
                        float centerX = (textBounds.Width - textSize.Width) / 2;
                        RectangleF rightRect = new RectangleF(textBounds.X + (textBounds.Width - textSize.Width), cellBounds.Y + textY, textSize.Width, cellBounds.Height-textY);
                        RectangleF centerRect = new RectangleF(textBounds.X + centerX, cellBounds.Y + textY, cellBounds.Width-centerX, cellBounds.Height-textY);
                       
                        if (double.TryParse(text, out d))//right
                        {
                            if (textSize.Width > OwningColumn.Width)
                            {
                                g.DrawString(text, _font, _fontColor, textPos, format);
                                //g.DrawString(text, _font, _fontColor, textBounds, format);
                            }
                            else
                            {
                                g.DrawString(text, _font, _fontColor, rightRect, format);
                            }
                            //g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width)) - textMargin, textY);
                        }
                        else//center
                        {
                            if (textSize.Width > OwningColumn.Width)
                            {
                                g.DrawString(text, _font, _fontColor, textPos, format);
                                //g.DrawString(text, _font, _fontColor, textBounds, format);
                            }
                            else
                            {
                                g.DrawString(text, _font, _fontColor, centerRect, format);
                            }
                            //g.DrawString(text, _font, _fontColor, cellBounds.X + (int)((OwningColumn.Width - textSize.Width) / 2), textY);
                        }
                        break;
                    }
                    
                case TextAlignModes.Left:
                default:
                    g.DrawString(text, _font, _fontColor, textPos, format);
                               
                    //g.DrawString(text, _font, _fontColor, textBounds, format);
                    //g.DrawString(text, _font, _fontColor, cellBounds.X + textMargin, textY);
                    break;


            }
        }
        */
        /*
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
         */
        /*
        void DrawStringMultiLines(Rectangle cellBounds, int textMargin, int textY, Graphics g)
        {
            Font font = new Font(_font, FontStyle.Regular);
            
            int lines = GetTextLines(Text);
            
            SizeF wordsSize = g.MeasureString(Text, _font, cellBounds.Width-textMargin);
            SizeF textSize = GetTextSize(Text);
            if ((textSize.Width + textMargin) < cellBounds.Width) //1줄
            {
                DrawSingleLine(cellBounds, g, textSize, textY, textMargin);
            }
            else
            {
                g.DrawString(Text, _font, _fontColor, new RectangleF(new Point(cellBounds.X + textMargin, cellBounds.Y), wordsSize));
            }
            
        }
         */
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
        public FontStyle FontStyle
        {
            get { return base.Style.Font.Style; }
            set
            {
     
                base.Style.Font = new Font(base.Style.Font, value);
                
            }
        }

        TextViewModes _textViewMode = TextViewModes.Default;
        public TextViewModes TextViewMode
        {
            get
            {
                if (_textViewMode == TextViewModes.Default)
                {
                    if ((OwningColumn as IEasyGridColumn).ItemType == ItemTypes.TextBox)
                    {
                        if ((OwningColumn as EasyGridTextBoxColumn).TextViewMode == TextViewModes.Default)
                        {
                            return _parent.TextViewMode;
                        }
                        else
                        {
                            return (OwningColumn as EasyGridTextBoxColumn).TextViewMode;
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
        /*
        protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            if (Span.SpanPos == SpanPosition.NoSpan)
            {
                base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);
            }
            else if (Span.SpanPos == SpanPosition.SpanBase)
            {
                
                base.PaintBorder(graphics, clipBounds, Span.GetSpanBaseRect(), cellStyle, advancedBorderStyle);        
                
                
            }else if(Span.SpanPos == SpanPosition.Spanned){
                base.PaintBorder(graphics, clipBounds, Span.GetSpanBaseRect(), cellStyle, advancedBorderStyle);        
                
            }
            else
            {
                
            }
        }
        */
        
        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle initCellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //IEasyGridCell baseCell = GetSpanBaseCell();
            //DataGridViewCell cell = baseCell as DataGridViewCell;
            //cellStyle.BackColor = CellFunctions.BackColor(this);
            //String text = this.Text;
            
            Span.Paint(base.Paint,
                g,
                clipBounds,
                initCellBounds,
                rowIndex,
                cellState,
                value,
                formattedValue,
                ToolTipText,
                cellStyle,
                advancedBorderStyle,
                paintParts);
            
            
        }

        
        
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if(cell!=null) cell.OnMouseDown(e);
            }else if (_enabled) base.OnMouseDown(e);

        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnMouseUp(e);
            }
            else if (_enabled) base.OnMouseUp(e);
        }
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnMouseClick(e);
            }
            else if (_enabled) base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnMouseDoubleClick(e);
            }
            else if (_enabled) base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseEnter(int rowIndex)
        {
            if (_enabled) if (Span.SpanPos == SpanPosition.Spanned)
                {
                    EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                    if (cell != null) cell.OnMouseEnter(rowIndex);
                }
                else base.OnMouseEnter(rowIndex);
        }
        protected override void OnMouseLeave(int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnMouseLeave(rowIndex);
            }
            else if (_enabled) base.OnMouseLeave(rowIndex);
        }
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnClick(e);
            }
            else if (_enabled) base.OnClick(e);
        }

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnContentClick(e);
            }
            else if (_enabled) base.OnContentClick(e);
        }
        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnContentDoubleClick(e);
            }
            else if (_enabled) base.OnContentDoubleClick(e);
        }
        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnEnter(rowIndex, throughMouseClick);
            }
            else if (_enabled) base.OnEnter(rowIndex, throughMouseClick);
        }
        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnKeyDown(e, rowIndex);
            }
            else if (_enabled) base.OnKeyDown(e, rowIndex);
        }
        protected override void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnKeyPress(e, rowIndex);
            }
            else if (_enabled) base.OnKeyPress(e, rowIndex);
        }
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (Span.SpanPos == SpanPosition.Spanned)
            {
                EasyGridTextBoxCell cell = (Span.SpanBaseCell as EasyGridTextBoxCell);
                if (cell != null) cell.OnKeyUp(e, rowIndex);
            }
            else if (_enabled) base.OnKeyUp(e, rowIndex);
        }
        
        /*
        int _textMargin = 2;
        void PaintCell(Rectangle initCellBounds, Graphics g)
        {
            System.Drawing.Drawing2D.GraphicsContainer con =  g.BeginContainer();
            Rectangle cellBounds =  initCellBounds;
            //Rectangle drawRect = initCellBounds;
           
            
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
                
                if (Span.SpanPos == SpanPosition.NoSpan)
                {
                    CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
                }
                else
                {
                    CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, 1,false, _parent.GridColor);
                }
                //CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
                //CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);


                if (TextViewMode == TextViewModes.ResizeForCellWid)
                {
                   
                    DrawStringResizedForCellWid(textSize, cellBounds, textMargin, textY, g);
                }
                else if (TextViewMode == TextViewModes.MultiLines)
                {


                    DrawStringMultiLines(cellBounds, textMargin, textY, g);
                }
                else //default.. single line..
                {
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
          */

    }
         
}
