using System;
using System.Net;
using System.Threading;

namespace Zadatak10
{
    public sealed class WebServer
    {
        private readonly HttpListener _http = new HttpListener();
        private readonly RequestRouter _router;
        private volatile bool _running;

        public WebServer(string[] prefixes, RequestRouter router)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("Bar jedan prefix je obavezan.");

            foreach (var p in prefixes) _http.Prefixes.Add(p);
            _router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public void Start()
        {
            _http.Start();
            _running = true;

            while (_running)
            {
                HttpListenerContext? ctx = null;
                try
                {
                    // blokira dok ne stigne zahtev
                    ctx = _http.GetContext();
                }
                catch (HttpListenerException)
                {
                    // nastaje prilikom Stop(); prekida blokiranje
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                if (ctx == null) continue;

                // klasicna nit
                var t = new Thread(() =>
                {
                    try { _router.Handle(ctx); }
                    catch (Exception ex)
                    {
                        try { Respond.Text(ctx, "Internal Server Error: " + ex.Message, 500); }
                        catch { /* ignore */ }
                    }
                });
                t.IsBackground = true;
                t.Start();
            }
        }

        public void Stop()
        {
            _running = false;
            if (_http.IsListening) _http.Stop();
            _http.Close();
            Console.WriteLine("Server zaustavljen.");
        }
    }
}
