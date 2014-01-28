/*
 * This file is part of the Spark Windows Library.
 * 
 * The Spark Windows Library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * The Spark Windows Library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with the Spark Windows Library.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * Instructions:
 * 
 * This is intended to be used with a configuration file
 * "config/port.conf" where config is a folder in the
 * root of the application folder.  Additionally, the
 * functions should be used instead of accessing the
 * serial port directly.  The only exception is writing
 * to the serial port, which should be done by accessing
 * commPort.Write()
 * 
 * author: Tim Ryan
*/

using System;
using System.IO;
using System.IO.Ports;

namespace Serial
{
	public class Port
	{
        // accessible to all functions
        public SerialPort commPort;

        // scans configuration file for information
        // starts the serial connection
        public void initSerial()
        {
            // for checking the conf file
            string line;
            string serialFile = "config/port.conf";

            commPort = new SerialPort();
            
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
                            setPortDataBits(line.Substring(line.IndexOf("=") + 1));
                        else if ( line.Contains("stopBits") )
                            setPortStopBits(line.Substring(line.IndexOf("=") + 1));
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
            }
        }

        // close the serial connections
        public  void terminateSerial()
        {
            commPort.Close();
        }

        // sets the port for serial
        public  void setPortName(string name)
        {
            commPort.PortName = name;
        }

        // sets the baud for serial
        public  void setPortBaud(string baud)
        {
            commPort.BaudRate = Convert.ToInt32(baud);
        }

        // sets the parity for serial
        public  void setPortParity(string parity)
        {
            commPort.Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
        }

        // sets the databits for serial
        public  void setPortDataBits(string dataBits)
        {
            commPort.DataBits = Convert.ToInt32(dataBits);
        }

        // sets the stopbits for serial
        public  void setPortStopBits(string stopBits)
        {
            commPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        // sets the handshake for serial
        public  void setPortHandshake(string handshake)
        {
            commPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
    }
}
