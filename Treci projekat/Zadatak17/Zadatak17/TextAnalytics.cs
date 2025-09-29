using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace Zadatak17
{
    public sealed class TextAnalytics
    {
        private readonly HashSet<string> _stop;
        private static readonly Regex _tokenize = new(@"[A-Za-z]+", RegexOptions.Compiled);

        public TextAnalytics(HashSet<string> stopWords) => _stop = stopWords;

        public IEnumerable<string> Tokens(string text)
        {
            foreach (Match m in _tokenize.Matches(text ?? string.Empty))
            {
                var w = m.Value.ToLowerInvariant();
                if (w.Length > 2 && !_stop.Contains(w)) yield return w;
            }
        }

        public IObservable<KeyValuePair<string, int>> WordCloud(IObservable<string> instructionsStream)
        {
            return instructionsStream
                .SelectMany(txt => Tokens(txt))          // stream reci
                .GroupBy(w => w)                         // po reci
                .SelectMany(g => g.Count()               // prebroj u grupi
                    .Select(c => new KeyValuePair<string, int>(g.Key, c)));
        }
    }
}
