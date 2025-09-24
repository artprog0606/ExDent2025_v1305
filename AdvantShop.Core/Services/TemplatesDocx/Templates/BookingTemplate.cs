using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class BookingTemplate : BaseTemplate
    {
        [TemplateDocxProperty("Id", LocalizeDescription = "Номер брони")]
        public int Id { get; set; }

        [TemplateDocxProperty("BeginDate", LocalizeDescription = "Дата и время начала брони")]
        public DateTime BeginDate { get; set; }

        [TemplateDocxProperty("EndDate", LocalizeDescription = "Дата и время начала брони")]
        public DateTime EndDate { get; set; }

        [TemplateDocxProperty("Begin Date", LocalizeDescription = "Дата начала брони")]
        public string BeginDateFormatted
        {
            get { return Culture.ConvertShortDate(BeginDate); }
        }

        [TemplateDocxProperty("End Date", LocalizeDescription = "Дата окончания брони")]
        public string EndDateFormatted
        {
            get { return Culture.ConvertShortDate(EndDate); }
        }

        [TemplateDocxProperty("Start Time", LocalizeDescription = "Время начала брони")]
        public string BeginTimeFormatted
        {
            get { return BeginDate.ToLongTimeString(); }
        }

        [TemplateDocxProperty("End Time", LocalizeDescription = "Время окончания брони")]
        public string EndTimeFormatted
        {
            get { return EndDate.ToLongTimeString(); }
        }

        [TemplateDocxProperty("Date Added", LocalizeDescription = "Дата и время оформления брони")]
        public DateTime DateAdded { get; set; }

        [TemplateDocxProperty("DateAddedFormatted", LocalizeDescription = "Дата и время оформления брони")]
        public string DateAddedFormatted
        {
            get { return Culture.ConvertDate(DateAdded); }
        }

        [TemplateDocxProperty("Affiliate", LocalizeDescription = "Филиал")]
        public string AffiliateName { get; set; }

        [TemplateDocxProperty("Resource", LocalizeDescription = "Ресурс")]
        public string ReservationResourceName { get; set; }

        [TemplateDocxProperty("Status", LocalizeDescription = "Core.Booking.Booking.Status")]
        public BookingStatus Status { get; set; }

        [TemplateDocxProperty("Customer", Type = TypeItem.InheritedFields)]
        public CustomerTemplate Customer { get; set; }

        [TemplateDocxProperty("Manager", Type = TypeItem.InheritedFields)]
        public ManagerTemplate Manager { get; set; }

        [TemplateDocxProperty("OrderSource", LocalizeDescription = "Core.Booking.Booking.OrderSource")]
        public string OrderSourceName { get; set; }

        [TemplateDocxProperty("Payed", LocalizeDescription = "Оплачена")]
        public bool Payed { get; set; }

        [TemplateDocxProperty("PaymentDate", LocalizeDescription = "Core.Booking.Booking.PaymentDate")]
        public DateTime? PaymentDate { get; set; }

        [TemplateDocxProperty("PaymentDateFormatted", LocalizeDescription = "Core.Booking.Booking.PaymentDate")]
        public string PaymentDateFormatted
        {
            get { return PaymentDate.HasValue ? Culture.ConvertDate(PaymentDate.Value) : string.Empty; }
        }

        [TemplateDocxProperty("PaymentMethodName", LocalizeDescription = "Core.Booking.Booking.PaymentName")]
        public string PaymentMethodName { get; set; }

        [TemplateDocxProperty("Currency", Type = TypeItem.InheritedFields)]
        public CurrencyTemplate CurrencyTemplate { get; set; }

        public BookingCurrency Currency { get; set; }

        [TemplateDocxProperty("Discount", LocalizeDescription = "Core.Booking.Booking.BookingDiscount")]
        public float DiscountCost { get; set; }

        //public string DiscountCostFormatted { get { return DiscountCost.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("PaymentCost", LocalizeDescription = "Core.Booking.Booking.PaymentCost")]
        public float PaymentCost { get; set; }

        [TemplateDocxProperty("PaymentCostFormatted", LocalizeDescription = "Core.Booking.Booking.PaymentCost")]
        public string PaymentCostFormatted { get { return PaymentCost.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Sum", LocalizeDescription = "Core.Booking.Booking.Sum")]
        public float Sum { get; set; }

        [TemplateDocxProperty("SumFormatted", LocalizeDescription = "Core.Booking.Booking.Sum")]
        public string SumFormatted { get { return Sum.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Amount in words", LocalizeDescription = "Сумма прописью")]
        public string SumInWords { get { return NumberToString.ConvertToString((decimal)Sum, false); } }

        [TemplateDocxProperty("Table Items", Type = TypeItem.Table, LocalizeDescription = "Услуги для табличного представления")]
        [TemplateDocxProperty("List Items", Type = TypeItem.List, LocalizeDescription = "Услуги для представления нумерованного или маркированного списка")]
        [TemplateDocxProperty("Repeat Items", Type = TypeItem.Repeat, LocalizeDescription = "Услуги для представления списком")]
        public List<BookingItemTemplate> BookingItems { get; set; }

        public static explicit operator BookingTemplate(Booking.Booking booking)
        {
            var template = new BookingTemplate
            {
                Id = booking.Id,
                BeginDate = booking.BeginDate,
                EndDate = booking.EndDate,
                DateAdded = booking.DateAdded,
                AffiliateName = booking.Affiliate != null ? booking.Affiliate.Name : string.Empty,
                ReservationResourceName = booking.ReservationResource != null ? booking.ReservationResource.Name : string.Empty,
                OrderSourceName = booking.OrderSource != null ? booking.OrderSource.Name : string.Empty,
                Status = booking.Status,
                Payed = booking.Payed,
                PaymentDate = booking.PaymentDate,
                PaymentMethodName = booking.PaymentMethodName,
                Currency = booking.BookingCurrency,
                CurrencyTemplate = (CurrencyTemplate)(Currency)booking.BookingCurrency,
                PaymentCost = booking.PaymentCost,
                DiscountCost = booking.DiscountCost,
                Sum = booking.Sum,
                BookingItems = new List<BookingItemTemplate>()
            };

            if (booking.Manager != null)
                template.Manager = (ManagerTemplate) booking.Manager;

            if (booking.Customer != null)
                template.Customer = (CustomerTemplate) booking.Customer;
            else
            {
                template.Customer = (CustomerTemplate)new Customer
                {
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    Patronymic = booking.Patronymic,
                    EMail = booking.Email,
                    Phone = booking.Phone,
                    StandardPhone = booking.StandardPhone,
                };
            }

            foreach (var item in booking.BookingItems)
            {
                var service = item.ServiceId.HasValue ? ServiceService.Get(item.ServiceId.Value) : null;

                template.BookingItems.Add(new BookingItemTemplate()
                {
                    ArtNo = item.ArtNo,
                    Name = item.Name,
                    Price = item.Price,
                    Amount = item.Amount,
                    Currency = booking.BookingCurrency,
                    Photo = service == null || string.IsNullOrEmpty(service.Image) ? null : FoldersHelper.GetPathAbsolut(FolderType.BookingService, service.Image)
                });
            }

            return template;
        }
    }

    public class BookingItemTemplate
    {
        [TemplateDocxProperty("Photo", Type = TypeItem.Image, LocalizeDescription = "Изображение услуги")]
        public string Photo { get; set; }

        [TemplateDocxProperty("ArtNo", LocalizeDescription = "Артикул услуги")]
        public string ArtNo { get; set; }

        [TemplateDocxProperty("Name", LocalizeDescription = "Название услуги")]
        public string Name { get; set; }

        [TemplateDocxProperty("Price", LocalizeDescription = "Цена")]
        public float Price { get; set; }

        public BookingCurrency Currency { get; set; }

        [TemplateDocxProperty("PriceFormatted", LocalizeDescription = "Цена")]
        public string PriceFormatted { get { return Price.RoundAndFormatPrice(Currency); } }

        [TemplateDocxProperty("Amount", LocalizeDescription = "Колличество")]
        public float Amount { get; set; }

        [TemplateDocxProperty("Sum Item", LocalizeDescription = "Сумма")]
        public string SumFormatted { get { return (Price * Amount).RoundAndFormatPrice(Currency); } }

    }
}
