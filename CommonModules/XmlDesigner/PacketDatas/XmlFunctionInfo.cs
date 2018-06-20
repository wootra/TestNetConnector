using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlDesigner.PacketDatas
{
    public class XmlFunctionInfo
    {
        String Name;
        string[] Args;
            
        public XmlFunctionInfo(String functionString)
        {
            if (functionString.Length > 0 && functionString[0].Equals('#'))
            {
                functionString = functionString.Substring(1);
            }
            int brOpen = functionString.IndexOf("(");
            int brClose = functionString.LastIndexOf(")");
            Name = functionString.Substring(0, brOpen);
            String argText = functionString.Substring(brOpen + 1, brClose - brOpen - 1);
            Args = argText.Split(",".ToCharArray());
            for (int i = 0; i < Args.Length; i++)
            {
                Args[i] = Args[i].Trim();//앞뒤 빈칸 없애줌..
            }
        }
    }
}
