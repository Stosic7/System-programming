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
        private readonly TheCocktailDbClient cocktailApi;
        private readonly TextAnalytics textAnalytics;
        private readonly Renderer userInterface;

        public CocktailSearchApp(TheCocktailDbClient api, TextAnalytics ta, Renderer ui)
        {
            cocktailApi = api;
            textAnalytics = ta;
            userInterface = ui;
        }

        public async Task RunInteractiveAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            Subject<string> inputStream = new Subject<string>();

            IObservable<(string query, IReadOnlyList<Drink> drinks)> drinksStream =
                inputStream
                    .Select(TrimInput)
                    .Where(IsNotNullOrWhiteSpace)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .DistinctUntilChanged(StringComparer.OrdinalIgnoreCase)
                    .SelectMany(query =>
                        Observable.FromAsync(ct => cocktailApi.SearchByNameAsync(query, ct))
                                  .Select(drinks => (query, drinks))
                                  // >>> ključna linija: eksplicitni generici + method group
                                  .Catch<(string query, IReadOnlyList<Drink> drinks), Exception>(ReturnEmptyOnError)
                    );

            IDisposable subscription = drinksStream.Subscribe(OnDrinksStreamNext, OnDrinksStreamError);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    userInterface.Prompt();
                    string? line = Console.ReadLine();
                    if (line == null) break;
                    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                    inputStream.OnNext(line);
                }
            }
            finally
            {
                inputStream.OnCompleted();
                subscription.Dispose();
            }
        }

        private static string TrimInput(string s)
        {
            return s == null ? string.Empty : s.Trim();
        }

        private static bool IsNotNullOrWhiteSpace(string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        // >>> potpis sada koristi isti imenovani tuple kao i stream
        private static IObservable<(string query, IReadOnlyList<Drink> drinks)> ReturnEmptyOnError(Exception _)
        {
            return Observable.Empty<(string query, IReadOnlyList<Drink> drinks)>();
        }

        private static int CompareByValueDescending(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
        {
            if (a.Value == b.Value) return 0;
            return a.Value > b.Value ? -1 : 1;
        }

        private void OnDrinksStreamNext((string, IReadOnlyList<Drink>) result)
        {
            string query = result.Item1;
            IReadOnlyList<Drink> drinks = result.Item2;

            userInterface.ShowDrinks(query, drinks);

            List<string> allTokens = new List<string>();
            for (int i = 0; i < drinks.Count; i++)
            {
                string? instructions = drinks[i].strInstructions;
                if (!string.IsNullOrWhiteSpace(instructions))
                {
                    string[] tokensArray = textAnalytics.Tokens(instructions!).ToArray();
                    for (int j = 0; j < tokensArray.Length; j++)
                    {
                        allTokens.Add(tokensArray[j]);
                    }
                }
            }

            Dictionary<string, int> freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int k = 0; k < allTokens.Count; k++)
            {
                string word = allTokens[k];
                int count;
                if (freq.TryGetValue(word, out count))
                {
                    freq[word] = count + 1;
                }
                else
                {
                    freq[word] = 1;
                }
            }

            List<KeyValuePair<string, int>> freqList = new List<KeyValuePair<string, int>>();
            string[] keys = freq.Keys.ToArray();
            for (int x = 0; x < keys.Length; x++)
            {
                string key = keys[x];
                freqList.Add(new KeyValuePair<string, int>(key, freq[key]));
            }
            freqList.Sort(CompareByValueDescending);

            userInterface.ShowTopWords(freqList, 10);
        }

        private void OnDrinksStreamError(Exception ex)
        {
            userInterface.Error(ex.Message);
        }
    }
}
