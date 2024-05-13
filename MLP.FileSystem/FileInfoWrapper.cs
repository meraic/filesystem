using System;
using System.IO;

namespace MLP.FileSystem
{
    public class FileInfoWrapper : IFileInfo
    {
        private readonly FileInfo fileInfo;
        public FileInfoWrapper(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            FullName = fileInfo.FullName;
        }

        public FileInfoWrapper(FileInfo fileInfo, string fakePath)
        {
            this.fileInfo = fileInfo;
            FullName = fakePath;
        }

        public string Name => fileInfo.Name;

        public string FullName { get; }

        public long Length => fileInfo.Length;

        public bool Exists => fileInfo.Exists;

        public DateTime CreationTimeUtc => fileInfo.CreationTimeUtc;

        public DateTime LastAccessTimeUtc => fileInfo.LastAccessTimeUtc;

        public DateTime LastWriteTimeUtc => fileInfo.LastWriteTimeUtc;
    }
}
