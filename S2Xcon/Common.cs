using System;
using System.Text;
using System.Collections.Generic;

using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace S2Xcon
{
    class Common
    {
        public static bool UsingJson;

        public static bool IsJsonString(string input)
        {
#if DEBUG
            Logger.logger.add2log("IsJsonString() in=" + input);
#endif
            bool result;
            try
            {
                JObject.Parse(input);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
#if DEBUG
            Logger.logger.add2log("IsJsonString() out=" + result);
#endif
            return result;
        }
        public static bool IsJsonActionValid(string input)
        {
#if DEBUG
            Logger.logger.add2log("IsJsonActionValid()");
#endif
            try
            {
                JObject jObject = JObject.Parse(input);
                if (jObject["action"] != null)
                {
                    JToken value = jObject["action"];
                    string a = value.Value<string>();
                    if (a != "set")
                    {
                        bool result = false;
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                bool result = false;
                return result;
            }
            return true;
        }

        /// <summary>
        /// return a shorte JSON like string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string TransformJsonText(string input)
        {
#if DEBUG
            Logger.logger.add2log("TransformJsonText() in=" + input);
#endif
            string result = input;
            try
            {
                JObject jObject = new JObject();
                JObject jObject2 = JObject.Parse(input);
                JToken jToken = jObject2.SelectToken("subsystems");
                if (jToken != null)
                {
                    JProperty content = new JProperty("s", jToken);
                    jObject.Add(content);
                }
                JToken jToken2 = jObject2.SelectToken("version");
                if (jToken2 != null)
                {
                    JProperty content2 = new JProperty("v", jToken2);
                    jObject.Add(content2);
                }
                else
                {
                    JProperty content3 = new JProperty("v", new JValue(1f));
                    jObject.Add(content3);
                }
                JToken jToken3 = jObject2.SelectToken("scanngo");
                if (jToken3 != null && jToken3.Type == JTokenType.Object)
                {
                    JObject jObject3 = jToken3 as JObject;
                    foreach (JToken current in jObject3.Children())
                    {
                        if (current.Type == JTokenType.Property)
                        {
                            JProperty jProperty = current as JProperty;
                            if (jObject[jProperty.Name] != null)
                            {
                                continue;
                            }
                        }
                        jObject.Add(current);
                    }
                }
                result = jObject.ToString();
            }
            catch (Exception)
            {
            }
#if DEBUG
            Logger.logger.add2log("TransformJsonText() out=" + result);
#endif
            return result;
        }
        internal static bool TestStringWithinJson(string input)
        {
            Logger.logger.add2log("TestStringWithinJson() in=" + input);
            bool result;
            try
            {
                JValue content = new JValue(input);
                JObject jObject = new JObject();
                JProperty content2 = new JProperty("test", content);
                jObject.Add(content2);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        private static string oldTransformJsonText(string input)
        {
            Logger.logger.add2log("oldTransformJsonText() in=" + input);
            string result = input;
            try
            {
                JObject jObject = JObject.Parse(input);
                string[] collection = new string[]
				{
					"result",
					"resultOf",
					"results",
					"uniqueID"
				};
                List<string> list = new List<string>(collection);
                foreach (string current in list)
                {
                    if (jObject[current] != null)
                    {
                        jObject.Remove(current);
                    }
                }
                Common.renameJsonToken(jObject, "version", "v");
                Common.renameJsonToken(jObject, "subsystems", "s");
                result = jObject.ToString();
            }
            catch (Exception)
            {
            }
            Logger.logger.add2log("oldTransformJsonText() out=" + result);
            return result;
        }
        private static void renameJsonToken(JObject jo, string tokenName, string newTokenName)
        {
            Logger.logger.add2log("renameJsonToken()");
            JToken jToken = jo.SelectToken(tokenName);
            if (jToken != null)
            {
                JProperty content = new JProperty(newTokenName, jToken);
                jo.Remove(tokenName);
                jo.Add(content);
            }
        }
        public static string TrimJsonWhitespace(string input)
        {
#if DEBUG
            Logger.logger.add2log("TrimJsonWhitespace() in=" + input);
#endif
            string result = input;
            try
            {
                JObject jObject = JObject.Parse(input);
                result = jObject.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]);
            }
            catch (Exception)
            {
            }
#if DEBUG
            Logger.logger.add2log("TrimJsonWhitespace() out=" + result);
#endif
            return result;
        }
        public static bool IsXmlString(string input)
        {
            bool result;
            try
            {
                XDocument.Parse(input);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public static bool IsXmlFile(string filePath)
        {
            Logger.logger.add2log("IsXmlFile() in=" + filePath);
            bool result;
            try
            {
                XDocument.Load(filePath);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            Logger.logger.add2log("IsXmlFile() out=" + result);
            return result;
        }

        /// <summary>
        /// removes xml line
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string PrepareXmlContent(string xml)
        {
#if DEBUG
            Logger.logger.add2log("PrepareXmlContent() in=" + xml);
#endif
            string result = xml;
            try
            {
                XDocument node = XDocument.Parse(xml);
                XElement xElement = node.XPathSelectElement("/DevInfo");
                if (xElement != null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (XElement current in xElement.Elements())
                    {
                        stringBuilder.Append(current.ToString());
                    }
                    result = stringBuilder.ToString();
                }
            }
            catch (Exception)
            {
            }
#if DEBUG
            Logger.logger.add2log("PrepareXmlContent out=" + result);
#endif
            return result;
        }
        public static string WrapXmlInDevInfo(string input)
        {
#if DEBUG
            Logger.logger.add2log("WrapXmlInDevInfo() in=" + input);
#endif
            string result = input;
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
                    result = xDocument.ToString();
                }
            }
            catch (XmlException)
            {
#if DEBUG
                result = "<DevInfo>" + input + "</DevInfo>";
#endif
            }
            catch (Exception)
            {
            }
#if DEBUG
            Logger.logger.add2log("WrapXmlInDevInfo() out=" + result);
#endif
            return result;
        }

        public static string SettingsSourceName
        {
            get;
            set;
        }
        public static string DownloadUrl
        {
            get;
            set;
        }
        public static string GetFullFooterAddition()
        {
            string result = string.Empty;
            string settingsSourceName = Common.SettingsSourceName;
            string text = Common.DownloadUrl;
            if (text != null)
            {
                try
                {
                    Uri uri = new Uri(text);
                    string text2 = uri.AbsolutePath;
                    if (text2.EndsWith("/"))
                    {
                        text2 = text2.TrimEnd(new char[]
						{
							'/'
						});
                    }
                    text = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("/") + 1);
                }
                catch (Exception)
                {
                }
            }
            if (string.IsNullOrEmpty(settingsSourceName) && !string.IsNullOrEmpty(text))
            {
                result = text;
            }
            else
            {
                if (!string.IsNullOrEmpty(settingsSourceName) && string.IsNullOrEmpty(text))
                {
                    result = settingsSourceName;
                }
            }
            if (!string.IsNullOrEmpty(settingsSourceName) && !string.IsNullOrEmpty(text))
            {
                result = settingsSourceName + " | " + text;
            }
            return result;
        }

        private static EventLog eventLog;

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
                    Common.eventLog = new EventLog(str1, Environment.MachineName, str);
                    Common.eventLog.MaximumKilobytes = (long)1024;
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
            catch (Exception )
            {
            }
        }
    }
}
