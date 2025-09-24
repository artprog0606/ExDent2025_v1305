using System.Collections.Generic;
using AdvantShop.Configuration;

namespace AdvantShop.ViewCommon
{

    public enum CardAddSize
    {
        XSmall = 0,
        Small = 1,
        Middle = 2,
        Big = 3,
        None = 4,
    }

    public class CartAddViewModel
    {
        public CartAddViewModel()
        {
            Classes = "btn btn-confirm icon-bag-before";
        }

        public int? OfferId { get; set; }
        public int? ProductId { get; set; }
        public float? Amount { get; set; }
        public string AttributesXml { get; set; }
        public int? Payment { get; set; }

        public bool? AllowAddProductToCart { get; set; }
        public string Mode { get; set; }

        /// <summary>
        /// Landing id
        /// </summary>
        public int? LpId { get; set; }

        /// <summary>
        /// Landing upsell id
        /// </summary>
        public string LpUpId { get; set; }

        /// <summary>
        /// Lannding page orderId or leadId
        /// </summary>
        public int? LpEntityId { get; set; }

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

        public string HideShipping { get; set; }

        public List<string> OfferIds { get; set; }

        public string ModeFrom { get; set; }


        /// <summary>
        /// add from
        /// </summary>
        public string Href { get; set; }

        public string Source { get; set; }
        public SettingsDesign.eCartAddTypeButton? CartAddType { get; set; }
        public string NgCartAddType { get; set; }

        public CardAddSize Size { get; set; } = CardAddSize.Middle;


        public string BtnContent { get; set; }
        public string StepSpinbox { get; set; }
        public string MaxStepSpinbox { get; set; }
        public string MinStepSpinbox { get; set; }


        public string NgOfferId { get; set; }
        public string NgProductId { get; set; }
        public string NgAmount { get; set; }
        public string NgAttributesXml { get; set; }
        public string NgPayment { get; set; }

        public string NgMode { get; set; }

        public string NgLpId { get; set; }

        public string NgLpUpId { get; set; }

        public string NgLpEntityId { get; set; }

        public string NgLpEntityType { get; set; }

        public string NgLpBlockId { get; set; }

        public string NgLpButtonName { get; set; }

        public string NgHideShipping { get; set; }

        public string NgOfferIds { get; set; }

        public string NgModeFrom { get; set; }

        public string NgHref { get; set; }

        /// <summary>
        /// validation function before add to cart
        /// </summary>
        public string NgCartAddValid { get; set; }

        public string NgSource { get; set; }

        /// <summary>
        /// Enable cart with spinbox
        /// </summary>
        public string NgBtnText { get; set; }

        public string NgStepSpinbox { get; set; }
        public string NgMaxStepSpinbox { get; set; }
        public string NgMinStepSpinbox { get; set; }
        public string NgForceHiddenPopup { get; set; }
        public string Classes { get; set; }

        public string Title { get; set; }
        public string TypeButton { get; set; }
        public int? TabIndex { get; set; }
        public string AriaLabel { get; set; }
        
        public string PartialName { get; set; }
        
        public string NgDisabled { get; set; }
    }
    
}