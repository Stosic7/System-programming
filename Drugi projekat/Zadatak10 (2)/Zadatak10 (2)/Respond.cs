using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public static class Respond
    {
        public static async Task TextAsync(HttpListenerContext ctx, string msg, int code = 200, CancellationToken ct = default)
        {
            var buf = Encoding.UTF8.GetBytes(msg ?? string.Empty);
            ctx.Response.StatusCode = code;
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            ctx.Response.ContentLength64 = buf.Length;
            using var os = ctx.Response.OutputStream;
            await os.WriteAsync(buf, 0, buf.Length, ct).ConfigureAwait(false);
        }

        public static async Task FileAsync(HttpListenerContext ctx, string path, CancellationToken ct = default)
        {
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = MimeTypes.FromPath(path);
            ctx.Response.ContentLength64 = bytes.Length;
            using var os = ctx.Response.OutputStream;
            await os.WriteAsync(bytes, 0, bytes.Length, ct).ConfigureAwait(false);
        }
    }
}
