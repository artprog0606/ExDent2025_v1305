using System;
using System.Collections.Generic;

namespace AdvantShop.Catalog
{
    public class DiscountByTime
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public float Discount { get; set; }
        public bool ShowPopup { get; set; }
        public string PopupText { get; set; }
        public int SortOrder { get; set; }
        private List<DayOfWeek> _daysOfWeek;
        public List<DayOfWeek> DaysOfWeek => _daysOfWeek ?? (_daysOfWeek = DiscountByTimeService.GetDiscountByTimeDaysOfWeek(Id));

        private List<DiscountByTimeCategory> _applyDiscountCategories;
        public List<DiscountByTimeCategory> ApplyDiscountCategories => _applyDiscountCategories ?? (_applyDiscountCategories = DiscountByTimeService.GetDiscountCategories(Id, applyDiscount: true));

        private List<DiscountByTimeCategory> _activeByTimeCategories;
        public List<DiscountByTimeCategory> ActiveByTimeCategories => _activeByTimeCategories ?? (_activeByTimeCategories = DiscountByTimeService.GetDiscountCategories(Id, activeByTime: true));
    }
}
