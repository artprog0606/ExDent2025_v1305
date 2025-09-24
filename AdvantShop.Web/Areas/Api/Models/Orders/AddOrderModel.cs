using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Helpers;

namespace AdvantShop.Areas.Api.Model.Orders
{
    public class AddOrderModel : IValidatableObject
    {
        public OrderModelCustomerDto OrderCustomer { get; set; }
        public List<OrderModelItemDto> OrderItems { get; set; }

        public string Number { get; set; }
        public string OrderPrefix { get; set; }
        public string OrderSource { get; set; }
        public string Currency { get; set; }

        public string CustomerComment { get; set; }
        public string AdminComment { get; set; }
        public string ShippingName { get; set; }
        public string PaymentName { get; set; }

        public float ShippingCost { get; set; }
        public float PaymentCost { get; set; }
        public float BonusCost { get; set; }
        public float OrderDiscount { get; set; }
        public float OrderDiscountValue { get; set; }

        public long? BonusCardNumber { get; set; }
        public int? LpId { get; set; }
        public int? AffiliateId { get; set; }

        public string ShippingTaxName { get; set; }
        public string TrackNumber { get; set; }

        public bool IsPaied { get; set; }

        public float? TotalWeight { get; set; }
        public float? TotalLength { get; set; }
        public float? TotalWidth { get; set; }
        public float? TotalHeight { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }

        public string OrderStatusName { get; set; }


        public bool CheckOrderItemExist { get; set; }
        public bool CheckOrderItemAvailable { get; set; }
        public bool ValidateBonusCost { get; set; }
        public string ManagerEmail { get; set; }

        public bool SkipBusinessProcessesAfterCreatingOrder { get; set; }
        public bool SkipBonusPurchase { get; set; }

        public AddOrderModel()
        {
            OrderCustomer = new OrderModelCustomerDto();
            OrderItems = new List<OrderModelItemDto>();

            CheckOrderItemExist = true;
            CheckOrderItemAvailable = true;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderCustomer == null ||
                (string.IsNullOrWhiteSpace(OrderCustomer.FirstName) && string.IsNullOrWhiteSpace(OrderCustomer.Email) &&
                 string.IsNullOrWhiteSpace(OrderCustomer.Phone) && string.IsNullOrWhiteSpace(OrderCustomer.Organization)))
            {
                yield return new ValidationResult("Заполните обязательное поле (имя, email, телефон или организацию)");
            }
            if (OrderCustomer != null && !string.IsNullOrEmpty(OrderCustomer.Email) && !ValidationHelper.IsValidEmail(OrderCustomer.Email))
            {
                yield return new ValidationResult("Поле email имеет не корректный формат");
            }
        }
    }

    public class OrderModelCustomerDto
    {
        public Guid? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
    }

    public class OrderModelItemDto
    {
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public float? Amount { get; set; }
    }
}