using System.IO;


namespace Vinco.Uploader
{
    public abstract class FileInfoBase
    {
        public abstract string Name { get; }

        public abstract string FullName { get; }

        public abstract long Length { get; }

        public abstract Stream OpenRead();
    }
}
