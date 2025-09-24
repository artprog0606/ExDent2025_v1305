using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class ManagerTemplate
    {
        [TemplateDocxProperty("FirstName", LocalizeDescription = "Имя")]
        public string FirstName { get; set; }

        [TemplateDocxProperty("LastName", LocalizeDescription = "Фамилия")]
        public string LastName { get; set; }

        [TemplateDocxProperty("Position", LocalizeDescription = "Должность")]
        public string Position { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Телефон")]
        public string Phone { get; set; }

        [TemplateDocxProperty("Email", LocalizeDescription = "E-mail")]
        public string EMail { get; set; }

        public static explicit operator ManagerTemplate(Manager manager)
        {
            var template = new ManagerTemplate();

            template.FirstName = manager.FirstName;
            template.LastName = manager.LastName;
            template.Position = manager.Position;
            template.Phone = manager.Customer != null ? manager.Customer.Phone : string.Empty;
            template.EMail = manager.Email;

            return template;
        }
    }
}
