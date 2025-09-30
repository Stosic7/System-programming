using System.IO;

namespace Zadatak10__2_
{
    public static class MimeTypes
    {
        public static string FromPath(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            if (extension == ".gif")
            {
                return "image/gif";
            }
            else
            {
                return "application/octet-stream";
            }
        }
    }
}
