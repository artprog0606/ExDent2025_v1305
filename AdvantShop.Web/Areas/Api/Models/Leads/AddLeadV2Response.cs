using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Leads
{
    public class AddLeadV2Response : ApiResponse
    {
        public AddLeadV2Response() { }
        public AddLeadV2Response(ApiStatus status, string errors):base(status, errors) { }
        public int LeadId { get; set; }
    }
}