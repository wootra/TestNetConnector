using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TableParserCS;
            
namespace TestPrj
{
    class Program
    {
        static void Main(string[] args)
        {
            String gstFile = @"C:\RTNgine\config\IOT\IGTE.IOT";
            
                if (TableParser.tpOpenTableFile(gstFile.Trim()))
                {
                    int count = TableParser.tpGetVarCount();
                    
                    int reslut = TableParser.tpMoveFirstVar();
                    int valIndex = 0;
                    Type valType = null;

                    String eVarName = "";

                    String eKey = "";
                    
                    while (TableParser.tpGetVarNext(out valIndex, out valType, out eVarName, out eKey))
                    {
                        
                        
                        Console.WriteLine("{0}:{1}:{2}/{3}", valIndex, valType, eVarName, eKey);// Encoding.ASCII.GetString(eKey).Trim());
                    }

                }
                Console.Read();

            

        }
    }
}
