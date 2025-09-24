//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.TemplatesDocx
{
    public class TemplateDocx
    {
        public int Id { get; set; }
        public virtual TemplateDocxType Type { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public int SortOrder { get; set; }
        public long FileSize { get; set; }
        public bool DebugMode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }


    public class BookingTemplateDocx : TemplateDocx
    {
        public override TemplateDocxType Type
        {
            get { return TemplateDocxType.Booking; }
        }
    }

    public class OrderTemplateDocx : TemplateDocx
    {
        public override TemplateDocxType Type
        {
            get { return TemplateDocxType.Order; }
        }
    }

    public enum TemplateDocxType
    {
        None = 0,
        [Localize("Core.TemplatesDocx.TemplateDocxType.Booking")]
        Booking = 1,
        [Localize("Core.TemplatesDocx.TemplateDocxType.Order")]
        Order = 2
    }
}
