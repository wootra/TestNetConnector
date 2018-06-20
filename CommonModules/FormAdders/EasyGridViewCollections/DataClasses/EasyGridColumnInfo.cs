using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FormAdders.EasyGridViewCollections
{
    public class EasyGridColumnInfo
    {

        public ItemTypes ItemType = ItemTypes.Various;//이 설정은 무시된다.
        public CheckBoxColors CheckboxColor;
        public ICollection<String> Items = null;
        public int SelectedIndex = -1;
        ICollection<int> SelectedIndice;
        public String SelectedText = null;
        public String TitleText = null;
        public String BaseText = null;
        public String ColumnName = "";
        public int Width=0;
        public ICollection<Image> Images = null;
        public bool IsEditable = false;
        public TextAlignModes TextAlignMode = TextAlignModes.None;
        public Actions ActionOnClick = Actions.CommonAction;
        public Actions ActionOnDoubleClick = Actions.CommonAction;
        public Actions ActionOnRightClick = Actions.CommonAction;

        #region Constructors
        public EasyGridColumnInfo()
        {
        }
        public enum CheckBoxTypes{ ImageCheckBox, CheckBox};
        /// <summary>
        /// Make ImageCheckBox Column.
        /// You can set the color of CheckBox..
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="color"></param>
        public EasyGridColumnInfo(int wid, String colName, CheckBoxColors color)
        {
            ItemType = ItemTypes.ImageCheckBox;
            CheckBoxColors CheckboxColor = color;
            ColumnName = colName;
            Width = wid;
        }

        /// <summary>
        /// Make CheckBox Column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="colHeaderText"></param>
        public EasyGridColumnInfo(int wid, String colName, String colHeaderText=null)
        {
            ItemType = ItemTypes.CheckBox;
            TitleText = colHeaderText;
            ColumnName = colName;
            Width = wid;
        }

        public enum ButtonTypes{Button, CloseButton};
        /// <summary>
        /// Make button column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="titleText"></param>
        /// <param name="baseText"></param>
        public EasyGridColumnInfo(int wid, String colName, String titleText="", String baseText="")
        {
            ColumnName = colName;
            Width = wid;
            TitleText = titleText;
            this.ItemType = ItemTypes.Button;
            this.BaseText = baseText;
        }

        /// <summary>
        /// make Text column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="Text"></param>
        /// <param name="editable"></param>
        /// <param name="alignMode"></param>
        public EasyGridColumnInfo(int wid, String colName, String Text, bool editable, TextAlignModes alignMode = TextAlignModes.None)
        {
            ColumnName = colName;
            Width = wid;
            this.ItemType = ItemTypes.TextBox;
            IsEditable = editable;
            this.TitleText = Text;
            this.TextAlignMode = alignMode;
        }

        public enum SelectionTypes{ ComboBox, RadioButton};
        /// <summary>
        /// make ComboBox or RadioButton Column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="titleText"></param>
        /// <param name="items"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="itemtype"></param>
        public EasyGridColumnInfo(int wid, String colName, String titleText, ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;
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

        /// <summary>
        /// make comboBox column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="titleText"></param>
        /// <param name="items"></param>
        /// <param name="selectedText"></param>
        public EasyGridColumnInfo(int wid, String colName, String titleText, ICollection<String> items, String selectedText)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        /// <summary>
        /// make new IEasyGridColumn
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="titleText"></param>
        /// <param name="type"></param>
        public EasyGridColumnInfo(int wid, String colName, String titleText, ItemTypes type)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;
            this.ItemType = ItemTypes.KeyValue;
        }

        /// <summary>
        /// make image column
        /// </summary>
        /// <param name="wid"></param>
        /// <param name="colName"></param>
        /// <param name="images"></param>
        /// <param name="selectedIndex"></param>
        public EasyGridColumnInfo(int wid, String colName, String titleText, ICollection<Image> images, bool useImageOnHeader=false, int selectedIndex=-1)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;

            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
        }

        public EasyGridColumnInfo(int wid, String colName, String titleText, Image image)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;
            this.ItemType = ItemTypes.Image;
            this.Images = new Image[]{image};
            this.SelectedIndex = 0;
        }

        public EasyGridColumnInfo(int wid, String colName, ICollection<String> Items, ICollection<int> selected = null)
        {
            ColumnName = colName;
            Width = wid;
            this.ItemType = ItemTypes.CheckBoxGroup;
            this.Items = Items;
            this.SelectedIndice = selected;
        }
        #endregion

        #region methods
        public void SetImageCheckBoxInfo(int wid, String colName, CheckBoxColors color)
        {
            ItemType = ItemTypes.ImageCheckBox;
            CheckBoxColors CheckboxColor = color;
            ColumnName = colName;
            Width = wid;
        }

        public void SetCheckBoxInfo(int wid, String colName, String colHeaderText=null)
        {
            ItemType = ItemTypes.CheckBox;
            TitleText = colHeaderText;
            ColumnName = colName;
            Width = wid;
        }

        public void SetButtonInfo(int wid, String colName, String titleText = "", String baseText = "")
        {
            ColumnName = colName;
            Width = wid;
            TitleText = titleText;
            this.ItemType = ItemTypes.Button;
            this.BaseText = baseText;
        }

        public void SetTextBoxInfo(int wid, String colName, String Text, bool editable, TextAlignModes alignMode = TextAlignModes.None)
        {
            ColumnName = colName;
            Width = wid;
            this.ItemType = ItemTypes.TextBox;
            IsEditable = editable;
            this.TitleText = Text;
            this.TextAlignMode = alignMode;
        }

        public void SetComboOrRadioInfo(int wid, String colName, ICollection<String> items, int selectedIndex, SelectionTypes itemtype = SelectionTypes.ComboBox)
        {
            ColumnName = colName;
            Width = wid;
            if (itemtype == SelectionTypes.ComboBox) this.ItemType = ItemTypes.ComboBox;
            else this.ItemType = ItemTypes.RadioButton;
            this.Items = items;
            this.SelectedIndex = selectedIndex;
            if (selectedIndex >= 0 && items.Count > selectedIndex) this.SelectedText = items.ElementAt(selectedIndex);
        }

        public void SetComboInfo(int wid, String colName, ICollection<String> items, String selectedText)
        {
            ColumnName = colName;
            Width = wid;
            this.ItemType = ItemTypes.ComboBox;
            this.Items = items;
            this.SelectedText = selectedText;
            this.SelectedIndex = Items.ToList().IndexOf(selectedText);
        }

        public void SetImageInfo(int wid, String colName, ICollection<Image> images, int selectedIndex)
        {
            this.ItemType = ItemTypes.Image;
            this.SelectedIndex = selectedIndex;
            this.Images = images;
        }

        public void SetImageInfo(int wid, String colName, String titleText, Image image)
        {
            ColumnName = colName;
            Width = wid;
            this.TitleText = titleText;
            this.ItemType = ItemTypes.Image;
            this.Images = new Image[] { image };
            this.SelectedIndex = 0;
        }

        public void SetCheckBoxGroupInfo(int wid, String colName, ICollection<String> Items, ICollection<int> selected = null)
        {
            ColumnName = colName;
            Width = wid;
            this.ItemType = ItemTypes.CheckBoxGroup;
            this.Items = Items;
            this.SelectedIndice = selected;
        }
#endregion
    }
}
