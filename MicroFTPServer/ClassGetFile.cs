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
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    /*
     * <summary>STOR/APPE Command</summary>
	 *
     * <remarks></remarks>
	 *
     * <param name="asCmd">Command STOR/APPE</param>
     * <param name="asParameter">Parameter of command (file name)</param>
     * <param name="abUpload">True if user can be download</param>
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
     * <param name="aiBlockSize">Size of block to end</param>
     */
    // STOR/APPE Command
    class ClassGetFile
    {
        // Command STOR/APPE
        public String gsCmd;
        // Parameter of command (file name)
        public String gsParameter;
        // True if user can be download
        public bool gbUpload;
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
    }
}
