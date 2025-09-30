using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak10__2_
{
    public sealed class FileService
    {
        private readonly string originalRootPath;

        public FileService(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("rootPath je obavezan.");

            originalRootPath = Path.GetFullPath(rootPath);
        }

        public Task<string?> FindAsync(string fileName, CancellationToken cancellationToken)
        {
            FindState state = new FindState { FileName = fileName, Token = cancellationToken };

            return Task.Factory.StartNew(
                FindInternalFromState,
                state,
                cancellationToken,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default
            );
        }

        private sealed class FindState
        {
            public string FileName = string.Empty;
            public CancellationToken Token;
        }

        private string? FindInternalFromState(object? boxedState)
        {
            FindState state = (FindState)boxedState!;
            state.Token.ThrowIfCancellationRequested();

            try
            {
                string[] found = Directory.GetFiles(originalRootPath, state.FileName, SearchOption.AllDirectories);

                for (int i = 0; i < found.Length; i++)
                {
                    // Vracamo prvi pogodak
                    return found[i];
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
