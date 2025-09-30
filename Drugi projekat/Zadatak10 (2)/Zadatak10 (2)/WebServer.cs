using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class WebServer
    {
        private readonly HttpListener httpListener = new HttpListener();
        private readonly RequestRouter requestRouter;
        private readonly SemaphoreSlim concurrencyLimiter = new SemaphoreSlim(100);

        public WebServer(string[] prefixes, RequestRouter router)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("Bar jedan prefix je obavezan.");

            for (int i = 0; i < prefixes.Length; i++)
            {
                httpListener.Prefixes.Add(prefixes[i]);
            }

            if (router == null) throw new ArgumentNullException(nameof(router));
            requestRouter = router;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            httpListener.Start();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    HttpListenerContext context;

                    try
                    {
                        context = await httpListener.GetContextAsync().ConfigureAwait(false);
                    }
                    catch (HttpListenerException)
                    {
                        break; // stop ili zatvaranje
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    Task processingTask = HandleOneAsync(context, cancellationToken);
                }
            }
            finally
            {
                if (httpListener.IsListening)
                {
                    httpListener.Stop();
                }

                httpListener.Close();
            }
        }

        private async Task HandleOneAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            await concurrencyLimiter.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await requestRouter.HandleAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                try
                {
                    await Respond.TextAsync(context, "Internal Server Error: " + ex.Message, 500, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // ignorisi gresku pri slanju odgovora
                }
            }
            finally
            {
                concurrencyLimiter.Release();
            }
        }
    }
}
