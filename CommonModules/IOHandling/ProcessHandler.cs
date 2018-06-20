using System;
using System.Diagnostics;

namespace IOHandling
{
    public class ProcessHandler
    {
        Process _process=null;
        public void setProcessBeforeStart(string program, string argument = "")
        {
            _process = getProcessBeforeStart(program, argument);
        }
        
        public void setProcessAfterStart(string program, string argument = "")
        {
            _process = getProcessAfterStart(program, argument);
        }
        public String runCommand(String command)
        {
            if (_process == null) setProcessAfterStart("cmd");
            _process.StandardOutput.ReadToEnd();//기본출력을 소모한다.
            _process.StartInfo.RedirectStandardInput = true;
            runCommand(_process, command);
            String output =  readOutputAndClose(_process);
            _process = null;
            return output;
        }


        public static Process getProcessBeforeStart(string program, string argument = "")
        {
            ProcessStartInfo cmd = new ProcessStartInfo(@program, @argument);
            Process process = new Process();
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.RedirectStandardError = true;
            cmd.RedirectStandardInput = true;
            cmd.RedirectStandardOutput = true;
            //process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);

            process.EnableRaisingEvents = true;
            process.StartInfo = cmd;
            return process;
        }


        public static Process getProcessAfterStart(string program, string argument="")
        {
            Process process = getProcessBeforeStart(program, argument);
            process.Start();
            return process;
        }

        public static void runCommand(Process process, string command)
        {
            if (process.HasExited == false)
            {
                process.StartInfo.RedirectStandardInput = true;
                
                process.StandardInput.Write(@command + Environment.NewLine);
            }
            else
            {
                throw new Exception("이미 끝난 프로세스입니다. runProcess로 다시 시작해 주세요");
            }
        }

        public static String readOutputAndClose(Process process)
        {
            try
            {
                process.StandardInput.Close();
            }
            catch { }
            try
            {
                return process.StandardOutput.ReadToEnd();
            }
            catch { }

            if (process.HasExited == false)
            {
               // process.WaitForExit();
                process.Close();
                process = null;
            }
            return null;
        }

        
    }
}
