using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadsKanbanModel : KanbanModel<LeadKanbanModel> 
    {

    }

    public class LeadsKanbanColumnModel : KanbanColumnModel<LeadKanbanModel>
    {
        public int? DealStatusId { get; set; }
    }

    public class LeadKanbanModel : KanbanCardModel
    {
        private const int DescriptionMaxLength = 80;

        public int Id { get; set; }

        public Guid? CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPatronymic { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerOrganization { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }

        public string FullName
        {
            get
            {
                var name = StringHelper.AggregateStrings(" ", LastName, FirstName);

                if (string.IsNullOrWhiteSpace(name) && CustomerId != null)
                {
                    name = StringHelper.AggregateStrings(" ", CustomerLastName, CustomerFirstName, CustomerPatronymic);

                    return GetFullName(name, CustomerOrganization, CustomerPhone, CustomerEmail);
                }

                return GetFullName(name, Organization, Phone, Email);
            }
        }

        private string GetFullName(string fullName, string organization, params string[] otherFields)
        {
            fullName = fullName.DefaultOrEmpty().Trim();
            organization = organization.DefaultOrEmpty().Trim();
            var result = fullName.IsNotEmpty()
                ? organization.IsNotEmpty() ? string.Format("{0} ({1})", fullName, organization) : fullName
                : organization;
            if (result.IsNullOrEmpty() && otherFields != null)
                result = otherFields.FirstOrDefault(x => x.IsNotEmpty());

            return result.IsNotEmpty() ? result : "Нет данных";
        }

        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }

        public string ManagerAvatar { get; set; }
        public string ManagerAvatarSrc
        {
            get
            {
                if (!ManagerId.HasValue)
                    return string.Empty;
                return ManagerAvatar.IsNotEmpty()
                    ? string.Format("{0}?rnd={1}", FoldersHelper.GetPath(FolderType.Avatar, ManagerAvatar, false), new Random().Next())
                    : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
            }
        }

        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted => Culture.ConvertDate(CreatedDate);

        public string CreatedDateInterval
        {
            get
            {
                TimeInterval ti;
                var datesRange = (CreatedDate - DateTime.Now).Duration();
                if (datesRange.TotalDays > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalDays), IntervalType = TimeIntervalType.Days };
                else if (datesRange.TotalHours > 1)
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalHours), IntervalType = TimeIntervalType.Hours };
                else
                    ti = new TimeInterval() { Interval = (int)Math.Ceiling(datesRange.TotalMinutes), IntervalType = TimeIntervalType.Minutes };

                return $"{ti.Interval} {ti.Numeral("минут", true)}";
            }
        }

        public string Description { get; set; }
        public string DescriptionCut
        {
            get
            {
                var desc = Description.IsNotEmpty() ? Description : LeadItemsString;
                return desc.IsNotEmpty() && desc.Length > DescriptionMaxLength 
                    ? desc.Substring(0, DescriptionMaxLength).Trim() + "..." 
                    : desc;
            }
        }

        public string LeadItemsString { get; set; }

        public float Sum { get; set; }

        public string SumFormatted => PriceFormatService.FormatPrice(Sum, Currency, true);

        public float SumTotal => PriceService.RoundPrice(Sum, baseCurrencyValue: CurrencyValue);

        public string OrderSourceName { get; set; }

        public int SalesFunnelId { get; set; }
        
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public float RoundNumbers { get; set; }
        public bool EnablePriceRounding { get; set; }

        public Currency Currency => new Currency()
        {
            Rate = CurrencyValue,
            Symbol = CurrencySymbol,
            IsCodeBefore = IsCodeBefore,
            RoundNumbers = RoundNumbers,
            EnablePriceRounding = EnablePriceRounding
        };
    }
}
