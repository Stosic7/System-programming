using System.IO;

namespace Zadatak10__2_
{
    public static class MimeTypes
    {
        public static string FromPath(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
