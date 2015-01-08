using S2X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace S2Xcon
{
    class Program
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("sampleapp", "0.1");
        
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("\r\nS2Xcon started");
            var options = new Options();
            Console.WriteLine("parsing options...");
            var parser = new CommandLine.Parser();//with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Console.WriteLine(options.GetUsage())))
            {
                Run(options);
            }
            Console.WriteLine("S2Xcon DONE");
            return;
        }

        private static void Run(Options options)
        {

            Console.WriteLine("input file: {0} ...", options.InputFile);

            Console.WriteLine("  output file: {0}", options.OutputFile);
            Console.WriteLine("  log file: {0}", options.logfile);
            Console.WriteLine("  message: {0}", options.message);
            Console.WriteLine("  password: {0}", options.password);
            Console.WriteLine("  nostartcode: {0}", options.nostartcode);
            Console.WriteLine("  noreboot: {0}", options.noreboot);

            Console.WriteLine();

            if (options.InputFile!=null)
            {
                string sFilename = options.InputFile;
                if (System.IO.File.Exists(sFilename))
                {
                    string sPath = System.IO.Path.GetDirectoryName(sFilename);
                    if (!sPath.EndsWith(@"\"))
                        sPath += @"\";

                    if(options.logfile!=null){
                        if(System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(options.logfile))){
                            logger.setLogName(options.logfile);
                            Console.WriteLine("using log file: '"+options.logfile+"'");
                        }
                    }else{
                        logger.setLogPath(sPath);
                        logger.setLogName(System.IO.Path.GetFileNameWithoutExtension(sFilename) + ".log");
                        Console.WriteLine("using log file: '"+System.IO.Path.GetFileNameWithoutExtension(sFilename) + ".log'");
                    }
                    logger.add2log("\r\n" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + 
                        " +++ S2Xcon started with");
                    logger.add2log(String.Format(
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
                        options.noreboot));

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
                        cBarcode.Save2PDF(sPDFname);
                    }
                    else
                    {
                        sPDFname = sPath + System.IO.Path.GetFileNameWithoutExtension(sFilename) + ".pdf";
                        Console.WriteLine("writing PDF to '" + sPDFname + "'");
                        cBarcode.Save2PDF(sPDFname);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid file: '" + sFilename + "'");
                }
            }

        }

    }
    class Options
    {
        [Option('i', "input", MetaValue = "FILE", Required = true, HelpText = "Input file with data to process.")]
        public string InputFile { get; set; }

        [Option('o', "output", MetaValue = "FILE", HelpText = "Output FILE with processed data (otherwise standard output).")]
        public string OutputFile { get; set; }

        [Option('m', "message", MetaValue = "STRING", HelpText = "Add message to print.")]
        public string message { get; set; }

        [Option('p', "password", MetaValue = "STRING", HelpText = "Use password")]
        public string password { get; set; }

        [Option('l', "logfile", MetaValue = "STRING", HelpText = "Use password")]
        public string logfile { get; set; }

        [Option('n', "nostartcode", HelpText = "no start barcode")]
        public bool nostartcode { get; set; }

        [Option('r', "rebootno", HelpText = "no reboot")]
        public bool noreboot { get; set; }

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
