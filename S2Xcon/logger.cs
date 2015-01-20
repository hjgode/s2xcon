using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S2Xcon
{
    public class logger
    {
        static string _LogPath = "";
        static string LogPath
        {
            get
            {
                if (_LogPath != "")
                    return _LogPath;
                else
                {
                    string s;
                    s = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    if (s.ToLower().StartsWith("file"))
                    {
                        Uri uri = new Uri(s);
                        s = uri.AbsolutePath;
                    }
                    if (!s.EndsWith(@"\"))
                        s += @"\";
                    _LogPath = s;
                    return _LogPath;
                }
            }
        }
        static string _LogFile = "default.log";

        public static void setLogPath(string s)
        {
            _LogPath = s;
        }
        public static void setLogName(string s)
        {
            _LogFile = s;
        }
        public static void add2log(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            
            System.IO.StreamWriter sw = new System.IO.StreamWriter(LogPath + _LogFile, true);
            sw.WriteLine(s);
            sw.Flush();
            sw.Close();
        }
    }
}
