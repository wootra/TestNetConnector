using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomParser
{
    public class CStructInfo
    {
        public String Comment;
        public DateTime LastModified;
        public bool Checked;

        public CStructInfo() { }
        public CStructInfo(String comment, DateTime lastModified)
        {
            this.Comment = comment;
            this.LastModified = lastModified;
        }

        public CStructInfo(String comment, String lastModified)
        {
            this.Comment = comment;
            SetModified(lastModified);
        }

        public void SetModified(String DateTimeFormatString)
        {
            if (DateTime.TryParse(DateTimeFormatString, out LastModified) == false)
            {
                throw new Exception("시간형식이 맞지 않습니다. :" + DateTimeFormatString);
            }
        }
    }

}
