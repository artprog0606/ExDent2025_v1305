using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using AdvantShop.ExportImport;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public class FileSmsLogger : ISmsLogger
    {
        private const string LogDirectoryPath = "~/app_data/SmsLogs/";
        private const string LogFileName = "LogSms_{0}.txt";
        private const int MaxFileLength = 1 * 1024 * 1024;
        private const int MaxFilesCont = 10;

        public void LogSms(SmsLog message)
        {
            new Task(() => LogEmailInternal(message)).Start();
        }

        private void LogEmailInternal(SmsLog message)
        {
            var directoryPath = HostingEnvironment.MapPath(LogDirectoryPath);
            if (directoryPath == null)
                throw new Exception("log folder not set");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var files = Directory.GetFiles(directoryPath);
            var file = files.LastOrDefault();

            for (int i=0; i < files.Count() - MaxFilesCont; i++)
            {
                File.Delete(files[i]);
            }

            if (file == null || (new FileInfo(file)).Length > MaxFileLength)
            {
                file =
                    HostingEnvironment.MapPath(LogDirectoryPath +
                                               string.Format(LogFileName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")));
            }


            if (!File.Exists(file))
            {
                using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (TextWriter tw = new StreamWriter(stream))
                using (var writer = new CsvWriter(tw, CsvConstants.DefaultCsvConfiguration))
                {
                    writer.Context.AutoMap<SmsLog>();
                    writer.WriteHeader<SmsLog>();
                }
            }

            
            using (var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (TextWriter tw = new StreamWriter(stream))
            using (var writer = new CsvWriter(tw, CsvConstants.DefaultCsvConfiguration))
            {
                writer.Context.AutoMap<SmsLog>();
                
                writer.WriteRecord(message);
            }
        }


        public List<SmsLogDto> GetSms(Guid customerid, long phone)
        {
            var list = new List<SmsLog>();

            var directoryPath = HostingEnvironment.MapPath(LogDirectoryPath);
            if (directoryPath == null)
                throw new Exception("log folder not set");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (TextReader tr = new StreamReader(stream, true))
                using (CsvReader reader = new CsvReader(tr, CsvConstants.DefaultCsvConfiguration))
                {
                    reader.Read();
                    reader.ReadHeader();
                    reader.Context.AutoMap<SmsLog>();
                    list.AddRange(reader.GetRecords<SmsLog>());
                }
            }

            return
                list.Where(x => (customerid != Guid.Empty && x.CustomerId == customerid) || x.Phone == phone)
                    .Select(x => new SmsLogDto(x.Phone, 
                                                        x.Message, 
                                                        x.CreatedOnUtc, 
                                                        Enum.TryParse(x.Status, true, out SmsStatus status) 
                                                            ? status 
                                                            : SmsStatus.Sent))
                    .ToList();
        }
    }
}
