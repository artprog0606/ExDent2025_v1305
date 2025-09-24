using System;

namespace AdvantShop.Web.Admin.Models.Settings.Jobs
{
    public class JobRunsFilterResultModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameFormatted =>
            Name
                .Replace("AdvantShop.Module.", string.Empty)
                .Replace("AdvantShop.Core.", string.Empty);

        public string Group { get; set; }
        public string Initiator { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateFormatted => FormatDate(StartDate);
        public DateTime? EndDate { get; set; }

        public string EndDateFormatted => EndDate.HasValue
            ? FormatDate(EndDate.Value)
            : string.Empty;

        public long ExecutionTime { get; set; }
        public bool HasLogs { get; set; }

        private static string FormatDate(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss:fff z");
    }
}
