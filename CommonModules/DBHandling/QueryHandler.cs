using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBHandling
{
    public class QueryHandler
    {
        public static String getWhereMatches(String WhereField, String delemeter = "OR", params string[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++) 
                str += " " + WhereField + " " + delemeter + " " + values[i];
            return str;
        }

        public static String getWhereMatches(String WhereField, String delemeter = "OR", params Int32[] values)
        {
            string str = "";
            for (int i = 0; i < values.Length; i++)
            {
                if (i != 0) str += " " + delemeter + " ";
                str += " " + WhereField + "=" + String.Format("{0}", values[i]);
            }
            return str;
        }

    }
}
