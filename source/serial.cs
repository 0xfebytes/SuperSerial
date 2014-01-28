// free
using System;
using System.IO;
using System.IO.Ports;

namespace Serial
{
	public class Serial
	{
        static SerialPort commPort;

        private static void initSerial()
        {
            string line;
            string serialFile = "config/port.conf";
            
            try
            {
                using ( StreamReader serialConf =
                        new StreamReader(serialFile) )
                {
                    while ( (line = serialConf.ReadLine()) != null )
                    {
                        if ( line.Contains("port") )
                             setPortName(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("baud") )
                            setPortBaud(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("parity") )
                            setPortParity(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("dataBits") )
                            setPortData(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("stopBits") )
                            setPortStop(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("handshake") )
                            setPortHandshake(line.Substring(line.IndexOf("=") + 1));
                    }
                }
                
                commPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read: {0}", serialFile);
                Console.WriteLine(e.Message);
                System.Environment.Exit( 1 );
            }
        }

        // sets the port for serial
        public static void setPortName(string name)
        {
            commPort.PortName = name;
        }

        // sets the baud for serial
        public static void setPortBaud(string baud)
        {
            commPort.BaudRate = Convert.ToInt32(baud);
        }

        // sets the parity for serial
        public static void setPortParity(string parity)
        {
            commPort.Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
        }

        // sets the databits for serial
        public static void setPortData(string dataBits)
        {
            commPort.DataBits = Convert.ToInt32(dataBits);
        }

        // sets the stopbits for serial
        public static void setPortStop(string stopBits)
        {
            commPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        // sets the handshake for serial
        public static void setPortHandshake(string handshake)
        {
            commPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
    }
}
