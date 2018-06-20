using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Flash.External
{
    public partial class FlashAdder : UserControl, IDisposable
    {
        public event FlashFunctionCallEventHandler U_FlashFuncCallBack=null;
        public new event MouseEventHandler Click=null;
        private ExternalInterfaceProxy _proxy=null;
        private ExternalInterfaceCallEventHandler _ExternalCall=null;
        private String _moviePath="";
        public FlashAdder()
        {
            InitializeComponent();
            //U_Movie = @".\NotLoaded.swf";
            //_proxy = null;
            _ExternalCall = new ExternalInterfaceCallEventHandler(_proxy_ExternalInterfaceCall);
            this.DoubleBuffered = true;
        }
        ~FlashAdder()
        {
            this.Dispose();
        }
        public new void Dispose()
        {
            if (_moviePath!=null && _proxy != null)
            {
                _proxy.ExternalInterfaceCall -= _ExternalCall;
                _proxy.Dispose();
                FlashMovie.Dispose();
                FlashMovie = null;
                _proxy = null;
            }
        }

        protected Object[] getDataArray(int startIndex, Array dataArr)
        {
            Object[] arr;
            int dataArrSize = 0;
            if (dataArr.GetValue(0).GetType().IsArray == true)
            {
                dataArrSize = ((Array)(dataArr.GetValue(0))).Length;
                arr = new Object[dataArrSize+1];
                arr[0] = startIndex;
                for (int i = 0; i < dataArrSize; i++)
                {
                    arr[i + 1] = ((Array)(dataArr.GetValue(0))).GetValue(i);
                }
            }
            else
            {
                dataArrSize = dataArr.Length;
                arr = new Object[dataArr.Length + 1];
                arr[0] = startIndex;
                for (int i = 0; i < dataArrSize; i++)
                {
                    arr[i + 1] = dataArr.GetValue(i);
                }
            }
            return arr;
        }

        public AxShockwaveFlashObjects.AxShockwaveFlash U_AxFlash { get { return FlashMovie; } }

        
        [Bindable(true,BindingDirection.TwoWay)]
        [SettingsBindable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public String U_Movie
        {
            get
            {
                return _moviePath;
                    
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    
                    //try
                    //{
                        _moviePath = value.Replace("/", "\\");
                        _moviePath = _moviePath.Replace(Directory.GetCurrentDirectory() + "\\", "");
                        _moviePath = _moviePath.Replace("..", "&__&");
                        _moviePath = _moviePath.Replace(".\\", "");
                        _moviePath = _moviePath.Replace("&__&", "..");
                        FlashMovie.Movie = Directory.GetCurrentDirectory() + "\\" + _moviePath;
                        //_moviePath = Directory.GetCurrentDirectory() +"\\"+ _moviePath;
                        FlashMovie.CausesValidation = false;
                        _proxy = new ExternalInterfaceProxy(FlashMovie);
                        _proxy.ExternalInterfaceCall += _ExternalCall;
                        OnMovieLoad();
                   // }
                   // catch(Exception e) {
                   //     throw;
                   // }
                }
            }
        }

        protected virtual void OnMovieLoad()
        {
        }

        public void U_addRightMenu(String menuText, String CallBackFuncName, Boolean sepBefore, Boolean makeNew)
        {
            //addRightMenu(menuText:String, funcName:String, sepBefore:Boolean=false, mekeNew:Boolean= false){
            _proxy.Call("addRightMenu", menuText, CallBackFuncName, sepBefore, makeNew);
        }

        public object U_CallFlashFunc(String funcName, params Object[] args)
        {
            if (_moviePath == null || _moviePath.Length==0) return null;
            try
            {
                return _proxy.Call(funcName, args);
            }
            catch(Exception e)
            {
                throw new Exception("플래시에서 함수를 call할때 에러가 발생하였습니다.시간: " + DateTime.Now.ToString() + "\n객체이름:"+ this.Name+"\n" + e.Message);
            }
        }

        public object U_CallFlashFuncWithArray(String funcName, Object[] args)
        {
            if (_moviePath == null || _moviePath.Length==0) return null;
            try
            {
                return _proxy.Call(funcName, args);
            }
            catch (Exception e)
            {
                throw new Exception("플래시에서 함수를 call할때 에러가 발생하였습니다.시간: " + DateTime.Now.ToString() + "\n객체이름:" + this.Name + "\n" + e.Message);
            }
        }

        public void U_setValue(params Object[] args)
        {
            if (_moviePath == null || _moviePath.Length==0) return;
            U_CallFlashFunc("setValue", args);
        }

        public void U_setValues(int start, Array args)
        {
            if (_moviePath == null || _moviePath.Length==0) return;
            Object[] objs = new Object[args.Length + 1];
            objs[0] = start;
            for (int i = 1; i <= args.Length; i++) objs[i] = args.GetValue(i - 1);

            U_CallFlashFunc("setValues", objs);
        }

        public Object U_getValue()
        {
            return (_moviePath!=null)? U_CallFlashFunc("getValue"):null;
        }


        object _proxy_ExternalInterfaceCall(object sender, ExternalInterfaceCallEventArgs e)
        {
            if (e.FunctionCall.FunctionName.ToLower().Equals("click"))
            {
                MouseButtons btn;
                
                try
                {
                    btn = MouseButtons.Left;
                    int numOfClicked = 1;
                    int x = 0;
                    int y = 0;
                    int delta = 0;
                    Object[] args = e.FunctionCall.Arguments;

                    if (args.Length > 0)
                    {
                        String mouseKey = ((String)args[0]).ToLower();
                        switch (mouseKey)
                        {
                            case "left":
                                btn = MouseButtons.Left;
                                break;
                            case "right":
                                btn = MouseButtons.Right;
                                break;
                            case "middle":
                                btn = MouseButtons.Middle;
                                break;
                            case "both":
                                btn = MouseButtons.Left | MouseButtons.Right;
                                break;
                            case "xbutton1":
                                btn = MouseButtons.XButton1;
                                break;
                            case "xbutton2":
                                btn = MouseButtons.XButton2;
                                break;
                            case "delta":
                                btn = MouseButtons.None;
                                break;
                            default:
                                btn = MouseButtons.Left;
                                break;
                        }
                    }
                    if (args.Length > 1) numOfClicked = (Int32)(double)args[1];
                    if (args.Length > 2) x = (int)(double)args[2];
                    if (args.Length > 3) y = (int)((double)args[3]);
                    if (args.Length > 4) delta = (Int32)((double)args[4]);


                    Console.Write(Click);
                    if (Click != null)
                    {

                        Click(this, new MouseEventArgs(btn, numOfClicked, x, y, delta));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("exception occured!:" + ex.Message);
                }
            }
            if (U_FlashFuncCallBack != null) return U_FlashFuncCallBack(sender, e.FunctionCall);
            else return null;
        }

    }
    public delegate object FlashFunctionCallEventHandler(object sender, ExternalInterfaceCall e);
}
