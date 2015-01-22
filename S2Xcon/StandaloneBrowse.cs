using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace S2Xcon
{
    class StandaloneBrowse
    {
        private string fileData;

        public StandaloneBrowse(string sFileName)
        {
            this.fileData = GetBrowseData(sFileName);
        }
        public string PageData()
        {
            return this.fileData;
        }
        public int Version()
        {
            if (Common.UsingJson)
            {
                return 4;
            }
            return 3;
        }
        public string GetBrowseData(string sFileName)
        {
            if (!string.IsNullOrEmpty(sFileName))
            {
                try
                {
                    string text = File.ReadAllText(sFileName);
                    bool flag1 = Common.IsJsonString(text);
                    bool flag2 = Common.IsXmlString(text);
                    string result;
                    if (flag1 || flag2)
                    {
                        string str2;
                        if (flag1)
                        {
                            //is JSON
                            if (!Common.IsJsonActionValid(text))
                            {
                                //int num = (int)MessageBox.Show("This JSON file is not valid for bar code generation since the action value is not \"set\"", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                                Logger.logger.add2log("This JSON file is not valid for bar code generation since the action value is not \"set\"");
                                return (string)null;
                            }
                            Common.UsingJson = true;
                            str2 = Common.TransformJsonText(text);
                        }
                        else
                        {
                            //is XML
                            Logger.logger.add2log(" processing as xml file");
                            Common.UsingJson = false;
                            str2 = Common.PrepareXmlContent(text);
                        }
                        //this.textBoxConfig.Text = openFileDialog.FileName;
                        Common.SettingsSourceName = new FileInfo(sFileName).Name;
                        result = text;
                        return result;
                    }
                    //int num1 = (int)MessageBox.Show("The file is not a valid JSON or XML file.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                    Logger.logger.add2log("The file is not a valid JSON or XML file.");
                    result = null;
                    return result;
                }
                catch (Exception ex)
                {
                    //int num = (int)MessageBox.Show("The file could not be read.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                    Logger.logger.add2log("The file could not be read.");
                    return (string)null;
                }
            }
            else
            {
                //int num = (int)MessageBox.Show("No file selected.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                Logger.logger.add2log("No file selected.");
                return (string)null;
            }
        }

    }
}
