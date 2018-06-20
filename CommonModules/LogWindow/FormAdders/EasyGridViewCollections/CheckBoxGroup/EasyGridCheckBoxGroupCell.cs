using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCheckBoxGroupCell:DataGridViewCell, IEasyGridCell
    {
        EasyGridCheckBoxGroupCollection _items;
        List<Bitmap> _images;
        List<Bitmap> _bigimages;
        List<Bitmap> _smallimages;
        Bitmap _disabledImage;
        Bitmap _bigDisabeldImage;
        Bitmap _smallDisabeldImage;
        DataGridView _parent = null;
        public EasyGridCheckBoxGroupCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            _items = new EasyGridCheckBoxGroupCollection(this);
            _bigimages = new List<Bitmap>();
            _smallimages = new List<Bitmap>();

            _bigimages.Add(Properties.Resources.check_normal);
            _bigimages.Add(Properties.Resources.check_red);
            _bigimages.Add(Properties.Resources.check_blue);
            _bigDisabeldImage = Properties.Resources.check_normal_press;

            _smallimages.Add(Properties.Resources.check_normal_small);
            _smallimages.Add(Properties.Resources.check_red_small);
            _smallimages.Add(Properties.Resources.check_blue_small);
            _smallDisabeldImage = Properties.Resources.check_normal_disabled_small;

            _font = new Font(_font.FontFamily, 8.0f, FontStyle.Regular);

            _images = _smallimages;
            _disabledImage = _smallDisabeldImage;
        }

        ItemTypes _itemType = ItemTypes.CheckBoxGroup;
        /// <summary>
        /// 현재 cell의 ItemType을 나타냅니다.
        /// </summary>
        public ItemTypes ItemType
        {
            get { return _itemType; }
        }

        Color _checkDrawColor = Color.FromArgb(100, 100, 200);
        /// <summary>
        /// IsImageRadio가 false일 때 유효하다.
        /// 그림으로 CheckBox을 그린다.
        /// </summary>
        public Color RadioDrawColor
        {
            get
            {
                return _checkDrawColor;
            }
            set
            {
                _checkDrawColor = value;
            }
        }

        bool _isImageCheck = false;
        public bool IsImageCheck
        {
            get { return _isImageCheck; }
            set { _isImageCheck = value; }
        }

        public EasyGridCheckBoxGroupCollection Items
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

        CheckBoxColors _checkBoxColor =  CheckBoxColors.Red;
        public CheckBoxColors CheckBoxColor{
            get{ return _checkBoxColor;}
            set{ 
                _checkBoxColor = value;
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
                if (_isImageCheck)
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
                   
                    DrawCheckBox(g, x, top, _items[i], textOffset, imgOffset);
                    

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
                int x = cellBounds.X+ 3;
                int imgWidth = _images[0].Width + 2;

                for (int i = 0; i < _items.Count; i++)
                {
                    y = top + (int)(height * i);
                    DrawCheckBox(g, x, y, _items[i], textOffset, imgOffset);
                   
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
                    DrawCheckBox(g, x, y, _items[i], textOffset, imgOffset);
                   
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
        void DrawCheckBox(Graphics g, int x, int y, CheckBox radio, int textOffset=0, int imageOffset=0)
        {
            String text = radio.Text;
            SizeF textSize = TextSize(text, g);
            Brush fontColor;

            if (this.Selected && _isImageCheck == false) fontColor = new SolidBrush(SystemColors.HighlightText);
            else if (this.Enabled == false) fontColor = Brushes.DarkGray;
            else fontColor = _fontColor;
            int imgWidth = 0;

            if (_isImageCheck)
            {
                if (_enabled == false) DrawImage(g, _disabledImage, new Point(x, y+imageOffset)); //checked
                else if (radio.Checked) DrawImage(g, _images[(int)_checkBoxColor], new Point(x, y + imageOffset)); //checked
                else DrawImage(g, _images[0], new Point(x, y + imageOffset));//unchecked
                imgWidth = _images[0].Width + 3;
            }
            else
            {
                if (_enabled == false) DrawCheck(g, Color.Gray, (int)textSize.Height, new Point(x, y + imageOffset)); //
                else if (radio.Checked) DrawCheck(g, _checkDrawColor, (int)textSize.Height, new Point(x, y + imageOffset));
                else DrawCheck(g, Color.White, (int)textSize.Height, new Point(x, y + imageOffset));
                imgWidth = (int)textSize.Height + 3;
            }

            g.DrawString(text, _font, fontColor, new Point(x + imgWidth, y+textOffset));
            radio.Bounds = new Rectangle(x,y,imgWidth + (int)textSize.Width, (int)textSize.Height);
        }
        void DrawCheck(Graphics g, Color color, int size, Point dstPoint)
        {
            Rectangle rect = new Rectangle(dstPoint.X, dstPoint.Y, size, size);
            g.FillRectangle(new SolidBrush(Color.FromArgb(200,200,200)), rect);
            
            rect = new Rectangle(dstPoint.X+2, dstPoint.Y+2, size-3, size-3);
            g.FillRectangle(new SolidBrush(Color.FromArgb(255,255,255)), rect);
            rect = new Rectangle(dstPoint.X+size/3, dstPoint.Y+size/3, size/2, size/2);
            
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            path.StartFigure();
            
            path.AddLine(rect.X,rect.Y, rect.X+rect.Width/2, rect.Y+rect.Height);
            path.AddLine( rect.X+rect.Width/2, rect.Y+rect.Height, rect.X+ rect.Width, rect.Y-2);
            path.AddLine( rect.X+ rect.Width, rect.Y-2,rect.X+rect.Width/2, rect.Y+rect.Height-4);
            path.CloseFigure();
            g.FillPath(new SolidBrush(color), path);
            //g.FillRectangle(new SolidBrush(color), rect);
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
                        if (_items[i].Left <= x && (_items[i].Left + _items[i].Width) >= x)
                        {
                            _items[i].Checked = !(_items[i].Checked);
                            index = i;
                            break;
                        }
                    }
                    else
                    {
                        if (_items[i].Bounds.Contains(x, y))
                        {
                            _items[i].Checked = !(_items[i].Checked);
                            index = i;
                            break;
                        }
                    }
                    
                }
                
                RePaint();

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
        public new ICollection<int> Value
        {
            get
            {
                List<int> selected = new List<int>();
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Checked) selected.Add(i);
                }
                return selected;
            }
            set
            {
                if (value != null)
                {
                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (value.Contains(i)) _items[i].Checked = true;
                        else _items[i].Checked = false;
                    }
                    base.Value = 1;
                }
                else
                {
                    for (int i = 0; i < _items.Count; i++)
                    {
                        _items[i].Checked = false;
                    }
                    base.Value = 0;
                }
                
            }
        }
    }

    public class EasyGridCheckBoxGroupCollection:ICollection<CheckBox>,IEnumerable<CheckBox>{
        EasyGridCheckBoxGroupCell _parent;

        List<CheckBox> _checkBoxs;
        public static int _count = 0;
        public int RefCount=0;

        internal EasyGridCheckBoxGroupCollection(EasyGridCheckBoxGroupCell parent)
        {
            _parent = parent;
            _checkBoxs = new List<CheckBox>();
            _count++;
            RefCount = _count;
        }

        public CheckBox this[int index]{
            get{
                if(_checkBoxs.Count>index) return _checkBoxs[index];
                else return null;
            }
        }

        public void CheckItem(ICollection<int> selected)
        {
            if (selected != null)
            {
                for(int i=0; i<_checkBoxs.Count; i++)
                {
                    if(selected.Contains(i)) _checkBoxs[i].Checked = true;
                    else _checkBoxs[i].Checked = false;
                }
            }
            else //null이면 모두 unchecked..
            {
                for (int i = 0; i < _checkBoxs.Count; i++)
                {
                    _checkBoxs[i].Checked = false;
                }
            }
        }

        public void Add(String item)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Text = item;
            _checkBoxs.Add(checkBox);
            if (_parent != null) _parent.RePaint();
            
        }
        public void Add(ICollection<String> items)
        {
            foreach (String item in items)
            {
                Add(item);
            }
        }

        public void Add(ICollection<CheckBox> items)
        {
            foreach (CheckBox item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _checkBoxs.Clear();
            //if (_parent != null) _parent.RePaint();
        }

        public int Count
        {
            get
            {
                return _checkBoxs.Count;
            }
        }

        public void Add(CheckBox item)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Text = item.Text;
            checkBox.Checked = item.Checked;
            
            _checkBoxs.Add(checkBox);
           // if (_parent != null) _parent.RePaint();
        }

        public bool Contains(CheckBox item)
        {
            return _checkBoxs.Contains(item);
        }

        public void CopyTo(CheckBox[] array, int arrayIndex)
        {
            _checkBoxs.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(CheckBox item)
        {
            bool isRemoved =  _checkBoxs.Remove(item);
            if (_parent != null) _parent.RePaint();
            return isRemoved;
        }

        public void Remove(int index)
        {
            _checkBoxs.RemoveAt(index);
            if (_parent != null) _parent.RePaint();
        }

        public IEnumerator<CheckBox> GetEnumerator()
        {
            return _checkBoxs.GetEnumerator();
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            
            return _checkBoxs.GetEnumerator();
        }
    }

}
