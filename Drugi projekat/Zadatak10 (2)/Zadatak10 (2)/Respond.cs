using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public static class Respond
    {
        public static async Task TextAsync(HttpListenerContext httpContext, string message, int statusCode = 200, CancellationToken cancellationToken = default)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message ?? string.Empty);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "text/plain; charset=utf-8";
            httpContext.Response.ContentLength64 = buffer.Length;

            using (Stream outputStream = httpContext.Response.OutputStream)
            {
                await outputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task FileAsync(HttpListenerContext httpContext, string filePath, CancellationToken cancellationToken = default)
        {
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = MimeTypes.FromPath(filePath);
            httpContext.Response.ContentLength64 = bytes.Length;

            using (Stream outputStream = httpContext.Response.OutputStream)
            {
                await outputStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
