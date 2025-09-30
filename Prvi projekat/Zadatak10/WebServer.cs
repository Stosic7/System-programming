using System;
using System.Net;
using System.Threading;

namespace Zadatak10
{
    public sealed class WebServer
    {
        private readonly HttpListener httpListener = new HttpListener();
        private readonly RequestRouter requestRouter;
        private volatile bool isRunning;

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

        public void Start()
        {
            httpListener.Start();
            isRunning = true;

            while (isRunning)
            {
                HttpListenerContext? context = null;

                try
                {
                    // blokira dok ne stigne zahtev
                    context = httpListener.GetContext();
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

                if (context == null)
                {
                    continue;
                }

                Thread worker = new Thread(ProcessContext);
                worker.IsBackground = true;
                worker.Start(context);
            }
        }

        private void ProcessContext(object? state)
        {
            HttpListenerContext? context = state as HttpListenerContext;
            if (context == null) return;

            try
            {
                requestRouter.Handle(context);
            }
            catch (Exception exception)
            {
                try
                {
                    Respond.Text(context, "Internal Server Error: " + exception.Message, 500);
                }
                catch
                {
                    // ignorisi sve izuzete prilikom slanja odgovora
                }
            }
        }

        public void Stop()
        {
            isRunning = false;

            if (httpListener.IsListening)
            {
                httpListener.Stop();
            }

            httpListener.Close();
            Console.WriteLine("Server zaustavljen.");
        }
    }
}
