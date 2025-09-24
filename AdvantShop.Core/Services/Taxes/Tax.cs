//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Taxes
{
    public class TaxElement
    {
        public int TaxId { get; set; }
        public string Name { get; set; }
        public bool  Enabled { get; set; }
        public float Rate { get; set; }
        public bool ShowInPrice { get; set; }
        public TaxType TaxType { get; set; }

        public TaxElement()
        {
            ShowInPrice = true;
        }
    }
    
    public enum TaxType
    {
        /// <summary>
        /// Не указано
        /// </summary>
        [Localize("Не указано")]
        None = 0,

        /// <summary>
        /// Без НДС
        /// </summary>
        [Localize("Без НДС")]
        VatWithout = 1,

        /// <summary>
        /// НДС по ставке 0%
        /// </summary>
        [Localize("НДС по ставке 0%")]
        Vat0 = 2,

        /// <summary>
        /// НДС по ставке 10%
        /// </summary>
        [Localize("НДС по ставке 10%")]
        Vat10 = 3,

        /// <summary>
        /// НДС по ставке 18%
        /// </summary>
        [Localize("НДС по ставке 18%")]
        Vat18 = 4,

        /// <summary>
        /// НДС по ставке 20%
        /// </summary>
        [Localize("НДС по ставке 20%")]
        Vat20 = 5,

        /// <summary>
        /// НДС по ставке 5%
        /// </summary>
        [Localize("НДС по ставке 5%")]
        Vat5 = 6,

        /// <summary>
        /// НДС по ставке 7%
        /// </summary>
        [Localize("НДС по ставке 7%")]
        Vat7 = 7,

        /// <summary>
        /// Другой
        /// </summary>
        [Localize("Другой")]
        Other = 100,
    }
}