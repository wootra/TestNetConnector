using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace NetworkModules5
{
    public class SerialConn : SerialPort
    {
        bool _continue;
        Thread _readThread;

        public SerialConn()
        {
            _continue = true;

            ReadTimeout = 500;
            WriteTimeout = 500;
        }
        ~SerialConn()
        {
            try
            {
                Close();
                this.Dispose(true);
            }
            catch (Exception) { }
            try
            {
                _readThread.Abort();
            }
            catch (Exception) { }
        }
        public void setTimeout(int readTimeout, int writeTimeout)
        {
            ReadTimeout = readTimeout;
            WriteTimeout = writeTimeout;
        }

        public void openPort(string portName="COM1", int baudRate = 9600, Parity parity=Parity.None, int dataBits=8, StopBits stopBits=StopBits.One, Handshake handShake=Handshake.None){
            setPort(portName, baudRate, parity, dataBits, stopBits, handShake);
            try
            {
                this.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.Close();
                //this.Open();
            }
        }

        public void setPort(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handShake = Handshake.None)
        {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
            Handshake = handShake;
        }

        public int read(Byte[] buff, int offset=0, int dataSize = 0)
        {
            return Read(buff, offset, (dataSize == 0) ? buff.Length : dataSize);
        }

        public void write(Byte[] buff, int offset = 0, int dataSize = 0)
        {
            Write(buff, offset, (dataSize == 0) ? buff.Length : dataSize);
        }
        public void startReadThread()
        {
            _continue = true;
            _readThread = new Thread(new ThreadStart(ReadThread));
            _readThread.Start();
        }

        public void startEchoReadThread()
        {
            _continue = true;
            _readThread = new Thread(new ThreadStart(EchoReadThread));
            _readThread.Start();
        }

        public void stopReadThread()
        {
            _continue = false;
            _readThread.Join();
        }

        public void testReadWrite()
        {
            if (this.IsOpen == false)
            {
                Console.WriteLine("SerialPort is closed... now quit the program.");
                return;
            }

            _continue = true;
            String message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            startReadThread();

            Console.WriteLine("[ "+PortName+"] Client started...");
            while (_continue)
            {
                //Console.Write("[ "+PortName+"] sending : ");
                message = Console.ReadLine();


                if (stringComparer.Equals("quit", message))
                {
                    _continue = false;
                }

                else
                {
                    try
                    {
                        WriteLine(String.Format("<{0}>: {1}", PortName, message));
                    }
                    catch (TimeoutException) { }

                }
            }

        }
        public void TestReadWriteWithDinamicSet()
        {
            if (this.IsOpen == false)
            {
                Console.WriteLine("SerialPort is closed... now quit the program.");
                return;
            }
            _continue = true;
            String message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            PortName = SetPortName(PortName);
            BaudRate = SetPortBaudRate(BaudRate);
            Parity = SetPortParity(Parity);
            DataBits = SetPortDataBits(DataBits);
            StopBits = SetPortStopBits(StopBits);
            Handshake = SetPortHandshake(Handshake);

            Open();

            startReadThread();

            Console.WriteLine("[ "+PortName+"] Client started...");
            while (_continue)
            {
                
                message = Console.ReadLine();


                if (stringComparer.Equals("quit", message))
                {
                    _continue = false;
                }

                else
                {
                    try
                    {
                        WriteLine(String.Format("<{0}>: {1}", PortName, message));
                    }
                    catch (TimeoutException) { }

                }
            }
            _readThread.Join();
            Close();

        }
        protected virtual void ReadThread()
        {
            if (this.IsOpen == false)
            {
                Console.WriteLine("SerialPort is closed... now quit the program.");
                return;
            }
            Console.WriteLine("[ " + PortName + "] Read is runnning...");
            string message = "";
            while (_continue)
            {
                //Console.Write("["+PortName+"] ");
                try
                {
                    message = ReadLine();
                    Console.WriteLine("[ " + PortName + "] read : " + message);
                    message = "";
                }

                catch (TimeoutException)
                {
                    //Console.WriteLine("timeout!:" + e.Message);
                }
            }
            Console.WriteLine("[" + PortName + "]ReadThread Finished...");
        }

        protected virtual void EchoReadThread()
        {
            if (this.IsOpen == false)
            {
                Console.WriteLine("SerialPort is closed... now quit the program.");
                return;
            }
            Console.WriteLine("[ " + PortName + "] Echo Read is runnning...");
            string message = "";
            while (_continue)
            {
                try
                {
                    message = ReadLine();
                    Console.WriteLine("[ " + PortName + "] read and Returned : " + message);
                    WriteLine("<"+PortName+">"+message);
                    message = "";
                }

                catch (TimeoutException)
                {
                    //Console.WriteLine("timeout!:" + e.Message);
                }
            }
            Console.WriteLine("["+PortName+"]ReadThread Finished...");

        }


        /*
        public  void test()
        {
            string name;

            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(ReadThread);


            // Create a new SerialPort object with default settings.
           // _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = SetPortName(_serialPort.PortName);
            _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            _serialPort.Parity = SetPortParity(_serialPort.Parity);
            _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
            //_serialPort.Encoding = Encoding.Default;
        

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;


                _serialPort3 = new SerialPort();

                // Allow the user to set the appropriate properties.
                _serialPort3.PortName = SetPortName("COM3");
                _serialPort3.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
                _serialPort3.Parity = SetPortParity(_serialPort.Parity);
                _serialPort3.DataBits = SetPortDataBits(_serialPort.DataBits);
                _serialPort3.StopBits = SetPortStopBits(_serialPort.StopBits);
                _serialPort3.Handshake = SetPortHandshake(_serialPort.Handshake);
                // _serialPort3.Encoding = Encoding.Default;
          

                // Set the read/write timeouts
                _serialPort3.ReadTimeout = 500;
                _serialPort3.WriteTimeout = 500;
                _serialPort3.Open();
 
            _serialPort.Open();

            _continue = true;


            Console.Write("Name: ");
            name = Console.ReadLine();
            //readThread.Start();

            Console.WriteLine("Type QUIT to exit");


            while (_continue)
            {
                message = Console.ReadLine();


                if (stringComparer.Equals( "quit", message))
                {
                    _continue = false;
                }

                else
                {
                    try
                    {
                        _serialPort.WriteLine(String.Format("<{0}>: {1}\n", name, message));
                    }
                    catch (TimeoutException) { }
                    
                }
            }

            //readThread.Join();
            _serialPort.Close();
        }

        */



        public  string SetPortName(
        string defaultPortName)
        {

            string portName;

            Console.WriteLine("Available Ports:");

            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine( "   {0}", s);
            }

            Console.Write("COM port({0}): ", defaultPortName);
            portName = Console.ReadLine();


            if (portName == "")
            {
                portName = defaultPortName;
            }

            return portName;
        }


        public  int SetPortBaudRate(int defaultPortBaudRate)
        {

            string baudRate;

            Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();


            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }


            return int.Parse(baudRate);
        }


        public  Parity SetPortParity(Parity defaultPortParity)
        {

            string parity;

            Console.WriteLine("Available Parity options:");

            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            parity = Console.ReadLine();


            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }


            return (Parity)Enum.Parse(typeof(Parity), parity);
        }


        public  int SetPortDataBits(int defaultPortDataBits)
        {

            string dataBits;

            Console.Write("Data Bits({0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();


            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }


            return int.Parse(dataBits);
        }


        public  StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {

            string stopBits;

            Console.WriteLine("Available Stop Bits options:");

            foreach (
            string s
            in Enum.GetNames(
            typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write(
                    "Stop Bits({0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();


            if (stopBits =="")
            {
                stopBits = defaultPortStopBits.ToString();
            }


            return (StopBits)Enum.Parse(
            typeof(StopBits), stopBits);
        }


        public  Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {

            string handshake;

            Console.WriteLine("Available Handshake options:");

            foreach (
            string s
            in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("HandShake({0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();


            if (handshake =="")
            {
                handshake = defaultPortHandshake.ToString();
            }


            return (Handshake)Enum.Parse(typeof(Handshake), handshake);
        }
    }
}
