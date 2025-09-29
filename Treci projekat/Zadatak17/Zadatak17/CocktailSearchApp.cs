using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak17
{
    public sealed class CocktailSearchApp
    {
        private readonly TheCocktailDbClient _api;
        private readonly TextAnalytics _ta;
        private readonly Renderer _ui;

        public CocktailSearchApp(TheCocktailDbClient api, TextAnalytics ta, Renderer ui)
        {
            _api = api; _ta = ta; _ui = ui;
        }

        public async Task RunInteractiveAsync(CancellationToken ct)
        {
            await Task.Yield();

            var input = new Subject<string>();

            var drinksStream =
                input
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Throttle(TimeSpan.FromMilliseconds(200))
                .DistinctUntilChanged(StringComparer.OrdinalIgnoreCase)
                .SelectMany(s =>
                    Observable.FromAsync(ct2 => _api.SearchByNameAsync(s, ct2))
                              .Select(drinks => (s, drinks))
                              .Catch<(string, IReadOnlyList<Drink>), Exception>(_ =>
                                  Observable.Empty<(string, IReadOnlyList<Drink>)>())
                );

            var sub = drinksStream.Subscribe(pair =>
            {
                var query  = pair.Item1;
                var drinks = pair.Item2;

                _ui.ShowDrinks(query, drinks);

                var tokens = drinks
                    .Where(d => !string.IsNullOrWhiteSpace(d.strInstructions))
                    .SelectMany(d => _ta.Tokens(d.strInstructions!));

                var top = tokens
                    .GroupBy(w => w)
                    .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                    .OrderByDescending(kv => kv.Value);
                    
                _ui.ShowTopWords(top, 10);
            },
            ex => _ui.Error(ex.Message));

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    _ui.Prompt();
                    var line = Console.ReadLine();
                    if (line is null) break;
                    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                    input.OnNext(line);
                }
            }
            finally
            {
                input.OnCompleted();
                sub.Dispose();
            }
        }
    }
}
