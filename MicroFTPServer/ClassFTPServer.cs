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
 *  
 * Methode :
 *  - Start() : start server,
 *  - Cancel() : asynchrous cancel.
 * ****************************************************************************
 * Variables names :
 *  xyZZZZZ :
 *            x : l : local variable
 *                g : global variable/public variable
 *                p : private/protected variable
 *                a : argument variable
 *                
 *            y : s : string
 *                i : integer
 *                f : fload
 *                d : double
 *                a : array
 *                l : list<>
 *                o : object
 *                b : bool
 *                c : char
 *                l : long
 *                
 *           ZZZZ : name of variable
 *  
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
[OK]     MDTM
          -> MDTM 2.zip  -> File.GetLastWriteTime()
             213 20080526101721
             Syntax: MDTM remote-filename
             Returns the last-modified time of the given file on the remote host in the format "YYYYMMDDhhmmss": YYYY is the four-digit year, MM is the month from 01 to 12, DD is the day of the month from 01 to 31, hh is the hour from 00 to 23, mm is the minute from 00 to 59, and ss is the second from 00 to 59. 
[OK]     SIZE
[OK]     UTF8 ON|OFF -> passe en mode utf8 les lignes de commande : 200 UTF8 mode enabled : 200 UTF8 mode disabled : 501 Invalid UFT8 options
[OK]     CLNT <ckient name> - Send FTP Client Name to server. : 200 Don't care
[OK]     MFMT bool ModifyTime
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
        public delegate void LogFunc(String Text);
        public LogFunc OnLog = null;

        /* root directory to find config file */
        private String psRootConfigDirectory = "";
        /* defaut listen port */
        private int piPort = 21;
        /* defaut listen ip adress */
        private String psIPAddress = "127.0.0.1";
        /* defaut welcom message */
        private String psWelcomeMessage = "";
        /* maximum sesion per user */ 
        private int piMaxSessionPerUser = 2;
        /* maximum client simultaneous */
        private int piMaxClient = 512;
        /* enable full log */
        private bool pbFullLog = false;
        /* default time out in second */
        private int piTimeOut = 45;
        /* start passive port */
        private int piPassivePortStart;
        /* end passive port */
        private int piPassivePortStop;
        /* cancel server */
        private bool pbCancel = false;
        /* object represent ip adress of server */
        private IPAddress poIpAddress;
        /* current number client */
        private int piNbClient = 0;
        /* current client ip */
        private Socket poClientIP;
        /* month letter to list command */
        private String[] paMonthLabel = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        /* passive port free list */
        private List<int> plPassivePortFree = new List<int>();
        /* passive port used list */
        private List<int> plPassivePortUse = new List<int>();
        /* list of user connected */
        private List<String> plConnectedUser = new List<String>();
        /* list of number session per user (link with plConnectedUser) */
        private List<int> plNbSessionPerUser = new List<int>();
        /* object to disabled interruption of thread */
        private Object poLockThread = new Object();
        /* array of allow ip adress */
        private String[] paAllowIPAddress ;
        /* array of deny ip adress */ 
        private String[] paDenyIPAddress;
        /* Encoding for translate è à é*/ 
        private Encoding poWindowsEncoding = Encoding.GetEncoding(1252);

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
         * Error
         */
        private const int SUCCESS = 0;
        private const int CANT_OPEN_DATA_CONNECTION = 1 ;
        private const int ACCES_DENIED = 2;
        private const int FILE_NOT_FOUND = 3 ;
        private const int CONNECTION_CLOSE_BY_CLIENT = 4 ;
        private const int CONNECTION_CLOSE_SERVER_SHUTDOWN = 5 ;
        private const int NO_ENOUGH_SPACE = 6 ;
        private const int NOT_VALID_DATE = 7 ;
        private const int SYNTAX_ERROR = 8 ;
        private const int FILE_ALREADY_EXIST = 9;
        private const int DIRECTORY_ALREADY_EXIST = 10;

        /*
         * Constante
         */
        private const String EOL = "\r\n";
        private const int BLOCK_SIZE = 512;
        private const int WAITING_TIME = 1000;

        /*
         * <summary>Constructor</summary>
         * <remarks></remarks>
         * <param name="asRootFile">directory contain config file</param>
         * <returns>no return</returns>
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        public ClassFTPServer(String asRootDirectory)
        {
            psRootConfigDirectory = AddDirSeparatorAtEnd(asRootDirectory, Path.DirectorySeparatorChar);
        }

        /*
         * <summary>Log function</summary>
         * <remarks></remarks>
         * <returns>no return</returns>
         * <history>
         * <history>
         */
        private void Log(String Text)
        {
            if (OnLog != null)
            {
                OnLog(Text);
            }
        }


        /*
         * <summary>Cancel function</summary>
         * <remarks></remarks>
         * <returns>no return</returns>
         * <history>
         * - 06/07/2008 : create function
         * <history>
         */
        public void Cancel()
        {
            pbCancel = true;
        }

        /*
         * <summary>Start connexion</summary>
         * 
         * <remarks></remarks>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 08/07/2008 : user string.Equals(),
         * <history>
         */
        public void Start()
        {
            ClassIniReader loGeneralIni = new ClassIniReader(psRootConfigDirectory + "general.ini");
            String lsTmp;
            int liIndexPassivePort;
            char[] laPassivePortSeparateur = { '-' };
            String[] laTmpPort;
            char[] laIPSeparator = { ',' };

            if (loGeneralIni.FileExists == true)
            {
                lsTmp = loGeneralIni.GetValue("main", "port");

                if (int.TryParse(lsTmp, out piPort) == true)
                {
                    if ((piPort >= 0) && (piPort < 65536))
                    {
                        try
                        {
                            psIPAddress = loGeneralIni.GetValue("main", "IPAddress");
                            poIpAddress = Dns.GetHostEntry(psIPAddress).AddressList[0];

                            psWelcomeMessage = loGeneralIni.GetValue("main", "WelcomeMessage");

                            psWelcomeMessage = psWelcomeMessage.Replace("\\n", EOL + "220-") ;

                            if (psWelcomeMessage.StartsWith("220-") == false)
                            {
                                psWelcomeMessage = "220-" + psWelcomeMessage;
                            }

                            if (psWelcomeMessage.EndsWith(EOL) == false)
                            {
                                psWelcomeMessage += EOL ;
                            }

                            psWelcomeMessage += "220 ";

                            lsTmp = loGeneralIni.GetValue("main", "MaxSessionPerUser");

                            if (int.TryParse(lsTmp, out piMaxSessionPerUser) == true)
                            {
                                lsTmp = loGeneralIni.GetValue("main", "MaxClient");

                                if (int.TryParse(lsTmp, out piMaxClient) == true)
                                {
                                    lsTmp = loGeneralIni.GetValue("main", "FullLog");

                                    if (lsTmp.Equals("yes", StringComparison.OrdinalIgnoreCase) == true)
                                    {
                                        pbFullLog = true;
                                    }
                                    else
                                    {
                                        pbFullLog = false;
                                    }

                                    lsTmp = loGeneralIni.GetValue("main", "TimeOut");

                                    if (int.TryParse(lsTmp, out piTimeOut) == true)
                                    {
                                        lsTmp = loGeneralIni.GetValue("main", "PassivePort");

                                        laTmpPort = lsTmp.Split(laPassivePortSeparateur);

                                        if (laTmpPort.Length == 2)
                                        {
                                            if (int.TryParse(laTmpPort[0], out piPassivePortStart) == true)
                                            {
                                                if (int.TryParse(laTmpPort[1], out piPassivePortStop) == true)
                                                {
                                                    for (liIndexPassivePort = piPassivePortStart; liIndexPassivePort < piPassivePortStop; liIndexPassivePort++)
                                                    {
                                                        plPassivePortFree.Add(liIndexPassivePort);
                                                    }

                                                    lsTmp = loGeneralIni.GetValue("main", "AllowIPAddress");

                                                    /* Convert to RegEx */
                                                    lsTmp = lsTmp.Replace("?", "[0-9]");
                                                    lsTmp = lsTmp.Replace("*", "[0-9]*");

                                                    paAllowIPAddress = lsTmp.Split(laIPSeparator);

                                                    lsTmp = loGeneralIni.GetValue("main", "DenyIPAddress");

                                                    /* Convert to RegEx */
                                                    lsTmp = lsTmp.Replace("?", "[0-9]");
                                                    lsTmp = lsTmp.Replace("*", "[0-9]*");

                                                    paDenyIPAddress = lsTmp.Split(laIPSeparator);

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
         * <summary>Run server</summary>
         * 
         * <remarks></remarks>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 10/07/2008 : use String.IsNullOrEmpty(),
         *                user String.Concat()
         * <history>
         */
        private void Run()
        {
            IPEndPoint loEp = new IPEndPoint(poIpAddress, piPort);
            TcpListener loTl = new TcpListener(loEp);
            Thread loClientThread;
            int liTimeOutUser = piTimeOut;
            System.Text.RegularExpressions.Match loMatchRegEx;
            int liIndex;
            bool lbAllow = false;
            bool lbDeny = false;
            String lsClientIPAddress;

            Log(String.Concat("Server start at ", poIpAddress.ToString(), " on port ", piPort.ToString()));

            while (pbCancel == false)
            {
                try
                {
                    loTl.Start();

                    TcpClient client = loTl.AcceptTcpClient();

                    /* Check if allow ip */
                    lsClientIPAddress = ExtractIp(client.Client.RemoteEndPoint.ToString());

                    for (liIndex = 0; liIndex < paAllowIPAddress.Length; liIndex++)
                    {
                        System.Text.RegularExpressions.Regex ExpressionReguliere = new System.Text.RegularExpressions.Regex(paAllowIPAddress[liIndex]);

                        if (String.IsNullOrEmpty(paAllowIPAddress[liIndex]) == false)
                        {
                            loMatchRegEx = ExpressionReguliere.Match(lsClientIPAddress);

                            if (loMatchRegEx.Success == true)
                            {
                                lbAllow = true;
                                break;
                            }
                        }
                    }

                    /* Check if deny ip */
                    for (liIndex = 0; liIndex < paAllowIPAddress.Length; liIndex++)
                    {
                        System.Text.RegularExpressions.Regex ExpressionReguliere = new System.Text.RegularExpressions.Regex(paDenyIPAddress[liIndex]);

                        if (String.IsNullOrEmpty(paDenyIPAddress[liIndex]) == false)
                        {
                            loMatchRegEx = ExpressionReguliere.Match(lsClientIPAddress);

                            if (loMatchRegEx.Success == true)
                            {
                                lbDeny = true;
                                break;
                            }
                        }
                    }

                    if ((lbAllow == true) && (lbDeny == false))
                    {
                        lock (poLockThread)
                        {
                            poClientIP = client.Client;
                        }

                        if ((piNbClient < piMaxClient) || (piMaxClient == 0))
                        {

                            piNbClient++;
							
                            Log(String.Format(LOG_NEW_CONNECTION, poClientIP.RemoteEndPoint.ToString()));

                            loClientThread = new Thread(new ThreadStart(FTPClientThread));

                            loClientThread.Start();

                            /* Wait for start of thread */
                            while (poClientIP != null)
                            {
                                Thread.Sleep(500);
                            }
                        }
                        else
                        {                            
                            SendAnswer(poClientIP, "421 Too many users connected.", ref liTimeOutUser);
                            client.Close();
                        }
                    }
                    else
                    {
                        SendAnswer(client.Client, "421 Unauthorized.", ref liTimeOutUser);
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
         * <summary>Send answer</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoClientSocket">client socket</param>
         * <param name="aoCmd">answer to send</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>true if function have read answer</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         *                add WAITING_TIME constante,
         * - 10/08/2008 : use String.Empty()
         * <history>
         */
        private bool SendAnswer(Socket aoClientSocket, String asCmd, ref int aiTimeOutUser)
        {
            bool lbRetour = false;
            Byte[] laMyBytes = poWindowsEncoding.GetBytes(asCmd + EOL ) ;
            bool lbBlockingState = aoClientSocket.Blocking;
            int liNbBytes;
            System.Net.Sockets.SocketError loError = new System.Net.Sockets.SocketError();
            int liIndex = 0;
            int liSize = laMyBytes.Length;

            aoClientSocket.Blocking = false;

            do
            {
                liNbBytes = aoClientSocket.Send(laMyBytes, liIndex, liSize, SocketFlags.None, out loError);

                if (liNbBytes == 0)
                {
                    if (loError != System.Net.Sockets.SocketError.WouldBlock)
                    {
                        /* Connection close */
                        lbRetour = false;
                    }
                    else
                    {
                        /* Why ??? */
                        Thread.Sleep(WAITING_TIME);
                        aiTimeOutUser--;

                        if (aiTimeOutUser == 0)
                        {
                            lbRetour = false;
                            break;
                        }
                    }
                }
                else
                {
                    lbRetour = true;
                }

                liIndex += liNbBytes;
                liSize = laMyBytes.Length - liIndex;
            }
            while (liSize > 0);

            if (pbFullLog == true)
            {
                Log(String.Concat("[", aoClientSocket.RemoteEndPoint.ToString(), "] ", asCmd));
            }

            aoClientSocket.Blocking = lbBlockingState;

            return lbRetour;
        }

        /*
         * <summary>Read command form client</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoClientSocket">client socket</param>
         * <param name="asLine">command to read</param>
         * <param name="abUtf8Mode">read in UTF8</param>
         * <param name="abReading">true if command read</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>true if not time out</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         *                add WAITING_TIME constante,
         * - 10/07/2008 : use String.Empty,
         *                use String.Concat(),
         * <history>
         */
        private bool ReadCommand(Socket aoClientSocket, ref String asLine, bool abUtf8Mode, ref bool abReading, ref int aiTimeOutUser)
        {        
            bool lbRetour = true;
            Byte[] laBuffer = new Byte[1];
            bool lbBlockingState = aoClientSocket.Blocking;
            int liNbBytes;
            System.Net.Sockets.SocketError loError = new System.Net.Sockets.SocketError();
            byte[] laByteLine = {};

            asLine = "";
            abReading = false;
            aoClientSocket.Blocking = false;

            do
            {
                liNbBytes = aoClientSocket.Receive(laBuffer, 0, laBuffer.Length, SocketFlags.None, out loError);

                if (liNbBytes == 0)
                {
                    if (loError != System.Net.Sockets.SocketError.WouldBlock)
                    {
                        /* Connection close */
                        lbRetour = false;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(WAITING_TIME);
                        aiTimeOutUser--;

                        if (aiTimeOutUser == 0)
                        {
                            asLine = String.Empty;
                            abReading = false;
                            lbRetour = false;
                            break;
                        }
                    }
                }
                else
                {
                    List<byte> li = new List<byte>(laByteLine.Length + laBuffer.Length);
                    li.AddRange(laByteLine);
                    li.AddRange(laBuffer);
                    laByteLine = li.ToArray();

                    /* Reinit time out */
                    aiTimeOutUser = piTimeOut;

                    abReading = true;
                }

                if (pbCancel == true)
                {
                    asLine = String.Empty;
                    abReading = false;
                }

                if (laByteLine.Length > 0)
                {
                    if ((laByteLine[laByteLine.Length - 1] == '\r') || (laByteLine[laByteLine.Length - 1] == '\n'))
                    {
                        break ;
                    }
                }
            }
            while (aiTimeOutUser > 0) ;

            if (abUtf8Mode == true)
            {
                asLine += Encoding.UTF8.GetString(laByteLine, 0, laByteLine.Length);
            }
            else
            {
                asLine += poWindowsEncoding.GetString(laByteLine, 0, laByteLine.Length);
            }

            aoClientSocket.Blocking = lbBlockingState;

            /* Normaly, after \r we have \n. But if telnet connection, we have only \r.
             * So, next read start by \n.
             * Delete \n and \r\n
             */
            asLine = asLine.Trim();

            if ((abReading == true) && (asLine != String.Empty))
            {
                Log(String.Concat("[", aoClientSocket.LocalEndPoint.ToString(), "] ", asLine));
            }

            return lbRetour;
        }

        /*
         * <summary>Add Directory separator at end if not set</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asPathDir">string to add directory separator</param>
         * <param name="acDirSeparator">directory separator</param>
         * <returns>string with directory separator at and</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 10/07/2008 : use acDirSeparator.ToString()
         * <history>
         */
        private String AddDirSeparatorAtEnd(String asPathDir, Char acDirSeparator)
        {
            String lsValue = asPathDir;

            if (asPathDir.EndsWith(acDirSeparator.ToString()) == false)
            {
                lsValue = lsValue + acDirSeparator;
            }

            return lsValue;
        }

        /*
         * <summary>Client thread</summary>
         * 
         * <remarks></remarks>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         *                add WAITING_TIME constante,
         * - 08/07/2008 : use string.Equals(),
         * - 10/07/2008 : use String.Empty,
         *                use String.Concat(),
         * <history> 
         */
        private void FTPClientThread()
        {
            ClassIniReader loIni;
            Socket loMySocket = poClientIP;
            /* Current line receive */
            String lsLine;
            /* Comande e.g. USER */
            String lsCmd;
            /* lsParameter of commande */
            String lsParameter;
            /* If login */
            bool lbLogined = false;
            /* Login name */
            String lsLogin = String.Empty;
            /* Time out */
            int liTimeOutUser = piTimeOut;
            /* Config Root */
            String lsUserRoot = String.Empty;
            /* User can download */
            bool lbDownload = false;
            /* User can upload */
            bool lbUpload = false;
            /* User can rename */
            bool lbRename = false;
            /* User can delete */
            bool lbDelete = false;
            /* User can make directory */
            bool lbMakeDirectory = false;
            /* User can remove directory */
            bool lbDeleteDirectory = false;
            /* User can modify time of file */
            bool lbModifyTime = false;
            /* Current password */
            String lsPassword = String.Empty;
            /* If ReadCommand have reading a command */
            bool lbReading = false;
            /* User current ftp directory */
            String lsUserCurrentDirectory = "/";
            /* if passive mode enable */
            bool lsPassiveMode = false;
            /* IP of client */
            IPAddress loClientIPAddress = null;
            /* Client port for data chanel */
            int liClientPort = 0;
            /* Socket for Active mode */
            Socket loClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            /* Listener for pasive mode */
            TcpListener loClientDataListener = null;
            /* if file transfert in binary mode */
            bool lbBinaryMode = false;
            /* an error occur in SendListDirectory */
            int liError = 0;
            /* String represente IP:Port of client */
            String lsClientIPAddressString = String.Empty;
            /* for seeking file */
            long llResumeIndex = 0;
            /* OS current work directory */
            String lsCurrentWorkDir = String.Empty;
            /* stream to read binary file */
            FileStream loInFileReadBinary;
            /* stream to read text */
            StreamReader loInFileReadText;
            /* tempory variable for make file */
            String lsLocalFileName;
            /* file to store on server */
            FileStream loOutFileWriteBinary;
            /* Temporary client Socket */
            TcpClient loTmpClient;
            /* file information to ensure file a good size */
            FileInfo loFi;
            /* Result of some function */
            String lsResult = String.Empty;
            /* Enabled/Disabled Utf8 mode */
            bool lbUtf8Mode = false;
            /* File name (use in MFMT) */
            String lsFileName = String.Empty;
            /* Modify time of file (use in MFMT) */
            String lsModifyTimeOfFile = String.Empty;
            /* File to rename */
            String lsRenameFrom = String.Empty;
            /* true if rename file is directory */
            bool lbIsDirectory = false ;

            /* free ClientIP for can start other client */
            poClientIP = null;

            lsClientIPAddressString = loMySocket.RemoteEndPoint.ToString();

            if (psWelcomeMessage.Equals(String.Empty) == true)
            {
                SendAnswer(loMySocket, String.Concat("220 Welcome to ", Dns.GetHostName()), ref liTimeOutUser);
            }
            else
            {
                SendAnswer(loMySocket, psWelcomeMessage, ref liTimeOutUser);
            }

            while (pbCancel == false)
            {
                lsLine = String.Empty;

                if (ReadCommand(loMySocket, ref lsLine, lbUtf8Mode, ref lbReading, ref liTimeOutUser) == true)
                {
                    if (lbReading == true)
                    {
                        liTimeOutUser = piTimeOut;
                    }

                    if (lsLine.Equals(String.Empty) == false)
                    {
                        lsCmd = String.Empty;
                        lsParameter = String.Empty;

                        ExplodeCommand(lsLine, ref lsCmd, ref lsParameter);

                        if (lsCmd.Equals(String.Empty) == false)
                        {
                            liTimeOutUser = piTimeOut;

                            if (lsCmd.Equals("QUIT") == true)
                            {
                                SendAnswer(loMySocket, "221 Goodbye!", ref liTimeOutUser);
                                loMySocket.Close();
                                break;
                            }
                            else if (lsCmd.Equals("USER") == true)
                            {
                                if (lbLogined == true)
                                {
                                    DisconnectLogin(lsLogin);
                                }

                                llResumeIndex = 0;
                                lbLogined = false;

                                lsLogin = lsParameter.ToLower();

                                if (lsLogin.Equals("anonymous") == true)
                                {
                                    SendAnswer(loMySocket, "331 Anonymous access allowed, send identity (e-mail name) as password.", ref liTimeOutUser);
                                }
                                else
                                {
                                    SendAnswer(loMySocket, String.Concat("331 Password required for ", lsLogin, "."), ref liTimeOutUser);
                                }
                            }
                            else
                            {
                                if (lsLogin.Equals(String.Empty) == false)
                                {
                                    if (lsCmd.Equals("PASS") == true)
                                    {
                                        llResumeIndex = 0;
                                        lbLogined = false;

                                        loIni = new ClassIniReader(psRootConfigDirectory + "users" + Path.DirectorySeparatorChar + lsLogin + ".ini");

                                        if (loIni.FileExists == true)
                                        {
                                            if (loIni.GetValue("user", "disabled").Equals("yes", StringComparison.OrdinalIgnoreCase) == false)
                                            {
                                                if (lsLogin == "anonymous")
                                                {
                                                    lbLogined = true;

                                                    Log(String.Concat("[", loMySocket.RemoteEndPoint.ToString(), "] Anonymous password : ", lsParameter));
                                                }
                                                else
                                                {
                                                    if (loIni.GetValue("user", "passwordProtected").Equals("yes", StringComparison.OrdinalIgnoreCase) == true)
                                                    {
                                                        lsPassword = loIni.GetValue("user", "password");

                                                        if (EncodePassword(lsParameter) == lsPassword)
                                                        {
                                                            lbLogined = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (loIni.GetValue("user", "disabled").Equals("yes", StringComparison.OrdinalIgnoreCase) == false)
                                                        {
                                                            lsPassword = loIni.GetValue("user", "password");

                                                            if (lsPassword == lsParameter)
                                                            {
                                                                lbLogined = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (lbLogined == false)
                                        {
                                            SendAnswer(loMySocket, "530 lsLogin incorrect.", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            if (NumberLogin(lsLogin) >= piMaxSessionPerUser)
                                            {
                                                SendAnswer(loMySocket, String.Concat("530 Sorry, the maximum number of clients (", piMaxSessionPerUser, ") from your login are already connected."), ref liTimeOutUser);
                                            }
                                            else
                                            {
                                                AddLogin(lsLogin);
                                                SendAnswer(loMySocket, String.Concat("230 User ", lsLogin, " logged in."), ref liTimeOutUser);

                                                lsUserRoot = loIni.GetValue("user", "Root");
                                                lsCurrentWorkDir = lsUserRoot;
                                                lbDownload = loIni.GetValue("user", "Download").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbUpload = loIni.GetValue("user", "Upload").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbRename = loIni.GetValue("user", "Rename").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbDelete = loIni.GetValue("user", "Delete").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbMakeDirectory = loIni.GetValue("user", "MakeDirectory").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbDeleteDirectory = loIni.GetValue("user", "DeleteDirectory").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbModifyTime = loIni.GetValue("user", "ModifyTime").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                            }
                                        }
                                    }
                                    else if (lsCmd.Equals("SYST") == true)
                                    {
                                        llResumeIndex = 0;
                                        SendAnswer(loMySocket, "215 UNIX Type: L8", ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("MODE") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (lsParameter.Equals("S", StringComparison.OrdinalIgnoreCase) == true)
                                        {
                                            SendAnswer(loMySocket, "200 Mode S ok.", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, String.Concat("500 Mode ", lsParameter, " not implemented."), ref liTimeOutUser);
                                        }
                                    }
                                    else if ((lsCmd.Equals("PWD") == true) || (lsCmd.Equals("XPWD") == true))
                                    {
                                        llResumeIndex = 0;
                                        SendAnswer(loMySocket, String.Concat("257 \"", lsUserCurrentDirectory, "\" is current directory."), ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("PORT") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (liClientPort != -1)
                                        {
                                            FreePassivePort(liClientPort);
                                            liClientPort = -1;
                                        }

                                        lsPassiveMode = false;

                                        if (GetClientPort(lsParameter, ref loClientIPAddress, ref liClientPort) == true)
                                        {
                                            IPEndPoint ep = new IPEndPoint(loClientIPAddress, liClientPort);

                                            try
                                            {
                                                try
                                                {
                                                    /* Try close connection. If exception. Object is disposed. We must create it. */
                                                    loClientDataSocket.Close();
                                                    /* After close, Object is disposed */
                                                    loClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                }
                                                catch
                                                {
                                                    loClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                                }


                                                loClientDataSocket.Connect(ep);

                                                if (loClientDataListener != null)
                                                {
                                                    loClientDataListener.Stop();
                                                    loClientDataListener = null;
                                                }

                                                SendAnswer(loMySocket, "200 PORT command successful.", ref liTimeOutUser);
                                            }
                                            catch
                                            {
                                                SendAnswer(loMySocket, "500 Invalid PORT command or already use.", ref liTimeOutUser);
                                            }

                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "501 'PORT': Invalid number of parameters", ref liTimeOutUser);
                                        }
                                    }
                                    else if (lsCmd.Equals("PASV") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (liClientPort != -1)
                                        {
                                            FreePassivePort(liClientPort);
                                        }

                                        if (loClientDataSocket.Connected)
                                        {
                                            loClientDataSocket.Close();
                                        }

                                        liClientPort = GetPassivePort(ref loClientDataListener);

                                        if (liClientPort != -1)
                                        {
                                            lsPassiveMode = true;
                                            SendAnswer(loMySocket, String.Concat("227 Entering Passive Mode (", psIPAddress.Replace('.', ','), ",", (liClientPort >> 8), ",", (liClientPort & 0xFF), ")."), ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "500 PASV exception: 'No available PASV Ports'.", ref liTimeOutUser);
                                        }
                                    }
                                    else if (lsCmd.Equals("TYPE") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (lsParameter.Equals("I", StringComparison.OrdinalIgnoreCase) == true)
                                        {
                                            lbBinaryMode = true;
                                            SendAnswer(loMySocket, "200 Type set to I.", ref liTimeOutUser);
                                        }
                                        else if (lsParameter.Equals("A", StringComparison.OrdinalIgnoreCase) == true)
                                        {
                                            lbBinaryMode = false;
                                            SendAnswer(loMySocket, "200 Type set to A.", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "504 TYPE must be A or I.", ref liTimeOutUser);
                                        }

                                    }
                                    else if ((lsCmd.Equals("LIST") == true) || (lsCmd.Equals("NLST") == true))
                                    {
                                        llResumeIndex = 0;

                                        /* if LIST -aL */
                                        if (lsParameter.StartsWith("-") == true)
                                        {
                                            ExplodeCommand(lsParameter, ref lsCmd, ref lsParameter);
                                        }

                                        SendAnswer(loMySocket, "150 Opening ASCII mode data connection for /bin/ls.", ref liTimeOutUser);

                                        if (SendListDirectory(lsParameter, loClientDataListener, loClientDataSocket, lsPassiveMode, ref liError, lsUserRoot, lsUserCurrentDirectory, ref liTimeOutUser, (lsCmd == "NLST")) == true)
                                        {
                                            SendAnswer(loMySocket, "226 Transfer complete.", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                        }
                                    }
                                    else if (lsCmd.Equals("RETR") == true)
                                    {
                                        if (lbDownload == true)
                                        {
                                            try
                                            {
                                                lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter);

                                                if (File.Exists(lsLocalFileName) == true)
                                                {
                                                    SendAnswer(loMySocket, String.Concat("150 Opening ", (lbBinaryMode == true ? "binary" : "ASCII"), " mode data connection for ", lsParameter, "."), ref liTimeOutUser);

                                                    if (lbBinaryMode == true)
                                                    {
                                                        loInFileReadBinary = new FileStream(lsLocalFileName, FileMode.Open, FileAccess.Read);

                                                        if (SendBinaryFile(loInFileReadBinary, loClientDataListener, loClientDataSocket, lsPassiveMode, ref liError, llResumeIndex, ref liTimeOutUser) == true)
                                                        {
                                                            SendAnswer(loMySocket, MSG_TRANSFERT_COMPLET, ref liTimeOutUser);
                                                        }
                                                        else
                                                        {
                                                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                                        }

                                                        loInFileReadBinary.Close();
                                                    }
                                                    else
                                                    {
                                                        loInFileReadText = new StreamReader(lsLocalFileName);

                                                        if (SendTextFile(loInFileReadText, loClientDataListener, loClientDataSocket, lsPassiveMode, ref liError, ref liTimeOutUser) == true)
                                                        {
                                                            SendAnswer(loMySocket, MSG_TRANSFERT_COMPLET, ref liTimeOutUser);
                                                        }
                                                        else
                                                        {
                                                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                                        }

                                                        loInFileReadText.Close();
                                                    }
                                                }
                                                else
                                                {
                                                    SendAnswer(loMySocket, String.Format(MSG_FILE_NOT_FOUND, lsParameter), ref liTimeOutUser);
                                                }

                                            }
                                            catch
                                            {
                                                if (lsPassiveMode == true)
                                                {
                                                    loTmpClient = loClientDataListener.AcceptTcpClient();
                                                    loTmpClient.Client.Close();
                                                }
                                                else
                                                {
                                                    loClientDataSocket.Close();
                                                }

                                                SendAnswer(loMySocket, ERROR_PERMISSION_DENIED, ref liTimeOutUser);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "500 Cannot RETR.", ref liTimeOutUser);
                                        }

                                        llResumeIndex = 0;
                                    }
                                    else if (lsCmd.Equals("NOOP") == true)
                                    {
                                        llResumeIndex = 0;

                                        SendAnswer(loMySocket, "200 NOOP command successful.", ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("REST") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (long.TryParse(lsParameter, out llResumeIndex) == true)
                                        {
                                            SendAnswer(loMySocket, String.Concat("350 Restart transfert at ", llResumeIndex, "."), ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, String.Concat("501 Syntax liError in parameter: '", lsParameter, "' is not a valid integer value."), ref liTimeOutUser);
                                        }
                                    }
                                    else if ((lsCmd.Equals("CWD") == true) || (lsCmd.Equals("CDUP") == true) || (lsCmd.Equals("XCUP") == true))
                                    {
                                        llResumeIndex = 0;

                                        if ((lsCmd.Equals("CDUP") == true) || (lsCmd.Equals("XCUP") == true))
                                        {
                                            lsParameter = AddDirSeparatorAtEnd(lsUserCurrentDirectory, '/') + "..";
                                        }

                                        switch (ChangeDirectory(lsUserRoot, lsParameter, ref lsUserCurrentDirectory, ref lsCurrentWorkDir))
                                        {
                                            case 0:
                                                SendAnswer(loMySocket, "250 CWD command successful.", ref liTimeOutUser);
                                                break;
                                            case 1:
                                                SendAnswer(loMySocket, "550 Permission Denied.", ref liTimeOutUser);
                                                break;
                                            default:
                                                SendAnswer(loMySocket, String.Concat("550 ", lsParameter, ": No such file or directory."), ref liTimeOutUser);
                                                break;
                                        }
                                    }
                                    else if ((lsCmd.Equals("STOR") == true) || (lsCmd.Equals("APPE") == true))
                                    {
                                        if (lbUpload == true)
                                        {
                                            try
                                            {
                                                lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter);

                                                SendAnswer(loMySocket, String.Concat("150 Opening ", (lbBinaryMode == true ? "binary" : "ASCII"), " mode data connection for ", lsParameter, "."), ref liTimeOutUser);

                                                if (File.Exists(lsLocalFileName) == true)
                                                {
                                                    if (lsCmd.Equals("STOR") == true)
                                                    {
                                                        loOutFileWriteBinary = new FileStream(lsLocalFileName, FileMode.Open, FileAccess.Write);

                                                        /* trunc file if necessary */
                                                        loFi = new FileInfo(lsLocalFileName);

                                                        if (loFi.Length > llResumeIndex)
                                                        {
                                                            loOutFileWriteBinary.SetLength(llResumeIndex);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        loOutFileWriteBinary = new FileStream(lsLocalFileName, FileMode.Append, FileAccess.Write);
                                                    }
                                                }
                                                else
                                                {
                                                    loOutFileWriteBinary = new FileStream(lsLocalFileName, FileMode.Create, FileAccess.Write);
                                                }

                                                if (GetFile(loOutFileWriteBinary, loClientDataListener, loClientDataSocket, lsPassiveMode, ref liError, llResumeIndex, lbBinaryMode, ref liTimeOutUser) == true)
                                                {
                                                    SendAnswer(loMySocket, MSG_TRANSFERT_COMPLET, ref liTimeOutUser);
                                                }
                                                else
                                                {
                                                    ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                                }

                                                loOutFileWriteBinary.Close();
                                            }
                                            catch
                                            {
                                                if (lsPassiveMode == true)
                                                {
                                                    loTmpClient = loClientDataListener.AcceptTcpClient();
                                                    loTmpClient.Client.Close();
                                                }
                                                else
                                                {
                                                    loClientDataSocket.Close();
                                                }

                                                SendAnswer(loMySocket, ERROR_PERMISSION_DENIED, ref liTimeOutUser);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "500 Cannot RETR.", ref liTimeOutUser);
                                        }

                                        llResumeIndex = 0;
                                    }
                                    else if (lsCmd.Equals("MDTM") == true)
                                    {
                                        llResumeIndex = 0;

                                        lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter);

                                        if (CommandMDTM(lsLocalFileName, ref lsResult, ref liError) == true)
                                        {
                                            SendAnswer(loMySocket, String.Concat("213 ", lsResult), ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                        }
                                    }
                                    else if (lsCmd.Equals("SIZE") == true)
                                    {
                                        llResumeIndex = 0;

                                        lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter);

                                        if (CommandSIZE(lsLocalFileName, ref lsResult, ref liError) == true)
                                        {
                                            SendAnswer(loMySocket, String.Concat("213 ", lsResult), ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                        }
                                    }
                                    else if (lsCmd.Equals("UTF8") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (lsParameter.Equals("on", StringComparison.OrdinalIgnoreCase) == true)
                                        {
                                            lbUtf8Mode = true;
                                            SendAnswer(loMySocket, "200 UTF8 mode enabled", ref liTimeOutUser);
                                        }
                                        else if (lsParameter.Equals("off", StringComparison.OrdinalIgnoreCase) == true)
                                        {
                                            lbUtf8Mode = false;
                                            SendAnswer(loMySocket, "200 UTF8 mode disabled", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "501 Invalid UFT8 options", ref liTimeOutUser);
                                        }
                                    }
                                    else if (lsCmd.Equals( "CLNT") == true)
                                    {
                                        llResumeIndex = 0;

                                        Log(String.Concat("[", loMySocket.RemoteEndPoint.ToString(), "] Client name : ", lsParameter));
                                        SendAnswer(loMySocket, "200 Thank you", ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("MFMT") == true)
                                    {
                                        llResumeIndex = 0;

                                        if (lbModifyTime == true)
                                        {
                                            ExplodeCommand(lsParameter, ref lsModifyTimeOfFile, ref lsFileName);

                                            lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsFileName);

                                            if (CommandMFMT(lsLocalFileName, lsModifyTimeOfFile, ref liError) == true)
                                            {
                                                SendAnswer(loMySocket, String.Concat("213 modify=", lsModifyTimeOfFile, "; ", lsFileName), ref liTimeOutUser);
                                            }
                                            else
                                            {
                                                ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                                            }

                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, "500 Cannot MFMT.", ref liTimeOutUser);
                                        }
                                    }
                                    else if (lsCmd.Equals("FEAT") == true)
                                    {
                                        SendAnswer(loMySocket, "211-Features:" , ref liTimeOutUser);
                                        SendAnswer(loMySocket, " MDTM", ref liTimeOutUser);
                                        SendAnswer(loMySocket, " SIZE", ref liTimeOutUser);
                                        SendAnswer(loMySocket, " UTF8 ON|OFF", ref liTimeOutUser);
                                        SendAnswer(loMySocket, " CLNT", ref liTimeOutUser);
                                        SendAnswer(loMySocket, " MFMT", ref liTimeOutUser);
                                        SendAnswer(loMySocket, "211 End", ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("RNFR") == true)
                                    {
                                        if (lbRename == true)
                                        {
                                            lsRenameFrom = lsParameter;

                                            lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsRenameFrom);

                                            if ((File.Exists(lsLocalFileName) == true) || (Directory.Exists(lsLocalFileName) == true))
                                            {
                                                SendAnswer(loMySocket, "350 File exists, ready for destination name." , ref liTimeOutUser);
                                            }
                                            else
                                            {
                                                ShowError(loMySocket, ref liTimeOutUser, lsRenameFrom, FILE_NOT_FOUND);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, ERROR_PERMISSION_DENIED , ref liTimeOutUser);
                                        }
                                    }
                                    else if (lsCmd.Equals("RNTO") == true)
                                    {
                                        if (lbRename == true)
                                        {
                                            lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsRenameFrom);

                                            if (RenameFileOrDirectory(lsLocalFileName, FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter), ref liError, ref lbIsDirectory) == true)
                                            {
                                                SendAnswer(loMySocket, String.Format("250 {0} '{1}' renamed to '{2}'", (lbIsDirectory ? "Directory" : "File"), lsRenameFrom, lsParameter), ref liTimeOutUser);
                                            }
                                            else
                                            {
                                                ShowError(loMySocket, ref liTimeOutUser, lsRenameFrom, liError);
                                            }
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, ERROR_PERMISSION_DENIED , ref liTimeOutUser);
                                        }
                                    }
                                    else
                                    {
                                        llResumeIndex = 0;

                                        SendAnswer(loMySocket, "500 Command not understood.", ref liTimeOutUser);
                                    }
                                }
                                else
                                {
                                    SendAnswer(loMySocket, "503 lsLogin with USER first.", ref liTimeOutUser);
                                }
                            }
                        }
                    }
                    else
                    {
                        /* break for some time other wise with the loop the CPU is 100% */
                        Thread.Sleep(WAITING_TIME);

                        if (piTimeOut != 0)
                        {
                            liTimeOutUser--;
                        }
                    }
                }
                else
                {
                    break;
                }

                if (liTimeOutUser == 0)
                {
                    break;
                }
            }

            if (liTimeOutUser == 0)
            {
                SendAnswer(loMySocket, "503 Time out.", ref liTimeOutUser);
                loMySocket.Close();
            }


            lock (poLockThread)
            {
                piNbClient--;
            }

            DisconnectLogin(lsLogin);

            Log(String.Concat("[", lsClientIPAddressString, "] Connection close"));
        }

        /*
         * <summary>Disconnect user</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asLogin">login to be disconnected</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 10/07/2008 : use String.IsNullOrEmpty()
         * <history>
         */
        private void DisconnectLogin(String asLogin)
        {
            int liPos;

            lock (poLockThread)
            {

                /* Disconnect user if connected */
                if (String.IsNullOrEmpty(asLogin) == false)
                {
                    liPos = plConnectedUser.IndexOf(asLogin);

                    if (liPos != -1)
                    {
                        plNbSessionPerUser[liPos]--;

                        if (plNbSessionPerUser[liPos] <= 0)
                        {
                            plConnectedUser.Remove(plConnectedUser[liPos]);
                            plNbSessionPerUser.Remove(plNbSessionPerUser[liPos]);
                        }
                    }
                }
            }
        }

        /*
         * <summary>Retrun number of session for login</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asLogin">login user</param>
         * 
         * <returns>number of session</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 10/08/2008 : use String.IsNullOrEmpty()
         * <history>
         */
        private int NumberLogin(String asLogin)
        {
            int liPos;
            int liRetour = 0;

            lock (poLockThread)
            {
                if (String.IsNullOrEmpty(asLogin) == false)
                {
                    liPos = plConnectedUser.IndexOf(asLogin);

                    if (liPos != -1)
                    {
                        liRetour = plNbSessionPerUser[liPos];
                    }
                }
            }

            return liRetour;
        }

        /*
         * <summary>Add session for login</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asLogin">login user</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 10/07/2008 : use String.IsNullOrEmpty()
         * <history>
         */
        private void AddLogin(String asLogin)
        {
            int liPos;

            lock (poLockThread)
            {
                /* Disconnect user if connected */
                if (String.IsNullOrEmpty(asLogin) == false)
                {
                    liPos = plConnectedUser.IndexOf(asLogin);

                    if (liPos != -1)
                    {
                        plNbSessionPerUser[liPos]++;
                    }
                    else
                    {
                        plConnectedUser.Add(asLogin);
                        plNbSessionPerUser.Add(1);
                    }
                }
            }
        }

        /*
         * <summary>Convert string to MD5</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asPassword">password to be crypted</param>
         * 
         * <returns>crypted password</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private static string EncodePassword(string asPassword)
        {
            byte[] laOriginalBytes = System.Text.Encoding.ASCII.GetBytes(asPassword);
            byte[] laEncodedBytes = new MD5CryptoServiceProvider().ComputeHash(laOriginalBytes);
            StringBuilder loResult = new StringBuilder();
            int liIndex;

            for (liIndex = 0; liIndex < laEncodedBytes.Length; liIndex++)
            {
                loResult.Append(laEncodedBytes[liIndex].ToString("x2"));
            }

            return loResult.ToString();
        }

        /*
         * <summary>Convert PORT command to IP</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asAdresseIP">adress in in PORT command format xxx,xxx,xxx,xxx,yyy,yyy</param>
         * <param name="aoClientIPAddress">ip adress object</param>
         * <param name="aiClientPort">port of client</param>
         * 
         * <returns>true if correct format</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private bool GetClientPort(String asAdresseIP, ref IPAddress aoClientIPAddress, ref int aiClientPort)
        {
            bool lbRetour = false;
            byte[] laAdr = { 0, 0, 0, 0 };
            char[] laSeparateur = { ',' };
            int liValue;

            aoClientIPAddress = new IPAddress(laAdr);
            aiClientPort = -1;

            String[] laTmpAdr = asAdresseIP.Split(laSeparateur);

            if (laTmpAdr.Length == 6)
            {
                if (int.TryParse(laTmpAdr[0], out liValue) == true)
                {
                    laAdr[0] = (byte)liValue;

                    if (int.TryParse(laTmpAdr[1], out liValue) == true)
                    {
                        laAdr[1] = (byte)liValue;

                        if (int.TryParse(laTmpAdr[2], out liValue) == true)
                        {
                            laAdr[2] = (byte)liValue;

                            if (int.TryParse(laTmpAdr[3], out liValue) == true)
                            {
                                laAdr[3] = (byte)liValue;

                                if (int.TryParse(laTmpAdr[4], out liValue) == true)
                                {
                                    aiClientPort = liValue << 8;

                                    if (Int32.TryParse(laTmpAdr[5], out liValue) == true)
                                    {
                                        aiClientPort += liValue;

                                        aoClientIPAddress = new IPAddress(laAdr);

                                        lbRetour = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return lbRetour;
        }

        /*
         * <summary>Get passive port and create connection</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoClientDataListener">passive connection socket</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private int GetPassivePort(ref TcpListener aoClientDataListener)
        {
            int liRetour = -1;

            lock (poLockThread)
            {
                if (plPassivePortFree.Count > 0)
                {
                    liRetour = plPassivePortFree[0];
                    plPassivePortFree.Remove(plPassivePortFree[0]);

                    plPassivePortUse.Add(liRetour);

                    if (aoClientDataListener != null)
                    {
                        aoClientDataListener.Stop();
                    }

                    try
                    {
                        aoClientDataListener = new TcpListener(poIpAddress, liRetour);
                        aoClientDataListener.Start();
                    }
                    catch
                    {
                        aoClientDataListener = null;
                    }
                }
            }

            return liRetour;
        }

        /*
         * <summary>Send list of file in directory</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asDirectory">directory to list</param>
         * <param name="aoClientDataListener">passive connection socket</param>
         * <param name="aoClientSocket">active connection socket</param>
         * <param name="abPassiveMode">set true to use passive mode</param>
         * <param name="abPassiveMode">set true to use passive mode</param>
         * <param name="aiError">return error code</param>
         * <param name="asRoot">root directory</param>
         * <param name="asUserCurrentDir">user directory</param>
         * <param name="aiTimeOutUser">time out time</param
         * <param name="abNlst">true to user NSLT command format</param>
         * 
         * <returns>true if no error</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         *                add WAITING_TIME constante,
         * <history>
         */
        private bool SendListDirectory(String asDirectory, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, String asRoot, String asUserCurrentDir, ref int aiTimeOutUser, bool abNlst)
        {
            bool lbRetour = true;
            TcpClient loClient;
            Byte[] laMyBytes;
            Socket loMySocket;
            int liNbBytesRead;
            int liIndex;
            int liSize;
            int liNbBytes;
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();

            laMyBytes = poWindowsEncoding.GetBytes(ListDirectory(asDirectory, asUserCurrentDir, asRoot, ref aiError, abNlst));

            if (aiError == 0)
            {
                try
                {
                    if (abPassiveMode == true)
                    {
                        loClient = aoClientDataListener.AcceptTcpClient();
                        loMySocket = loClient.Client;
                    }
                    else
                    {
                        loMySocket = aoClientSocket;
                    }

                    liNbBytesRead = laMyBytes.Length;
                    liIndex = 0;
                    liSize = liNbBytesRead;

                    do
                    {
                        liNbBytes = loMySocket.Send(laMyBytes, liIndex, liSize, SocketFlags.None, out loErrorSocket);

                        if (liNbBytes == 0)
                        {
                            if (loErrorSocket != System.Net.Sockets.SocketError.WouldBlock)
                            {
                                /* Connection close */
                                aiError = CONNECTION_CLOSE_BY_CLIENT ;
                                break;
                            }
                            else
                            {
                                /* Why ??? */
                                Thread.Sleep(WAITING_TIME);
                                aiTimeOutUser--;

                                if (aiTimeOutUser == 0)
                                {
                                    lbRetour = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            lbRetour = true;
                            aiTimeOutUser = piTimeOut;
                        }

                        if (pbCancel == true)
                        {
                            break;
                        }

                        liIndex += liNbBytes;
                        liSize = laMyBytes.Length - liIndex;
                    }
                    while (liSize > 0);

                    lbRetour = true;

                    loMySocket.Close();
                }
                catch
                {
                    lbRetour = false;
                }
            }

            return lbRetour;
        }

        /*
         * <summary>Extract file name in FTP format string</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asFileName">string contain directory et file name</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 08/07/2008 : use Char.Equals(),
         * <history>
         */
        private String ExtractFileName(String asFileName)
        {
            int liIndex;
            String lsRetour = String.Empty;

            for (liIndex = asFileName.Length - 1; liIndex >= 0; liIndex--)
            {
                if (asFileName[liIndex].Equals('/') == true)
                {
                    break;
                }
                else
                {
                    lsRetour = asFileName[liIndex] + lsRetour;
                }
            }

            return lsRetour;
        }

        /*
         * <summary>Create line of directory for SendListDirectory() function</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asDirName">directory name</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 10/07/2008 : use String.Concat(),
         * - 14/07/2008 : use StringBuilder
         * <history>
         */
        private void ListDir(String asDirName, System.Text.StringBuilder aoString)
        {
            DateTime loDtDir = Directory.GetCreationTime(asDirName);
            String lsRights;

            aoString.Append("d");

            try
            {
                String[] laListOfFile = System.IO.Directory.GetFiles(asDirName, "*.*");
                lsRights = "rwx";
            }
            catch
            {
                lsRights = "-wx";
            }

            aoString.Append(lsRights);
            aoString.Append(lsRights);
            aoString.Append(lsRights);
            aoString.Append("   1 ftp      ftp 0 ");
            aoString.Append(paMonthLabel[loDtDir.Month - 1]);
            aoString.Append(" ");

            if (loDtDir.Day < 10)
            {
                aoString.Append("0");
            }

            aoString.Append(loDtDir.Day.ToString());
            aoString.Append(" ");

            if (loDtDir.Year == DateTime.Now.Year)
            {
                if (loDtDir.Hour < 10)
                {
                    aoString.Append("0");
                }

                aoString.Append(loDtDir.Hour.ToString());
                aoString.Append(":");

                if (loDtDir.Minute < 10)
                {
                    aoString.Append("0");
                }

                aoString.Append(loDtDir.Minute.ToString());
                aoString.Append(" ");
            }
            else
            {
                aoString.Append(" ");
                aoString.Append(loDtDir.Year.ToString());
                aoString.Append(" ");
            }

            aoString.Append(Path.GetFileName(asDirName));
        }

        /*
         * <summary>Create line of file for SendListDirectory() function</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asFileName">file name</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 10/07/2008 : use String.Concat(),
         * - 14/07/2008 : use StringBuilder,
         * <history>
         */
        private void ListFile(String asFileName, System.Text.StringBuilder aoString)
        {
            String lsRights = String.Empty;
            DateTime loDtFile = File.GetLastWriteTime(asFileName);
            FileInfo loFi = new FileInfo(asFileName); ;

            aoString.Append("-");

            try
            {
                FileStream fs = new FileStream(asFileName, FileMode.Open, FileAccess.Read);
                fs.Close();

                lsRights = "r";
            }
            catch
            {
                lsRights = "-";
            }

            if ((File.GetAttributes(asFileName) & FileAttributes.ReadOnly) != 0)
            {
                lsRights = String.Concat(lsRights, "-");
            }
            else
            {
                lsRights = String.Concat(lsRights, "w");
            }

            lsRights = String.Concat(lsRights, "w");

            aoString.Append(lsRights);
            aoString.Append(lsRights);
            aoString.Append(lsRights);
            aoString.Append("   1 ftp      ftp ");
            aoString.Append(loFi.Length);
            aoString.Append(" ");
            aoString.Append(paMonthLabel[loDtFile.Month - 1]);
            aoString.Append(" ");

            if (loDtFile.Day < 10)
            {
                aoString.Append("0");
            }

            aoString.Append(loDtFile.Day.ToString());
            aoString.Append(" ");

            if (loDtFile.Year == DateTime.Now.Year)
            {
                if (loDtFile.Hour < 10)
                {
                    aoString.Append("0");
                }

                aoString.Append(loDtFile.Hour.ToString());
                aoString.Append(":");

                if (loDtFile.Minute < 10)
                {
                    aoString.Append("0");
                }

                aoString.Append(loDtFile.Minute.ToString());
                aoString.Append(" ");
            }
            else
            {
                aoString.Append(" ");
                aoString.Append(loDtFile.Year.ToString());
                aoString.Append(" ");
            }

            aoString.Append(Path.GetFileName(asFileName));
        }

        /*
         * <summary>List file and sub-directory of directory</summary>
         * 
         * <remarks>
         * error = 0 : OK
         * error = 1 : can't open data connection
         * error = 2 : access denied
         * error = 3 : not find
         * </remarks>
         * 
         * <param name="asDir">directory name to list</param>
         * <param name="asUserCurrentDir">user current directory</param>
         * <param name="asRoot">root directory</param>
         * <param name="aiError">return error</param>
         * <param name="abNlst">true to user NLST command format</param>
         * 
         * <returns>return list of directory in string</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 10/07/2008 : use String.Empty,
         * <history>
         */
        private String ListDirectory(String asDir, String asUserCurrentDir, String asRoot, ref int aiError, bool abNlst)
        {
            int liIndex;
            String lsLocalDir = FTPPathToOSPath(asRoot, asUserCurrentDir, asDir);
            System.Text.StringBuilder loString = new System.Text.StringBuilder(String.Empty);

            aiError = 1;

            try
            {
                if (Directory.Exists(lsLocalDir) == true)
                {
                    String[] laListOfFile = System.IO.Directory.GetFiles(lsLocalDir, "*.*");
                    String[] laListOfDirecotry = System.IO.Directory.GetDirectories(lsLocalDir, "*.*");

                    for (liIndex = 0; liIndex < laListOfDirecotry.Length; liIndex++)
                    {
                        if (abNlst == true)
                        {
                            loString.Append(laListOfDirecotry[liIndex]);
                        }
                        else
                        {
                            ListDir(laListOfDirecotry[liIndex], loString);
                        }

                        loString.Append(EOL);
                    }

                    for (liIndex = 0; liIndex < laListOfFile.Length; liIndex++)
                    {
                        if (abNlst == true)
                        {
                            loString.Append(laListOfFile[liIndex]);
                        }
                        else
                        {
                            ListFile(laListOfFile[liIndex], loString);
                        }

                        loString.Append(EOL);
                    }

                    aiError = SUCCESS;
                }
                else if (File.Exists(lsLocalDir) == true)
                {
                    ListFile(lsLocalDir, loString);
                    loString.Append(EOL);
                }
                else
                {
                    aiError = FILE_NOT_FOUND;
                }
            }
            catch
            {
                aiError = ACCES_DENIED;
            }

            return loString.ToString();
        }

        /*
         * <summary>Extract command and parameter</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asLine">line reading</param>
         * <param name="asCmd">return command (e.g. LIST)</param>
         * <param name="asParameter">return parameter (e.g. /truc)</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * - 08/07/2008 : use Char.Equals()
         * <history>
         */
        private void ExplodeCommand(String asLine, ref String asCmd, ref String asParameter)
        {
            int liIndex;

            asCmd = "";
            asParameter = "";

            for (liIndex = 0; liIndex < asLine.Length; liIndex++)
            {
                if (asLine[liIndex].Equals(' ') == true)
                {
                    liIndex++;
                    break;
                }
                else
                {
                    asCmd += asLine[liIndex];
                }
            }

            asCmd = asCmd.ToUpper();
            asParameter = asLine.Substring(liIndex);
        }

        /*
         * <summary>Free a passive port</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aiPort">passive port to be free</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private void FreePassivePort(int aiPort)
        {
            int liPos;
            int liFreePort;

            liPos = plPassivePortUse.IndexOf(aiPort);

            if (liPos != -1)
            {
                liFreePort = plPassivePortUse[liPos];
                plPassivePortUse.Remove(plPassivePortUse[liPos]);
                plPassivePortFree.Add(liFreePort);
            }
        }

        /*
         * <summary>Change directory</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asRoot">root user directory</param>
         * <param name="asDir">directory to be change</param>
         * <param name="asUserCurrentDirectory">current user directory</param>
         * <param name="asCurrentWorkDirectory">user current work directory</param>
         * 
         * <returns>0 = OK
         *  1 = Acces denied
         *  2 = Not found</returns>
         *  
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private int ChangeDirectory(String asRoot, String asDir, ref String asUserCurrentDirectory, ref String asCurrentWorkDirectory)
        {
            int liRetour = -1;
            String lsNewDir = "";
            String lsNewPath = "";

            if (asDir.Length > 0)
            {
                /* if it's relative path */
                if (asDir[0].Equals('/') == false)
                {
                    lsNewDir = AddDirSeparatorAtEnd(asUserCurrentDirectory, '/') + asDir;
                }
                else
                {
                    lsNewDir = asDir;
                }
            }
            else
            {
                lsNewDir = asUserCurrentDirectory;
            }

            /* Vérification du chemin */
            lsNewPath = lsNewDir;

            if (lsNewPath.Length > 0)
            {
                if (lsNewPath[0].Equals('/') == true)
                {
                    lsNewPath = lsNewPath.Substring(1);
                }
            }

            lsNewPath = AddDirSeparatorAtEnd(asRoot, Path.DirectorySeparatorChar) + lsNewPath.Replace('/', Path.DirectorySeparatorChar);
            lsNewPath = AddDirSeparatorAtEnd(Path.GetFullPath(lsNewPath), Path.DirectorySeparatorChar) ;

            if (Directory.Exists(lsNewPath) == true)
            {
                if (asCurrentWorkDirectory.StartsWith(asRoot) == true)
                {
                    try
                    {
                        String[] ListOfFile = System.IO.Directory.GetFiles(lsNewPath, "*.*");

                        liRetour = 0;

                        if (lsNewPath.StartsWith(asRoot) == true)
                        {
                            asUserCurrentDirectory = lsNewPath.Substring(asRoot.Length).Replace(Path.DirectorySeparatorChar, '/');
                            asCurrentWorkDirectory = lsNewPath;
                        }
                        else
                        {
                            liRetour = 1;
                        }
                    }
                    catch
                    {
                        liRetour = 1;
                    }
                }
                else
                {
                    liRetour = 1;
                }
            }
            else
            {
                liRetour = 2;
            }

            return liRetour;
        }

        /*
         * <summary>Translate FTP path to OS path</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="asRoot">user root directory</param>
         * <param name="asUserCurrentDir">user current directory</param>
         * <param name="asName">name of directory/file name</param>
         * 
         * <returns>return path</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 08/07/2008 : use Char.Equals(),
         * - 10/07/2008 : use String.Empty,
         * <history>
         */
        private String FTPPathToOSPath(String asRoot, String asUserCurrentDir, String asName)
        {
            String lsRetour = String.Empty;

            lsRetour = AddDirSeparatorAtEnd(asRoot, Path.DirectorySeparatorChar);

            /* We delete first / */
            if (asUserCurrentDir.Length > 0)
            {
                if (asUserCurrentDir[0].Equals('/') == true)
                {
                    asUserCurrentDir = asUserCurrentDir.Substring(1);
                }
            }

            /* We delete first / */
            if (asName.Length > 0)
            {
                if (asName[0].Equals('/') == true)
                {
                    asName = asName.Substring(1);
                }
                else
                {
                    asName = AddDirSeparatorAtEnd(asUserCurrentDir, '/') + asName;
                }
            }
            else
            {
                asName = asUserCurrentDir;
            }

            asName = asName.Replace('/', Path.DirectorySeparatorChar);

            return lsRetour + asName;

        }

        /*
         * <summary>Show error</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoClientSocket">client socket</param>
         * <param name="aiTimeOutUser">time out time</param>
         * <param name="asFileName">file name that have error</param>
         * <param name="aiError">error number</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private void ShowError(Socket aoClientSocket, ref int aiTimeOutUser, String asFileName, int aiError)
        {
            if (aiError == CANT_OPEN_DATA_CONNECTION)
            {
                SendAnswer(aoClientSocket, "425 Can't open data connection.", ref aiTimeOutUser);
            }
            else if (aiError == ACCES_DENIED)
            {
                SendAnswer(aoClientSocket, "500 Access denied.", ref aiTimeOutUser);
            }
            else if (aiError == FILE_NOT_FOUND)
            {
                SendAnswer(aoClientSocket, String.Format(MSG_FILE_NOT_FOUND, asFileName), ref aiTimeOutUser);
            }
            else if (aiError == CONNECTION_CLOSE_BY_CLIENT)
            {
                SendAnswer(aoClientSocket, "421 Connection close by client.", ref aiTimeOutUser);
            }
            else if (aiError == CONNECTION_CLOSE_SERVER_SHUTDOWN)
            {
                SendAnswer(aoClientSocket, "421 Connection close cause server shutdown.", ref aiTimeOutUser);
            }
            else if (aiError == NO_ENOUGH_SPACE)
            {
                SendAnswer(aoClientSocket, "426 Connection close by server. Not enough space.", ref aiTimeOutUser);
            }
            else if (aiError == NOT_VALID_DATE)
            {
                SendAnswer(aoClientSocket, "501 Not a valid date.", ref aiTimeOutUser);
            }
            else if (aiError == SYNTAX_ERROR)
            {
                SendAnswer(aoClientSocket, "501 Syntax error.", ref aiTimeOutUser);
            }
            else if (aiError == FILE_ALREADY_EXIST)
            {
                SendAnswer(aoClientSocket, String.Format("553 '{0}': file already exists.", asFileName), ref aiTimeOutUser);
            }
            else if (aiError == DIRECTORY_ALREADY_EXIST)
            {
                SendAnswer(aoClientSocket, String.Format("553 '{0}': directory already exists.", asFileName), ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Send a binary file</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoInFileReadBinary">object file binary to read</param>
         * <param name="aoClientDataListener">client socket for pasive mode</param>
         * <param name="aoClientSocket">client socket for active mode</param>
         * <param name="abPassiveMode">true use passive mode</param>
         * <param name="aiError">!= 0 -> error see ShowError() for more detail</param>
         * <param name="alStartIndex">start index of file (resume command)</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name, 
         *                add WAITING_TIME constante,
         * <history>
         */
        private bool SendBinaryFile(FileStream aoInFileReadBinary, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, long alStartIndex, ref int aiTimeOutUser)
        {
            bool lbRetour = false;
            TcpClient loClient;
            Byte[] laMyBytes = new Byte[BLOCK_SIZE];
            Socket loMySocket;
            int liNbBytesRead ;
            bool lbBlockingState = aoClientSocket.Blocking;
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            int liIndex = 0;
            int liSize = laMyBytes.Length;
            int liNbBytes = 0;
            bool lbInternalError = false;

            aiError = SUCCESS;

            aoClientSocket.Blocking = false;

            try
            {
                if (abPassiveMode == true)
                {
                    loClient = aoClientDataListener.AcceptTcpClient();
                    loMySocket = loClient.Client;
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                loMySocket.Blocking = false ;

                if (alStartIndex > 0)
                {
                    aoInFileReadBinary.Seek(alStartIndex, SeekOrigin.Begin);
                }

                do
                {
                    liNbBytesRead = aoInFileReadBinary.Read(laMyBytes, 0, laMyBytes.Length);

                    if (liNbBytesRead > 0)
                    {
                        liIndex = 0 ;
                        liSize = liNbBytesRead ;

                        do
                        {
                            liNbBytes = loMySocket.Send(laMyBytes, liIndex, liSize, SocketFlags.None, out loErrorSocket);

                            if (liNbBytes == 0)
                            {
                                if (loErrorSocket != System.Net.Sockets.SocketError.WouldBlock)
                                {
                                    /* Connection close */
                                    aiError = CONNECTION_CLOSE_BY_CLIENT;
                                    lbInternalError = true;
                                    break;
                                }
                                else
                                {
                                    /* Why ??? */
                                    Thread.Sleep(WAITING_TIME);
                                    aiTimeOutUser--;

                                    if (aiTimeOutUser == 0)
                                    {
                                        lbRetour = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                lbRetour = true;
                                aiTimeOutUser = piTimeOut;
                            }

                            if (pbCancel == true)
                            {
                                break;
                            }

                            liIndex += liNbBytes;
                            liSize = laMyBytes.Length - liIndex;
                        }
                        while (liSize > 0);
                    }
                }
                while ((liNbBytesRead == laMyBytes.Length) && (lbInternalError == false)); 

                if (pbCancel == true)
                {
                    aiError = CONNECTION_CLOSE_SERVER_SHUTDOWN;
                }
                else
                {
                    lbRetour = true;
                }

                loMySocket.Close();
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            aoClientSocket.Blocking = lbBlockingState;

            return lbRetour;
        }

        /*
         * <summary>Send a text file</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoInFileReadText">object text file to read</param>
         * <param name="aoClientDataListener">client socket for pasive mode</param>
         * <param name="aoClientSocket">client socket for active mode</param>
         * <param name="abPassiveMode">true use passive mode</param>
         * <param name="aiError">!= 0 -> error see ShowError() for more detail</param>
         * <param name="alStartIndex">start index of file (resume command)</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         *                add WAITING_TIME constante,
         * <history>
         */
        private bool SendTextFile(StreamReader aoInFileReadText, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, ref int aiTimeOutUser)
        {
            bool lbRetour = false;
            TcpClient loClient;
            Byte[] laMyBytes  ;
            Socket loMySocket;
            bool lbBlockingState = aoClientSocket.Blocking;
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            int liIndex = 0;
            int liSize = 0;
            int nbBytes = 0;
            String lsLine;

            aiError = SUCCESS;

            aoClientSocket.Blocking = false;

            try
            {
                if (abPassiveMode == true)
                {
                    loClient = aoClientDataListener.AcceptTcpClient();
                    loMySocket = loClient.Client;
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                loMySocket.Blocking = false ;

                do
                {
                    lsLine = aoInFileReadText.ReadLine() + EOL;

                    laMyBytes = poWindowsEncoding.GetBytes(lsLine);

                    liIndex = 0;
                    liSize = laMyBytes.Length;

                    do
                    {
                        nbBytes = loMySocket.Send(laMyBytes, liIndex, liSize, SocketFlags.None, out loErrorSocket);

                        if (nbBytes == 0)
                        {
                            if (loErrorSocket != System.Net.Sockets.SocketError.WouldBlock)
                            {
                                /* Connection close */
                                aiError = CONNECTION_CLOSE_BY_CLIENT;
                                break;
                            }
                            else
                            {
                                /* Why ??? */
                                Thread.Sleep(WAITING_TIME);
                                aiTimeOutUser--;

                                if (aiTimeOutUser == 0)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            lbRetour = true;
                            aiTimeOutUser = piTimeOut;
                        }

                        if (pbCancel == true)
                        {
                            break;
                        }

                        liIndex += nbBytes;
                        liSize = laMyBytes.Length - liIndex;
                    }
                    while (liSize > 0);
                }
                while (aoInFileReadText.EndOfStream == false);

                if (pbCancel == true)
                {
                    aiError = CONNECTION_CLOSE_SERVER_SHUTDOWN;
                }
                else
                {
                    lbRetour = true;
                }

                loMySocket.Close();
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            aoClientSocket.Blocking = lbBlockingState;

            return lbRetour;
        }

        /*
         * <summary>Extract IP Address from IPAdress.ToString()</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asIPAddr">IPAdress.ToString()</param>
		 *
         * <returns>no return</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 08/07/2008 : use Char.Equals(),
         * - 10/07/2008 : use String.Empty,
         * <history>
         */
        private String ExtractIp(String asIPAddr)
        {
            String lsRetour = String.Empty;
            int liIndex;

            for (liIndex = 0; liIndex < asIPAddr.Length; liIndex++)
            {
                if (asIPAddr[liIndex].Equals(':') == true)
                {
                    break;
                }
                else
                {
                    lsRetour += asIPAddr[liIndex];
                }
            }

            return lsRetour;
        }

        /*
         * <summary>Get a file (text or binary) from client</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoOutFileWriteBinary">object file to write</param>
         * <param name="aoClientDataListener">client socket for pasive mode</param>
         * <param name="aoClientSocket">client socket for active mode</param>
         * <param name="abPassiveMode">true use passive mode</param>
         * <param name="aiError">!= 0 -> error see ShowError() for more detail</param>
         * <param name="alStartIndex">start index of file (resume command)</param>
         * <param name="abBinaryMode">true if write binary file</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name,
         *                add WAITING_TIME constante,
         * <history>
         */
        private bool GetFile(FileStream aoOutFileWriteBinary, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, long alStartIndex, bool abBinaryMode, ref int aiTimeOutUser)
        {
            bool lbRetour = false;
            TcpClient loClient;
            Byte[] laMyBytes = new Byte[BLOCK_SIZE];
            Socket loMySocket;
            bool lbBlockingState = aoClientSocket.Blocking;
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            int liSize = laMyBytes.Length;
            int liNbBytes = 0;
            String lsLine;

            aiError = SUCCESS;

            aoClientSocket.Blocking = false;

            try
            {
                if (alStartIndex > 0)
                {
                    aoOutFileWriteBinary.Seek(alStartIndex, SeekOrigin.Begin);
                }

                if (abPassiveMode == true)
                {
                    loClient = aoClientDataListener.AcceptTcpClient();
                    loMySocket = loClient.Client;
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                loMySocket.Blocking = false;

                do
                {
                    liNbBytes = loMySocket.Receive(laMyBytes, 0, laMyBytes.Length, SocketFlags.None, out loErrorSocket);

                    if (liNbBytes == 0)
                    {
                        if (loErrorSocket != System.Net.Sockets.SocketError.WouldBlock)
                        {
                            /* Connection close */
                            lbRetour = true;
                            break;
                        }
                        else
                        {
                            Thread.Sleep(WAITING_TIME);
                            aiTimeOutUser--;

                            if (aiTimeOutUser == 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (abBinaryMode == false)
                        {
                            lsLine = poWindowsEncoding.GetString(laMyBytes);
                            lsLine = lsLine.Replace("\r\n", Environment.NewLine);

                            /* when can split \r\n when read block */
                            if (lsLine.EndsWith("\r") == true)
                            {
                                lsLine = lsLine.Substring(0, lsLine.Length - 1) + Environment.NewLine;
                            }

                            if (lsLine.StartsWith("\n") == true)
                            {
                                lsLine = lsLine.Substring(1);
                            }

                            laMyBytes = poWindowsEncoding.GetBytes(lsLine);
                        }

                        try
                        {
                            aoOutFileWriteBinary.Write(laMyBytes, 0, liNbBytes);
                        }
                        catch
                        {
                            aiError = NO_ENOUGH_SPACE;
                            break;
                        }

                        /* Reinit time out */
                        aiTimeOutUser = piTimeOut;
                    }
                }
                while (aiTimeOutUser > 0);
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            aoClientSocket.Blocking = lbBlockingState;

            return lbRetour;
        }

        /*
         * <summary>Send modification time of a file</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asLocalFileName">file to get last write time</param>
         * <param name="asResult">result of command</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>no return</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private bool CommandMDTM(String asLocalFileName, ref String asResult, ref int aiError)
        {
            DateTime loFdt = File.GetLastWriteTime(asLocalFileName);
            bool lbRetour = false;

            asResult = "";

            if (File.Exists(asLocalFileName) == true)
            {
                aiError = SUCCESS;
                lbRetour = true;                
                asResult = loFdt.ToString("yyyyMMddHHmmss");
            }
            else
            {
                aiError = FILE_NOT_FOUND;
            }

            return lbRetour;
        }

        /*
         * <summary>Send size of a file</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asLocalFileName">file to get last write time</param>
         * <param name="asResult">result of command (size of file)</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>no return</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name,
         * - 10/07/2008 : use String.Empty,
         * <history>
         */
        private bool CommandSIZE(String asLocalFileName, ref String asResult, ref int aiError)
        {
            FileInfo loFi;
            bool lbRetour = false;

            asResult = String.Empty;

            if (File.Exists(asLocalFileName) == true)
            {
                aiError = SUCCESS;
                lbRetour = true;
                loFi = new FileInfo(asLocalFileName);
                asResult = loFi.Length.ToString();
            }
            else
            {
                aiError = FILE_NOT_FOUND;
            }

            return lbRetour;
        }

        /*
         * <summary>Modify time of file</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asLocalFileName">file to get last write time</param>
         * <param name="asFDateTime">new date/time of last write access</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>no return</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private bool CommandMFMT(String asLocalFileName, String asFDateTime, ref int aiError)
        {
            String lsYear;
            String lsMonth;
            String lsDay;
            String lsHour;
            String lsMinute;
            String lsSeconde;
            bool lbRetour = false;

            aiError = SYNTAX_ERROR;

            if (File.Exists(asLocalFileName) == true)
            {
                if (asFDateTime.Length == 14)
                {
                    lsYear = asFDateTime.Substring(0, 4);
                    lsMonth = asFDateTime.Substring(4, 2);
                    lsDay = asFDateTime.Substring(6, 2);
                    lsHour = asFDateTime.Substring(8, 2);
                    lsMinute = asFDateTime.Substring(10, 2);
                    lsSeconde = asFDateTime.Substring(12, 2);

                    try
                    {
                        File.SetLastWriteTime(asLocalFileName, new DateTime(int.Parse(lsYear), int.Parse(lsMonth), int.Parse(lsDay), int.Parse(lsHour), int.Parse(lsMinute), int.Parse(lsSeconde)));

						aiError = 0;
						
                        lbRetour = true;
                    }
                    catch
                    {
                        aiError = NOT_VALID_DATE;
                    }
                }
            }
            else
            {
                aiError = FILE_NOT_FOUND;
            }

            return lbRetour;
        }

        /*
         * <summary>Rename file or directory</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asLocalFileNameFrom">file to rename</param>
         * <param name="asLocalFileNameTo">new name</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>true if success</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private bool RenameFileOrDirectory(String asLocalFileNameFrom, String asLocalFileNameTo, ref int aiError, ref bool abDirectory)
        {
            bool lbRetour = false;

            abDirectory = false;

            if (File.Exists(asLocalFileNameFrom) == true)
            {
                try
                {
                    File.Move(asLocalFileNameFrom, asLocalFileNameTo);

                    aiError = SUCCESS;
                    lbRetour = true;
                }
                catch (IOException)
                {
                    aiError = FILE_ALREADY_EXIST;
                }
                catch
                {
                    aiError = ACCES_DENIED;
                }
            }
            else if (Directory.Exists(asLocalFileNameFrom) == true)
            {
                try
                {
                    Directory.Move(asLocalFileNameFrom, asLocalFileNameTo);

                    aiError = SUCCESS;
                    lbRetour = true;
                    abDirectory = true;
                }
                catch (IOException)
                {
                    aiError = FILE_ALREADY_EXIST;
                }
                catch
                {
                    aiError = ACCES_DENIED;
                }
            }
            else
            {
                aiError = FILE_NOT_FOUND;
            }

            return lbRetour;
        }
    }
}
