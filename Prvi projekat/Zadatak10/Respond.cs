using System.IO;
using System.Net;
using System.Text;

namespace Zadatak10
{
    public static class Respond
    {
        public static void Text(HttpListenerContext context, string message, int code = 200)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message ?? string.Empty);

            context.Response.StatusCode = code;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;

            using (Stream outputStream = context.Response.OutputStream)
            {
                outputStream.Write(buffer, 0, buffer.Length);
            }
        }

        public static void SendFile(HttpListenerContext context, string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            context.Response.StatusCode = 200;
            context.Response.ContentType = MimeTypes.FromPath(path);
            context.Response.ContentLength64 = bytes.Length;

            using (Stream outputStream = context.Response.OutputStream)
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
