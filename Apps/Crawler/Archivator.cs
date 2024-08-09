using Crawler.Interfaces;
using System.IO.Compression;

namespace Crawler
{
    public class Archivator : IArchivator
    {
        public string Create(string filePath)
        {
            string archivePath = filePath + ".zip";
            ZipFile.CreateFromDirectory(filePath, archivePath);
            return archivePath;
        }
    }
}
