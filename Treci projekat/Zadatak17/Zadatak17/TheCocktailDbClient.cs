using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak17
{
    public sealed class TheCocktailDbClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public TheCocktailDbClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<Drink>> SearchByNameAsync(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name)) return Array.Empty<Drink>();

            var url = $"https://www.thecocktaildb.com/api/json/v1/1/search.php?s={Uri.EscapeDataString(name)}";

            using var resp = await _http.GetAsync(url, ct).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            var data = await JsonSerializer.DeserializeAsync<CocktailResponse>(stream, _json, ct).ConfigureAwait(false);

            var list = data?.Drinks; // List<Drink>?
            return list is null ? Array.Empty<Drink>() : (IReadOnlyList<Drink>)list;
        }
    }
}
