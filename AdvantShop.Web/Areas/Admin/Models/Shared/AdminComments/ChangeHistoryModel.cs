using System;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Localization;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Shared.AdminComments
{
    public class ChangeHistoryModel
    {
        public int Id { get; set; }
        public string ParameterName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedByName { get; set; }
        public DateTime ModificationTime { get; set; }
        public string ModificationTimeFormatted { get { return Culture.ConvertDate(ModificationTime); } }
    }

    public class ChangeHistoryFilter : BaseFilterModel
    {
        public int ObjId { get; set; }
        public ChangeHistoryObjType Type { get; set; }
    }
}
