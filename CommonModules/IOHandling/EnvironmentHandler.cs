using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IOHandling
{
    public class EnvironmentHandler
    {
        public enum PATH_SET { INSERT_FRONT, INSERT_LAST, REMOVE, CHECK };

        public static String setPath(String pathOrRemoveKey, EnvironmentVariableTarget target, PATH_SET setMode)
        {

            String path = Environment.GetEnvironmentVariable("PATH", target);
            String[] pathAll = path.Split(";".ToCharArray());

            //Console.WriteLine(path);
            String newPath = "";
            String existPath = "";
            foreach (String aPath in pathAll)
            {
                if (aPath.ToLower().IndexOf(pathOrRemoveKey) >= 0)
                {
                    existPath += aPath + ";";
                }
                else newPath += aPath + ";";
                //Console.WriteLine(aPath);
            }
            //Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
            //String rtNginePath = @"C:\RTNgine\host\x86-mingw32\ppc-toolchain\powerpc-wrs-vxworks\bin;C:\RTNgine\host\x86-mingw32\ppc-toolchain\lib\gcc-lib\powerpc-wrs-vxworks55\2.95.4;C:\RTNgine\host\x86-mingw32\ppc-toolchain\bin;C:\RTNgine\host\x86-mingw32\MinGW\msys\bin;C:\RTNgine\host\x86-mingw32\MinGW\bin;";

            if (pathOrRemoveKey != null && pathOrRemoveKey.Length > 0 && pathOrRemoveKey[pathOrRemoveKey.Length - 1] != ';') pathOrRemoveKey += ";";
            switch (setMode)
            {
                case PATH_SET.INSERT_FRONT:
                    Environment.SetEnvironmentVariable("PATH", pathOrRemoveKey + path, target);
                    break;
                case PATH_SET.INSERT_LAST:
                    Environment.SetEnvironmentVariable("PATH", path + pathOrRemoveKey, target);
                    break;
                case PATH_SET.REMOVE:
                    Environment.SetEnvironmentVariable("PATH", newPath, target);
                    if (existPath.Length > 0) Environment.SetEnvironmentVariable("REMOVED_PATH", existPath, EnvironmentVariableTarget.User);
                    else Environment.SetEnvironmentVariable("REMOVED_PATH", "", EnvironmentVariableTarget.User);
                    break;
                case PATH_SET.CHECK:
                    File.Delete("IS_IN_PATH_YES");
                    if (existPath.Length > 0)
                    {
                        StreamWriter file = File.CreateText("IS_IN_PATH_YES");
                        file.Write(existPath);
                        file.Close();
                    }
                    //if (existPath.Length>0) Environment.SetEnvironmentVariable("IS_IN_PATH", "1", EnvironmentVariableTarget.User);
                    //else Environment.SetEnvironmentVariable("IS_IN_PATH", "0", EnvironmentVariableTarget.User);
                    return existPath;
            }
            String now = DateTime.Now.ToLongTimeString();
            now = now.Replace(":", "_");
            now = now.Replace("-", "");
            now = now.Replace(" ", "_");
            File.WriteAllText("./oldPath" + now + ".txt", path);
            Environment.SetEnvironmentVariable("OLD_PATH", "0", EnvironmentVariableTarget.User);
            return existPath;
        }

        public static void setRtngineToPath(EnvironmentVariableTarget target, PATH_SET setMode)
        {
            String rtNginePath = @"C:\RTNgine\host\x86-mingw32\ppc-toolchain\powerpc-wrs-vxworks\bin;C:\RTNgine\host\x86-mingw32\ppc-toolchain\lib\gcc-lib\powerpc-wrs-vxworks55\2.95.4;C:\RTNgine\host\x86-mingw32\ppc-toolchain\bin;C:\RTNgine\host\x86-mingw32\MinGW\msys\bin;C:\RTNgine\host\x86-mingw32\MinGW\bin;";

            setPath(rtNginePath, target, setMode);
        }

        public static String GetEnvironmentVariable(String key, EnvironmentVariableTarget target)
        {
            return Environment.GetEnvironmentVariable(key, target);
        }
    }
}
