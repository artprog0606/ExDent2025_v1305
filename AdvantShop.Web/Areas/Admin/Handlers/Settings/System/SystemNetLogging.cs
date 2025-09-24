using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class SystemNetLogging
    {
        public const string DateTimeFormatFileName = "dd-MM-yyyy";
        public const string MyListenerName = "MyListener";

        public static bool? PrevLoggingEnabled;
        public static bool? PrevAutoFlush;
        public static SourceLevels? PrevWebTraceSourceLevel;
        public static string PrevWebTraceSourceTracemode;
        public static string PrevWebTraceSourceMaxdatasize;
        public static string PathFiles = Path.Combine(SettingsGeneral.AbsolutePath, "App_Data", "SystemNetLogging");

        public static TextWriterTraceListener MyListener;

        public static bool Activate()
        {
            FileHelpers.CreateDirectory(PathFiles);

            var logging = typeof(WebRequest).Assembly.GetType("System.Net.Logging");
            var isInitializedField = logging.GetField("s_LoggingInitialized", BindingFlags.NonPublic | BindingFlags.Static);
            if (!(bool)isInitializedField.GetValue(null))
            {
                //// force initialization
                HttpWebRequest.Create("http://localhost");
                var taskInitializing = Task.Factory.StartNew(() =>
                {
                    var counter = 0;
                    while (!(bool)isInitializedField.GetValue(null) && (counter++) < 30) // max 3 second
                        Task.Delay(100);

                    return (bool)isInitializedField.GetValue(null);
                });
                taskInitializing.Wait();

                if (!taskInitializing.Result)
                    return false;
            }

            var isEnabledField = logging.GetField("s_LoggingEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            var webTraceSource = (TraceSource)logging.GetField("s_WebTraceSource", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            if (!PrevLoggingEnabled.HasValue)
            {
                PrevLoggingEnabled = (bool)isEnabledField.GetValue(null);
                PrevAutoFlush = Trace.AutoFlush;
                PrevWebTraceSourceLevel = webTraceSource.Switch.Level;
                if (webTraceSource.Attributes.ContainsKey("tracemode"))
                    PrevWebTraceSourceTracemode = webTraceSource.Attributes["tracemode"];
                else
                    PrevWebTraceSourceTracemode = null;

                if (webTraceSource.Attributes.ContainsKey("maxdatasize"))
                    PrevWebTraceSourceMaxdatasize = webTraceSource.Attributes["maxdatasize"];
                else
                    PrevWebTraceSourceMaxdatasize = null;
            }

            // признак активированности 
            if (webTraceSource.Listeners[MyListenerName] == null)
            {
                MyListener = new TextWriterTraceListener(
                    Path.Combine(PathFiles,
                        string.Format("SystemNetLogging-{0}.txt", DateTime.Now.ToString(DateTimeFormatFileName))),
                    MyListenerName);
                MyListener.TraceOutputOptions = TraceOptions.ProcessId | TraceOptions.DateTime;
                webTraceSource.Listeners.Add(MyListener);
                webTraceSource.Switch.Level = SourceLevels.Verbose;

                if (webTraceSource.Attributes.ContainsKey("tracemode"))
                    webTraceSource.Attributes["tracemode"] = "protocolonly";
                else 
                    webTraceSource.Attributes.Add("tracemode", "protocolonly");

                if (webTraceSource.Attributes.ContainsKey("maxdatasize"))
                    webTraceSource.Attributes["maxdatasize"] = "262144";
                else
                    webTraceSource.Attributes.Add("maxdatasize", "262144");

                Trace.AutoFlush = true;
                isEnabledField.SetValue(null, true);
            }

            return true;
        }

        public static bool Deactivate()
        {
            var logging = typeof(WebRequest).Assembly.GetType("System.Net.Logging");
            var isEnabledField = logging.GetField("s_LoggingEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            var webTraceSource = (TraceSource)logging.GetField("s_WebTraceSource", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            // признак активированности 
            if (webTraceSource.Listeners[MyListenerName] != null)
            {
                webTraceSource.Listeners.Remove(MyListener);
                webTraceSource.Switch.Level = PrevWebTraceSourceLevel.Value;
                Trace.AutoFlush = PrevAutoFlush.Value;

                if (PrevWebTraceSourceTracemode != null)
                    webTraceSource.Attributes["tracemode"] = PrevWebTraceSourceTracemode;
                else
                    webTraceSource.Attributes.Remove("tracemode");

                if (PrevWebTraceSourceMaxdatasize != null)
                    webTraceSource.Attributes["maxdatasize"] = PrevWebTraceSourceMaxdatasize;
                else
                    webTraceSource.Attributes.Remove("maxdatasize");

                isEnabledField.SetValue(null, PrevLoggingEnabled.Value);
                MyListener.Dispose();

                return true;
            }

            return false;
        }

        public static bool IsActivated
        {
            get
            {
                var logging = typeof(WebRequest).Assembly.GetType("System.Net.Logging");
                var webTraceSource = (TraceSource)logging.GetField("s_WebTraceSource", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                return webTraceSource.Listeners[MyListenerName] != null;
            }
        }

        public static List<string> GetLogFiles()
        {
            FileHelpers.CreateDirectory(PathFiles);
            return Directory.GetFiles(PathFiles, "SystemNetLogging-*.txt").Select(Path.GetFileName).ToList();
        }

        public static void DeleteLogFile(string fileName)
        {
            FileHelpers.DeleteFile(Path.Combine(PathFiles, fileName));
        }
    }
}
