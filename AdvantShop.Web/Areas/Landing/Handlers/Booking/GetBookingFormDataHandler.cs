using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class GetBookingFormDataHandler : ICommandHandler<object>
    {
        private readonly BookingFormDataDto _model;
        private readonly LpFormService _lpFormService;

        public GetBookingFormDataHandler(BookingFormDataDto model)
        {
            _model = model;
            _lpFormService = new LpFormService();
        }

        public object Execute()
        {
            var form = _lpFormService.GetByBlock(_model.BlockId);
            if (form == null)
                throw new BlException("Форма не найдена");

            var categories = new List<BookingServiceCategoryModel>();
            if (_model.LoadServices)
            {
                var services = ServiceService.GetListReservationResourceServices(_model.AffiliateId, _model.ResourceId).Where(x => x.Enabled).ToList();

                var categoryIds = services.Select(x => x.CategoryId).Distinct().ToList();
                foreach (var categoryId in categoryIds)
                {
                    var category = (BookingServiceCategoryModel)CategoryService.Get(categoryId);
                    if (category == null || !category.Enabled)
                        continue;
                    category.Services = services.Where(x => x.CategoryId == categoryId).Select(x => (BookingServiceModel)x).OrderBy(x => x.SortOrder).ToList();
                    if (category.Services.Any())
                        categories.Add(category);
                }
                categories = categories.OrderBy(x => x.SortOrder).ToList();
            }

            return new
            {
                form,
                categories,
                shoppingCart = LPageSettings.ShowShoppingCart(_model.LpId) && LPageSettings.GetShoppingCartType(_model.LpId) == ELpShoppingCartType.Booking
            };
        }
    }
}
