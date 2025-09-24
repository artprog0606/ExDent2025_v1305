using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public class FileEmailLogger : IEmailLogger
    {
        private const string LogDirectoryPath = "~/app_data/EmailLogs/";
        private const string LogFileName = "LogEmail_{0}.txt";
        private const int MaxFileLength = 1*1024*1024;
        private const int MaxFilesCont = 10;

        public void LogEmail(EmailLog email)
        {
            Task.Run(() => Log(email));
        }
        
        public List<EmailLogDto> GetEmails(Guid customerId, string email)
        {
            var list = new List<EmailLog>();

            var directoryPath = HostingEnvironment.MapPath(LogDirectoryPath);
            if (directoryPath == null)
                throw new Exception("log folder not set");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            foreach (string file in Directory.GetFiles(directoryPath))
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (TextReader tr = new StreamReader(stream, true))
                using (var reader = new CsvReader(tr, CsvConstants.DefaultCsvConfiguration))
                {
                    reader.Read();
                    reader.ReadHeader();
                    reader.Context.AutoMap<EmailLog>();
                    list.AddRange(reader.GetRecords<EmailLog>());
                }
            }

            return
                list.Where(mail =>
                        (customerId != Guid.Empty && mail.CustomerId == customerId) ||
                        email.IsNotEmpty() && mail.Email == email)
                    .Select(x => new EmailLogDto(x.CustomerId, x.Email, x.Subject, x.Body, x.CreatedOnUtc, 
                        Enum.TryParse(x.Status, true, out EmailStatus status) ? status : EmailStatus.None))
                    .ToList();
        }

        private void Log(EmailLog email)
        {
            var directoryPath = HostingEnvironment.MapPath(LogDirectoryPath);
            if (directoryPath == null)
                throw new Exception("log folder not set");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string[] files = Directory.GetFiles(directoryPath);
            string file = files.LastOrDefault();

            for (int i = 0; i < files.Count() - MaxFilesCont; i++)
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
                    writer.Context.AutoMap<EmailLog>();
                    writer.WriteHeader<EmailLog>();
                }
            }


            using (var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (TextWriter tw = new StreamWriter(stream))
            using (var writer = new CsvWriter(tw, CsvConstants.DefaultCsvConfiguration))
            {
                writer.Context.AutoMap<EmailLog>();

                writer.WriteRecord(email);
            }
        }
    }
}