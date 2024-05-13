using System.IO;

namespace MLP.FileSystem
{
    public interface IWriteStrategy
    {
        void Write(string path, Stream content);
    }
}
