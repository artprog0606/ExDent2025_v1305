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
    public class ExportJobRunsHandler
    {
        private readonly List<JobRunsFilterResultModel> _jobRuns;
        private readonly string _fileName;

        public ExportJobRunsHandler(FilterResult<JobRunsFilterResultModel> filterResult, string fileName)
        {
            if (filterResult?.DataItems is null) return;

            _jobRuns = filterResult.DataItems;
            _fileName = fileName;
        }

        public string Execute()
        {
            if (_jobRuns is null)
                return string.Empty;

            var pathAbsolut = FoldersHelper.GetPathAbsolut(FolderType.AdminContent);
            DeleteOldFiles(pathAbsolut);

            using (var writer = new CsvWriter(new StreamWriter(pathAbsolut + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                WriteHeader(writer);

                foreach (var jobRun in _jobRuns)
                    WriteItem(writer, jobRun);
            }

            return pathAbsolut + _fileName;
        }

        private static void DeleteOldFiles(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                if (file.Contains("export_jobRuns_"))
                    File.Delete(file);
            }
        }

        private static void WriteHeader(IWriter writer)
        {
            writer.WriteField("Id");
            writer.WriteField("Name");
            writer.WriteField("Group");
            writer.WriteField("Initiator");
            writer.WriteField("Status");
            writer.WriteField("StartDate");
            writer.WriteField("EndDate");

            writer.NextRecord();
        }

        private static void WriteItem(IWriter writer, JobRunsFilterResultModel model)
        {
            writer.WriteField(model.Id);
            writer.WriteField(model.Name);
            writer.WriteField(model.Group);
            writer.WriteField(model.Initiator);
            writer.WriteField(model.Status);
            writer.WriteField(model.StartDate.ToString("yyyy-MM-dd hh:mm:ss:fff"));
            writer.WriteField(model.EndDate?.ToString("yyyy-MM-dd hh:mm:ss:fff") ?? string.Empty);

            writer.NextRecord();
        }
    }
}
