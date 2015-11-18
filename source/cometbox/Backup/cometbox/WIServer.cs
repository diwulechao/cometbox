using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace cometbox
{
    class WIServer : HTTP.Server
    {
        Config.WebInterfaceConfig config;

        public WIServer(Config.WebInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port, c.Authentication)
        {
            config = c;
        }

        public override HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {
            Console.WriteLine("WIServer HandleRequest(). " + request.Url);

            int pos = 0;
            string doc = request.Url;
            if ((pos = doc.IndexOf("//")) >= 0) {
                doc = doc.Substring(pos + 2, doc.Length - pos - 2); 
            }
            if ((pos = doc.IndexOf("/")) == 0) {
                doc = doc.Substring(pos + 1, doc.Length - pos - 1);
            }
            if ((pos = doc.IndexOf("?")) >= 0) {
                doc = doc.Substring(0, pos + 1);
            }
            if (doc == "") {
                doc = "index.html";
            }

            FileInfo f = new FileInfo(config.WWWDir + doc);

            return HTTP.Response.GetFileResponse(f, request.Url);
        }
    }
}
