using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SourceCode.MessageBus;
using System.Net.Mime;

namespace SourceCode.Examples.MessageBus.FSW
{

    /// <summary>
    /// This class is both the Origin and the Listener.
    /// The origin creates messages that are read by the message bus by listeners.
    /// So, when a new file is placed, all the origin code will fire, after that the messagebus will fire the Listener code.
    /// 
    /// We can seperate the Origin and Listener code in two different classes, but we haven't done that because we'd like to
    /// have access to the connection string information (to write files to the directory).
    /// </summary>
    [MessageOriginExport("SourceCode.Examples.MessageBus.FSW")]
    [MessageListenerExport(Priority = 99)]
    [MessageDestinationExport]
    public class FSWMessageHandler : IMessageOrigin, IMessageListener, IMessageDestination
    {
        [PrimaryMessageDestinationImport]
        private IMessageDestination _destination;

        private List<FileSystemWatcher> _fileWatchers = new List<FileSystemWatcher>();
        private List<ConnectionInformation> _connections = new List<ConnectionInformation>();


        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        void IMessageOrigin.Start(IEnumerable<ConnectionInformation> connections)
        {
            foreach (ConnectionInformation con in connections)
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
                fsw.Path = string.Concat(con.ConnectionString, "\\in"); // Monitor the "IN" folder of the given connectionstring.
                fsw.Created += new FileSystemEventHandler(fileWatcher_Created);
                fsw.EnableRaisingEvents = true;

                _fileWatchers.Add(fsw);
                _connections.Add(con);
            }
        }

        void IMessageOrigin.Stop()
        {
            foreach (FileSystemWatcher fsw in _fileWatchers)
            {
                fsw.EnableRaisingEvents = false;
            }
        }

        void IDisposable.Dispose()
        {
            foreach (FileSystemWatcher fsw in _fileWatchers)
            {
                fsw.EnableRaisingEvents = false;
                fsw.Dispose();
            }
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




        ContinuationAction IMessageListener.MessageReceived(ListenerContext e)
        {
            string body;
            using (Stream bodyStream = e.ReceivedInformation.Message.OpenView(new ContentType("text/plain")))
            {
                if (bodyStream != null)
                {
                    // Another plugin may have moved the position within the stream.
                    bodyStream.Seek(0, System.IO.SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(bodyStream))
                    {
                        body = sr.ReadToEnd().Trim();
                    }
                }
                else
                {
                    body = string.Empty;
                }
            }

            foreach (ConnectionInformation con in _connections)
            {
                string path = con.ConnectionString;
                string filename = string.Format(@"{0}\received\MessageReceived_{1}.txt", path, e.ReceivedInformation.Message.Title.Replace(':', '_'));
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.Write(body);
                    writer.Close();
                }
            }

            // This method should fire when a message is received by the MessageBus.
            // Processing can be done by yourself.
            e.ReceivedInformation.Commit();
            return ContinuationAction.Halt;
        }

        bool IMessageDestination.ReplyTo(IMessage message, MessageBodyReader reply, MessageExtendedInformation extended)
        {
            foreach (ConnectionInformation con in _connections)
            {

                string path = con.ConnectionString;
                string filename = string.Format(@"{0}\out\ReplyTo_{1}.txt", path, message.Title);
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    reply.Reset();
                    writer.Write(reply.ReadToEnd());
                    writer.Close();
                }
            }
            reply.Reset();
            return true;
        }

        bool IMessageDestination.Send(string title, MessageExtendedInformation extended, IEnumerable<System.Net.Mail.Attachment> attachments, params MessageBodyReader[] messageBodies)
        {
            foreach (ConnectionInformation con in _connections)
            {

                string path = con.ConnectionString;
                string filename = string.Format(@"{0}\out\Send_{1}.txt", path, title);

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    foreach (MessageBodyReader reader in messageBodies)
                    {
                        reader.Reset();
                        writer.Write(reader.ReadToEnd());
                    }
                    writer.Close();
                }
            }

            return true;
        }


    }
}
