using System.Diagnostics;

namespace MLP.FileSystem
{
    public class FileVersionInfoWrapper : IFileVersionInfo
    {
        private readonly FileVersionInfo fileVersionInfo;

        public FileVersionInfoWrapper(FileVersionInfo fileVersionInfo)
        {
            this.fileVersionInfo = fileVersionInfo;
        }
        public string FileVersion => fileVersionInfo.FileVersion;
    }
}
