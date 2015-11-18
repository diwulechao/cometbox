using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox.Config
{
    public class WebInterfaceConfig
    {
        public AuthConfig Authentication;
        public string BindTo = "127.0.0.1";
        public int Port = 1801;
        public string WWWDir = @"wi\";
    }
}
