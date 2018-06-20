using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridVariousTypeCellInfo:EasyGridCellInfo
    {
        
        #region Constructors
        public EasyGridVariousTypeCellInfo():base()
        {
        }
        //public enum CheckBoxTypes{ ImageCheckBox, CheckBox};
        public EasyGridVariousTypeCellInfo(bool? isChecked, CheckBoxTypes checkBoxType)
            : base()
        {
            if(checkBoxType == CheckBoxTypes.ImageCheckBox) ItemType = ItemTypes.ImageCheckBox;
            else ItemType = ItemTypes.CheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
        }
        //public enum ButtonTypes{Button, CloseButton};
        public EasyGridVariousTypeCellInfo(String text, ButtonTypes itemType = ButtonTypes.Button)
            : base()
        {
            if(itemType == ButtonTypes.Button) this.ItemType = ItemTypes.Button;
            else this.ItemType = ItemTypes.CloseButton;
            this.Text = text;
        }
        public EasyGridVariousTypeCellInfo(String Text, bool editable, String Tooltip = "")
            : base()
        {
            this.ItemType = ItemTypes.TextBox;
            IsEditable = editable;
            this.Text = Text;
            if (Tooltip.Length > 0) this.ToolTip = Tooltip;
        }

       
        public EasyGridVariousTypeCellInfo(String Text, bool editable, String Tooltip, TextBoxModes modes)
            : base()
        {
            if (modes == TextBoxModes.TextBox)
            {
                this.ItemType = ItemTypes.TextBox;
            }
            else
            {
                this.ItemType = ItemTypes.FileOpenBox;
            }
            IsEditable = editable;
            this.Text = Text;
            if (Tooltip.Length > 0) this.ToolTip = Tooltip;
        }


        //public enum SelectionTypes{ ComboBox, RadioButton};
        public EasyGridVariousTypeCellInfo(ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
            : base()
        {
            if(itemtype == SelectionTypes.ComboBox) this.ItemType = ItemTypes.ComboBox;
            else this.ItemType = ItemTypes.RadioButton;
            this.Items = items;
            this.SelectedIndex = selectedIndex;
            if(selectedIndex>=0 && items.Count>selectedIndex) this.SelectedText = items.ElementAt(selectedIndex);
        }

        public EasyGridVariousTypeCellInfo(ICollection<String> items, String selectedText)
            : base()
        {
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        public EasyGridVariousTypeCellInfo(Dictionary<String, String> KeyValue, Dictionary<String, Brush> KeyColor)
            : base()
        {
            this.ItemType = ItemTypes.KeyValue;
            this.KeyValue = KeyValue;
            this.KeyColor = KeyColor;
        }
        /*
        public EasyGridVariousTypeCellInfo(Dictionary<String, Brush> KeyColor)
            : base()
        {
            this.ItemType = ItemTypes.KeyColor;
            this.KeyColor = KeyColor;
        }
        */

        public EasyGridVariousTypeCellInfo(int selectedIndex, ImageLayout mode, int fixedWidth=0, int fixedHeight=0)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }

        public EasyGridVariousTypeCellInfo(int selectedIndex, ImageLayout mode, String text, int fixedWidth = 0, int fixedHeight = 0)
            : base()
        {
            this.ItemType = ItemTypes.ImageButton;
            this.SelectedIndex = selectedIndex;
            this.Text = text;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }

        public EasyGridVariousTypeCellInfo(ICollection<Image> images, int selectedIndex, ImageLayout mode = ImageLayout.Zoom, int fixedWidth = 0, int fixedHeight = 0)
            : base()
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }
        
        public EasyGridVariousTypeCellInfo(ICollection<Image> images, int selectedIndex, String text, ImageLayout mode = ImageLayout.Zoom, int fixedWidth = 0, int fixedHeight = 0)
            : base()
        {
            this.ItemType = ItemTypes.ImageButton;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
            this.Text = text;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }

        public EasyGridVariousTypeCellInfo(ICollection<Image> images, bool? isChecked)
            : base()
        {
            this.ItemType = ItemTypes.ImageCheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
            this.Images = images;
        }

        public EasyGridVariousTypeCellInfo(Image image)
        {
            this.ItemType = ItemTypes.Image;
            this.Images = new Image[]{image};
            this.SelectedIndex = 0;
        }

        public EasyGridVariousTypeCellInfo(ICollection<String> Items, ICollection<int> selected = null)
            : base()
        {
            this.ItemType = ItemTypes.CheckBoxGroup;
            this.Items = Items;
            this.SelectedIndices = selected;
        }
        #endregion

    }
}
