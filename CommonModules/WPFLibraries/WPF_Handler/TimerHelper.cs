using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;

namespace WPF_Handler
{
    public class TimerHelper
    {
    
        DispatcherTimer _timer;
        
        //RoutedEventArgs _clickEventArg;
        public delegate void DelFunc();
        DelFunc _funcForElipsed=null;
        public TimerHelper(DispatcherTimer timer, DelFunc func = null)
        {
            _timer = timer;
            _timer.Tick += new EventHandler(_timer_Tick);
            
            _funcForElipsed = func;
        }
        public TimerHelper(int interval, UIElement parent, DelFunc func = null, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            _timer = new DispatcherTimer(priority, parent.Dispatcher);
            _timer.Interval = new TimeSpan(interval*10000);
            _timer.Tick += new EventHandler(_timer_Tick);

            _funcForElipsed = func;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_timer.Dispatcher != null && _funcForElipsed != null)
               
                    _funcForElipsed();
                    //_timer.Dispatcher.BeginInvoke(_funcForElipsed);
                    //_timer.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(_funcForElipsed));
                    
               
        }

        public DispatcherTimer Timer
        {
            get
            {
                return _timer;
            }
        }

    }
}
