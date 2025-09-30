using System;
using System.IO;

namespace Zadatak10
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

            return "application/octet-stream";
        }
    }
}
