using System;

namespace Zadatak10
{
    internal static class Program
    {
        private static WebServer? runningServer;

        private static void Main(string[] args)
        {
            string originalRootPath = AppDomain.CurrentDomain.BaseDirectory;

            FileService fileService = new FileService(originalRootPath);
            RequestRouter requestRouter = new RequestRouter(fileService);

            string[] serverPrefixes = new string[]
            {
                "http://localhost:5050/",
                "http://127.0.0.1:5050/"
            };

            runningServer = new WebServer(serverPrefixes, requestRouter);

            Console.WriteLine("Server startuje na http://localhost:5050/");
            Console.WriteLine("Root: " + originalRootPath);
            Console.WriteLine("Pritisni Ctrl+C za stop.");

            Console.CancelKeyPress += OnCancelKeyPress;

            // blokira nit
            runningServer.Start();
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            if (runningServer != null)
            {
                runningServer.Stop();
            }
        }
    }
}
