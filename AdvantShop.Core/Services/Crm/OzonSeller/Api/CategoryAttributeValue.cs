using System;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class CategoryAttributeValue
    {
        /// <summary>
        /// Идентификатор характеристики.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Значение характеристики.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// URL изображения
        /// </summary>
        public Uri Picture { get; set; }

        /// <summary>
        /// Информация о категории товаров, в которой доступна характеристика.
        /// </summary>
        public string Info { get; set; }
    }
}
