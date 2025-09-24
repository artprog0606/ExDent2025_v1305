using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Import
{
    public static class OkMarketImportState
    {
        private static readonly object SyncObject = new object();

        private static string _fileLog = "";

        private static bool _isRun = false;
        public static bool IsRun
        {
            get { return _isRun; }
        }

        private const string FileLogPath = "~/content/okmarket/";
        private const string FileLogPattern = "report_import_*.txt";

        public static void Start()
        {
            lock (SyncObject)
            {
                _isRun = true;
                _fileLog = GetDirectory() + "report_import_" + DateTime.Now.ToString("yy-MM-dd_hh-mm") + ".txt";

                if (File.Exists(_fileLog))
                    File.Delete(_fileLog);
            }
        }

        public static void Stop()
        {
            lock (SyncObject)
            {
                _isRun = false;
                _fileLog = "";
            }
        }

        public static void WriteLog(string message, params object[] p)
        {
            WriteLog(string.Format(message, p));
        }

        public static void WriteLog(string message)
        {
            lock (SyncObject)
            {
                if (!string.IsNullOrEmpty(_fileLog))
                    using (var fs = new FileStream(_fileLog, FileMode.Append, FileAccess.Write))
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                        sw.WriteLine(message);
            }
        }


        public static void DeleteExpiredLogs()
        {
            var dir = GetDirectory();
            var dirInfo = new DirectoryInfo(dir);

            var reports = dirInfo.GetFiles(FileLogPattern).OrderByDescending(x => x.CreationTime).ToList();
            if (reports.Count > 10)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    foreach (var rep in reports.Skip(10))
                        File.Delete(rep.FullName);
                });
            }
        }

        public static List<string> GetReports()
        {
            var dir = GetDirectory();
            var dirInfo = new DirectoryInfo(dir);
            var reports = dirInfo.GetFiles(FileLogPattern).OrderByDescending(x => x.CreationTime).Take(10).Select(x => x.Name).ToList();

            return reports;
        }

        private static string GetDirectory()
        {
            var dir = HostingEnvironment.MapPath(FileLogPath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }
}