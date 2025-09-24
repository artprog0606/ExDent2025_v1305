using System;

namespace AdvantShop.Web.Admin.ViewModels.Emailings
{
    public class ManualEmailingViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime SendTime { get; set; }
    }
}
