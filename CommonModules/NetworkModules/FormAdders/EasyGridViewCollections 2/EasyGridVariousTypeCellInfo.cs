using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridVariousTypeCellInfo
    {

        public ItemTypes ItemType = ItemTypes.Various;//이 설정은 무시된다.
        public ICollection<String> Items = null;
        public ICollection<int> SelectedIndices = null;
        public Dictionary<String, String> KeyValue = null;
        public Dictionary<String, Color> KeyColor = null;
        public int SelectedIndex = -1;
        public String SelectedText = null;
        public String Text = null;
        public bool? Checked = false;
        public int CheckInt = 0;
        public ICollection<Image> Images = null;
        public bool IsEditable = false;
        
        #region Constructors
        public EasyGridVariousTypeCellInfo()
        {
        }
        public enum CheckBoxTypes{ ImageCheckBox, CheckBox};
        public EasyGridVariousTypeCellInfo(bool? isChecked, CheckBoxTypes checkBoxType)
        {
            if(checkBoxType == CheckBoxTypes.ImageCheckBox) ItemType = ItemTypes.ImageCheckBox;
            else ItemType = ItemTypes.CheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
        }
        public enum ButtonTypes{Button, CloseButton};
        public EasyGridVariousTypeCellInfo(String text, ButtonTypes itemType = ButtonTypes.Button )
        {
            if(itemType == ButtonTypes.Button) this.ItemType = ItemTypes.Button;
            else this.ItemType = ItemTypes.CloseButton;
            this.Text = text;
        }
        public EasyGridVariousTypeCellInfo(String Text, bool editable)
        {
            this.ItemType = ItemTypes.TextBox;
            IsEditable = editable;
            this.Text = Text;
        }

        public enum SelectionTypes{ ComboBox, RadioButton};
        public EasyGridVariousTypeCellInfo(ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
        {
            if(itemtype == SelectionTypes.ComboBox) this.ItemType = ItemTypes.ComboBox;
            else this.ItemType = ItemTypes.RadioButton;
            this.Items = items;
            this.SelectedIndex = selectedIndex;
            if(selectedIndex>=0 && items.Count>selectedIndex) this.SelectedText = items.ElementAt(selectedIndex);
        }
        
        public EasyGridVariousTypeCellInfo(ICollection<String> items, String selectedText)
        {
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        public EasyGridVariousTypeCellInfo(Dictionary<String, String> KeyValue)
        {
            this.ItemType = ItemTypes.KeyValue;
            this.KeyValue = KeyValue;
        }

        public EasyGridVariousTypeCellInfo(Dictionary<String, Color> KeyColor)
        {
            this.ItemType = ItemTypes.KeyColor;
            this.KeyColor = KeyColor;
        }

        
        public EasyGridVariousTypeCellInfo(ICollection<Image> images, int selectedIndex)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
        }

        public EasyGridVariousTypeCellInfo(ICollection<Image> images, bool? isChecked)
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

        public EasyGridVariousTypeCellInfo(ICollection<String> Items, ICollection<int> selected=null)
        {
            this.ItemType = ItemTypes.CheckBoxGroup;
            this.Items = Items;
            this.SelectedIndices = selected;
        }
        #endregion

        #region methods
        public void SetCheckBoxInfo(bool? isChecked, CheckBoxTypes checkBoxType = CheckBoxTypes.ImageCheckBox)
        {
            if (checkBoxType == CheckBoxTypes.ImageCheckBox) ItemType = ItemTypes.ImageCheckBox;
            else ItemType = ItemTypes.CheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
        }
        public void SetButtonInfo(String text, ButtonTypes itemType = ButtonTypes.Button)
        {
            if (itemType == ButtonTypes.Button) this.ItemType = ItemTypes.Button;
            else this.ItemType = ItemTypes.CloseButton;
            this.Text = text;
        }
        public void SetTextBoxInfo(String Text, bool editable)
        {
            this.ItemType = ItemTypes.TextBox;
            IsEditable = editable;
            this.Text = Text;
        }

        public void SetComboOrRadioInfo(ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
        {
            if (itemtype == SelectionTypes.ComboBox) this.ItemType = ItemTypes.ComboBox;
            else this.ItemType = ItemTypes.RadioButton;
            this.Items = items;
            this.SelectedIndex = selectedIndex;
            if (selectedIndex >= 0 && items.Count > selectedIndex) this.SelectedText = items.ElementAt(selectedIndex);
        }

        public void SetComboInfo(ICollection<String> items, String selectedText)
        {
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        public void SetImageInfo(ICollection<Image> images, int selectedIndex)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
        }
        
        public void SetImageCheckBoxInfo(ICollection<Image> images, bool? isChecked)
        {
            this.ItemType = ItemTypes.ImageCheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
            this.Images = images;
        }

        public void SetImageInfo(Image image)
        {
            this.ItemType = ItemTypes.Image;
            this.Images = new Image[] { image };
            this.SelectedIndex = 0;
        }

        public void SetCheckBoxGroupInfo(ICollection<String> Items, ICollection<int> selected = null)
        {
            this.ItemType = ItemTypes.CheckBoxGroup;
            this.Items = Items;
            this.SelectedIndices = selected;
        }
#endregion
    }
}
