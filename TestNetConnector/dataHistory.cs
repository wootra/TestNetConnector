using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestNetConnector
{
    public class dataHistory
    {
        public int size { set; get; }
        public String ip { set; get; }
        public String data { set; get; }
        public dataHistory(int size, String data, String ip="...")
        {
            this.ip = ip;
            this.size = size;
            this.data = data;
        }
    }
}
