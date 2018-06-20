using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridCellInfo
    {

        public ItemTypes ItemType = ItemTypes.Various;//이 설정은 무시된다.
        public ICollection<String> Items = null;
        public ICollection<int> SelectedIndices = null;
        public Dictionary<String, String> KeyValue = null;
        public Dictionary<String, Brush> KeyColor = null;
        public int SelectedIndex = -1;
        public String SelectedText = null;
        public String Text = null;
        public String ToolTip = null;
        public bool? Checked = false;
        public int CheckInt = 0;
        public ICollection<Image> Images = null;
        public bool IsEditable = false;
        public TextAlignModes TextAlignMode = TextAlignModes.None;
        public ImageLayout ImageLayout =  ImageLayout.Stretch;
        
        /// <summary>
        /// 0이면 ImageSizeMode가 None인 것과 같다.
        /// </summary>
        public int ImageFixedWidth=0;

        /// <summary>
        /// 0이면 ImageSizeMode가 None인 것과 같다.
        /// </summary>
        public int ImageFixedHeight = 0;

        #region Constructors
        public EasyGridCellInfo()
        {
        }
        public enum CheckBoxTypes{ ImageCheckBox, CheckBox};
        public EasyGridCellInfo(bool? isChecked, CheckBoxTypes checkBoxType)
        {
            if(checkBoxType == CheckBoxTypes.ImageCheckBox) ItemType = ItemTypes.ImageCheckBox;
            else ItemType = ItemTypes.CheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
        }
        public enum ButtonTypes{Button, CloseButton};
        public EasyGridCellInfo(String text, ButtonTypes itemType = ButtonTypes.Button )
        {
            if(itemType == ButtonTypes.Button) this.ItemType = ItemTypes.Button;
            else this.ItemType = ItemTypes.CloseButton;
            this.Text = text;
        }

        public enum TextBoxModes { TextBox, FileOpenBox };
        public EasyGridCellInfo(String Text, bool editable, TextAlignModes alignMode= TextAlignModes.None, TextBoxModes mode= TextBoxModes.TextBox)
        {
            if (mode == TextBoxModes.TextBox)
            {
                this.ItemType = ItemTypes.TextBox;
            }
            else
            {
                this.ItemType = ItemTypes.FileOpenBox;
            }
            IsEditable = editable;
            this.Text = Text;
            this.TextAlignMode = alignMode;
        }

        public enum SelectionTypes{ ComboBox, RadioButton};
        public EasyGridCellInfo(ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
        {
            if(itemtype == SelectionTypes.ComboBox) this.ItemType = ItemTypes.ComboBox;
            else this.ItemType = ItemTypes.RadioButton;
            this.Items = items;
            if (selectedIndex < 0)
            {
                if (items!=null && items.Count > 0)
                {
                    this.SelectedText = items.ElementAt(0);
                    this.SelectedIndex = 0;
                }
            }
            else
            {
                this.SelectedIndex = selectedIndex;
                if (selectedIndex >= 0 && items.Count > selectedIndex) this.SelectedText = items.ElementAt(selectedIndex);
            }
        }
        
        public EasyGridCellInfo(ICollection<String> items, String selectedText)
        {
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        public EasyGridCellInfo(Dictionary<String, String> KeyValue, Dictionary<String, Brush> KeyColor)
        {
            this.ItemType = ItemTypes.KeyValue;
            this.KeyValue = KeyValue;
            this.KeyColor = KeyColor;
        }

        public EasyGridCellInfo(int selectedIndex, ImageLayout mode, int fixedWidth = 0, int fixedHeight = 0)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }

        public EasyGridCellInfo(ICollection<Image> images, int selectedIndex, ImageLayout mode = ImageLayout.Stretch, int fixedWidth = 0, int fixedHeight = 0)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }
        public EasyGridCellInfo(ICollection<Image> images, int selectedIndex, String Text, ImageLayout mode = ImageLayout.Stretch, int fixedWidth = 0, int fixedHeight = 0)
        {
            this.ItemType = ItemTypes.ImageButton;
            this.SelectedIndex = selectedIndex;
            this.Text = Text;
            this.Images = images;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
        }

        public EasyGridCellInfo(ICollection<Image> images, bool? isChecked)
        {
            this.ItemType = ItemTypes.ImageCheckBox;
            Checked = isChecked;
            CheckInt = (isChecked == true) ? 1 : (isChecked == false) ? 0 : 2;
            this.Images = images;
        }

        public EasyGridCellInfo(Image image)
        {
            this.ItemType = ItemTypes.Image;
            this.Images = new Image[]{image};
            this.SelectedIndex = 0;
        }

        public EasyGridCellInfo(ICollection<String> Items, ICollection<int> selected = null)
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
        public void SetTextBoxInfo(String Text, bool editable, TextBoxModes mode= TextBoxModes.TextBox)
        {
            if (mode == TextBoxModes.TextBox)
            {
                this.ItemType = ItemTypes.TextBox;
            }
            else
            {
                this.ItemType = ItemTypes.FileOpenBox;
            }
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
        
        public void SetImageButtonInfo(ICollection<Image> images, int selectedIndex, String Text, ImageLayout mode = ImageLayout.Stretch, int fixedWidth = 0, int fixedHeight = 0)
        {
            this.ItemType = ItemTypes.ImageButton;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
            this.Text = Text;
            this.ImageLayout = mode;
            this.ImageFixedWidth = fixedWidth;
            this.ImageFixedHeight = fixedHeight;
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
