using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace cometbox.Config
{
    public class AppConfig
    {
        public ServerInterfaceConfig ServerInterface;
        public ClientInterfaceConfig ClientInterface;
        public WebInterfaceConfig WebInterface;

        public static AppConfig LoadConfig(string file)
        {
            AppConfig c = null;
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                    c = (AppConfig)serializer.Deserialize(fs);
                }
            }
            catch { }

            return c;
        }

        public static void BuildNewConfigFile(string file)
        {
            Config.AppConfig c = new Config.AppConfig();

            c.ClientInterface = new Config.ClientInterfaceConfig();
            c.ClientInterface.Authentication = new Config.AuthConfig();
            c.ClientInterface.Authentication.Type = Config.AuthType.None;
            c.ClientInterface.Authentication.Realm = "2cometbox Client Interface";
            c.ClientInterface.Authentication.Username = "none";
            c.ClientInterface.Authentication.Password = "none";

            c.WebInterface = new Config.WebInterfaceConfig();
            c.WebInterface.Authentication = new Config.AuthConfig();
            c.WebInterface.Authentication.Type = Config.AuthType.Basic;
            c.WebInterface.Authentication.Realm = "Cometbox Web Interface";
            c.WebInterface.Authentication.Username = "cometbox";
            c.WebInterface.Authentication.Password = "cometbox123";

            c.ServerInterface = new Config.ServerInterfaceConfig();
            c.ServerInterface.Authentication = new Config.AuthConfig();
            c.ServerInterface.Authentication.Type = Config.AuthType.Basic;
            c.ServerInterface.Authentication.Realm = "Cometbox Server Interface";
            c.ServerInterface.Authentication.Username = "server";
            c.ServerInterface.Authentication.Password = "server123";
            c.ServerInterface.AcceptedIPs = new string[] {"127.0.0.1"};

            using (FileStream fs = new FileStream(file, FileMode.Create)) {
                XmlSerializer serializer = new XmlSerializer(typeof(Config.AppConfig));
                serializer.Serialize(fs, c);
                fs.Close();
            }
        }
    }
}
