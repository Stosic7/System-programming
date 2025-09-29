using System;
using System.Threading;

namespace Zadatak10
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;

            var router = new RequestRouter(new FileService(root));
            var server = new WebServer(
                new[] { "http://localhost:5050/", "http://127.0.0.1:5050/" },
                router
            );

            Console.WriteLine("Server startuje na http://localhost:5050/");
            Console.WriteLine("Root: " + root);
            Console.WriteLine("Pritisni Ctrl+C za stop.");

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                server.Stop();
            };

            server.Start(); // blokira nit
        }
    }
}
