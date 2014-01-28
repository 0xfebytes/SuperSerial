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
 * "config/commands.conf" where config is a folder in the
 * root of the application folder.
 * 
 * author: Tim Ryan
*/

using System;
using System.IO;
using Serial;

namespace LCD
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

    // all of the commands and setup
	public class Commands
	{
        // serial port and commands class need
        // to be accessible to all functions
        Port serialComm;
        CommandsInfo commands;

        /*
         * ********************************CONFIG*******************************
         */
        // retrieves the commands from the config file
        public void initCommands(Port p)
        {
            // bring in the serial port
            serialComm = p;
            commands = new CommandsInfo();

            // for reading form the file
            string line;
            string commandsFile = "config/commands.conf";
            
            // read all setting from our config
            try
            {
                using ( StreamReader commandsConf =
                        new StreamReader(commandsFile) )
                {
                    // revisit; think about using reflections and doing
                    // this with a foreach loop (add string[] to commandsInfo)
                    while ( (line = commandsConf.ReadLine()) != null )
                    {
                        if ( line.Contains("command") )
                            commands.command =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayOn") )
                            commands.displayOn =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayOff") )
                            commands.displayOff =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorHome") )
                            commands.cursorHome =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("underlineOn") )
                            commands.underlineOn =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("underlineOff") )
                            commands.underlineOff =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorLeft") )
                            commands.cursorLeft =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("cursorRight") )
                            commands.cursorRight =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("blinkOn") )
                            commands.blinkOn =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("blinkOff") )
                            commands.blinkOff =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("backspace") )
                            commands.backspace =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("clear") )
                            commands.clear =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("contrast") )
                            commands.contrast =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("brightness") )
                            commands.brightness =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("customChar") )
                            commands.customChar =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayLeft") )
                            commands.displayLeft =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayRight") )
                            commands.displayRight =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("baudRate") )
                            commands.baudRate =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("i2dAddress") )
                            commands.i2cAddress =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayBaud") )
                            commands.displayBaud =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("displayI2c") )
                            commands.displayI2c =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                        else if ( line.Contains("setCursor") )
                            commands.setCursor =
                                stringToByte(line.Substring(line.IndexOf("0x"), 4));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read: {0}", commandsFile);
                Console.WriteLine(e.Message);
            }
        }
        /*
         * ********************************COMMANDS*****************************
         */

        // writes bytes to serial port
        private void command(byte cmd)
        {
            byte[] command = { commands.command, cmd };
            serialComm.commPort.Write(command, 0, command.Length);
        }

        // writes command with additional arguments
        private void command1(byte cmd, byte arg)
        {
            byte[] command = { commands.command, cmd, arg };
            serialComm.commPort.Write(command, 0, command.Length);
        }

        // turns on/off the display
        public void display(int val)
        {
            if ( val == 0 )
                command(commands.displayOff);
            else
                command(commands.displayOn);
        }

        // sets the cursor position
        // first position 1,1
        public void setCursor(byte x, byte y)
        {
            // check values
            if ( x < 1 || x > 20 || y < 1 || y > 4 )
            {
                Console.WriteLine("Bad cursor position.\nCursor x: 1 - 20\n" +
                        "Cursor y: 1 - 4");
                return;
            }

            byte position = 0x00;
            if ( y == 1 )
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
        public void cursorHome()
        {
            command(commands.cursorHome);
        }

        // turns on/off underline
        public void underline(int val)
        {
            if ( val == 0 )
                command(commands.underlineOff);
            else
                command(commands.underlineOn);
        }

        // move cursor left
        public void cursorLeft()
        {
            command(commands.cursorLeft);
        }

        // move cursor right
        public void cursorRight()
        {
            command(commands.cursorRight);
        }

        // turns on/off blink
        public void blink(int val)
        {
            if ( val == 0 )
                command(commands.blinkOff);
            else
                command(commands.blinkOn);
        }

        // backspace
        public void backspace()
        {
            command(commands.backspace);
        }

        // clear screen
        public void clear()
        {
            command(commands.clear);
        }

        // sets the contrast
        // 1-50, default 40
        public void setContrast(byte val)
        {
            command1(commands.contrast, val);
        }

        // sets the brightness
        // 1-8, default 5
        public void setBrightness(byte val)
        {
            command1(commands.brightness, val);
        }

        // loads custom char
        // 9 additional bytes
        // 1st byte address - 1-7
        // other 8 patter
        public void loadCustomChar(byte addr, byte[] character)
        {
            command1(commands.customChar, addr);
            serialComm.commPort.Write(character, 0, character.Length);
        }

        // move display left
        public void displayLeft()
        {
            command(commands.displayLeft);
        }

        // move display right
        public void displayRight()
        {
            command(commands.displayRight);
        }

        // sets the baudrate
        // 1-8, default 4 (9600)
        public void setBaud(byte val)
        {
            if ( val > 8 )
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
        public void setI2c(byte val)
        {
            command1(commands.i2cAddress, val);
        }

        // display baudrate
        public void displayBaud()
        {
            command(commands.displayBaud);
        }

        // display i2c address
        public void displayI2c()
        {
            command(commands.displayI2c);
        }

        /*
         * *********************************MISC********************************
         */
        // converts hex string to byte
        private byte stringToByte(string hex)
        {
             return (byte)Convert.ToInt16(hex, 16);
        }
	}
}
