using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RtwEnums.Network;

namespace NetworkModules3
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    public class ConnectionEventArgs : EventArgs
    {
        public int id { get; set; }
        
        public ConnType connType;
        public String comment;

        public ConnectionEventArgs(ConnType connType, String comment="",int id=-1)
            : base()
        {
            this.id = id;
            this.connType = connType;
            this.comment = comment;
        }
    }
}
