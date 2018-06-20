using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHandling
{
    public class DataComparer
    {
        /// <summary>
        /// fromValue에서 minusValue를 뺀다.
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="fromValue"></param>
        /// <param name="minusType"></param>
        /// <param name="minusValue"></param>
        /// <returns></returns>
        public static double Minus(string fromType, string fromValue, string minusType, string minusValue){
            
            if (fromType.ToLower().Equals("string") || minusType.ToLower().Equals("string")) //string은 길이를 비교한다.
            {
                return (double)(fromValue.Length - minusValue.Length);
            }
            double fromD;
            double minusD;
            long fromLong;
            long minusLong;

            if(double.TryParse(fromValue, out fromD)==false){
                if(long.TryParse(fromValue, System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("ko-KR", true),  out fromLong)){
                    fromD = fromLong;
                }
            }
            if(double.TryParse(minusValue, out minusD)==false){
                if(long.TryParse(minusValue, System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("ko-KR", true),  out minusLong)){
                    minusD = minusLong;
                }
            }
            return fromD - minusD;
        }

       
    }
}
