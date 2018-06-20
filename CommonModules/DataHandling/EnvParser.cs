using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataHandling
{
    public static class EnvParser
    {
        public static String GetDepthBack(String orgPath, int backs)
        {
            if (backs > 0)
            {
                int lastIndex = orgPath.LastIndexOf("\\");
                return GetDepthBack(orgPath.Substring(0, lastIndex), backs - 1);
            }
            else
            {
                return orgPath;
            }
        }



        public static void GetEnv(String filePath, Dictionary<String, String> destEnv)
        {

            String[] info = null;
            try
            {
                info = File.ReadAllLines(filePath);
                for (int i = 0; i < info.Length; i++) getAEnvLine(info[i],destEnv);
            }
            catch (Exception ex)
            {
                throw new Exception(filePath + " doesn't exist..\n" + ex.Message);
            }

        }

        static void getAEnvLine(String aLine, Dictionary<String, String> destEnv)
        {
            if (aLine.Trim().Length == 0 || aLine.Trim()[0] == '#') return;

            String[] var = aLine.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (var.Count() > 1)
            {
                destEnv[var[0].Trim()] = var[1].Trim(" \t;\"".ToCharArray());
            }
        }
    }
}
