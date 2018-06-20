using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridKeyValueCell : DataGridViewTextBoxCell, IEasyGridCell
    {
        DataGridView _parent;
        
        //public Dictionary<String, String> KeyValue = new Dictionary<string, string>();
        //public Dictionary<String, Color> KeyColor = new Dictionary<string, Color>();

        public EasyGridKeyValueCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            //_parent.CellMouseMove +=new DataGridViewCellMouseEventHandler(_parent_CellMouseMove);
            _keyValue = new EasyGridKeyValueCollections(this);
            _timerOver.Tick += new EventHandler(_timerOver_Tick);
            _timerOver.Interval = 200;
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        FontStyle _fontStyle = FontStyle.Regular;
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set {
                _fontStyle = value; 
                _font = new Font(_font, _fontStyle);
                this.Style.Font = _font;//this.Font.Bold = 
            }
        }


        void _timerOver_Tick(object sender, EventArgs e)
        {
            Point pt = Control.MousePosition;
            Point pt1 = _parent.PointToClient(pt);
            Point pt2 = (_floatingList==null)? pt1 : _floatingList.PointToClient(pt);
            Rectangle rect1 = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
            Rectangle rect2 = (_floatingList == null) ? rect1 : new Rectangle(0, 0, _floatingList.Width, _floatingList.Height);// _floatingList.Bounds;
            if (rect1.Contains(pt1) == false && rect2.Contains(pt2)==false)
            {
                if (_isContentShown)
                {
                    _parent.Controls.Remove(_floatingList);
                    _floatingList.Dispose();
                    _floatingList = null;
                    _isContentShown = false;
                }
                _timerOver.Stop();
            }
        }



        #region eventHandlers
        bool _isContentShown = false;
        ContextListForm _floatingList = null;
        Timer _timerOver = new Timer();
        void OpenPopup()
        {
            if (Items.Count == 0) return;
            if (_isContentShown == false)
            {
                if (_floatingList == null)
                {
                    
                    _floatingList = new ContextListForm();
                    _floatingList.Width = (int)_maxWidth;
                    Items.SetNormalFontColor(Brushes.Black);

                    _floatingList.BackColor = Color.White; //background

                    for (int i = 0; i < Items.Count; i++)
                    {
                        String text = Items.Keys.ElementAt(i) + ":" + Items.Values.ElementAt(i);
                        _floatingList.Items.Add(text, Items.FontColor(i));
                    }

                    _isContentShown = true;
                    if (_timerOver.Enabled == false) _timerOver.Start();
                    Rectangle cellBounds = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);

                    Point pt2 = cellBounds.Location;
                    pt2 = _parent.PointToScreen(pt2);
                    _floatingList.Location = new Point(pt2.X, pt2.Y + cellBounds.Height);

                    //_floatingList.TopMost = true;
                    _floatingList.ResumeLayout();
                    _floatingList.Show();
                    _parent.Focus();

                    _floatingList.SetBounds(pt2.X, pt2.Y + cellBounds.Height, 0, 0, BoundsSpecified.Location);
                }
            }
            else
            {
                Rectangle cellBounds = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
                //Rectangle parentBounds = _parent.Bounds;

                Point pt2 = cellBounds.Location;
                pt2 = _parent.PointToScreen(pt2);

                _floatingList.SetBounds(pt2.X, pt2.Y + cellBounds.Height, 0, 0, BoundsSpecified.Location);

            }
        }
        void ClosePopup()
        {
            if (_isContentShown)
            {
                _parent.Controls.Remove(_floatingList);
                _floatingList.Dispose();
                _floatingList = null;
                _isContentShown = false;
            }
        }

        void  _parent_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            if (e.ColumnIndex == this.ColumnIndex && e.RowIndex == this.RowIndex)
            {
                //Control root = _parent;
                //Control temp = root;
                //while ((temp = root.Parent) != null) root = temp;
                OpenPopup();

            }
            else 
            {
                ClosePopup();
            }
        }
        #endregion

        #region Properties
        int _colSpan = 1;
        public int ColSpan
        {
            get { return _colSpan; }
            set { _colSpan = value; }
        }
        int _rowSpan = 1;
        public int RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = value; }
        }
        
        public override Type ValueType
        {
            get
            {
                return typeof(EasyGridKeyValueCollections);
            }
        }

        public new object Value
        {
            get
            {
                return _keyValue;
            }
            set
            {
                if (value is EasyGridCellInfo)
                {
                    EasyGridCellInfo info = value as EasyGridCellInfo;
                    if (info.KeyColor != null)
                    {
                        this.Items.Add(info.KeyValue, info.KeyColor);
                    }
                    else
                    {
                        Items.Add(info.KeyValue);
                    }
                }
                else if (value is Dictionary<String, String>)
                {
                    Items.Add(value as Dictionary<String, String>);
                }
                else if (value == null)
                {
                    Items.Clear();
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[]{
                        typeof(EasyGridCellInfo), typeof( Dictionary<String, String>)});
                }
            }
        }

        public void SetValue(EasyGridCellInfo info)
        {
            if (info.ItemType == ItemTypes.KeyValue)
            {
                this.Value = info.KeyValue;
                if (info.KeyColor != null)
                {
                    foreach (String key in info.KeyColor.Keys)
                    {
                        this.Items.SetFontColor(key, info.KeyColor[key]);
                    }
                }
            }
        }

        public void SetValue(Dictionary<String, String> keyValue)
        {
            this.Items.Clear();
            this.Items.Add(keyValue);
        }

        public void SetValue(Dictionary<String, String> keyValue, Dictionary<String, Brush> keyColor)
        {
            this.Items.Clear();
            this.Items.Add(keyValue);
                foreach (String key in keyColor.Keys)
                {
                    this.Items.SetFontColor(key, keyColor[key]);
                }
            
        }
               
        EasyGridKeyValueCollections _keyValue;
        public EasyGridKeyValueCollections Items
        {
            get
            {
                return _keyValue;
            }
        }

        public String Text
        {
            get
            {
                String text = "";
                for (int i = 0; i < _keyValue.Count; i++)
                {
                    text += _keyValue.Keys.ElementAt(i);
                    String value = _keyValue.Values.ElementAt(i);
                    if (value != null && value.Length > 0) text += ":" + value;
                    text += "  ";
                }
                return text;
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
                _enabled = value;
                RePaint();
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.KeyValue; }
        }

        bool _isEditable = true;
        public bool IsEditable
        {
            get { return false; }
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
                _fontColor = value;
                Items.SetNormalFontColor(value);
                RePaint();
            }
        }

        private SizeF TextSize(String text, Graphics g)
        {
            // Set the return value to the normal node bounds.

            if (text != null)
            {
                return g.MeasureString(text, _font);
            }
            return new SizeF(0, 0);

        }

        #endregion


        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
        }
        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
        
        #region overrides
        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            
            try
            {
                PaintCell(g, cellBounds);
            }
            catch (InvalidOperationException) { }
            //_hdc = g.GetHdc(); //다음에쓸 RePaint를 위해 저장한다.
            //_bounds = cellBounds;
            paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.Focus | DataGridViewPaintParts.SelectionBackground| DataGridViewPaintParts.ContentForeground;
            
            base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            
        }
        #endregion
        float _maxWidth = 0;
        int _fontHeight = 0;
        protected virtual void PaintCell(Graphics g, Rectangle cellBounds)
        {
            if (CellFunctions.MouseHitTest(_parent, cellBounds))
            {
                OpenPopup();
            }
            else
            {
                ClosePopup();
            }

            Brush fontColor = new SolidBrush(this.Style.ForeColor);// CellFunctions.DrawPlainBackground(this, _enabled, cellBounds, g, this.RowIndex, this.Selected, _parent.GridColor);
            
            //SizeF textSize = TextSize("Text", g);
            //_fontHeight = (int)textSize.Height;
            //String allText = "";
            Items.SetNormalFontColor(fontColor);
            String text = "";
            int remainedSize = cellBounds.Width;
            for (int i = 0; i < Items.Count; i++)
            {
                text += Items.Keys.ElementAt(i) + ":" + Items.Values.ElementAt(i);
                if (i != 0) text = "," + text;
                //allText += text;
                //textSize = TextSize(text, g);
                //if (_maxWidth < textSize.Width) _maxWidth = textSize.Width;
                /*
                if (_enabled)
                {
                    fontColor = Items.FontColor(Items.Keys.ElementAt(i));
                }
                else
                {
                    fontColor = Brushes.DarkSlateGray;
                }
                if (textSize.Width >= remainedSize)
                {
                    float over = textSize.Width - remainedSize;
                    float rate = (remainedSize - over) / remainedSize;
                    int remained = (int)(text.Length * rate);
                    if (remained >= 3)
                    {
                        text = text.Substring(0, remained - 1);
                        text += "...";
                    }
                    else
                    {
                        text = "...";
                    }
                    
                    g.DrawString(text, _font, fontColor, cellBounds.X+(cellBounds.Width-remainedSize), cellBounds.Y + (cellBounds.Height - textSize.Height) / 2);
                    break;
                }
                else
                {
                    g.DrawString(text, _font, fontColor, cellBounds.X + (cellBounds.Width - remainedSize), cellBounds.Y + (cellBounds.Height - textSize.Height) / 2);
                }
                remainedSize -= (int)textSize.Width;
                 */
            }
            base.Value = text;
        }
    }

    public class EasyGridKeyValueCollections : IDictionary<String, String>
    {
        EasyGridKeyValueCell _parent;
        Dictionary<String,String> _dic = new Dictionary<string,String>();
        Dictionary<String, Brush> _color = new Dictionary<string, Brush>();
        Dictionary<String, bool> _isVisibleInField = new Dictionary<string, bool>();
        Dictionary<String, bool> _isVisibleInTooltip = new Dictionary<string, bool>();
        Brush _normalBrush = Brushes.Black;
        internal EasyGridKeyValueCollections(EasyGridKeyValueCell parent)
        {
            _parent = parent;
        }

        internal EasyGridKeyValueCollections(EasyGridKeyValueCell parent, Brush normalBrush)
        {
            _parent = parent;
            _normalBrush = normalBrush;
        }

        public String this[String key]{
            get{
                return _dic[key];
            }
            set
            {
                _dic[key] = value;
                _parent.RePaint();
            }
        }

        public SolidBrush FontColor(String key)
        {
            if (_color.Keys.Contains(key)) return _color[key] as SolidBrush;
            else return _normalBrush as SolidBrush;
        }

        public SolidBrush FontColor(int keyIndex)
        {
            String key = _dic.Keys.ElementAt(keyIndex);
            if(key!=null && _color.Keys.Contains(key)) return _color[key] as SolidBrush;
            else return _normalBrush as SolidBrush;
        }

        public void SetFontColor(String key, Brush brush)
        {
            _color[key] = brush;
        }

        public void SetFontColor(int keyIndex, Brush fontColor)
        {
            String key = _dic.Keys.ElementAt(keyIndex);
            if (key != null && _color.Keys.Contains(key)) _color[key] = fontColor;
            else _color[key] = fontColor;
        }

        public void SetNormalFontColor(Brush color)
        {
            _normalBrush = color;
        }
        
        public void SetNormalFontColor(Color color)
        {
            _normalBrush = new SolidBrush(color);
        }

        public void Add(string key, string value, Brush color)
        {
            _dic.Add(key, value);
            _color.Add(key, color);
                
            _parent.RePaint();
        }
        
        public void Add(string key, string value, Brush color, bool isVisibleTextField, bool isVisibleToolTip)
        {
            _dic.Add(key, value);
            _color.Add(key, color);
            _isVisibleInField.Add(key, isVisibleTextField);
            _isVisibleInTooltip.Add(key, isVisibleToolTip);
            _parent.RePaint();
        }
        
        public void Add(string key, string value)
        {
            _dic.Add(key, value);
            _parent.RePaint();
        }

        public bool ContainsKey(string key)
        {
            return _dic.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dic.Keys; }
        }


        public bool TryGetValue(string key, out string value)
        {
            return _dic.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get
            {
                return _dic.Values;
            }
        }

        public void SetValue(String key, String Value)
        {
            _dic[key] = Value;
        }

        public void SetValue(String key, String Value, Brush color)
        {
            _dic[key] = Value;
            _color[key] = color;
        }

        public void SetValue(String key, String Value, Brush color, bool fieldVisible, bool tooltipVisible)
        {
            _dic[key] = Value;
            _color[key] = color;
            _isVisibleInField[key] = fieldVisible;
            _isVisibleInTooltip[key] = tooltipVisible;
        }

        public void SetValue(String key, String Value, bool fieldVisible, bool tooltipVisible)
        {
            _dic[key] = Value;
            _isVisibleInField[key] = fieldVisible;
            _isVisibleInTooltip[key] = tooltipVisible;
        }

        public void SetColor(String key, Brush color)
        {
            if (_dic.ContainsKey(key))
            {
                _color[key] = color;
            }
            else
            {
                throw new Exception("저장된 key '" + key + "'가 없습니다. 글자색을 지정하려면 글자내용을 먼저 지정해야 합니다.");
            }
        }

        public void SetVisible(String key, bool fieldVisible, bool tooltipVisible)
        {
            if (_dic.ContainsKey(key))
            {
                _isVisibleInField[key] = fieldVisible;
                _isVisibleInTooltip[key] = tooltipVisible;
            }
            else
            {
                throw new Exception("저장된 key '" + key + "'가 없습니다. Visible 속성을 지정하려면 글자내용을 먼저 지정해야 합니다.");
            }
        }


        public void Add(KeyValuePair<string, string> item)
        {
            _dic[item.Key] = item.Value;
            _parent.RePaint();
        }

        public void Add(KeyValuePair<string, string> item, Brush color)
        {
            _dic[item.Key] = item.Value;
            _color[item.Key] = color;
            _parent.RePaint();
        }
        
        public void Add(KeyValuePair<string, string> item, Brush color, bool isVisibleInField, bool isVisibleInTooltip)
        {
            _dic[item.Key] = item.Value;
            _color[item.Key] = color;
            _isVisibleInField[item.Key] = isVisibleInField;
            _isVisibleInTooltip[item.Key] = isVisibleInTooltip;
            _parent.RePaint();
        }

        public void Add(Dictionary<String, String> dic, Dictionary<String, Brush> colors)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];
                if (colors.Keys.Contains(key)) _color[key] = colors[key];
            }
        }


        public void Add(Dictionary<String, String> dic, Dictionary<String, Brush> colors, Dictionary<String, bool> fieldVisibles, Dictionary<String, bool> tooltipVisibles)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];
                if (colors.Keys.Contains(key)) _color[key] = colors[key];
                if (fieldVisibles.ContainsKey(key)) _isVisibleInField[key] = fieldVisibles[key];
                if (tooltipVisibles.ContainsKey(key)) _isVisibleInTooltip[key] = tooltipVisibles[key];
            }
        }

        public void Add(Dictionary<String, String> dic, Dictionary<String, bool> fieldVisibles, Dictionary<String, bool> tooltipVisibles)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];
                if (fieldVisibles.ContainsKey(key)) _isVisibleInField[key] = fieldVisibles[key];
                if (tooltipVisibles.ContainsKey(key)) _isVisibleInTooltip[key] = tooltipVisibles[key];
            }
        }

        public void Add(Dictionary<String, String> dic)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];

            }
        }

        public void Clear()
        {
            _dic.Clear();
            _color.Clear();
            _isVisibleInField.Clear();
            _isVisibleInTooltip.Clear();
            _parent.RePaint();
        }

        

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dic.Contains(item);
        }
        

        public bool ContainsColor(String key)
        {
            return _color.ContainsKey(key);
        }

        public bool ContainsVisibleInField(String key)
        {
            return _isVisibleInField.ContainsKey(key);
        }

        public bool ContainsVisibleInTooltip(String key)
        {
            return _isVisibleInTooltip.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _dic.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get{
            return _dic.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            bool isRemoved = _dic.Remove(item.Key);
            if (_color.ContainsKey(item.Key)) _color.Remove(item.Key);
            if (_isVisibleInField.ContainsKey(item.Key)) _isVisibleInField.Remove(item.Key);
            if (_isVisibleInTooltip.ContainsKey(item.Key)) _isVisibleInTooltip.Remove(item.Key);
            if (isRemoved) _parent.RePaint();
            return isRemoved;
            
        }
        
        public bool Remove(string key)
        {
            bool isRemoved = _dic.Remove(key);
            if (_color.ContainsKey(key)) _color.Remove(key);
            if (_isVisibleInField.ContainsKey(key)) _isVisibleInField.Remove(key);
            if (_isVisibleInTooltip.ContainsKey(key)) _isVisibleInTooltip.Remove(key);
            if (isRemoved) _parent.RePaint();
            return isRemoved;

        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        

    }
}
