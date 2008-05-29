/*
 * µLeechFTPServer
 * 
 * A little FTP server in .Net technologie
 * 
 * CopyRight MARTINEAU Emeric (C) 2008
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.See the GNU GENERAL PUBLIC LICENSE for more
 * details.
 *
 * You should have received a copy of the GNU GENERAL PUBLIC LICENSE along
 * with this program; if not, write to the Free Software Foundation, Inc., 59
 * Temple Place, Suite 330, Boston, MA 02111-1307 USA.
 * 
 * ****************************************************************************
 * Attributs :
 *  - Root : root path to find config,
 *  - Port : port to listen,
 *  - IPAdress : IP adress of server,
 *  - OnLog : function to call to log,
 *  - Cancel : stop server,
 *  
 * Methode :
 *  - Start() : start server,
 * ****************************************************************************
 * FTP Command
 * [OK] USER
 * [OK] PASS
 * ACCT
 * [OK] CWD
 * [OK] CDUP XCUP
 * SMNT
 * REIN
 * [OK] QUIT
 * [OK] PORT
 * [OK] PASV
 * [OK] TYPE
 * STRU
 * [OK] MODE
 * [OK] RETR
 * [OK] STOR
 * STOU
 * [OK] APPE file name - APPEND (with create) ()       501 Permission Denied
 * ALLO
 * REST
 * RNFR       Rename from.	        bool Rename
 * RNTO       501 Permission Denied bool Rename
 * ABOR
 * DELE       501 Permission Denied bool Delete
 * RMD XRMD      501 Permission Denied bool DeleteDirectory
 * MKD XMKD      501 Permission Denied bool MakeDirectory
 * [OK] PWD XPWD
 * [OK] LIST
 * [OK] NLST
 * SITE
 * [OK] SYST
 * STAT
 * HELP
214-The following commands are recognized:
   USER   PASS   QUIT   CWD    PWD    PORT   PASV   TYPE
   LIST   REST   CDUP   RETR   STOR   SIZE   DELE   RMD
   MKD    RNFR   RNTO   ABOR   SYST   NOOP   APPE   NLST
   MDTM   XPWD   XCUP   XMKD   XRMD   NOP    EPSV   EPRT
   AUTH   ADAT   PBSZ   PROT   FEAT   MODE   OPTS   HELP
   ALLO   MLST   MLSD   SITE   P@SW   STRU   CLNT   MFMT
214 Have a nice day.
help utf8
502 Command UTF8 is not recognized or supported by FileZilla Server
help user
214 Command USER is supported by FileZilla Server
 
 * [OK] NOOP
 * FEAT
 *  211-Features:
     MDTM
          -> MDTM 2.zip  -> File.GetLastWriteTime()
             213 20080526101721
             Syntax: MDTM remote-filename
             Returns the last-modified time of the given file on the remote host in the format "YYYYMMDDhhmmss": YYYY is the four-digit year, MM is the month from 01 to 12, DD is the day of the month from 01 to 31, hh is the hour from 00 to 23, mm is the minute from 00 to 59, and ss is the second from 00 to 59. 
     REST STREAM
     SIZE
     UTF8 ON|OFF -> passe en mode utf8 les lignes de commande : 200 UTF8 mode enabled : 200 UTF8 mode disabled : 501 Invalid UFT8 options
     CLNT <ckient name> - Send FTP Client Name to server. : 200 Don't care
     MFMT bool ModifyTime
         mfmt 20080101121212 consolidation.exe
         213 modify=20080101121212; /consolidation.exe
         501 Not a valid date
         501 Syntax error
         MSG_TRANSFERT_COMPLET
         ShowError(2)
    211 End
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;

namespace ConsoleApplication1
{
    class ClassFTPServer
    {
        public String RootConfigDirectory = "";
        public int Port = 21;
        public String IPAddress = "127.0.0.1";
        public String WelcomeMessage = "";
        public int MaxSessionPerUser = 2;
        public bool Cancel = false;
        public int MaxClient = 512;
        public bool FullLog = false;
        public int TimeOut = 45;
        public int PassivePortStart;
        public int PassivePortStop;

        public delegate void LogFunc(String Text);
        public LogFunc OnLog = null;

        private IPAddress ipAddress;
        private int NbClient = 0;
        private Socket ClientIP;
        private String[] MonthLabel = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private List<int> PassivePortFree = new List<int>();
        private List<int> PassivePortUse = new List<int>();
        private List<String> ConnectedUser = new List<String>();
        private List<int> NbSessionPerUser = new List<int>();
        private Object LockThread = new Object();
        private String[] AllowIPAddress ;
        private String[] DenyIPAddress;
        /* Encoding for translate è à é*/ 
        private Encoding WindowsEncoding = Encoding.GetEncoding(1252);

        /*
         * Constante message
         */
        private const String ERROR_CONFIG_FILE_NOT_FOUND = "** ERROR ** Config file not found.";
        private const String ERROR_INVALID_PORT = "** ERROR ** Invalid port. Must be 0 to 65535.";
        private const String ERROR_INVALID_IP_ADRESS = "** ERROR ** Invalid IP address.";
        private const String ERROR_INVALID_MAX_SESSION_PER_USER = "** ERROR ** Is not a valide number in MaxSessionPerUser.";
        private const String ERROR_INVALID_TIME_OUT = "** ERROR ** Is not a valide number in TimeOut.";
        private const String ERROR_CANT_OPEN_PORT = "** ERROR ** Can't open port. Maybe already use ?";
        private const String ERROR_INVALID_PASSIVE_PORT = "** ERROR ** Is not a valide range number in PassivePort.";
        private const String ERROR_PERMISSION_DENIED = "501 Permission Denied.";
        private const String LOG_NEW_CONNECTION = "[{0}] New connection";

        private const String MSG_TRANSFERT_COMPLET = "226 Transfer complete.";
        private const String MSG_FILE_NOT_FOUND = "550 {0}: No such file or directory.";

        /*
         * Constant
         */
        private const String EOL = "\r\n";
        private const int BLOCK_SIZE = 512;

        /*
         * Constructor
         */
        public ClassFTPServer(String RootFile)
        {
            this.RootConfigDirectory = AddDirSeparatorAtEnd(RootFile, Path.DirectorySeparatorChar);
        }

        /*
         * Constructor
         */
        public ClassFTPServer()
        {            
        }

        /*
         * Log function
         */
        private void Log(String Text)
        {
            if (OnLog != null)
            {
                OnLog(Text);
            }
        }

        /*
         * Start connexion
         */
        public void Start()
        {
            ClassIniReader GeneralIni = new ClassIniReader(RootConfigDirectory + "general.ini");
            String tmp;
            int i;
            char[] PassivePortSeparateur = { '-' };
            String[] tmpPort;
            char[] IPSeparator = { ',' };

            if (GeneralIni.FileExists == true)
            {
                tmp = GeneralIni.GetValue("main", "port");

                if (int.TryParse(tmp, out this.Port) == true)
                {
                    if ((this.Port >= 0) && (this.Port < 65536))
                    {
                        try
                        {
                            this.IPAddress = GeneralIni.GetValue("main", "IPAddress");
                            this.ipAddress = Dns.GetHostEntry(this.IPAddress).AddressList[0];

                            this.WelcomeMessage = GeneralIni.GetValue("main", "WelcomeMessage");

                            this.WelcomeMessage = this.WelcomeMessage.Replace("\\n", EOL + "220-") ;

                            if (this.WelcomeMessage.StartsWith("220-") == false)
                            {
                                this.WelcomeMessage = "220-" + this.WelcomeMessage;
                            }

                            if (this.WelcomeMessage.EndsWith(EOL) == false)
                            {
                                this.WelcomeMessage += EOL ;
                            }

                            this.WelcomeMessage += "220 ";

                            tmp = GeneralIni.GetValue("main", "MaxSessionPerUser");

                            if (int.TryParse(tmp, out this.MaxSessionPerUser) == true)
                            {
                                tmp = GeneralIni.GetValue("main", "MaxClient");

                                if (int.TryParse(tmp, out this.MaxClient) == true)
                                {
                                    tmp = GeneralIni.GetValue("main", "FullLog");

                                    if (tmp.ToLower() == "yes")
                                    {
                                        this.FullLog = true;
                                    }
                                    else
                                    {
                                        this.FullLog = false;
                                    }

                                    tmp = GeneralIni.GetValue("main", "TimeOut");

                                    if (int.TryParse(tmp, out this.TimeOut) == true)
                                    {
                                        tmp = GeneralIni.GetValue("main", "PassivePort");

                                        tmpPort = tmp.Split(PassivePortSeparateur);

                                        if (tmpPort.Length == 2)
                                        {
                                            if (int.TryParse(tmpPort[0], out this.PassivePortStart) == true)
                                            {
                                                if (int.TryParse(tmpPort[1], out this.PassivePortStop) == true)
                                                {
                                                    for (i = this.PassivePortStart; i < this.PassivePortStop; i++)
                                                    {
                                                        this.PassivePortFree.Add(i);
                                                    }

                                                    tmp = GeneralIni.GetValue("main", "AllowIPAddress");

                                                    /* Convert to RegEx */
                                                    tmp = tmp.Replace("?", "[0-9]");
                                                    tmp = tmp.Replace("*", "[0-9]*");

                                                    this.AllowIPAddress = tmp.Split(IPSeparator);

                                                    tmp = GeneralIni.GetValue("main", "DenyIPAddress");

                                                    /* Convert to RegEx */
                                                    tmp = tmp.Replace("?", "[0-9]");
                                                    tmp = tmp.Replace("*", "[0-9]*");

                                                    this.DenyIPAddress = tmp.Split(IPSeparator);

                                                    Run();
                                                }
                                                else
                                                {
                                                    Log(ERROR_INVALID_PASSIVE_PORT);
                                                }
                                            }
                                            else
                                            {
                                                Log(ERROR_INVALID_PASSIVE_PORT);
                                            }
                                        }
                                        else
                                        {
                                            Log(ERROR_INVALID_PASSIVE_PORT);
                                        }                                        
                                    }
                                    else
                                    {
                                        Log(ERROR_INVALID_TIME_OUT);
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                Log(ERROR_INVALID_MAX_SESSION_PER_USER);
                            }
                        }
                        catch
                        {
                            Log(ERROR_INVALID_IP_ADRESS);
                        }
                    }
                    else
                    {
                        Log(ERROR_INVALID_PORT);
                    }
                }
                else
                {
                    Log(ERROR_INVALID_PORT);
                }
            }
            else
            {
                Log(ERROR_CONFIG_FILE_NOT_FOUND);
            }
        }

        /*
         * Run server
         */
        private void Run()
        {
            IPEndPoint ep = new IPEndPoint(this.ipAddress, this.Port);
            TcpListener tl = new TcpListener(ep);
            Thread ClientThread;
            int TimeOutUser = this.TimeOut;
            System.Text.RegularExpressions.Match MatchRegEx;
            int i;
            bool Allow = false;
            bool Deny = false;
            String ClientIPAddress;

            Log("Server start at " + this.ipAddress.ToString() + " on port " + this.Port);

            while (Cancel == false)
            {
                try
                {
                    tl.Start();

                    TcpClient client = tl.AcceptTcpClient();

                    /* Check if allow ip */
                    ClientIPAddress = ExtractIp(client.Client.RemoteEndPoint.ToString());

                    for (i = 0; i < this.AllowIPAddress.Length; i++)
                    {
                        System.Text.RegularExpressions.Regex ExpressionReguliere = new System.Text.RegularExpressions.Regex(this.AllowIPAddress[i]);

                        if (this.AllowIPAddress[i] != "")
                        {
                            MatchRegEx = ExpressionReguliere.Match(ClientIPAddress);

                            if (MatchRegEx.Success == true)
                            {
                                Allow = true;
                                break;
                            }
                        }
                    }

                    /* Check if deny ip */
                    for (i = 0; i < this.AllowIPAddress.Length; i++)
                    {
                        System.Text.RegularExpressions.Regex ExpressionReguliere = new System.Text.RegularExpressions.Regex(this.DenyIPAddress[i]);

                        if (this.DenyIPAddress[i] != "")
                        {
                            MatchRegEx = ExpressionReguliere.Match(ClientIPAddress);

                            if (MatchRegEx.Success == true)
                            {
                                Deny = true;
                                break;
                            }
                        }
                    }

                    if ((Allow == true) && (Deny == false))
                    {
                        lock (this.LockThread)
                        {
                            this.ClientIP = client.Client;
                        }

                        if ((this.NbClient < this.MaxClient) || (this.MaxClient == 0))
                        {

                            this.NbClient++;
							
                            Log(String.Format(LOG_NEW_CONNECTION, ClientIP.RemoteEndPoint.ToString()));

                            ClientThread = new Thread(new ThreadStart(FTPClientThread));

                            ClientThread.Start();

                            /* Wait for start of thread */
                            while (this.ClientIP != null)
                            {
                                Thread.Sleep(500);
                            }
                        }
                        else
                        {                            
                            SendAnswer(this.ClientIP, "421 Too many users connected.", ref TimeOutUser);
                            client.Close();
                        }
                    }
                    else
                    {
                        SendAnswer(client.Client, "421 Unauthorized.", ref TimeOutUser);
                        client.Close();
                    }
                }
                catch
                {
                    Log(ERROR_CANT_OPEN_PORT);
                    break;
                }
            }
        }

        /*
         * Send answer
         */
        private bool SendAnswer(Socket ClientSocket, String cmd, ref int TimeOutUser)
        {
            bool retour = false;
            Byte[] MyBytes = WindowsEncoding.GetBytes(cmd + EOL ) ;
            bool blockingState = ClientSocket.Blocking;
            int nbBytes;
            System.Net.Sockets.SocketError error = new System.Net.Sockets.SocketError();
            int index = 0;
            int size = MyBytes.Length;

            ClientSocket.Blocking = false;

            do
            {
                nbBytes = ClientSocket.Send(MyBytes, index, size, SocketFlags.None, out error);

                if (nbBytes == 0)
                {
                    if (error != System.Net.Sockets.SocketError.WouldBlock)
                    {
                        /* Connection close */
                        retour = false;
                    }
                    else
                    {
                        /* Why ??? */
                        Thread.Sleep(1000);
                        TimeOutUser--;

                        if (TimeOutUser == 0)
                        {
                            retour = false;
                            break;
                        }
                    }
                }
                else
                {
                    retour = true;
                }

                index += nbBytes;
                size = MyBytes.Length - index;
            }
            while (size > 0);

            if (this.FullLog == true)
            {
                Log("[" + ClientSocket.RemoteEndPoint.ToString() + "] " + cmd);
            }

            ClientSocket.Blocking = blockingState;

            return retour;
        }

        /*
         * Read command form client
         */
        private bool ReadCommand(Socket ClientSocket, ref String Line, ref bool Reading, ref int TimeOutUser)
        {        
            bool retour = true;
            Byte[] buffer = new Byte[1];
            bool blockingState = ClientSocket.Blocking;
            int nbBytes;
            System.Net.Sockets.SocketError error = new System.Net.Sockets.SocketError();

            Line = "";
            Reading = false;
            ClientSocket.Blocking = false;

            do
            {
                nbBytes = ClientSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out error);

                if (nbBytes == 0)
                {
                    if (error != System.Net.Sockets.SocketError.WouldBlock)
                    {
                        /* Connection close */
                        retour = false;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        TimeOutUser--;

                        if (TimeOutUser == 0)
                        {
                            Line = "";
                            Reading = false;
                            retour = false;
                            break;
                        }
                    }
                }
                else
                {
                    Line += WindowsEncoding.GetString(buffer, 0, nbBytes);

                    /* Reinit time out */
                    TimeOutUser = this.TimeOut;

                    Reading = true;
                }

                if (this.Cancel == true)
                {
                    Line = "";
                    Reading = false;
                }
            }
            while ((Line.EndsWith("\r") != true) && (Line.EndsWith("\n") != true));

            ClientSocket.Blocking = blockingState;

            /* Normaly, after \r we have \n. But if telnet connection, we have only \r.
             * So, next read start by \n.
             * Delete \n and \r\n
             */
            Line = Line.Trim();

            if ((Reading == true) && (Line != ""))
            {
                Log("[" + ClientSocket.LocalEndPoint.ToString() + "] " + Line);
            }

            return retour;
        }

        /*
         * Add Directory separator at end if not set
         */
        private String AddDirSeparatorAtEnd(String PathDir, Char DirSeparator)
        {
            String Value = PathDir;

            if (PathDir.EndsWith("" + DirSeparator) == false)
            {
                Value = Value + DirSeparator;
            }

            return Value;
        }

        /*
         * Client thread
         */
        private void FTPClientThread()
        {
            ClassIniReader Ini;
            Socket MySocket = this.ClientIP;
            /* Current line receive */
            String Line;
            /* Comande e.g. USER */
            String Cmd;
            /* Parameter of commande */
            String Parameter;
            /* If login */
            bool Logined = false;
            /* Login name */
            String Login = "";
            /* Time out */
            int TimeOutUser = this.TimeOut;
            /* Config Root */
            String UserRoot = "";
            /* User can download */
            bool Download = false;
            /* User can upload */
            bool Upload = false;
            /* User can rename */
            bool Rename = false;
            /* User can delete */
            bool Delete = false;
            /* User can make directory */
            bool MakeDirectory = false;
            /* User can remove directory */
            bool DeleteDirectory = false;
            /* User can modify time of file */
            bool ModifyTime = false;
            /* Current password */
            String Password = "";
            /* If ReadCommand have reading a command */
            bool Reading = false;
            /* User current ftp directory */
            String UserCurrentDirectory = "/";
            /* if passive mode enable */
            bool PassiveMode = false;
            /* IP of client */
            IPAddress ClientIPAddress = null;
            /* Client port for data chanel */
            int ClientPort = 0;
            /* Socket for Active mode */
            Socket ClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            /* Listener for pasive mode */
            TcpListener ClientDataListener = null;
            /* if file transfert in binary mode */
            bool BinaryMode = false;
            /* an error occur in SendListDirectory */
            int error = 0;
            /* String represente IP:Port of client */
            String ClientIPAddressString = "";
            /* for seeking file */
            long ResumeIndex = 0;
            /* OS current work directory */
            String CurrentWorkDir = "";
            /* stream to read binary file */
            FileStream inFileReadBinary;
            /* stream to read text */
            StreamReader inFileReadText;
            /* tempory variable for make file */
            String LocalFileName ;
            /* file to store on server */
            FileStream outFileWriteBinary;
            /* Temporary client Socket */
            TcpClient tmpClient;
            /* file information to ensure file a good size */
            FileInfo fi;

            /* free ClientIP for can start other client */
            this.ClientIP = null;

            ClientIPAddressString = MySocket.RemoteEndPoint.ToString();

            if (this.WelcomeMessage == "")
            {
                SendAnswer(MySocket, "220 Welcome to " + Dns.GetHostName(), ref TimeOutUser);
            }
            else
            {
                SendAnswer(MySocket, this.WelcomeMessage, ref TimeOutUser);
            }

            while (Cancel == false)
            {
                Line = "";

                if (ReadCommand(MySocket, ref Line, ref Reading, ref TimeOutUser) == true)
                {
                    if (Reading == true)
                    {
                        TimeOutUser = this.TimeOut;
                    }

                    if (Line != "")
                    {
                        Cmd = "";
                        Parameter = "";

                        ExplodeCommand(Line, ref Cmd, ref Parameter);

                        if (Cmd != "")
                        {
                            TimeOutUser = this.TimeOut;

                            if (Cmd == "QUIT")
                            {
                                SendAnswer(MySocket, "221 Goodbye!", ref TimeOutUser);
                                MySocket.Close();
                                break;
                            }
                            else if (Cmd == "USER")
                            {
                                if (Logined == true)
                                {
                                    DisconnectLogin(Login);
                                }

                                ResumeIndex = 0;
                                Logined = false;                               

                                Login = Parameter.ToLower();

                                if (Login == "anonymous")
                                {
                                    SendAnswer(MySocket, "331 Anonymous access allowed, send identity (e-mail name) as password.", ref TimeOutUser);
                                }
                                else
                                {
                                    SendAnswer(MySocket, "331 Password required for " + Login + ".", ref TimeOutUser);
                                }
                            }
                            else
                            {
                                if (Login != "")
                                {
                                    if (Cmd == "PASS")
                                    {
                                        ResumeIndex = 0;
                                        Logined = false;

                                        Ini = new ClassIniReader(this.RootConfigDirectory + "users" + Path.DirectorySeparatorChar + Login + ".ini");

                                        if (Ini.FileExists == true)
                                        {
                                            if (Ini.GetValue("user", "disabled").ToLower() != "yes")
                                            {
                                                if (Login == "anonymous")
                                                {
                                                    Logined = true;

                                                    Log("[" + MySocket.RemoteEndPoint.ToString() + "] Anonymous password : " + Parameter);
                                                }
                                                else
                                                {
                                                    if (Ini.GetValue("user", "passwordProtected").ToLower() == "yes")
                                                    {
                                                        Password = Ini.GetValue("user", "password").ToLower();

                                                        if (EncodePassword(Parameter) == Password)
                                                        {
                                                            Logined = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Ini.GetValue("user", "disabled").ToLower() != "yes")
                                                        {
                                                            Password = Ini.GetValue("user", "password");

                                                            if (Password == Parameter)
                                                            {
                                                                Logined = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (Logined == false)
                                        {
                                            SendAnswer(MySocket, "530 Login incorrect.", ref TimeOutUser);
                                        }
                                        else
                                        {                                         
                                            if (NumberLogin(Login) >= this.MaxSessionPerUser)
                                            {
                                                SendAnswer(MySocket, "530 Sorry, the maximum number of clients (" + this.MaxSessionPerUser + ") from your login are already connected.", ref TimeOutUser);
                                            }
                                            else
                                            {
                                                AddLogin(Login);
                                                SendAnswer(MySocket, "230 User " + Login + " logged in.", ref TimeOutUser);

                                                UserRoot = Ini.GetValue("user", "Root");
                                                CurrentWorkDir = UserRoot;
                                                Download = Ini.GetValue("user", "Download").ToLower() == "yes";
                                                Upload = Ini.GetValue("user", "Upload").ToLower() == "yes";
                                                Rename = Ini.GetValue("user", "Rename").ToLower() == "yes";
                                                Delete = Ini.GetValue("user", "Delete").ToLower() == "yes";
                                                MakeDirectory = Ini.GetValue("user", "MakeDirectory").ToLower() == "yes";
                                                DeleteDirectory = Ini.GetValue("user", "DeleteDirectory").ToLower() == "yes";
                                                ModifyTime = Ini.GetValue("user", "ModifyTime").ToLower() == "yes";
                                            }
                                        }
                                    }
                                    else if (Cmd == "SYST")
                                    {
                                        ResumeIndex = 0;
                                        SendAnswer(MySocket, "215 UNIX Type: L8", ref TimeOutUser);
                                    }
                                    else if (Cmd == "MODE")
                                    {
                                        ResumeIndex = 0;

                                        if (Parameter.ToUpper() == "S")
                                        {
                                            SendAnswer(MySocket, "200 Mode S ok.", ref TimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "500 Mode " + Parameter + " not implemented.", ref TimeOutUser);
                                        }
                                    }
                                    else if ((Cmd == "PWD") || (Cmd == "XPWD"))
                                    {
                                        ResumeIndex = 0;
                                        SendAnswer(MySocket, "257 \"" + UserCurrentDirectory + "\" is current directory.", ref TimeOutUser);
                                    }
                                    else if (Cmd == "PORT")
                                    {
                                        ResumeIndex = 0;

                                        if (ClientPort != -1)
                                        {
                                            FreePassivePort(ClientPort);
                                            ClientPort = -1;
                                        }

                                        PassiveMode = false;

                                        if (GetClientPort(Parameter, ref ClientIPAddress, ref ClientPort) == true)
                                        {
                                            IPEndPoint ep = new IPEndPoint(ClientIPAddress, ClientPort);

                                            try
                                            {
                                                /*
                                                if (ClientDataSocket.Connected)
                                                {
                                                    ClientDataSocket.Close();
                                                }*/

                                                try
                                                {
                                                    /* Try close connection. If exception. Object is disposed. We must create it. */
                                                    ClientDataSocket.Close();
                                                    /* After close, Object is disposed */
                                                    ClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
                                                }
                                                catch
                                                {
                                                    ClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
                                                }


                                                ClientDataSocket.Connect(ep);

                                                if (ClientDataListener != null)
                                                {
                                                    ClientDataListener.Stop();
                                                    ClientDataListener = null;
                                                }

                                                SendAnswer(MySocket, "200 PORT command successful.", ref TimeOutUser);
                                            }
                                            catch
                                            {
                                                SendAnswer(MySocket, "500 Invalid PORT command or already use.", ref TimeOutUser);
                                            }

                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "501 'PORT': Invalid number of parameters", ref TimeOutUser);
                                        }
                                    }
                                    else if (Cmd == "PASV")
                                    {
                                        ResumeIndex = 0;

                                        if (ClientPort != -1)
                                        {
                                            FreePassivePort(ClientPort);
                                        }

                                        if (ClientDataSocket.Connected)
                                        {
                                            ClientDataSocket.Close();
                                        }

                                        ClientPort = GetPassivePort(ref ClientDataListener);

                                        if (ClientPort != -1)
                                        {
                                            PassiveMode = true;
                                            SendAnswer(MySocket, "227 Entering Passive Mode (" + this.IPAddress.Replace('.', ',') + "," + (ClientPort >> 8) + "," + (ClientPort & 0xFF) + ").", ref TimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "500 PASV exception: 'No available PASV Ports'.", ref TimeOutUser);
                                        }
                                    }
                                    else if (Cmd == "TYPE")
                                    {
                                        ResumeIndex = 0;

                                        if (Parameter.ToUpper() == "I")
                                        {
                                            BinaryMode = true;
                                            SendAnswer(MySocket, "200 Type set to I.", ref TimeOutUser);
                                        }
                                        else if (Parameter.ToUpper() == "A")
                                        {
                                            BinaryMode = false;
                                            SendAnswer(MySocket, "200 Type set to A.", ref TimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "504 TYPE must be A or I.", ref TimeOutUser);
                                        }

                                    }
                                    else if ((Cmd == "LIST") || (Cmd == "NLST"))
                                    {
                                        ResumeIndex = 0;

                                        /* if LIST -aL */
                                        if (Parameter.StartsWith("-") == true)
                                        {
                                            ExplodeCommand(Parameter, ref Cmd, ref Parameter);
                                        }

                                        SendAnswer(MySocket, "150 Opening ASCII mode data connection for /bin/ls.", ref TimeOutUser);

                                        if (SendListDirectory(Parameter, ClientDataListener, ClientPort, ClientDataSocket, PassiveMode, ref error, UserRoot, UserCurrentDirectory, ref TimeOutUser, (Cmd == "NLST")) == true)
                                        {
                                            SendAnswer(MySocket, "226 Transfer complete.", ref TimeOutUser);
                                        }
                                        else
                                        {
                                            ShowError(MySocket, ref TimeOutUser, Parameter, error);
                                        }
                                    }
                                    else if (Cmd == "RETR")
                                    {
                                        if (Download == true)
                                        {
                                            try
                                            {
                                                LocalFileName = FTPPathToOSPath(UserRoot, UserCurrentDirectory, Parameter);

                                                if (File.Exists(LocalFileName) == true)
                                                {
                                                    SendAnswer(MySocket, "150 Opening " + (BinaryMode == true ? "binary" : "ASCII") + " mode data connection for " + Parameter + ".", ref TimeOutUser);

                                                    if (BinaryMode == true)
                                                    {
                                                        inFileReadBinary = new FileStream(LocalFileName, FileMode.Open, FileAccess.Read);

                                                        if (SendBinaryFile(inFileReadBinary, ClientDataListener, ClientPort, ClientDataSocket, PassiveMode, ref error, ResumeIndex, ref TimeOutUser) == true)
                                                        {
                                                            SendAnswer(MySocket, MSG_TRANSFERT_COMPLET, ref TimeOutUser);
                                                        }
                                                        else
                                                        {
                                                            ShowError(MySocket, ref TimeOutUser, Parameter, error);
                                                        }

                                                        inFileReadBinary.Close();
                                                    }
                                                    else
                                                    {
                                                        inFileReadText = new StreamReader(LocalFileName);

                                                        if (SendTextFile(inFileReadText, ClientDataListener, ClientPort, ClientDataSocket, PassiveMode, ref error, ref TimeOutUser) == true)
                                                        {
                                                            SendAnswer(MySocket, MSG_TRANSFERT_COMPLET, ref TimeOutUser);
                                                        }
                                                        else
                                                        {
                                                            ShowError(MySocket, ref TimeOutUser, Parameter, error);
                                                        }

                                                        inFileReadText.Close();
                                                    }
                                                }
                                                else
                                                {
                                                    SendAnswer(MySocket, String.Format(MSG_FILE_NOT_FOUND, Parameter), ref TimeOutUser);
                                                }

                                            }
                                            catch
                                            {
                                                if (PassiveMode == true)
                                                {
                                                    tmpClient = ClientDataListener.AcceptTcpClient();
                                                    tmpClient.Client.Close();
                                                }
                                                else
                                                {
                                                    ClientDataSocket.Close();
                                                }

                                                SendAnswer(MySocket, ERROR_PERMISSION_DENIED, ref TimeOutUser);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "500 Cannot RETR.", ref TimeOutUser);
                                        }

                                        ResumeIndex = 0;
                                    }
                                    else if (Cmd == "NOOP")
                                    {
                                        ResumeIndex = 0;

                                        SendAnswer(MySocket, "200 NOOP command successful.", ref TimeOutUser);
                                    }
                                    else if (Cmd == "REST")
                                    {
                                        ResumeIndex = 0;

                                        if (long.TryParse(Parameter, out ResumeIndex) == true)
                                        {
                                            SendAnswer(MySocket, "350 Restart transfert at " + ResumeIndex + ".", ref TimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "501 Syntax error in parameter: '" + Parameter + "' is not a valid integer value.", ref TimeOutUser);
                                        }
                                    }
                                    else if ((Cmd == "CWD") || (Cmd == "CDUP") || (Cmd == "XCUP"))
                                    {
                                        ResumeIndex = 0;

                                        if ((Cmd == "CDUP") || (Cmd == "XCUP"))
                                        {
                                            Parameter = AddDirSeparatorAtEnd(UserCurrentDirectory, '/') + "..";
                                        }

                                        switch (ChangeDirectory(UserRoot, Parameter, ref UserCurrentDirectory, ref CurrentWorkDir))
                                        {
                                            case 0:
                                                SendAnswer(MySocket, "250 CWD command successful.", ref TimeOutUser);
                                                break;
                                            case 1:
                                                SendAnswer(MySocket, "550 Permission Denied.", ref TimeOutUser);
                                                break;
                                            default:
                                                SendAnswer(MySocket, "550 " + Parameter + ": No such file or directory.", ref TimeOutUser);
                                                break;
                                        }
                                    }
                                    else if ((Cmd == "STOR") || (Cmd == "APPE"))
                                    {
                                        if (Upload == true)
                                        {
                                            try
                                            {
                                                LocalFileName = FTPPathToOSPath(UserRoot, UserCurrentDirectory, Parameter);

                                                SendAnswer(MySocket, "150 Opening " + (BinaryMode == true ? "binary" : "ASCII") + " mode data connection for " + Parameter + ".", ref TimeOutUser);

                                                if (File.Exists(LocalFileName) == true)
                                                {
                                                    if (Cmd == "STOR")
                                                    {
                                                        outFileWriteBinary = new FileStream(LocalFileName, FileMode.Open, FileAccess.Write);

                                                        /* trunc file if necessary */
                                                        fi = new FileInfo(LocalFileName);

                                                        if (fi.Length > ResumeIndex)
                                                        {
                                                            outFileWriteBinary.SetLength(ResumeIndex);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        outFileWriteBinary = new FileStream(LocalFileName, FileMode.Append, FileAccess.Write);
                                                    }
                                                }
                                                else
                                                {
                                                    outFileWriteBinary = new FileStream(LocalFileName, FileMode.Create, FileAccess.Write);
                                                }

                                                if (GetFile(outFileWriteBinary, ClientDataListener, ClientPort, ClientDataSocket, PassiveMode, ref error, ResumeIndex, BinaryMode, ref TimeOutUser) == true)
                                                {
                                                    SendAnswer(MySocket, MSG_TRANSFERT_COMPLET, ref TimeOutUser);
                                                }
                                                else
                                                {
                                                    ShowError(MySocket, ref TimeOutUser, Parameter, error);
                                                }

                                                outFileWriteBinary.Close();
                                            }
                                            catch
                                            {
                                                if (PassiveMode == true)
                                                {
                                                    tmpClient = ClientDataListener.AcceptTcpClient();
                                                    tmpClient.Client.Close();
                                                }
                                                else
                                                {
                                                    ClientDataSocket.Close();
                                                }

                                                SendAnswer(MySocket, ERROR_PERMISSION_DENIED, ref TimeOutUser);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(MySocket, "500 Cannot RETR.", ref TimeOutUser);
                                        }

                                        ResumeIndex = 0;
                                    }
                                    else
                                    {
                                        ResumeIndex = 0;

                                        SendAnswer(MySocket, "500 Command not understood.", ref TimeOutUser);
                                    }
                                }
                                else
                                {
                                    SendAnswer(MySocket, "503 Login with USER first.", ref TimeOutUser);
                                }
                            }
                        }
                    }
                    else
                    {
                        /* break for some time other wise with the loop the CPU is 100% */
                        Thread.Sleep(1000);

                        if (this.TimeOut != 0)
                        {
                            TimeOutUser--;
                        }
                    }
                }
                else
                {
                    break;
                }

                if (TimeOutUser == 0)
                {
                    break;
                }
            }

            if (TimeOutUser == 0)
            {
                SendAnswer(MySocket, "503 Time out.", ref TimeOutUser);
                MySocket.Close();
            }


            lock (this.LockThread)
            {
                this.NbClient--;                
            }

            DisconnectLogin(Login);

            Log("[" + ClientIPAddressString + "] Connection close");
        }

        /*
         * Disconnect user
         */
        private void DisconnectLogin(String Login)
        {
            int pos;

            lock (this.LockThread)
            {

                /* Disconnect user if connected */
                if (Login != "")
                {
                    pos = ConnectedUser.IndexOf(Login);

                    if (pos != -1)
                    {
                        NbSessionPerUser[pos]--;

                        if (NbSessionPerUser[pos] <= 0)
                        {
                            ConnectedUser.Remove(ConnectedUser[pos]);
                            NbSessionPerUser.Remove(NbSessionPerUser[pos]);
                        }
                    }
                }
            }
        }

        /*
         * Disconnect user
         */
        private int NumberLogin(String Login)
        {
            int pos;
            int retour = 0;

            lock (this.LockThread)
            {
                if (Login != "")
                {
                    pos = ConnectedUser.IndexOf(Login);

                    if (pos != -1)
                    {
                        retour = NbSessionPerUser[pos];
                    }
                }
            }

            return retour;
        }

        /*
         * Disconnect user
         */
        private void AddLogin(String Login)
        {
            int pos;

            lock (this.LockThread)
            {
                /* Disconnect user if connected */
                if (Login != "")
                {
                    pos = ConnectedUser.IndexOf(Login);

                    if (pos != -1)
                    {
                        NbSessionPerUser[pos]++;
                    }
                    else
                    {
                        ConnectedUser.Add(Login);
                        NbSessionPerUser.Add(1);
                    }
                }
            }
        }

        /*
         * Convert string to MD5
         */
        private static string EncodePassword(string password)
        {
            byte[] original_bytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }
            return result.ToString();
        }

        /*
         * Convert PORT command to IP
         */
        private bool GetClientPort(String AdresseIP, ref IPAddress ClientIPAddress, ref int ClientPort)
        {
            bool retour = false;
            byte[] adr = { 0, 0, 0, 0 };
            char[] separateur = { ',' };
            Port = -1;
            int Value;

            ClientIPAddress = new IPAddress(adr);
            ClientPort = -1;

            String[] tmpAdr = AdresseIP.Split(separateur);

            if (tmpAdr.Length == 6)
            {
                if (int.TryParse(tmpAdr[0], out Value) == true)
                {
                    adr[0] = (byte)Value;

                    if (int.TryParse(tmpAdr[1], out Value) == true)
                    {
                        adr[1] = (byte)Value;

                        if (int.TryParse(tmpAdr[2], out Value) == true)
                        {
                            adr[2] = (byte)Value;

                            if (int.TryParse(tmpAdr[3], out Value) == true)
                            {
                                adr[3] = (byte)Value;

                                if (int.TryParse(tmpAdr[4], out Value) == true)
                                {
                                    ClientPort = Value << 8;

                                    if (Int32.TryParse(tmpAdr[5], out Value) == true)
                                    {
                                        ClientPort += Value;

                                        ClientIPAddress = new IPAddress(adr);

                                        retour = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return retour;
        }

        /*
         * Get passive port
         */
        private int GetPassivePort(ref TcpListener ClientDataListener)
        {
            int retour = -1;

            lock (this.LockThread)
            {
                if (this.PassivePortFree.Count > 0)
                {
                    retour = this.PassivePortFree[0];
                    this.PassivePortFree.Remove(this.PassivePortFree[0]);

                    this.PassivePortUse.Add(retour);

                    if (ClientDataListener != null)
                    {
                        ClientDataListener.Stop();
                    }

                    try
                    {
                        ClientDataListener = new TcpListener(this.ipAddress, retour);
                        ClientDataListener.Start();
                    }
                    catch
                    {
                        ClientDataListener = null;
                    }
                }
            }

            return retour;
        }

        /*
         * Send list of file in directory
         */
        private bool SendListDirectory(String Directory, TcpListener ClientDataListener, int ClientPort, Socket ClientSocket, bool PassiveMode, ref int error, String Root, String UserCurrentDir, ref int TimeOutUser, bool nlst)
        {
            bool retour = true;
            TcpClient client;
            Byte[] MyBytes;
            Socket MySocket;
            int NbBytesRead;
            int index;
            int size;
            int nbBytes;
            System.Net.Sockets.SocketError errorSocket = new System.Net.Sockets.SocketError();
            
            MyBytes = WindowsEncoding.GetBytes(ListDirectory(Directory, UserCurrentDir, Root, ref error, nlst));

            if (error == 0)
            {
                try
                {
                    if (PassiveMode == true)
                    {
                        client = ClientDataListener.AcceptTcpClient();
                        MySocket = client.Client;
                    }
                    else
                    {
                        MySocket = ClientSocket;
                    }

                    NbBytesRead = MyBytes.Length;
                    index = 0;
                    size = NbBytesRead;

                    do
                    {
                        nbBytes = MySocket.Send(MyBytes, index, size, SocketFlags.None, out errorSocket);

                        if (nbBytes == 0)
                        {
                            if (errorSocket != System.Net.Sockets.SocketError.WouldBlock)
                            {
                                /* Connection close */
                                error = 4;
                                break;
                            }
                            else
                            {
                                /* Why ??? */
                                Thread.Sleep(1000);
                                TimeOutUser--;

                                if (TimeOutUser == 0)
                                {
                                    retour = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            retour = true;
                            TimeOutUser = this.TimeOut;
                        }

                        if (this.Cancel == true)
                        {
                            break;
                        }

                        index += nbBytes;
                        size = MyBytes.Length - index;
                    }
                    while (size > 0);

                    retour = true;

                    MySocket.Close();
                }
                catch
                {
                    retour = false;
                }
            }

            return retour;
        }

        /*
         * Extract file name
         */
        private String ExtractFileName(String FileName)
        {
            int i;
            String retour = "";

            for (i = FileName.Length - 1; i >= 0; i--)
            {
                if (FileName[i] == '/')
                {
                    break;
                }
                else
                {
                    retour = FileName[i] + retour;
                }
            }

            return retour;
        }

        /*
         * Create line of directory 
         */
        private String ListDir(String DirName)
        {
            String Line = "d";
            DateTime dtDir = Directory.GetCreationTime(DirName);
            String Rights;

            try
            {
                String[] ListOfFile = System.IO.Directory.GetFiles(DirName, "*.*");
                Rights = "rwx";
            }
            catch
            {
                Rights = "-wx";
            }

            Line += Rights + Rights + Rights + "   1 ftp      ftp 0 " + MonthLabel[dtDir.Month] + " ";

            if (dtDir.Day < 10)
            {
                Line += "0";
            }

            Line += "" + dtDir.Day + " ";

            if (dtDir.Year == DateTime.Now.Year)
            {
                if (dtDir.Hour < 10)
                {
                    Line += "0";
                }

                Line += "" + dtDir.Hour + ":";

                if (dtDir.Minute < 10)
                {
                    Line += "0";
                }

                Line += "" + dtDir.Minute + " ";
            }
            else
            {
                Line += " " + DateTime.Now.Year + " ";
            }

            Line += Path.GetFileName(DirName);

            return Line;
        }

        /*
        * Create line of file 
        */
        private String ListFile(String FileName)
        {
            String Line = "-";
            String Rights = "";
            DateTime dtFile = File.GetCreationTime(FileName);
            FileInfo f = new FileInfo(FileName); ;

            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                fs.Close();

                Rights = "r";
            }
            catch
            {
                Rights = "-";
            }

            if ((File.GetAttributes(FileName) & FileAttributes.ReadOnly) != 0)
            {
                Rights += "-";
            }
            else
            {
                Rights += "w";
            }

            Rights += "-";

            Line += Rights + Rights + Rights + "   1 ftp      ftp " + f.Length + " " + MonthLabel[dtFile.Month - 1] + " ";

            if (dtFile.Day < 10)
            {
                Line += "0";
            }

            Line += "" + dtFile.Day + " ";

            if (dtFile.Year == DateTime.Now.Year)
            {
                if (dtFile.Hour < 10)
                {
                    Line += "0";
                }

                Line += "" + dtFile.Hour + ":";

                if (dtFile.Minute < 10)
                {
                    Line += "0";
                }

                Line += "" + dtFile.Minute + " ";
            }
            else
            {
                Line += " " + DateTime.Now.Year + " ";
            }

            Line += Path.GetFileName(FileName);

            return Line;
        }

        /*
         * List file and sub-directory of directory
         * 
         * error = 0 : OK
         * error = 1 : can't open data connection
         * error = 2 : access denied
         * error = 3 : not find
         */
        private String ListDirectory(String Dir, String UserCurrentDir, String Root, ref int error, bool nlst)
        {
            int i;
            String retour = "";
            error = 1;
            String LocalDir = FTPPathToOSPath(Root, UserCurrentDir, Dir);

            try
            {
                if (Directory.Exists(LocalDir) == true)
                {
                    String[] ListOfFile = System.IO.Directory.GetFiles(LocalDir, "*.*");
                    String[] ListOfDirecotry = System.IO.Directory.GetDirectories(LocalDir, "*.*");

                    for (i = 0; i < ListOfDirecotry.Length; i++)
                    {
                        if (nlst == true)
                        {
                            retour += ListOfDirecotry[i];
                        }
                        else
                        {
                            retour += ListDir(ListOfDirecotry[i]);
                        }

                        retour += EOL;
                    }

                    for (i = 0; i < ListOfFile.Length; i++)
                    {
                        if (nlst == true)
                        {
                            retour += ListOfFile[i];
                        }
                        else
                        {
                            retour += ListFile(ListOfFile[i]);
                        }

                        retour += EOL;
                    }

                    error = 0;
                }
                else if (File.Exists(LocalDir) == true)
                {
                    retour = ListFile(LocalDir) + EOL;
                }
                else
                {
                    error = 3;
                }
            }
            catch
            {
                error = 2;
            }

            return retour;
        }

        /*
         * Extract command and parameter
         */
        private void ExplodeCommand(String Line, ref String Cmd, ref String Parameter)
        {
            int i;

            Cmd = "";
            Parameter = "";

            for (i = 0; i < Line.Length; i++)
            {
                if (Line[i] == ' ')
                {
                    i++;
                    break;
                }
                else
                {
                    Cmd += Line[i];
                }
            }

            Cmd = Cmd.ToUpper();
            Parameter = Line.Substring(i);
        }

        /*
         * Free a free port
         */
        private void FreePassivePort(int Port)
        {
            int pos;
            int FreePort;

            pos = this.PassivePortUse.IndexOf(Port);

            if (pos != -1)
            {
                FreePort = this.PassivePortUse[pos];
                this.PassivePortUse.Remove(this.PassivePortUse[pos]);
                this.PassivePortFree.Add(FreePort);
            }
        }

        /*
         * Change directory
         *  0 = OK
         *  1 = Acces denied
         *  2 = Not found
         */
        private int ChangeDirectory(String Root, String Dir, ref String UserCurrentDirectory, ref String CurrentWorkDirectory)
        {
            int retour = -1;
            String newdir = "";
            String newpath = "";

            if (Dir.Length > 0)
            {
                if (Dir[0] != '/')
                {
                    newdir = AddDirSeparatorAtEnd(UserCurrentDirectory, '/') + Dir;
                }
                else
                {
                    newdir = Dir;
                }
            }
            else
            {
                newdir = UserCurrentDirectory;
            }

            /* Vérification du chemin */
            newpath = newdir;

            if (newpath.Length > 0)
            {
                if (newpath[0] == '/')
                {
                    newpath = newpath.Substring(1);
                }
            }

            newpath = AddDirSeparatorAtEnd(Root, Path.DirectorySeparatorChar) + newpath.Replace('/', Path.DirectorySeparatorChar);

            newpath = AddDirSeparatorAtEnd(Path.GetFullPath(newpath), Path.DirectorySeparatorChar) ;

            if (Directory.Exists(newpath) == true)
            {
                if (CurrentWorkDirectory.StartsWith(Root) == true)
                {
                    try
                    {
                        String[] ListOfFile = System.IO.Directory.GetFiles(newpath, "*.*");

                        retour = 0;

                        if (newpath.StartsWith(Root) == true)
                        {
                            UserCurrentDirectory = newpath.Substring(Root.Length).Replace(Path.DirectorySeparatorChar, '/');
                            CurrentWorkDirectory = newpath;
                        }
                        else
                        {
                            retour = 1;
                        }
                    }
                    catch
                    {
                        retour = 1;
                    }
                }
                else
                {
                    retour = 1;
                }
            }
            else
            {
                retour = 2;
            }

            return retour;
        }

        /*
         * Translate FTP path to OS path
         */
        private String FTPPathToOSPath(String Root, String UserCurrentDir, String Name)
        {
            String retour = "";

            retour = AddDirSeparatorAtEnd(Root, Path.DirectorySeparatorChar);

            /* We delete first / */
            if (UserCurrentDir.Length > 0)
            {
                if (UserCurrentDir[0] == '/')
                {
                    UserCurrentDir = UserCurrentDir.Substring(1);
                }
            }

            /* We delete first / */
            if (Name.Length > 0)
            {
                if (Name[0] == '/')
                {
                    Name = Name.Substring(1);
                }
                else
                {
                    Name = AddDirSeparatorAtEnd(UserCurrentDir, '/') + Name;
                }
            }
            else
            {
                Name = UserCurrentDir;
            }

            Name = Name.Replace('/', Path.DirectorySeparatorChar);

            return retour + Name;

        }

        /*
         * Show error
         */
        private void ShowError(Socket ClientSocket, ref int TimeOutUser, String FileName, int error)
        {
            if (error == 1)
            {
                SendAnswer(ClientSocket, "425 Can't open data connection.", ref TimeOutUser);
            }
            else if (error == 2)
            {
                SendAnswer(ClientSocket, "500 Access denied.", ref TimeOutUser);
            }
            else if (error == 3)
            {
                SendAnswer(ClientSocket, String.Format(MSG_FILE_NOT_FOUND, FileName), ref TimeOutUser);
            }
            else if (error == 4)
            {
                SendAnswer(ClientSocket, "421 Connection close by client.", ref TimeOutUser);
            }
            else if (error == 5)
            {
                SendAnswer(ClientSocket, "421 Connection close cause server shutdown.", ref TimeOutUser);
            }
            else if (error == 6)
            {
                SendAnswer(ClientSocket, "426 Connection close by server. Not enough space.", ref TimeOutUser);
            }
        }

        /*
         * Send a binary file
         * error : see ShowError for more detail
         */
        private bool SendBinaryFile(FileStream inFileReadBinary, TcpListener ClientDataListener, int ClientPort, Socket ClientSocket, bool PassiveMode, ref int error, long StartIndex, ref int TimeOutUser)
        {
            bool retour = false;
            TcpClient client;
            Byte[] MyBytes = new Byte[BLOCK_SIZE];
            Socket MySocket;
            int NbBytesRead ;
            bool blockingState = ClientSocket.Blocking;
            System.Net.Sockets.SocketError errorSocket = new System.Net.Sockets.SocketError();
            int index = 0;
            int size = MyBytes.Length;
            int nbBytes = 0;
            bool InternalError = false;

            error = 0;

            try
            {
                if (PassiveMode == true)
                {
                    client = ClientDataListener.AcceptTcpClient();
                    MySocket = client.Client;
                }
                else
                {
                    MySocket = ClientSocket;
                }

                MySocket.Blocking = false ;

                if (StartIndex > 0)
                {
                    inFileReadBinary.Seek(StartIndex, SeekOrigin.Begin);
                }

                do
                {
                    NbBytesRead = inFileReadBinary.Read(MyBytes, 0, MyBytes.Length);

                    if (NbBytesRead > 0)
                    {
                        index = 0 ;
                        size = NbBytesRead ;

                        do
                        {
                            nbBytes = MySocket.Send(MyBytes, index, size, SocketFlags.None, out errorSocket);

                            if (nbBytes == 0)
                            {
                                if (errorSocket != System.Net.Sockets.SocketError.WouldBlock)
                                {
                                    /* Connection close */
                                    error = 4;
                                    InternalError = true;
                                    break;
                                }
                                else
                                {
                                    /* Why ??? */
                                    Thread.Sleep(1000);
                                    TimeOutUser--;

                                    if (TimeOutUser == 0)
                                    {
                                        retour = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                retour = true;
                                TimeOutUser = this.TimeOut;
                            }

                            if (this.Cancel == true)
                            {
                                break;
                            }

                            index += nbBytes;
                            size = MyBytes.Length - index;
                        }
                        while (size > 0);
                    }
                }
                while ((NbBytesRead == MyBytes.Length) && (InternalError == false)); 

                if (this.Cancel == true)
                {
                    error = 5;
                }
                else
                {
                    retour = true;
                }

                MySocket.Close();
            }
            catch
            {
                error = 1;
            }


            return retour;
        }

        /*
         * Send a text file
         * error : see ShowError for more detail
         */
        private bool SendTextFile(StreamReader inFileReadText, TcpListener ClientDataListener, int ClientPort, Socket ClientSocket, bool PassiveMode, ref int error, ref int TimeOutUser)
        {
            bool retour = false;
            TcpClient client;
            Byte[] MyBytes  ;
            Socket MySocket;
            bool blockingState = ClientSocket.Blocking;
            System.Net.Sockets.SocketError errorSocket = new System.Net.Sockets.SocketError();
            int index = 0;
            int size = 0;
            int nbBytes = 0;
            String Line;
            bool InternalError = false;

            error = 0;

            try
            {
                if (PassiveMode == true)
                {
                    client = ClientDataListener.AcceptTcpClient();
                    MySocket = client.Client;
                }
                else
                {
                    MySocket = ClientSocket;
                }

                MySocket.Blocking = false ;

                do
                {
                    Line = inFileReadText.ReadLine() + EOL;

                    MyBytes = WindowsEncoding.GetBytes(Line);

                    index = 0;
                    size = MyBytes.Length;

                    do
                    {
                        nbBytes = MySocket.Send(MyBytes, index, size, SocketFlags.None, out errorSocket);

                        if (nbBytes == 0)
                        {
                            if (errorSocket != System.Net.Sockets.SocketError.WouldBlock)
                            {
                                /* Connection close */
                                error = 4;
                                break;
                            }
                            else
                            {
                                /* Why ??? */
                                Thread.Sleep(1000);
                                TimeOutUser--;

                                if (TimeOutUser == 0)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            retour = true;
                            TimeOutUser = this.TimeOut;
                        }

                        if (this.Cancel == true)
                        {
                            break;
                        }

                        index += nbBytes;
                        size = MyBytes.Length - index;
                    }
                    while (size > 0);
                }
                while ((inFileReadText.EndOfStream == false) && (InternalError == false));

                if (this.Cancel == true)
                {
                    error = 5;
                }
                else
                {
                    retour = true;
                }

                MySocket.Close();
            }
            catch
            {
                error = 1;
            }


            return retour;
        }

        /*
         * Extract IP Address
         */
        private String ExtractIp(String IPAddr)
        {
            String retour = "";
            int i;

            for (i = 0; i < IPAddr.Length; i++)
            {
                if (IPAddr[i] == ':')
                {
                    break;
                }
                else
                {
                    retour += IPAddr[i];
                }
            }

            return retour;
        }


        /*
         * Get a  file
         * error : see ShowError for more detail
         */
        private bool GetFile(FileStream outFileWriteBinary, TcpListener ClientDataListener, int ClientPort, Socket ClientSocket, bool PassiveMode, ref int error, long StartIndex, bool BinaryMode, ref int TimeOutUser)
        {
            bool retour = false;
            TcpClient client;
            Byte[] MyBytes = new Byte[BLOCK_SIZE];
            Socket MySocket;
            bool blockingState = ClientSocket.Blocking;
            System.Net.Sockets.SocketError errorSocket = new System.Net.Sockets.SocketError();
            int size = MyBytes.Length;
            int nbBytes = 0;
            String Line;


            error = 0;

            try
            {
                if (PassiveMode == true)
                {
                    client = ClientDataListener.AcceptTcpClient();
                    MySocket = client.Client;
                }
                else
                {
                    MySocket = ClientSocket;
                }

                MySocket.Blocking = false;

                do
                {
                    nbBytes = MySocket.Receive(MyBytes, 0, MyBytes.Length, SocketFlags.None, out errorSocket);

                    if (nbBytes == 0)
                    {
                        if (errorSocket != System.Net.Sockets.SocketError.WouldBlock)
                        {
                            /* Connection close */
                            retour = true;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            TimeOutUser--;

                            if (TimeOutUser == 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (BinaryMode == false)
                        {
                            Line = WindowsEncoding.GetString(MyBytes);
                            Line = Line.Replace("\r\n", Environment.NewLine);

                            if (Line.EndsWith("\r") == true)
                            {
                                Line = Line.Substring(0, Line.Length - 1) + Environment.NewLine;
                            }

                            if (Line.StartsWith("\n") == true)
                            {
                                Line = Line.Substring(1);
                            }

                            MyBytes = WindowsEncoding.GetBytes(Line);
                        }

                        try
                        {
                            outFileWriteBinary.Write(MyBytes, 0, nbBytes);
                        }
                        catch
                        {
                            error = 6;
                            break;
                        }

                        /* Reinit time out */
                        TimeOutUser = this.TimeOut;
                    }
                }
                while (TimeOutUser > 0);

                return retour;                
            }
            catch
            {
                error = 1;
            }

            return retour;
        }
    }
}
