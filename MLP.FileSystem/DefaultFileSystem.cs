using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MLP.FileSystem
{
    public class DefaultFileSystem : IFileSystem
    {
        public IEnumerable<string> List(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public IEnumerable<string> List(string directoryPath, string searchPattern)
        {
            return Directory.GetFiles(directoryPath, searchPattern);
        }

        public IEnumerable<string> ListDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

        public Stream Read(string path)
        {
            return File.OpenRead(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public Stream ReadNoLock(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }

        public Stream Write(string path)
        {
            return File.OpenWrite(path);
        }

        public void Write(string path, Stream content, IWriteStrategy writeStrategy)
        {
            writeStrategy.Write(path, content);
        }

        public void WriteAllLines(string path, IEnumerable<string> contents)
        {
            File.WriteAllLines(path, contents);
        }

        public async Task Write(Stream stream, string path)
        {
            using (var fs = File.OpenWrite(path))
            {
                await stream.CopyToAsync(fs);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public bool ExistsDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        public void Move(string path, string destinationPath)
        {
            Move(path, destinationPath, true);
        }

        public void Move(string path, string destinationPath, bool overwrite)
        {
            if (overwrite && File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            File.Move(path, destinationPath);
        }

        public void Copy(string path, string destinationPath)
        {
            Copy(path, destinationPath, true);
        }

        public void Copy(string path, string destinationPath, bool overwrite)
        {
            File.Copy(path, destinationPath, overwrite);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public void DeleteDirectory(string directoryPath, bool recursive = false)
        {
            Directory.Delete(directoryPath);
        }

        public IFileInfo GetFileInfo(string path)
        {
            var fileInfo = new FileInfo(path);
            return new FileInfoWrapper(fileInfo);
        }

        public IFileVersionInfo GetFileVersionInfo(string path)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
            return new FileVersionInfoWrapper(fileVersionInfo);
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!ExistsDirectory(directoryPath))
            {
                CreateDirectory(directoryPath);
            }
        }

        public string GetAbsolutePath(string path)
        {
            return Path.GetFullPath(path);
        }
    }
}
