using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IOHandling
{
    public class DirHandler
    {
        public static string EvaluateRelativePath(String mainDirPath, String absoluteFilePath)
        {
            // 입력값 검증
            if (String.IsNullOrEmpty(mainDirPath)) throw new ArgumentNullException("mainDirPath 입력값이 null 또는 공백입니다.");
            if (String.IsNullOrEmpty(absoluteFilePath)) throw new ArgumentNullException("absoluteFilePath 입력값이 null 또는 공백입니다.");
            if (Path.GetPathRoot(mainDirPath).ToLower() != Path.GetPathRoot(absoluteFilePath).ToLower()) throw new ArgumentException("입력값의 루트가 다르므로 처리할 수 없습니다.");
            if (Path.IsPathRooted(mainDirPath) == false) throw new ArgumentException("mainDirPath 이 절대경로가 아닙니다.");
            if (Path.IsPathRooted(absoluteFilePath) == false) throw new ArgumentException("absoluteFilePath 이 절대경로가 아닙니다.");

            // 입력값 보정, C:\test 일때 test가 파일인지 디렉토리인지 애매하다
            mainDirPath = mainDirPath.Trim();
            absoluteFilePath = absoluteFilePath.Trim();
            if (Directory.Exists(mainDirPath + Path.DirectorySeparatorChar)) mainDirPath = mainDirPath + Path.DirectorySeparatorChar;
            if (Directory.Exists(absoluteFilePath + Path.DirectorySeparatorChar)) absoluteFilePath = absoluteFilePath + Path.DirectorySeparatorChar;

            // 상대 경로 추출
            Uri mainDirUri = new Uri(mainDirPath);
            Uri absoluteFileUri = new Uri(absoluteFilePath);
            Uri relativeUri = mainDirUri.MakeRelativeUri(absoluteFileUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // 리턴
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
