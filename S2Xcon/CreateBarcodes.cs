using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using S2X;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;
using System.IO;

using Logger;

namespace S2Xcon
{
    class CreateBarcodes
    {
        string InputData = "";
        int VersionNumber = 3;

        bool IsNoReboot = false;
        bool IsNoStartBarcode = true;
        string Instruction = "printed from s2con";
        string Password = "";
        
        string EstimatedPDF417 = "1";
        string EstimatedCode128 = "2";
        private Hashtable barcodeResult = new Hashtable();

        private Symbol _BarcodeType = Symbol.PDF417;
        public Symbol BarcodeType
        {
            get { return _BarcodeType; }
        }

        /// <summary>
        /// read file and provide converted data for call to s2x
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="sMessage"></param>
        /// <param name="sPassword"></param>
        /// <param name="bNostart"></param>
        /// <param name="bNoReboot"></param>
        public CreateBarcodes(string sFileName, string sMessage, string sPassword, bool bNostart, bool bNoReboot)
        {
            if(sMessage!=null)
                Instruction = sMessage;
            if (sPassword != null)
                Password = sPassword;
            this.IsNoReboot= bNoReboot;
            this.IsNoStartBarcode = bNostart;

            InputData = GetBarCodeData(sFileName);
        }

        bool UsingJson = false;

        public string GetBarCodeData(string sFileName)
        {
            logger.add2log("GetBarCodeData() in=" + sFileName);
            string text = string.Empty;
            text = this.GetBrowseData(sFileName);
            //empty = string.Format("<DevInfo>{0}</DevInfo>", empty);
            if (!Common.UsingJson)
            {
                text = Common.WrapXmlInDevInfo(text);
            }

            Logger.logger.add2log("MainViewModel::GetBarCodeData() out=" + text);
            logger.add2log("GetBarCodeData() out=" + text);
            return text;
        }

        /// <summary>
        /// read all input from file and perfom checks and set global flags
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        private string GetBrowseData(string sFileName)
        {
            Logger.logger.add2log("CreateBarcodes::GetBrowseData()");
            if (!string.IsNullOrEmpty(sFileName))
            {
                try
                {
                    string text = File.ReadAllText(sFileName);
                    bool flag = Common.IsJsonString(text);
                    bool flag2 = Common.IsXmlString(text);
                    string result;
                    if (flag || flag2)
                    {
                        if (flag)
                        {
                            if (!Common.IsJsonActionValid(text))
                            {
                                //MessageBox.Show("This JSON file is not valid for bar code generation since the action value is not \"set\"", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                                logger.add2log("This JSON file is not valid for bar code generation since the action value is not \"set\"");
                                result = null;
                                return result;
                            }
                            Common.UsingJson = true;
                            text = Common.TransformJsonText(text);
                            //setting defaults for JSON comm settings file
                            logger.add2log("CreateBarcodes::GetBrowseData() setting defaults for JSON, noReboot=false, noStartBarcode=true");
                            this.IsNoReboot = false;
                            this.IsNoStartBarcode = true;
                        }
                        else
                        {
                            Common.UsingJson = false;
                            text = Common.PrepareXmlContent(text);
                        }
                            
                        FileInfo fileInfo = new FileInfo(sFileName);
                        Common.SettingsSourceName = fileInfo.Name;
                        result = text;
                        return result;
                    }
                    //MessageBox.Show("The file is not a valid JSON or XML file.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                    Logger.logger.add2log("The file is not a valid JSON or XML file.");
                    result = null;
                    return result;
                }
                catch (Exception)
                {
                    //MessageBox.Show("The file could not be read.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
                    Logger.logger.add2log("The file could not be read.");
                    string result = null;
                    return result;
                }
            }
            //MessageBox.Show("No file selected.", Settings.Default.AppDisplayName, MessageBoxButton.OK, MessageBoxImage.Hand);
            logger.add2log("No file selected.");
            return null;
        }

        public bool Save2PDF(string sPDF_Filename)
        {
            bool bReturn = true;
            try
            {
                if (System.IO.File.Exists(sPDF_Filename))
                {
                    System.IO.File.Delete(sPDF_Filename);
                    logger.add2log("existing file '" + sPDF_Filename + "' deleted");
                }

                MemoryStream memoryStream = this.SaveAsPDF();
                if (memoryStream != null)
                {
                    FileStream fileStream = new FileStream(sPDF_Filename, FileMode.Create);
                    using (fileStream)
                    {
                        byte[] buffer = memoryStream.GetBuffer();
                        fileStream.Write(buffer, 0, (int)memoryStream.Length);
                    }
                    logger.add2log("PDF created: '" + sPDF_Filename + "'");
                }
                else
                {
                    logger.add2log("memoryStream=null, NO PDF created: '" + sPDF_Filename + "'");
                    bReturn = false;
                }
            }
            catch (Exception exception)
            {
                logger.add2log("NO PDF created: '" + sPDF_Filename + "'" + "\r\n" +
                    exception.Message);
                bReturn = false;
            }
            return bReturn;
        }

