using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class RequestRouter
    {
        private readonly FileService _files;
        public RequestRouter(FileService files) => _files = files;

        public async Task HandleAsync(HttpListenerContext ctx, CancellationToken ct)
        {
            if (ctx.Request.HttpMethod != "GET")
            {
                await Respond.TextAsync(ctx, "Method Not Allowed", 405).ConfigureAwait(false);
                return;
            }

            var raw = ctx.Request.Url!.AbsolutePath;
            var name = WebUtility.UrlDecode(raw).TrimStart('/');

            if (string.IsNullOrWhiteSpace(name))
            {
                await Respond.TextAsync(ctx, "Prosledi naziv fajla: /ime.gif", 400).ConfigureAwait(false);
                return;
            }

            if (name.Contains("..") || name.Contains(":") || name.StartsWith("/"))
            {
                await Respond.TextAsync(ctx, "Zabranjena putanja.", 400).ConfigureAwait(false);
                return;
            }

            var path = await _files.FindAsync(name, ct).ConfigureAwait(false);
            if (path is null)
            {
                await Respond.TextAsync(ctx, $"Fajl '{name}' nije pronadjen.", 404).ConfigureAwait(false);
                return;
            }

            await Respond.FileAsync(ctx, path, ct).ConfigureAwait(false);
        }
    }
}
