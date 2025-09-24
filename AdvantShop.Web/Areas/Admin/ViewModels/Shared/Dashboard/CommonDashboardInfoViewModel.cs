namespace AdvantShop.Web.Admin.ViewModels.Shared.Dashboard
{
    public class CommonDashboardInfoViewModel
    {
        public bool IsSaas { get; set; }
        public int AllowedSitesCount { get; set; }
        public int CurrentSitesCount { get; set; }
        public bool AddNewAllowed => !IsSaas || CurrentSitesCount < AllowedSitesCount;
    }
}