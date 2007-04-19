using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace cometbox
{
    class Program
    {
        public static Config.AppConfig Configuration;

        static void Main(string[] args)
        {
            string configfile = "";

            if (args.Length == 0)
            {
                configfile = "cometbox.conf";
            }
            else
            {
                configfile = args[0];
            }

            if (!new FileInfo(configfile).Exists) {
                try {
                    Config.AppConfig.BuildNewConfigFile(configfile);
                } catch { }
            }

            if (new FileInfo(configfile).Exists)
            {
                if ((Configuration = Config.AppConfig.LoadConfig(configfile)) == null)
                {
                    Console.WriteLine("Error loading configuration.");
                    GracefullyChokeAndDie();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Configuration not found: " + configfile);
                GracefullyChokeAndDie();
                return;
            }

            WIServer wiserver = new WIServer(Configuration.WebInterface);
            SIServer siserver = new SIServer(Configuration.ServerInterface);

        }

        static void GracefullyChokeAndDie()
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
