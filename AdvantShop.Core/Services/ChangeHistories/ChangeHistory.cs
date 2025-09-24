using System;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public enum ChangeHistoryObjType
    {
        None = 0,
        Lead = 1,
        Task = 2,
        Booking = 3,
        Product = 4,
        Category = 5,
        Customer = 6
    }

    public enum ChangeHistoryParameterType
    {
        None = 0,

        SalesFunnel = 1,
        Manager = 2,
        OrderSource = 3,
        LeadItemField = 4,
        Task = 5,
        Order = 6,
        BookingItemField = 7,
        Tax = 8,
        Brand = 9,
        Currency = 10,
        Color = 11,
        Size = 12,
        CustomerGroup = 13,
        Unit = 14
    }

    /// <summary>
    /// История изменений
    /// </summary>
    public class ChangeHistory
    {
        public int Id { get; set; }

        /// <summary>
        /// Id лида/таска/тд
        /// </summary>
        public int ObjId { get; set; }

        /// <summary>
        /// Тип (лид, таск, тд)
        /// </summary>
        public ChangeHistoryObjType ObjType { get; set; }

        /// <summary>
        /// Название параметра, который поменялся
        /// </summary>
        public string ParameterName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public ChangeHistoryParameterType ParameterType { get; set; }

        public int? ParameterId { get; set; }


        public string ChangedByName { get; set; }
        public Guid? ChangedById { get; set; }

        public DateTime ModificationTime { get; set; }

        public ChangeHistory()
        {
        }

        public ChangeHistory(ChangedBy changedBy)
        {
            if (changedBy == null && CustomerContext.CurrentCustomer != null)
                changedBy = new ChangedBy(CustomerContext.CurrentCustomer);

            if (changedBy != null)
            {
                ChangedByName = changedBy.Name;
                ChangedById = changedBy.CustomerId;
                ModificationTime = changedBy.ModificationTime;
            }
        }
    }
}
