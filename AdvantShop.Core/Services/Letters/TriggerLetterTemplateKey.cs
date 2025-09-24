using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum TriggerLetterTemplateKey
    {
        [LetterFormatKey("#GENERATED_COUPON_CODE#", Description = "Сгенерированный код купона (код генерируется каждый раз новый)")]
        GeneratedCouponCode,
        
        [LetterFormatKey("#TRIGGER_COUPON#", Description = "Сгенерированный код купона (код генерируется один раз для триггера)")]
        TriggerCouponCode,
    } 
}