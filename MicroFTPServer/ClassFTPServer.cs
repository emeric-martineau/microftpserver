/*
 * MicroFTPServer
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
 * [OK] STOU
 * [OK] APPE file name - APPEND (with create) ()       501 Permission Denied
 * ALLO
 * [OK] REST
 * [OK] RNFR       Rename from.	        bool Rename
 * [OK] RNTO       501 Permission Denied bool Rename
 * [OK] ABOR
 * [OK] DELE       501 Permission Denied bool Delete
 * [OK] RMD XRMD      501 Permission Denied bool DeleteDirectory
 * [OK] MKD XMKD      501 Permission Denied bool MakeDirectory
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
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class ClassFTPServer
    {
        public delegate void LogFunc(String Text);
        public LogFunc OnLog = null;

        /* root directory to find config file */
        private String psRootConfigDirectory = String.Empty;
        /* defaut listen port */
        private int piPort = 21;
        /* defaut listen ip adress */
        private String psIPAddress = "127.0.0.1";
        /* defaut welcom message */
        private String psWelcomeMessage = String.Empty;
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
        /* Order of deny/allow */
        private bool pbDenyPriority = true;
        /* Goodbye message */
        private String psGoodbyeMessage = String.Empty;
        /* Block size to read/write file from/to client */
        private int piBlockSize ;
        /* Waiting time between each block */
        private int piWaitingTime = 0;

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
        private const String ERROR_INVALID_BUFFER_SIZE = "** ERROR ** Invalide buffer size.";
        private const String ERROR_PERMISSION_DENIED = "501 Permission Denied.";
        private const String LOG_NEW_CONNECTION = "[{0}] New connection";
        private const String MSG_TRANSFERT_COMPLET = "226 Transfer complete.";
        private const String MSG_FILE_NOT_FOUND = "550 {0}: No such file or directory.";
        private const String ERROR_TRANSFERT_IN_PROGRESS = "501 File transfert in progress, stop it before do an other.";
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
        private const int TIME_OUT_FOR_PASSIVE_CONNECTION = 11;

        /*
         * Constante
         */
        private const String EOL = "\r\n";
        private const int WAITING_TIME = 1000;
        private int BLOCK_SIZE = 512;

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
         * - 05/09/2008 : add DenyPriority parameter,
         * - 09/09/2008 : add goodbye message,
         * <history>
         */
        public void Start()
        {
            /* Config file */
            ClassIniReader loGeneralIni = new ClassIniReader(psRootConfigDirectory + "general.ini");
            /* Temporary string to read ini key before convert in integer or regex */
            String lsTmp;
            /* Loop counter of passive port */ 
            int liIndexPassivePort;
            /* Passive port separator */
            char[] laPassivePortSeparateur = { '-' };
            /* String to read port of server in ini file */
            String[] laTmpPort;
            /* Separator of allow/deny IP adress */
            char[] laIPSeparator = { ',' };
            /* Byte rate per user per secondes */
            int liByteRateUser;

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

                            if (psWelcomeMessage.Equals(String.Empty) == true)
                            {
                                psWelcomeMessage = String.Concat("220 Welcome to ", Dns.GetHostName());
                            }
                            else
                            {
                                psWelcomeMessage = ConvertStringWithEOL(psWelcomeMessage, "220");
                            }

                            psGoodbyeMessage = loGeneralIni.GetValue("main", "GoodbyeMessage");

                            if (psGoodbyeMessage.Equals(String.Empty) == true)
                            {
                                psGoodbyeMessage = "221 Goodbye.";
                            }
                            else
                            {
                                psGoodbyeMessage = ConvertStringWithEOL(psGoodbyeMessage, "221");
                            }

                            lsTmp = loGeneralIni.GetValue("main", "MaxSessionPerUser");

                            if (int.TryParse(lsTmp, out piMaxSessionPerUser) == true)
                            {
                                lsTmp = loGeneralIni.GetValue("main", "MaxClient");

                                if (int.TryParse(lsTmp, out piMaxClient) == true)
                                {
                                    lsTmp = loGeneralIni.GetValue("main", "FullLog");

                                    pbFullLog = lsTmp.Equals("yes", StringComparison.OrdinalIgnoreCase);

                                    lsTmp = loGeneralIni.GetValue("main", "DenyPriority");

                                    pbDenyPriority = lsTmp.Equals("yes", StringComparison.OrdinalIgnoreCase);

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

                                                    lsTmp = loGeneralIni.GetValue("main", "BufferSize");

                                                    if (int.TryParse(lsTmp, out piBlockSize) == true)
                                                    {
                                                        BLOCK_SIZE = piBlockSize ;

                                                        lsTmp = loGeneralIni.GetValue("main", "ByteRateUser");

                                                        if (int.TryParse(lsTmp, out liByteRateUser) == true)
                                                        {
                                                            if (liByteRateUser > 0)
                                                            {
                                                                if (liByteRateUser <= BLOCK_SIZE)
                                                                {
                                                                    piBlockSize = liByteRateUser;
                                                                    piWaitingTime = 1000; /* 1 second */
                                                                }
                                                                else if (liByteRateUser > BLOCK_SIZE)
                                                                {
                                                                    piWaitingTime = (int)((float)BLOCK_SIZE / liByteRateUser * 1000);
                                                                }
                                                            }
                                                        }

                                                        Run();
                                                    }
                                                    else
                                                    {
                                                        Log(ERROR_INVALID_BUFFER_SIZE);
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
         *                user String.Concat(),
         * - 05/09/2008 : add comments,
         * <history>
         */
        private void Run()
        {
            /* Server IP */
            IPEndPoint loEp = new IPEndPoint(poIpAddress, piPort);
            /* Listener */
            TcpListener loTl = new TcpListener(loEp);
            /* Client thread */
            Thread loClientThread;
            /* Time out user */
            int liTimeOutUser = piTimeOut;
            /* Regular expression of allow/deny IP address */
            System.Text.RegularExpressions.Match loMatchRegEx;
            /* Loop counter of allow/deny IP address */
            int liIndex;
            /* true if IP address are allow */
            bool lbAllow = false;
            /* true if IP address are deny */
            bool lbDeny = false;
            /* IP address of client */
            String lsClientIPAddress;
            /* IP are allowed ? */
            bool lbIPAllowed = false;

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

                    if (pbDenyPriority == true)
                    {
                        lbIPAllowed = ((lbAllow == true) && (lbDeny == false));
                    }
                    else
                    {
                        lbIPAllowed = lbAllow;
                    }

                    if (lbIPAllowed == true)
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
         * - 10/08/2008 : use String.Empty(),
         * - 05/09/2008 : add coments,
         * <history>
         */
        private bool SendAnswer(Socket aoClientSocket, String asCmd, ref int aiTimeOutUser)
        {
            /* true if send command ok */
            bool lbRetour = false;
            /* Buffer of command to be send */
            Byte[] laMyBytes = poWindowsEncoding.GetBytes(asCmd + EOL ) ;
            /* Save old blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Number of byte send */
            int liNbBytes;
            /* Socket error */
            System.Net.Sockets.SocketError loError = new System.Net.Sockets.SocketError();
            /* Index byte to be send */
            int liIndex = 0;
            /* Size of buffer */
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
                        aiTimeOutUser -= WAITING_TIME / 1000 ;

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
         * - 05/09/2008 : add comments
         * <history>
         */
        private bool ReadCommand(Socket aoClientSocket, ref String asLine, bool abUtf8Mode, ref bool abReading, ref int aiTimeOutUser)
        {    
            /* true if answer are read */
            bool lbRetour = true;
            /* Buffer to read */
            Byte[] laBuffer = new Byte[1];
            /* Old blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Number byte to be read*/
            int liNbBytes;
            /* Socket error */
            System.Net.Sockets.SocketError loError = new System.Net.Sockets.SocketError();
            /* Line to be read */
            byte[] laByteLine = {};

            asLine = String.Empty;
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
                    // 05/09/2008 : possible Deny Of Service if client send byte per byte a infinity string each second
                    //aiTimeOutUser = piTimeOut;

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
         * - 02/09/2008 : create ActivePortCommand and PassivePortCommand function,
         * - 03/09/2008 : create TypeCommand, ListNlstCommand, SendFileCommand,
         *                ChangeDirectoryCommand, ReadFileTimeCommand, SizeFileCommand
         *                UTF8Command function,
         * - 04/09/2008 : create ModifyFileTimeCommand, FeatCommand, RenameFileFromCommand,
         *                RenameFileToCommand function,
         * - 09/09/2008 : add goodbye message, add subdir right,
         * - 28/09/2008 : add DELE, RMD, XRMD, MKD, XMKD command,
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
            bool lbPassiveMode = false;
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
            /* String represente IP:Port of client */
            String lsClientIPAddressString = String.Empty;
            /* for seeking file */
            long llResumeIndex = 0;
            /* OS current work directory */
            String lsCurrentWorkDir = String.Empty;
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
            /* SubDir right */
            bool lbSubDir = false;
            /* Size of block to send/receive from/to client */
            int liBlockSize = piBlockSize;
            /* Waiting to between  */
            int liWaitingTime = piWaitingTime;
            /* User byte rate */
            int liByteRateUser = 0 ;
            /* Temporary string */
            String lsTmp;
            /* Object to get a file from client */
            ClassGetFile loGetFile = new ClassGetFile();
            /* Object to send a file to client */
            ClassSendFile loSendFile = new ClassSendFile();
            /* Object to get a file */
            BackgroundWorker loGetSendFileBackgroundWorker = null;

            /* free ClientIP for can start other client */
            poClientIP = null;

            lsClientIPAddressString = loMySocket.RemoteEndPoint.ToString();

            SendAnswer(loMySocket, psWelcomeMessage, ref liTimeOutUser);

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
                                SendAnswer(loMySocket, psGoodbyeMessage, ref liTimeOutUser);
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
                                            lsLogin = String.Empty;
                                            SendAnswer(loMySocket, "530 Login incorrect.", ref liTimeOutUser);
                                        }
                                        else
                                        {
                                            if ((NumberLogin(lsLogin) >= piMaxSessionPerUser) && (piMaxSessionPerUser > 0))
                                            {
                                                lsLogin = String.Empty;
                                                SendAnswer(loMySocket, String.Concat("530 Sorry, the maximum number of clients (", piMaxSessionPerUser, ") from your login are already connected."), ref liTimeOutUser);
                                            }
                                            else
                                            {
                                                liBlockSize = piBlockSize;
                                                liWaitingTime = piWaitingTime;

                                                lsUserRoot = loIni.GetValue("user", "Root");
                                                lsCurrentWorkDir = lsUserRoot;
                                                lbDownload = loIni.GetValue("user", "Download").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbUpload = loIni.GetValue("user", "Upload").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbRename = loIni.GetValue("user", "Rename").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbDelete = loIni.GetValue("user", "Delete").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbMakeDirectory = loIni.GetValue("user", "MakeDirectory").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbDeleteDirectory = loIni.GetValue("user", "DeleteDirectory").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbModifyTime = loIni.GetValue("user", "ModifyTime").Equals("yes", StringComparison.OrdinalIgnoreCase);
                                                lbSubDir = loIni.GetValue("user", "SubDir").Equals("yes", StringComparison.OrdinalIgnoreCase);

                                                lsTmp = loIni.GetValue("user", "ByteRate");

                                                if (int.TryParse(lsTmp, out liByteRateUser) == true)
                                                {
                                                    if (liByteRateUser > 0)
                                                    {
                                                        if (liByteRateUser <= BLOCK_SIZE)
                                                        {
                                                            liBlockSize = liByteRateUser;
                                                            liWaitingTime = 1000; /* 1 second */
                                                        }
                                                        else if (liByteRateUser > BLOCK_SIZE)
                                                        {
                                                            liWaitingTime = (int)((float)BLOCK_SIZE / liByteRateUser * 1000);
                                                        }
                                                    }
                                                    else if (liByteRateUser == -1)
                                                    {
                                                        liBlockSize = BLOCK_SIZE;
                                                        liWaitingTime = 0;
                                                    }
                                                }

                                                if (System.IO.Directory.Exists(lsUserRoot) == false)
                                                {
                                                    lsLogin = String.Empty;
                                                    SendAnswer(loMySocket, "530 Not  logged in, cannot find home directory.", ref liTimeOutUser);                                                    
                                                }
                                                else
                                                {
                                                    AddLogin(lsLogin);
                                                    SendAnswer(loMySocket, String.Concat("230 User ", lsLogin, " logged in."), ref liTimeOutUser);
                                                }
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

                                        lbPassiveMode = false;

                                        ActivePortCommand(ref liClientPort, lsParameter, ref loClientIPAddress, ref loClientDataSocket, ref loClientDataListener, loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("PASV") == true)
                                    {
                                        llResumeIndex = 0;

                                        lbPassiveMode = PassivePortCommand(ref liClientPort, loClientDataSocket, loMySocket, ref liTimeOutUser, ref loClientDataListener) ;
                                    }
                                    else if (lsCmd.Equals("TYPE") == true)
                                    {
                                        llResumeIndex = 0;

                                        TypeCommand(lsParameter, ref lbBinaryMode, loMySocket, ref liTimeOutUser);
                                    }
                                    else if ((lsCmd.Equals("LIST") == true) || (lsCmd.Equals("NLST") == true))
                                    {
                                        llResumeIndex = 0;

                                        ListNlstCommand(lsCmd, lsParameter, lbSubDir, loClientDataSocket, loClientDataListener, loMySocket, lbPassiveMode, lsUserRoot, lsUserCurrentDirectory, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("RETR") == true)
                                    {
                                        loSendFile.gsParameter = lsParameter;
                                        loSendFile.gbDownload = lbDownload;
                                        loSendFile.gsUserRoot = lsUserRoot;
                                        loSendFile.gsUserCurrentDirectory = lsUserCurrentDirectory;
                                        loSendFile.goMySocket = loMySocket;
                                        loSendFile.goClientDataListener = loClientDataListener;
                                        loSendFile.goClientDataSocket = loClientDataSocket;
                                        loSendFile.gbPassiveMode = lbPassiveMode;
                                        loSendFile.glResumeIndex = llResumeIndex;
                                        loSendFile.giTimeOutUser = liTimeOutUser;
                                        loSendFile.gbBinaryMode = lbBinaryMode;
                                        loSendFile.giWaitingTime = liWaitingTime ;
                                        loSendFile.giBlockSize = liBlockSize;

                                        if ((loGetSendFileBackgroundWorker == null) || (loGetSendFileBackgroundWorker.IsBusy == false))
                                        {
                                            loGetSendFileBackgroundWorker = new BackgroundWorker();
                                            loGetSendFileBackgroundWorker.WorkerReportsProgress = true;
                                            loGetSendFileBackgroundWorker.WorkerSupportsCancellation = true;
                                            loGetSendFileBackgroundWorker.DoWork += SendFileBackGroundWorker_DoWork;
                                            loGetSendFileBackgroundWorker.RunWorkerAsync(loSendFile);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, ERROR_TRANSFERT_IN_PROGRESS, ref liTimeOutUser);
                                        }

                                        llResumeIndex = 0;
                                    }
                                    else if ((lsCmd.Equals("NOOP") == true) || (lsCmd.Equals("NOP") == true))
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

                                        ChangeDirectoryCommand(lsCmd, lbSubDir, lsUserRoot, ref lsUserCurrentDirectory, ref lsCurrentWorkDir, loMySocket, ref liTimeOutUser);
                                    }
                                    else if ((lsCmd.Equals("STOR") == true) || (lsCmd.Equals("APPE") == true) || (lsCmd.Equals("STOU") == true))
                                    {
                                        loGetFile.gsCmd = lsCmd;
                                        loGetFile.gsParameter = lsParameter;
                                        loGetFile.gbUpload = lbUpload;
                                        loGetFile.gsUserRoot = lsUserRoot;
                                        loGetFile.gsUserCurrentDirectory = lsUserCurrentDirectory;
                                        loGetFile.goMySocket = loMySocket;
                                        loGetFile.goClientDataListener = loClientDataListener;
                                        loGetFile.goClientDataSocket = loClientDataSocket;
                                        loGetFile.gbPassiveMode = lbPassiveMode;
                                        loGetFile.glResumeIndex = llResumeIndex;
                                        loGetFile.giTimeOutUser = liTimeOutUser;
                                        loGetFile.gbBinaryMode = lbBinaryMode;
                                        loGetFile.giWaitingTime = liWaitingTime;

                                        if ((loGetSendFileBackgroundWorker == null) || (loGetSendFileBackgroundWorker.IsBusy == false))
                                        {
                                            loGetSendFileBackgroundWorker = new BackgroundWorker();
                                            loGetSendFileBackgroundWorker.WorkerReportsProgress = true;
                                            loGetSendFileBackgroundWorker.WorkerSupportsCancellation = true;
                                            loGetSendFileBackgroundWorker.DoWork += GetFileBackGroundWorker_DoWork;
                                            loGetSendFileBackgroundWorker.RunWorkerAsync(loGetFile);
                                        }
                                        else
                                        {
                                            SendAnswer(loMySocket, ERROR_TRANSFERT_IN_PROGRESS, ref liTimeOutUser);
                                        }

                                        llResumeIndex = 0;
                                    }
                                    else if (lsCmd.Equals("MDTM") == true)
                                    {
                                        llResumeIndex = 0;

                                        ReadFileTimeCommand(lsParameter, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("SIZE") == true)
                                    {
                                        llResumeIndex = 0;

                                        SizeFileCommand(lsParameter, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("UTF8") == true)
                                    {
                                        llResumeIndex = 0;

                                        UTF8Command(lsParameter, ref lbUtf8Mode, loMySocket, ref liTimeOutUser);
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

                                        ModifyFileTimeCommand(lsParameter, lsFileName, lbModifyTime, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser) ;
                                    }
                                    else if (lsCmd.Equals("FEAT") == true)
                                    {
                                        llResumeIndex = 0;

                                        FeatCommand(loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("RNFR") == true)
                                    {
                                        llResumeIndex = 0;

                                        RenameFileFromCommand(lsParameter, ref lsRenameFrom, lbRename, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("RNTO") == true)
                                    {
                                        llResumeIndex = 0;

                                        RenameFileToCommand(lsParameter, ref lsRenameFrom, lbRename, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);                                        
                                    }
                                    else if (lsCmd.Equals("DELE") == true)
                                    {
                                        llResumeIndex = 0;

                                        DeleteFileCommand(lsParameter, lbDelete, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if ((lsCmd.Equals("RMD") == true) || (lsCmd.Equals("XRMD") == true))
                                    {
                                        llResumeIndex = 0;

                                        DeleteDirectoryCommand(lsParameter, lbDeleteDirectory, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if ((lsCmd.Equals("MKD") == true) || (lsCmd.Equals("XMKD") == true))
                                    {
                                        llResumeIndex = 0;

                                        MakeDirectoryCommand(lsParameter, lbMakeDirectory, lsUserRoot, lsUserCurrentDirectory, loMySocket, ref liTimeOutUser);
                                    }
                                    else if (lsCmd.Equals("ABOR") == true)
                                    {
                                        if (loGetSendFileBackgroundWorker != null)
                                        {
                                            loGetSendFileBackgroundWorker.CancelAsync() ;

                                            SendAnswer(loMySocket, "426 Connection closed; transfer aborted.", ref liTimeOutUser);
                                        }

                                        SendAnswer(loMySocket, "226 ABOR command successful.", ref liTimeOutUser);
                                    }
                                    else
                                    {
                                        llResumeIndex = 0;

                                        SendAnswer(loMySocket, "500 Command not understood.", ref liTimeOutUser);
                                    }
                                }
                                else
                                {
                                    SendAnswer(loMySocket, "503 Login with USER first.", ref liTimeOutUser);
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

                /* Close active port socket */
                if (loClientDataSocket.Connected)
                {
                    loClientDataSocket.Close();
                }

                if (liClientPort != -1)
                {
                    FreePassivePort(liClientPort);
                }
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
            /* Number of session */
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
         * - 06/07/2008 : rename variable name,
         * - 05/09/2008 : add comments,
         * <history>
         */
        private bool GetClientPort(String asAdresseIP, ref IPAddress aoClientIPAddress, ref int aiClientPort)
        {
            bool lbRetour = false;
            /* IP address */
            byte[] laAdr = { 0, 0, 0, 0 };
            /* Separator of byte IP */
            char[] laSeparateur = { ',' };
            /* Temporary value of IP address byte */
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
         * - 06/07/2008 : rename variable name,
         * - 05/09/2008 : create a loop for if port already use by other program,
         * - 06/09/2008 : add comments,
         * <history>
         */
        private int GetPassivePort(ref TcpListener aoClientDataListener)
        {
            /* Return value */
            int liRetour = -1;
            /* Loop counter of free port list */
            int liIndexFreePort;
            /* Count of free port list */
            int liCountOfFreePort = plPassivePortFree.Count;

            lock (poLockThread)
            {
                if (aoClientDataListener != null)
                {
                    aoClientDataListener.Stop();
                    aoClientDataListener = null;
                }

                for(liIndexFreePort = 0; liIndexFreePort < liCountOfFreePort; liIndexFreePort++)
                {
                    liRetour = plPassivePortFree[0];
                    plPassivePortFree.Remove(plPassivePortFree[0]);

                    try
                    {
                        aoClientDataListener = new TcpListener(poIpAddress, liRetour);
                        aoClientDataListener.Start();

                        /* put port to used list */
                        plPassivePortUse.Add(liRetour);

                        break;
                    }
                    catch
                    {
                        /* port is already used */
                        aoClientDataListener = null;

                        /* put port at end of free port list */
                        plPassivePortFree.Add(liRetour);

                        /* reinit return port */
                        liRetour = -1;
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
         * <param name="abSubDir">True if user can be show sub dir</param>
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
         * - 06/07/2008 : add comments,
         * <history>
         */
        private bool SendListDirectory(String asDirectory, bool abSubDir, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, String asRoot, String asUserCurrentDir, ref int aiTimeOutUser, bool abNlst)
        {
            /* Return value */
            bool lbRetour = true;
            /* Byte arry to be send */
            Byte[] laMyBytes;
            /* Client socket */
            Socket loMySocket;
            /* Index of byte to be send */
            int liIndex;
            /* Size to be send */
            int liSize;
            /* Number of byte are sending ok */
            int liNbBytes;
            /* Old blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Socket error */
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();

            laMyBytes = poWindowsEncoding.GetBytes(ListDirectory(asDirectory, abSubDir, asUserCurrentDir, asRoot, ref aiError, abNlst));

            if (aiError == 0)
            {
                try
                {
                    aoClientSocket.Blocking = false;

                    if (abPassiveMode == true)
                    {
                        loMySocket = GetPassiveConnection(aoClientDataListener, ref aiTimeOutUser);
                    }
                    else
                    {
                        loMySocket = aoClientSocket;
                    }

                    if (loMySocket != null)
                    {
                        liIndex = 0;
                        liSize = laMyBytes.Length;

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
                    else
                    {
                        aiError = TIME_OUT_FOR_PASSIVE_CONNECTION;
                    }
                }
                catch
                {
                    lbRetour = false;
                }
            }

            if (abPassiveMode == true)
            {
                aoClientDataListener.Stop();
            }
            else
            {
                aoClientSocket.Blocking = lbBlockingState;
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
         * - 06/09/2008 : add comments, use IsReadOnly,
         * <history>
         */
        private void ListFile(String asFileName, System.Text.StringBuilder aoString)
        {
            /* Rights of file */
            String lsRights = String.Empty;
            /* Date/Time of file */
            DateTime loDtFile = File.GetLastWriteTime(asFileName);
            /* File information (to know length, right...) */
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

            if (loFi.IsReadOnly)
            {
                lsRights = String.Concat(lsRights, "--");
            }
            else
            {
                lsRights = String.Concat(lsRights, "ww");
            }

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
         * <param name="abSubDir">True if user can be show directory</param>
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
         * - 06/09/2008 : add comments,
         * <history>
         */
        private String ListDirectory(String asDir, bool abSubDir, String asUserCurrentDir, String asRoot, ref int aiError, bool abNlst)
        {
            /* Loop counter for file and directory list */
            int liIndex;
            /* Add current directory */
            String lsLocalDir = FTPPathToOSPath(asRoot, asUserCurrentDir, asDir);
            /* Create string builder */
            System.Text.StringBuilder loString = new System.Text.StringBuilder(String.Empty);

            aiError = 1;

            try
            {
                if (Directory.Exists(lsLocalDir) == true)
                {
                    if (abSubDir == true)
                    {
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
                    }

                    String[] laListOfFile = System.IO.Directory.GetFiles(lsLocalDir, "*.*");

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

            asCmd = String.Empty;
            asParameter = String.Empty;

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
         * - 06/07/2008 : rename variable name,
         * - 06/09/2008 : add coments,
         * <history>
         */
        private void FreePassivePort(int aiPort)
        {
            /* Position of port in used port list */
            int liPos;

            liPos = plPassivePortUse.IndexOf(aiPort);

            if (liPos != -1)
            {
                plPassivePortUse.Remove(plPassivePortUse[liPos]);
                plPassivePortFree.Add(aiPort);
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
         * - 06/07/2008 : rename variable name,
         * - 06/09/2008 : add coments,
         * <history>
         */
        private int ChangeDirectory(String asRoot, String asDir, ref String asUserCurrentDirectory, ref String asCurrentWorkDirectory)
        {
            /* return value */
            int liRetour = -1;
            /* New directory name (ftp format) */
            String lsNewDir = String.Empty;
            /* New path (OS format) */
            String lsNewPath = String.Empty;

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
            else if (aiError == TIME_OUT_FOR_PASSIVE_CONNECTION)
            {
                SendAnswer(aoClientSocket, "425 Time out for passive connection.", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Send a binary file</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoBackgroundWorker">BackGroundWorker</param>
         * <param name="aoInFileReadBinary">object file binary to read</param>
         * <param name="aoClientDataListener">client socket for pasive mode</param>
         * <param name="aoClientSocket">client socket for active mode</param>
         * <param name="abPassiveMode">true use passive mode</param>
         * <param name="aiError">!= 0 -> error see ShowError() for more detail</param>
         * <param name="alStartIndex">start index of file (resume command)</param>
         * <param name="aiWaitingTime">Waiting time between send block</param>
         * <param name="aiBlockSize">Size of block to send</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name, 
         *                add WAITING_TIME constante,
         * - 07/09/2008 : add comments,
         * - 30/09/2008 : add aoBackgroundWorker,
         * <history>
         */
        private bool SendBinaryFile(BackgroundWorker aoBackgroundWorker, FileStream aoInFileReadBinary, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, long alStartIndex, int aiWaitingTime, int aiBlockSize, ref int aiTimeOutUser)
        {
            /* return value */
            bool lbRetour = false;
            /* buffer */
            Byte[] laMyBytes = new Byte[aiBlockSize];
            /* Client socket */
            Socket loMySocket;
            /* Number byte read in file */
            int liNbBytesRead ;
            /* Old socket blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Socket error */
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            /* Index in buffer to be send */
            int liIndex = 0;
            /* Size of buffer */
            int liSize = laMyBytes.Length;
            /* Number byte send */
            int liNbBytes = 0;
            /* True in error occure */
            bool lbInternalError = false;

            aiError = SUCCESS;            

            aoClientSocket.Blocking = false ;

            try
            {
                if (abPassiveMode == true)
                {
                    loMySocket = GetPassiveConnection(aoClientDataListener, ref aiTimeOutUser);
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                if (loMySocket != null)
                {
                    loMySocket.Blocking = false;

                    /* If resume */
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

                        Thread.Sleep(aiWaitingTime);

                        if ((aoBackgroundWorker.CancellationPending == true) || (pbCancel == true))
                        {
                            break;
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
                else
                {
                    aiError = TIME_OUT_FOR_PASSIVE_CONNECTION;
                }
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            if (abPassiveMode == false)
            {
                aoClientSocket.Blocking = lbBlockingState;
            }
            else
            {
                aoClientDataListener.Stop();
            }

            return lbRetour;
        }

        /*
         * <summary>Send a text file</summary>
         * 
         * <remarks></remarks>
         * 
         * <param name="aoBackgroundWorker">BackGroundWorker</param>
         * <param name="aoInFileReadText">object text file to read</param>
         * <param name="aoClientDataListener">client socket for pasive mode</param>
         * <param name="aoClientSocket">client socket for active mode</param>
         * <param name="abPassiveMode">true use passive mode</param>
         * <param name="aiError">!= 0 -> error see ShowError() for more detail</param>
         * <param name="alStartIndex">start index of file (resume command)</param>
         * <param name="aiWaitingTime">Waiting time between send block</param>
         * <param name="aiBlockSize">Size of block to send</param>
         * <param name="aiTimeOutUser">time out time</param>
         * 
         * <returns>no return</returns>
         * 
         * <history>
         * - 06/07/2008 : rename variable name
         *                add WAITING_TIME constante,
         * - 07/09/2008 : add comments,
         * - 30/09/2008 : add aoBackgroundWorker,
         * <history>
         */
        private bool SendTextFile(BackgroundWorker aoBackgroundWorker, StreamReader aoInFileReadText, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, int aiWaitingTime, int aiBlockSize, ref int aiTimeOutUser)
        {
            /* Return value */
            bool lbRetour = false;
            /* buffer */
            Byte[] laMyBytes  ;
            /* Client socket */
            Socket loMySocket;
            /* Old blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Socket error */
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            /* Index in buffer */
            int liIndex = 0;
            /* Buffer size */
            int liSize = 0;
            /* Number byte are send */
            int liNbBytes = 0;
            /* Current line in file */
            String lsLine;

            aiError = SUCCESS;

            aoClientSocket.Blocking = false;

            try
            {
                if (abPassiveMode == true)
                {
                    loMySocket = GetPassiveConnection(aoClientDataListener, ref aiTimeOutUser);
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                if (loMySocket != null)
                {
                    loMySocket.Blocking = false;

                    do
                    {
                        lsLine = aoInFileReadText.ReadLine() + EOL;

                        laMyBytes = poWindowsEncoding.GetBytes(lsLine);

                        liIndex = 0;
                        liSize = laMyBytes.Length;

                        do
                        {
                            liNbBytes = loMySocket.Send(laMyBytes, liIndex, liSize, SocketFlags.None, out loErrorSocket);

                            if (liNbBytes == 0)
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

                            liIndex += liNbBytes;
                            liSize = laMyBytes.Length - liIndex;

                            if ((aoBackgroundWorker.CancellationPending == true) || (pbCancel == true))
                            {
                                break;
                            }
                        }
                        while (liSize > 0);

                        Thread.Sleep(lsLine.Length * aiWaitingTime / aiBlockSize);
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
                else
                {
                    aiError = TIME_OUT_FOR_PASSIVE_CONNECTION;
                }
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            if (abPassiveMode == false)
            {
                aoClientSocket.Blocking = lbBlockingState;
            }
            else
            {
                aoClientDataListener.Stop();
            }

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
         * - 07/09/2008 : add comments,
         * <history>
         */
        private bool GetFile(BackgroundWorker aoBackgroundWorker, FileStream aoOutFileWriteBinary, TcpListener aoClientDataListener, Socket aoClientSocket, bool abPassiveMode, ref int aiError, long alStartIndex, bool abBinaryMode, ref int aiTimeOutUser)
        {
            /* Return value */
            bool lbRetour = false;
            /* buffer */
            Byte[] laMyBytes = new Byte[BLOCK_SIZE];
            /* Client soclet */
            Socket loMySocket;
            /* Old blocking state */
            bool lbBlockingState = aoClientSocket.Blocking;
            /* Socket error */
            System.Net.Sockets.SocketError loErrorSocket = new System.Net.Sockets.SocketError();
            /* Buffer size */
            int liSize = laMyBytes.Length;
            /* Number byte reading */
            int liNbBytes = 0;
            /* Current line if text file */
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
                    loMySocket = GetPassiveConnection(aoClientDataListener, ref aiTimeOutUser);
                }
                else
                {
                    loMySocket = aoClientSocket;
                }

                if (loMySocket != null)
                {
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

                        if ((aoBackgroundWorker.CancellationPending == true) || (pbCancel == true))
                        {
                            break;
                        }
                    }
                    while (aiTimeOutUser > 0);

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
                else
                {
                    aiError = TIME_OUT_FOR_PASSIVE_CONNECTION;
                }
            }
            catch
            {
                aiError = CANT_OPEN_DATA_CONNECTION;
            }

            if (abPassiveMode == false)
            {
                aoClientSocket.Blocking = lbBlockingState;
            }
            else
            {
                aoClientDataListener.Stop();
            }

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
         * - 06/07/2008 : rename variable name,
         * - 07/09/2008 : add comments,
         * <history>
         */
        private bool CommandMDTM(String asLocalFileName, ref String asResult, ref int aiError)
        {
            /* Time of file */
            DateTime loFdt = File.GetLastWriteTime(asLocalFileName);
            /* return value */
            bool lbRetour = false;

            asResult = String.Empty;

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
         * - 07/09/2008 : add comments,
         * <history>
         */
        private bool CommandSIZE(String asLocalFileName, ref String asResult, ref int aiError)
        {
            /* File information */
            FileInfo loFi;
            /* Return value */
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
            /* Return value */
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
         * <param name="abDirectory">return true if file is directory</param>
		 *
         * <returns>true if success</returns>
		 *
         * <history>
         * - 06/07/2008 : rename variable name
         * <history>
         */
        private bool RenameFileOrDirectory(String asLocalFileNameFrom, String asLocalFileNameTo, ref int aiError, ref bool abDirectory)
        {
            /* Return value */
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

        /*
         * <summary>PORT Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="aiClientPort">client port</param>
         * <param name="asIPAdressAndPort">IP adress and port send by PORT command</param>
         * <param name="aoClientIPAddress">Object IPAdress to setup</param>
         * <param name="aoClientDataSocket">Socket client</param>
         * <param name="aoClientDataListener">Passive client listener</param>
         * <param name="aoMySocket">Command socket</param>
         * <param name="aiTimeOutUser">Time out</param>
		 *
         * <returns>void</returns>
		 *
         * <history>
         * - 02/09/2008 : create function
         * <history>
         */
        private void ActivePortCommand(ref int aiClientPort, String asIPAdressAndPort, ref IPAddress aoClientIPAddress, ref Socket aoClientDataSocket, ref TcpListener aoClientDataListener, Socket aoMySocket, ref int aiTimeOutUser)
        {
            if (aiClientPort != -1)
            {
                FreePassivePort(aiClientPort);
                aiClientPort = -1;
            }

            if (GetClientPort(asIPAdressAndPort, ref aoClientIPAddress, ref aiClientPort) == true)
            {
                IPEndPoint ep = new IPEndPoint(aoClientIPAddress, aiClientPort);

                try
                {
                    try
                    {
                        /* Try close connection. If exception. Object is disposed. We must create it. */
                        aoClientDataSocket.Close();
                        /* After close, Object is disposed */
                        aoClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                    catch
                    {
                        aoClientDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }

                    aoClientDataSocket.Connect(ep);

                    if (aoClientDataListener != null)
                    {
                        aoClientDataListener.Stop();
                        aoClientDataListener = null;
                    }

                    SendAnswer(aoMySocket, "200 PORT command successful.", ref aiTimeOutUser);
                }
                catch
                {
                    SendAnswer(aoMySocket, "500 Invalid PORT command or already use.", ref aiTimeOutUser);
                }
            }
            else
            {
                SendAnswer(aoMySocket, "501 'PORT': Invalid number of parameters", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>PASV Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="aiClientPort">client port</param>
         * <param name="aoClientDataSocket">Socket client</param>
         * <param name="aoMySocket">Command socket</param>         
         * <param name="aiTimeOutUser">Time out</param>
         * <param name="aoClientDataListener">Passive data listener</param>
		 *
         * <returns>true if success</returns>
		 *
         * <history>
         * - 02/09/2008 : create function
         * <history>
         */
        private bool PassivePortCommand(ref int aiClientPort, Socket aoClientDataSocket, Socket aoMySocket, ref int aiTimeOutUser, ref TcpListener aoClientDataListener)
        {
            /* Return value */
            bool lbPassiveMode = false ;

            if (aiClientPort != -1)
            {
                FreePassivePort(aiClientPort);
                aiClientPort = -1 ;
            }

            if (aoClientDataSocket.Connected)
            {
                aoClientDataSocket.Close();
            }

            aiClientPort = GetPassivePort(ref aoClientDataListener);

            if (aiClientPort != -1)
            {
                lbPassiveMode = true;

                SendAnswer(aoMySocket, String.Concat("227 Entering Passive Mode (", psIPAddress.Replace('.', ','), ",", (aiClientPort >> 8), ",", (aiClientPort & 0xFF), ")."), ref aiTimeOutUser);
            }
            else
            {
                SendAnswer(aoMySocket, "500 PASV exception: 'No available PASV Ports'.", ref aiTimeOutUser);
            }

            return lbPassiveMode ;
        }

        /*
         * <summary>TYPE Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command</param>
         * <param name="abBinaryMode">Set binary mode or not set if invalide type</param>
         * <param name="aoMySocket">Command socket</param>         
         * <param name="aiTimeOutUser">Time out</param>
		 *
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void TypeCommand(String asParameter, ref bool abBinaryMode, Socket aoMySocket, ref int aiTimeOutUser)
        {
            if (asParameter.Equals("I", StringComparison.OrdinalIgnoreCase) == true)
            {
                abBinaryMode = true;
                SendAnswer(aoMySocket, "200 Type set to I.", ref aiTimeOutUser);
            }
            else if (asParameter.Equals("A", StringComparison.OrdinalIgnoreCase) == true)
            {
                abBinaryMode = false;
                SendAnswer(aoMySocket, "200 Type set to A.", ref aiTimeOutUser);
            }
            else
            {
                SendAnswer(aoMySocket, "504 TYPE must be A or I.", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>LIST/NLST Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asCmd">Command (LIST or NLST)</param>
         * <param name="asParameter">Parameter of command</param>
         * <param name="abSubDir">True if user can be show sub dir</param>
         * <param name="aoClientDataSocket">Socket client</param>
         * <param name="aoClientDataListener">Passive client listener</param>
         * <param name="aoMySocket">Command socket</param>
         * <param name="abPassiveMode">True if client is in passive mode</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void ListNlstCommand(String asCmd, String asParameter, bool abSubDir, Socket aoClientDataSocket, TcpListener aoClientDataListener, Socket aoMySocket, bool abPassiveMode, String asUserRoot, String asUserCurrentDirectory, ref int aiTimeOutUser)
        {
            String lsTmp = String.Empty;
            int aiError = 0;

            /* if LIST -aL */
            if (asParameter.StartsWith("-") == true)
            {
                ExplodeCommand(asParameter, ref lsTmp, ref asParameter);
            }

            SendAnswer(aoMySocket, "150 Opening ASCII mode data connection for /bin/ls.", ref aiTimeOutUser);

            if (SendListDirectory(asParameter, abSubDir, aoClientDataListener, aoClientDataSocket, abPassiveMode, ref aiError, asUserRoot, asUserCurrentDirectory, ref aiTimeOutUser, (asCmd.Equals("NLST"))) == true)
            {
                SendAnswer(aoMySocket, "226 Transfer complete.", ref aiTimeOutUser);
            }
            else
            {
                ShowError(aoMySocket, ref aiTimeOutUser, asParameter, aiError);
            }
        }

        /*
         * <summary>RETR Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="sender">BackGroundWorker</param>
         * <param name="e">e.Argument is ClassGetFile</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function,
         * - 15/09/2008 : add aiBlockSize, aiWaitingTime parameters,
         * - 29/09/2008 : pass to thread function,
         * <history>
         */
        //private void SendFileCommand(String asParameter, bool abDownload, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, TcpListener aoClientDataListener, Socket aoClientDataSocket, bool abPassiveMode, long alResumeIndex, ref int aiTimeOutUser, bool abBinaryMode, int aiWaitingTime, int aiBlockSize)
        private void SendFileBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            /* Mapping */
            ClassSendFile loSendFile = (ClassSendFile)e.Argument;
            String asParameter = loSendFile.gsParameter;
            bool abDownload = loSendFile.gbBinaryMode;
            String asUserRoot = loSendFile.gsUserRoot;
            String asUserCurrentDirectory = loSendFile.gsUserCurrentDirectory;
            Socket aoMySocket = loSendFile.goMySocket;
            TcpListener aoClientDataListener = loSendFile.goClientDataListener;
            Socket aoClientDataSocket = loSendFile.goClientDataSocket;
            bool abPassiveMode = loSendFile.gbPassiveMode;
            long alResumeIndex = loSendFile.glResumeIndex;
            int aiTimeOutUser = loSendFile.giTimeOutUser;
            bool abBinaryMode = loSendFile.gbBinaryMode;
            int aiWaitingTime = loSendFile.giWaitingTime;
            int aiBlockSize = loSendFile.giBlockSize;
            /* BackgroundWorker */
            BackgroundWorker loBackgroundWorker = (BackgroundWorker)sender;
            /* File */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0 ;
            /* Tcp client for passive mode */
            TcpClient loTmpClient = null ;
            /* stream to read binary file */
            FileStream loInFileReadBinary;
            /* stream to read text */
            StreamReader loInFileReadText;

            if (abDownload == true)
            {
                try
                {
                    lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

                    if (File.Exists(lsLocalFileName) == true)
                    {
                        SendAnswer(aoMySocket, String.Concat("150 Opening ", (abBinaryMode == true ? "binary" : "ASCII"), " mode data connection for ", asParameter, "."), ref aiTimeOutUser);

                        if (abBinaryMode == true)
                        {
                            loInFileReadBinary = new FileStream(lsLocalFileName, FileMode.Open, FileAccess.Read);

                            if (SendBinaryFile(loBackgroundWorker, loInFileReadBinary, aoClientDataListener, aoClientDataSocket, abPassiveMode, ref liError, alResumeIndex, aiWaitingTime, aiBlockSize, ref aiTimeOutUser) == true)
                            {
                                if (loBackgroundWorker.CancellationPending == false)
                                {
                                    SendAnswer(aoMySocket, MSG_TRANSFERT_COMPLET, ref aiTimeOutUser);
                                }
                            }
                            else
                            {
                                ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                            }

                            loInFileReadBinary.Close();
                        }
                        else
                        {
                            loInFileReadText = new StreamReader(lsLocalFileName);

                            if (SendTextFile(loBackgroundWorker, loInFileReadText, aoClientDataListener, aoClientDataSocket, abPassiveMode, ref liError, aiWaitingTime, aiBlockSize, ref aiTimeOutUser) == true)
                            {
                                if (loBackgroundWorker.CancellationPending == false)
                                {
                                    SendAnswer(aoMySocket, MSG_TRANSFERT_COMPLET, ref aiTimeOutUser);
                                }
                            }
                            else
                            {
                                ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                            }

                            loInFileReadText.Close();
                        }
                    }
                    else
                    {
                        SendAnswer(aoMySocket, String.Format(MSG_FILE_NOT_FOUND, asParameter), ref aiTimeOutUser);
                    }
                }
                catch
                {
                    if (abPassiveMode == true)
                    {
                        loTmpClient = aoClientDataListener.AcceptTcpClient();
                        loTmpClient.Client.Close();
                    }
                    else
                    {
                        aoClientDataSocket.Close();
                    }

                    SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
                }
            }
            else
            {
                SendAnswer(aoMySocket, "500 Cannot RETR.", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>CDUP/XCUP Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asCmd">Command (CDUP/XCUP)</param>
         * <param name="abSubDir">True if user can be access to sub dir</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="asCurrentWorkDir">Current work directory</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void ChangeDirectoryCommand(String asCmd, bool abSubDir, String asUserRoot, ref String asUserCurrentDirectory, ref String asCurrentWorkDir, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* Directory */
            String lsDirectory = String.Empty;

            if (abSubDir == true)
            {
                if ((asCmd.Equals("CDUP") == true) || (asCmd.Equals("XCUP") == true))
                {
                    lsDirectory = AddDirSeparatorAtEnd(asUserCurrentDirectory, '/') + "..";
                }

                switch (ChangeDirectory(asUserRoot, lsDirectory, ref asUserCurrentDirectory, ref asCurrentWorkDir))
                {
                    case 0:
                        SendAnswer(aoMySocket, "250 CWD command successful.", ref aiTimeOutUser);
                        break;
                    case 1:
                        SendAnswer(aoMySocket, "550 Permission Denied.", ref aiTimeOutUser);
                        break;
                    default:
                        SendAnswer(aoMySocket, String.Concat("550 No such file or directory."), ref aiTimeOutUser);
                        break;
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
            }
        }

        /*
         * <summary>STOR/APPE Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="sender">BackGroundWorker</param>
         * <param name="e">e.Argument is ClassGetFile</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function,
         * - 15/09/2008 : add aiBlockSize, aiWaitingTime parameters,
         * - 29/09/2008 : pass to thread function,
         * <history>
         */
        private void GetFileBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //private void GetFileCommand(String asCmd, String asParameter, bool abUpload, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, TcpListener aoClientDataListener, Socket aoClientDataSocket, bool abPassiveMode, long alResumeIndex, ref int aiTimeOutUser, bool abBinaryMode, int aiWaitingTime, int aiBlockSize)
        {
            /* Mapping */
            ClassGetFile loGetFile = (ClassGetFile)e.Argument;
            bool lbUpload = loGetFile.gbUpload;
            String lsCmd = loGetFile.gsCmd;
            String lsParameter = loGetFile.gsParameter;
            Socket loMySocket = loGetFile.goMySocket;
            String lsUserRoot = loGetFile.gsUserRoot ;
            String lsUserCurrentDirectory = loGetFile.gsUserCurrentDirectory;
            TcpListener aoClientDataListener = loGetFile.goClientDataListener;
            Socket loClientDataSocket = loGetFile.goClientDataSocket;
            bool lbPassiveMode = loGetFile.gbPassiveMode;
            long llResumeIndex = loGetFile.glResumeIndex;
            int liTimeOutUser = loGetFile.giTimeOutUser;
            bool lbBinaryMode = loGetFile.gbBinaryMode;
            int liWaitingTime = loGetFile.giWaitingTime;
            /* BackgroundWorker */
            BackgroundWorker loBackgroundWorker = (BackgroundWorker)sender;
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;
            /* file to store on server */
            FileStream loOutFileWriteBinary;
            /* Temporary client Socket */
            TcpClient loTmpClient;
            /* file information to ensure file a good size */
            FileInfo loFi;
            /* Count number to try a random file */
            int liFileNumberOut = 0 ;
            /* Error to get random file name */
            bool lbErrorInRandomFile = false ;

            if (lbUpload == true)
            {
                try
                {
                    if (lsCmd.Equals("STOU") == true)
                    {
                        do
                        {
                            lsParameter = System.IO.Path.GetRandomFileName();
                            liFileNumberOut++;
                        }
                        while ((System.IO.File.Exists(lsParameter) == false) || (liFileNumberOut > 100));

                        if (liFileNumberOut > 100)
                        {
                            lbErrorInRandomFile = true ;

                            SendAnswer(loMySocket, "501 Cannot create unique file name. Try again.", ref liTimeOutUser);
                        }
                    }

                    if (lbErrorInRandomFile == false)
                    {
                        lsLocalFileName = FTPPathToOSPath(lsUserRoot, lsUserCurrentDirectory, lsParameter);

                        if (lsCmd.Equals("STOU") == true)
                        {
                            SendAnswer(loMySocket, String.Concat("150 FILE: ", lsParameter), ref liTimeOutUser);
                        }
                        else
                        {
                            SendAnswer(loMySocket, String.Concat("150 Opening ", (lbBinaryMode == true ? "binary" : "ASCII"), " mode data connection for ", lsParameter, "."), ref liTimeOutUser);
                        }

                        if (File.Exists(lsLocalFileName) == true)
                        {
                            if ((lsCmd.Equals("STOR") == true) || (lsCmd.Equals("STOU") == true))
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

                        if (GetFile(loBackgroundWorker, loOutFileWriteBinary, aoClientDataListener, loClientDataSocket, lbPassiveMode, ref liError, llResumeIndex, lbBinaryMode, ref liTimeOutUser) == true)
                        {
                            // Only send success full message if no abort
                            if (loBackgroundWorker.CancellationPending == false)
                            {
                                SendAnswer(loMySocket, MSG_TRANSFERT_COMPLET, ref liTimeOutUser);
                            }
                        }
                        else
                        {
                            ShowError(loMySocket, ref liTimeOutUser, lsParameter, liError);
                        }

                        loOutFileWriteBinary.Close();
                    }
                }
                catch
                {
                    if (lbPassiveMode == true)
                    {
                        loTmpClient = aoClientDataListener.AcceptTcpClient();
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
        }

        /*
         * <summary>MDTM Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void ReadFileTimeCommand(String asParameter, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Result of MDTM command to be send at client */ 
            String lsResult = String.Empty;
            /* Error */
            int liError = 0 ;

            lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

            if (CommandMDTM(lsLocalFileName, ref lsResult, ref liError) == true)
            {
                SendAnswer(aoMySocket, String.Concat("213 ", lsResult), ref aiTimeOutUser);
            }
            else
            {
                ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
            }
        }

        /*
         * <summary>MDTM Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void SizeFileCommand(String asParameter, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;
            /* Size of file */
            String lsSize = String.Empty;

            lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

            if (CommandSIZE(lsLocalFileName, ref lsSize, ref liError) == true)
            {
                SendAnswer(aoMySocket, String.Concat("213 ", lsSize), ref aiTimeOutUser);
            }
            else
            {
                ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
            }
        }

        /*
         * <summary>UTF8 Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="abUtf8Mode">return utf8 mode</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 03/09/2008 : create function
         * <history>
         */
        private void UTF8Command(String asParameter, ref bool abUtf8Mode, Socket aoMySocket, ref int aiTimeOutUser)
        {
            if (asParameter.Equals("on", StringComparison.OrdinalIgnoreCase) == true)
            {
                abUtf8Mode = true;
                SendAnswer(aoMySocket, "200 UTF8 mode enabled", ref aiTimeOutUser);
            }
            else if (asParameter.Equals("off", StringComparison.OrdinalIgnoreCase) == true)
            {
                abUtf8Mode = false;
                SendAnswer(aoMySocket, "200 UTF8 mode disabled", ref aiTimeOutUser);
            }
            else
            {
                SendAnswer(aoMySocket, "501 Invalid UFT8 options", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>MFMT Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="asFileName">File name to modify time</param>
         * <param name="abModifyTime">True if user can modify file time</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void ModifyFileTimeCommand(String asParameter, String asFileName, bool abModifyTime, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* Modify time */
            String lsModifyTimeOfFile = String.Empty;
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;

            if (abModifyTime == true)
            {
                ExplodeCommand(asParameter, ref lsModifyTimeOfFile, ref asFileName);

                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asFileName);

                if (CommandMFMT(lsLocalFileName, lsModifyTimeOfFile, ref liError) == true)
                {
                    SendAnswer(aoMySocket, String.Concat("213 modify=", lsModifyTimeOfFile, "; ", asFileName), ref aiTimeOutUser);
                }
                else
                {
                    ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                }

            }
            else
            {
                SendAnswer(aoMySocket, "500 Cannot MFMT.", ref aiTimeOutUser);
            }
        }

        /*
         * <summary>FEAT Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void FeatCommand(Socket aoMySocket, ref int aiTimeOutUser)
        {
            SendAnswer(aoMySocket, "211-Features:" , ref aiTimeOutUser);
            SendAnswer(aoMySocket, " MDTM", ref aiTimeOutUser);
            SendAnswer(aoMySocket, " SIZE", ref aiTimeOutUser);
            SendAnswer(aoMySocket, " UTF8 ON|OFF", ref aiTimeOutUser);
            SendAnswer(aoMySocket, " CLNT", ref aiTimeOutUser);
            SendAnswer(aoMySocket, " MFMT", ref aiTimeOutUser);
            SendAnswer(aoMySocket, "211 End", ref aiTimeOutUser);
        }

        /*
         * <summary>RNFR Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="asRenameFrom">Filename to be rename</param>
         * <param name="abRename">True if user can rename file time</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void RenameFileFromCommand(String asParameter, ref String asRenameFrom, bool abRename, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;

            if (abRename == true)
            {
                asRenameFrom = asParameter;

                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asRenameFrom);

                if ((File.Exists(lsLocalFileName) == true) || (Directory.Exists(lsLocalFileName) == true))
                {
                    SendAnswer(aoMySocket, "350 File exists, ready for destination name." , ref aiTimeOutUser);
                }
                else
                {
                    asRenameFrom = String.Empty;

                    ShowError(aoMySocket, ref aiTimeOutUser, asRenameFrom, FILE_NOT_FOUND);
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED , ref aiTimeOutUser);
            }
        }

        /*
         * <summary>RNFR Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="asRenameFrom">Filename to be rename</param>
         * <param name="abRename">True if user can rename file time</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void RenameFileToCommand(String asParameter, ref String asRenameFrom, bool abRename, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* True if file name is directory */
            bool lbIsDirectory = false;
            /* Error */
            int liError = 0;

            if (abRename == true)
            {
                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asRenameFrom);

                if (RenameFileOrDirectory(lsLocalFileName, FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter), ref liError, ref lbIsDirectory) == true)
                {
                    SendAnswer(aoMySocket, String.Format("250 {0} '{1}' renamed to '{2}'", (lbIsDirectory ? "Directory" : "File"), asRenameFrom, asParameter), ref aiTimeOutUser);
                }
                else
                {
                    ShowError(aoMySocket, ref aiTimeOutUser, asRenameFrom, liError);
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Convert string with \n to string for show message</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asText">String</param>
         * 
         * <returns>New string</returns>
		 *
         * <history>
         * - 09/09/2008 : create function
         * <history>
         */
        private String ConvertStringWithEOL(String asText, String asPrefixe)
        {
            asText = asPrefixe + "-" + asText;

            asText = asText.Replace("\\n", EOL + asPrefixe + "-");

            if (asText.EndsWith(EOL) == false)
            {
                asText += EOL;
            }

            asText += asPrefixe + " ";

            return asText;
        }

        /*
         * <summary>Delete a file</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asFileName">file to remove</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>true if success</returns>
		 *
         * <history>
         * - 28/09/2008 : create function
         * <history>
         */
        private bool RemoveFile(String asFileName, ref int aiError)
        {
            /* Return value */
            bool lbRetour = false;

            if (File.Exists(asFileName) == true)
            {
                try
                {
                    File.Delete(asFileName);

                    aiError = SUCCESS;
                    lbRetour = true;
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

        /*
         * <summary>DELE Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="abDelete">True if user can remove file</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void DeleteFileCommand(String asParameter, bool abDelete, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;

            if (abDelete == true)
            {
                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

                if (RemoveFile(lsLocalFileName, ref liError) == true)
                {
                    SendAnswer(aoMySocket, String.Format("250 File '{0}' deleted.", asParameter), ref aiTimeOutUser);
                }
                else
                {
                    ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Delete a directory</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asDirectoryName">directory to remove</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
		 *
         * <returns>true if success</returns>
		 *
         * <history>
         * - 28/09/2008 : create function
         * <history>
         */
        private bool RemoveDirectory(String asDirectoryName, ref int aiError)
        {
            /* Return value */
            bool lbRetour = false;

            if (Directory.Exists(asDirectoryName) == true)
            {
                try
                {
                    Directory.Delete(asDirectoryName);

                    aiError = SUCCESS;
                    lbRetour = true;
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

        /*
         * <summary>RMD/XRMD Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="abDeleteDirectory">True if user can remove directory</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void DeleteDirectoryCommand(String asParameter, bool abDeleteDirectory, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;

            if (abDeleteDirectory == true)
            {
                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

                if (RemoveDirectory(lsLocalFileName, ref liError) == true)
                {
                    SendAnswer(aoMySocket, String.Format("250 '{0}': directory deleted.", asParameter), ref aiTimeOutUser);
                }
                else
                {
                    ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Make a directory</summary>
         *
         * <remarks></remarks>
         *
         * <param name="asDirectoryName">directory to create</param>
         * <param name="aiError">0 -> no error, see ShowError() for more detail</param>
         *
         * <returns>true if success</returns>
         *
         * <history>
         * - 28/09/2008 : create function
         * <history>
         */
        private bool CreateDirectory(String asDirectoryName, ref int aiError)
        {
            /* Return value */
            bool lbRetour = false;

            if (Directory.Exists(asDirectoryName) == false)
            {
                try
                {
                    Directory.CreateDirectory(asDirectoryName);

                    aiError = SUCCESS;
                    lbRetour = true;
                }
                catch
                {
                    aiError = ACCES_DENIED;
                }
            }
            else
            {
                aiError = DIRECTORY_ALREADY_EXIST;
            }

            return lbRetour;
        }

        /*
         * <summary>MKD/XMKD Command</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="asParameter">Parameter of command (file name)</param>
         * <param name="abMakeDirectory">True if user can create directory</param>
         * <param name="asUserRoot">Directory root of user</param>
         * <param name="asUserCurrentDirectory">Current directory of user</param>
         * <param name="aoMySocket">Command socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private void MakeDirectoryCommand(String asParameter, bool abMakeDirectory, String asUserRoot, String asUserCurrentDirectory, Socket aoMySocket, ref int aiTimeOutUser)
        {
            /* File name */
            String lsLocalFileName = String.Empty;
            /* Error */
            int liError = 0;

            if (abMakeDirectory == true)
            {
                lsLocalFileName = FTPPathToOSPath(asUserRoot, asUserCurrentDirectory, asParameter);

                if (CreateDirectory(lsLocalFileName, ref liError) == true)
                {
                    SendAnswer(aoMySocket, String.Format("257 '{0}': directory created.", asParameter), ref aiTimeOutUser);
                }
                else
                {
                    ShowError(aoMySocket, ref aiTimeOutUser, asParameter, liError);
                }
            }
            else
            {
                SendAnswer(aoMySocket, ERROR_PERMISSION_DENIED, ref aiTimeOutUser);
            }
        }

        /*
         * <summary>Return passive connection or null</summary>
		 *
         * <remarks></remarks>
		 *
         * <param name="aoClientDataListener">passive connection socket</param>
		 * <param name="aiTimeOutUser">Time out</param>
         * 
         * <returns>void</returns>
		 *
         * <history>
         * - 04/09/2008 : create function
         * <history>
         */
        private Socket GetPassiveConnection(TcpListener aoClientDataListener, ref int aiTimeOutUser)
        {
            Socket loMySocket = null;
            /* Tcp Client for passive transfert */
            TcpClient loClient;
            aiTimeOutUser = 5;
            do
            {
                if (aoClientDataListener.Pending())
                {
                    loClient = aoClientDataListener.AcceptTcpClient();
                    loMySocket = loClient.Client;
                }
                else
                {
                    Thread.Sleep(WAITING_TIME);
                    aiTimeOutUser--;
                }
            }
            while ((aiTimeOutUser > 0) && (loMySocket == null));

            return loMySocket;
        }
    }   
}
