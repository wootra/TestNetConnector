using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders; using DataHandling;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageButtonCell : DataGridViewButtonCell, IEasyGridCell
    {
        DataGridView _parent;
        ICollection<Image> _savedImages = new List<Image>();
        Image _image = null;
        int ImageFixedWidth = 0;
        int ImageFixedHeight = 0;
        Timer _refreshTimer;

        public EasyGridImageButtonCell(DataGridView parent)
            : base()
        {
            _parent = parent;
            this.ImageLayout = System.Windows.Forms.ImageLayout.Center;
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += new EventHandler(_refreshTimer_Tick);
            _info = new CellSpanInfo(this);
        }

        CellSpanInfo _info;
        public CellSpanInfo Span
        {
            get { return _info; }
        }

        CustomDictionary<String, Object> _relativeObject = new CustomDictionary<string, object>();
        public CustomDictionary<String, Object> RelativeObject { get { return _relativeObject; } }

        void _refreshTimer_Tick(object sender, EventArgs e)
        {
            Point pt = this.DataGridView.PointToClient(Control.MousePosition);
            Rectangle rect = _parent.GetCellDisplayRectangle(this.ColumnIndex, this.RowIndex, false);
            if ( rect.Contains(pt) && _visible)
            {
            }
            else
            {
                _parent.InvalidateCell(this);
                _parent.Update();
                //_refreshTimer.Stop();
            }
        }

        public enum ButtonStates { Over, Out, Press, Up };
        ButtonStates _buttonStates = ButtonStates.Out;
        public ButtonStates ButtonState
        {
            get
            {
                return _buttonStates;
            }
            set
            {
                if (_buttonStates != value)
                {
                    _buttonStates = value;
                    RePaint();
                }
            }
        }
        

        public ICollection<Image> Images
        {
            get { return _savedImages; }
            set {
                _savedImages = value;
                if (_image == null && _savedImages.Count > 0) _image = _savedImages.ElementAt(0);
            }
        }
        int _disabledIndex = 1;
        public int DisabledIndex
        {
            get { return _disabledIndex; }
            set { _disabledIndex = value; }
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
                if (_enabled == true)
                {
                    if(Images.Count>0) _image = Images.ElementAt(0);
                }
                else
                {
                    if(Images.Count>_disabledIndex) _image = Images.ElementAt(_disabledIndex);
                }
                //RePaint();
            }
        }
        string _text="";
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        bool _visible = true;
        bool _tempEnabled = false;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {

                _visible = value;
                if (value == false) Enabled = false;// _enabled = false;
                else if (Enabled == false) Enabled = _tempEnabled;

                _tempEnabled = _enabled;
                //RePaint();
            }
        }

        public ItemTypes ItemType
        {
            get { return ItemTypes.ImageButton; }
        }

        public new Type ValueType
        {
            get
            {
                return typeof(Image);
            }
        }
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
        
        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            paintParts = DataGridViewPaintParts.Background| DataGridViewPaintParts.Border| DataGridViewPaintParts.Focus | DataGridViewPaintParts.SelectionBackground ;
            base.Paint(g, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            /*
            switch(_buttonStates)
            {
                case ButtonStates.Out:
                    //CellFunctions.DrawPlainBackground(this, this.Enabled, cellBounds, g, rowIndex, this.Selected, this.DataGridView.GridColor);
                    break;
                case ButtonStates.Over:
                    CellFunctions.DrawLensBack(this, cellBounds, g, this.DataGridView.GridColor, true, Enabled);
                    break;
                case ButtonStates.Press:
                    CellFunctions.DrawLensBack(this, cellBounds, g, this.DataGridView.GridColor, false, Enabled);
                    break;
                case ButtonStates.Up:
                    CellFunctions.DrawLensBack(this, cellBounds, g, this.DataGridView.GridColor, true, Enabled);
                    break;
            }
            */

            PaintCell(cellBounds, g);

            
        }

        void PaintCell(Rectangle cellBounds, Graphics g)
        {
            if (_visible)
            {
                if (_image != null)
                {
                    int minWidth = (cellBounds.Width < _image.Width) ? cellBounds.Width : _image.Width;
                    int minHeight = (cellBounds.Height < _image.Height) ? cellBounds.Height : _image.Height;
                    float widRate = (float)cellBounds.Width/ (float)_image.Width ;
                    float higRate = (float)cellBounds.Height / (float)_image.Height;
                    float zoomRate = (widRate < higRate) ? widRate : higRate;
                    float fitToBigRate = (zoomRate > 1) ? 1 : zoomRate;
                    
                    if (_imageLayout == System.Windows.Forms.ImageLayout.Center)
                    {
                        int wid = (ImageFixedWidth == 0) ? (int)(_image.Width*fitToBigRate) : ImageFixedWidth;
                        int hig = (ImageFixedHeight == 0) ? (int)(_image.Height*fitToBigRate) : ImageFixedHeight;
                        int x = cellBounds.Location.X + (cellBounds.Width - wid) / 2;

                        int y = cellBounds.Location.Y + (cellBounds.Height - hig) / 2;
                        g.DrawImage(_image, x, y, wid, hig);
                    }
                    else if (_imageLayout == System.Windows.Forms.ImageLayout.None || _imageLayout == System.Windows.Forms.ImageLayout.Tile)
                    {
                        int wid = (ImageFixedWidth == 0) ? (int)(_image.Width * fitToBigRate) : ImageFixedWidth;
                        int hig = (ImageFixedHeight == 0) ? (int)(_image.Height * fitToBigRate) : ImageFixedHeight;
                        int x = cellBounds.Location.X;
                        int y = cellBounds.Location.Y + (cellBounds.Height - hig) / 2;
                        g.DrawImage(_image, new Rectangle(x, y, wid, hig));
                    }
                    else if (_imageLayout == System.Windows.Forms.ImageLayout.Zoom)
                    {
                        Image image = _image;
                        
                        int wid = (int)(image.Width * zoomRate);
                        int hig = (int)(image.Height * zoomRate);
                        int x = cellBounds.Location.X + (cellBounds.Width - wid) / 2;

                        int y = cellBounds.Location.Y + (cellBounds.Height - hig) / 2;
                        g.DrawImage(_image, new Rectangle(x, y, wid, hig));
                    }
                    else if (_imageLayout == System.Windows.Forms.ImageLayout.Stretch)
                    {
                        Image image = _image;
                        float xrate = (image.Width * 1.0f / cellBounds.Width);
                        float yrate = (image.Height * 1.0f / cellBounds.Height);


                        int wid = (int)(image.Width * xrate);
                        int hig = (int)(image.Height * yrate);
                        int x = cellBounds.Location.X + (cellBounds.Width - wid) / 2;

                        int y = cellBounds.Location.Y + (cellBounds.Height - hig) / 2;
                        g.DrawImage(_image, new Rectangle(x, y, wid, hig));
                    }
                }
            }
        }
        
        public void OnPaint(DataGridViewCellPaintingEventArgs e)
        {
            if (_enabled && _visible)
            {
                Point pt = this.DataGridView.PointToClient(Control.MousePosition);
                if (e.CellBounds.Contains(pt) && _visible && _enabled)
                {
                    
                    CellFunctions.DrawPlainBackground(this, this.Enabled, e.CellBounds, e.Graphics, e.RowIndex, true, this.DataGridView.GridColor);
                    //_refreshTimer.Start();
                }
                else
                {
                    CellFunctions.DrawPlainBackground(this, this.Enabled, e.CellBounds, e.Graphics, e.RowIndex, false, this.DataGridView.GridColor);
                    //_refreshTimer.Stop();
                }
                PaintCell(e.CellBounds, e.Graphics);
                e.Handled = true;
            }
        }
        int _selectedIndex = -1;
        public new object Value
        {
            get
            {
                return _image;
            }
            set
            {
                if(value is EasyGridCellInfo){
                    SetValue(value as EasyGridCellInfo);
                }
                else if (value is String)
                {
                    SetValue(value as String);
                }
                else if (value is Image)
                {
                    SetValue(value as Image);
                }
                else if (value is int)
                {
                    SetValue((int)value);
                }
                else if (value is Enum)
                {
                    SetValue((int)(Convert.ChangeType(value, typeof(int))));
                }
                else
                {
                    throw new InvalidTypeException(value, new Type[] { typeof(EasyGridCellInfo), typeof(int), typeof(Image) });
                    //throw new Exception("테이블 [" + this.Name + "]의 입력 멤버타입이 틀립니다. row:" + row.Index + ", cell:" + colIndex + ", trying value:" + (value as String) + "/ 원래 타입:int, Image");
                }
            }
        }
        public void SetValue(EasyGridCellInfo info){
            if (info.Images != null) this.Images = info.Images;

            if (info.SelectedIndex >= 0)
            {
                _text = info.Text;
                if (Images != null && Images.Count >info.SelectedIndex)
                {
                    _selectedIndex = info.SelectedIndex;
                    _image = Images.ElementAt(_selectedIndex);
                }
                
                this.ImageLayout = info.ImageLayout;
            }
        }
        ImageLayout _imageLayout = ImageLayout.Center;
        public new ImageLayout ImageLayout
        {
            get
            {
                return _imageLayout;
            }
            set
            {
                _imageLayout = value;
                
            }
        }

        public void SetValue(Image image)
        {
            
            _image = image;
            if (Images != null)
            {
                int index = Images.ToList().IndexOf(image);
                _selectedIndex = index;
            }
        }

        public void SetValue(String text)
        {
            _text =  text;
        }

        public void SetValue(int index)
        {
            if (index < 0)
            {
                _image = null;
                _selectedIndex = -1;
            }else if (Images != null && Images.Count > index)
            {
                _image = Images.ElementAt(index);
                _selectedIndex = index;
            }
            //_image = Images.ElementAt(index);

        }

        public int GetValueInt()
        {
            return _selectedIndex;
        }

        public Image GetValue()
        {
            return _image as Image;
        }

        public Image GetValueImage()
        {
            return _image as Image;
        }

        public String GetValueText()
        {
            return _text;
        }

        public void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }


        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseUp(e);
        }
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (_enabled) base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseEnter(int rowIndex)
        {
            if (_enabled) base.OnMouseEnter(rowIndex);
        }
        protected override void OnMouseLeave(int rowIndex)
        {
            if (_enabled) base.OnMouseLeave(rowIndex);
        }
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnClick(e);
        }
        protected override void OnDoubleClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnDoubleClick(e);
        }
        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnContentClick(e);
        }
        protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
        {
            if (_enabled) base.OnContentDoubleClick(e);
        }
        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (_enabled) base.OnEnter(rowIndex, throughMouseClick);
        }
        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyDown(e, rowIndex);
        }
        protected override void OnKeyPress(KeyPressEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyPress(e, rowIndex);
        }
        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            if (_enabled) base.OnKeyUp(e, rowIndex);
        }

    }
}
