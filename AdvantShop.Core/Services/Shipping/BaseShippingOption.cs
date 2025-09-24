using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Orders;
using AdvantShop.Shipping.ShippingWithInterval;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdvantShop.Shipping
{
    public class BaseShippingOption : AbstractShippingOption
    {
        public BaseShippingOption()
        {
        }

        public BaseShippingOption(ShippingMethod method)
        {
            MethodId = method.ShippingMethodId;
            Name = method.Name;
            Desc = method.Description;
            DisplayCustomFields = method.DisplayCustomFields;
            DisplayIndex = method.DisplayIndex;
            IconName = method.IconFileName != null
                ? ShippingIcons.GetShippingIcon(method.ShippingType, method.IconFileName.PhotoName, method.Name)
                : null;
            ShowInDetails = method.ShowInDetails;
            ZeroPriceMessage = method.ZeroPriceMessage;
            TaxId = method.TaxId;
            PaymentMethodType = method.PaymentMethodType;
            PaymentSubjectType = method.PaymentSubjectType;
            ShippingType = method.ShippingType;
            ShippingCurrency = method.ShippingCurrency;

            TypeOfDelivery = method.TypeOfDelivery;

            if (method.UseExtracharge)
            {
                UseExtracharge = method.UseExtracharge;
                ExtrachargeInNumbers = method.ExtrachargeInNumbers;
                ExtrachargeInPercents = method.ExtrachargeInPercents;
                ExtrachargeFromOrder = method.ExtrachargeFromOrder;
            }

            if (method.UseExtraDeliveryTime)
            {
                ExtraDeliveryTime = method.ExtraDeliveryTime;
            }

            if (method.UseDeliveryInterval)
            {
                var deliveryIntervalsStr = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.DeliveryIntervals, string.Empty);
                DeliveryIntervalsStr = deliveryIntervalsStr;
                if (deliveryIntervalsStr.IsNotEmpty())
                {
                    UseDeliveryInterval = true;
                    var timezoneId = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.TimezoneId);
                    if (timezoneId != null)
                    {
                        var time = TimeZoneInfo.FindSystemTimeZoneById(timezoneId).BaseUtcOffset;
                        TimeZoneOffset = time.Hours + (float)time.Minutes / 60;
                    }
                    var minDeliveryTime = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.MinDeliveryTime, "0").TryParseInt();
                    var countVisibleDeliveryDay = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.CountVisibleDeliveryDay, "0").TryParseInt();
                    countVisibleDeliveryDay = countVisibleDeliveryDay > 0 ? countVisibleDeliveryDay - 1 : 0;
                    var countHiddenDeliveryDay = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.CountHiddenDeliveryDay, "0").TryParseInt();
                    var currentDate = ((DateTimeOffset)TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc))
                        .ToOffset(TimeSpan.FromHours(TimeZoneOffset ?? 0));
                    var orderProcessingDeadline = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.OrderProcessingDeadline).TryParseTimeSpan(true);
                    StartDateTime = currentDate.AddMinutes(minDeliveryTime);
                    var minDate = StartDateTime.AddDays(countHiddenDeliveryDay);
                    if (orderProcessingDeadline.HasValue && currentDate.TimeOfDay > orderProcessingDeadline)
                        minDate = minDate.AddDays(1);
                    MinDate = minDate.ToString("d");
                    MaxDate = currentDate.AddDays(countVisibleDeliveryDay).ToString("d");
                    DateOfDeliveryStr = MinDate;
                    ShowSoonest = method.Params.ElementOrDefault(ShippingWithIntervalTemplate.ShowSoonest).TryParseBool();
                }
            }

            Warehouses = Core.Services.Shipping.ShippingWarehouseMappingService.GetByMethod(method.ShippingMethodId);
        }

        public BaseShippingOption(ShippingMethod method, float preCost) : this (method)
        {
            PreCost = preCost;
        }

        public void UpdateFromBase(BaseShippingOption option)
        {
            this.Update(option);
            UpdateDeliveryInterval(option);
        }

        public void UpdateDeliveryInterval(BaseShippingOption option)
        {
            if (UseDeliveryInterval && option != null)
            {
                this.TimeOfDelivery = option.TimeOfDelivery;
                if (option.TimeOfDelivery == null && option.ShowSoonest)
                {
                    this.DateOfDeliveryStr = this.MinDate;
                    this.DateOfDelivery = this.MinDate.TryParseDateTime();
                    this.TimeOfDelivery = null;
                }
                else if (option.DateOfDeliveryStr != null && option.DateOfDeliveryStr.TryParseDateTime() < this.MinDate.TryParseDateTime())
                {
                    this.DateOfDeliveryStr = null;
                    this.DateOfDelivery = null;
                }
                else
                {
                    var dateOfDelivery = option.DateOfDeliveryStr?.TryParseDateTime();
                    if (dateOfDelivery?.Date >= this.StartDateTime.Date)
                    {
                        this.DateOfDelivery = dateOfDelivery;
                        this.DateOfDeliveryStr = option.DateOfDeliveryStr;
                    }
                    else if (dateOfDelivery == null)
                    {
                        this.DateOfDelivery = null;
                        this.DateOfDeliveryStr = null;
                    }
                }
            }
        }

        public override OptionValidationResult Validate()
        {
            var result = base.Validate();
            if (!result.IsValid)
                return result;

            if (UseDeliveryInterval)
                return ValidateDeliveryInterval();
            
            return result;
        }

        public virtual void Update(BaseShippingOption option)
        {

        }

        public virtual OrderPickPoint GetOrderPickPoint()
        {
            return SelectedPoint != null
                ? new OrderPickPoint()
                {
                    PickPointId = SelectedPoint.Id,
                    WarehouseIds = SelectedPoint.WarehouseId.HasValue 
                        ? new List<int> {SelectedPoint.WarehouseId.Value} 
                        : null,
                    PickPointAddress = SelectedPoint.Address,
                }
                : null;
        }

        public virtual void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {

        }

        public bool IsCustom { get; set; }

        private OptionValidationResult ValidateDeliveryInterval()
        {
            var result = new OptionValidationResult { IsValid = true };

            if (Asap is true)
            {
                if (!ShowSoonest)
                {
                    result.IsValid = false;
                    result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryIntervals.AsapUnavailable");
                }
                return result;
            }

            if (!DateOfDelivery.HasValue || TimeOfDelivery.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryIntervals.IntervalNotSelected");
                return result;
            }

            if (DateOfDelivery.Value.Date < StartDateTime.Date
                || DateOfDelivery.Value.Date < MinDate.TryParseDateTime()
                || DateOfDelivery.Value.Date > MaxDate.TryParseDateTime())
            {
                result.IsValid = false;
                result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryIntervals.SelectedDateNotAvailable");
                return result;
            }

            if (!new Regex(@"^\d{2}:\d{2}(-\d{2}:\d{2})?(\|((-)?\d*[,.]??\d*))?$").IsMatch(TimeOfDelivery))
            {
                result.IsValid = false;
                result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryIntervals.IntervalNotSelected");
                return result;
            }

            if (DateOfDelivery.Value.Date > StartDateTime.Date)
                return result;

            var selectedTimeOfDelivery = TimeOfDelivery.Split('-')[0].TryParseTimeSpan();
            if (StartDateTime.TimeOfDay >= selectedTimeOfDelivery)
            {
                result.IsValid = false;
                result.ErrorMessage = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryIntervals.SelectedIntervalNotAvailable");
                return result;
            }

            return result;
        }
    }
}