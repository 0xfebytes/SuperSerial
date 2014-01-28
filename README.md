SuperSerial
===========

A cli serial interface for windows optimized for controlling serial LCD modules (up to 4 lines, 20 characters)

Additional support for modules to come.

I created this so that I could do something like:
echo -en '\xfe\x51' >> /dev/ttyAMA0

So for this application edit the port.conf file in the conf folder.
The command will be:
serialTool_x86 <text>

Use serialTool_x86 -h to view options.

This is still a work in progress.
