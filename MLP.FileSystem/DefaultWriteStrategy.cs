using System.IO;

namespace MLP.FileSystem
{
    class DefaultWriteStrategy : IWriteStrategy
    {

        public void Write(string path, Stream content)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                content.CopyTo(fileStream);
            }
        }
    }
}
