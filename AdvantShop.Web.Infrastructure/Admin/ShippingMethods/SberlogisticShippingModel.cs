using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Shipping.Sberlogistic;
using AdvantShop.Shipping.Sberlogistic.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
	[ShippingAdminModel("Sberlogistic")]
	public class SberlogisticShippingModel : ShippingMethodAdminModel, IValidatableObject
	{
		public string APIToken
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.APIToken); }
			set { Params.TryAddValue(SberlogisticTemplate.APIToken, value); }
		}

		public string CityFrom
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.CityFrom); }
			set { Params.TryAddValue(SberlogisticTemplate.CityFrom, value); }
		}

		public string StreetFrom
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.StreetFrom); }
			set { Params.TryAddValue(SberlogisticTemplate.StreetFrom, value); }
		}

		public string HouseFrom
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.HouseFrom); }
			set { Params.TryAddValue(SberlogisticTemplate.HouseFrom, value); }
		}
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}

		public string TypeViewPoints
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.TypeViewPoints, ((int)Shipping.Sberlogistic.TypeViewPoints.WidgetPVZ).ToString()); }
			set { Params.TryAddValue(SberlogisticTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
		}

		public string YaMapsApiKey
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.YaMapsApiKey); }
			set { Params.TryAddValue(SberlogisticTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
		}

		public List<SelectListItem> ListTypesViewPoints
		{
			get
			{
				return Enum.GetValues(typeof(TypeViewPoints))
					.Cast<TypeViewPoints>()
					.Select(x => new SelectListItem()
					{
						Text = x.Localize(),
						Value = ((int)x).ToString()
					})
					.ToList();
			}
		}

		public override bool StatusesSync
        {
			get { return Params.ElementOrDefault(SberlogisticTemplate.StatusesSync).TryParseBool(); }
			set { Params.TryAddValue(SberlogisticTemplate.StatusesSync, value.ToString()); }
		}


		public override string StatusesReference
		{
			get { return Params.ElementOrDefault(SberlogisticTemplate.StatusesReference); }
			set { Params.TryAddValue(SberlogisticTemplate.StatusesReference, value.DefaultOrEmpty()); }
		}

		private Dictionary<string, string> _statuses;
		public override Dictionary<string, string> Statuses
		{
			get
			{
				if (_statuses == null)
				{
					var sberlogisticApiService = new SberlogisticApiService(string.Empty);
					var statuses = sberlogisticApiService.GetListStatuses();
					if (statuses != null && statuses.Statuses?.Count > 0)
						_statuses = statuses.Statuses.ToDictionary(x => x.Code, x => x.Name);
					else
						_statuses = new Dictionary<string, string>();
				}

				return _statuses;
			}
		}

		public List<SelectListItem> ListStatuses
		{
			get
			{
				var statuses = OrderStatusService.GetOrderStatuses()
					.Select(x => new SelectListItem() { Text = x.StatusName, Value = x.StatusID.ToString() }).ToList();

				statuses.Insert(0, new SelectListItem() { Text = "", Value = "" });

				return statuses;
			}
		}

		public string[] DeliveryTypes
		{
			get { return (Params.ElementOrDefault(SberlogisticTemplate.DeliveryTypes) ?? string.Empty).Split(","); }
			set { Params.TryAddValue(SberlogisticTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
		}

		public List<SelectListItem> ListDeliveryTypes
		{
			get
			{
				var listDeliveryTypes = new List<SelectListItem>();

				foreach (var delivertyType in Enum.GetValues(typeof(DeliveryType)).Cast<DeliveryType>())
				{
					listDeliveryTypes.Add(new SelectListItem()
					{
						Text = delivertyType.Localize(),
						Value = ((int)delivertyType).ToString()
					});
				}

				return listDeliveryTypes;
			}
		}


	}
}
