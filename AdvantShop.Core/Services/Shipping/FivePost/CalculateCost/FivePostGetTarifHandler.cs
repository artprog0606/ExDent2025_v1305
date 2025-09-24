using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.FivePost.Helpers;
using AdvantShop.Shipping.FivePost.Api;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Shipping.FivePost.CalculateCost
{
    public class FivePostGetTarifHandler
    {
        private FivePostGetTarifParams _model;

        public FivePostGetTarifHandler(FivePostGetTarifParams model)
        {
            _model = model;
        }

        public FivePostGetTarifResult Execute()
        {
            var result = new FivePostGetTarifResult();

            var rateList = _model.RateList
                .Where(x => _model.ActiveTarifTypes.Contains(x.TypeCode))
                .ToList();
            var deliveryList = _model.PossibleDeliveryList
                .Where(x => _model.WarehouseDeliveryTypeReference.Values.Contains(x.Code))
                .ToList();

            result.Rate = GetBestRate(rateList, deliveryList);
            if (result.Rate == null)
                return null;

            if (_model.RateDeliverySLReference.ContainsKey(result.Rate.TypeCode) is false)
                return null;

            result.PossibleDelivery = deliveryList
                .FirstOrDefault(x => _model.RateDeliverySLReference[result.Rate.TypeCode].Contains(x.Type));

            if (result.PossibleDelivery == null)
                return null;

            result.WarehouseId = _model.WarehouseDeliveryTypeReference.Keys.FirstOrDefault(x =>
                _model.WarehouseDeliveryTypeReference[x] == result.PossibleDelivery.Code);

            if (result.WarehouseId.IsNullOrEmpty())
                return null;

            return result;
        }

        private FivePostRate GetBestRate(List<FivePostRate> tarifs, List<FivePostPossibleDelivery> deliveryList)
        {
            FivePostRate bestRate = null;
            float minValue = float.MaxValue;

            foreach (var tarif in tarifs)
            {
                if (tarif.ValueWithVat == 0)
                    continue;

                if (!(_model.RateDeliverySLReference.ContainsKey(tarif.TypeCode)
                        && deliveryList.Any(x => _model.RateDeliverySLReference[tarif.TypeCode].Contains(x.Type))))
                    continue;

                var currentValue = FivePostHelper.Calculate(
                tarif.ValueWithVat,
                tarif.ExtraValueWithVat,
                _model.Weight);

                if (currentValue < minValue)
                {
                    minValue = currentValue;
                    bestRate = tarif;
                }
            }

            return bestRate;
        }
    }
}
