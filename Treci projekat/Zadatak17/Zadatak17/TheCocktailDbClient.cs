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
        private readonly HttpClient httpClient;
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TheCocktailDbClient(HttpClient http)
        {
            httpClient = http;
        }

        public async Task<IReadOnlyList<Drink>> SearchByNameAsync(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Array.Empty<Drink>();
            }

            string url = "https://www.thecocktaildb.com/api/json/v1/1/search.php?s=" + Uri.EscapeDataString(name);

            HttpResponseMessage resp = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
            try
            {
                resp.EnsureSuccessStatusCode();

                System.IO.Stream stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                try
                {
                    CocktailResponse? data = await JsonSerializer.DeserializeAsync<CocktailResponse>(stream, jsonOptions, ct).ConfigureAwait(false);
                    List<Drink>? list = data?.Drinks;
                    if (list == null)
                    {
                        return Array.Empty<Drink>();
                    }
                    return list;
                }
                finally
                {
                    await stream.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                resp.Dispose();
            }
        }
    }
}
