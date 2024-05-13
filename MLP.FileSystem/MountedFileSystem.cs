using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MLP.FileSystem
{
    public class MountedFileSystem : IFileSystem, IDisposable
    {
        private readonly object mountLock = new object();

        private bool didMount;

        public string MountPoint { get; set; }

        public CleanOnDisposeAction CleanOnDisposeAction { get; set; } = CleanOnDisposeAction.None;

        public async Task Write(Stream stream, string path)
        {
            var mountedPath = GetMountedPath(path);

            using (var fs = File.OpenWrite(mountedPath))
            {
                await stream.CopyToAsync(fs).ConfigureAwait(false);
            }
        }

        public string GetMountedPath(string path)
        {
            if (string.IsNullOrEmpty(MountPoint))
            {
                return path;
            }

            EnsureMounted();

            path = path.TrimStart('/', '\\');

            if (Path.IsPathRooted(path))
            {
                path = path.Replace(":", "_");
            }

            var mountedPath = Path.Combine(MountPoint, path);

            if (!Path.GetFullPath(mountedPath).StartsWith(Path.GetFullPath(MountPoint)))
            {
                throw new ArgumentException("Path traversal not supported", nameof(path));
            }

            return mountedPath;
        }

        private void EnsureMounted()
        {
            if (didMount || string.IsNullOrEmpty(MountPoint))
            {
                return;
            }

            lock (mountLock)
            {
                if (!Directory.Exists(MountPoint))
                {
                    Directory.CreateDirectory(MountPoint);
                }

                didMount = true;
            }
        }

        public void CreateDirectory(string directoryPath)
        {
            var mountedPath = GetMountedPath(directoryPath);
            Directory.CreateDirectory(mountedPath);
        }

        public IEnumerable<string> List(string directoryPath)
        {
            return ListPathContents(directoryPath, Directory.GetFiles);
        }

        public IEnumerable<string> List(string directoryPath, string searchPattern)
        {
            return ListPathContents(directoryPath,  s => Directory.GetFiles(s, searchPattern));
        }

        public IEnumerable<string> ListDirectories(string directoryPath)
        {
            return ListPathContents(directoryPath, Directory.GetDirectories);
        }

        private string GetMountPointPrefix(string directoryPath)
        {
            if (string.IsNullOrEmpty(MountPoint))
            {
                return string.Empty;
            }

            return 
                directoryPath.StartsWith(@"\\") || directoryPath.StartsWith(@"//")
                ? @"\" 
                : string.Empty;
        }

        private IEnumerable<string> ListPathContents(string directoryPath, Func<string, IEnumerable<string>> listFunx)
        {
            var prefix = GetMountPointPrefix(directoryPath);

            var mountedPath = Path.GetFullPath(GetMountedPath(directoryPath));

            var mountedPathPrefix = GetMountedPath(string.Empty);

            var mountedFullPathLength = string.IsNullOrEmpty(mountedPathPrefix)
                ? 0
                : Path.GetFullPath(mountedPathPrefix).Length;

            return listFunx(mountedPath).Select(d => $"{prefix}{d.Substring(mountedFullPathLength)}");
        }

        public Stream Read(string path)
        {
            return File.OpenRead(GetMountedPath(path));
        }

        public Stream ReadNoLock(string path)
        {
            return File.Open(GetMountedPath(path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(GetMountedPath(path));
        }

        public Stream Write(string path)
        {
            return File.OpenWrite(GetMountedPath(path));
        }

        public void WriteAllLines(string path, IEnumerable<string> contents)
        {
            File.WriteAllLines(GetMountedPath(path), contents);
        }

        public void Write(string path, Stream content, IWriteStrategy writeStrategy)
        {
            writeStrategy.Write(GetMountedPath(path), content);
        }

        public bool ExistsDirectory(string directoryPath)
        {
            return Directory.Exists(GetMountedPath(directoryPath));
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!ExistsDirectory(directoryPath))
            {
                CreateDirectory(directoryPath);
            }
        }

        public void Move(string path, string destinationPath)
        {
            Move(path, destinationPath, true);
        }

        public void Move(string path, string destinationPath, bool overwrite)
        {
            var mountedPath = GetMountedPath(path);
            var mountedDestPath = GetMountedPath(destinationPath);

            if (overwrite && File.Exists(mountedDestPath))
            {
                File.Delete(mountedDestPath);
            }

            File.Move(mountedPath, mountedDestPath);
        }

        public void Copy(string path, string destinationPath)
        {
            Copy(path, destinationPath, true);
        }

        public void Copy(string path, string destinationPath, bool overwrite)
        {
            var mountedPath = GetMountedPath(path);
            var mountedDestPath = GetMountedPath(destinationPath);

            File.Copy(mountedPath, mountedDestPath, overwrite);
        }

        public void Delete(string path)
        {
            File.Delete(GetMountedPath(path));
        }

        public void DeleteDirectory(string directoryPath, bool recursive = false)
        {
            Directory.Delete(GetMountedPath(directoryPath), recursive);
        }

        public bool Exists(string path)
        {
            return File.Exists(GetMountedPath(path));
        }

        public IFileInfo GetFileInfo(string path)
        {
            return new FileInfoWrapper(new FileInfo(GetMountedPath(path)), path);
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(MountPoint) || !Directory.Exists(MountPoint))
            {
                return;
            }

            if (CleanOnDisposeAction == CleanOnDisposeAction.Delete)
            {
                Directory.Delete(MountPoint, true);
            }
        }

        public string GetAbsolutePath(string path)
        {
            return GetMountedPath(path);
        }

        public IFileVersionInfo GetFileVersionInfo(string path)
        {
            return new FileVersionInfoWrapper(FileVersionInfo.GetVersionInfo(GetMountedPath(path)));
        }
    }
}
