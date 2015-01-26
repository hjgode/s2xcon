using S2X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text;
using Logger;

namespace S2Xcon
{
    class Program
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("s2xcon", "0.1");
        
        [STAThread]
        static int Main(string[] args)
        {
            int iReturn=0;
            Console.WriteLine("\r\nS2Xcon "+ System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()+ " started");
            var options = new Options();
            Console.WriteLine("parsing options...");
            var parser = new CommandLine.Parser();//with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Console.WriteLine(options.GetUsage())))
            {
                iReturn = Run(options, args);
            }
            else
                iReturn = -3;
            Console.WriteLine("S2Xcon DONE with code="+iReturn.ToString());
            return iReturn;
        }

        private static int Run(Options options, string[] args)
        {
            int iReturn = -99;

            Console.WriteLine("barcode_type: {0}", options.barcode_type);
            if (options.barcode_type == null)
            {
                options.barcode_type = Options.BarcodeType.COMM.ToString();
                logger.add2log("set default barcode type=COMM");
            }
            Console.WriteLine("input file: {0} ...", options.InputFile);
            Console.WriteLine("  output file: {0}", options.OutputFile);
            Console.WriteLine("  log file: {0}", options.logfile);
            Console.WriteLine("  message: {0}", options.message);
            Console.WriteLine("  password: {0}", options.password);
            Console.WriteLine("  nostartcode: {0}", options.nostartcode);
            Console.WriteLine("  noreboot: {0}", options.noreboot);

            Console.WriteLine("  xml download URL: {0}", options.xmlDownloadURL);
            Console.WriteLine("  android download set: {0}", options.andDownloadURL);

            Console.WriteLine();

            //determine log file to use
            if (options.logfile != null)
            {
                //test
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(options.logfile)))
                {
                    logger.setLogName(options.logfile);
                }
                else
                {
                    string sP = logger.LogPath;
                    logger.setLogPath(sP);
                    logger.setLogName(options.logfile);
                }
            }
            else//NO log file is specified
            {
                //is input file defined
                if(options.InputFile!=null){
                    if(System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(options.InputFile))){
                        options.logfile=System.IO.Path.GetFileNameWithoutExtension(options.InputFile) + ".log";
                        logger.setLogName(options.logfile);
                    }
                    else
                    {
                        options.logfile = options.InputFile + ".log";
                        logger.setLogName(options.logfile);
                    }
                }
                else
                {
                    //use name of output + .log
                    options.logfile = options.OutputFile + ".log";
                    logger.setLogName(options.logfile);
                }
            }
            if (options.logfile == null)
            {
                //use default log
                options.logfile = "Default.log";
                logger.setLogName(options.logfile);
            }


            Console.WriteLine("using log file: '" + options.logfile + "'");
            logger.add2log("\r\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() +
                " +++ S2Xcon " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() + " started with: ");
            string sArg="";
            foreach (string s in args)
                sArg += s + " ";
            logger.add2log("--------------------------\r\nargs line: " + sArg + "\r\n--------------------------");
            logger.add2log(String.Format(
                "barcode type: {7}\r\n" +
                "  XML download URL: {8}\r\n" +
                "  ANDroid download set: {9}\r\n" +
                "input file: {0} ...\r\n" +
                "  output file: {1}\r\n" +
                "  log file: {2}\r\n" +
                "  message: {3}\r\n" +
                "  password: {4}\r\n" +
                "  nostartcode: {5}\r\n" +
                "  noreboot: {6}",
                options.InputFile,
                options.OutputFile,
                options.logfile,
                options.message,
                options.password,
                options.nostartcode,
                options.noreboot,
                options.barcode_type,
                options.xmlDownloadURL,
                options.andDownloadURL));

            if (options.barcode_type != null)
            {
                try
                {
                    Enum.Parse(typeof(Options.BarcodeType), options.barcode_type);
                }
                catch (Exception)
                {
                    logger.add2log("unknown type: " + options.barcode_type);
                    Console.WriteLine("unknown type: " + options.barcode_type);
                    iReturn = -12;
                    goto end_parsing;
                }
            }

            //=============== process xml download ====================
            if (options.barcode_type == Options.BarcodeType.DOWNXML.ToString())
            {
                if (options.xmlDownloadURL != null)
                {
                    Common.UsingJson = false;
                    //start barcode generation
                    DownloadOption dwnopt= new DownloadOption(options.xmlDownloadURL);
                    string text = dwnopt.getBarcodeData();

                    //prepare output pdf file name
                    string sPDFname;
                    if (options.OutputFile != null)
                    {
                        sPDFname = options.OutputFile;
                        Console.WriteLine("writing PDF to '" + sPDFname + "'");
                    }
                    else
                    {
                        logger.add2log("Missing PDF file name!");
                        iReturn = -3;
                        goto end_parsing;
                    }//OutputFile set?

                    CreateBarcodes cBarcodeDown = new CreateBarcodes(options.nostartcode, options.noreboot, text, options.message, options.password);
                    if (cBarcodeDown.Save2PDF(sPDFname)){
                        Console.WriteLine("...done");
                        iReturn = 0;
                        goto end_parsing;
                    }
                    else
                    {
                        Console.WriteLine("...failed");
                        iReturn = -1;   //error
                        goto end_parsing;

                    }
                }//xmlDownloadURL?
                else
                {
                    logger.add2log("Missing in xmlDownloadURL");
                    iReturn = -2;
                    goto end_parsing;
                }
            }//process xml download


            //============== process android download 'DOWNAND' ========================
            if (options.barcode_type == Options.BarcodeType.DOWNAND.ToString())
            {
                if (options.andDownloadURL != null)
                {
                    Common.UsingJson = true;
                    string[] parms = options.andDownloadURL.Split(new char[] { '^' });
                    if(parms.Length!=6)
                    {
                        logger.add2log("number or parms for android Download Set does not fit");
                        Console.WriteLine("number or parms for android Download Set does not fit");
                        iReturn = -4;
                        goto end_parsing;
                    }
                    //start barcode generation

                    // do we have a text file to read?
                    string sFile = parms[1];
                    if (!string.IsNullOrWhiteSpace(sFile))
                    {
                        if (System.IO.File.Exists(sFile))
                        {
                            string test = System.IO.File.ReadAllText(sFile);
                            if (Common.TestStringWithinJson(test))
                            {
                                System.IO.FileInfo fileInfo = new System.IO.FileInfo(sFile);
                                parms[1] = fileInfo.FullName;                            
                            }
                            else
                            {
                                logger.add2log("The file is unsuitable for inclusion in JSON bar codes.  It may contain characters such as brackets that cause JSON parsing failure.");
                                Console.WriteLine("The file is unsuitable for inclusion in JSON bar codes.  It may contain characters such as brackets that cause JSON parsing failure.");
                                parms[1] = null;
                                iReturn = -9;
                                goto end_parsing;
                            }
                        }
                        else
                        {
                            logger.add2log("file does not exist: " + sFile);
                            Console.WriteLine("file does not exist: " + sFile);
                            iReturn = -10;
                            goto end_parsing;
                        }
                    }
                    // LoadSoftwareFromURL^D:\svn\git\s2xconsole\sample\textfile.txt.txt^TextFileDestination^LoadUpdateFromURL^LoadTextFileFromURL^DestinationForTextFileFromURL/test
                    DownloadOption dwnopt = new DownloadOption(parms);

                    string text = dwnopt.getBarcodeData();

                    //prepare output pdf file name
                    string sPDFname;
                    if (options.OutputFile != null)
                    {
                        sPDFname = options.OutputFile;
                        Console.WriteLine("writing PDF to '" + sPDFname + "'");
                    }
                    else
                    {
                        logger.add2log("Missing PDF file name!");
                        iReturn = -3;
                        goto end_parsing;
                    }//OutputFile set?

                    CreateBarcodes cBarcodeDown = new CreateBarcodes(true, false, text, options.message, options.password);
                    if (cBarcodeDown.Save2PDF(sPDFname))
                    {
                        Console.WriteLine("...done");
                        iReturn = 0;
                        goto end_parsing;
                    }
                    else
                    {
                        Console.WriteLine("...failed");
                        iReturn = -1;   //error
                        goto end_parsing;

                    }

                }
                else
                {
                    //missong options
                    Console.WriteLine("missing android download set (-a argument)");
                    logger.add2log("missing android download set");
                    iReturn = -8;
                    goto end_parsing;
                }
            }

            //============== process input file (XML or JSON) ========================
            if (options.InputFile != null)
            {
                string sFilename = options.InputFile;
                if (System.IO.File.Exists(sFilename))
                {
                    string sPath = System.IO.Path.GetDirectoryName(sFilename);
                    if (sPath != null && sPath != "")
                    {
                        if (!sPath.EndsWith(@"\"))
                            sPath += @"\";
                    }
                    else
                    {
                        sPath = logger.LogPath;
                    }
                    // # filtering communication data only does only work within S2Xconsole of an installed SmartSystems server
                    //CreateBarcodes cBarcode = new CreateBarcodes(sFilename, false);
                    Console.WriteLine("creating page data...'"+sFilename+"'");
                    CreateBarcodes cBarcode = new CreateBarcodes(sFilename, options.message, options.password, options.nostartcode, options.noreboot);

                    //cBarcode.PrintPreview();
                    string sPDFname;
                    if (options.OutputFile != null)
                    {
                        sPDFname = options.OutputFile;
                        Console.WriteLine("writing PDF to '"+sPDFname+"'");
                        if (cBarcode.Save2PDF(sPDFname))
                            Console.WriteLine("...done");
                        else
                        {
                            Console.WriteLine("...failed");
                            iReturn = -5;
                            goto end_parsing;
                        }
                    }
                    else
                    {
                        sPDFname = sPath + System.IO.Path.GetFileNameWithoutExtension(sFilename) + ".pdf";
                        Console.WriteLine("writing PDF to '" + sPDFname + "'");
                        if(cBarcode.Save2PDF(sPDFname))
                            Console.WriteLine("...done");
                        else
                        {
                            Console.WriteLine("...failed");
                            iReturn = -6;
                            goto end_parsing;
                        }
                    }
                }//file exists
                else
                {
                    Console.WriteLine("Invalid file: '" + sFilename + "'");
                    iReturn = -7;
                    goto end_parsing;
                }//if file exists
            }

end_parsing:
            logger.add2log("\r\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() +
                " +++ S2Xcon ended with " + iReturn.ToString() + " +++");
            return iReturn;
        }
    } // end class

    class Options
    {
        public enum BarcodeType
        {
            COMM,
            DOWNXML,
            DOWNAND
        }

        [Option('t', "type", MetaValue = "STRING", HelpText = "type of barcode (COMM|DOWNXML|DOWNAND). Default=COMM. \r\n"+
            "\tCOMM is for use with a communications input file (XML or JSON format)\r\n"
            + "\tDOWNXML is to define a xml download URL, see --xmldown\r\n"
            + "\tDOWNAND is to define a set of downloads for Android, see --anddown\r\n")]
        public string barcode_type {get; set;}

        [Option('i', "input", MetaValue = "FILE", HelpText = "Input file with data to process. No default.")]
        public string InputFile { get; set; }

        [Option('o', "output", MetaValue = "FILE", HelpText = "Output FILE with processed data (default: name of input as .pdf).")]
        public string OutputFile { get; set; }

        [Option('m', "message", MetaValue = "STRING", HelpText = "Add message to print. default: 'printed from s2con'")]
        public string message { get; set; }

        [Option('p', "password", MetaValue = "STRING", HelpText = "Use password. default: '' (no password)")]
        public string password { get; set; }

        [Option('l', "logfile", MetaValue = "STRING", HelpText = "log file name. default: name of input as .log")]
        public string logfile { get; set; }

        [Option('n', "nostartcode", HelpText = "no start barcode. default: print start barcode\r\n\tobsolete for JSON input files as these do never have a start barcode")]
        public bool nostartcode { get; set; }

        [Option('r', "rebootno", HelpText = "no reboot. default: device will reboot\r\n\tobsolete for JSON input files as these do not reboot for comm settings barcodes")]
        public bool noreboot { get; set; }

        [Option('x', "xmldown", MetaValue = "STRING", HelpText = "use with -t DOWNXML. xml URL download location, ie ftp://199.64.70.66/loadurl/scanngo.xml")]
        public string xmlDownloadURL { get; set; }

        [Option('a', "anddown", MetaValue = "STRING", HelpText = "Android download set, ie \r\n"+
            "SelectedURL^TextFile^TextFileDestination^SelectedOta^SelectedTextFileFromUrl^TextFileFromUrlDestination\r\n" +
            ", where the ^ is used to separate the entries.")]
        public string andDownloadURL { get; set; }

        [ValueList(typeof(List<string>))]
        public IList<string> DefinitionFiles { get; set; }


        //
        // Marking a property of type IParserState with ParserStateAttribute allows you to
        // receive an instance of ParserState (that contains a IList<ParsingError>).
        // This is equivalent from inheriting from CommandLineOptionsBase (of previous versions)
        // with the advantage to not propagating a type of the library.
        //
        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

}
