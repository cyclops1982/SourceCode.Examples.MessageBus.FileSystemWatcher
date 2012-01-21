//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SourceCode.MessageBus;
//using System.IO;

//namespace SourceCode.Examples.MessageBus.FSW
//{
//    [MessageDestinationExport]
//    public class FSWMessageDestination : IMessageDestination
//    {

//        public bool ReplyTo(IMessage message, MessageBodyReader reply, MessageExtendedInformation extended)
//        {
//            string filename = string.Format(@"c:\testing\RubenFiles\out\ReplyTo_{0}", message.Title);
//            using (StreamWriter writer = new StreamWriter(filename))
//            {
//                writer.Write(reply.ReadToEnd());
//                writer.Flush();
//                writer.Close();
//            }
//            return true;
//        }

//        public bool Send(string title, MessageExtendedInformation extended, IEnumerable<System.Net.Mail.Attachment> attachments, params MessageBodyReader[] messageBodies)
//        {
//            string filename = string.Format(@"c:\testing\RubenFiles\out\Send_{0}.txt", title);
//            using (StreamWriter writer = new StreamWriter(filename))
//            {
//                foreach (MessageBodyReader reader in messageBodies)
//                {
//                    writer.Write(reader.ReadToEnd());
//                    writer.WriteLine("=======================");
//                }
//                writer.Flush();
//                writer.Close();

//            }
//            return true;
//        }

//    }
//}
