using System;

namespace AdvantShop.Web.Admin.Models.Settings.Jobs
{
    public class JobRunLogFilterResultModel
    {
        public int Id { get; set; }
        public string JobRunId { get; set; }
        public string Event { get; set; }
        public string Message { get; set; }
        public DateTime AddDate { get; set; }
        public string AddDateFormatted => AddDate.ToString("dd/MM/yyyy HH:mm:ss:fff z");
    }
}
