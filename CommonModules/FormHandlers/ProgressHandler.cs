using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Th = System.Threading;


namespace FormHandlers
{
    public class ProgressHandler
    {
        Form _form;
        IProgressBar _progressBar;
        Timer _timer = new Timer();
        Th.Thread t;
        Func<bool> funcToRun;
        public event ProgressBarFinished E_ProgressEnd;

        public void SetProgressForm(Form form, IProgressBar progressBar, bool showWindowOnStart = true)
        {
            _form = form;
            _progressBar = progressBar;
            
            _timer.Interval = 1000;
            _timer.Tick += _t_Tick;
            _timer.Start();
            ShowWindowOnStart = showWindowOnStart;
        }

        public void SetFuncToRun(Func<bool> func)
        {
            funcToRun = func;

            t = new Th.Thread(new Th.ParameterizedThreadStart(loading));

        }

        public bool ShowWindowOnStart
        {
            get;
            set;
        }
        bool _isLoading = false;

        public void loading(object obj)
        {
            if (_isLoading) return;
            _isLoading = true;
            bool isOk = false;
            if (funcToRun != null)
            {
                isOk = funcToRun();
            }
                
            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action(() =>
                {
                    if (E_ProgressEnd != null) E_ProgressEnd(isOk);
                }));
            }
            else
            {
                if (E_ProgressEnd != null) E_ProgressEnd(isOk);
            }
        }
        /// <summary>
        /// parent가 null이면 ShowWindowOnStart는 자동으로 false..
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public void Start(int max, IWin32Window parent = null)
        {
            if (t == null) throw new Exception("run SetFuncToRun(func) first..");
            _progressBar.Maximum = max;
            _progressBar.Value = 0;
            t.Start();
            if (parent == null) ShowWindowOnStart = false;
            if (ShowWindowOnStart)
            {
                
                _form.TopMost = true;
                _form.StartPosition = FormStartPosition.CenterParent;
                _form.Show(parent);
            }
        }
        public void Stop()
        {
            _isLoading = false;
            if (ShowWindowOnStart)
            {
                _form.Hide();
            }
            t = new Th.Thread(new Th.ParameterizedThreadStart(loading));

        }

        void _t_Tick(object sender, EventArgs e)
        {
            try
            {
                _progressBar.Value = (_progressBar.Maximum >= _v) ? _v : _progressBar.Maximum;
            }
            catch
            {

            }
        }

        int _v = 0;
        public void SetValue(int v)
        {
            _v = v;
        }
    }

    /// <summary>
    /// ToolstripProgressBarExtend 또는 ProgressBarExtend 를 사용한다.
    /// </summary>
    public interface IProgressBar
    {
        int Maximum { get; set; }
        int Minimum { get; set; }
        int Value { get; set; }

    }

    public class ToolstripProgressBarExtend : ToolStripProgressBar, IProgressBar
    {
        public ToolstripProgressBarExtend():base(){

        }

    }

    public class ProgressBarExtend : ProgressBar, IProgressBar
    {
        public ProgressBarExtend()
            : base()
        {

        }
    }

    public delegate void ProgressBarFinished(bool isOk);
}
