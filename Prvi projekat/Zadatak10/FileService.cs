using System;
using System.IO;

namespace Zadatak10
{
    public sealed class FileService
    {
        private readonly string originalRootPath;

        public FileService(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("root je obavezan");

            originalRootPath = Path.GetFullPath(rootPath);
        }

        public string? Find(string fileName)
        {
            try
            {
                string[] foundFiles = Directory.GetFiles(originalRootPath, fileName, SearchOption.AllDirectories);

                for (int i = 0; i < foundFiles.Length; i++)
                {
                    // vraćamo prvi pogodak
                    return foundFiles[i];
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
