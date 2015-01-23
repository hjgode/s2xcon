disclaimer!
	This software is provided as is and there is no waranty for the function of this application nor 
	any damage possibly caused by this application.
	The application is written for the below described purpose only.

S2Xcon
	application to automate configuration barcodes for ITC devices
	creates pdf with name of input file and log file (always append content)
	The input file should be a reduced ITC backup settings file.
		Example:
		===========================================================================
		<?xml version="1.0" encoding="UTF-8" ?> 
		 <DevInfo Action="Set" Persist="true">
		 <Subsystem Name="Funk Security">
		 <Group Name="Profile" Instance="Profile_1">
  			<Field Name="SSID">SUPPORT</Field> 
  			<Field Name="8021x">None</Field>
			<Field Name="Association">WPA</Field>
			<Field Name="Encryption">TKIP</Field>
			<Field Name="PreSharedKey" Encrypt="binary.base64">dzcrvPwmAWjcJAOO75RQEQ--</Field>
		</Group>
		</Subsystem>
		<Subsystem Name="IQueue">
  			<Field Name="Associated Server IP">199.64.70.66</Field> 
		</Subsystem>
		</DevInfo>
		===========================================================================

	The intention is to use s2xcon with different xml config files and simply get the
	configuartion barcodes inside a PDF file and a log file, all with the same base name. In example:
		s2xcon -i E:\WLAN_support.xml
	creates a new PDF named
		E:\WLAN_support.pdf
	and a log file named
		E:\WLAN_support.log
	for later use.

run
	arguments

		you can use short or long args
		all args except -i are optional
		use double quotes for files and strings
		the order of args is not required

		-t STRING, --type=STRING        type of barcode (COMM|DOWNXML|DOWNAND).
										Default=COMM.

				COMM is for use with a communications input file (XML or JSON format)
				DOWNXML is to define a xml download URL, see --xmldown
				DOWNAND is to define a set of downloads for Android, see --anddown

		-i FILE, --input=FILE           Input file with data to process. No
										default.

		-o FILE, --output=FILE          Output FILE with processed data (default:
										name of input as .pdf).

		-m STRING, --message=STRING     Add message to print. default: 'printed from
										s2con'

		-p STRING, --password=STRING    Use password. default: '' (no password)

		-l STRING, --logfile=STRING     log file name. default: name of input as .log

		-n, --nostartcode               no start barcode. default: print start barcode
										obsolete for JSON input files as these do never have a start barcode

		-r, --rebootno                  no reboot. default: device will reboot
										obsolete for JSON input files as these do not reboot for comm settings barcodes

		-x, --xmldown					use with -t DOWNXML. xml URL download location, ie ftp://199.64.70.66/loadurl/scanngo.xml

		-a, --anddown					Android download set, ie 
										SelectedURL^SelectedFolder^TextFile^TextFileDestination^SelectedOta^TextFileFromUrlDestination
										(7 strings separated by the ^ symbol)
										the ^ is used to separate the entries.

		--help                          Display this help screen.

	examples
		s2xcon -i E:\WLAN_support.xml
			creates E:\WLAN_support.pdf and E:\WLAN_support.log with start barcode and reboot after scan

		s2xcon -i E:\WLAN_support.xml -o "E:\WLAN_support msg.pdf"
			creates 'E:\WLAN_support msg.pdf' and E:\WLAN_support.log with start barcode and reboot after scan

		s2xcon -i E:\WLAN_support.xml -o "E:\WLAN_support msg.pdf" -m "does no reboot" -r
			creates 'E:\WLAN_support msg.pdf' and E:\WLAN_support.log with start barcode, the message 'does no reboot' and does no reboot after scan

		s2xcon -i E:\WLAN_support.xml -l "E:\s2con.log.txt" -p "password"
			creates 'E:\WLAN_support.pdf'. Log msg will be appended to 'E:\s2con.log.txt', the configuration will ask for the password after scan 

		s2xcon -i e:\myExample-Support.json.config  -m "wlan support" -o e:\json_comm.pdf -n -r -t COMM
			creates 'e:\json_comm.pdf'. Log msg will be appended to 'Default.log' in program dir
			-n and -r are obsolete for JSON inpit files!

		s2xcon -x ftp://199.64.70.66/loadurl/scanngo.xml  -m "xml down test" -o e:\xml_down.pdf -n -r -t DOWNXML
			create e:\xml_down.pdf with xml download url, no start barcode and reboot, logs to default log in prog dir

		s2xcon -t DOWNXML -x ftp://199.64.70.66/loadurl/scanngo.xml  -m "xml down test" -o e:\xml_down.pdf -n -r -l "E:\xml_down.log.txt"
			create e:\xml_down.pdf with xml download url, no start barcode and reboot, logs to E:\xml_down.log.txt

		s2xcon -t DOWNAND -m "android down test" -o e:\and_down.pdf -l "E:\xml_down.log.txt" -a SelectedURL^SelectedFolder^TextFile^TextFileDestination^SelectedOta^SelectedTextFileFromUrl^TextFileFromUrlDestination
			create a barcode with all the Android options
			log to E:\xml_down.log.txt

		s2xcon -t DOWNAND -m "android down test" -o e:\and_down.pdf -l "E:\xml_down.log.txt" -a SelectedURL^SelectedFolder^^^SelectedOta^SelectedTextFileFromUrl^TextFileFromUrlDestination
			create a barcode with all the Android options except a TextFile (a JSON file to include) and the destination for the text file
			log to E:\xml_down.log.txt
		
		-t DOWNAND -m "android down test" -o e:\and_down.pdf -l "E:\xml_down.log.txt" -a LoadSoftwareFromURL^D:\svn\git\s2xconsole\sample\textfile.txt.txt^TextFileDestination^LoadUpdateFromURL^LoadTextFileFromURL^DestinationForTextFileFromURL/test

