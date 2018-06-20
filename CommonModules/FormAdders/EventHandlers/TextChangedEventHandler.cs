using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

    public class TextChangedEventArgs : EventArgs
    {
        public String Text { get; set; }
        public String BeforeText { get; set; }
        public bool IsCancel { get; set; }
        public TextChangedEventArgs(String beforeText, String afterText)
        {
            Text = afterText;
            BeforeText = beforeText;
            IsCancel = false;
        }
    }
}
