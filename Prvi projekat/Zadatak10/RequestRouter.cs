using System;
using System.Net;

namespace Zadatak10
{
    public sealed class RequestRouter
    {
        private readonly FileService fileService;

        public RequestRouter(FileService files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            fileService = files;
        }

        public void Handle(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
            {
                Respond.Text(context, "Method Not Allowed", 405);
                return;
            }

            if (context.Request.Url == null)
            {
                Respond.Text(context, "Neispravan zahtev.", 400);
                return;
            }

            string rawPath = context.Request.Url.AbsolutePath;
            string requestedName = WebUtility.UrlDecode(rawPath).TrimStart('/');

            if (string.IsNullOrWhiteSpace(requestedName))
            {
                Respond.Text(context, "Prosledi naziv fajla: /ime.gif", 400);
                return;
            }

            if (requestedName.Contains("..") || requestedName.Contains(":") || requestedName.StartsWith("/"))
            {
                Respond.Text(context, "Zabranjena putanja.", 400);
                return;
            }

            string? fullPath = fileService.Find(requestedName);
            if (fullPath == null)
            {
                Respond.Text(context, $"Fajl '{requestedName}' nije pronađen.", 404);
                return;
            }

            Respond.SendFile(context, fullPath);
        }
    }
}
