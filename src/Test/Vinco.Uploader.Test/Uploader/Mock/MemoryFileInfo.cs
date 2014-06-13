using System.IO;


namespace Vinco.Uploader.Mock
{
    public class MemoryFileInfo : FileInfoBase
    {
        private readonly Stream _stream;

        public MemoryFileInfo(string text)
        {
            _stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public override string FullName
        {
            get { return typeof(MemoryFileInfo).FullName; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override string Name
        {
            get { return typeof(MemoryFileInfo).Name; }
        }

        public override Stream OpenRead()
        {
            return _stream;
        }
    }
}