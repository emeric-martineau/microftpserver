@echo off

set SOURCE_PATH=C:\Documents and Settings\m56emar\Mes documents\Visual Studio 2008\Projects\MicroFTPServeur\MicroFTPServeur
set MONO_PATH=C:\Program Files\Mono-1.9.1\bin
set MONO_EXTERNAL_ENCODINGS=default_locale
set MONO_ARGUMENTS_LIBRARY=-r:ClassIni.dll
set MONO_ARGUMENTS_MAIN=-r:ClassFTPServer.dll - r:ClassIni.dll

del ClassIni.dll

call "%MONO_PATH%\gmcs.bat" "%SOURCE_PATH%\ClassIni.cs" -out:ClassIni.dll -target:library %MONO_ARGUMENTS_LIBRARY%

rem del ClassFTPServer.dll

rem call "%MONO_PATH%\gmcs.bat" "%SOURCE_PATH%\ClassFTPServer.cs" -out:ClassFTPServer.dll -target:library %MONO_ARGUMENTS_LIBRARY%

rem del MainForm.dll

rem call "%MONO_PATH%\gmcs.bat" "%SOURCE_PATH%\Program.cs" -out:MicroFTPServer.exe %MONO_ARGUMENTS_FORM_MAIN%

pause