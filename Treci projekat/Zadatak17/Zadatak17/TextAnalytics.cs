using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace Zadatak17
{
    public sealed class TextAnalytics
    {
        private readonly HashSet<string> stopWordsSet;
        private static readonly Regex tokenizeRegex = new Regex("[A-Za-z]+", RegexOptions.Compiled);

        public TextAnalytics(HashSet<string> stopWords)
        {
            stopWordsSet = stopWords;
        }

        public IEnumerable<string> Tokens(string text)
        {
            MatchCollection matches = tokenizeRegex.Matches(text ?? string.Empty);
            for (int i = 0; i < matches.Count; i++)
            {
                string w = matches[i].Value.ToLowerInvariant();
                if (w.Length > 2 && !stopWordsSet.Contains(w))
                {
                    yield return w;
                }
            }
        }

        private static string Identity(string w)
        {
            return w;
        }

        private static IObservable<KeyValuePair<string, int>> ToCount(IGroupedObservable<string, string> group)
        {
            return group.Count().Select(c => new KeyValuePair<string, int>(group.Key, c));
        }

        public IObservable<KeyValuePair<string, int>> WordCloud(IObservable<string> instructionsStream)
        {
            return instructionsStream
                .SelectMany(Tokens)
                .GroupBy(Identity)
                .SelectMany(ToCount); 
        }
    }
}
