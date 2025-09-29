using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class FileService
    {
        private readonly string _root;
        public FileService(string root) => _root = Path.GetFullPath(root);

        public Task<string?> FindAsync(string fileName, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    return Directory
                        .GetFiles(_root, fileName, SearchOption.AllDirectories)
                        .FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }, ct);
        }
    }
}
