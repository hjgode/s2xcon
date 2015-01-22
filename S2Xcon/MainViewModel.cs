using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace S2Xcon
{
    class MainViewModel
    {
        bool IsBrowseMode = false; //we are in standalone usage
        public MainViewModel()
        {

        }
        private string GetBrowseDataXML(string sFileName)
        {
            if (!string.IsNullOrEmpty(sFileName))
            {
                return System.IO.File.ReadAllText(sFileName);
            }
            return null;
        }

        //see MainViewModel.cs
        public string GetBarCodeData(string sFileName)
        {
            string text = string.Empty;
            if (this.IsBrowseMode)
            {
                text = this.GetBrowseDataXML(sFileName);    //read xml input using file explorer
                System.Diagnostics.Debug.WriteLine("GetBrowseData(): " + text);

            }
            else
            {
                if (!Common.UsingJson) //true for JSON / Android download only
                {
                    //using (LinkedList<System.Windows.Controls.UserControl>.Enumerator enumerator = this.PageList.GetEnumerator())
                    //{
                    //    int i = 0;
                    //    while (enumerator.MoveNext())
                    //    {
                    //        System.Windows.Controls.UserControl current = enumerator.Current;
                    //        if (current is IPage)
                    //        {
                    //            IPage page = current as IPage;
                    //            text += page.PageData();
                    //            System.Diagnostics.Debug.WriteLine("page " + i.ToString() + ": " + text);
                    //        }
                    //    }
                    //    goto IL_12B;
                    //}
                }
                List<string> list = new List<string>();
                string text2 = GetBarCodeData(sFileName);
                try
                {
                    JObject.Parse(text2);
                    list.Add(text2);
                }
                catch (Exception)
                {
                    logger.add2log("excpetion in parsing JSON ");
                }

                //foreach (System.Windows.Controls.UserControl current2 in this.PageList)
                //{
                //    if (current2 is IPage)
                //    {
                //        IPage page2 = current2 as IPage;
                //        string text2 = page2.PageData();
                //        if (text2 != null && text2 != string.Empty)
                //        {
                //            try
                //            {
                //                JObject.Parse(text2);
                //                list.Add(text2);
                //            }
                //            catch (Exception)
                //            {
                //            }
                //        }
                //    }
                //}
                if (list.Count == 0)
                {
                    logger.add2log("  No data supplied.");
                    return string.Empty;
                }
                text = MainViewModel.processJsonStrings(list);
                text = Common.TrimJsonWhitespace(text);
            }
        //IL_12B:
            if (!Common.UsingJson)
            {
                text = Common.WrapXmlInDevInfo(text); //wrap xml with <DevInfo>
            }
            return text;
            // "{\"s\":{\"WiFi\":{\"base\":0,\"v\":0,\"mode\":\"full\",\"nets\":[{\"ssid\":\"\\\"MyNetworkName1\\\"\",\"pri\":3,\"hid\":0,\"auth\":\"OPEN,SHARED\",\"km\":\"NONE\",\"wepk\":[\"*\",\"\",\"\",\"\"]},{\"ssid\":\"\\\"MyNetworkName2\\\"\",\"pri\":1,\"hid\":0,\"auth\":\"\",\"km\":\"WPA_PSK\",\"pskey\":\"*\"}]}},\"v\":\"1.0\"}"
        }

        private static string processJsonStrings(List<string> listJson)
        {
            JObject jobject1 = JObject.Parse(listJson[0]);
            JObject jobject2 = jobject1["subsystems"] as JObject;
            for (int index = 1; index < listJson.Count; ++index)
            {
                JObject jobject3 = JObject.Parse(listJson[index]);
                foreach (JToken jtoken1 in jobject3.Children())
                {
                    if (jtoken1.Type == JTokenType.Property)
                    {
                        JProperty jproperty = jtoken1 as JProperty;
                        if (jproperty.Name != "subsystems")
                        {
                            if (jobject1[jproperty.Name] == null)
                                jobject1.Add(jproperty.Name, jproperty.Value);
                            else if (jproperty.Name == "c" || jproperty.Name == "r" || jproperty.Name == "w")
                            {
                                JToken jtoken2 = jobject1[jproperty.Name];
                                if (jtoken2 is JArray)
                                {
                                    JArray jarray1 = jtoken2 as JArray;
                                    JArray jarray2 = jobject3[jproperty.Name] as JArray;
                                    if (jarray2.HasValues)
                                        jarray1.Add(jarray2.First);
                                }
                                else
                                {
                                    jobject1.Remove(jproperty.Name);
                                    jobject1.Add((object)jproperty);
                                }
                            }
                        }
                        else
                        {
                            JObject jobject4 = jobject3["subsystems"] as JObject;
                            if (jobject4 != null)
                            {
                                foreach (KeyValuePair<string, JToken> keyValuePair in jobject4)
                                {
                                    if (jobject2[keyValuePair.Key] == null)
                                        jobject2.Add(keyValuePair.Key, keyValuePair.Value);
                                }
                            }
                        }
                    }
                    else
                        jobject1.Add((object)jtoken1);
                }
            }
            return jobject1.ToString();
        }
    }//class
}//namespace
