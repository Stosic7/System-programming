using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class RequestRouter
    {
        private readonly FileService fileService;

        public RequestRouter(FileService fileService)
        {
            this.fileService = fileService;
        }

        public async Task HandleAsync(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            if (httpContext.Request.HttpMethod != "GET")
            {
                await Respond.TextAsync(httpContext, "Method Not Allowed", 405, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (httpContext.Request.Url == null)
            {
                await Respond.TextAsync(httpContext, "Neispravan zahtev.", 400, cancellationToken).ConfigureAwait(false);
                return;
            }

            string rawPath = httpContext.Request.Url.AbsolutePath;
            string requestedName = WebUtility.UrlDecode(rawPath).TrimStart('/');

            if (string.IsNullOrWhiteSpace(requestedName))
            {
                await Respond.TextAsync(httpContext, "Prosledi naziv fajla: /ime.gif", 400, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (requestedName.Contains("..") || requestedName.Contains(":") || requestedName.StartsWith("/"))
            {
                await Respond.TextAsync(httpContext, "Zabranjena putanja.", 400, cancellationToken).ConfigureAwait(false);
                return;
            }

            string? fullPath = await fileService.FindAsync(requestedName, cancellationToken).ConfigureAwait(false);
            if (fullPath == null)
            {
                await Respond.TextAsync(httpContext, "Fajl '" + requestedName + "' nije pronadjen.", 404, cancellationToken).ConfigureAwait(false);
                return;
            }

            await Respond.FileAsync(httpContext, fullPath, cancellationToken).ConfigureAwait(false);
        }
    }
}
