using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace cometbox.Config
{
    public class ServerInterfaceConfig
    {
        public AuthConfig Authentication;
        public string[] AcceptedIPs;
        public string BindTo = "127.0.0.1";
        public int Port = 1802;
    }
}
