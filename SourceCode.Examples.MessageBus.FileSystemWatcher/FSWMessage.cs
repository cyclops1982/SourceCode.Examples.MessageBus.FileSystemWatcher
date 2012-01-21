using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.MessageBus;
using System.IO;

namespace SourceCode.Examples.MessageBus.FSW
{
    public class FSWMessage : IMessage
    {
        private string _fileName;
        private string _filePath; 



        public FSWMessage(FileSystemEventArgs fileEvent)
        {
            _fileName = fileEvent.Name;
            _filePath = fileEvent.FullPath;
        }

        private string ReadFileContent(string filePath)
        {
            String content;
            using (StreamReader sr = new StreamReader(filePath))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }


        public IEnumerable<IAttachmentInformation> Attachments
        {
            // JD: We don't use this yet, but it's best to return an empty list and not null.
            get { return Enumerable.Empty<IAttachmentInformation>(); }
        }

        public T GetExtendedInformation<T>(Func<T> constructor) where T : MessageExtendedInformation
        {
            T extender = constructor();
            AlternativeCollection from = new AlternativeCollection();
            from.Add("Fqn", "K2:DENALLIX\\Administrator");
            from.IsBlindCarbonCopy = false;
            from.IsCarbonCopy = false;

            AlternativeCollection[] to = new AlternativeCollection[1];
            AlternativeCollection t = new AlternativeCollection();
            //t.Add("Email", "k2service@denallix.com");
            t.Add("Fqn", "K2:DENALLIX\\Administrator");
            t.IsCarbonCopy = false;
            t.IsBlindCarbonCopy = false;
            to[0] = t;

            extender.Initialize(this, from, to);
            return extender;
        }

        public bool HasHtml
        {
            get { 
                return false;
            }
        }

        public bool IsReply
        {
            get { 
                return true;
            }
        }

        public string Title
        {
            get {
                return _fileName;
            }
        }

        public IEnumerable<IViewInformation> Views
        {
            get {
                List<IViewInformation> vs = new List<IViewInformation>();
                vs.Add(new FSWView(_filePath, this));
                return vs;
            }
        }

        public void Dispose()
        {
        }
    }
}
