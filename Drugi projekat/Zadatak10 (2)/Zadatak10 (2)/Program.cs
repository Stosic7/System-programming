using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    internal static class Program
    {
        private static CancellationTokenSource? appCancellation;
        private static WebServer? runningServer;

        public static async Task Main(string[] args)
        {
            appCancellation = new CancellationTokenSource();

            Console.CancelKeyPress += OnCancelKeyPress;

            string originalRootPath = AppDomain.CurrentDomain.BaseDirectory;

            FileService fileService = new FileService(originalRootPath);
            RequestRouter requestRouter = new RequestRouter(fileService);
            string[] prefixes = new string[] { "http://localhost:5050/", "http://127.0.0.1:5050/" };

            runningServer = new WebServer(prefixes, requestRouter);

            Console.WriteLine("Drugi projekat – async varijanta");
            Console.WriteLine("URL: http://localhost:5050/");
            Console.WriteLine("Root: " + originalRootPath);

            await runningServer.StartAsync(appCancellation.Token);

            Console.WriteLine("Server ugašen.");
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            CancellationTokenSource? localCts = appCancellation;
            if (localCts != null && !localCts.IsCancellationRequested)
            {
                localCts.Cancel();
            }
        }
    }
}
