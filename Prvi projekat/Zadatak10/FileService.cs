using System;
using System.IO;
using System.Linq;

namespace Zadatak10
{
    public sealed class FileService
    {
        private readonly string _root;

        public FileService(string root)
        {
            if (string.IsNullOrWhiteSpace(root)) throw new ArgumentException("root je obavezan");
            _root = Path.GetFullPath(root);
        }

        public string? Find(string fileName)
        {
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
        }
    }
}
