using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FormAdders.EasyGridViewCollections;
using FormAdders.ImageTabControlEnums;
using System.Collections;
using System.Drawing.Drawing2D;
namespace FormAdders
{
    [Designer(typeof(FormAdders.Designer.ImageTabControlDesigner))]
    public partial class ImageTabControl : Panel
    {
        ImageTabControl.ControlCollection _tabs;
        //SplitContainer _sContainer = new SplitContainer();
        public event DrawItemEventHandler E_DrawTabItems = null;
        /// <summary>
        /// Occurs after the <see cref="ScrollButtonStyle"/>
        /// property has changed.
        /// </summary>
        public event EventHandler ScrollButtonStyleChanged;
        //internal Panel _tabPanel = new Panel();

        public event ImageTabSelectedEvent E_TabSelected;
        public new event EventHandler TabIndexChanged;
    
        public ImageTabControl():base()
        {
            
            _imageList.Add(Properties.Resources.Radio_Back);
            _imageList.Add(Properties.Resources.RadioBlue);
            _tabs = base.Controls as ImageTabControl.ControlCollection;
            
            this.Width = 50;
            this.Height = 30;
            this.Dock = DockStyle.None;
            this.Location = new Point(0, 0);
            this.BackColor = Color.Transparent;
            this.ActiveTextColor = Color.Black;
            this.InactiveTextColor = Color.Gray;
            /*
            _sContainer.Dock = DockStyle.Fill;
            _sContainer.SplitterDistance = 20;
            _sContainer.Margin = this.Padding;
            _sContainer.Orientation = Orientation.Horizontal;
            _sContainer.SplitterWidth = 1;
            _sContainer.BackColor = Color.Transparent;
            _sContainer.Panel1.BackColor = Color.Transparent;
            _sContainer.Panel2.BackColor = Color.Transparent;

            base.Controls.Add(_sContainer);
            */
            this.Click += new EventHandler(ImageTabContol_Click);
            
            //_tabs = new ImageTabPageCollection(this);
            //_tabs.Add(new ImageTabPage("Tab1"));
            
            
            
            
            Init();
            
        }

        
        void Init()
		{
			SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true );
			defaultImageIndex = -1;
			yaTabLengths = new ArrayList( 5 );
			leftArrow = new Point[ 3 ];
			rightArrow = new Point[ 3 ];
			for( int i = 0; i < 3; i++ )
			{
				leftArrow[ i ] = new Point( 0, 0 );
				rightArrow[ i ] = new Point( 0, 0 );
			}
			yaTabFont = Font;
			yaBoldTabFont = new Font( Font.FontFamily.Name, Font.Size, FontStyle.Bold );
			yaMargin = 3;
			//_tabDock = DockStyle.Top;
			SelectedIndex = -1;
			yaForeBrush = new SolidBrush( ForeColor );
			yaActiveBrush = ( Brush ) SystemBrushes.Control.Clone();
			yaActiveColor = SystemColors.Control;
			yaInactiveBrush = ( Brush ) SystemBrushes.Window.Clone();
			yaInactiveColor = SystemColors.Window;
			Border = ( Pen ) Pens.DarkGray.Clone();
			yaShadowPen = ( Pen ) SystemPens.ControlDark.Clone();
			yaHighlightPen = ( Pen ) SystemPens.ControlLight.Clone();

            _transformedDisplayRectangle = Rectangle.Empty;
            
			ChildTextChangeEventHandler = new EventHandler( YaTabPage_TextChanged );
            
