using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Loging.Emails;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Mails.Analytics
{
    public class GetEmailingAnalyticsDto
    {
        public string LicKey { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<Guid> EmailingIds { get; set; }
    }

    public class GetEmailingWithoutAnalyticsDto
    {
        public string LicKey { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<int?> FormatIds { get; set; }
    }

    public class EmailingStatistics
    {
        public EmailingStatistics()
        {
            Statuses = new List<EmailStatusStatistics>();
        }

        public Guid? EmailingId { get; set; }
        public int Count { get; set; }
        public List<EmailStatusStatistics> Statuses { get; set; }
    }

    public class EmailStatusStatistics
    {
        public EmailStatusStatistics()
        {
            Data = new List<EmailDailyStatistics>();
        }

        [JsonProperty("status")]
        public string StatusStr { get; set; }
        public List<EmailDailyStatistics> Data { get; set; }

        [JsonIgnore]
        public EmailStatus Status
        {
            get { return StatusStr.TryParseEnum<EmailStatus>(); }
        }

        public string StatusName
        {
            get { return Status != EmailStatus.None ? Status.Localize() : StatusStr; }
        }
    }

    public class EmailDailyStatistics
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class GetSubjectWithoutEmailingDto
    {
        [Required]
        public string LicKey { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class BaseGetEmailLogsDto
    {
        [Required]
        public string LicKey { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<EmailStatus> Statuses { get; set; }
        public override int GetHashCode()
        {
            int hash = GetHash();
            return hash;
        }

        protected int GetHash()
        {
            int hash = LicKey.GetHashCode();
            hash = (hash * 17) + Page.GetHashCode();
            hash = (hash * 17) + ItemsPerPage.GetHashCode();
            
            if (DateFrom.HasValue) hash = (hash * 17) + DateFrom.GetHashCode();
            if (DateTo.HasValue) hash = (hash * 17) + DateTo.GetHashCode();
            if (!Email.IsNullOrEmpty()) hash = (hash * 17) + Email.GetHashCode();
            if (!Subject.IsNullOrEmpty()) hash = (hash * 17) + Subject.GetHashCode();

            if(Statuses != null)
            {
                hash = (hash * 17) + Statuses.Count;
                foreach(var item in Statuses)
                {
                    hash = (hash * 17) + item.GetHashCode();
                }
            }

            return hash;
        }
    }

    public class GetEmailingLogDto : BaseGetEmailLogsDto
    {
        public Guid EmailingId { get; set; }

        public override int GetHashCode()
        {
            int hash = GetHash();
            hash = (hash * 17) + EmailingId.GetHashCode();
            return hash;
        }
    }

    public class GetWithoutEmailLogsDto : BaseGetEmailLogsDto
    {
        public int? FormatId { get; set; }
        public override int GetHashCode()
        {
            int hash = GetHash();
            if (FormatId.HasValue) hash = (hash * 17) + FormatId.GetHashCode();
            return hash;
        }
    }

    public class GetAdvantShopMailDto
    {
        public string LicKey { get; set; }
        public int Id { get; set; }
    }

    public class EmailingLog
    {
        public List<AdvantShopMail> DataItems { get; set; }
        public int TotalItemsCount { get; set; }
        public int PageIndex { get; set; }
    }

    public class SubjectWithoutEmailingResultDto
    {
        public int? FormatId { get; set; }
        public string FormatName { get; set; }
        public int Count { get; set; }
    }

    public class AdvantShopMail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [JsonProperty("status")]
        public string StatusStr { get; set; }

        [JsonIgnore]
        public EmailStatus Status
        {
            get { return StatusStr.TryParseEnum<EmailStatus>(); }
        }

        public string StatusName
        {
            get { return Status != EmailStatus.None ? Status.Localize() : StatusStr; }
        }
                
        public string StatusDesc
        {
            get { return Status != EmailStatus.None ? Status.DescriptionKey() : StatusStr; }
        }

        public bool ShowSubscibeButton
        {
            get { return 
                    Status == EmailStatus.Unsubscribed ||
                    Status == EmailStatus.Error || 
                    Status == EmailStatus.SoftBounced ||
                    Status == EmailStatus.HardBounced ||
                    Status == EmailStatus.Spam; }
        }

        public string CreatedFormatted
        {
            get { return Created.ToString("dd.MM.yy HH:mm:ss"); }
        }

        public AdvantShopEmailErrorStatus? ErrorStatusName { get; set; }
        public string ErrorStatusDescription { get { return ErrorStatusName != null ? ErrorStatusName.Localize() : null; } }

    }
}
