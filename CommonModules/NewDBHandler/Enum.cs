using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewDBHandler
{
        public enum TextType { CHAR, VARCHAR, BINARY, TINYBLOB, MEDIUMBLOB, LONGBLOB, TINYTEXT, TEXT, MEDIUMTEXT, LONGTEXT, ENUM, SET };//문자형
        public enum NumType { TINYINT, SMALLINT, MEDIUMINT, INT, INTEGER, BIGINT, REAL, DOUBLE, FLOAT, DECIMAL, NUMERIC }; //숫자형
        public enum TimeType { DATETIME, DATE, TIME, YEAR, TIMESTAMP };//날짜형
}
