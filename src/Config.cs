using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hirc
{
    public class Config
    {
        public Config()
        {
            hostname = "localhost";
            port = 6667;
            nickname = "hoxily";
            password = "password";
            realname = "HIRC V4.0";
            favoritechannel = "#hirc";
            encoding = Encoding.UTF8;
        }
        public override string ToString()
        {
            string s = "";
            s += "hostname="+hostname+"\r\n";
            s += "port="+port+"\r\n";
            s += "nickname=" + nickname + "\r\n";
            s += "password=" + password + "\r\n";
            s += "realname=" + realname + "\r\n";
            s += "favoritechannel=" + favoritechannel + "\r\n";
            s += "encoding=" + encoding.WebName + "\r\n";
            return s;
        }
        private static string GetField(string[] arr, string fieldName)
        {
            string result = "";
            int index;
            foreach (string s in arr)
            {
                if (s.StartsWith(fieldName))
                {
                    index = s.IndexOf('=');
                    if (index == s.Length - 1)
                    {
                        result = "";
                    }
                    else
                    {
                        result = s.Substring(index + 1);
                    }
                    break;
                }
            }
            return result;
        }
        public static Config FromStrings(string[] strArr)
        {
            Config c = new Config();
            c.hostname = GetField(strArr, "hostname");
            c.port=Convert.ToInt32(GetField(strArr,"port"));
            c.nickname = GetField(strArr, "nickname");
            c.password = GetField(strArr, "password");
            c.realname = GetField(strArr, "realname");
            c.favoritechannel = GetField(strArr, "favoritechannel");
            c.encoding = Encoding.GetEncoding(GetField(strArr, "encoding"));
            return c;
        }
        public string hostname
        {
            get;
            set;
        }
        public int port
        {
            get;
            set;
        }
        public string nickname
        {
            get;
            set;
        }
        public string password
        {
            get;
            set;
        }
        public string realname
        {
            get;
            set;
        }
        public string favoritechannel
        {
            get;
            set;
        }
        public Encoding encoding
        {
            get;
            set;
        }
        
    }
}
