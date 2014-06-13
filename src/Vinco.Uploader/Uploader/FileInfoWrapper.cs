using System;
using System.IO;


namespace Vinco.Uploader
{
    public class FileInfoWrapper : FileInfoBase
    {
        private readonly FileInfo _fileInfo;

        public FileInfoWrapper(FileInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _fileInfo = info;
        }

        public override  string Name
        {
            get { return _fileInfo.Name; }
        }

        public override  long Length
        {
            get { return _fileInfo.Length; }
        }

        public override  Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public override string FullName
        {
            get { return _fileInfo.ToString(); }
        }
    }
}