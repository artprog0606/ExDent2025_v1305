using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Customers.CustomerTags;
using System;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerTags
{
    public class AddUpdateCustomerTagHandler
    {
        private readonly CustomerTagModel _model;

        public AddUpdateCustomerTagHandler(CustomerTagModel model)
        {
            _model = model;
        }

        public int Execute()
        {
            var tag = new Tag()
            {
                Id = _model.IsEditMode || _model.Id != 0 ? _model.Id : 0,
                Name = _model.Name.DefaultOrEmpty().Trim(),
                Enabled = _model.Enabled,
                SortOrder = _model.SortOrder
            };

            try
            {
                if (_model.IsEditMode || _model.Id != 0)
                {
                    TagService.Update(tag);
                }
                else
                {
                    tag.Id = TagService.Add(tag);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_TagCreated);
                }

                if (tag.Id == 0)
                    return 0;

                return tag.Id;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("AddUpdate customer tag", ex);
            }

            return 0;
        }
    }
}
