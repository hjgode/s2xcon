using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logger
{
    public class logger
    {
        static string _LogPath = "";
        public static string LogPath
        {
            get
            {
                if (_LogPath != "")
                    return _LogPath;
                else
                {
                    string s;
                    //s = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    s = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    //if (s.ToLower().StartsWith("file"))
                    //{
                    //    Uri uri = new Uri(s);
                    //    s = uri.AbsolutePath;
                    //    if (!s.EndsWith("/"))
                    //        s += "/";
                    //}
                    if (!s.EndsWith(@"\") && !s.EndsWith("/"))
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
            if (System.IO.Path.GetFileName(s) != s)
            {
                //split path
                string sPath = System.IO.Path.GetDirectoryName(s);
                logger.setLogPath(sPath);
                string sNameOnly = System.IO.Path.GetFileName(s);
                _LogFile = sNameOnly;
            }
            else
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

        public static void logInput(string sData, string sMessage, string sPassword, bool bNostart, bool bNoReboot, int iVersion)
        {
            string sLogStr = "logInput: " + sData + "|" +
                sMessage + "|" +
                sPassword + "|" +
                bNostart.ToString() + "|" +
                bNoReboot.ToString()+"|"+
                iVersion.ToString();

            System.Diagnostics.Debug.WriteLine(sLogStr);
            string sPath;
            sPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (sPath.ToLower().StartsWith("file"))
            {
                Uri uri = new Uri(sPath);
                sPath = uri.AbsolutePath;
            }
            if (!sPath.EndsWith(@"\"))
                sPath += @"\";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath + "inputstr.txt", false);
            sw.WriteLine(sLogStr);
            sw.Flush();
            sw.Close();
        }
    }
}
