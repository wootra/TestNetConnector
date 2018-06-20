using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormAdders;
using System.Windows.Forms;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridImageCheckBoxCell : DataGridViewImageCell, IEasyGridCell
    {
        DataGridView _parent;
        
        public EasyGridImageCheckBoxCell(DataGridView parent)
            : base()
        {
            _parent = parent;
        }

        ICollection<Image> _images = null;
        public ICollection<Image> Images
        {
            get { return _images; }
            set { _images = value; }
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
            get { return ItemTypes.ImageCheckBox; }
        }

        public bool? IsChecked
        {
            get
            {
                if (_value == 0) return false;
                else if (_value == 1) return true;
                else return null;
            }
            set
            {
                if (value == true) _value = 1;
                else if (value == false) _value = 0;
                else _value = 2;
                Value = _value;
            }
        }

        int _value = 0;
        public new object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value is Image)
                {
                    base.Value = value;
                }
                else if (value is int)
                {
                    int intValue = (int)value;
                    if (_images.Count > intValue && intValue >= 0)
                    {
                        base.Value = _images.ElementAt((int)value);
                    }
                    else if (_images.Count == 0)
                    {

                    }
                    else
                    {
                        base.Value = _images.ElementAt(0);
                    }
                    _value = intValue;
                }
                else if (value is bool?)
                {
                    IsChecked = (bool?)value;
                }
                else
                {
                    throw new Exception("형식이 맞지 않습니다. System.Drawing.Image 나 int 또는 bool? 형식을 넣어주세요");
                }
            }

        }

        public new object Tag
        {
            get
            {
                return _value;
            }
            set
            {
                if (value is int) _value = (int)value;
            }
        }

        internal void RePaint()
        {
            if (_parent != null && this.RowIndex >= 0 && this.ColumnIndex >= 0) _parent.InvalidateCell(this.ColumnIndex, this.RowIndex);
        }
    }
}
