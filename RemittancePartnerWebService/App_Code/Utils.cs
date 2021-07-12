using System;
using System.Diagnostics;

/// <summary>
/// Summary description for Utils
/// </summary>
public class Utils
{
    public static void WriteToEventLog(string logMessage, EventLogEntryType eventLogEntryType)
    {
        
        try
        {
            if (!EventLog.SourceExists(RemittancePartnerConfiguration.ApplicationName, "."))
            {
                EventSourceCreationData eventSourceCreationData = new EventSourceCreationData(RemittancePartnerConfiguration.ApplicationName, "Application");
                EventLog.CreateEventSource(eventSourceCreationData);
            }

            using (EventLog eventLog = new EventLog("Application", ".", RemittancePartnerConfiguration.ApplicationName))
            {
                EventLogEntryCollection evec = eventLog.Entries;
                eventLog.WriteEntry(logMessage, eventLogEntryType);
            }
        }
        catch (Exception)
        {
        }
    }
}
