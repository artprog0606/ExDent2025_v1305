using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Landing
{
    public class SubmitFormModel
    {
        public int Id { get; set; }
        public int? BlockId { get; set; }
        public Dictionary<string, SaveFormFieldModel> Fields { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        public int? OfferId { get; set; }
        public List<int>OfferIds { get; set; }

        public float? Amount { get; set; }
        public int? ColorId { get; set; }

        public int? EntityId { get; set; }
        public string EntityType { get; set; }

        public string ButtonTitle { get; set; }
    }

    public class SaveFormFieldModel
    {
        public string Value { get; set; }

        public int Type { get; set; }
        
        /// <summary>
        /// Customer field id
        /// </summary>
        public int? ObjId { get; set; }

        public string FieldName { get; set; }
    }
}
