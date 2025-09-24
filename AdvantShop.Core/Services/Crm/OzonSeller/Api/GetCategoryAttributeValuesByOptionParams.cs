using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class GetCategoryAttributeValuesByOptionParams
    {
        public List<AttributeOptionOld> Options { get; set; }
        public string Language { get; set; }
    }

    public class AttributeOptionOld
    {
        /// <summary>
        /// Старый идентификатор характеристики.
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Старый идентификатор справочника.
        /// </summary>
        public int OptionId { get; set; }
    }
}
