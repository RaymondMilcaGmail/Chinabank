using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace RegisterEventLogSource
{
    class Program
    {
        private static string _applicationName = ConfigurationManager.AppSettings["EventSourceName"];

        static void Main(string[] args)
        {
            if (!EventLog.SourceExists(_applicationName, "."))
            {
                EventSourceCreationData eventSourceCreationData = new EventSourceCreationData(_applicationName, "Application");
                EventLog.CreateEventSource(eventSourceCreationData);
            }
        }
    }
}
