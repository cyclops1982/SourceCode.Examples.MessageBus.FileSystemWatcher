using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.MessageBus;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SourceCode.Examples.MessageBus.FSW
{
    public class FSWMessage : IMessage
    {
        private string _fileName;
        private string _filePath;
        private IViewInformation[] _views;

        public FSWMessage(FileSystemEventArgs fileEvent)
        {
            _fileName = fileEvent.Name;
            _filePath = fileEvent.FullPath;
            _views = new IViewInformation[1];
            _views[0] = new FSWView(_filePath, this);
        }

        public IEnumerable<IAttachmentInformation> Attachments
        {
            // JD: We don't use this yet, but it's best to return an empty list and not null.
            get { return Enumerable.Empty<IAttachmentInformation>(); }
        }

        public T GetExtendedInformation<T>(Func<T> constructor) where T : MessageExtendedInformation
        {
            T extender = constructor();


            // We tried getting the file owner, but that's "buildin\administrator", which is not resolvable by our k2 security provider.
            // So, for now we hardcoded this.
            AlternativeCollection from = new AlternativeCollection();
            from.Add("Fqn", "K2:DENALLIX\\Administrator");


            AlternativeCollection[] to = new AlternativeCollection[1];
            AlternativeCollection t = new AlternativeCollection();
            t.Add("Fqn", "K2:DENALLIX\\Administrator");
            t.IsCarbonCopy = false;
            t.IsBlindCarbonCopy = false;
            to[0] = t;

            extender.Initialize(this, from, to);
            return extender;
        }

        public bool HasHtml
        {
            get
            {
                return false;
            }
        }

        public bool IsReply
        {
            get
            {
                return true;
            }
        }

        public string Title
        {
            get
            {
                return _fileName;
            }
        }

        public IEnumerable<IViewInformation> Views
        {
            get
            {
                return _views;
            }
        }

        public void Dispose()
        {
        }
    }
}
