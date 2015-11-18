using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox.Config
{
    public struct AuthConfig
    {
        public AuthType Type;
        public string Username;
        public string Password;
        public string Realm;
    }
}
