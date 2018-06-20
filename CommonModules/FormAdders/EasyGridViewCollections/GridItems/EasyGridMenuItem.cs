using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public class EasyGridMenuItem : MenuItem
    {

        public EasyGridMenuItem(String text) : base(text) { }
        public EasyGridMenuItem() : base() { }
        public EasyGridMenuItem(String text, EventHandler onClick) : base(text, onClick) { }

        public EasyGridMenuItem(String text, EasyGridMenuItem[] items)
            : base(text)
        {
            for (int i = 0; i < items.Length; i++)
            {
                this.AddItem(items[i]);
                //this.MenuItems.Add(items[i]);
                //items[i].setDepth(_depth+1);

            }

        }
        public EasyGridMenuItem(String text, String[] children)
            : base(text)
        {
            AddItem(children);
        }


        public EasyGridMenuItem(String text, EventHandler onClick, String[] children, bool setEventRecursive)
            : base(text, onClick)
        {
            AddItem(children, setEventRecursive);
        }

        public EasyGridMenuItem(String text, EventHandler onClick, EasyGridMenuItem[] children, bool EventRecursive)
            : base(text, onClick)
        {
            AddItem(children);
            if (EventRecursive)
            {
                for (int i = 0; i < MenuItems.Count; i++)
                {
                    (MenuItems[i] as EasyGridMenuItem).Click = _clickEvent;
                }
            }

        }


        /// <summary>
        /// 새로운 EasyGridMenuItem을 아래에 추가한다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>새로 추가된 item의 depth</returns>
        public int AddItem(EasyGridMenuItem item)
        {
            this.MenuItems.Add(item);
            item.setParent(this);
            item.setDepth(_depth + 1);
            return item.Depth;
        }

        public int AddItem(EasyGridMenuItem[] items, bool setEvent = false)
        {
            MenuItems.AddRange(items);
            for (int i = 0; i < items.Length; i++)
            {
                items[i].setParent(this);
                items[i].setDepth(_depth + 1);
                if (setEvent) items[i].Click = _clickEvent;
            }
            return _depth + 1;
        }

        /// <summary>
        /// 새로운 EasyGridMenuItem들을 아래에 추가한다.
        /// </summary>
        /// <param name="items">자식 item들의 text</param>
        /// <param name="setEvent">부모와 동일한 event를 추가하려면 true</param>
        /// <returns></returns>
        public new int AddItem(String[] items, bool setEvent = false)
        {
            for (int i = 0; i < items.Length; i++)
            {
                EasyGridMenuItem item = new EasyGridMenuItem(items[i]);
                item.setDepth(_depth + 1);
                item.setParent(this);
                if (setEvent) item.Click = _clickEvent;
                this.MenuItems.Add(item);
            }
            return _depth + 1;
        }

        int _depth = 0;
        public int Depth
        {
            get
            {
                return _depth;
            }
        }
        internal void setDepth(int depth)
        {
            _depth = depth;
        }


        EasyGridMenuItem _parent;
        public new EasyGridMenuItem Parent
        {
            get
            {
                return _parent;
            }
        }
        internal void setParent(EasyGridMenuItem parent)
        {
            _parent = parent;
        }


        EventHandler _clickEvent = null;
        public new EventHandler Click
        {
            get
            {
                return _clickEvent;
            }
            set
            {
                _clickEvent = value;
                try
                {
                    base.Click -= _clickEvent;
                }
                catch { }
                if (value != null) base.Click += value;
            }
        }

        public void SetClickedEvent(EventHandler onClick, bool setRecursive = false)
        {
            Click = onClick;
            if (setRecursive && MenuItems.Count > 0)
            {
                foreach (EasyGridMenuItem item in MenuItems)
                {
                    item.Click = onClick;
                }
            }
        }


    }
}
