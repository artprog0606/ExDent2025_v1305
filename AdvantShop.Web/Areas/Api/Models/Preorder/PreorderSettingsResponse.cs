using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Preorder
{
    public sealed class PreorderSettingsResponse : IApiResponse
    {
        public PreorderSettingsCustomer Customer { get; set; }
        public PreorderSettings Settings { get; set; }
    }
    public sealed class PreorderSettingsCustomer 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    
    public sealed class PreorderSettings 
    {
        public bool IsShowUserAgreementText { get; set; }
        public string UserAgreementText { get; set; }
        public bool AgreementDefaultChecked { get; set; }
        public string PreOrderText { get; set; }
    }
}