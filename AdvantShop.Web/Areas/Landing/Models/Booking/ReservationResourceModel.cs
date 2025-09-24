using System;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Models
{
    public class ReservationResourceModel : IConvertibleBlockModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int AffiliateId { get; set; }
        public string Affiliate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeEnd { get; set; }
        public bool TimeEndAtNextDay { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            throw new NotImplementedException();
        }
    }
}
