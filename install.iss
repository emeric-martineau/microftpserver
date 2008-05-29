[Setup]
AppName=Micro FTP Server
AppVerName=Micro FTP Server
DefaultDirName={pf}\Micro_FTP_Server
DefaultGroupName=Micro FTP Server
UninstallDisplayIcon={uninstallexe}
LicenseFile=license.gpl-3.0.txt
WizardImageFile=WizModernImage-IS.bmp
WizardSmallImageFile=WizModernSmallImage-IS.bmp
LanguageDetectionMethod=none
OutPutDir="..\MicroFTPServer-output"
OutputBaseFilename="windows-setup-en"

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[Types]
; Type d'installation
Name: "standard"; Description: "Standard installation";
Name: "full"; Description: "Full installation with tool for run NT service";
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "program"; Description: "Base core"; Types: full standard custom;
Name: "service"; Description: "NT service"; Types: full custom;

[Files]
Source: "MicroFTPServer\bin\Debug\MicroFTPServer.exe"; DestDir: "{app}"; Components: program;
Source: "license.gpl-3.0.txt"; DestDir: "{app}"; Components: program;
Source: "historique.txt"; DestDir: "{app}"; Components: program;
Source: "MD5Crypter\MD5Crypter\bin\Debug\MD5Crypter.exe"; DestDir: "{app}"; Components: program; 
Source: "MicroFTPServerGUI\MicroFTPServerGUI\bin\Debug\MicroFTPServerGUI.exe"; DestDir: "{app}"; Components: program;
Source: "MicroFTPServerGUI\GUIConfig.ini"; DestDir: "{app}"; Components: program;
Source: "MicroFTPServer\bin\Debug\general.ini"; DestDir: "{app}";  Components: program;
Source: "MicroFTPServer\bin\Debug\users\*.*"; DestDir: "{app}\users";  Components: program;
Source: "Documentation.txt"; DestDir: "{app}"; Components: program;
Source: "instsrv.exe"; DestDir: "{app}"; Components: service;
Source: "srvany.exe"; DestDir: "{app}"; Components: service;

[Icons]
Name: "{group}\FTP Server"; Filename: "{app}\MicroFTPServer.exe"; WorkingDir: "{app}"
Name: "{group}\Crypte password"; Filename: "{app}\MD5Crypter.exe"; WorkingDir: "{app}"
Name: "{group}\Graphic User Interface"; Filename: "{app}\MicroFTPServerGUI.exe"; WorkingDir: "{app}"
Name: "{group}\Licence"; Filename: "{app}\license.gpl-3.0.txt"; WorkingDir: "{app}"
Name: "{group}\Documentation"; Filename: "{app}\Documentation.txt"; WorkingDir: "{app}"
Name: "{group}\Uninstall MicroFTPServer"; Filename: "{uninstallexe}"

[INI]
;Filename: "{app}\GUIConfig.ini"; Section: "main"; Flags: uninsdeletesection
Filename: "{app}\GUIConfig.ini"; Section: "main"; Key: "config"; String: {app};
Filename: "{app}\GUIConfig.ini"; Section: "main"; Key: "server"; String: {app};
Filename: "{app}\GUIConfig.ini"; Section: "main"; Key: "HideSystray"; String: "no";
Filename: "{app}\GUIConfig.ini"; Section: "main"; Key: "RunAutomatically"; String: "no";
