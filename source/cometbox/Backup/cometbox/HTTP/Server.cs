using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace cometbox.HTTP
{
    abstract public class Server
    {
        private Thread thread = null;
		private TcpListener listener = null;
        private Config.AuthConfig authconfig;

		public Server(IPAddress ip, Int32 port, Config.AuthConfig a)
		{
            Console.WriteLine("Server: Starting on {0}:{1}.", ip.ToString(), port);

            authconfig = a;

			listener = new TcpListener(port);
			listener.Start();
		
			thread = new Thread(new ThreadStart(Loop));
			thread.Start();
		}
				
		private void Loop()
		{
			List<Client> clients = new List<Client>();
	
			while ( true ) {
				TcpClient client = listener.AcceptTcpClient();

				Client dc = new Client(client, this, authconfig);
				clients.Add(dc);
				
				int i = 0;
				while (i < clients.Count) {
					if ( !clients[i].IsLive ) {
						clients.RemoveAt(i);
					}
					i++;
				}
			}
		}
		
		public bool IsRunning()
		{
			return thread.IsAlive;
		}

        abstract public Response HandleRequest(Request request);
    }
}
