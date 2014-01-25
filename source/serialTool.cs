using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;

namespace SerialTool
{
    // for all of the command information
    public class CommandsInfo
    {
	    public byte command, displayOn, displayOff,
	        cursorHome, underlineOn, underlineOff,
	        cursorLeft, cursorRight, blinkOn,
	        blinkOff, backspace, clear, contrast,
	        brightness, customChar, displayLeft,
	        displayRight, baudRate, i2cAddress,
	        displayBaud, displayI2c, setCursor;
	}

	public class hello
	{
        static SerialPort commPort;
        static CommandsInfo commands;

        /*
         * *****************************MAIN************************************
         */
		static int Main(string[] args)
		{
            // for storing commands
            commands = new CommandsInfo();
            // get command bytes from config file
            initCommands();

            // create serial
            commPort = new SerialPort();
            // set up serial
            initSerial();
            commPort.Open();

            clear();

			// keep the Console open
			Console.WriteLine("Press any key  to exit...");
			Console.ReadKey();

            // release comm
            commPort.Close();
            return 0;
		}

        /*
         * ***************************SERIAL************************************
         */
        // retrieves serial port info from config and sets it up
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

        /*
         * *********************************COMMANDS****************************
         */
        // writes command without additional arguments
        private static void command(byte cmd)
        {
            byte[] command = {commands.command, cmd};
            commPort.Write(command, 0, command.Length);
        }

        // writes command with additional arguments
        private static void command1(byte cmd, byte arg)
        {
            byte[] command = {commands.command, cmd, arg};
            commPort.Write(command, 0, command.Length);
        }

        // turns on the display
        public static void displayOn()
        {
            command(commands.displayOn);
        }

        // turns off the display
        public static void displayOff()
        {
            command(commands.displayOff);
        }

        // sets the cursor position
        // first position 1,1
        public static void setCursor(byte x, byte y)
        {
            // check values
            if (x < 1 || x > 20 || y < 1 || y > 4)
            {
                Console.WriteLine("Bad cursor position.\nCursor x: 1 - 20\n" +
                        "Cursor y: 1 - 4");
                return;
            }

            byte position = 0x00;
            if (y == 1)
                position = (byte)(0x00 + x - 1);
            else if (y == 2)
                position = (byte)(0x40 + x - 1);
            else if (y == 3)
                position = (byte)(0x14 + x - 1);
            else if (y == 4)
                position = (byte)(0x54 + x - 1);

            command1(commands.setCursor, position);
        }

        // sets cursor home
        public static void cursorHome()
        {
            command(commands.cursorHome);
        }

        // turns on underline
        public static void underlineOn()
        {
            command(commands.underlineOn);
        }

        // turns off underline
        public static void underlineOff()
        {
            command(commands.underlineOff);
        }

        // move cursor left
        public static void cursorLeft()
        {
            command(commands.cursorLeft);
        }

        // move cursor right
        public static void cursorRight()
        {
            command(commands.cursorRight);
        }

        // turns on blink
        public static void blinkOn()
        {
            command(commands.blinkOn);
        }

        // turns off blink
        public static void blinkOff()
        {
            command(commands.blinkOff);
        }

        // backspace
        public static void backspace()
        {
            command(commands.backspace);
        }

        // clear screen
        public static void clear()
        {
            command(commands.clear);
        }

        // sets the contrast
        // 1-50, default 50
        public static void setContrast(byte val)
        {
            command1(commands.contrast, val);
        }

        // sets the brightness
        // 1-8, default 5
        public static void setBrightness(byte val)
        {
            command1(commands.brightness, val);
        }

        // loads custom char
        // 9 additional bytes
        // 1st byte address - 1-7
        // other 8 patter
        public static void loadCustomChar(byte addr, byte[] character)
        {
            command1(commands.customChar, addr);
            commPort.Write(character, 0, character.Length);
        }

        // move display left
        public static void displayLeft()
        {
            command(commands.displayLeft);
        }

        // move display right
        public static void displayRight()
        {
            command(commands.displayRight);
        }

        // sets the baudrate
        // 1-8, default 4 (9600)
        public static void setBaud(byte val)
        {
            if (val > 8)
            {
                Console.WriteLine("Invalid Baud Rate\n1 - 300\n2 - 1200\n" +
                        "3 - 2400\n4 - 9600\n5 - 14400\n6 - 19200\n" +
                        "7 - 57600\n8 - 115200");
                return;
            }
            
            command1(commands.baudRate, val);
        }

        // sets the I2C address
        // 0x00 - 0xfe, default 0x50
        public static void setI2C(byte val)
        {
            command1(commands.i2cAddress, val);
        }

        // display baudrate
        public static void displayBaud()
        {
            command(commands.displayBaud);
        }

        // display i2c address
        public static void displayI2c()
        {
            command(commands.displayI2c);
        }

        /*
         * ********************************CONFIG*******************************
         */
        // retrieves the commands from the config file
        private static void initCommands()
        {
            string line;
            string commandsFile = "config/commands.conf";
            
            try
            {
                using ( StreamReader commandsConf =
                        new StreamReader(commandsFile) )
                {
                    while ( (line = commandsConf.ReadLine()) != null )
                    {
                        if ( line.Contains("command") )
                            commands.command = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayOn") )
                            commands.displayOn = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayOff") )
                            commands.displayOff = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorHome") )
                            commands.cursorHome = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("underlineOn") )
                            commands.underlineOn = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("underlineOff") )
                            commands.underlineOff = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorLeft") )
                            commands.cursorLeft = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorRight") )
                            commands.cursorRight = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("blinkOn") )
                            commands.blinkOn = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("blinkOff") )
                            commands.blinkOff = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("backspace") )
                            commands.backspace = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("clear") )
                            commands.clear = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("contrast") )
                            commands.contrast = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("brightness") )
                            commands.brightness = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("customChar") )
                            commands.customChar = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayLeft") )
                            commands.displayLeft = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayRight") )
                            commands.displayRight = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("baudRate") )
                            commands.baudRate = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("i2dAddress") )
                            commands.i2cAddress = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayBaud") )
                            commands.displayBaud = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayI2c") )
                            commands.displayI2c = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("setCursor") )
                            commands.setCursor = stringToByte(line.Substring(line.IndexOf("0x"), 4));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read: {0}", commandsFile);
                Console.WriteLine(e.Message);
                System.Environment.Exit( 1 );
            }
        }

        /*
         * *********************************MISC********************************
         */
        // converts hex string to byte
        private static byte stringToByte(string hex)
        {
             return (byte)Convert.ToInt16(hex, 16);
        }

	}
}
