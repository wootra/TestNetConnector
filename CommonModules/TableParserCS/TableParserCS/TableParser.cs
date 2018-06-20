using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TableParserCS
{
    public enum TableType { GST, IOT};
    public class TableParser
    {
        public TableParser()
        {

        }
        internal static int[] _index = new int[1];
        internal static int[] _type = new int[1];
        internal static byte[] _name = new byte[256];
        internal static byte[] _key = new byte[50];
        internal static Type[] _types = new Type[]{
            typeof(string), //char*
            typeof(short), //short
            typeof(int), //int
            typeof(int), //long
            typeof(float), //float
            typeof(double), //double
            typeof(ushort), //u_short
            typeof(uint), //u_int
            typeof(uint), //u_long
            null, //unknown
            typeof(byte), //char
            typeof(byte)}; //u_char

        internal static String[] _typesStr = new String[]{
            "char*", //char*
            "short", //short
            "int", //int
            "long", //long
            "float", //float
            "double", //double
            "unsigned short", //u_short
            "unsigned int", //u_int
            "unsigned long", //u_long
            "unknown", //unknown
            "char", //char
            "unsigned char"}; //u_char

        public static bool tpOpenTableFile(String filePath, TableType type)
        {
            return tpOpenTableFile(filePath, type.ToString()) == 1;
        }

        public static bool tpOpenTableFile(String filePath)
        {
            int lastp = filePath.LastIndexOf(".")+1;
            String type = filePath.Substring(lastp, filePath.Length - lastp);
            return tpOpenTableFile(filePath, type.ToString()) == 1;
        }
        
        public static bool tpGetVarPrev(out int eVarIndex, out String eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetVarPrev(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _typesStr[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        public static bool tpGetVarPrev(out int eVarIndex, out Type eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetVarPrev(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _types[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        public static bool tpGetVarNext(out int eVarIndex, out String eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetVarNext(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _typesStr[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        public static bool tpGetVarNext(out int eVarIndex, out Type eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetVarNext(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _types[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        public static bool tpGetCurVarInfo(out int eVarIndex, out String eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetCurVarInfo(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _typesStr[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        public static bool tpGetCurVarInfo(out int eVarIndex, out Type eVarType, out String eVarName, out String eKey)
        {
            int result = tpGetCurVarInfo(_index, _type, _name, _key);

            if (result == 0)
            {
                eVarIndex = -1;
                eVarType = null;
                eVarName = "";
                eKey = "";
                return false;
            }
            else
            {
                eVarIndex = _index[0];
                eVarType = _types[_type[0]];
                eVarName = Encoding.ASCII.GetString(_name).Trim("\0".ToCharArray());
                eKey = Encoding.ASCII.GetString(_key).Trim("\0".ToCharArray());
                return true;
            }
        }

        [DllImport("TablePaserApiR.dll")]
        extern internal static int tpOpenTableFile(String ePathFile, String eTableType); 
	    [DllImport("TablePaserApiR.dll")]
        extern public static int  tpSaveTableFile(String ePathFile) ; 
	    [DllImport("TablePaserApiR.dll")] 
        extern public static int  tpGetVarCount();
	    [DllImport("TablePaserApiR.dll")] 
        extern public static int  tpMoveFirstVar();
	    [DllImport("TablePaserApiR.dll")]
        extern public static int  tpMoveNextVar();
	    [DllImport("TablePaserApiR.dll")]
        extern internal static int tpGetVarNext(int[] eVarIndex, int[] eVarType, byte[] eVarName, byte[] ekey);
	    [DllImport("TablePaserApiR.dll")]
        extern public static int  tpMovePrevVar();
	    [DllImport("TablePaserApiR.dll")]
        extern internal static int tpGetVarPrev(int[] eVarIndex, int[] eVarType, byte[] eVarName, byte[] ekey);
	    [DllImport("TablePaserApiR.dll")]
        extern public static int  tpMoveLastVar();
	    [DllImport("TablePaserApiR.dll")]
        extern internal static int tpGetCurVarInfo(int[] eVarIndex, int[] eVarType, byte[] eVarName, byte[] ekey);
    }
    
    


}
