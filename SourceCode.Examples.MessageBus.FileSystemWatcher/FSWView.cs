using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.MessageBus;
using System.Net.Mime;
using System.IO;

namespace SourceCode.Examples.MessageBus.FSW
{
    class FSWView : IViewInformation
    {
        private string _filePath;
        private FileStream _fileStream;
        private IMessage _sourceMessage = null;
        

        public FSWView(string filePath, IMessage sourceMessage)
        {
            _filePath = filePath;
        }


        public System.Net.Mime.ContentType ContentType
        {
            get {
                // JD: Never tried this code - rather just return text/plain only.
                // I will have to test this style of content type.
                //System.Net.Mime.ContentType c = new System.Net.Mime.ContentType("text/plain");
                //c.Name = "plaintext";
                //c.MediaType = MediaTypeNames.Text.Plain;
                return new System.Net.Mime.ContentType("text/plain");
            }
        }

        public System.IO.Stream Open()
        {
            if (_fileStream == null)
            {
                _fileStream = File.OpenRead(_filePath);
            }
            return _fileStream as Stream;
        }

        public IMessage SourceMessage
        {
            get {
                return _sourceMessage;
            }
        }
    }
}
