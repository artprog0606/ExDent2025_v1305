using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ISuggestions
    {
        bool SuggestAddressInAdmin { get; }
        bool SuggestAddressInClient { get; }
        string SuggestAddressUrl { get; }
        bool SuggestFullNameInClient { get; }
        string SuggestFullNameUrl { get; }
        string SuggestCompanyNameUrl { get; }
        string SuggestBankNameUrl { get; }
        void ProcessCheckoutAddress(CheckoutAddressQueryModel address);
        void ProcessAddress(SuggestAddressQueryModel model, bool inAdminPart);

        void GetSuggestionsHtmlAttributes(int customerFieldId, Dictionary<string, object> htmlAttributes, CustomerFieldAssignment fieldAssignment, string onSelectFunc);
    }


    public enum EAddressPart
    {
        None,
        [StringName("street")]
        Street,
        [StringName("house")]
        House,
        [StringName("area")]
        Area
    }

    public class CheckoutAddressQueryModel : CheckoutAddress
    {
        public bool ByCity { get; set; }
    }

    public class SuggestAddressModel : LocationModel
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
    }

    public class SuggestAddressQueryModel : SuggestAddressModel
    {
        public string Q { get; set; }
        public EAddressPart Part { get; set; }
        public bool ByCity { get; set; }
        public bool InAdminPart { get; set; }
    }


    public enum EFullNamePart
    {
        None,
        FirstName,
        LastName,
        Patronymic
    }

    public class SuggestFullNameModel
    {
        public string Value { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
    }

    public class SuggestFullNameQueryModel : SuggestFullNameModel
    {
        public string Q { get; set; }
        public EFullNamePart Part { get; set; }
    }
}