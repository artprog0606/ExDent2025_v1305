using System.Collections.Generic;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Home
{
    public class CongratulationsDashboardModel
    {
        public bool AllDone { get; set; }
        public bool FirstVisit { get; set; }
        public string ActionText { get; set; }

        public string StoreName { get; set; }
        public int CountryId { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public int RegionId { get; set; }
        public List<SelectListItem> Regions { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        
        public List<CongratulationsDashboardStep> Steps { get; set; }
        public bool IsTest { get; set; }
    }

    public class CongratulationsDashboardStep
    {
        public int Id { get; set; }
        public bool Activated { get; set; }

        public CongratulationsDashboardStep(int id, bool activated)
        {
            Id = id;
            Activated = activated;
        }
    }
}
