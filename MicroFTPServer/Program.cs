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

            Console.WriteLine("MicroFTPServer v0.2");

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
