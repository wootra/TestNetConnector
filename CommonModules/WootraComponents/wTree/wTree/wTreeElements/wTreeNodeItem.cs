using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WootraComs.wTreeElements
{
    public class wTreeNodeItem
    {
        internal event wTreeNodeItemChanged E_ItemChanged;

        wTreeNodeItemTypes _type;

        /// <summary>
        /// item의 타입이 무엇인지 나타낸다.
        /// </summary>
        public wTreeNodeItemTypes ItemType
        {
            get { return _type; }
        }

        internal Image _image = null;
        public wTreeNodeItem(Image image)
        {
            _type = wTreeNodeItemTypes.Image;
            _image = image;
        }

        internal string _text;
        public wTreeNodeItem(String text)
        {
            _type = wTreeNodeItemTypes.Text;
            _text = text;
        }

        internal bool? _isChecked;
        public wTreeNodeItem(bool? check)
        {
            _type = wTreeNodeItemTypes.CheckBox;
            _isChecked = check;
        }

        public void ChangeItem(Image image)
        {
            if (_type != wTreeNodeItemTypes.Image || _image.Equals(image) == false)
            {
                _type = wTreeNodeItemTypes.Image;
                _image = image;
                if (E_ItemChanged != null) E_ItemChanged(this);
            }
        }

        public void ChangeItem(string text)
        {
            if (_type != wTreeNodeItemTypes.Text || _text.Equals(text))
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
    }
}
