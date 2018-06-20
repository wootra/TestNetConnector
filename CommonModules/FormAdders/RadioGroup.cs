using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public partial class RadioGroup : UserControl
    {
        /// <summary>
        /// List of RadioButtons. Don't add Radio Button to this...
        /// Instead, Use AddRadio Method.
        /// </summary>
        public List<RadioButton> RadioButtons = new List<RadioButton>();
        public event IndexSelectedEventHandler E_RadioButtonSelected;

        public RadioGroup()
        {
            InitializeComponent();
        }

        public RadioButton AddRadio(String text)
        {
            RadioButton radio = new RadioButton();
            this.P_Main.Controls.Add(radio);
            // 
            // radio
            // 
            radio.Location = new System.Drawing.Point(3, 3);
            radio.Name = "radio";
            radio.Size = _unitSize;
            radio.AutoSize = _autoSize;
            radio.TabIndex = 0;
            radio.TabStop = true;
            radio.Text = text;
            radio.UseVisualStyleBackColor = true;

            RadioButtons.Add(radio);
            radio.Click += new EventHandler(radio_Click);
            return radio;
        }

        void radio_Click(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            int index = RadioButtons.IndexOf(radio);
            _selectedIndex = index;
            if (E_RadioButtonSelected != null) E_RadioButtonSelected(new IndexSelectedEventArgs(this, radio, index));
        }

        bool _autoSize = true;
        [Browsable(true)]
        public new bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                _autoSize = value;
                for (int i = 0; i < RadioButtons.Count; i++)
                {
                    RadioButtons[i].AutoSize = value;
                }
            }
        }
        Size _unitSize = new Size(100, 20);
        [Browsable(true)]
        public Size UnitSize {
            get { return _unitSize; }
            set
            {
                _unitSize = value;
                for (int i = 0; i < RadioButtons.Count; i++)
                {
                    RadioButtons[i].Size = value;
                }
            }
        }

        [Browsable(true)]
        public String[] Items
        {
            get
            {
                String[] texts = new String[RadioButtons.Count];
                for (int i = 0; i < RadioButtons.Count; i++)
                {
                    texts[i] = RadioButtons[i].Text;
                }
                return texts;
            }

            set
            {
                RadioButtons.Clear();
                this.P_Main.Controls.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    AddRadio(value[i]);
                }
                if(value.Length>0) SelectedIndex = 0;
            }
        }

        int _selectedIndex = -1;
        [Browsable(true)]
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value < RadioButtons.Count)
                {
                    _selectedIndex = value;
                    for (int i = 0; i < RadioButtons.Count; i++)
                    {
                        if (value == i) RadioButtons[i].Checked = true;
                        else RadioButtons[i].Checked = false;
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("라디오버튼의 개수보다 많은 숫자를 선택했음");
                }

            }
        }


    }
}
