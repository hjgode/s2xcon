using System;
using System.Diagnostics;

namespace S2Xcon
{
    class Common
    {
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
            catch (Exception exception)
            {
            }
        }
    }
}
