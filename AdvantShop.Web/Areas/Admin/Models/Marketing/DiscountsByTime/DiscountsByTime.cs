using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime
{
    public class DiscountsByTimeModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public float Percent { get; set; }
        public bool ShowPopup { get; set; }
        public string PopupText { get; set; }
        public int SortOrder { get; set; }
        public List<DayOfWeek> SelectedDays { get; set; }
        public List<int> DiscountCategories { get; set; }
        public List<int> ActiveByTimeCategories { get; set; }
    }
}
