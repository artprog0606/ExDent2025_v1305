using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.StaticPages
{
    public class StaticPageResponse : IApiResponse
    {
        public int Id { get; }
        
        public string Title { get; }

        public string Text { get; }
        
        public string Icon { get; }

        public bool ShowInProfile { get; }
        
        public StaticPageResponse(StaticPageApi page)
        {
            Id = page.Id;
            Title = page.Title.Default(null);
            Text = page.Text.Default(null);
            Icon = page.Icon.Default(null);
            ShowInProfile = page.ShowInProfile;
        }
    }
}