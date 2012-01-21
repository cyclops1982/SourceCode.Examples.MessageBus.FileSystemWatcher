using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SourceCode.MessageBus;

namespace SourceCode.Examples.MessageBus.FSW
{
    [MessageOriginExport("SourceCode.Examples.MessageBus.FSW")]
    [MessageDestinationExport]
    public class FSWMessageOrigin : IMessageOrigin, IMessageDestination
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public List<ConnectionInformation> _connectionInfo = new List<ConnectionInformation>();

        private List<FileSystemWatcher> _fileWatchers = new List<FileSystemWatcher>();

        public void Start(IEnumerable<ConnectionInformation> connections)
        {
            foreach (ConnectionInformation con in connections)
            {
                Start(con);
                _connectionInfo.Add(con);
            }
        }

        private void Start(ConnectionInformation con)
        {
            /* ConnectionIdentifier: FileWatcher1
             * ConnectionString: c:\testing\RubenFiles\
             * ConnectionType: SourceCode.Examples.MessageBus.FileSystemWatcher
             *
             * ConnectionIdentifier: FileWatcher2
             * ConnectionString: c:\testing\RubenFiles\
             * ConnectionType: SourceCode.Examples.MessageBus.FileSystemWatcher
             */

            FileSystemWatcher fsw = new FileSystemWatcher();
            fsw.Path = string.Concat(con.ConnectionString, "\\In");
            fsw.Created += new FileSystemEventHandler(fileWatcher_Created);
            fsw.EnableRaisingEvents = true;

            _fileWatchers.Add(fsw);
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            FSWMessage msg = new FSWMessage(e);

            MessageReceivedEventArgs args = new MessageReceivedEventArgs(msg);
            if (MessageReceived != null)
            {
                MessageReceived(this, args);
            }

        }

        public void Stop()
        {
            foreach (FileSystemWatcher fsw in _fileWatchers)
            {
                fsw.EnableRaisingEvents = false;
            }
        }

        public void Dispose()
        {
            foreach (FileSystemWatcher fsw in _fileWatchers)
            {
                fsw.EnableRaisingEvents = false;
                fsw.Dispose();
            }
        }

        public bool ReplyTo(IMessage message, MessageBodyReader reply, MessageExtendedInformation extended)
        {

            string path = _connectionInfo[0].ConnectionString;
            string filename = string.Format(@"{0}\out\ReplyTo_{1}.txt", path, message.Title);
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(reply.ReadToEnd());
                writer.Close();
            }
            return true;
        }

        public bool Send(string title, MessageExtendedInformation extended, IEnumerable<System.Net.Mail.Attachment> attachments, params MessageBodyReader[] messageBodies)
        {
            string path = _connectionInfo[0].ConnectionString;
            string filename = string.Format(@"{0}\out\Send_{1}.txt", path, title);



            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (MessageBodyReader reader in messageBodies)
                {
                    writer.Write(reader.ReadToEnd());
                }
                writer.Close();
            }


            return true;
        }


    }
}
