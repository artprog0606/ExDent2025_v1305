using System;
using System.Collections.Generic;
using AdvantShop.App.Landing.Models.Landing;

namespace AdvantShop.App.Landing.Models
{
    public class AddBookingDto
    {
        public AddBookingDto()
        {
            SelectedServices = new List<int>();
        }

        public SubmitFormModel Form { get; set; }
        public DateTime BeginDate { get; set; }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                if (_endDate < BeginDate && _endDate.TimeOfDay == TimeSpan.Zero)
                    _endDate = _endDate.AddDays(1);
                return _endDate;
            }
            set { _endDate = value; }
        }
        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }

        public List<int> SelectedServices { get; set; }

        public int? Lid { get; set; }
        public int LpId { get; set; }
    }
}
