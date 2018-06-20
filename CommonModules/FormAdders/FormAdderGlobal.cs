using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormAdders
{
    /// <summary>
    /// 모든 Form에서 static으로 사용할 것들을 모아놓는다.
    /// </summary>
    public class FormAdderGlobalTimer
    {
        static Dictionary<int, Timer> UiRefreshTimer =new Dictionary<int,Timer>();

        public static event TimerRefreshedEventHandler UiRefreshed;

        static FormAdderGlobalTimer This=null;

        public static void InitTimer(int intervalGroup){
            if (This == null)
            {
                FormAdderGlobalTimer timer = new FormAdderGlobalTimer();
                This = timer;
            }
            This.AddTimerGroup(intervalGroup);
        }

        public static void RemoveTimer(int intervalGroup)
        {
            if (This == null) throw new Exception("No TimerGroup Exist!!");

            This.RemoveTimerGroup(intervalGroup);
        }



        private FormAdderGlobalTimer()
        {

        }

         Dictionary<int, EventHandler> _eventHandlers = new Dictionary<int, EventHandler>();

         void AddTimerGroup(int refreshTime)
        {
            if (UiRefreshTimer.ContainsKey(refreshTime) == false)
            {
                UiRefreshTimer[refreshTime] = new Timer();
                UiRefreshTimer[refreshTime].Interval = refreshTime;
                _eventHandlers[refreshTime] = new EventHandler(new Action<object, EventArgs>((Action, b) =>
                {
                    if (UiRefreshed != null) UiRefreshed(refreshTime);
                }));

                UiRefreshTimer[refreshTime].Tick += _eventHandlers[refreshTime];
                UiRefreshTimer[refreshTime].Start();
            }
            
        }
         void RemoveTimerGroup(int refreshTime)
        {
            if (UiRefreshTimer.ContainsKey(refreshTime) == false)
            {

                UiRefreshTimer[refreshTime].Tick -= _eventHandlers[refreshTime];
                UiRefreshTimer[refreshTime].Stop();
                UiRefreshTimer.Remove(refreshTime);
                _eventHandlers.Remove(refreshTime);
            }
            
        }


    }

    public delegate void TimerRefreshedEventHandler(int timerGroup);

}
