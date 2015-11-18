using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace SITestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(100);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:1802");

            byte[] bytes;
            using (Stream reader = File.OpenRead("Data.xml")) {
                bytes = new byte[reader.Length];
                reader.Read(bytes, 0, (int)reader.Length);
            }

            request.SendChunked = false;
            request.KeepAlive = false;
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.ContentLength = bytes.Length - 3;

            Stream stream = request.GetRequestStream();
            stream.Write(bytes, 3, bytes.Length - 3);
            
 
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(bytes));

            try {
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                StreamReader s = new StreamReader( res.GetResponseStream());
                Console.WriteLine(s.ReadToEnd());
                s.Close();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
               
            }
            stream.Close();

            Console.ReadKey();
        }
    }
}
