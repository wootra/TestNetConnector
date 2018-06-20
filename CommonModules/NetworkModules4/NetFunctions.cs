using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkModules4
{
    public static class NetFunctions
    {
        public static String getMyIP()
        {
            
            String hostName = Dns.GetHostName();
            IPAddress[] addrs = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ips in addrs)
            {
                if (ips.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ips.ToString().Equals("127.0.0.1")) continue;
                    if (ips.ToString().Equals("0.0.0.0")) continue;
                    
                    return ips.ToString();
                }
            }
            return "127.0.0.1";
        }
        public static String getIP(String IPOrHostName)
        {
            IPHostEntry myIPAddr = Dns.GetHostEntry(IPOrHostName);
            foreach (IPAddress ips in myIPAddr.AddressList)
            {
                if (ips.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ips.ToString();
                }
            }
            return null;
        }
        public static IPAddress getMyIP4Address()
        {
            
            IPHostEntry myIPAddr = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ips in myIPAddr.AddressList)
            {
                if (ips.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ips;
                }
            }
            return null;
        }
        public static IPAddress getIP4Address(String IPOrHostName)
        {
            IPAddress[] addrs= Dns.GetHostAddresses(IPOrHostName);
            //IPHostEntry myIPAddr = Dns.GetHostEntry(IPOrHostName);
            //foreach (IPAddress ips in myIPAddr.AddressList)
            foreach (IPAddress ips in addrs) 
            {
                if (ips.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ips;
                }
            }
            return null;
        }
    }
}
