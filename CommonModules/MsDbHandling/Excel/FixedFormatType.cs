using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsDbHandler.Excel
{
    /// <summary>
    /// 내보내기 파일 형식.
    /// </summary>
    /// <remarks>
    /// Microsoft.Office.Interop.Excel.XlFixedFormatType과 동일합니다.
    /// 사용 편의상 복사본을MsOffice.Excel에도 위치 시킵니다.
    /// </remarks>
    public enum FixedFormatType
    {
        xlTypePDF = 0,
        xlTypeXPS = 1,
    }
}
