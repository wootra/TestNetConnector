using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WootraComs.wTreeElements
{
    public class wTreeNodeItem
    {
        internal event wTreeNodeItemChanged E_ItemChanged;
        //internal event wTreeNodeItemMouseEvent E_ItemSelected;

         public wTreeNode OwnerNode
         {
             get { return _ownerNode; }
         }

         void Init()
         {
             Visible = true;
             TextColor = Color.Empty;
             SelectedTextColor = Color.Empty;
             HoveredTextColor = Color.Empty;
         }
         public wTreeMouseEventsHandler MouseEventsHandler { get { return OwnerNode.OwnerTree.wMouseEventsHandler; } }

         public wTreeScroll ScrollHandler { get { return OwnerNode.OwnerTree.wScrollHandler; } }

         public wTreeSelections SelectionHandler { get { return OwnerNode.OwnerTree.wSelectionHandler; } }

         public DrawHandler DrawHandler { get { return OwnerNode.OwnerTree.wDrawHandler; } }

         public EditorHandlerClass EditorHandler { get { return OwnerNode.OwnerTree.wEditorHandler; } }

        Image _image = null;
        public Image Image
        {
            get { return _image; }
            internal set { _image = value; }
        }
        /// <summary>
        /// Image 타입일때 editor에 사용할 에디터..
        /// </summary>
       public ImageEditorTypes ImageEditorType { get; set; }
        /// <summary>
        /// Editor가 있으면 활성화시킬 Action..
        /// </summary>
        public EditorActivateActions EditorActivateAction { get; set; }

        /// <summary>
        /// Text나 combobox일 때 나타나는 글자의 색깔..
        /// </summary>
        public Color TextColor
        {
            get;
            set;
        }

        public Color SelectedTextColor
        {
            get;
            set;
        }

        public Color HoveredTextColor
        {
            get;
            set;
        }

        /// <summary>
        /// 사용자가 원하는 object를 할당한다.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 사용자가 원하는 이름을 지정하여 사용한다.
        /// </summary>
        public object Name { get; set; }
        internal int _imageWidth;
        internal int _imageHeight;

        internal wTreeNodeItem(wTreeNode ownerNode, Image image, int wid=-1, int height=-1)
        {
            _type = wTreeNodeItemTypes.Image;
            _image = image;
            _imageWidth = wid;
            _imageHeight = height;
            _ownerNode = ownerNode;

            Init();
        }

        /// <summary>
        /// 특정 nodeItem을 만든다.
        /// </summary>
        /// <param name="ownerNode"></param>
        /// <param name="nodeItemType"></param>
        internal wTreeNodeItem(wTreeNode ownerNode,  wTreeNodeItemTypes nodeItemType)
        {
            _type = nodeItemType;
            _ownerNode = ownerNode;
            Init();
        }

        

        string _text;
        public String Text { 
            get { return _text; }
            internal set { _text = value; }
        }
        public TextEditorTypes TextEditorType { get; set; }
        Font _tempFont;
        internal wTreeNodeItem(wTreeNode ownerNode, String text)
        {
            _type = wTreeNodeItemTypes.Text;
            _text = text;
            
            _ownerNode = ownerNode;
            Init();
        }

        ICollection<String> _texts;

        /// <summary>
        /// TextArray타입일 때, 그 선택가능한 text들..
        /// </summary>
        public ICollection<String> TextArray
        {
            get { return _texts; }
            internal set {
                _texts = value; 
            }
        }

        
        int _selectedIndex = -1;
        /// <summary>
        /// TextArray타입일 때, 선택된 Index..
        /// </summary>
        public int TextArraySelectedIndex
        {
            get { return _selectedIndex; }
            set {
                if (value < 0 || value >= _texts.Count) _selectedIndex = -1;
                else _selectedIndex = value;
            }
        }
        public TextArrayEditorTypes TextArrayEditorType { get; set; }
       
        internal wTreeNodeItem(wTreeNode ownerNode, ICollection<String> texts, int initIndex=0)
        {
            _type = wTreeNodeItemTypes.TextArray;
            _texts = new List<String>();
            foreach (string str in texts)
            {
                _texts.Add(str);
            }
            _selectedIndex = initIndex;
            _ownerNode = ownerNode;
            Init();
        }

         bool? _isChecked;
         CheckboxActiveActions _checkBoxActiveAction;

         public CheckboxActiveActions CheckBoxActiveAction { get { return _checkBoxActiveAction; } set { _checkBoxActiveAction = value; } }

        /// <summary>
        /// 체크박스.
        /// </summary>
        /// <param name="ownerNode">부모노드</param>
        /// <param name="check"></param>
        /// <param name="checkboxActiveAction"></param>
        internal wTreeNodeItem(wTreeNode ownerNode, bool? check, CheckboxActiveActions checkboxActiveAction)
        {
            _type = wTreeNodeItemTypes.CheckBox;
            _isChecked = check;
            _ownerNode = ownerNode;
            _checkBoxActiveAction = checkboxActiveAction;
            Init();
        }

        

        internal wTreeNodeItem(wTreeNode ownerNode, Control control)
        {
            _type = wTreeNodeItemTypes.Control;
            _control = control;
            _control.MouseDown += _control_MouseDown;
            _ownerNode = ownerNode;
            Init();
            
        }

        void _control_MouseDown(object sender, MouseEventArgs e)
        {
            SelectionHandler.SelectedNode = _ownerNode;
            
           
           // SelectionHandler.SelectedItem = this;
            DrawHandler.ReDrawTree(false);
            SelectionHandler.SetNodeSelected(_ownerNode, this);
            //if (E_ItemSelected != null) E_ItemSelected(_ownerNode, this, e);
        }

        
        WhiteSpaceTypes _whiteSpaceType;
        internal wTreeNodeItem(wTreeNode ownerNode, WhiteSpaceTypes type)
        {
            _type = wTreeNodeItemTypes.WhiteSpace;
            _whiteSpaceType = type;
            _ownerNode = ownerNode;
            Init();
        }

        wTreeEditor _customEditor;
        /// <summary>
        /// Text나 Image의 경우 사용할 수 있는 Default Editor이외에 CustomType을 이용할 수 있다.
        /// 그런 경우 CustomEditor를 사용한다.
        /// </summary>
        public wTreeEditor CustomEditor
        {
            get { return _customEditor; }
            set
            {
                _customEditor = value;
                if (value != null)
                {
                    _customEditor.EditorControl.Width = ItemArea.Width;
                    _customEditor.EditorControl.Height = ItemArea.Height;
                    _customEditor.EditorControl.Location = new Point(ItemArea.X, ItemArea.Y);

                }
            }
        }

        public object Value
        {
            get
            {
                if (_type == wTreeNodeItemTypes.CheckBox)
                {

                    return _isChecked;
                }
                else if (_type == wTreeNodeItemTypes.Image)
                {
                    return _image;
                }
                else if (_type == wTreeNodeItemTypes.Text)
                {
                    return _text;
                }
                else if (_type == wTreeNodeItemTypes.TextArray)
                {
                    return _selectedIndex;
                    //if (_selectedIndex < 0 || _selectedIndex>=_text.Length) return null;
                    //else return _texts[_selectedIndex];
                }
                else if (_type == wTreeNodeItemTypes.Control)
                {
                    return _control;
                }
                else//whiteSpace
                {
                    return _whiteSpaceType;
                }
            }
            set
            {
                bool isChanged = false;
                if (_type == wTreeNodeItemTypes.CheckBox)
                {
                    if (value!=null && (value is bool?) == false) throw new Exception("value must be bool!");
                    if (_isChecked != (bool?)value) isChanged = true;
                    _isChecked = (bool?)value;
                }
                else if (_type == wTreeNodeItemTypes.Image)
                {
                    if (value == null)
                    {
                        if (_image == null) isChanged = false;
                        else isChanged = true;
                    }
                    else if ((value is Image) == false) throw new Exception("value must be image!");
                    else if (value.Equals(_image) == false) isChanged = true;
                    _image = value as Image;
                }
                else if (_type == wTreeNodeItemTypes.Text)
                {
                    if ((value is string) == false) throw new Exception("value must be string!");
                    if (_text.Equals(value) == false) isChanged = true;
                    _text = value as String;
                }
                else if (_type == wTreeNodeItemTypes.TextArray)
                {
                    if (value is ICollection<String>) _texts = value as ICollection<String>;
                    else if (value is int) TextArraySelectedIndex = (int)value;
                    else throw new Exception("Value of TextArray Type must be int or String[]");
                }
                else if (_type == wTreeNodeItemTypes.Control)
                {
                    if ((value is Control) == false) throw new Exception("value must be Control!");
                    if (_control.Equals(value) == false) isChanged = true;
                    _control = value as Control;
                }
                else//whiteSpace
                {
                    if ((value is WhiteSpaceTypes) == false) throw new Exception("value must be WhiteSpaceTypes!");
                    if (_whiteSpaceType != (WhiteSpaceTypes)value) isChanged = true;
                    _whiteSpaceType = (WhiteSpaceTypes)value;
                }
                
                if (isChanged)//바뀌었을 때에만 이벤트 생성..
                {
                    
                    if (E_ItemChanged != null) E_ItemChanged(this);
                }
                
            }
        }
        /// <summary>
        /// E_ItemChanged 이벤트를 발생시키지 않고 내부의 값만 바꾼다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>값이 기존값과 다른 값으로 셋팅되었으면 true..</returns>
        internal bool SetValue(object value){
            bool isChanged=false;
            if (_type == wTreeNodeItemTypes.CheckBox)
            {
                if (value != null && (value is bool?) == false) throw new Exception("value must be bool!");
                if (_isChecked != (bool?)value) isChanged = true;
                _isChecked = (bool?)value;
            }
            else if (_type == wTreeNodeItemTypes.Image)
            {
                if (value == null)
                {
                    if (_image == null) isChanged = false;
                    else isChanged = true;
                }
                else if ((value is Image) == false) throw new Exception("value must be image!");
                else if (value.Equals(_image) == false) isChanged = true;
                _image = value as Image;
            }
            else if (_type == wTreeNodeItemTypes.Text)
            {
                if ((value is string) == false) throw new Exception("value must be string!");
                if (_text.Equals(value) == false) isChanged = true;
                _text = value as String;
            }
            else if (_type == wTreeNodeItemTypes.TextArray)
            {
                if (value is String[]) _texts = value as String[];
                else if (value is int) TextArraySelectedIndex = (int)value;
                else throw new Exception("Value of TextArray Type must be int or String[]");
            }
            else if (_type == wTreeNodeItemTypes.Control)
            {
                if ((value is Control) == false) throw new Exception("value must be Control!");
                if (_control.Equals(value) == false) isChanged = true;
                _control = value as Control;
            }
            else//whiteSpace
            {
                if ((value is WhiteSpaceTypes) == false) throw new Exception("value must be WhiteSpaceTypes!");
                if (_whiteSpaceType != (WhiteSpaceTypes)value) isChanged = true;
                _whiteSpaceType = (WhiteSpaceTypes)value;
            }
            return isChanged;
        }
        #region changeItem
        public void ChangeItem(Image image)
        {

            if (_type != wTreeNodeItemTypes.Image || (_image == null) || _image.Equals(image) == false)
            {
                _type = wTreeNodeItemTypes.Image;
                _image = image;
                if (E_ItemChanged != null) E_ItemChanged(this);
            }

        }
        public void ChangeItem(string text)
        {
            if (_type != wTreeNodeItemTypes.Text || _text.Equals(text)==false)
            {
                _type = wTreeNodeItemTypes.Text;
                _text = text;
                if (E_ItemChanged != null) E_ItemChanged(this);
            }

        }

 
        public void ChangeItem(bool? isChecked)
        {
            if (_type != wTreeNodeItemTypes.CheckBox || _isChecked != isChecked)
            {
                _type = wTreeNodeItemTypes.CheckBox;
                _isChecked = isChecked;
                if (E_ItemChanged != null) E_ItemChanged(this);
            }
        }

        public void ChangeItem(Control control)
        {
            if (_type != wTreeNodeItemTypes.Control || _control.Equals(control)==false)
            {
                _type = wTreeNodeItemTypes.Control;
                _control = control;
                if (E_ItemChanged != null) E_ItemChanged(this);
            }
        }

        public void ChangeItem(WhiteSpaceTypes type)
        {
            if (_type != wTreeNodeItemTypes.WhiteSpace || _whiteSpaceType!=type)
            {
                _type = wTreeNodeItemTypes.WhiteSpace;
                _whiteSpaceType = type;
                
                if (E_ItemChanged != null) E_ItemChanged(this);
            }
        }
        #endregion

        #region drawItem

        /// <summary>
        /// item area가 시작될 point..
        /// </summary>
        Point _point = new Point(0, 0);
        Size _size = new Size(0, 0);
        /// <summary>
        /// Item들 중 그릴 수 있는 것을 그리고 그 크기를 리턴한다.
        /// </summary>
        /// <param name="g">그릴 Graphics</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="drawnSize"></param>
        /// <returns></returns>
        public Size DrawItem(Graphics g, int x, int y, int lineHeight)
        {
            
            if (_type == wTreeNodeItemTypes.CheckBox)
            {
                
                return DrawCheckBox(g, x, y, lineHeight);
            }
            else if (_type == wTreeNodeItemTypes.Image)
            {
                return DrawImage(g, x, y, lineHeight);
            }
            else if (_type == wTreeNodeItemTypes.Text)
            {
                return DrawString(g, x, y, lineHeight);
            }
            else if (_type == wTreeNodeItemTypes.Control)
            {
                return DrawControl(x, y, lineHeight);
            }
            else//whiteSpace
            {
                if(_whiteSpaceType == WhiteSpaceTypes.Space){
                    _point = new Point(x, y);
                    _size = new Size(10, lineHeight);
                    return _size;
                }else{
                    _point = new Point(x,y);
                    _size = new Size(10, 0);
                         
                    return new Size(-x, 0);//다음 줄로 넘어감..
                }
            }
        }

        /// <summary>
        /// baseSize 안에서 itemSize를 가진 item이 중앙이 되려면 어느 위치에서 시작해야 하는지..
        /// </summary>
        /// <param name="itemSize"></param>
        /// <param name="baseSize"></param>
        /// <returns></returns>
        private int getCenter(int itemSize, int baseSize)
        {
            return (baseSize - itemSize) / 2;
        }

        public Size DrawCheckBox(Graphics g, int x, int y, int lineHeight)
        {
            
            _point = new Point(x, y);

            Image checkBox;
            if (_isChecked == true)
            {
                checkBox = DrawHandler.CheckBoxImages[1];
            }
            else if (_isChecked == false)
            {
                checkBox = DrawHandler.CheckBoxImages[0];
            }
            else
            {
                checkBox = DrawHandler.CheckBoxImages[2];
            }
            int toY = getCenter(checkBox.Height, lineHeight) + y;
            g.DrawImage(checkBox, new Point(x + ItemXMargin, toY));
            _size = new Size(checkBox.Width + ItemXMargin, lineHeight);
            return _size;
        }

        public Size DrawImage(Graphics g, int x, int y, int lineHeight)
        {
            
            _point = new Point(x, y);
            if (_image != null)
            {
                int wid = _imageWidth < 0 ? _image.Width : _imageWidth;
                int height = _imageHeight < 0 ? _image.Height : _imageHeight;
                
                int toY = getCenter(height, lineHeight) + y;
                Image img;
                if (_imageWidth >= 0 || _imageHeight >= 0)
                {
                    img = new Bitmap(wid, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    Graphics ig = Graphics.FromImage(img);
                    ig.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    ig.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    ig.DrawImage(_image, 0, 0, wid, height);
                }
                else img = _image;

                g.FillRectangle(_ownerNode.BackColor, new Rectangle(new Point(x+ItemXMargin), img.Size));
                
                g.DrawImage(img, new Point(x + ItemXMargin, toY));

                _size = new Size(wid + ItemXMargin, lineHeight);
                return _size;
            }
            else
            {
                return Size.Empty;
            }
        }

        
       
        
 

        /// <summary>
        /// Main에서 _control의 위치를 잡거나 추가한다.
        /// </summary>
        /// <param name="x">node 안에서 상대적인 X위치..</param>
        /// <param name="y">node안에서 상대적인 Y위치..</param>
        /// <returns></returns>
        public Size DrawControl(int x, int y, int lineHeight)
        {
            
            _point = new Point(x, y);
            if (_control != null && _ownerNode.IsVisible && _ownerNode.TreeParent.IsExpanded)
            {
                
                int toY = getCenter(_control.Height, lineHeight)+y;
                Point pt = new Point(_ownerNode.Location.X + x + ItemXMargin, _ownerNode.Location.Y + toY);
                _ownerNode.OwnerTree.wDrawHandler.SetControlPositionInMain(_control, pt);
                
                _size=new Size(_control.Width + ItemXMargin, lineHeight);
                return _size;
            }
            else {
                return Size.Empty;
            }
            Visible = true;
        }
        
        public Size DrawString(Graphics g, int x, int y, int lineHeight)
        {
            if (TextColor == Color.Empty) return DrawString(g, x, y, lineHeight, _ownerNode.ForeColor);
            else return DrawString(g, x, y, lineHeight, TextColor);
        }
        public Size DrawSelectedString(Graphics g, int x, int y, int lineHeight)
        {
            if(SelectedTextColor == Color.Empty) return DrawString(g, x, y, lineHeight, _ownerNode.SelectedForeColor);
            else return DrawString(g, x, y, lineHeight, SelectedTextColor);
        }

        public Size DrawHoveredString(Graphics g, int x, int y, int lineHeight)
        {
            if(HoveredTextColor == Color.Empty) return DrawString(g, x, y, lineHeight, _ownerNode.HoveredForeColor);
            else return DrawString(g, x, y, lineHeight, HoveredTextColor);
        }

        public Size DrawString(Graphics g, int x, int y, int lineHeight, Color textColor)
        {
            _point = new Point(x, y);
            string text;
            if (ItemType == wTreeNodeItemTypes.TextArray)
            {
                if (_selectedIndex >= 0)
                {
                    if (_selectedIndex < _texts.Count)
                    {
                        text = _texts.ElementAt(_selectedIndex);
                    }
                    else
                    {
                        text = _texts.ElementAt(0);
                    }
                }
                else
                {
                    text = "";
                }

                if (text.Length == 0)
                {
                    text = "       ";//없으면 크기가 적당한 빈 Text할당.
                }
            }
            else// if(ItemType == wTreeNodeItemTypes.Text)
            {
                text = _text;
            }

            SizeF size = g.MeasureString(text, _ownerNode.Font);

            int toY = getCenter((int)size.Height, lineHeight) + y;

            g.DrawString(text, _ownerNode.Font, new SolidBrush(textColor), new PointF(x + ItemXMargin, toY));

            _size = new Size((int)size.Width + ItemXMargin, lineHeight);
            return _size;
        }

        #endregion

        #region getSize
        public Size GetItemSize()
        {
            if (_type == wTreeNodeItemTypes.CheckBox)
            {
                return GetCheckBoxSize();
            }
            else if (_type == wTreeNodeItemTypes.Image)
            {
                return GetImageSize();
            }
            else if (_type == wTreeNodeItemTypes.Text)
            {
                return GetStringSize();
            }
            else if (_type == wTreeNodeItemTypes.TextArray)
            {
                return GetStringArraySize();
            }
            else if (_type == wTreeNodeItemTypes.Control)
            {
                return GetControlSize();
            }
            else//whiteSpace
            {
                if (_whiteSpaceType == WhiteSpaceTypes.Space)
                {
                    return new Size(10, 0);
                }
                else
                {
                    return new Size(-1, 0);//다음 줄로 넘어감..
                }
            }
        }

        public Size GetCheckBoxSize()
        {
            Image checkBox;
            if (_isChecked == true)
            {
                checkBox = DrawHandler.CheckBoxImages[1];
            }
            else if (_isChecked == false)
            {
                checkBox = DrawHandler.CheckBoxImages[0];
            }
            else
            {
                checkBox = DrawHandler.CheckBoxImages[2];
            }
            return new Size(checkBox.Size.Width + ItemXMargin, checkBox.Height);
        }

        public Size GetImageSize()
        {
            if (_image != null)
            {
                int wid = _imageWidth < 0 ? _image.Width : _imageWidth;
                int height = _imageHeight < 0 ? _image.Height : _imageHeight;
                return new Size(wid + ItemXMargin, height);
            }
            else
            {
                return Size.Empty;
            }
        }

        public Size GetControlSize()
        {
            if (_control != null)
            {
                return new Size(_control.Size.Width + ItemXMargin, _control.Height);
            }
            else
            {
                return Size.Empty;
            }
        }

        public Size GetStringSize()
        {
            Graphics g = Graphics.FromImage(_ownerNode.OwnerTree.wDrawHandler.ImageTempBufferToDrawForTree);

            if (_text.Length == 0)
            {
                string textToMeasure = _text;
                textToMeasure = " ";
                SizeF size = g.MeasureString(textToMeasure, _ownerNode.Font);
                return new Size((int)size.Width + ItemXMargin + 2, (int)size.Height);
            }
            else
            {
                SizeF size = g.MeasureString(_text, _ownerNode.Font);
                return new Size((int)size.Width + ItemXMargin+2, (int)size.Height);
            }
        }

        public Size GetStringArraySize()
        {
            Graphics g = Graphics.FromImage(_ownerNode.OwnerTree.wDrawHandler.ImageTempBufferToDrawForTree);
            string text;

            if (_selectedIndex >= 0)
            {
                if (_selectedIndex < _texts.Count)
                {
                    text = _texts.ElementAt(_selectedIndex);
                }
                else
                {
                    text = _texts.ElementAt(0);
                }
            }
            else
            {
                text = "";
            }

            if (text.Length == 0)
            {
                string textToMeasure = "       ";//없으면 크기가 적당한 빈 Text할당.
                SizeF size = g.MeasureString(textToMeasure, _ownerNode.Font);
                return new Size(0, (int)size.Height);
            }
            else
            {
                SizeF size = g.MeasureString(text, _ownerNode.Font);
                return new Size((int)size.Width + ItemXMargin, (int)size.Height);
            }
        }
        #endregion

        #region properties
        wTreeNodeItemTypes _type;

        /// <summary>
        /// item의 타입이 무엇인지 나타낸다.
        /// </summary>
        public wTreeNodeItemTypes ItemType
        {
            get { return _type; }
        }

        

        wTreeNode _ownerNode = null;

        /// <summary>
        /// x축 방향의 공백..
        /// </summary>
        int ItemXMargin
        {
            get { return DrawHandler.ItemXMargin; }
        }


        //Rectangle _itemArea = new Rectangle();
        public Rectangle ItemArea
        {
            get
            {
                return new Rectangle(_point, _size);
                
            }
        }

        Control _control;
        /// <summary>
        /// Control이라면 해당 Control를 리턴한다.
        /// </summary>
        public Control Control {
            get { return _control; }
            internal set { _control = value; }
        }
        
        internal int _itemIndex=-1;
        /// <summary>
        /// wTreeNodeItem이 wTreeNode안에서 몇번째인지. 0부터 시작. 아직 할당 안되었으면 -1.
        /// </summary>
        public int ItemIndex { get { return _itemIndex; } internal set { _itemIndex = value; } }
        #endregion

        /// <summary>
        /// ItemType이 WhiteSpace일 때, 그 타입을가져온다.
        /// </summary>
        public WhiteSpaceTypes WhiteSpaceType { 
            get{ return _whiteSpaceType;}
        }

        /// <summary>
        /// Item이 보이는지 아닌지 나타낸다.
        /// </summary>
        public bool Visible { get; set; }

        
    }

    
    /// <summary>
    /// 빈칸 혹은 다음줄로 넘어가는 것을 나타내는 클래스..
    /// </summary>
    public class WhiteSpace{
        WhiteSpaceTypes _type;
        public WhiteSpace(WhiteSpaceTypes type){
            _type = type;
        }
    }
}
