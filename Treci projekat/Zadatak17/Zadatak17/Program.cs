using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak17
{
    internal static class Program
    {
        private static CancellationTokenSource? appCancellation;

        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            appCancellation = new CancellationTokenSource();
            Console.CancelKeyPress += OnCancelKeyPress;

            HttpClient httpClient = HttpClientFactory.Shared;
            TheCocktailDbClient cocktailApi = new TheCocktailDbClient(httpClient);
            TextAnalytics textAnalytics = new TextAnalytics(StopWords.Default);
            Renderer userInterface = new Renderer();
            CocktailSearchApp app = new CocktailSearchApp(cocktailApi, textAnalytics, userInterface);

            userInterface.Banner();
            await app.RunInteractiveAsync(appCancellation.Token);
            userInterface.Goodbye();

            appCancellation.Dispose();
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            if (appCancellation != null && !appCancellation.IsCancellationRequested)
            {
                appCancellation.Cancel();
            }
        }
    }
}
