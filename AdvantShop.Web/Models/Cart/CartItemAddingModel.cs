using System.Collections.Generic;

namespace AdvantShop.Models.Cart
{
    public class CartItemAddingModel
    {
        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public float Amount { get; set; }
        public string AttributesXml { get; set; }
        public int Payment { get; set; }

        public string Mode { get; set; }

        /// <summary>
        /// Landing id
        /// </summary>
        public int? LpId { get; set; }

        /// <summary>
        /// Landing upsell id
        /// </summary>
        public int? LpUpId { get; set; }

        /// <summary>
        /// Lannding page orderId or leadId
        /// </summary>
        public  int? LpEntityId { get; set; }

        /// <summary>
        /// Landing page type (order, lead) 
        /// </summary>
        public string LpEntityType { get; set; }

        /// <summary>
        /// Landing block id
        /// </summary>
        public int? LpBlockId { get; set; }

        /// <summary>
        /// Landing button name
        /// </summary>
        public string LpButtonName { get; set; }

        public bool? HideShipping { get; set; }

        public List<int> OfferIds { get; set; }

        public string ModeFrom { get; set; }
    }
}