using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak17
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            var http = HttpClientFactory.Shared; // singleton
            var api  = new TheCocktailDbClient(http);
            var ta   = new TextAnalytics(StopWords.Default);
            var ui   = new Renderer();

            var app  = new CocktailSearchApp(api, ta, ui);

            ui.Banner();
            await app.RunInteractiveAsync(cts.Token);
            ui.Goodbye();
        }
    }
}
