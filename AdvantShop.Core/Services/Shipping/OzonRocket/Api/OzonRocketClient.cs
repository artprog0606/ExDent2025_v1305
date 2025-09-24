using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.OzonRocket.Api
{
    public class OzonRocketClient : OzonRocketApiService, IOzonRocketClient, IDelivery, IOrders, ITracking
    {
        private OzonRocketClient(string clientId, string clientSecret) : base(clientId, clientSecret) { }

        public static IOzonRocketClient Create(string clientId, string clientSecret) =>
            new OzonRocketClient(clientId, clientSecret);

        #region IOzonRocketClient

        public IDelivery Delivery => this;
        public IOrders Orders => this;
        public ITracking Tracking => this;

        #endregion

        #region IDelivery

        public List<DeliveryVariant> GetVariants(GetDeliveryVariantsParams @params) =>
            base.GetDeliveryVariants(@params)?.Data;

        public List<DeliveryVariant> GetVariantsByIds(params long[] ids) =>
            base.GetDeliveryVariantsByIds(
                new GetDeliveryVariantsByIds() {Ids = ids.ToList()})?.Data;

        public List<DeliveryVariant> GetVariantsByAddress(GetDeliveryVariantsByAddressParams @params) =>
            base.GetDeliveryVariantsByAddress(@params)?.Data;

        public DeliveryCalculateInformation Calculate(GetDeliveryCalculateInformationParams @params) =>
            base.GetDeliveryCalculateInformation(@params);

        public int? GetDeliveryTimeInDays(GetDeliveryTimeParams @params)
        {
            var result = base.GetDeliveryTime(@params)?.Days;
            return result == 0 ? null : result;
        }

        public List<DropOffPlace> GetDropOffPlaces() => base.GetDeliveryFromPlaces()?.Places;
        public List<PickupPlace> GetPickupPlaces() => base.GetDeliveryPickupPlaces()?.Places;

        #endregion

        #region IOrders

        public CreatedOrder Create(NewOrder order) => base.CreateOrder(order);
        public UpdatedOrder Update(UpdateOrder order) => base.UpdateOrder(order);

        public CreatedDraftOrder CreateDraft(NewDraftOrder order) => base.CreateDraftOrder(order);

        public List<ResponseCancellationOrderResult> Cancel(params long[] ids) => base.CancelOrders(ids)?.Responses;

        #endregion

        #region ITracking

        public TrackingByPostingNumber ByPostingNumber(string postingNumber) =>
            base.TrackingByPostingNumber(new TrackingByPostingNumberParams(){PostingNumber = postingNumber});

        public List<TrackingItem> ByPostingNumbersOrIds(params string[] postingNumbersOrIds) =>
            base.TrackingByPostingNumbersOrIds(
                new TrackingByPostingNumbersOrIdsParams() {Articles = postingNumbersOrIds.ToList()})?.Items;

        #endregion
    }

    public interface IOzonRocketClient
    {
        IDelivery Delivery { get; }
        IOrders Orders { get; }
        ITracking Tracking { get; }
        List<string> LastActionErrors { get; set; }
    }

    public interface IDelivery
    {
        List<DeliveryVariant> GetVariants(GetDeliveryVariantsParams @params);
        List<DeliveryVariant> GetVariantsByIds(params long[] ids);
        List<DeliveryVariant> GetVariantsByAddress(GetDeliveryVariantsByAddressParams @params);
        DeliveryCalculateInformation Calculate(GetDeliveryCalculateInformationParams @params);
        int? GetDeliveryTimeInDays(GetDeliveryTimeParams @params);
        List<DropOffPlace> GetDropOffPlaces();
        List<PickupPlace> GetPickupPlaces();
    }

    public interface IOrders
    {
        CreatedOrder Create(NewOrder order);
        UpdatedOrder Update(UpdateOrder order);
        CreatedDraftOrder CreateDraft(NewDraftOrder order);
        ConvertedDraftToOrder ConvertDraftToOrder(string draftLogisticOrderNumber);
        List<ResponseCancellationOrderResult> Cancel(params long[] ids);
    }

    public interface ITracking
    {
        TrackingByPostingNumber ByPostingNumber(string postingNumber);
        List<TrackingItem> ByPostingNumbersOrIds(params string[] postingNumbersOrIds);
    }
}