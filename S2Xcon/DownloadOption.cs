using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using System.IO;

namespace S2Xcon
{
    class DownloadOption
    {
        /// <summary>
        /// XML only: where to put the device in SmartSystems console
        /// </summary>
        string SelectedFolder;
        /// <summary>
        /// XML: full download URL address, ie 
        /// </summary>
        string SelectedURL;

        //json
        bool DestinationSpecified = false;
        /// <summary>
        /// JSON: 
        /// </summary>
        string Destination = null;

        /// <summary>
        /// The location of the local text file you want to copy
        /// </summary>
        string TextFile = null;

        /// <summary>
        /// A path that is relative to the Android external storage directory. If this field is left blank, the text file is copied to the 
        /// default location: the standard download directory on the Intermec computer. Use this optional field to specify a location in 
        /// the download directory. For example, to copy the file to a subfolder named "temp," type temp. The destination folder must exist 
        /// on the mobile computer before you can download the text file.
        /// </summary>
        string TextFileDestination = null;

        /// <summary>
        /// The web server address of the over the air (OTA) file to update the operating system.
        /// </summary>
        string SelectedOta = null;

        /// <summary>
        /// The web server or FTP server address of the text file. The bar code only includes the URL of the text file and the destination 
        /// on the Intermec computer. Use this option for large text files.
        /// </summary>
        string SelectedTextFileFromUrl = null;

        /// <summary>
        /// The filename of the text file to download. This field may also include a path that is relative to the Android external storage 
        /// directory. For example, to copy the file to a subfolder named "temp," type temp/myfile.txt. The destination folder must exist 
        /// on the mobile computer before you can download the text file.
        /// </summary>
        string TextFileFromUrlDestination = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedFolder">where to put the device?</param>
        /// <param name="downloadURL">download URL, which file to load</param>
        public DownloadOption(string selectedFolder, string downloadURL)
        {
            this.SelectedFolder = selectedFolder;
            this.SelectedURL = downloadURL;
        }

        public DownloadOption(string downloadURL)
        {
            this.SelectedFolder = null;
            this.SelectedURL = downloadURL;
        }

        public DownloadOption(string[] jsonParms)
        {
            // LoadSoftwareFromURL^D:\svn\git\s2xconsole\sample\textfile.txt.txt^TextFileDestination^LoadUpdateFromURL^LoadTextFileFromURL^DestinationForTextFileFromURL/test
            this.SelectedURL = jsonParms[0];
            this.TextFile = jsonParms[1];
            this.TextFileDestination = jsonParms[2];
            this.SelectedOta = jsonParms[3];
            this.SelectedTextFileFromUrl = jsonParms[4];
            this.TextFileFromUrlDestination = jsonParms[5];
        }

        public string getBarcodeData()
        {
            string text = getPageData();

            if(Common.IsJsonString(text))
                text = Common.TrimJsonWhitespace(text);

            return text;
        }

