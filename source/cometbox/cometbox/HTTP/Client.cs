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

            if (!dc.IsLive) return;
            bytes = dc.stream.EndRead(ar);
            if (bytes <= 0)
            {
                dc.CleanUp();
                return;
            }

            dc.buffer += Encoding.ASCII.GetString(dc.read_buffer, 0, bytes);
            dc.ParseInput();

            if (dc.stream != null)
            {
                try
                {
                    dc.stream.BeginRead(dc.read_buffer, 0, 1024, new AsyncCallback(Client.callbackRead), dc);
                }
                catch (Exception e)
                {
                }
            }
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
            while (bufferpos < buffer.Length - 1 && state != ParseState.Done && !skip)
            {
                switch (state)
                {
                    case ParseState.Start:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0)
                        {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            string[] parts = temp.Split(' ');
                            if (parts.Length == 3)
                            {
                                request = new HTTP.Request();

                                request.Method = parts[0];
                                request.Url = parts[1];
                                request.Version = parts[2];

                                state = ParseState.Headers;
                            }
                            else
                            {
                                CleanUp();
                            }
                        }
                        break;
                    case ParseState.Headers:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0)
                        {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            if (temp.Length > 0)
                            {
                                string[] parts = temp.Split(new string[1] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2)
                                {
                                    request.Headers.Add(parts[0], parts[1]);
                                }
                            }
                            else
                            {
                                if (request.HasContent())
                                {
                                    state = ParseState.Content;
                                }
                                else
                                {
                                    state = ParseState.Done;
                                }
                            }
                        }
                        break;
                    case ParseState.Content:
                        if (bufferpos + request.ContentLength <= buffer.Length)
                        {
                            int t = bufferpos + request.ContentLength - 1;
                            request.Body = buffer.Substring(bufferpos, request.ContentLength);
                            bufferpos += request.ContentLength;

                            state = ParseState.Done;
                        }
                        else
                        {
                            skip = true;
                        }
                        break;
                }
            }

            if (state == ParseState.Done)
            {
                state = ParseState.Start;

                string toId = null, id = null, command = null;
                try
                {
                    string[] array = request.Body.Split('|');
                    id = array[0];
                    toId = array[1];
                    command = array[2];
                }
                catch (Exception e)
                {
                    CleanUp();
                    request = null;
                    return;
                }
                

                if (toId.Equals("no"))
                {
                    // this is a query then register
                    if (SIServer.dic.ContainsKey(id))
                    {
                        Client client = SIServer.dic[id];
                        if (client.IsLive) client.CleanUp();
                    }

                    SIServer.dic[id] = this;
                }
                else
                {
                    // this is a send
                    bool send = false;
                    if (SIServer.dic.ContainsKey(toId))
                    {
                        Client client = SIServer.dic[toId];
                        if (client.IsLive)
                        {
                            Response.GetHtmlResponse(id + '|' + command).SendResponse(client.stream, client);
                            client.CleanUp();

                            SIServer.dic.TryRemove(toId, out client);

                            Response.GetHtmlResponse("sent").SendResponse(stream, this);
                            send = true;
                        }
                    }

                    if (!send)
                    {
                        Response.GetHtmlResponse("fail").SendResponse(stream, this);
                    }

                    CleanUp();
                }

                request = null;
            }
        }

        public void Send(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            int offset = 0;
            int len = 0;
            // Console.WriteLine("Sending: " + bytes.Length);
            while (offset < bytes.Length)
            {
                offset = Math.Min(offset, bytes.Length - 1);
                len = Math.Min(1024, bytes.Length - offset);

                stream.Write(bytes, offset, len);

                offset += 1024;
            }
            // Console.WriteLine("Done.");
        }

        public void CleanUp()
        {
            IsLive = false;

            try
            {
                stream.Close();
            }
            catch { }
            try
            {
                stream.Dispose();
            }
            catch { }
            try
            {
                client.Close();
            }
            catch { }

            stream = null;
            client = null;
        }
    }
}