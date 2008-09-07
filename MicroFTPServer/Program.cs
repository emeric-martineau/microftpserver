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
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Log(String asText)
        {
            String CurrentDateTime;
            System.Security.Principal.IPrincipal MyThread = Thread.CurrentPrincipal;

            CurrentDateTime = String.Format("{0:d2}/{1:d2}/{2:d4} {3:d2}:{4:d2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute);

            lock (MyThread)
            {
                Console.WriteLine(CurrentDateTime + " - " + asText.TrimEnd());
            }
        }

        static void Main(string[] aaArgs)
        {
            int liIndex;
            String lsRootFile ;
            ClassFTPServer loServer;

            Console.WriteLine("MicroFTPServer v0.3") ;
            Console.WriteLine("CopyLeft (C) MARTINEAU Emeric (php4php@free.fr)") ;
            Console.WriteLine("Web site : http://php4php.free.fr/leechdotnet/");
            Console.WriteLine("License : GNU GPL v3");

            lsRootFile = System.IO.Directory.GetCurrentDirectory();

            if (aaArgs.Length > 1)
            {
                for (liIndex = 0; liIndex < aaArgs.Length; liIndex++)
                {
                    if (aaArgs[liIndex].ToLower() == "-root")
                    {
                        liIndex++;
                        lsRootFile = aaArgs[liIndex];
                    }
                }
            }

            loServer = new ClassFTPServer(lsRootFile);
            loServer.OnLog = Log;
            loServer.Start();

            Log("Server terminate");
        }
    }
}
