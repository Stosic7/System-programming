using System;
using System.Net;

namespace Zadatak10
{
    public sealed class RequestRouter
    {
        private readonly FileService _files;

        public RequestRouter(FileService files) => _files = files ?? throw new ArgumentNullException(nameof(files));

        public void Handle(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod != "GET")
            {
                Respond.Text(ctx, "Method Not Allowed", 405);
                return;
            }

            var rawPath = ctx.Request.Url!.AbsolutePath;
            var name = WebUtility.UrlDecode(rawPath).TrimStart('/');

            if (string.IsNullOrWhiteSpace(name))
            {
                Respond.Text(ctx, "Prosledi naziv fajla: /ime.ext", 400);
                return;
            }

            if (name.Contains("..") || name.Contains(":") || name.StartsWith("/"))
            {
                Respond.Text(ctx, "Zabranjena putanja.", 400);
                return;
            }

            var full = _files.Find(name);
            if (full == null)
            {
                Respond.Text(ctx, $"Fajl '{name}' nije pronađen.", 404);
                return;
            }

            Respond.SendFile(ctx, full);
        }
    }
}
