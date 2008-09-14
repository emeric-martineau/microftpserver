@echo off

del unix_package /f /s /q
del unix_package.zip /q


md unix_package\users

copy "MD5Crypter\MD5Crypter\bin\Debug\MD5Crypter.exe" unix_package\
copy "MicroFTPServer\bin\Debug\MicroFTPServer.exe" unix_package\
copy "MicroFTPServerGUI\MicroFTPServerGUI\bin\Debug\MicroFTPServerGUI.exe" unix_package\
copy "MicroFTPServer\bin\Debug\general.ini" unix_package\
xcopy "MicroFTPServer\bin\Debug\users" unix_package\users /s /y
copy license.gpl-3.0.txt unix_package\
copy changelog.txt unix_package\
copy Documentation.txt unix_package\


"c:\program files\izarc\izarc.exe" -ad unix_package\*.*