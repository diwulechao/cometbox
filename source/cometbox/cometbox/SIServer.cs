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
using cometbox.HTTP;
using System.Collections.Concurrent;

namespace cometbox
{
    public class SIServer : HTTP.Server
    {
        Config.ServerInterfaceConfig config;
        public static ConcurrentDictionary<string, Client> dic = new ConcurrentDictionary< string, Client>();

        public SIServer(Config.ServerInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port, c.Authentication)
        {
            config = c;
        }

        public override HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {
            return HTTP.Response.GetHtmlResponse("GOOD!");
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
