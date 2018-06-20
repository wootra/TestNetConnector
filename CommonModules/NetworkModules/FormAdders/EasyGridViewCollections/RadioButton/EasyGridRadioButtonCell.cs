using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;
using System.Drawing;
using System.Drawing.Imaging;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridRadioButtonCell:DataGridViewCell, IEasyGridCell
    {
        EasyGridRadioButtonCollection _items;
        List<Bitmap> _images;
        List<Bitmap> _bigimages;
        List<Bitmap> _smallimages;
        Bitmap _disabledImage;
        Bitmap _bigDisabeldImage;
        Bitmap _smallDisabeldImage;
        DataGridView _parent = null;
        public EasyGridRadioButtonCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            _items = new EasyGridRadioButtonCollection(this);
            _bigimages = new List<Bitmap>();
            _smallimages = new List<Bitmap>();
            
            _bigimages.Add(Properties.Resources.Radio_Back);
            _bigimages.Add(Properties.Resources.RadioGreen);
            _bigimages.Add(Properties.Resources.RadioBlue);
            _bigimages.Add(Properties.Resources.RadioRed);
            _bigimages.Add(Properties.Resources.RadioOrange);
            _bigDisabeldImage = Properties.Resources.RadioDisabled;

            _smallimages.Add(Properties.Resources.Radio_Back_small);
            _smallimages.Add(Properties.Resources.RadioGreen_small);
            _smallimages.Add(Properties.Resources.RadioBlue_small);
            _smallimages.Add(Properties.Resources.RadioRed_small);
            _smallimages.Add(Properties.Resources.RadioOrange_small);
            _smallDisabeldImage = Properties.Resources.RadioDisabled_small;

            _font = new Font(_font.FontFamily, 8.0f, FontStyle.Regular);

            _images = _smallimages;
            _disabledImage = _smallDisabeldImage;
        }

        ItemTypes _itemType = ItemTypes.RadioButton;
        /// <summary>
        /// 현재 cell의 ItemType을 나타냅니다.
        /// </summary>
        public ItemTypes ItemType
        {
            get { return _itemType; }
        }

        Color _radioDrawColor = Color.Red;
        /// <summary>
        /// IsImageRadio가 false일 때 유효하다.
        /// 그림으로 RadioButton을 그린다.
        /// </summary>
        public Color RadioDrawColor
        {
            get
            {
                return _radioDrawColor;
            }
            set
            {
                _radioDrawColor = value;
            }
        }

        bool _isImageRadio = true;
        public bool IsImageRadio
        {
            get { return _isImageRadio; }
            set { _isImageRadio = value; }
        }

        public EasyGridRadioButtonCollection Items
        {
            get{
                return _items;
            }
        }

        bool _enabled = true;
        public bool Enabled{
            get { return _enabled; }
            set {
                _enabled = value;
                RePaint();
            }
        }

        

        Font _font = new Font( SystemFonts.DefaultFont, FontStyle.Regular);
        public Font Font
        {
            get { return _font; }
            set { 
                _font = value;
                RePaint();
            }
        }

        Brush _fontColor = Brushes.Black;
        public Brush FontColor
        {
            get { return _fontColor; }
            set { 
                _fontColor = value;
                RePaint();
            }
        }

        EasyGridRadioBoxOrientation _orientation = EasyGridRadioBoxOrientation.VerticalFirstInHeight;
        public EasyGridRadioBoxOrientation Orientation
        {
            get { return _orientation; }
            set { 
                _orientation = value;
                RePaint();
            }
        }

        RadioButtonColors _radioButtonColor =  RadioButtonColors.Red;
        public RadioButtonColors RadioButtonColor{
            get{ return _radioButtonColor;}
            set{ 
                _radioButtonColor = value;
                RePaint();
            }
        }

       // IntPtr _hdc = IntPtr.Zero;
       // Rectangle _bounds;
        
        protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            
            try
            {
                PaintCell(g, cellBounds);
            }
            catch (InvalidOperationException) { }
            //_hdc = g.GetHdc(); //다음에쓸 Repaint를 위해 저장한다.
            //_bounds = cellBounds;
            
            base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            
        }

        
        protected virtual void PaintCell(Graphics g, Rectangle cellBounds)
        {
            //Graphics g = Graphics.FromHdcInternal(_hdc);
           // if (_items.Count == 0) return;
            Rectangle clearRect = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
            if (_parent == null) g.FillRectangle(Brushes.Gray, cellBounds);
            else g.FillRectangle(new SolidBrush(_parent.GridColor), cellBounds);
            if (this.Selected)
            {
                if (_isImageRadio)
                {
                    if (_enabled) g.FillRectangle(Brushes.White, clearRect); //background
                    else g.FillRectangle(Brushes.WhiteSmoke, clearRect);
                }
                else
                {
                    g.FillRectangle(SystemBrushes.Highlight, clearRect); //background
                }
            }
            else
            {
                if (_enabled) g.FillRectangle(Brushes.White, clearRect); //background
                else g.FillRectangle(Brushes.WhiteSmoke, clearRect);
            }
            //g.DrawString(_items.RefCount.ToString(), _font, Brushes.Blue, 10, cellBounds.Y);
            //g.FillRectangle(Brushes.Red, new Rectangle(cellBounds.X + 0, cellBounds.Y + 0, 10, 10));
            Brush fontColor;
            if (this.Selected) fontColor = new SolidBrush(SystemColors.HighlightText);
            else fontColor = _fontColor;
            
            //g.FillRectangle(Brushes.Blue, new Rectangle(cellBounds.X + 0, cellBounds.Y + 0, 10, 10));
            
            if (_orientation == EasyGridRadioBoxOrientation.Horizontal)
            {
                //Point p = new Point(cellBounds.Left, cellBounds.Top+3);

                SizeF textSize = TextSize(_items[0].Text, g);
                int textOffset = (textSize.Height < _images[0].Height) ? (int)((_images[0].Height - textSize.Height)) / 2 : 0;
                int imgOffset = (textSize.Height >= _images[0].Height) ? (int)(textSize.Height - (_images[0].Height)) / 2 : 0;
                
                int textY = cellBounds.Y + (int)((cellBounds.Height - textSize.Height) / 2.0);
                
                int imgY = cellBounds.Y + (int)((cellBounds.Height - _images[0].Height) / 2.0);
                int top = (textY < imgY) ? textY : imgY;
                //int bottom = (textSize.Height < _images[0].Height) ? _images[0].Height : (int) textSize.Height;
                int x = cellBounds.Left+3;
                //Point p = new Point(cellBounds.Left, textY);
                //float height = (textSize.Height < _images[0].Height) ? _images[0].Height : (textSize.Height);
                

                for (int i = 0; i < _items.Count; i++)
                {
                   
                    DrawRadioButton(g, x, top, _items[i], textOffset, imgOffset);
                    /*
                      Rectangle bound = new Rectangle();
                    bound.Location = new Point(x, top);
                    
                    textSize = TextSize(_items[i].Text, g);
                   
                    if (_isImageRadio)
                    {
                        if (_enabled == false) DrawImage(g, _disabledImage, new Point(x, imgY)); //disabled
                        else if (_items[i].Checked) DrawImage(g, _images[(int)_radioButtonColor], new Point(x, imgY)); //checked
                        else DrawImage(g, _images[0], new Point(x, imgY));//unchecked
                    }
                    else
                    {
                        if (_enabled == false) DrawRadio(g, Color.Gray, (int)textSize.Height, new Point(x, imgY)); //
                        else if (_items[i].Checked) DrawRadio(g, _radioDrawColor, (int)textSize.Height, new Point(x, imgY));
                        else DrawRadio(g, Color.White, (int)textSize.Height, new Point(x, imgY));
                    }
                    x += _images[0].Width + 2;
                    g.DrawString(_items[i].Text, _font, fontColor, new Point(x,textY));
                    x += (int)textSize.Width + 5;
                     bound.Height = (int)height;
                    _items[i].Bounds = bound;
                     */


                    x += _items[i].Width + 3;
                    
                }
            }
            else if (_orientation == EasyGridRadioBoxOrientation.Vertical)
            {
                SizeF textSize = TextSize(_items[0].Text, g);
                int textOffset = (textSize.Height < _images[0].Height) ? (int)((_images[0].Height - textSize.Height)) / 2 : 0;
                int imgOffset = (textSize.Height >= _images[0].Height) ? (int)(textSize.Height - (_images[0].Height)) / 2 : 0;
                float height = (textSize.Height < _images[0].Height) ? _images[0].Height : (textSize.Height);
                int top = cellBounds.Y + (int)((cellBounds.Height - height * _items.Count) / 2.0);
                int bottom = top + (int)(height * _items.Count);

                int y = top;
                int x = cellBounds.X + 3;
                int imgWidth = _images[0].Width + 2;

                for (int i = 0; i < _items.Count; i++)
                {
                    y = top + (int)(height * i);
                    DrawRadioButton(g, x, y, _items[i], textOffset, imgOffset);
                   
                    /*
                    Rectangle bound = new Rectangle();
                    textSize = TextSize(_items[i].Text, g);
                    y = top + (int)(height * i);

                    bound.Location = new Point(x, y);
                    if (_isImageRadio)
                    {
                        if (_enabled == false) DrawImage(g, _disabledImage, new Point(x, y + imgOffset)); //checked
                        else if (_items[i].Checked) DrawImage(g, _images[(int)_radioButtonColor], new Point(x, y + imgOffset)); //checked
                        else DrawImage(g, _images[0], new Point(x, y + imgOffset));//unchecked
                    }
                    else
                    {
                        if (_enabled == false) DrawRadio(g, Color.Gray, (int)textSize.Height, new Point(x, y + imgOffset)); //
                        else if (_items[i].Checked) DrawRadio(g, _radioDrawColor, (int)textSize.Height, new Point(x, y + imgOffset));
                        else DrawRadio(g, Color.White, (int)textSize.Height, new Point(x, y + imgOffset));
                    }


                    g.DrawString(_items[i].Text, _font, fontColor, new Point(x + imgWidth, y + textOffset));

                    bound.Width = (int)(imgWidth + textSize.Width + 1); //float으로 인해 잘리는 것을 방지하기 위해 +1
                    bound.Height = (int)height;
                    _items[i].Bounds = bound;
                     */
                }
            }
            else
            {
                SizeF textSize = TextSize(_items[0].Text, g);
                int textOffset = (textSize.Height < _images[0].Height) ? (int)((_images[0].Height - textSize.Height)) / 2 : 0;
                int imgOffset = (textSize.Height >= _images[0].Height) ? (int)(textSize.Height - (_images[0].Height) )/ 2 : 0;
                float height = (textSize.Height < _images[0].Height) ? _images[0].Height : (textSize.Height);
                
                int maxCount = (_items.Count < 2) ? _items.Count : 2;
                int top = cellBounds.Y + (int)((cellBounds.Height - height * maxCount) / 2.0);
                int bottom = top + (int)(height * maxCount);

                int y = top;
                int x = cellBounds.X + 3;
                int imgWidth = _images[0].Width + 2;

                for (int i = 0; i < _items.Count; i++)
                {
                    y = top + (int)(height * (i % 2));
                    DrawRadioButton(g, x, y, _items[i], textOffset, imgOffset);
                   
                    /*
                    Rectangle bound = new Rectangle();
                    textSize = TextSize(_items[i].Text, g);
                    y = top + (int)(height * (i%2));

                    bound.Location = new Point(x, y);
                    if (_isImageRadio)
                    {
                        if (_enabled == false) DrawImage(g, _disabledImage, new Point(x, y + imgOffset)); //checked
                        else if (_items[i].Checked) DrawImage(g, _images[(int)_radioButtonColor], new Point(x, y + imgOffset)); //checked
                        else DrawImage(g, _images[0], new Point(x, y + imgOffset));//unchecked
                    }
                    else
                    {
                        if (_enabled == false) DrawRadio(g, Color.Gray, (int)textSize.Height, new Point(x, y + imgOffset)); //
                        else if (_items[i].Checked) DrawRadio(g, _radioDrawColor, (int)textSize.Height, new Point(x, y + imgOffset));
                        else DrawRadio(g, Color.White, (int)textSize.Height, new Point(x, y + imgOffset));
                    }

                    g.DrawString(_items[i].Text, _font, fontColor, new Point(x + imgWidth, y + textOffset));

                    bound.Width = (int)(imgWidth + textSize.Width + 1); //float으로 인해 잘리는 것을 방지하기 위해 +1
                    bound.Height = (int)height;
                    _items[i].Bounds = bound;
                    if (i % 2 == 1) x += bound.Width+5;
                     */
                    if (i % 2 == 1) x += _items[i].Width + 5;
                }
            }

            
        }

        void DrawImage(Graphics dstGraphics, Bitmap srcBitmap, Point dstPoint)
        {
            //Bitmap bmp = new Bitmap(srcBitmap.Width, srcBitmap.Height);
            //Graphics g = Graphics.FromImage(bmp);
            
            ImageAttributes att = new ImageAttributes();
            att.SetColorKey(Color.Transparent, Color.Transparent);   //색상 영역.
            Rectangle rect = new Rectangle(dstPoint, srcBitmap.Size);
            dstGraphics.DrawImage(srcBitmap, rect, 0, 0, srcBitmap.Width, srcBitmap.Height, GraphicsUnit.Pixel, att);
                                 
            //dstGraphics.DrawImage(bmp, new Point(0, 0)); 

        }
        void DrawRadioButton(Graphics g, int x, int y, RadioButton radio, int textOffset=0, int imageOffset=0)
        {
            String text = radio.Text;
            SizeF textSize = TextSize(text, g);
            Brush fontColor;

            if (this.Selected && _isImageRadio == false) fontColor = new SolidBrush(SystemColors.HighlightText);
            else if (this.Enabled == false) fontColor = Brushes.DarkGray;
            else fontColor = _fontColor;
            int imgWidth = 0;

            if (_isImageRadio)
            {
                if (_enabled == false) DrawImage(g, _disabledImage, new Point(x, y+imageOffset)); //checked
                else if (radio.Checked) DrawImage(g, _images[(int)_radioButtonColor], new Point(x, y + imageOffset)); //checked
                else DrawImage(g, _images[0], new Point(x, y + imageOffset));//unchecked
                imgWidth = _images[0].Width + 3;
            }
            else
            {
                if (_enabled == false) DrawRadio(g, Color.Gray, (int)textSize.Height, new Point(x, y + imageOffset)); //
                else if (radio.Checked) DrawRadio(g, _radioDrawColor, (int)textSize.Height, new Point(x, y + imageOffset));
                else DrawRadio(g, Color.White, (int)textSize.Height, new Point(x, y + imageOffset));
                imgWidth = (int)textSize.Height + 3;
            }

            g.DrawString(text, _font, fontColor, new Point(x + imgWidth, y+textOffset));
            radio.Bounds = new Rectangle(x,y,imgWidth + (int)textSize.Width, (int)textSize.Height);
        }
        void DrawRadio(Graphics g, Color color, int size, Point dstPoint)
        {
            Rectangle rect = new Rectangle(dstPoint.X, dstPoint.Y, size, size);
            g.FillPie(new SolidBrush(Color.FromArgb(200,200,200)), rect, 0, 360);
            
            rect = new Rectangle(dstPoint.X+2, dstPoint.Y+2, size-3, size-3);
            g.FillPie(new SolidBrush(Color.FromArgb(255,255,255)), rect, 0, 360);
            rect = new Rectangle(dstPoint.X+size/3, dstPoint.Y+size/3, size/2, size/2);
            g.FillPie(new SolidBrush(color), rect, 0, 360);
        }

        public int ClickOnCell(int x, int y)
        {
            int index = -1;
            if (Enabled)
            {
                
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_orientation == EasyGridRadioBoxOrientation.Horizontal)
                    {
                        if (_items[i].Left<= x && (_items[i].Left + _items[i].Width) >= x)
                        {
                            index = i;
                            break;
                        }
                    }
                    else
                    {
                        if (_items[i].Bounds.Contains(x, y))
                        {
                            index = i;
                            break;
                        }
                    }
                    
                }
                if (index >= 0)
                {
                    for (int i = 0; i < _items.Count; i++)
                    {

                        if (i == index)
                        {
                            _items[i].Checked = true;
                            this.Value = i;
                        }
                        else _items[i].Checked = false;
                        
                    }
                    RePaint();
                }
            }
            return index;
        }

        private SizeF TextSize(String text, Graphics g)
        {
            // Set the return value to the normal node bounds.
            
            if (text != null)
            {
                return g.MeasureString(text, _font);
            }
            return new SizeF(0,0);

        }

        internal void RePaint()
        {
            
            //Graphics g = Graphics.FromHdc(_hdc);
            //this.Value = this.Value;
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);  
            //PaintCell(g, _bounds);
            
        }

        public new Type ValueType
        {
            get
            {
                return typeof(int);
            }
        }

        int _value = -1;
        public new int Value
        {
            get
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Checked) return i;
                }
                return -1;
            }
            set
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (i == value) _items[i].Checked = true;
                    else _items[i].Checked = false;
                }
                base.Value = value;
            }
        }
    }

    public class EasyGridRadioButtonCollection:ICollection<RadioButton>,IEnumerable<RadioButton>{
        EasyGridRadioButtonCell _parent;
        
        List<RadioButton> _radioButtons;
        public static int _count = 0;
        public int RefCount=0;

        internal EasyGridRadioButtonCollection(EasyGridRadioButtonCell parent){
            _parent = parent;
            _radioButtons = new List<RadioButton>();
            _count++;
            RefCount = _count;
        }

        public RadioButton this[int index]{
            get{
                if(_radioButtons.Count>index) return _radioButtons[index];
                else return null;
            }
        }

        public void CheckItem(int index)
        {
            for (int i = 0; i < _radioButtons.Count; i++)
            {
                if (i == index) _radioButtons[i].Checked = true;
                else _radioButtons[i].Checked = false;
            }
        }

        public void Add(String item)
        {
            RadioButton radio = new RadioButton();
            radio.Text = item;

            if (_radioButtons.Count == 0)
            {
                if(_parent!=null) _parent.Value = 0;
                radio.Checked = true;
            }
            else
            {
                radio.Checked = false;
            }
            _radioButtons.Add(radio);
            if (_parent != null) _parent.RePaint();
            
        }
        public void Add(ICollection<String> items)
        {
            foreach (String item in items)
            {
                Add(item);
            }
        }

        public void Add(ICollection<RadioButton> items)
        {
            foreach (RadioButton item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _radioButtons.Clear();
            //if (_parent != null) _parent.RePaint();
        }

        public int Count
        {
            get
            {
                return _radioButtons.Count;
            }
        }

        public void Add(RadioButton item)
        {
            if (_radioButtons.Count == 0)
            {
                if (_parent != null) _parent.Value = 0;
                item.Checked = true;
            }
            else
            {
                if (item.Checked == true)
                {
                    for (int i = 0; i< _radioButtons.Count; i++)
                    {
                        _radioButtons[i].Checked = false;
                    }
                    if (_parent != null) _parent.Value = _radioButtons.Count;
                }
            }
            RadioButton radio = new RadioButton();
            radio.Text = item.Text;
            radio.Checked = item.Checked;
            
            _radioButtons.Add(radio);
           // if (_parent != null) _parent.RePaint();
        }

        public bool Contains(RadioButton item)
        {
            return _radioButtons.Contains(item);
        }
        
        public bool Contains(String item)
        {
            for (int i = 0; i < _radioButtons.Count; i++)
            {
                if (_radioButtons[i].Text.Equals(item)) return true;
            }
            return false;
        }

        public int IndexOf(String item)
        {
            for (int i = 0; i < _radioButtons.Count; i++)
            {
                if (_radioButtons[i].Text.Equals(item)) return i;
            }
            return -1;
        }

        public void CopyTo(RadioButton[] array, int arrayIndex)
        {
            _radioButtons.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(RadioButton item)
        {
            bool isRemoved =  _radioButtons.Remove(item);
            if (_parent != null) _parent.RePaint();
            return isRemoved;
        }

        public void Remove(int index)
        {
            _radioButtons.RemoveAt(index);
            if (_parent != null) _parent.RePaint();
        }

        public IEnumerator<RadioButton> GetEnumerator()
        {
            return _radioButtons.GetEnumerator();
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            
            return _radioButtons.GetEnumerator();
        }
    }

}
