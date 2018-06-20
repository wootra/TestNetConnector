using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WPF_Handler
{
    public class EventHelper
    {
        public event MouseButtonEventHandler E_Click;
        public event MouseButtonEventHandler E_DoubleClick;
        public event MouseButtonEventHandler E_MouseDown;
        public event MouseButtonEventHandler E_MouseUp;
        public event MouseEventHandler E_MouseOver;
        public event MouseEventHandler E_MouseOut;
        Control _btn;
        Control _baseControl;
        public Control OriginalSource { get { return _btn; } }

        public EventHelper(Control baseControl, Control btn)
        {
            _btn = btn;
            _baseControl = baseControl;
            _btn.MouseUp += new MouseButtonEventHandler(_btn_MouseUp);
            _btn.MouseDown += new MouseButtonEventHandler(_btn_MouseDown);
            _btn.MouseEnter += new MouseEventHandler(_btn_MouseEnter);
            _btn.MouseLeave += new MouseEventHandler(_btn_MouseLeave);
            //if (_btn is Button) (_btn as Button).Click += new RoutedEventHandler(ButtonHelper_Click);
            _btn.MouseUp += new MouseButtonEventHandler(ButtonHelper_Click);
            _btn.MouseDoubleClick += new MouseButtonEventHandler(ButtonHelper_MouseDoubleClick);
        }

        void ButtonHelper_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (E_DoubleClick != null) E_DoubleClick(_baseControl, e);
        }

        void ButtonHelper_Click(object sender, MouseButtonEventArgs e)
        {
            if (E_Click != null) E_Click(_baseControl, e);
        }

        void _btn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (E_MouseOut != null) E_MouseOut(_baseControl, e);
        }

        void _btn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (E_MouseOver != null) E_MouseOver(_baseControl, e);
        }

        void _btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (E_MouseDown != null) E_MouseDown(_baseControl, e);
        }

        void _btn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (E_MouseUp != null) E_MouseUp(_baseControl, e);
        }



    }
}
