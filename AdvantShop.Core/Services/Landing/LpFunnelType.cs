using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Landing
{
    /// <summary>
    /// Тип воронки
    /// </summary>
    public enum LpFunnelType
    {
        None,
        
        /// <summary>
        /// Пустой шаблон
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Default")]
        Default,

        /// <summary>
        /// Лид магнит статья
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.LeadMagnet")]
        LeadMagnet,

        /// <summary>
        /// Лид магнит видео
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.VideoLeadMagnet")]
        VideoLeadMagnet,

        /// <summary>
        /// Товарная воронка с заявкой
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.LandingFunnel")]
        LandingFunnel,

        /// <summary>
        /// Товарная воронка с оплатой
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.LandingFunnelOrder")]
        LandingFunnelOrder,

        /// <summary>
        /// Товарная видео воронка с заявкой
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.VideoLandingFunnel")]
        VideoLandingFunnel,

        /// <summary>
        /// Товарная видео воронка с оплатой
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.VideoLandingFunnelOrder")]
        VideoLandingFunnelOrder,

        /// <summary>
        /// Товарная воронка бесплатно + доставка
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.OneProductUpSellDownSell")]
        OneProductUpSellDownSell,

        /// <summary>
        /// Мультитоварная
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.MultyProducts")]
        MultyProducts,

        /// <summary>
        /// Воронка мероприятия
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Events")]
        Events,

        /// <summary>
        /// Купонатор воронка с заявкой 
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Couponator")]
        Couponator,

        /// <summary>
        /// Купонатор воронка с заказом
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.CouponatorOrder")]
        CouponatorOrder,

        /// <summary>
        /// Эксперт
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Expert")]
        Expert,

        /// <summary>
        /// Допродажи
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.ProductCrossSellDownSell")]
        ProductCrossSellDownSell,

        /// <summary>
        /// Воронка конференции
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Conference")]
        Conference,

        /// <summary>
        /// Воронка курса
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Course")]
        Course,

        /// <summary>
        /// Воронка Консалтинговая
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.Consulting")]
        Consulting,

        /// <summary>
        /// Воронка Услуга с онлайн-записью
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.ServicesOnline")]
        ServicesOnline,

        /// <summary>
        /// Квиз
        /// </summary>
        [Localize("Landing.Domain.LpFunnelType.QuizFunnel")]
        QuizFunnel
    }


    public enum LpFunnelCategory
    {
        [Localize("Landing.Domain.LpFunnelType.Default")]
        Default,

        [Localize("Продажа товаров")]
        SalesOfGoods,

        [Localize("Сбор контактов + Продажа")]
        CollectContacts,

        [Localize("Допродажи")]
        AdditionalSales,

        [Localize("Продажа через Контент")]
        SellingThrowContent,
    }
    
    public class LpFunnelModel
    {
        public LpFunnelCategory Category { get; set; }
        public string CategoryTypeStr { get { return Category.ToString(); } }

        //public LpSiteCategory SiteCategory { get; set; }

        public string Name { get { return Category.Localize(); } }
        
        public string SvgIcon { get; set; }

        public string Photo { get; set; }

        public string Description { get; set; }

        public string Video { get; set; }
    }
}
