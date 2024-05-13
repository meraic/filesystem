using System.Collections.Generic;
using System.IO;

namespace MLP.FileSystem
{
    public interface IFileSystem
    {
        Stream Read(string path);
        Stream ReadNoLock(string path);
        string ReadAllText(string path);
        Stream Write(string path);
        void WriteAllLines(string path, IEnumerable<string> contents);
        void Write(string path, Stream content, IWriteStrategy writeStrategy);
        bool Exists(string path);
        void Move(string path, string destinationPath);
        void Copy(string path, string destinationPath);
        void Move(string path, string destinationPath, bool overwrite);
        void Copy(string path, string destinationPath, bool overwrite);
        void Delete(string path);
        IEnumerable<string> List(string directoryPath);
        IEnumerable<string> ListDirectories(string directoryPath);
        IEnumerable<string> List(string directoryPath, string searchPattern);
        void CreateDirectory(string directoryPath);
        bool ExistsDirectory(string directoryPath);
        void EnsureDirectoryExists(string directoryPath);
        void DeleteDirectory(string directoryPath, bool recursive = false);
        IFileInfo GetFileInfo(string path);
        IFileVersionInfo GetFileVersionInfo(string path);
        string GetAbsolutePath(string path);
    }
}
