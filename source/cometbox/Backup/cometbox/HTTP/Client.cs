using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox.HTTP
{
    public class Client
    {
        private TcpClient client = null;
        private NetworkStream stream = null;
        private Config.AuthConfig authconfig;

        Server server;

        byte[] read_buffer = new byte[1024];
        string buffer = "";
        int bufferpos = 0;

        HTTP.Request request;
        ParseState state = ParseState.Start;

        public bool IsLive = true;

        public Client(TcpClient c, Server s, Config.AuthConfig a)
        {
            client = c;
            server = s;
            authconfig = a;

            stream = client.GetStream();

            stream.BeginRead(read_buffer, 0, 1024, new AsyncCallback(callbackRead), this);
        }

        public static void callbackRead(IAsyncResult ar)
        {
            Client dc = (Client)ar.AsyncState;
            int bytes = 0;

            //try {
            bytes = dc.stream.EndRead(ar);
            if (bytes <= 0) {
                dc.CleanUp();
                return;
            }

            dc.buffer += Encoding.ASCII.GetString(dc.read_buffer, 0, bytes);

            //Console.WriteLine(dc.buffer);

            dc.ParseInput();

            if (dc.stream != null) {
                dc.stream.BeginRead(dc.read_buffer, 0, 1024, new AsyncCallback(Client.callbackRead), dc);
            }
            //} catch (Exception e) {
            //	Console.WriteLine("Client: Error in callbackRead() - {0}", e.Message);
            //}
        }

        private enum ParseState
        {
            Start,
            Headers,
            Content,
            Done
        }

        private void ParseInput()
        {
            int pos;
            string temp;
            bool skip = false;
            while (bufferpos < buffer.Length - 1 && state != ParseState.Done && !skip) {
                switch (state) {
                    case ParseState.Start:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0) {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            string[] parts = temp.Split(' ');
                            if (parts.Length == 3) {
                                request = new HTTP.Request();

                                request.Method = parts[0];
                                request.Url = parts[1];
                                request.Version = parts[2];

                                state = ParseState.Headers;
                            } else {
                                CleanUp();
                            }
                        }
                        break;
                    case ParseState.Headers:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0) {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            if (temp.Length > 0) {
                                string[] parts = temp.Split(new string[1] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2) {
                                    request.Headers.Add(parts[0], parts[1]);
                                }
                            } else {
                                if (request.HasContent()) {
                                    state = ParseState.Content;
                                } else {
                                    state = ParseState.Done;
                                }
                            }
                        }
                        break;
                    case ParseState.Content:
                        if (bufferpos + request.ContentLength <= buffer.Length) {
                            int t = bufferpos + request.ContentLength - 1;
                            request.Body = buffer.Substring(bufferpos, request.ContentLength);
                            bufferpos += request.ContentLength;

                            state = ParseState.Done;
                        } else {
                            skip = true;
                        }
                        break;
                }
            }

            if (state == ParseState.Done) {
                state = ParseState.Start;

                HTTP.Response response;
                if (request.VerifyAuth(authconfig)) {
                    response = server.HandleRequest(request);
                } else {
                    response = Response.Get401Response(authconfig);
                }
                response.SendResponse(stream, this);

                request = null;
            }
        }

        public void Send(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            int offset = 0;
            int len = 0;
            Console.WriteLine("Sending: " + bytes.Length);
            while (offset < bytes.Length) {
                offset = Math.Min(offset, bytes.Length - 1);
                len = Math.Min(1024, bytes.Length - offset);

                stream.Write(bytes, offset, len);

                offset += 1024;
            }
            Console.WriteLine("Done.");
        }

        public void CleanUp()
        {
            try {
                stream.Close();
            }
            catch { }
            try {
                stream.Dispose();
            }
            catch { }
            try {
                client.Close();
            }
            catch { }

            stream = null;
            client = null;

            IsLive = false;
        }
    }
}


