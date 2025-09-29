using System.Net.Http;

namespace Zadatak17
{
    public static class HttpClientFactory
    {
        public static readonly HttpClient Shared = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
    }
}
