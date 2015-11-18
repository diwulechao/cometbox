using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace cometbox
{
    public class SIServer : HTTP.Server
    {
        Config.ServerInterfaceConfig config;

        public SIServer(Config.ServerInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port, c.Authentication)
        {
            config = c;
        }

        public override HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {
            Console.WriteLine("SIServer HandleRequest(). " + request.Url);

            Console.WriteLine("---");
            Console.WriteLine("\""+request.Body+"\"");
            Console.WriteLine("---");

            try {
                XmlSerializer s = new XmlSerializer(typeof(SIRequest));

                MemoryStream mem = new MemoryStream();
                mem.Write(System.Text.Encoding.ASCII.GetBytes(request.Body), 0, request.Body.Length);
                mem.Seek(0, 0);

                SIRequest data = (SIRequest)s.Deserialize(mem);

                return HTTP.Response.GetHtmlResponse("GOOD!");
            } catch (Exception e) {
                Console.WriteLine("Error parsing xml: " + e.Message);
                return HTTP.Response.GetHtmlResponse("Error parsing xml: " + e.Message);
            }
        }

        [XmlRoot("Request")]
        public class SIRequest
        {
            public enum CommandType
            {
                Queue,
                Remove
            }

            public struct MessageData
            {
                public string Message;
            }

            public CommandType Command = CommandType.Queue;
            public string User = "";

            [XmlElement("Message")]
            public string[] Messages;
        }
    }
}
