using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace IOHandling
{
    public class FileOpenHandler
    {
        public static void FileOpenAndSavePosition(String savingDirectoryFile, Func<String,int> func ){
            
            OpenFileDialog dlg = new OpenFileDialog();
            String initDir;
            if (File.Exists(savingDirectoryFile))
            {
                
                initDir = File.ReadAllText(savingDirectoryFile).Trim();
            }
            else initDir = Directory.GetCurrentDirectory();

            dlg.InitialDirectory = initDir;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.Abort) return;

            func.Invoke(dlg.FileName);

            initDir = getDepthBack(dlg.FileName, 1);

            if (isWithDirectoryPath(savingDirectoryFile))
            {
                String dir = getDepthBack(savingDirectoryFile, 1);
                try { Directory.CreateDirectory(dir); }
                catch { }//이미 디렉토리가 있으면 만들 필요가 없다.
            }
            File.WriteAllText(savingDirectoryFile, initDir);
        }

        public static void FileOpenInDirectory(String initDir, Func<String, int> func)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            if (Directory.Exists(initDir) == false) Directory.CreateDirectory(initDir);

            dlg.InitialDirectory = initDir;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.Abort) return;

            func.Invoke(dlg.FileName);

        }

        public static void FileSaveAndSavePosition(String savingDirectoryFile, String initName, Func<String, int> func)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            String initDir;
            if (File.Exists(savingDirectoryFile))
            {
                
                initDir = File.ReadAllText(savingDirectoryFile).Trim();
            }
            else initDir = Directory.GetCurrentDirectory();

            dlg.InitialDirectory = initDir;
            dlg.FileName = (initName.Length==0)? "SAVE"+DateTime.Now.ToString("yyMMdd_HHmmss")+".rec" : initName;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.Abort) return;

            func.Invoke(dlg.FileName);

            initDir = getDepthBack(dlg.FileName, 1);
            
            if (isWithDirectoryPath(savingDirectoryFile))
            {
                String dir = getDepthBack(savingDirectoryFile, 1);
                try { Directory.CreateDirectory(dir); }
                catch { }//이미 디렉토리가 있으면 만들 필요가 없다.
            }
            File.WriteAllText(savingDirectoryFile, initDir);
        }

        public static void FileSaveInDirectory(String initDir, String initName, Func<String, int> func, Boolean saveNow)
        {

            SaveFileDialog dlg = new SaveFileDialog();

            if (Directory.Exists(initDir) == false) Directory.CreateDirectory(initDir);
            String newFileName = (initName.Length == 0) ? "SAVE" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".rec" : initName;
            if (saveNow) { func.Invoke(initDir+"\\"+newFileName); }
            else
            {
                dlg.InitialDirectory = initDir;
                dlg.FileName = newFileName;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.Abort) return;

                func.Invoke(dlg.FileName);
            }

        }
        public static String getDepthBack(String orgPath, int backs)
        {
            if (backs > 0)
            {
                int lastIndex = orgPath.LastIndexOf("\\");
                if (lastIndex < 0) lastIndex = orgPath.LastIndexOf("/");
                return getDepthBack(orgPath.Substring(0, lastIndex), backs - 1);
            }
            else
            {
                return orgPath;
            }
        }
        public static bool isWithDirectoryPath(String path)
        {
            return path.IndexOf("\\") >= 0 || path.IndexOf("/") >= 0;
        }

        public static void writeLineWithSameLength(StreamWriter file, String text, int totalLength=100)
        {
            int rest = totalLength - text.Length;
            StringBuilder sb = new StringBuilder(totalLength-file.NewLine.Length);
            sb.Insert(0,text,1);
            sb.Insert(text.Length, " ", rest- file.NewLine.Length);
            TextWriter writer = TextWriter.Synchronized(file);
            writer.WriteLine(sb.ToString());
        }
        public static long findLine(StreamReader file, int lineNumber=0, int textLengthOfALine=100, SeekOrigin seekOrigin= SeekOrigin.Begin)
        {
            Stream stream = file.BaseStream;
            long offset = lineNumber*(textLengthOfALine*Buffer.ByteLength(" ".ToCharArray()));
            return stream.Seek(offset, seekOrigin);
        }
        
    }
}
