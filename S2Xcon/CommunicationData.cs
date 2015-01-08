using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using S2Xcon.Properties;
using System.Reflection;

namespace S2Xcon
{
    class CommunicationData
    {
        string _settingsFile="";
        bool _isWWANExist=false;

        public bool isWWANExist{
            get{return _isWWANExist;}
            set{_isWWANExist=value;}
        }

        public int Version()
        {
            if (this._isWWANExist)
            {
                return 3;
            }
            return 1;
        }

        public CommunicationData(string sSettingsFile){
            _settingsFile=sSettingsFile;
        }

        //return the xml for the configuration
        public string PageData()
        {
            if (string.IsNullOrEmpty(_settingsFile) || !File.Exists(_settingsFile))
            {
                return string.Empty;
            }
            return this.GetSettingsData(_settingsFile);
        }

		internal string GetSettingsData(string path)
		{
			string empty;
			string str = path;
			if (!File.Exists(str))
			{
				MessageBox.Show(string.Concat("The settings file ", str, " could not be found"), "s2xcon", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return string.Empty;
			}
            //# load translatesettingsforstc.xsl from resource
			//string str1 = string.Concat("pack://application:,,/Models/", Settings.Default.TransformFile);
			//Uri uri = new Uri(str1);
            //# read translatesettingsforstc.xsl
			//StreamReader streamReader = new StreamReader(Application.GetResourceStream(uri).Stream);

            //# read translatesettingsforstc.xsl from file
            //StreamReader streamReader = new StreamReader("translatesettingsforstc.xsl");
            //# read from resource
            StreamReader streamReader = null;
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("S2Xcon.translatesettingsforstc.xsl");
                streamReader = new StreamReader(stream);                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception for '" + "translatesettingsforstc.xsl" + "'" + ex.Message);
            }


			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(streamReader.ReadToEnd()));
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(str);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				MessageBox.Show(string.Concat("Unable to load ", str, " Exception - ", exception.Message), "Exception");
				empty = string.Empty;
				return empty;
			}
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			try
			{
				xslCompiledTransform.Load(xmlTextReader, new XsltSettings(true, false), new XmlUrlResolver());
			}
			catch (XmlException xmlException)
			{
				MessageBox.Show(xmlException.Message, "Exception Loading XSLT");
				empty = string.Empty;
				return empty;
			}
			catch (XsltException xsltException)
			{
				MessageBox.Show(xsltException.Message, "Exception Loading XSLT");
				empty = string.Empty;
				return empty;
			}
			catch (Exception exception2)
			{
				MessageBox.Show(exception2.Message, "Exception Loading XSLT");
				empty = string.Empty;
				return empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings());
			try
			{
				xslCompiledTransform.Transform(xmlDocument, xmlWriter);
				xmlWriter.Close();
				XmlDocument xmlDocument1 = new XmlDocument();
				xmlDocument1.LoadXml(stringBuilder.ToString());
				if (xmlDocument1.SelectSingleNode(Settings.Default.WWANPath) == null)
				{
					this._isWWANExist = false;
				}
				else
				{
					this._isWWANExist = true;
				}
				if (xmlDocument1.DocumentElement.Name != "DevInfo")
				{
					return xmlDocument1.InnerXml;
				}
				return xmlDocument1.DocumentElement.InnerXml;
			}
			catch (XmlException xmlException2)
			{
				XmlException xmlException1 = xmlException2;
				logger.add2log(string.Concat("Exception attempting to transform data: ", xmlException1.Message) + "Transform Exception");
				xmlWriter.Close();
				empty = string.Empty;
			}
			catch (XsltException xsltException2)
			{
				XsltException xsltException1 = xsltException2;
				string empty1 = string.Empty;
				empty1 = (!xsltException1.Message.Contains("_cache") ? string.Concat("XSLT Exception attempting to transform data: ", xsltException1.Message) : "It appears that the selected backup may not have been created on or properly installed to this computer.\r\nPlease create or install the backup again before launching this tool.");
				if (xsltException1.InnerException != null)
				{
					empty1 = string.Concat(empty1, "\r\n\r\nError message: ", xsltException1.InnerException.Message);
				}
				if (xsltException1.LineNumber != 0)
				{
					empty1 = string.Concat(empty1, " Line: ", xsltException1.LineNumber);
				}
				if (xsltException1.LinePosition != 0)
				{
					empty1 = string.Concat(empty1, " Position: ", xsltException1.LinePosition);
				}
				MessageBox.Show(empty1, "Transform Exception");
				xmlWriter.Close();
				empty = string.Empty;
			}
			catch (Exception exception4)
			{
				Exception exception3 = exception4;
				MessageBox.Show(string.Concat("Exception attempting to transform data: ", exception3.Message), "Transform Exception");
				xmlWriter.Close();
				empty = string.Empty;
			}
			return empty;
		}
    }
}
