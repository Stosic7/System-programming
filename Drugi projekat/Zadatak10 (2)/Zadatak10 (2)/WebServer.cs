using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class WebServer
    {
        private readonly HttpListener _http = new();
        private readonly RequestRouter _router;
        private readonly SemaphoreSlim _limit = new(100);

        public WebServer(string[] prefixes, RequestRouter router)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("Bar jedan prefix je obavezan.");
            foreach (var p in prefixes) _http.Prefixes.Add(p);
            _router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public async Task StartAsync(CancellationToken ct)
        {
            _http.Start();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    HttpListenerContext ctx;
                    try
                    {
                        ctx = await _http.GetContextAsync().ConfigureAwait(false);
                    }
                    catch (HttpListenerException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    _ = HandleOneAsync(ctx, ct);
                }
            }
            finally
            {
                if (_http.IsListening) _http.Stop();
                _http.Close();
            }
        }

        private async Task HandleOneAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            await _limit.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                await _router.HandleAsync(ctx, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                try { await Respond.TextAsync(ctx, "Internal Server Error: " + ex.Message, 500).ConfigureAwait(false); }
                catch { /* ignore */ }
            }
            finally
            {
                _limit.Release();
            }
        }
    }
}
