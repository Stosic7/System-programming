using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            string root = AppDomain.CurrentDomain.BaseDirectory;
            var router = new RequestRouter(new FileService(root));
            var server = new WebServer(
                new[] { "http://localhost:5050/", "http://127.0.0.1:5050/" },
                router
            );

            Console.WriteLine("Drugi projekat – async varijanta");
            Console.WriteLine("URL: http://localhost:5050/");
            Console.WriteLine("Root: " + root);

            await server.StartAsync(cts.Token);
            Console.WriteLine("Server ugašen.");
        }
    }
}
