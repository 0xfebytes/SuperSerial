@echo off
set source="%~p0source"
csc /out:serialTool_x86.exe %source%\serialTool.cs %source%\serial.cs %source%\commands.cs %source%\Options.cs
