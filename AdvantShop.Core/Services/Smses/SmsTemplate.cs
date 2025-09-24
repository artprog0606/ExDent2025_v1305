namespace AdvantShop.Core.Services.Smses
{
    public class SmsTemplate
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class SmsTemplateParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}