            this.ControlAdded += new ControlEventHandler(ImageTabContol_ControlAdded);
            this.SizeChanged += new EventHandler(ImageTabContol_SizeChanged);
            SetLayouts();
		}
        public void SetLayouts(bool resetPageIndexes=false)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    SetLayouts(resetPageIndexes);
                    
                }));
                return;
            }
            
            this.Visible = false;
            int tabY = (_tabDock == DockStyle.Bottom) ? this.Height - _tabHeight : 0;
            int clientY = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
            int clientHeight = this.Height - _tabHeight;

            _tabsRectangle = new Rectangle(0, tabY, this.Width, _tabHeight);

            _clientRectangle = new Rectangle(0, clientY, this.Width, clientHeight);
            _displayRectangle = new Rectangle(0, clientY, this.Width, clientHeight);

            if (resetPageIndexes)
            {

                

                int selectedIndex = this.SelectedIndex;

                

                Dictionary<int, ImageTabPage> pages = new Dictionary<int, ImageTabPage>();
                //this.SuspendLayout();
                for (int i = 0; i < this.TabPages.Count; i++)
                {
                    ImageTabPage page = this.TabPages[i] as ImageTabPage;
                    pages[page.TabIndex] = page;

                }
                
                    
                this.Controls.Clear();
                this.SuspendLayout();
                for (int i = 0; i < pages.Count; i++)// = pages.Count-1; i>=0; i--)
                {
                    try
                    {
                        this.Controls.Add(pages[i]);
                    }
                    catch
                    {
                    }

                }
                this.ResumeLayout();
                this.PerformLayout();
                
                
                this.SelectedIndex = selectedIndex;
                
            }
            for (int i = 0; i < _tabs.Count; i++)
            {
                //(_tabs[i] as ImageTabPage).SetBounds(this.Location.X, this.Location.Y + _tabHeight, this.Width, this.Height - _tabHeight);
                (_tabs.TabPages[i] as ImageTabPage).SetBounds(this.Location.X, this.Location.Y + clientY, this.Width, this.Height - _tabHeight);

            }
            try
            {
                CalculateTabLengths();
                CalculateLastVisibleTabIndex();
            }
            catch { }
            //if (resetPageIndexes) this.ResumeLayout();
           
            //Height = Width = 300;
            //BackColor = SystemColors.Control;
            //CalculateTabSpan();
           
                this.Visible = true;
           
        }


        /// <summary>
        /// Handles when the text changes for a control.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="YaTabPage"/> whose text changed.
        /// </param>
        /// <param name="e">
        /// Some <see cref="EventArgs"/>.
        /// </param>
        private void YaTabPage_TextChanged(object sender, EventArgs e)
        {
            //SetLayouts();
            CalculateTabLengths();
            CalculateLastVisibleTabIndex();
        }

        void ImageTabContol_SizeChanged(object sender, EventArgs e)
        {
 
            SetLayouts(false);


        }

        [EditorBrowsable]
        /// <summary>
        /// Gets and sets the docking side of the tabs.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if the property tries to get set to
        /// <see cref="DockStyle.Fill"/> of <see cref="DockStyle.None"/>.
        /// </exception>
        /// <remarks>
        /// The default value of this property is <see cref="DockStyle.Top"/>.
        /// </remarks>
        public virtual DockStyle TabDock
        {
            get
            {
                return _tabDock;
            }
            set
            {
                if (value != DockStyle.Top && value != DockStyle.Bottom)
                {
                    _tabDock = DockStyle.Top;
                }
                else _tabDock = value;
                /*
                if (yaTabDrawer == null || yaTabDrawer.SupportsTabDockStyle(value))
                {
                    yaTabDock = value;
                }
                else
                {
                    throw new ArgumentException("Tried to set the TabDock property to a value not supported by the current tab drawer.");
                }
                
                 */
                CalculateRectangles();
                PerformLayout();
                InU();
                //OnTabDockChanged(new EventArgs());
            }
        }
        /// <summary>
        /// Calculates the rectangles for the tab area, the client area,
        /// the display area, and the transformed display area.
        /// </summary>
        private void CalculateRectangles()
        {
            int spanAndMargin = Convert.ToInt32(yaTabSpan) + 3 * yaMargin + 2;
            Size s;

            int tabY = (_tabDock == DockStyle.Bottom) ? this.Height - _tabHeight : 0;
            int clientY = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
            int clientHeight = this.Height - _tabHeight;

            calcHeight = (DockStyle.Top == _tabDock || DockStyle.Bottom == _tabDock) ? Height : Width;
            calcWidth = (DockStyle.Top == _tabDock || DockStyle.Bottom == _tabDock) ? Width : Height;
            
            _tabsRectangle = new Rectangle(0, tabY, this.Width, _tabHeight);
            _clientRectangle = new Rectangle(0, clientY, this.Width, clientHeight);
            _displayRectangle = new Rectangle(0, clientY, this.Width, clientHeight);
            for (int i = 0; i < _tabs.Count; i++)
            {
                //(_tabs[i] as ImageTabPage).SetBounds(this.Location.X, this.Location.Y + _tabHeight, this.Width, this.Height - _tabHeight);
                (_tabs.TabPages[i] as ImageTabPage).SetBounds(0, clientY, this.Width, this.Height - _tabHeight);

            }
            /*
            yaTabsRectangle.X = 0;
            yaTabsRectangle.Y = 0;
            s = yaTabsRectangle.Size;
            s.Width = calcWidth;
            s.Height = spanAndMargin;
            yaTabsRectangle.Size = s;
            */
            s = _clientRectangle.Size;

            leftArrow[0].X = s.Width - s.Height - yaMargin;
            leftArrow[0].Y = yaMargin;
            leftArrow[1].X = leftArrow[0].X;
            leftArrow[1].Y = s.Height - yaMargin;
            leftArrow[2].X = s.Width - 2 * s.Height + yaMargin;
            leftArrow[2].Y = s.Height / 2;

            rightArrow[0].X = s.Width - s.Height + yaMargin;
            rightArrow[0].Y = yaMargin;
            rightArrow[1].X = rightArrow[0].X;
            rightArrow[1].Y = s.Height - yaMargin;
            rightArrow[2].X = s.Width - yaMargin;
            rightArrow[2].Y = s.Height / 2;
            /*
            yaClientRectangle.X = 0;
            yaClientRectangle.Y = spanAndMargin;
            s = yaClientRectangle.Size;
            s.Width = calcWidth;
            s.Height = calcHeight - spanAndMargin;
            yaClientRectangle.Size = s;
            
            yaDisplayRectangle.X = yaMargin + 1;
            yaDisplayRectangle.Y = spanAndMargin + yaMargin + 1;
            s = yaDisplayRectangle.Size;
            s.Width = calcWidth - 2 * (yaMargin + 1);
            s.Height = yaClientRectangle.Size.Height - 2 * yaMargin - 2;
            yaDisplayRectangle.Size = s;
            */
            switch (_tabDock)
            {
                case DockStyle.Top:
                    _transformedDisplayRectangle.Location = _displayRectangle.Location;
                    _transformedDisplayRectangle.Size = _displayRectangle.Size;
                    break;
                case DockStyle.Bottom:
                    _transformedDisplayRectangle.X = yaMargin + 1;
                    _transformedDisplayRectangle.Y = yaMargin + 1;
                    _transformedDisplayRectangle.Size = _displayRectangle.Size;
                    break;
                case DockStyle.Right:
                    _transformedDisplayRectangle.X = yaMargin + 1;
                    _transformedDisplayRectangle.Y = yaMargin + 1;
                    s.Height = _displayRectangle.Size.Width;
                    s.Width = _displayRectangle.Size.Height;
                    _transformedDisplayRectangle.Size = s;
                    break;
                case DockStyle.Left:
                    _transformedDisplayRectangle.X = _displayRectangle.Top;
                    _transformedDisplayRectangle.Y = calcWidth - _displayRectangle.Right;
                    s.Height = _displayRectangle.Size.Width;
                    s.Width = _displayRectangle.Size.Height;
                    _transformedDisplayRectangle.Size = s;
                    break;
            }
        }
        public void SetTabSize(int index)
        {
            int clientY = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
            (_tabs.TabPages[index] as ImageTabPage).SetBounds(0, clientY, this.Width, this.Height - _tabHeight);
        }

        public void SetTabSize(ref ImageTabPage tab)
        {
            int clientY = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
            (tab as ImageTabPage).SetBounds(0, clientY, this.Width, this.Height - _tabHeight);
        }

        bool _isAdding = false;
        /// <summary>
        /// 이 ImageTabControl이 부모에게 추가되었을 때 호출되는 이벤트이다. 자식이 추가되었을 때는
        /// OnControlAdded 함수를 override하여 사용하여야 한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ImageTabContol_ControlAdded(object sender, ControlEventArgs e)
        {
            if (_isAdding == false)
            {
                _isAdding = true;
                yaTabLengths.Clear();

                for (int i = 0; i < _tabs.Count; i++)
                {
                    Rectangle rect = GetTabHeadBounds(_tabs.TabPages[i]);
                    yaTabLengths.Add(rect.Width);
                    /*
                    if (_tabDock == DockStyle.Bottom)
                    {
                        _tabs.TabPages[i].SetBounds(_clientRectangle.X, _clientRectangle.Y, _clientRectangle.Width, _clientRectangle.Height);
                    }
                    else
                    {
                        _tabs.TabPages[i].SetBounds(_clientRectangle.X, _clientRectangle.Y, _clientRectangle.Width, _clientRectangle.Height);
                    }
                    */
                }
                /*
                if (SelectedIndex < 0) //딱히 선택한 것이 없다면
                {
                    SelectedIndex = _tabs.Count-1; //제일 마지막 것을 선택한다.
                }
                */
                //SetLayouts(true);
                if (_selectedIndex >= 0)
                {
                    _tabs.TabPages[_selectedIndex].BringToFront();
                    //this.InU();
                }
                _isAdding = false;
            }
        }

        public bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        base.Enabled = value;
                    }));
                }
                else
                {
                    base.Enabled = value;
                }
            }
        }

        void ImageTabContol_Click(object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                for (int i = 0; i < _tabs.TabPages.Count; i++)
                {
                    if (this.Enabled && _tabs.TabPages[i].Enabled)
                    {
                        Rectangle rect = GetTabHeadBounds(_tabs.TabPages[i]);

                        if (CellFunctions.MouseHitTest(this, rect))
                        {
                            SelectedIndex = i;
                            ImageTabSelectedArgs args = new ImageTabSelectedArgs(_tabs.TabPages[i], i);
                            if (E_TabSelected != null) E_TabSelected(this, args);
                            if (TabIndexChanged != null) TabIndexChanged(this, args);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the bounding rectangle for a specified tab in this tab control.
        /// </summary>
        /// <param name="index">The 0-based index of the tab you want.</param>
        /// <returns>A <see cref="Rectangle"/> that represents the bounds of the specified tab.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The index is less than zero.<br />-or-<br />The index is greater than or equal to <see cref="Control.ControlCollection.Count" />.
        /// </exception>
        public virtual Rectangle GetTabRect(ImageTabPage tabPage)
        {

            return GetTabHeadBounds(tabPage);
            #region before...
            /*
            float l = 0.0f;
            if (yaTabDock == DockStyle.Left)
            {
                l = Height - Convert.ToSingle(yaTabLengths[0]) + yaTabLeftDif;
                for (int i = 0; i < index; i++)
                {
                    l -= Convert.ToSingle(yaTabLengths[i + 1]);
                }
            }
            else
            {
                
                for (int i = 0; i < index; i++)
                {
                    l += GetTabHeadBounds(i).Width;
                }
            }*/
              /*
            switch (yaTabDock)
            {
                case DockStyle.Bottom:
                    return new Rectangle(Convert.ToInt32(l), 3 * yaMargin + yaClientRectangle.Height, Convert.ToInt32(yaTabLengths[index]), Convert.ToInt32(yaTabSpan) + yaMargin);
                case DockStyle.Right:
                    return new Rectangle(yaClientRectangle.Height, Convert.ToInt32(l), Convert.ToInt32(yaTabSpan) + yaMargin, Convert.ToInt32(yaTabLengths[index]));
                case DockStyle.Left:
                    return new Rectangle(yaMargin, Convert.ToInt32(l), Convert.ToInt32(yaTabSpan) + yaMargin, Convert.ToInt32(yaTabLengths[index]));
            }
              */
            //return GetTabHeadBounds(index);//new Rectangle(Convert.ToInt32(l),0, GetTabHeadBounds(index).Width, _tabHeight);
            #endregion

        }

        /// <summary>
        /// Gets the <see cref="Rectangle"/> that contains the left
        /// scroll button.
        /// </summary>
        /// <returns>
        /// A <see cref="Rectangle"/>.
        /// </returns>
        public virtual Rectangle GetRightScrollButtonRect()
        {
            Rectangle r = Rectangle.Empty;
            if (yaShowScrollButtons == YaScrollButtonStyle.Always)
            {
                int tabSpan = Convert.ToInt32(yaTabSpan);
                switch (_tabDock)
                {
                    case DockStyle.Top:
                        r = new Rectangle(Width - _tabsRectangle.Height, 0, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Bottom:
                        r = new Rectangle(Width - _tabsRectangle.Height, _clientRectangle.Height, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Left:
                        r = new Rectangle(0, 0, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Right:
                        r = new Rectangle(Width - _tabsRectangle.Height, Height - _tabsRectangle.Height, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                }
            }
            return r;
        }

        /// <summary>
        /// Scrolls the tabs by the specified <i>amount</i>.
        /// </summary>
        /// <param name="amount">
        /// The number of pixels to scroll the tabs.
        /// </param>
        /// <remarks>
        /// Positive amounts will scroll the tabs to the left. Negative
        /// amounts will scroll the tabs to the right.
        /// </remarks>
        public virtual void ScrollTabs(int amount)
        {
            yaTabLeftDif = Math.Max(0, yaTabLeftDif - amount);
            if (yaTabLeftDif <= 0 || yaTabLeftDif >= yaTotalTabSpan - Convert.ToSingle(yaTabLengths[yaTabLengths.Count - 1]))
            {
                lock (this)
                {
                    yaKeepScrolling = false;
                }
            }
            if (yaTabLeftDif >= yaTotalTabSpan - Convert.ToSingle(yaTabLengths[yaTabLengths.Count - 1]))
            {
                canScrollLeft = false;
            }
            if (yaTabLeftDif <= 0)
            {
                canScrollRight = false;
            }
            CalculateLastVisibleTabIndex();
            InU();
        }

        //private float yaLastVisibleTabLeft;
        //private int yaLastVisibleTabIndex=0;
        //private int calcWidth;
        /// <summary>
        /// Calculates the last visible tab shown on the control.
        /// </summary>
        private void CalculateLastVisibleTabIndex()
        {
            yaLastVisibleTabLeft = 0.0f;
            float t = 0.0f;
            for (int i = 0; i < _tabs.TabPages.Count; i++)
            {
                yaLastVisibleTabIndex = i;
                t += Convert.ToSingle(GetTabHeadBounds(_tabs.TabPages[i]).Width);
                if (t > calcWidth + yaTabLeftDif)
                {
                    break;
                }
                yaLastVisibleTabLeft = t;
            }
        }

        //private DockStyle yaTabDock;
        //private float yaTabSpan;
        /// <summary>
        /// Gets the <see cref="Rectangle"/> that contains the left
        /// scroll button.
        /// </summary>
        /// <returns>
        /// A <see cref="Rectangle"/>.
        /// </returns>
        public virtual Rectangle GetLeftScrollButtonRect()
        {
            Rectangle r = Rectangle.Empty;
            if (yaShowScrollButtons == YaScrollButtonStyle.Always)
            {
                int tabSpan = Convert.ToInt32(yaTabSpan);
                switch (_tabDock)
                {
                    case DockStyle.Top:
                        r = new Rectangle(Width - 2 * _tabsRectangle.Height, 0, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Bottom:
                        r = new Rectangle(Width - 2 * _tabsRectangle.Height, _clientRectangle.Height, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Left:
                        r = new Rectangle(0, _tabsRectangle.Height, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                    case DockStyle.Right:
                        r = new Rectangle(Width - _tabsRectangle.Height, Height - 2 * _tabsRectangle.Height, _tabsRectangle.Height, _tabsRectangle.Height);
                        break;
                }
            }
            return r;
        }

        [Category("Custom")]
        public Color[] TextColor
        {
            get
            {
                return new Color[] { _activeTextColor.Color, _inactiveTextColor.Color};
            }
            set
            {
                if (value != null && value.Length > 0) _activeTextColor = new SolidBrush(value[0]);
                if (value != null && value.Length > 1) _inactiveTextColor = new SolidBrush(value[1]);
            }
        }

        SolidBrush _activeTextColor = new SolidBrush(Color.Black);
        [Category("Custom")]
        public Color ActiveTextColor{
            get{ return _activeTextColor.Color;
            }
            set{
                _activeTextColor = new SolidBrush(value);
                this.InU();
            }
        }



        SolidBrush _inactiveTextColor = new SolidBrush(Color.Gray);
        [Category("Custom")]
        public Color InactiveTextColor
        {
            get
            {
                return _inactiveTextColor.Color;
            }
            set
            {
                _inactiveTextColor = new SolidBrush(value);
                this.InU();
            }
        }

        int _tabHeight = 20;
        public int TabHeight
        {
            get { return _tabHeight; }
            set { 
                _tabHeight = value;
                //_sContainer.SplitterDistance = value;
                InU();
                //_sContainer.InU();
            }
        }


        //private YaScrollButtonStyle yaShowScrollButtons = YaScrollButtonStyle.Auto;
        /// <summary>
        /// Gets and sets how the scroll buttons should get
        /// shown when drawing the tabs in the tab area.
        /// </summary>
        /// <remarks>
        /// The default value for this is <see cref="YaScrollButtonStyle.Always"/>.
        /// </remarks>
        public virtual YaScrollButtonStyle ScrollButtonStyle
        {
            get
            {
                return yaShowScrollButtons;
            }
            set
            {
                yaShowScrollButtons = value;
                InU();
                OnScrollButtonStyleChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Fires the <see cref="ScrollButtonStyleChanged"/> event.
        /// </summary>
        /// <param name="ea">
        /// Some <see cref="EventArgs"/>.
        /// </param>
        protected virtual void OnScrollButtonStyleChanged(EventArgs ea)
        {
            if (ScrollButtonStyleChanged != null)
            {
                ScrollButtonStyleChanged(this, ea);
            }
        }

        /// <summary>
        /// Invalidates and updates the <see cref="ImageTabControl"/>.
        /// </summary>
        internal void InU()
        {
            Invalidate();
            try
            {
                Update();
            }
            catch { }
        }

        Brush _backColor = new SolidBrush(Color.Transparent);
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                _backColor = new SolidBrush(value);
                //_sContainer.Panel1.BackColor = value;
                //_sContainer.Panel2.BackColor = value;
                base.BackColor = value;
                this.InU();
            }
        }

        Brush _activeTabItemBackColor = Brushes.Transparent;
        public Brush ActiveTabItemBackColor
        {
            get { return _activeTabItemBackColor; }
            set { 
                _activeTabItemBackColor = value;
                this.InU();
            }
        }
        
        Brush _inactiveTabItemBackColor = Brushes.Transparent;
        public Brush InActiveTabItemBackColor
        {
            get { return _inactiveTabItemBackColor; }
            set {
                _inactiveTabItemBackColor = value;
                this.InU();
            }
        }

        bool _useImageAsTabBackground = false;
        public bool UseImageAsTabBackground
        {
            get { return _useImageAsTabBackground; }
            set {
                _useImageAsTabBackground = value;
                this.InU();
            }
        }

        FineImageList _imageList = new FineImageList();
        public FineImageList ImageList
        {
            get { return _imageList; }
            set { 
                _imageList = value;
                this.InU();
            }
        }

        int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                
                for (int j = 0; j < _tabs.TabPages.Count; j++)
                {
                    if (j == value) (_tabs.TabPages[j] as ImageTabPage)._selected = true;
                    else (_tabs.TabPages[j] as ImageTabPage)._selected = false;
                }
                if (value >= 0 && _tabs.TabPages.Count>value)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            _tabs.TabPages[value].BringToFront();
                            InU();
                        }));
                    }
                    else
                    {
                        _tabs.TabPages[value].BringToFront();
                        InU();
                    }
                }
                
            }
        }

        public bool SelectTab(String tabText, bool caseIgnore=false)
        {
            for (int j = 0; j < _tabs.TabPages.Count; j++)
            {
                if (caseIgnore)
                {
                    if ((_tabs.TabPages[j] as ImageTabPage).Text.ToLower().Equals(tabText.ToLower()))
                    {
                        SelectedIndex = j;
                        return true;
                    }
                }
                else
                {
                    if ((_tabs.TabPages[j] as ImageTabPage).Text.Equals(tabText))
                    {
                        SelectedIndex = j;
                        return true;
                    }
                }
            }
            return false;
        }

        Pen _border;
        Color _borderColor;
        public Color BorderLine{
            get{ return _borderColor;}
            set{
                _borderColor = value;

                _border = new Pen(value,1);
            }
        }

        public ImageTabPage SelectedTab
        {
            get
            {
                if (_selectedIndex >= 0) return _tabs.TabPages[_selectedIndex] as ImageTabPage;
                else return null;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            //Graphics g = e.Graphics;
            //g.FillRectangle(_backColor, e.ClipRectangle);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            try
            {
                for (int i = 0; i < _tabs.TabPages.Count; i++)
                {
                    if (E_DrawTabItems == null)
                    {
                        if (_tabs.TabPages[i].Visible)
                        {
                            if (UseImageAsTabBackground) drawTabUseImageAsBack(i, e.Graphics);
                            else drawTabImageAsHeadIcon(i, e.Graphics);
                        }

                    }
                    else
                        E_DrawTabItems(this,
                            new DrawItemEventArgs(
                                e.Graphics,
                                this.Font,
                                GetTabHeadBounds(_tabs.TabPages[i]),
                                i,
                                (i == _selectedIndex) ? DrawItemState.Selected : DrawItemState.None));

                }
                if (_tabs.TabPages.Count == 0)
                {
                    e.Graphics.FillRectangle(Brushes.Transparent, e.ClipRectangle);
                    string msg = "ImageTabPage가 없음";
                    PointF pt = CellFunctions.TextCenterInRact(e.ClipRectangle, e.Graphics, this.Font, msg);
                    e.Graphics.DrawString(msg, this.Font, Brushes.Black, pt);
                    return;
                }
                /*
                if (E_DrawTabItems == null)
                {
                    if (UseImageAsTabBackground) drawTabUseImageAsBack(_selectedIndex, e.Graphics);
                    else drawTabImageAsHeadIcon(_selectedIndex, e.Graphics);
                }
                 */
            }
            catch {
                e.Graphics.FillRectangle(Brushes.Transparent, e.ClipRectangle);
                string msg = "ImageTabPage가 없음";
                PointF pt = CellFunctions.TextCenterInRact(e.ClipRectangle, e.Graphics, this.Font, msg);
                e.Graphics.DrawString(msg, this.Font, Brushes.Black, pt);
                return;
            }

            bool invert = false;
            // Create a transformation given the orientation of the tabs.
            /*
            switch (_tabDock)
            {
                case DockStyle.Bottom:
                    invert = true;
                    e.Graphics.TranslateTransform(Convert.ToSingle(calcWidth), Convert.ToSingle(calcHeight));
                    e.Graphics.RotateTransform(180.0f);
                    break;
                case DockStyle.Left:
                    e.Graphics.TranslateTransform(0, Convert.ToSingle(Height));
                    e.Graphics.RotateTransform(-90.0f);
                    break;
                case DockStyle.Right:
                    e.Graphics.TranslateTransform(Convert.ToSingle(Width), 0);
                    e.Graphics.RotateTransform(90.0f);
                    break;
            }
            
            Matrix m = e.Graphics.Transform;
				
            canScrollLeft = canScrollRight = false;
            if (yaShowScrollButtons == YaScrollButtonStyle.Always || (yaShowScrollButtons == YaScrollButtonStyle.Auto && yaTotalTabSpan > calcWidth))
            {
                if (invert)
                {
                    e.Graphics.ResetTransform();
                    e.Graphics.TranslateTransform(0, _clientRectangle.Height);
                }
                else
                {
                    e.Graphics.Transform = m;
                }
                e.Graphics.FillRectangle(yaInactiveBrush, calcWidth - 2 * _tabsRectangle.Height, 0, 2 * _tabsRectangle.Height, _tabsRectangle.Height);
                e.Graphics.DrawRectangle(Border, calcWidth - 2 * _tabsRectangle.Height, 0, 2 * _tabsRectangle.Height, _tabsRectangle.Height);

                if (((yaShowScrollButtons == YaScrollButtonStyle.Always && yaTotalTabSpan > calcWidth - 2 * Convert.ToInt32(_tabsRectangle.Height)) || (yaShowScrollButtons == YaScrollButtonStyle.Auto && yaTotalTabSpan > calcWidth)) && yaTabLeftDif < yaTotalTabSpan - Convert.ToSingle(yaTabLengths[yaTabLengths.Count - 1]))
                {
                    canScrollLeft = true;
                    e.Graphics.FillPolygon(Border.Brush, leftArrow);
                }
                if (yaTabLeftDif > 0)
                {
                    canScrollRight = true;
                    e.Graphics.FillPolygon(Border.Brush, rightArrow);
                }
                e.Graphics.DrawPolygon(Border, leftArrow);
                e.Graphics.DrawPolygon(Border, rightArrow);
            }
            */
        }

        void drawTabUseImageAsBack(int index, Graphics g)
        {
            //int tabHeight = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
            String text = (_tabs.TabPages[index] as ImageTabPage).Text;
            Image image = (this.Enabled && _tabs.TabPages[index].Enabled && (_tabs.TabPages[index] as ImageTabPage).Selected) ? (_tabs.TabPages[index] as ImageTabPage).ActiveImage : (_tabs.TabPages[index] as ImageTabPage).InActiveImage;
            if (image == null) image = (this.Enabled && _tabs.TabPages[index].Enabled && (_tabs.TabPages[index] as ImageTabPage).Selected) ? _imageList[1] : _imageList[0];

            SizeF textSize = CellFunctions.TextSize(text, g, this.Font);
            int textWidth = (int)(textSize.Width);
            Brush backColor = (this.Enabled && _tabs.TabPages[index].Enabled && (_tabs.TabPages[index] as ImageTabPage).Selected) ? _activeTabItemBackColor : _inactiveTabItemBackColor;
            Brush textColor = Brushes.Black;
            Rectangle rect = GetTabHeadBounds(_tabs.TabPages[index]);

            if ((_tabs.TabPages[index] as ImageTabPage).Selected && this.Enabled && _tabs.TabPages[index].Enabled)
            {
                if ((_tabs.TabPages[index] as ImageTabPage).ActiveTextColor != null) textColor = (_tabs.TabPages[index] as ImageTabPage).ActiveTextColor;
                else textColor = _activeTextColor;
            }
            else
            {
                if ((_tabs.TabPages[index] as ImageTabPage).InactiveTextColor != null) textColor = (_tabs.TabPages[index] as ImageTabPage).InactiveTextColor;
                else textColor = _inactiveTextColor;
            }
            int tabY = (_tabDock == DockStyle.Bottom) ? this.Height - _tabHeight : 0;
            if(image!=null){
                g.FillRectangle(backColor, rect);
                g.DrawImage(image, new Point(rect.X + _tabItemsMargin, CellFunctions.CenterY(_tabHeight, tabY, g, image.Height)));
            }else{
                g.FillRectangle(backColor, rect);
            }

            g.DrawString(text, this.Font, textColor, new PointF(rect.X + _tabItemsMargin, CellFunctions.CenterTextY(_tabHeight, tabY, g, this.Font)));
        }

        void drawTabImageAsHeadIcon(int index, Graphics g)
        {
            int tabHeight = (_tabDock == DockStyle.Bottom) ? 0 : _tabHeight;
           
            String text = (_tabs.TabPages[index] as ImageTabPage).Text;
            Image image = (this.Enabled && _tabs.TabPages[index].Enabled && index == _selectedIndex) ? (_tabs.TabPages[index] as ImageTabPage).ActiveImage : (_tabs.TabPages[index] as ImageTabPage).InActiveImage;
            if (image == null) image = (this.Enabled && _tabs.TabPages[index].Enabled && index == _selectedIndex) ? _imageList[1] : _imageList[0];

            SizeF textSize = CellFunctions.TextSize(text,g, this.Font);
            int textWidth = (int)(textSize.Width);

            Rectangle rect = GetTabHeadBounds(_tabs.TabPages[index]); ;

            Brush backColor = (this.Enabled && _tabs.TabPages[index].Enabled && index == _selectedIndex) ? _activeTabItemBackColor : _inactiveTabItemBackColor;

            Brush textColor = Brushes.Black;
            if (index == _selectedIndex && this.Enabled && _tabs.TabPages[index].Enabled)
            {
                if ((_tabs.TabPages[index] as ImageTabPage).ActiveTextColor != null) textColor = (_tabs.TabPages[index] as ImageTabPage).ActiveTextColor;
                else textColor = _activeTextColor;
            }
            else
            {
                if ((_tabs.TabPages[index] as ImageTabPage).InactiveTextColor != null) textColor = (_tabs.TabPages[index] as ImageTabPage).InactiveTextColor;
                else textColor = _inactiveTextColor;
            }
            
            g.FillRectangle(backColor, rect);
            if (_border != null) g.DrawRectangle(_border, rect);
            int tabY = (_tabDock == DockStyle.Bottom) ? this.Height - _tabHeight : 0;
            if (image != null)
            {
                int imageY = CellFunctions.CenterY(_tabHeight, tabY, g, image.Height);
                g.DrawImage(image, rect.X + _tabItemsMargin, imageY);
                int textY = CellFunctions.CenterY(_tabHeight, tabY, g, (int)textSize.Height);
                g.DrawString(text, this.Font, textColor, rect.X+ (image.Width + _tabItemsMargin * 2), textY);
            }
            else
            {
                int textY = CellFunctions.CenterY(_tabHeight, tabY, g, (int)textSize.Height);
                g.DrawString(text, this.Font, textColor, rect.X+ (_tabItemsMargin * 1), textY);
            }
            
        }

        [Browsable(true)]
        [EditorBrowsable]
        public ImageTabControl.ControlCollection TabPages
        {
            get { return _tabs; }
        }
        /*
        public List<ImageTabPage> TabPages
        {
            get
            {
                return (this.Controls as ImageTabControl.ControlCollection).TabPages;
            }
        }
        */

        public new ImageTabControl.ControlCollection Controls
        {
            get { return TabPages; }
        }

        int _tabItemsMargin = 2;
        public int TabItemsMargin
        {
            get { return _tabItemsMargin; }
            set { _tabItemsMargin = value; }
        }

        public Rectangle GetTabHeadBounds(ImageTabPage tabPage)
        {
            int x = _tabItemsMargin;
            int width = 0;
            for (int i = 0; i < _tabs.Count; i++)
            {
                width = 0;
                if (_tabs.TabPages[i].Visible == false) continue;
                if ((_tabs.TabPages[i] as ImageTabPage).ActiveImage != null) width += (_tabs.TabPages[i] as ImageTabPage).ActiveImage.Width + _tabItemsMargin;
                else if (i == _selectedIndex && this._imageList[1] != null) width += _imageList[1].Width + _tabItemsMargin;
                else if (i != _selectedIndex && this._imageList[0] != null) width += _imageList[0].Width + _tabItemsMargin;
                else if (UseImageAsTabBackground) throw new Exception("UseImageAsTabBackground가 true이므로 ImageTab이나 ImageTabControl에 image가 지정되어야 합니다.");
                

                if (UseImageAsTabBackground == false) //이미지가 배경으로 사용되지 않을 시... 글자의 길이를 사용함.
                {
                    String text = (_tabs.TabPages[i] as ImageTabPage).Text;
                    if (text != null && text.Length>0)
                    {
                        Graphics g = CreateGraphics();
                        //SizeF size = CellFunctions.TextSize((_tabs.TabPages[i] as ImageTabPage).Text, g, this.Font);
                        width += (int)(g.MeasureString((_tabs.TabPages[i] as ImageTabPage).Text, this.Font).Width + _tabItemsMargin);// (int)size.Width + _tabItemsMargin;
                    }
                    
                }
                if(_tabs.TabPages[i].Equals(tabPage)) break;
                x += width;
            }
            if (_tabDock == DockStyle.Bottom)
            {
                return new Rectangle(x, this.Height-_tabHeight, width, _tabHeight);
            }
            else
            {
                return new Rectangle(x, 0, width, _tabHeight);
            }
        }

        /// <summary>
        /// Overriden from <see cref="Control"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="YaTabControl.ControlCollection"/>.
        /// </returns>
        protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
        {
            return new ImageTabControl.ControlCollection(this);
        }

        protected override void OnControlAdded(ControlEventArgs cea)
        {
            base.OnControlAdded(cea);
            cea.Control.Visible = false;
            //ImageTabPage tab = cea.Control as ImageTabPage;
            //if (tab != null) AddTab(tab);

            
                this.SelectedIndex = this.Controls.Count-1;
                SelectedIndex = this.Controls.Count - 1; 
                SelectedTab.Visible = true;
            
            cea.Control.TextChanged += ChildTextChangeEventHandler;
            ImageTabPage tab = cea.Control as ImageTabPage;
            tab.Move += new EventHandler(tab_Move);
            if (tab != null)
            {
                SetTabSize(ref tab);
                CalculateLastVisibleTabIndex();
            }
            InU();
        }

        void tab_Move(object sender, EventArgs e)
        {
            ImageTabPage page = sender as ImageTabPage;
            SetTabSize(ref page);
        }

        /// <summary>
        /// Monitors when child <see cref="YaTabPage"/>s have their
        /// <see cref="YaTabPage.Text"/> property changed.
        /// </summary>
        /// <param name="sender">A <see cref="YaTabPage"/>.</param>
        /// <param name="ea">Some <see cref="EventArgs"/>.</param>
        internal void ChildTabTextChanged(object sender, EventArgs ea)
        {
            CalculateTabLengths();
            InU();
        }

        protected override void OnControlRemoved(ControlEventArgs cea)
        {
            cea.Control.TextChanged -= ChildTextChangeEventHandler;
            base.OnControlRemoved(cea);
            //ImageTabPage tab = cea.Control as ImageTabPage;
            //if (tab != null) RemoveTab(tab);

            if (Controls.Count > 0)
            {
                this.SelectedIndex = Controls.Count - 1;
                SelectedIndex = Controls.Count - 1;
                SelectedTab.Visible = false;
            }
            else
            {
                this.SelectedIndex = -1;
                SelectedIndex = -1;
            }
            CalculateTabLengths();
            CalculateLastVisibleTabIndex();
            
            InU();
        }

        internal void CalculateTabLengths()
        {
            yaTotalTabSpan = 0.0f;
            yaTabLengths.Clear();
            Graphics g = CreateGraphics();
            float f = 0.0f;
            ImageTabPage ytp;

            for (int i = 0; i < _tabs.TabPages.Count; i++)
            {
                f = GetTabHeadBounds(_tabs.TabPages[i]).Width;
                yaTabLengths.Add(f);
                yaTotalTabSpan += f;
                /*
                f = g.MeasureString(Controls[i].Text, yaBoldTabFont).Width + 4.0f * Convert.ToSingle(yaMargin);
                ytp = Controls[i] as ImageTabPage;
                if (ytp != null && images != null && ytp.ImageIndex > -1 && ytp.ImageIndex < images.Images.Count && images.Images[ytp.ImageIndex] != null)
                {
                    f += images.Images[ytp.ImageIndex].Width + 2.0f * yaMargin;
                }
                else if (ytp != null && images != null && defaultImageIndex > -1 && defaultImageIndex < images.Images.Count && images.Images[defaultImageIndex] != null)
                {
                    f += images.Images[defaultImageIndex].Width + 2.0f * yaMargin;
                }
                yaTabLengths.Add(f);
                yaTotalTabSpan += f;
                 */
            }
        }
/*

        internal void AddTab(ImageTabPage tab)
        {
            //tab.Panel.SetBounds(this.Location.X, this.Location.Y + _tabHeight, this.Width, this.Height - _tabHeight);

            tab.SetBounds(0,  _tabHeight, this.Width, this.Height - _tabHeight);

                //base.Controls.Add(tab);
                //tab.BringToFront();
                //_sContainer.Panel2.Controls.Add(tab.Panel);
                yaTabLengths.Add(GetTabHeadBounds(tab.Index).Width);
                InU();
            
            tab.BackColor = Color.White;
            SelectedIndex = tab.Index;
        }

        internal void RemoveTab(ImageTabPage tab)
        {
            //if (Parent != null) base.Controls.Remove(tab);
            if (_tabs.TabPages.Count <= _selectedIndex) SelectedIndex = _tabs.TabPages.Count - 1;
            
            if (_tabs.TabPages.Count == 0) _selectedIndex = -1;
        }
        */

        #region Private Members

        /// <summary>
        /// The index to use as the default image for the tabs.
        /// </summary>
        private int defaultImageIndex;

        /// <summary>
        /// The <see cref="ImageList"/> used to draw the images in
        /// the tabs.
        /// </summary>
        private ImageList images;

        /// <summary>
        /// A flag to indicate if the tabs can scroll left.
        /// </summary>
        private bool canScrollLeft;

        /// <summary>
        /// A flag to indicate if the tabs can scroll right.
        /// </summary>
        private bool canScrollRight;

        /// <summary>
        /// A flag to indicate if scroll buttons should get drawn.
        /// </summary>
        private YaScrollButtonStyle yaShowScrollButtons;

        /// <summary>
        /// The array of floats whose each entry measures a tab's width.
        /// </summary>
        private ArrayList yaTabLengths;

        /// <summary>
        /// The sum of the lengths of all the tabs.
        /// </summary>
        private float yaTotalTabSpan;

        /// <summary>
        /// The margin around the visible <see cref="ImageTabPage"/>.
        /// </summary>
        private int yaMargin;

        /// <summary>
        /// The span of the tabs. Used as the height/width of the
        /// tabs, depending on the orientation.
        /// </summary>
        private float yaTabSpan;

        /// <summary>
        /// The amount that the tabs have been scrolled to the left.
        /// </summary>
        private float yaTabLeftDif;

        /// <summary>
        /// The <see cref="Point"/>s that define the left scroll arrow.
        /// </summary>
        private Point[] leftArrow;

        /// <summary>
        /// The <see cref="Point"/>s that define the right scroll arrow.
        /// </summary>
        private Point[] rightArrow;

        /// <summary>
        /// The index of the last visible tab.
        /// </summary>
        private int yaLastVisibleTabIndex;

        /// <summary>
        /// The length from the left of the tab control
        /// to the left of the last visible tab.
        /// </summary>
        private float yaLastVisibleTabLeft;

        /// <summary>
        /// The brush used to draw the strings in the tabs.
        /// </summary>
        private Brush yaForeBrush;

        /// <summary>
        /// The color of the active tab and area.
        /// </summary>
        private Color yaActiveColor;

        /// <summary>
        /// The brush used to color the active-colored area.
        /// </summary>
        private Brush yaActiveBrush;

        /// <summary>
        /// The color of the inactive areas.
        /// </summary>
        private Color yaInactiveColor;

        /// <summary>
        /// The brush used to color the inactive-colored area.
        /// </summary>
        private Brush yaInactiveBrush;

        /// <summary>
        /// The pen used to draw the highlight lines.
        /// </summary>
        private Pen yaHighlightPen;

        /// <summary>
        /// The pen used to draw the shadow lines.
        /// </summary>
        private Pen yaShadowPen;

        /// <summary>
        /// The pen used to draw the border.
        /// </summary>
        private Pen Border;

        /// <summary>
        /// The side on which the tabs get docked.
        /// </summary>
        private DockStyle _tabDock = DockStyle.Top;

        /// <summary>
        /// The rectangle in which the tabs get drawn.
        /// </summary>
        private Rectangle _tabsRectangle;

        /// <summary>
        /// The rectangle in which the client gets drawn.
        /// </summary>
        private Rectangle _clientRectangle;

        /// <summary>
        /// The rectangle in which the currently selected
        /// <see cref="ImageTabPage"/> gets drawn oriented as
        /// if the tabs were docked to the top of the control.
        /// </summary>
        private Rectangle _displayRectangle;

        /// <summary>
        /// The rectangle transformed for the <see cref="DisplayRectangle"/>
        /// property to return.
        /// </summary>
        private Rectangle _transformedDisplayRectangle;

        /// <summary>
        /// The height used to calculate the rectangles.
        /// </summary>
        private int calcHeight;

        /// <summary>
        /// The width used to calculate the rectangles.
        /// </summary>
        private int calcWidth;

        /// <summary>
        /// The regular font used to draw the strings in the tabs.
        /// </summary>
        private Font yaTabFont;

        /// <summary>
        /// The bold font used to draw the strings in the active tab.
        /// </summary>
        private Font yaBoldTabFont;

        
        /// <summary>
        /// Used to monitor the text changing of a <see cref="ImageTabPage" />.
        /// </summary>
        private EventHandler ChildTextChangeEventHandler;

        /// <summary>
        /// Used to monitor if a person has elected to scroll the tabs.
        /// </summary>
        internal bool yaKeepScrolling;

        #endregion



    }

    public class ImageTabSelectedArgs:EventArgs
    {
        public ImageTabPage TabPage;
        public int SelectedIndex;

        public ImageTabSelectedArgs(ImageTabPage page, int selectedIndex)
        {
            TabPage = page;
            SelectedIndex = selectedIndex;
        }
    }
    public delegate void ImageTabSelectedEvent(object sender, ImageTabSelectedArgs e);

}
