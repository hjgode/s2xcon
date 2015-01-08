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

namespace S2Xcon
{
    class CreateBarcodes
    {
        string InputData = "";
        int VersionNumber = 2;

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

        public CreateBarcodes(string sFileName, string sMessage, string sPassword, bool bNostart, bool bNoReboot)
        {
            if(sMessage!=null)
                Instruction = sMessage;
            if (sPassword != null)
                Password = sPassword;
            IsNoReboot = bNoReboot;
            IsNoStartBarcode = bNostart;

            InputData = GetBarCodeData(sFileName);
        }

        public string GetBarCodeData(string sFileName)
        {
            string empty = string.Empty;
            empty = this.GetBrowseData(sFileName);
            //empty = string.Format("<DevInfo>{0}</DevInfo>", empty);
            return empty;
        }

        private string GetBrowseData(string sFileName)
        {
            if (!string.IsNullOrEmpty(sFileName))
            {
                return System.IO.File.ReadAllText(sFileName);
            }
            return null;
        }

        public void PrintPreview()
        {
            List<System.Drawing.Image> imgList = this.Update().GetPages(); // (new PrintingModel()).PrintPreview(this.Update().GetPages());
            int i = 0;
            foreach (System.Drawing.Image img in imgList)
            {
                string filename="img"+i.ToString()+".gif";
                try
                {
                    System.IO.File.Delete(filename);
                }
                catch (Exception)
                {
                }
                img.Save(filename, System.Drawing.Imaging.ImageFormat.Gif);
            }
        }

        public void Save2PDF(string sPDF_Filename)
        {
            try
            {
                if (System.IO.File.Exists(sPDF_Filename))
                    System.IO.File.Delete(sPDF_Filename);
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
                    logger.add2log("memoryStream=null, NO PDF created: '" + sPDF_Filename + "'");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                //MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                logger.add2log("NO PDF created: '" + sPDF_Filename + "'" + "\r\n" +
                    exception.Message);
            }
        }

        MemoryStream SaveAsPDF()
        {
            MemoryStream pDF;
            S2X.S2X s2X = this.Update();
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

        public S2X.S2X Update()
        {
            S2X.S2X s2xPages;
            try
            {
                string str = "";
                string inputData = this.InputData;//the xml to convert
                if (!string.IsNullOrEmpty(str))
                {
                    XDocument xDocument = XDocument.Parse(inputData);
                    XDocument xDocument1 = XDocument.Parse(str);
                    xDocument.Root.Add(xDocument1.Root);
                    inputData = xDocument.ToString();
                }
                this.VersionNumber = this.VersionNum();

                string[] names = Enum.GetNames(typeof(Symbol));
                for (int i = 0; i < (int)names.Length; i++)
                {
                    string str1 = names[i];
                    s2xPages = (!this.barcodeResult.ContainsKey(str1) ? new S2X.S2X() : this.barcodeResult[str1] as S2X.S2X);
                    
                    s2xPages.IsNoReboot = this.IsNoReboot;
                    s2xPages.IsNoStartBarcode = this.IsNoStartBarcode;

                    Symbol symbol = (Symbol)Enum.Parse(typeof(Symbol), str1);

                    s2xPages.PrintPages(this.Instruction, this.Password, inputData, symbol, this.VersionNumber);
                    
                    this.barcodeResult[str1] = s2xPages;
                    string str2 = string.Format("Estimated barcodes {0}", s2xPages.EstimatedBarcodes);
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
                System.IO.File.WriteAllText(System.IO.Path.GetTempFileName(), inputData);
                string name = Enum.GetName(typeof(Symbol), this._BarcodeType);
                S2X.S2X item = this.barcodeResult[name] as S2X.S2X;
                return item;
            }
            catch (XmlException xmlException)
            {
                Common.WriteEntryToLog(xmlException.ToString(), EventLogEntryType.Error);
                //MessageBox.Show("Invalid data", "s2xcon", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                logger.add2log("Update(): Invalid data\r\n"+xmlException.Message);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Common.WriteEntryToLog(exception.ToString(), EventLogEntryType.Error);
                //MessageBox.Show(exception.Message, "s2xcon", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                logger.add2log("Update(): Invalid data\r\n" + exception.Message);
            }
            return new S2X.S2X();//ERROR!
        }

        public int VersionNum()
        {
            if (!this.IsNoReboot && !this.IsNoStartBarcode)
            {
                return 1;
            }
            return 3;
        }
    }
}
