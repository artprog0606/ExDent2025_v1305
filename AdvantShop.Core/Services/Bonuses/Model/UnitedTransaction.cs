using System;
using AdvantShop.Core.Services.Bonuses.Model.Enums;

namespace AdvantShop.Core.Services.Bonuses.Model
{
    public class UnitedTransaction
    {
        public decimal Amount { get; set; }
        public string Basis { get; set; }
        public DateTime CreateOn { get; set; }
        public EOperationType OperationType { get; set; }
    }
}