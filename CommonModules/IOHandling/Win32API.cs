using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace IOHandling
{
    public static class Win32APIs
    {

        public const Int32 WM_COPYDATA = 0x004A;
        public static StreamWriter Logger = null;

        public struct COPYDATASTRUCT
        {

            public IntPtr dwData;

            public UInt32 cbData;

            [MarshalAs(UnmanagedType.LPStr)]

            public string lpData;

        }

        //static Win32APIs.COPYDATASTRUCT data;

        public static void SendMsgData(String message, String processOrWindowName, Boolean isSendToOnlyOne=false)
        {
            Win32APIs.COPYDATASTRUCT data = new Win32APIs.COPYDATASTRUCT();

            data.dwData = (IntPtr)(1024 + 604);

            data.cbData = (uint)message.Length * sizeof(char);

            data.lpData = message;

            IntPtr handle;
            try
            {
                Process[] process = Process.GetProcessesByName(processOrWindowName);

                if (process.Length == 0)
                {
                    handle = Win32APIs.FindWindow(null, processOrWindowName);
                    if (handle.ToInt32() > 0)
                    {
                        Win32APIs.SendMessage(handle, Win32APIs.WM_COPYDATA, IntPtr.Zero, ref data);
                    }
                }
                else
                {

                    for (int i = 0; i < process.Length; i++)
                    {
                        handle = process[i].MainWindowHandle;
                        if (handle.ToInt32() > 0)
                        {
                            Win32APIs.SendMessage(handle, Win32APIs.WM_COPYDATA, IntPtr.Zero, ref data);
                            if (isSendToOnlyOne) break;
                        }
                    }
                }
                
            }
            catch {
                handle = Win32APIs.FindWindow(null, processOrWindowName);
                if (handle.ToInt32() > 0)
                    Win32APIs.SendMessage(handle, Win32APIs.WM_COPYDATA, IntPtr.Zero, ref data);
            }
            if (Logger != null)
            {
                try
                {
                    TextWriter writer = TextWriter.Synchronized(Logger);
                    writer.WriteLine(message);
                }
                catch (ObjectDisposedException) { }
            }
        }
        public static IntPtr SendMessage(String processName, UInt32 Msg, UInt32 wParam = 0, Boolean isSendToOnlyOne = false, Win32APIs.COPYDATASTRUCT lParam = (new Win32APIs.COPYDATASTRUCT()))
        {
            Win32APIs.COPYDATASTRUCT data = lParam;
            IntPtr blankHandle = Win32APIs.FindWindow(null,"___NO_IT'S NOT A WINDOW___");
            IntPtr handle;
            IntPtr result=IntPtr.Zero;
            try
            {
                Process[] process = Process.GetProcessesByName(processName);

                if (process.Length == 0)
                {
                    handle = Win32APIs.FindWindow(null, processName);
                    if (handle != blankHandle && handle.ToInt32() > 0)
                        Win32APIs.SendMessage(handle, Msg, (IntPtr)wParam, ref data);
                }
                else
                {

                    for (int i = 0; i < process.Length; i++)
                    {
                        handle = process[i].MainWindowHandle;
                        if (handle != blankHandle && handle.ToInt32() > 0)
                        {
                            result = Win32APIs.SendMessage(handle, Msg, (IntPtr)wParam, ref data);
                            if (isSendToOnlyOne) break;
                        }

                    }
                }
            }
            catch
            {
                handle = Win32APIs.FindWindow(null, processName);
                if (handle != blankHandle && handle.ToInt32() > 0)
                    result = Win32APIs.SendMessage(handle, Msg, (IntPtr)wParam, ref data);
            }
            return result;
        }

        /// <summary>
        // 폼 안에서 
        /// protected override void WndProc(ref Message wMessage) 
        /// 함수를 오버라이드 한 후, argument를 받아와서 넣어주면 출력으로 lParam을 준다.
        /// 이 함수 호출 이후에 
        /// base.WndProc(wMessage)
        /// 를 해 주고 함수를 끝내야한다.
        /// </summary>
        /// <param name="wMessage">WndProc에서 받아온 Message</param>
        /// <param name="lParam">받은 실제 값</param>
        public static void getMsgData(ref Message wMessage, out COPYDATASTRUCT lParam)
        {
            switch (wMessage.Msg)
            {
                case Win32APIs.WM_COPYDATA:

                    lParam = (Win32APIs.COPYDATASTRUCT)Marshal.PtrToStructure(wMessage.LParam, typeof(Win32APIs.COPYDATASTRUCT));
                    /*
                    Win32API.COPYDATASTRUCT lParam2 = new Win32API.COPYDATASTRUCT();
                    lParam2 = (Win32API.COPYDATASTRUCT)wMessage.GetLParam(lParam2.GetType());
                    MessageBox.Show("WM_COPYDATA : " + lParam1.lpData + "/ " + lParam2.lpData);
                    */
                    break;

                default:
                    lParam = new COPYDATASTRUCT();
                    break;

            }

            //base.WndProc(ref wMessage);


        }

        public static bool GetKeyState(Keys key)
        {
            if ((Win32APIs.GetKeyState((int)key) & 0xffff) != 0)
            {
                return true;
            }
            else return false;

        }
        public static void KeyToggleOff(Keys key)
        {
            Win32APIs.keybd_event((byte)key, (byte)0, 0, 0);
            Win32APIs.keybd_event((byte)key, (byte)0, 2, 0);
        }

        public static void KeyToggleOn(Keys key)
        {
            Win32APIs.keybd_event((byte)key, (byte)0, 2, 0);
            Win32APIs.keybd_event((byte)key, (byte)0, 0, 0);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref Win32APIs.COPYDATASTRUCT lParam);



        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);



        [DllImport("user32.dll")]

        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);



        [DllImport("user32.dll", EntryPoint = "SendMessageW")]

        public static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);



        [DllImport("user32.dll")]

        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]

        public static extern IntPtr SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);



        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);





        [DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);



    }




}