return codes
	s2xcon returns exit code 0 if no error. Exit codes can be used in batch files to verify the result.

	other exit codes
		-1 means error in xml, see log for details
		-2 means input file is not found
		-3 error in command arguments

sample run screen
	===========================================================================
	S2Xcon.exe -i e:\WLAN_support.xml -m "wlan support" -o e:\wlan_supp_msg.pdf

	S2Xcon started
	parsing options...
	input file: e:\WLAN_support.xml ...
	  output file: e:\wlan_supp_msg.pdf
	  log file:
	  message: wlan support
	  password:
	  nostartcode: False
	  noreboot: False

	using log file: 'WLAN_support.log'
	creating page data...'e:\WLAN_support.xml'
	writing PDF to 'e:\wlan_supp_msg.pdf'
	...done
	S2Xcon DONE with code=0
	===========================================================================
	
sample log
	===========================================================================
	09.01.2015 14:41 +++ S2Xcon started with
	input file: e:\WLAN_support.xml ...
	  output file: e:\wlan_supp_msg.pdf
	  log file: 
	  message: wlan support
	  password: 
	  nostartcode: False
	  noreboot: False
	existing file 'e:\wlan_supp_msg.pdf' deleted
	S2X: version=1
	S2X: IsNoReboot=False, IsNoStartBarcode=False
	S2X: IsNoReboot=False, IsNoStartBarcode=False
	PDF will be Estimated barcodes 2 pages
	writing temp file
	getting PDF417
	S2X PDF417 done
	PDF stream generated
	PDF created: 'e:\wlan_supp_msg.pdf'

	09.01.2015 14:41 +++ S2Xcon ended with 0 +++
	===========================================================================

needed runtimes
	CommandLine.dll
	S2pHelper.dll
	S2X.dll
	Syncfusion.Compression.Base.dll
	Syncfusion.Core.dll
	Syncfusion.Pdf.Base.dll
	zlib1.dll
