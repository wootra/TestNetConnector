using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModules
{
    public delegate void DataReceivedEventHandler( object sender, DataReceivedEventArgs e);

    public class DataReceivedEventArgs:EventArgs
    {
        private HEADER_RESPONSE _header;
        public int id { get; set; }
        public HEADER_RESPONSE header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }
        public DataReceivedEventArgs(int iid, HEADER_RESPONSE iheader):base()
        {
            _header = new HEADER_RESPONSE();
            id = iid;
            _header.CopyFrom(iheader);
        }
    }
}
