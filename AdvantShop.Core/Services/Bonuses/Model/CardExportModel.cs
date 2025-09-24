using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Bonuses.Model
{
    public class CardExportModel
    {
        public Guid CardId { get; set; }
        public long CardNumber { get; set; }
        public decimal BonusAmount { get; set; }
        public string GradeName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
