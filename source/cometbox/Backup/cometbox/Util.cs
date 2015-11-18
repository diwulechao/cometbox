using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox
{
    class Util
    {
        public static string DecodeBase64(string str)
        {
            try
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
            }
            catch
            {
                return "";
            }
        }
    }
}