        MemoryStream SaveAsPDF()
        {
            MemoryStream pDF;
            S2X.S2X s2X = this.Update();
            if (s2X == null)
            {
                logger.add2log("No PDF stream from Update()");
                return null;
            }
            try
            {
                pDF = s2X.ToPDF();
                logger.add2log("PDF stream generated");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                //MessageBox.Show(string.Concat("Error generating PDF. ", exception.ToString()));
                logger.add2log(string.Concat("Error generating PDF. ", exception.ToString()));
                return null;
            }
            return pDF;
        }

        /// <summary>
        /// add xml for noReboot
        /// </summary>
        /// <returns></returns>
		private string PageData()
		{
            Logger.logger.add2log("CreateBarcodes::PageData()");
			if (!this.IsNoReboot || Common.UsingJson)
			{
                Logger.logger.add2log("CreateBarcodes::PageData() return null");
				return string.Empty;
			}
            Logger.logger.add2log("CreateBarcodes::PageData() return '" + "<Subsystem Name=\"SS_Client\"><Group Name=\"Download\"><Field Name=\"ProcessNow\">True</Field></Group></Subsystem>"+"'");
			return "<Subsystem Name=\"SS_Client\"><Group Name=\"Download\"><Field Name=\"ProcessNow\">True</Field></Group></Subsystem>";
		}

        public S2X.S2X Update()
        {
            Logger.logger.add2log("CreateBarcodes::Update()");
            S2X.S2X s2x;
            try
            {
                string text = this.PageData();
                string text2 = this.InputData;
                if (!string.IsNullOrEmpty(text) && !Common.UsingJson)
                {
                    XDocument xDocument = XDocument.Parse(text2);
                    XDocument xDocument1 = XDocument.Parse(text);
                    xDocument.Root.Add(xDocument1.Root);
                    text2 = xDocument.ToString();
                }
                this.VersionNumber = this.VersionNum();
                logger.add2log("S2X: version=" + this.VersionNumber.ToString());

                string[] names = Enum.GetNames(typeof(Symbol));
                names = new string[] { Symbol.PDF417.ToString() };  //only interested in PDF417
                for (int i = 0; i < (int)names.Length; i++)
                {
                    string str1 = names[i];
                    s2x = (!this.barcodeResult.ContainsKey(str1) ? new S2X.S2X() : this.barcodeResult[str1] as S2X.S2X);
                    
                    s2x.IsNoReboot = this.IsNoReboot;
                    s2x.IsNoStartBarcode = this.IsNoStartBarcode;
                    logger.add2log("S2X: IsNoReboot=" + this.IsNoReboot.ToString()+
                        ", IsNoStartBarcode="+this.IsNoStartBarcode.ToString());

                    Symbol symbol = (Symbol)Enum.Parse(typeof(Symbol), str1);

                    s2x.SetSourceName(Common.GetFullFooterAddition());
                    s2x.PrintPages(this.Instruction, this.Password, text2, symbol, this.VersionNumber);

                    if (symbol == Symbol.PDF417)
                    {
                        Logger.logger.add2log(string.Format(
                            "data={0}\r\n instructions={1}\r\n pass={2}\r\n version={3}\r\nCommon.UsingJson={4}\r\nSetSourceName={5}\r\nIsNoReboot={6}\r\nIsNoStartBarcode={7}",
                            text2,
                            this.Instruction,
                            this.Password,
                            this.VersionNumber,
                            Common.UsingJson,
                            Common.GetFullFooterAddition(),
                            s2x.IsNoReboot,
                            s2x.IsNoStartBarcode));
                    }

                    this.barcodeResult[str1] = s2x;
                    string str2 = string.Format("Estimated barcodes {0}", s2x.EstimatedBarcodes);
                    if (symbol == Symbol.PDF417)
                    {
                        this.EstimatedPDF417 = str2;
                    }
                    else if (symbol == Symbol.Code128)
                    {
                        this.EstimatedCode128 = str2;
                    }
                }
                logger.add2log("PDF will be " + EstimatedPDF417 + " pages");
                logger.add2log("writing temp file");
                System.IO.File.WriteAllText(System.IO.Path.GetTempFileName(), text2);
                string name = Enum.GetName(typeof(Symbol), this._BarcodeType);
                logger.add2log("getting PDF417");
                S2X.S2X item = this.barcodeResult[name] as S2X.S2X;
                logger.add2log("S2X PDF417 done");
                return item;
            }
            catch (XmlException xmlException)
            {
                Common.WriteEntryToLog(xmlException.ToString(), EventLogEntryType.Error);
                //MessageBox.Show("Invalid data", "s2xcon", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                logger.add2log("Update(): XmlException-Invalid data\r\n" + xmlException.Message);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Common.WriteEntryToLog(exception.ToString(), EventLogEntryType.Error);
                //MessageBox.Show(exception.Message, "s2xcon", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                logger.add2log("Update(): Exception-Invalid data\r\n" + exception.Message);
            }
            return null;//ERROR!
        }

        public int VersionNum()
        {
            if (!this.IsNoReboot && !this.IsNoStartBarcode)
            {
                return 1;
            }
            if (Common.UsingJson)
                return 4;
            else
                return 3;
        }
    }
}
