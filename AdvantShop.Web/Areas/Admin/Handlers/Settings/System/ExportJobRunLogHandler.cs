using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Web.Admin.Models.Settings.Jobs;
using AdvantShop.Web.Infrastructure.Admin;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    internal class ExportJobRunLogHandler
    {
        private readonly List<JobRunLogFilterResultModel> _jobRunLogs;
        private readonly string _fileName;

        public ExportJobRunLogHandler(FilterResult<JobRunLogFilterResultModel> filterResult, string fileName)
        {
            if (filterResult?.DataItems is null) return;

            _jobRunLogs = filterResult.DataItems;
            _fileName = fileName;
        }

        public string Execute()
        {
            if (_jobRunLogs is null)
                return string.Empty;

            var pathAbsolut = FoldersHelper.GetPathAbsolut(FolderType.AdminContent);
            DeleteOldFiles(pathAbsolut);

            using (var writer = new CsvWriter(new StreamWriter(pathAbsolut + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                WriteHeader(writer);

                foreach (var jobRunLog in _jobRunLogs)
                    WriteItem(writer, jobRunLog);
            }

            return pathAbsolut + _fileName;
        }

        private static void DeleteOldFiles(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                if (file.Contains("export_jobRunLog_"))
                    File.Delete(file);
            }
        }

        private static void WriteHeader(IWriter writer)
        {
            writer.WriteField("Id");
            writer.WriteField("JobRunId");
            writer.WriteField("Event");
            writer.WriteField("Message");
            writer.WriteField("AddDate");

            writer.NextRecord();
        }

        private static void WriteItem(IWriter writer, JobRunLogFilterResultModel model)
        {
            writer.WriteField(model.Id);
            writer.WriteField(model.JobRunId);
            writer.WriteField(model.Event);
            writer.WriteField(model.Message);
            writer.WriteField(model.AddDate.ToString("yyyy-MM-dd hh:mm:ss:fff"));

            writer.NextRecord();
        }
    }
}
