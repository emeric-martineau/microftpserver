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
        private static String psLogFileName = "log-%Y%M%d.log";
        private static long plLogFileSize = 0;
        private static bool pbEraseLogFile = false ;

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

        static void LogFile(String asText)
        {
            System.IO.StreamWriter logFile;
            System.IO.FileInfo fi;
            String lsLogFileName = psLogFileName;

            //yyyyMMddHHmmss
            lsLogFileName = lsLogFileName.Replace("%Y", DateTime.Now.ToString("yyyy"));
            lsLogFileName = lsLogFileName.Replace("%M", DateTime.Now.ToString("MM"));
            lsLogFileName = lsLogFileName.Replace("%d", DateTime.Now.ToString("dd"));
            lsLogFileName = lsLogFileName.Replace("%H", DateTime.Now.ToString("HH"));
            lsLogFileName = lsLogFileName.Replace("%m", DateTime.Now.ToString("mm"));
            lsLogFileName = lsLogFileName.Replace("%s", DateTime.Now.ToString("ss"));

            if ((plLogFileSize > 0) && (pbEraseLogFile == false))
            {
                lsLogFileName = GetLastLogFile(lsLogFileName);
            }

            try
            {
                try
                {
                    fi = new System.IO.FileInfo(lsLogFileName);

                    if (fi.Length > plLogFileSize)
                    {
                        if (pbEraseLogFile == false)
                        {
                            lsLogFileName = GetNextFile(lsLogFileName);
                        }
                        else
                        {
                            System.IO.File.Delete(lsLogFileName);
                        }
                    }
                }
                catch
                {
                    if ((plLogFileSize > 0) && (pbEraseLogFile == false))
                    {
                        lsLogFileName = GetNextFile(lsLogFileName);
                    }
                }

                logFile = new System.IO.StreamWriter(lsLogFileName, true);

                logFile.WriteLine(asText);

                logFile.Close();
            }
            catch//(Exception e)
            {
                Console.WriteLine("Cannot write to log file ('" + lsLogFileName + "').");
                //Console.WriteLine(e.ToString());
            }
        }

        static String AddFinalDirSeparator(String asDir)
        {
            if (asDir.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
            {
                asDir = String.Concat(asDir, System.IO.Path.DirectorySeparatorChar);
            }

            return asDir;
        }

        static String GetLastLogFile(String asFileName)
        {
            /* List of file in directory */
            String[] laListOfFile;
            /* File name of log without extension */
            String lsFileName = System.IO.Path.GetFileNameWithoutExtension(asFileName);
            /* Base name of file */
            String lsBaseFileName = String.Empty;
            /* Extension of file */
            String lsExt = System.IO.Path.GetExtension(asFileName);
            /* Last log file name */
            String lsLastLogFile ;
            /* Path of log */
            String lsPath = System.IO.Path.GetDirectoryName(asFileName) ;
            /* Array to sort list of file */
            List<String> loNewListOfFile = new List<string>();
            /* Loop counter */
            int liIndexNewListOfFile;
            /* Loop counter */
            int liIndexListOfFile;
            /* Number of file */
            int liNumberOfFile;
            /* True if file is insered in loNewListOfFile */
            bool lbInsered;

            if (lsPath.Equals(String.Empty))
            {
                lsPath = System.IO.Directory.GetCurrentDirectory();
            }

            lsLastLogFile = AddFinalDirSeparator(lsPath) + lsFileName + "-1" + lsExt;

            laListOfFile = System.IO.Directory.GetFiles(lsPath, lsFileName + "-*" + lsExt);

            if (laListOfFile.Length > 0)
            {
                /* Sort file */
                for (liIndexListOfFile = 0; liIndexListOfFile < laListOfFile.Length; liIndexListOfFile++)
                {
                    liNumberOfFile = GetNumber(System.IO.Path.GetFileNameWithoutExtension(laListOfFile[liIndexListOfFile]));

                    if (liNumberOfFile > 0)
                    {
                        lbInsered = false;

                        for (liIndexNewListOfFile = 0; liIndexNewListOfFile < loNewListOfFile.Count; liIndexNewListOfFile++)
                        {
                            if (liNumberOfFile < GetNumber(System.IO.Path.GetFileNameWithoutExtension(loNewListOfFile[liIndexNewListOfFile])))
                            {
                                loNewListOfFile.Insert(liIndexNewListOfFile, laListOfFile[liIndexListOfFile]);
                                break;
                            }
                        }

                        if (lbInsered == false)
                        {
                            loNewListOfFile.Add(laListOfFile[liIndexListOfFile]);
                        }
                    }
                }

                lsLastLogFile = loNewListOfFile[loNewListOfFile.Count - 1];
            }

            return lsLastLogFile;
        }

        static String GetNextFile(String asLogFileName)
        {
            /* List of file in directory */
            String[] laListOfFile ;
            /* File name of log without extension */
            String lsFileName = System.IO.Path.GetFileNameWithoutExtension(asLogFileName);
            /* Loop count */
            int liIndex;
            /* Maximum value of number of file */
            int liMaxValue = 0 ;
            /* Temporary integer */
            int liTmp ;
            /* Base name of file */
            String lsBaseFileName = String.Empty;
            /* Extension of file */
            String lsExt = System.IO.Path.GetExtension(asLogFileName);
            /* Path of file */
            String lsPath = System.IO.Path.GetDirectoryName(asLogFileName);

            lsBaseFileName = lsFileName.Substring(0, PosOfSeparator(lsFileName));

            laListOfFile = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(asLogFileName), lsBaseFileName + "-*" + lsExt);

            for(liIndex = 0; liIndex < laListOfFile.Length; liIndex++)
            {
                laListOfFile[liIndex] = System.IO.Path.GetFileNameWithoutExtension(laListOfFile[liIndex]) ;

                liTmp = GetNumber(laListOfFile[liIndex]);

                if (liTmp > liMaxValue)
                {
                        liMaxValue = liTmp;
                }
            }

            return AddFinalDirSeparator(lsPath) + lsBaseFileName + "-" + (liMaxValue + 1) + lsExt;
        }

        static int GetNumber(String asFileName)
        {
            /* Temporary string */
            String lsTmp;
            /* Temporary integer */
            int liTmp = -1 ;

            liTmp = PosOfSeparator(asFileName);

            lsTmp = asFileName.Substring(PosOfSeparator(asFileName) + 1);

            int.TryParse(lsTmp, out liTmp);

            return liTmp ;
        }

        static int PosOfSeparator(String lsFileName)
        {
            /* Loop count */
            int liIndex;
            /* Position of - */
            int liPos = 1;
            char lcChar;

            for (liIndex = lsFileName.Length - 1; liIndex >= 0; liIndex--)
            {
                lcChar = lsFileName[liIndex] ;

                if (lcChar.Equals('-'))
                {
                    liPos = liIndex;
                    break;
                }
            }

            return liPos;
        }

        static void Main(string[] aaArgs)
        {
            int liIndex;
            double ldMultiple = 0;
            String lsRootFile ;
            ClassFTPServer loServer;
            bool lbLogFile = false;

            Console.WriteLine("MicroFTPServer v0.6") ;
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
                    else if (aaArgs[liIndex].ToLower() == "-logfile")
                    {
                        liIndex++;
                        psLogFileName = aaArgs[liIndex];
                        lbLogFile = true;
                    }
                    else if (aaArgs[liIndex].ToLower() == "-logsize")
                    {
                        liIndex++;

                        if (aaArgs[liIndex].EndsWith("k", StringComparison.OrdinalIgnoreCase))
                        {
                            ldMultiple = 2;
                        }
                        else if (aaArgs[liIndex].EndsWith("M", StringComparison.OrdinalIgnoreCase))
                        {
                            ldMultiple = 3;
                        }
                        else if (aaArgs[liIndex].EndsWith("G", StringComparison.OrdinalIgnoreCase))
                        {
                            ldMultiple = 4;
                        }
                        else if (aaArgs[liIndex].EndsWith("T", StringComparison.OrdinalIgnoreCase))
                        {
                            ldMultiple = 5;
                        }

                        if (ldMultiple > 0)
                        {
                            aaArgs[liIndex] = aaArgs[liIndex].Substring(0, aaArgs[liIndex].Length - 1);
                            ldMultiple = Math.Pow(1024, ldMultiple);
                        }
                        else
                        {
                            ldMultiple = 1;
                        }

                        if (long.TryParse(aaArgs[liIndex], out plLogFileSize) == false)
                        {
                            Console.WriteLine(aaArgs[liIndex] + " is not valid number, use " + plLogFileSize + " by default.");
                        }
                        else
                        {
                            plLogFileSize = plLogFileSize * (long)ldMultiple;
                        }
                    }
                    else if (aaArgs[liIndex].ToLower() == "-logerase")
                    {
                        pbEraseLogFile = true;
                    }
                }
            }

            loServer = new ClassFTPServer(lsRootFile);

            if (lbLogFile)
            {
                loServer.OnLog = LogFile;
            }
            else
            {
                loServer.OnLog = Log;
            }

            loServer.Start();

            Log("Server terminate");
        }
    }
}