        /// <summary>
        /// loads the selected settings of the download options dialog and creates JSON or XML data
        /// </summary>
        /// <returns></returns>
        string getPageData()
        {
            Logger.logger.add2log("DownloadOption::getPageData()");
            string text = string.Empty;
            if (!Common.UsingJson)
            {
                #region __xml_download__
                if (!string.IsNullOrEmpty(this.SelectedFolder) && string.Empty != this.SelectedFolder.Trim())
                {
                    this.SelectedFolder = this.SelectedFolder.Trim();
                    if (!this.SelectedFolder.StartsWith("\\"))
                    {
                        string selectedFolder = this.SelectedFolder;
                        this.SelectedFolder = "\\" + selectedFolder;
                    }
                    XElement xElement = new XElement("Subsystem", new object[]
					{
						new XAttribute("Name", "SS_Client"),
						new XElement("Group", new object[]
						{
							new XAttribute("Name", "Identity"),
							new XElement("Field", new object[]
							{
								new XAttribute("Name", "ConsoleFolder"),
								new XText(this.SelectedFolder)
							})
						})
					});
                    text += xElement.ToString(SaveOptions.DisableFormatting);
                }

                if (!string.IsNullOrEmpty(this.SelectedURL) && string.Empty != this.SelectedURL.Trim())
                {
                    XElement xElement2 = new XElement("Subsystem", new object[]
					{
						new XAttribute("Name", "SS_Client"),
						new XElement("Group", new object[]
						{
							new XAttribute("Name", "FileSystem"),
							new XElement("Group", new object[]
							{
								new XAttribute("Name", "Download"),
								new XElement("Field", new object[]
								{
									new XAttribute("Name", "Url"),
									new XText(this.SelectedURL)
								})
							})
						})
					});
                    text += xElement2.ToString(SaveOptions.DisableFormatting);
                }
                #endregion
            }
            else
            {
                #region __json_download__
                string json = "{'v':1.0}";
                JObject jObject = JObject.Parse(json);
                if (!string.IsNullOrEmpty(this.SelectedURL) && string.Empty != this.SelectedURL.Trim())
                {
                    if (this.DestinationSpecified && this.Destination != null && this.Destination != string.Empty)
                    {
                        JProperty jProperty = new JProperty("s", new JValue(this.SelectedURL));
                        JProperty jProperty2 = new JProperty("d", new JValue(this.Destination));
                        JObject item = new JObject(new object[]
						{
							jProperty,
							jProperty2
						});
                        JProperty content = new JProperty("c", new JArray
						{
							item
						});
                        jObject.Add(content);
                    }
                    else
                    {
                        JArray content2 = new JArray(this.SelectedURL);
                        JProperty content3 = new JProperty("r", content2);
                        jObject.Add(content3);
                    }
                }
                if (this.TextFile != null && this.TextFile.Trim() != string.Empty)
                {
                    FileInfo fileInfo = new FileInfo(this.TextFile);
                    string name;
                    if (this.TextFileDestination == null || this.TextFileDestination == string.Empty)
                    {
                        name = fileInfo.Name;
                    }
                    else
                    {
                        if (this.TextFileDestination.EndsWith("/"))
                        {
                            name = this.TextFileDestination + fileInfo.Name;
                        }
                        else
                        {
                            name = this.TextFileDestination + "/" + fileInfo.Name;
                        }
                    }
                    string value = File.ReadAllText(this.TextFile);
                    JValue content4 = new JValue(value);
                    JProperty content5 = new JProperty(name, content4);
                    JObject content6 = new JObject(content5);
                    JProperty content7 = new JProperty("w", content6);
                    jObject.Add(content7);
                }
                if (this.SelectedOta != null && this.SelectedOta.Trim() != string.Empty)
                {
                    JProperty content8 = new JProperty("o", new JValue(this.SelectedOta));
                    jObject.Add(content8);
                }
                if (this.SelectedTextFileFromUrl != null && this.SelectedTextFileFromUrl.Trim() != string.Empty)
                {
                    JArray jArray = new JArray();
                    JProperty content9 = new JProperty("c", jArray);
                    if (this.TextFileFromUrlDestination != null && this.TextFileFromUrlDestination.Trim() != string.Empty)
                    {
                        JObject jObject2 = new JObject();
                        JProperty content10 = new JProperty("s", new JValue(this.SelectedTextFileFromUrl));
                        JProperty content11 = new JProperty("d", new JValue(this.TextFileFromUrlDestination));
                        jObject2.Add(content10);
                        jObject2.Add(content11);
                        jArray.Add(jObject2);
                    }
                    else
                    {
                        jArray.Add(this.SelectedTextFileFromUrl);
                    }
                    jObject.Add(content9);
                }
                text = jObject.ToString();
                #endregion
            }
            Logger.logger.add2log("LoadSettingViewModel::PageData() return=" + text);
            return text;
        }
    }
}
