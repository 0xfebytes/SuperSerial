/*
 * SuperSerial is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SuperSerial is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with SuperSerial.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * Instructions:
 * 
 * This program allows writing bytes to a serial com.
 * Using the LCD namespace.
 * 
 * author: Tim Ryan
*/
using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using NDesk.Options;
using Serial;
using LCD;

namespace SuperSerial
{
	public class SerialTool
	{
        static Port serialComm;
        static Commands commands;

        /*
         * *****************************MAIN************************************
         */
		static int Main(string[] args)
		{
            // set up serial
            serialComm = new Port();
            serialComm.initSerial();

            // set up commands
            commands = new Commands();
            commands.initCommands(serialComm);

            // flags for options
            bool showHelp = false;
            bool printSentence = false;

            // get the options
            OptionSet options = new OptionSet()
                // short (common)
                .Add("b|backspace", "equivalent to backspace key", bs => commands.backspace())
                .Add("c|clear", "clears the LCD screen", c => commands.clear())
                .Add("displayleft", "shifts display x-1", dl => commands.displayLeft())
                .Add("displayright", "shifts display x+1", dr => commands.displayRight())
                .Add("home", "sets cursor position to 0,0", home => commands.cursorHome())
                .Add("i|info", "dipslays baud and i2c address", i => {
                        commands.displayBaud();
                        commands.displayI2c(); })
                .Add("l|left", "sets cursor position x-1", l => commands.cursorLeft())
                .Add("r|right", "sets cursor position x+1", r => commands.cursorRight())
                // long only (not used as much)
                .Add("baud=", "change the baud rate; default 9600"
                        , (byte baud) => commands.setBaud(baud))
                .Add("blink=", "turns blinking cursor on(1)/off(0); default off (0)"
                        , (int b) => commands.blink(b))
                .Add("brightness=", "change brightness 1-8; default 5"
                        , (byte br) => commands.display(br))
                .Add("contrast=", "change contrast 1-50; default 40"
                        , (byte w) => commands.setContrast(w))
                .Add("display=", "turns LCD on(1)/off(0); default off (0)"
                        , (int d) => commands.display(d))
                .Add("i2c=", "change the I2C address; default 0x50"
                        , (byte addr) => commands.setI2c(addr))
                .Add("position=", "sets the cursor position x,y (top left" +
                        "1,1)", (byte x, byte y) => commands.setCursor(x, y))
                .Add("underline=", "turns underline on(1)/off(0); default off (0)"
                        , (int u) => commands.underline(u))
                // help and default
                .Add("?|h|help", "displays this help", h => showHelp = true)
                .Add("<>", words => printSentence = true);

            // parse options
            try {
                options.Parse(args);
            } catch (OptionException e) {
                Console.Write("Error: ");
                Console.WriteLine(e.Message);
                showHelp = true;
            }

            // shows help if flag is set
            // otherwise prints text
            if ( showHelp )
                displayHelp(options);
            else if (printSentence)
                serialComm.commPort.Write(String.Join(" ", args));

			// keep the Console open
			Console.WriteLine("Press any key  to exit...");
			Console.ReadKey();

            // release comm
            serialComm.terminateSerial();

            return 0;
		}

        /*
         * *********************************MISC********************************
         */
        // prints the options
        public static void displayHelp(OptionSet options)
        {
            Console.WriteLine("Usage:\nserialTool [OPTIONS] <text>");
            Console.WriteLine("Sends options and / or text to the serial LCD module\n" +
                    "@COMX as specified in ports.conf\n");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions (Console.Out);
        }

	}
}
