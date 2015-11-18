using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox.HTTP
{
    public class Request
    {
        public string Method;
        public string Url;
        public string Version;
        public string Body;
        public int ContentLength;
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public bool HasContent()
        {
            if (Headers.ContainsKey("Content-Length"))
            {
                ContentLength = int.Parse(Headers["Content-Length"]);
                return true;
            }
            return false;
        }

        public bool VerifyAuth(Config.AuthConfig auth)
        {
            //if (auth.Type == Config.AuthType.Basic)
            //{
            //    string[] pair;
            //    if (Headers.ContainsKey("Authorization"))
            //    {
            //        pair = Headers["Authorization"].Split(' ');
            //        if (pair.Length == 2)
            //        {
            //            string temp = Util.DecodeBase64(pair[1]);
            //            if (pair[0] == "Basic" && Util.DecodeBase64(pair[1]) == auth.Username + ":" + auth.Password)
            //            {
            //                return true;
            //            }
            //            else
            //            {
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            return false;
            //        }

            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else if (auth.Type == Config.AuthType.Digest)
            //{
            //    //TODO: Handle digest authenticaion
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            return true;
        }
    }
}
