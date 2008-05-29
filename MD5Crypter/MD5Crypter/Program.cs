using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ConsoleApplication1
{
    class Program
    {
        public static string EncodePassword(string password)
        {
            byte[] original_bytes = System.Text.Encoding.Default.GetBytes(password);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }
            return result.ToString();
        }

        static void Main(string[] args)
        {
            String mdp;

            Console.WriteLine("Enter passord :") ;
            mdp = Console.ReadLine();

            Console.WriteLine("Password string is :");
            Console.WriteLine(EncodePassword(mdp));

            Console.WriteLine("Hit ENTER to exit.");
            Console.ReadLine();
        }
    }
}
