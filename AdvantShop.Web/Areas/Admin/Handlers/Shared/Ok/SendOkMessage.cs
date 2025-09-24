using AdvantShop.Core;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments;
using AdvantShop.Web.Admin.Models.Customers.CustomerSegments;
using AdvantShop.Web.Admin.Models.Shared.Socials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Shared.Ok
{
    public class SendOkMessage
    {
        private readonly SocialSendMessageModel _model;

        public SendOkMessage(SocialSendMessageModel model)
        {
            _model = model;
            if (!OkApiService.IsActive())
                throw new BlException("Интеграция с ОК не настроена");
        }

        public SendOkMessageResult Execute()
        {
            var customerIds = new List<Guid>();

            if (_model.CustomerId != null)
                customerIds.Add(_model.CustomerId.Value);

            else if (_model.CustomerSegmentId != null && _model.CustomerSegmentId != 0)
                customerIds = GetCustomerIdsBySegment(_model.CustomerSegmentId.Value);

            else if (_model.CustomerIds != null)
                customerIds = _model.CustomerIds;

            var model = new SendOkMessageResult();

            if (customerIds == null || customerIds.Count == 0)
                return model;

            foreach (var customerId in customerIds)
            {
                try
                {
                    var user = OkService.GetUser(customerId);
                    if (user == null)
                        continue;

                    var result = OkApiService.SendOKMessage(0 , _model.Message.Replace("\r\n", "<br>"), customerId: customerId);
                    if (result != 0)
                        model.SendedCount++;
                }
                catch (BlException ex)
                {
                    model.Errors.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            return model;
        }

        private List<Guid> GetCustomerIdsBySegment(int segmentId)
        {
            var segment = CustomerSegmentService.Get(segmentId);
            if (segment == null)
                throw new BlException("Сегмент не найден");

            var customers =
                new GetCustomersBySegment(
                    new CustomersBySegmentFilterModel() { ItemsPerPage = 1000000, Id = segment.Id }, true).Execute();

            return customers.DataItems.Select(x => x.CustomerId).ToList();
        }
    }

    public class SendOkMessageResult
    {
        public int SendedCount { get; set; }
        public List<string> Errors { get; set; }

        public SendOkMessageResult()
        {
            Errors = new List<string>();
        }
    }
}
