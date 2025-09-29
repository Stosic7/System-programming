using System.Net;
using System.Text;

namespace Zadatak10
{
    public static class Respond
    {
        public static void Text(HttpListenerContext ctx, string msg, int code = 200)
        {
            var buf = Encoding.UTF8.GetBytes(msg ?? string.Empty);
            ctx.Response.StatusCode = code;
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            ctx.Response.ContentLength64 = buf.Length;
            using var os = ctx.Response.OutputStream;
            os.Write(buf, 0, buf.Length);
        }

        public static void SendFile(HttpListenerContext ctx, string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = MimeTypes.FromPath(path);
            ctx.Response.ContentLength64 = bytes.Length;
            using var os = ctx.Response.OutputStream;
            os.Write(bytes, 0, bytes.Length);
        }
    }
}
