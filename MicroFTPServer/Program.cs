using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Log(String Text)
        {
            String CurrentDateTime;
            System.Security.Principal.IPrincipal MyThread = Thread.CurrentPrincipal;

            CurrentDateTime = String.Format("{0:d2}/{1:d2}/{2:d4} {3:d2}:{4:d2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute);

            lock (MyThread)
            {
                Console.WriteLine(CurrentDateTime + " - " + Text.TrimEnd());
            }
        }

        static void Main(string[] args)
        {
            int i;
            String RootFile ;
            ClassFTPServer server;

            Console.WriteLine("MicroFTPServer v0.1");

            RootFile = System.IO.Directory.GetCurrentDirectory();

            if (args.Length > 1)
            {
                for (i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower() == "-root")
                    {
                        i++;
                        RootFile = args[i];
                    }
                }
            }

            server = new ClassFTPServer(RootFile);
            server.OnLog = Log;
            server.Start();

            Log("Server terminate");
        }
    }
}
