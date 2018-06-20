using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class wTreeScroll
    {
        wTree _ownerTree;
        public event ScrollEventHandler E_VerticalScrollChanged;
        public event ScrollEventHandler E_HorizontalScrollChanged;

        internal VScrollBar Scroll_Vertical;
        internal HScrollBar Scroll_Horizontal;

        internal wTreeScroll(wTree ownerTree, VScrollBar scroll_Vertical, HScrollBar scroll_Horizontal)
        {
            _ownerTree = ownerTree;
            this.Scroll_Horizontal = scroll_Horizontal;
            this.Scroll_Vertical = scroll_Vertical;
            Scroll_Vertical.Visible = false;
            Scroll_Vertical.Scroll += Scroll_Vertical_Scroll;
            Scroll_Horizontal.Visible = false;
            Scroll_Horizontal.Scroll += Scroll_Horizontal_Scroll;
            Scroll_Vertical.Maximum = 0;
            Scroll_Vertical.Minimum = 0;
            MouseWheelHandler.Add(_ownerTree, wTree_MouseWheel);
            
        }

        wTreeMouseEventsHandler MouseEventsHandler { get { return _ownerTree.wMouseEventsHandler; } }

        
        wTreeSelections SelectionHandler { get { return _ownerTree.wSelectionHandler; } }

        DrawHandler DrawHandler { get { return _ownerTree.wDrawHandler; } }

        EditorHandlerClass EditorHandler { get { return _ownerTree.wEditorHandler; } }

        public bool ScrollVerticalVisible
        {
            get { return Scroll_Vertical.Visible; }
            set { Scroll_Vertical.Visible = value; }
        }

        public bool ScrollHorizontalVisible
        {
            get { return Scroll_Horizontal.Visible; }
            set { Scroll_Horizontal.Visible = value; }
        }

        public bool VerticalScrollVisible
        {
            get { return Scroll_Vertical.Visible; }
        }

        public bool HorizontalScrollVisible
        {
            get { return Scroll_Horizontal.Visible; }
        }
        int _vScrollOffset = 0;
        /// <summary>
        /// 세로방향으로 어느정도 스크롤 되었는지 나타냄..
        /// </summary>
        public int VScrollOffset
        {
            get { return _vScrollOffset; }
            set
            {
                _vScrollOffset = value;
                DrawHandler.ReDrawTreeForScroll();
            }
        }


        int _hScrollOffset = 0;

        /// <summary>
        /// 가로방향으로 스크롤 되었을 때..
        /// </summary>
        public int HScrollOffset
        {
            get { return _hScrollOffset; }
        }

        void Scroll_Horizontal_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue != _hScrollOffset)
            {
                _hScrollOffset = e.NewValue;
                _ownerTree.wDrawHandler.ReDrawTreeForScroll();
                if (E_HorizontalScrollChanged != null) E_HorizontalScrollChanged(this, e);
            }
        }

        void wTree_MouseWheel(MouseEventArgs e)
        {
            try
            {
                int newValue;

                if (e.Delta < 0)
                {
                    newValue = _vScrollOffset + 18;
                    if (newValue > Scroll_Vertical.Maximum) newValue = Scroll_Vertical.Maximum;

                }
                else
                {
                    newValue = _vScrollOffset - 18;
                    if (newValue < Scroll_Vertical.Minimum) newValue = Scroll_Vertical.Minimum;
                }

                if (newValue != _vScrollOffset)
                {
                    Scroll_Vertical.Value = newValue;
                    _vScrollOffset = newValue;
                    _ownerTree.wDrawHandler.ReDrawTreeForScroll();
                    if (E_VerticalScrollChanged != null) E_VerticalScrollChanged(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, newValue));
                }

            }
            catch { }
        }

        //int _vScrollOffset = 0;
        void Scroll_Vertical_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue != _vScrollOffset)
            {
                _vScrollOffset = e.NewValue;
                _ownerTree.wDrawHandler.ReDrawTreeForScroll();

                if (E_VerticalScrollChanged != null) E_VerticalScrollChanged(this, e);
            }

        }

        internal void SetScrollBars(Size totalSize, Size boundSize)
        {
            //Size totalSize = new Size(tempTotalSize.Width + _ownerTree.Margin.Left + _ownerTree.Margin.Right, tempTotalSize.Height + _ownerTree.Margin.Top + _ownerTree.Margin.Bottom);
            if ((totalSize.Height) > (boundSize.Height))
            {
                int scrollHeight = totalSize.Height - (boundSize.Height);
                _ownerTree.Scroll_Vertical.Maximum = scrollHeight + 10;
                _ownerTree.Scroll_Vertical.Minimum = 0;

                _ownerTree.Scroll_Vertical.LargeChange = 10;
                _ownerTree.Scroll_Vertical.SmallChange = 10;
                
                if (_ownerTree.Scroll_Vertical.Visible == false)
                {
                    _ownerTree.Scroll_Vertical.Visible = true;
                    _ownerTree.Scroll_Vertical.Value = 0;
                    //_ownerTree.DrawHandler.ResizeTempDrawBuffer();
                }
            }
            else
            {
                _ownerTree.Scroll_Vertical.Visible = false;
                _vScrollOffset = 0;
                _ownerTree.Scroll_Vertical.Maximum = 0;
                _ownerTree.Scroll_Vertical.Minimum = 0;
            }

            if ((totalSize.Width) > (boundSize.Width))
            {
                int scrollWidth = totalSize.Width - (boundSize.Width);
                Scroll_Horizontal.Maximum = scrollWidth + 10;
                Scroll_Horizontal.Minimum = 0;

                Scroll_Horizontal.LargeChange = 10;
                Scroll_Horizontal.SmallChange = 10;
                if (Scroll_Horizontal.Visible == false)
                {
                    Scroll_Horizontal.Visible = true;
                    Scroll_Horizontal.Value = 0;
                    _ownerTree.wDrawHandler.ResizeTempDrawBuffer();
                }
            }
            else
            {
                Scroll_Horizontal.Visible = false;
                _hScrollOffset = 0;
            }
        }
    }
}
