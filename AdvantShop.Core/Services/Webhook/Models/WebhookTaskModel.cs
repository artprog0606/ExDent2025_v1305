using AdvantShop.Core.Services.Crm;

namespace AdvantShop.Core.Services.Webhook.Models
{
    public class WebhookTaskModel : WebhookModel
    {
        public int Id { get; set; }

        public static explicit operator WebhookTaskModel(Task task)
        {
            return new WebhookTaskModel
            {
                Id = task.Id
            };
        }
    }
}
