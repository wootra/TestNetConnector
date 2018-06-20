using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridKeyValueCell : DataGridViewTextBoxCell, IEasyGridCell
    {
        DataGridView _parent;
        
        public Dictionary<String, String> KeyValue = new Dictionary<string, string>();
        public Dictionary<String, Color> KeyColor = new Dictionary<string, Color>();

        public EasyGridKeyValueCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            _parent.CellMouseMove +=new DataGridViewCellMouseEventHandler(_parent_CellMouseMove);
            _keyValue = new EasyGridKeyValueCollections(this);
            _timerOver.Tick += new EventHandler(_timerOver_Tick);
            _timerOver.Interval = 200;
        }

        void _timerOver_Tick(object sender, EventArgs e)
        {
            Point pt = Control.MousePosition;
            pt = _parent.PointToClient(pt);
            Rectangle rect = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
            if (rect.Contains(pt) == false)
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
        Form _floatingList = null;
        Timer _timerOver = new Timer();
        void  _parent_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            if (e.ColumnIndex == this.ColumnIndex && e.RowIndex == this.RowIndex)
            {
                //Control root = _parent;
                //Control temp = root;
                //while ((temp = root.Parent) != null) root = temp;

                if (_isContentShown == false)
                {
                    if (_floatingList == null)
                    {
                        _floatingList = new Form();
                        _floatingList.SuspendLayout();
                        _floatingList.FormBorderStyle = FormBorderStyle.None;
                        NameValuePairList list = new NameValuePairList();
                        //ListBox list = new ListBox();
                        list.Dock = DockStyle.Fill;
                        _floatingList.Controls.Add(list);
                        _floatingList.Visible = false;
                        //root.Controls.Add(_floatingList);
                        _floatingList.Width = (int)_maxWidth;


                        for (int i = 0; i < Items.Count; i++)
                        {
                            //String text = Items.Keys.ElementAt(i) + ":" + Items.Values.ElementAt(i);
                            //TextBox box = new TextBox();
                            //box.Text = text;
                            list.AddRow(Items.Keys.ElementAt(i), Items.Values.ElementAt(i));

                            list.U_ForeColors = new Color[] { Color.Black, Color.Red };
                            //list.Items.Add(box);
                            
                        }

                        int height = list.Count * _fontHeight + 10;
                        //if (height > 50) height = 50;
                        //if (height < 30)
                        {
                            _floatingList.Height = height;
                        }
                        //_parent.ContextMenu.MdiListItem.MenuItems.Clear();
                        _isContentShown = true;
                        if (_timerOver.Enabled == false) _timerOver.Start();
                        //Point pt = new Point(Control.MousePosition.X+5, Control.MousePosition.Y+5);
                        //pt = root.PointToClient(pt);
                        Rectangle cellBounds = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
                        //Rectangle parentBounds = _parent.Bounds;

                        Point pt2 = cellBounds.Location;
                        pt2 = _parent.PointToScreen(pt2);
                        _floatingList.Location = new Point(pt2.X + cellBounds.Width, pt2.Y);
                        

                        


                        //_floatingList.BringToFront();
                        _floatingList.TopMost = true;
                        _floatingList.ResumeLayout();
                        //_floatingList.Visible = true;
                        _floatingList.Show();
                        _parent.Focus();
                        
                        _floatingList.SetBounds(pt2.X + cellBounds.Width, pt2.Y, 0, 0, BoundsSpecified.Location);
                    }


                }
                else
                {
                    Rectangle cellBounds = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
                    //Rectangle parentBounds = _parent.Bounds;

                    Point pt2 = cellBounds.Location;
                    pt2 = _parent.PointToScreen(pt2);

                    _floatingList.SetBounds(pt2.X + cellBounds.Width, pt2.Y, 0, 0, BoundsSpecified.Location);

                }

            }
            else if(_isContentShown)
            {
                _parent.Controls.Remove(_floatingList);
                _floatingList.Dispose();
                _floatingList = null;
                _isContentShown = false;
            }
        }
        #endregion

        #region Properties

        public new EasyGridKeyValueCollections Value
        {
            get
            {
                return _keyValue;
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
        
        

        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
        
        #region overrides
        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            try
            {
                PaintCell(g, cellBounds);
            }
            catch (InvalidOperationException) { }
            //_hdc = g.GetHdc(); //다음에쓸 Repaint를 위해 저장한다.
            //_bounds = cellBounds;

            
        }
        #endregion
        float _maxWidth = 0;
        int _fontHeight = 0;
        protected virtual void PaintCell(Graphics g, Rectangle cellBounds)
        {
            Rectangle clearRect = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
            if (_parent == null) g.FillRectangle(Brushes.Gray, cellBounds);
            else g.FillRectangle(new SolidBrush(_parent.GridColor), cellBounds);
            
            if (this.Selected)
            {
                    g.FillRectangle(SystemBrushes.Highlight, clearRect); //background
            }
            else
            {
                if (_enabled) g.FillRectangle(Brushes.White, clearRect); //background
                else g.FillRectangle(Brushes.WhiteSmoke, clearRect);
            }
            Brush fontColor;
            if (this.Selected) fontColor = new SolidBrush(SystemColors.HighlightText);
            else fontColor = _fontColor;

            SizeF textSize = TextSize("Text", g);
            _fontHeight = (int)textSize.Height;
            String allText = "";
            String beforeText = "";
            String text = "";

            for (int i = 0; i < Items.Count; i++)
            {
                if (i != 0) allText += ", ";
                text = Items.Keys.ElementAt(i) + ":" + Items.Values.ElementAt(i);
                allText += text;
                textSize = TextSize(allText, g);
                if (_maxWidth < textSize.Width) _maxWidth = textSize.Width;

                if(textSize.Width>=cellBounds.Width){
                    float over = textSize.Width - cellBounds.Width;
                    float rate = (textSize.Width - over) / textSize.Width;
                    allText = allText.Substring(0, (int)(allText.Length * rate) - 3) + "...";
                    break;
                }
                beforeText = allText;
            }
            g.DrawString(allText, _font, fontColor, cellBounds.X, cellBounds.Y+ (cellBounds.Height-textSize.Height)/2);
        }
    }

    public class EasyGridKeyValueCollections : IDictionary<String, String>
    {
        EasyGridKeyValueCell _parent;
        Dictionary<String,String> _dic = new Dictionary<string,String>();
        Dictionary<String, Brush> _color = new Dictionary<string, Brush>();
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

        public Brush FontBrush(String key)
        {
            if (_color.Keys.Contains(key)) return _color[key];
            else return _normalBrush;
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

        public bool Remove(string key)
        {
            return _dic.Remove(key);
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

        public void Add(Dictionary<String, String> dic, Dictionary<String, Brush> colors)
        {
            foreach (String key in dic.Keys)
            {
                _dic[key] = dic[key];
                if (colors.Keys.Contains(key)) _color[key] = colors[key];
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
            _parent.RePaint();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dic.Contains(item);
        }

        public bool Contains(KeyValuePair<string, Brush> item)
        {
            return _color.Contains(item);
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
