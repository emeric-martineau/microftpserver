using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    /*
     * <summary>RETR Command</summary>
	 *
     * <remarks></remarks>
	 *
     * <param name="asParameter">Parameter of command (file name)</param>
     * <param name="abDownload">True if user can be download</param>
     * <param name="asUserRoot">Directory root of user</param>
     * <param name="asUserCurrentDirectory">Current directory of user</param>
     * <param name="aoMySocket">Command socket</param>
     * <param name="aoClientDataListener">Passive client listener</param>
     * <param name="aoClientDataSocket">Socket client</param>
     * <param name="abPassiveMode">True if client is in passive mode</param>
     * <param name="alResumeIndex">Start of file</param>
	 * <param name="aiTimeOutUser">Time out</param>
     * <param name="abBinaryMode">True if binary mode</param>
     * <param name="aiWaitingTime">Waiting time between send block</param>
     * <param name="aiBlockSize">Size of block to send</param>
     */ 
    class ClassSendFile
    {
        // Parameter of command (file name)
        public String gsParameter;
        // True if user can be download
        public bool gbDownload;
        // Directory root of user
        public String gsUserRoot;
        // Current directory of user
        public String gsUserCurrentDirectory;
        // Command socket
        public Socket goMySocket;
        // Passive client listener
        public TcpListener goClientDataListener;
        // Socket client
        public Socket goClientDataSocket;
        // True if client is in passive mode
        public bool gbPassiveMode;
        // Start of file
        public long glResumeIndex;
        // Time out
        public int giTimeOutUser;
        // True if binary mode
        public bool gbBinaryMode;
        // Waiting time between send block
        public int giWaitingTime;
        // Size of block to send
        public int giBlockSize;
    }
}
