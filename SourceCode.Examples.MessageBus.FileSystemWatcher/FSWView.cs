using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.MessageBus;
using System.Net.Mime;
using System.IO;
using System.Threading;

namespace SourceCode.Examples.MessageBus.FSW
{
    class FSWView : IViewInformation, IDisposable
    {
        private string _filePath;
        private Stream _fileStream;
        private FSWMessage _sourceMessage = null;


        public FSWView(string filePath, FSWMessage sourceMessage)
        {
            _filePath = filePath;
            _sourceMessage = sourceMessage;
        }


        public System.Net.Mime.ContentType ContentType
        {
            get
            {
                // JD: Never tried this code - rather just return text/plain only.
                // I will have to test this style of content type.
                //System.Net.Mime.ContentType c = new System.Net.Mime.ContentType("text/plain");
                //c.Name = "plaintext";
                //c.MediaType = MediaTypeNames.Text.Plain;
                //return c;
                return new System.Net.Mime.ContentType("text/plain");
            }
        }

        public System.IO.Stream Open()
        {
            if (_fileStream == null)
            {
                _fileStream = File.OpenRead(_filePath);
            }
            _fileStream.Seek(0, SeekOrigin.Begin);
            return (_fileStream as Stream).ProtectFromDispose();
        }

        public IMessage SourceMessage
        {
            get
            {
                return _sourceMessage;
            }
        }

        public void Dispose()
        {
            Stream s = _fileStream as Stream;

            Stream stream = Interlocked.Exchange<Stream>(ref s, null);
            if (stream != null)
            {
                stream.Dispose();
            }
        }
    }
}
