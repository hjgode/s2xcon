using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using S2XConsole.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace S2Xcon
{
    public class Common
    {
        public const int PDF_COMPARISON_LIMIT = 4;

        private static bool inStandaloneMode=true;

        private static EventLog eventLog;

        public static string DownloadUrl
        {
            get;
            set;
        }

        public static bool IsInStandaloneMode
        {
            get
            {
                return Common.inStandaloneMode;
            }
            set
            {
                Common.inStandaloneMode = value;
            }
        }

        public static string SettingsSourceName
        {
            get;
            set;
        }

        /// <summary>
        /// true for Android Download
        /// false for config barcodes
        /// </summary>
        public static bool UsingJson
        {
            get;
            set;
        }

        static Common()
        {
            Common.inStandaloneMode = true;
        }

        public Common()
        {
        }

        //public static string GetConnectionString()
        //{
        //    string empty = string.Empty;
        //    try
        //    {
        //        string sQLInstanceKey = Settings.Default.SQLInstanceKey;
        //        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(sQLInstanceKey);
        //        string value = (string)registryKey.GetValue(Settings.Default.SQLInstance);
        //        string str = (string)registryKey.GetValue("DBName");
        //        registryKey.Close();
        //        empty = string.Format(Settings.Default.ConnectionString, value, str);
        //    }
        //    catch
        //    {
        //    }
        //    return empty;
        //}

        //public static string GetEntityConnectionString()
        //{
        //    string empty = string.Empty;
        //    try
        //    {
        //        string sQLInstanceKey = Settings.Default.SQLInstanceKey;
        //        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(sQLInstanceKey);
        //        string value = (string)registryKey.GetValue(Settings.Default.SQLInstance);
        //        string str = (string)registryKey.GetValue("DBName");
        //        registryKey.Close();
        //        empty = string.Format(Settings.Default.EntityConnectionString, value, str);
        //    }
        //    catch
        //    {
        //    }
        //    return empty;
        //}

        public static string GetFullFooterAddition()
        {
            string empty = string.Empty;
            string settingsSourceName = Common.SettingsSourceName;
            string downloadUrl = Common.DownloadUrl;
            if (downloadUrl != null)
            {
                try
                {
                    Uri uri = new Uri(downloadUrl);
                    string absolutePath = uri.AbsolutePath;
                    if (absolutePath.EndsWith("/"))
                    {
                        absolutePath = absolutePath.TrimEnd(new char[] { '/' });
                    }
                    downloadUrl = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("/") + 1);
                }
                catch (Exception exception)
                {
                }
            }
            if (string.IsNullOrEmpty(settingsSourceName) && !string.IsNullOrEmpty(downloadUrl))
            {
                empty = downloadUrl;
            }
            else if (!string.IsNullOrEmpty(settingsSourceName) && string.IsNullOrEmpty(downloadUrl))
            {
                empty = settingsSourceName;
            }
            if (!string.IsNullOrEmpty(settingsSourceName) && !string.IsNullOrEmpty(downloadUrl))
            {
                empty = string.Concat(settingsSourceName, " | ", downloadUrl);
            }
            return empty;
        }

        public static bool IsJsonActionValid(string input)
        {
            bool flag;
            try
            {
                JObject jObjects = JObject.Parse(input);
                if (jObjects["action"] == null || !(jObjects["action"].Value<string>() != "set"))
                {
                    return true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsJsonString(string input)
        {
            bool flag;
            try
            {
                JObject.Parse(input);
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsXmlFile(string filePath)
        {
            bool flag;
            try
            {
                XDocument.Load(filePath);
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsXmlString(string input)
        {
            bool flag;
            try
            {
                XDocument.Parse(input);
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        private static string oldTransformJsonText(string input)
        {
            string str = input;
            try
            {
                JObject jObjects = JObject.Parse(input);
                foreach (string str1 in new List<string>(new string[] { "result", "resultOf", "results", "uniqueID" }))
                {
                    if (jObjects[str1] == null)
                    {
                        continue;
                    }
                    jObjects.Remove(str1);
                }
                Common.renameJsonToken(jObjects, "version", "v");
                Common.renameJsonToken(jObjects, "subsystems", "s");
                str = jObjects.ToString();
            }
            catch (Exception exception)
            {
            }
            return str;
        }

        public static string PrepareXmlContent(string xml)
        {
            string str = xml;
            try
            {
                XElement xElement = XDocument.Parse(xml).XPathSelectElement("/DevInfo");
                if (xElement != null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (XElement xElement1 in xElement.Elements())
                    {
                        stringBuilder.Append(xElement1.ToString());
                    }
                    str = stringBuilder.ToString();
                }
            }
            catch (Exception exception)
            {
            }
            return str;
        }

        private static void renameJsonToken(JObject jo, string tokenName, string newTokenName)
        {
            JToken jTokens = jo.SelectToken(tokenName);
            if (jTokens != null)
            {
                JProperty jProperty = new JProperty(newTokenName, jTokens);
                jo.Remove(tokenName);
                jo.Add(jProperty);
            }
        }

        internal static bool TestStringWithinJson(string input)
        {
            bool flag;
            try
            {
                JValue jValue = new JValue(input);
                (new JObject()).Add(new JProperty("test", jValue));
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        internal static string TransformJsonText(string input)
        {
            string str = input;
            try
            {
                JObject jObjects = new JObject();
                JObject jObjects1 = JObject.Parse(input);
                JToken jTokens = jObjects1.SelectToken("subsystems");
                if (jTokens != null)
                {
                    jObjects.Add(new JProperty("s", jTokens));
                }
                JToken jTokens1 = jObjects1.SelectToken("version");
                if (jTokens1 == null)
                {
                    jObjects.Add(new JProperty("v", new JValue(1f)));
                }
                else
                {
                    jObjects.Add(new JProperty("v", jTokens1));
                }
                JToken jTokens2 = jObjects1.SelectToken("scanngo");
                if (jTokens2 != null && jTokens2.Type == JTokenType.Object)
                {
                    foreach (JToken jTokens3 in (jTokens2 as JObject).Children())
                    {
                        if (jTokens3.Type == JTokenType.Property && jObjects[(jTokens3 as JProperty).Name] != null)
                        {
                            continue;
                        }
                        jObjects.Add(jTokens3);
                    }
                }
                str = jObjects.ToString();
            }
            catch (Exception exception)
            {
            }
            return str;
        }

        public static string TrimJsonWhitespace(string input)
        {
            string str = input;
            try
            {
                JObject jObjects = JObject.Parse(input);
                str = jObjects.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]);
            }
            catch (Exception exception)
            {
                logger.add2log("exception in TrimJsonWhitespace(): " + exception.Message);
            }
            return str;
        }

        public static string WrapXmlInDevInfo(string input)
        {
            string str = input;
            try
            {
                XDocument xDocument = XDocument.Parse(input);
                XElement root = xDocument.Root;
                if (root.Name != "DevInfo")
                {
                    XElement xElement = new XElement("DevInfo");
                    root.Remove();
                    xDocument.Add(xElement);
                    xElement.Add(root);
                    str = xDocument.ToString();
                }
            }
            catch (XmlException xmlException)
            {
                str = string.Concat("<DevInfo>", input, "</DevInfo>");
            }
            catch (Exception exception)
            {
            }
            return str;
        }

        public static void WriteEntryToLog(string message, EventLogEntryType type)
        {
            try
            {
                if (Common.eventLog == null)
                {
                    string str = "SmartSystems ScanNGo";
                    string str1 = "SmartSystems (Intermec)";
                    if (!EventLog.SourceExists(str, Environment.MachineName))
                    {
                        EventLog.CreateEventSource(new EventSourceCreationData(str, str1));
                    }
                    Common.eventLog = new EventLog(str1, Environment.MachineName, str)
                    {
                        MaximumKilobytes = (long)1024
                    };
                    Common.eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
                }
                Common.eventLog.WriteEntry(message, type);
            }
            catch (ArgumentException argumentException)
            {
                if (argumentException.Message.StartsWith("Log entry string is too long"))
                {
                    Common.WriteEntryToLog(message.Substring(0, 32766), type);
                }
            }
            catch (Exception exception)
            {
            }
        }
    }
}