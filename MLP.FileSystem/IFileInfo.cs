using System;

namespace MLP.FileSystem
{
    public interface IFileInfo
    {
        string Name { get; }
        string FullName { get; }
        long Length { get; }
        bool Exists { get; }
        DateTime CreationTimeUtc { get; }
        DateTime LastAccessTimeUtc { get; }
        DateTime LastWriteTimeUtc { get; }
    }
}
