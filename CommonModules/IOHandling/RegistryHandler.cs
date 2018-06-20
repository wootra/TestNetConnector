using System;
using System.Text;
using System.Diagnostics;
using System.Timers;
namespace IOHandling
{
    public class RegistryHandler
    {
        public static String getRegValue(String keyPath, String keyName)
        {
            Process process = ProcessHandler.getProcessAfterStart("reg", "query " + keyPath);
            String outStr = ProcessHandler.readOutputAndClose(process);
            String[] lines = outStr.Split("\n".ToCharArray());
            for (int num = 0; num < lines.Length; num++)
            {
                String aLine = lines[num];
                aLine = aLine.Replace("\r", "");
                aLine = aLine.Trim();
                String newLine = aLine.Replace("  "," ");
                while (aLine.Equals(newLine) == false)
                {
                    aLine = newLine;
                    newLine = aLine.Replace("  ", " ");
                }
                String[] tokens = aLine.Split(" ".ToCharArray());
                if (tokens[0].Equals(keyName)) return tokens[2];
            }
            return null;
        }

        public static Timer _timer;
        public static Process _process;
        static Boolean _isOk = true;
        public static Boolean addReg(String regPath, String regValueName, String value, Boolean isForced)
        {
            String arg = (regValueName == null || regValueName.Length == 0) ? "" : " /v " + regValueName;
            arg += (value == null || value.Length == 0) ? "" : " /d  " + value;
            arg += (isForced) ? " /f" : "";
            _timer = new Timer(1000);
            _timer.AutoReset = false;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            Process process = ProcessHandler.getProcessBeforeStart("reg", "add " + regPath+arg);
            _process = process;

            _timer.Start();
            process.Start();
            ProcessHandler.readOutputAndClose(process);
            
            _timer.Close();
            return _isOk;
        }

        public static Boolean delReg(String regPath, String regValueName, Boolean isForced)
        {
            String arg = (regValueName == null || regValueName.Length == 0) ? "" : " /v " + regValueName;
            arg += (isForced) ? " /f /va" : "";
            _timer = new Timer(1000);
            _timer.AutoReset = false;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            Process process = ProcessHandler.getProcessBeforeStart("reg", "delete " + regPath + arg);
            _process = process;

            _timer.Start();
            process.Start();
            ProcessHandler.readOutputAndClose(process);

            _timer.Close();
            return _isOk;
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isOk = false;
            _process.Kill();
        }
    }
}
