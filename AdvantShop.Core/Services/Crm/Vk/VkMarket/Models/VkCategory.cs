namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    /// <summary>
    /// Категория (альбом/подборка товаров в терминологии ВКонтакте)
    /// </summary>
    public class VkCategory
    {
        /// <summary>
        /// Внутренний id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id альбома/подборки в ВКонтакте
        /// </summary>
        public long VkId { get; set; }

        /// <summary>
        /// Id раздела товаров  (Гардероб, Детскиие товар и тд, чтобы не назначать каждому товару, назначаем всей категории)
        /// </summary>
        public long VkCategoryId { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }
    }
}
