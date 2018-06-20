using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;

namespace WPF_Handler
{
    public delegate void CountedClickEventHandler(CountedClickEventArgs e);

    public class CountedClickEventArgs : EventArgs
    {
        public Object Sender;
        public RoutedEventArgs MouseBtnArgs;
        public int Count;
        public MouseButton PressedButton;
        
        public CountedClickEventArgs(Object sender, RoutedEventArgs args, int count, MouseButton button)
        {
            Sender = sender;
            MouseBtnArgs = args;
            Count = count;
            PressedButton = button;
        }
    }

    public class ClickCounter
    {
        DispatcherTimer _clickTimer;
        int _clickCount = 0;
        Object _clickSender;
        int _mouseClickCount = 0;
        MouseButton _clickedButton;
        RoutedEventArgs _clickEventArg;
        public delegate void DelFunc();
        public event CountedClickEventHandler OnCountedClick;
        DelFunc _funcForElipsed=null;
        public CountedClickEventArgs Args;
        
        public ClickCounter(DispatcherTimer clickTimer, DelFunc func = null)
        {
            _clickTimer = clickTimer;
            _clickTimer.Tick += new EventHandler(_clickTimer_Tick);
            
            _funcForElipsed = func;
        }
        public ClickCounter(int interval, UIElement parent, DelFunc func=null, DispatcherPriority priority= DispatcherPriority.Normal)
        {
            _clickTimer = new DispatcherTimer(priority, parent.Dispatcher);
            _clickTimer.Interval = new TimeSpan(interval*10000);
            _clickTimer.Tick += new EventHandler(_clickTimer_Tick);

            _funcForElipsed = func;
            
        }

        Boolean _isClickCheching = false;
        void _clickTimer_Tick(object sender, EventArgs e)
        {

            if (_isClickCheching == true) return;
            _isClickCheching = true;
            _mouseClickCount = _clickCount;
            _clickCount = 0;
            if (_clickTimer.IsEnabled) _clickTimer.Stop();
            Args = new CountedClickEventArgs(_clickSender, _clickEventArg, _mouseClickCount, _clickedButton);
            if (OnCountedClick != null) OnCountedClick(Args);
            //addItemForTest();
            _isClickCheching = false;

            if (_clickTimer.Dispatcher != null && _funcForElipsed != null) _clickTimer.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(_funcForElipsed));
        }

        public void CountClick(Object sender, RoutedEventArgs e)
        {
            _clickSender = sender;
            _clickEventArg = e;
            _clickCount++;
            MouseButtonEventArgs arg = e as MouseButtonEventArgs;
            _clickedButton = arg.ChangedButton;
            //_clickTimer.Stop();
            if (_clickTimer.IsEnabled == false)
            {
                _clickTimer.Start();
            }
            Args = null;
        }



    }
}
