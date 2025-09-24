using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies
{
    public enum EMessageReplyFieldType
    {
        [Localize("Core.Crm.EMessageReplyFieldType.None"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 10)]
        None = 0,
        [Localize("Core.Crm.EMessageReplyFieldType.Vk"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 10)]
        Vk = 1,
        [Localize("Core.Crm.EMessageReplyFieldType.Facebook"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 20)]
        Facebook = 2,
        [Localize("Core.Crm.EMessageReplyFieldType.Instagram"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 30)]
        Instagram = 3,
        [Localize("Core.Crm.EMessageReplyFieldType.Telegram"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 40)]
        Telegram = 4,
        [Localize("Core.Crm.EMessageReplyFieldType.Ok"), FieldType(EFieldType.None), FieldTypeValue(EFieldType.Text, "Core.Crm.EMessageReplyFieldType.SocialGroup", 50)]
        Ok = 5
    }

    public class MessageReplyBizObj : Customer, IBizObject
    {
        public EMessageReplyFieldType Type { get; set; }

        public MessageReplyBizObj(){}

        public MessageReplyBizObj(Customer customer, EMessageReplyFieldType type)
        {
            Type = type;

            Id = customer.Id;
            InnerId = customer.InnerId;
            EMail = customer.EMail;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Patronymic = customer.Patronymic;
            Organization = customer.Organization;
            ManagerId = customer.ManagerId;
            Enabled = customer.Enabled;
            Phone = customer.Phone;
            StandardPhone = customer.StandardPhone;
        }
    }

    public class MessageReplyFieldComparer : IBizObjectFieldComparer<MessageReplyBizObj>
    {
        public EMessageReplyFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public string FieldName => FieldType.Localize();

        public string FieldValueObjectName { get; private set; }


        public bool CheckField(MessageReplyBizObj bizObject)
        {
            if (FieldType == EMessageReplyFieldType.None)
                return true;

            if (bizObject.Type != FieldType)
                return false;

            return true;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
