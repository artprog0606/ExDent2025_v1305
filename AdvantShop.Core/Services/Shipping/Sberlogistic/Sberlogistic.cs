//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.Sberlogistic.Api;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Shipping.Sberlogistic
{
	[ShippingKey("Sberlogistic")]
	public class Sberlogistic : BaseShippingWithCargo, IShippingSupportingPaymentCashOnDelivery, IShippingSupportingSyncOfOrderStatus, IShippingSupportingTheHistoryOfMovement, IShippingLazyData
	{

		#region Ctor
		private readonly SberlogisticApiService _sberlogisticApiService;
		private readonly string _apiToken;
		private readonly string _cityFrom;
		private readonly string _streetFrom;
		private readonly string _houseFrom;
		private readonly TypeViewPoints _typeViewPoints;
		private readonly string _yaMapsApiKey;
		private readonly List<DeliveryType> _deliveryTypes;
		private readonly bool _statusesSync;
		public const string KeyNameSberlogisticOrderUuidInOrderAdditionalData = "SberlogisticOrderUuid";
		public const string KeyNameSberlogisticOrderIsCanceledInOrderAdditionalData = "SberlogisticOrderIsCanceled";
		public const string KeyNameSberlogisticOrderBarcodeInOrderAdditionalData = "SberlogisticOrderBarcode";
		public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }
		public Sberlogistic(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
		{
			_apiToken = _method.Params.ElementOrDefault(SberlogisticTemplate.APIToken);
			_cityFrom = _method.Params.ElementOrDefault(SberlogisticTemplate.CityFrom);
			_streetFrom = _method.Params.ElementOrDefault(SberlogisticTemplate.StreetFrom);
			_houseFrom = _method.Params.ElementOrDefault(SberlogisticTemplate.HouseFrom);
			_typeViewPoints = (TypeViewPoints)_method.Params.ElementOrDefault(SberlogisticTemplate.TypeViewPoints).TryParseInt();
			_yaMapsApiKey = _method.Params.ElementOrDefault(SberlogisticTemplate.YaMapsApiKey);
			_deliveryTypes = (method.Params.ElementOrDefault(SberlogisticTemplate.DeliveryTypes) ?? string.Empty).Split(",").Select(x => (DeliveryType)(x.TryParseInt())).ToList();
			_sberlogisticApiService = new SberlogisticApiService(_apiToken);
			_statusesSync = method.Params.ElementOrDefault(SberlogisticTemplate.StatusesSync).TryParseBool();
			var statusesReference = method.Params.ElementOrDefault(SberlogisticTemplate.StatusesReference);
			if (!string.IsNullOrEmpty(statusesReference))
			{
				string[] arr = null;
				StatusesReference =
					statusesReference.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
							.ToDictionary(x => (arr = x.Split(','))[0],
								x => arr.Length > 1 ? arr[1].TryParseInt(true) : null);
			}
			else
				StatusesReference = new Dictionary<string, int?>();
		}

		public string CityFrom => _cityFrom;
		public string StreetFrom => _streetFrom;
		public string HouseFrom => _houseFrom;

		public SberlogisticApiService SberlogisticApiService => _sberlogisticApiService;

		#endregion

		#region IShippingSupportingTheHistoryOfMovement

		public bool ActiveHistoryOfMovement => true;

		public List<HistoryOfMovement> GetHistoryOfMovement(Order order)
		{
			var postingNumber = OrderService.GetOrderAdditionalData(order.OrderID, KeyNameSberlogisticOrderBarcodeInOrderAdditionalData);

			if (postingNumber.IsNotEmpty())
			{
				var movementResult = _sberlogisticApiService.GetListPackageStatuses(new string[] { postingNumber });
				if (movementResult?.Statuses?.Count > 0)
				{
					return movementResult.Statuses
						.Select(x => new HistoryOfMovement
						{
							Code = x.Status,
							Name = x.Status,
							Comment = x.Paid ? "Оплата за услуги по отправлению прошла успешно" : String.Empty
						})
						.ToList();
				}
			}

			return null;
		}

		#endregion

		#region IShippingSupportingSyncOfOrderStatus
		public bool SyncByAllOrders => true;

		public Dictionary<string, int?> StatusesReference { get; private set; }

		public bool StatusesSync => _statusesSync;

		public void SyncStatusOfOrder(Order order) => throw new NotImplementedException();

		public void SyncStatusOfOrders(IEnumerable<Order> orders)
		{
			var postingNumbers = new Dictionary<string, Order>();
			foreach (var order in orders)
			{
				if (string.IsNullOrEmpty(order.TrackNumber) || postingNumbers.ContainsKey(order.TrackNumber))
					continue;
				postingNumbers.Add(order.TrackNumber, order);

				if (postingNumbers.Count >= 100)
				{
					UpdateStatusOrders(postingNumbers);
					postingNumbers.Clear();
				}
			}

			UpdateStatusOrders(postingNumbers);
		}

		private void UpdateStatusOrders(Dictionary<string, Order> postingNumbers)
		{
			if (postingNumbers.Count <= 0)
				return;

			var history = _sberlogisticApiService.GetListPackageStatuses(postingNumbers.Keys.ToArray());


			if (history != null && history.Statuses?.Count > 0)
			{
				foreach (var trackingItem in history.Statuses)
				{
					if (postingNumbers.ContainsKey(trackingItem.Id))
					{
						var order = postingNumbers[trackingItem.Id];
						int? sberlogisticOrderStatus = null;

						if (StatusesReference.ContainsKey(trackingItem.Status))
						{
							sberlogisticOrderStatus = StatusesReference[trackingItem.Status];
							if (!sberlogisticOrderStatus.HasValue || order.OrderStatusId == sberlogisticOrderStatus.Value)
								continue;
						}

						if (sberlogisticOrderStatus.HasValue &&
							order.OrderStatusId != sberlogisticOrderStatus.Value &&
							OrderStatusService.GetOrderStatus(sberlogisticOrderStatus.Value) != null)
						{
							OrderStatusService.ChangeOrderStatus(order.OrderID,
								sberlogisticOrderStatus.Value, "Синхронизация статусов для сберлогистики");
						}
					}
				}
			}
		}

		#endregion Statuses

		protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
		{
			var shippingOptions = new List<BaseShippingOption>();
			var orderCost = _totalPrice;
			var deliveryPoint = string.Join(",", new[] { _calculationParameters.City, _calculationParameters.Region, _calculationParameters.Country });
			if (deliveryPoint.IsNullOrEmpty())
				return shippingOptions;

            var serviceCodes = new List<int>();
            if (calculationVariants.HasFlag(CalculationVariants.PickPoint) && _deliveryTypes.Contains(DeliveryType.PVZ))
                serviceCodes.Add(1);
            if (calculationVariants.HasFlag(CalculationVariants.Courier) && _deliveryTypes.Contains(DeliveryType.Courier))
                serviceCodes.Add(2);
            if (serviceCodes.Count == 0)
                return shippingOptions;

            var weight = GetTotalWeight(1000);
			var dimensions = GetCalcDimensions();
			var cVolume = new CargoVolume
			{
				Weight = (int)Math.Ceiling(weight),
				Length = dimensions[0],
				Width = dimensions[1],
				Height = dimensions[2]
			};
			var tariffs = _sberlogisticApiService.GetTariffs(new Tariff
			{
				Uid = CustomerContext.CustomerId.ToString(),
				CargoVolumes = new List<CargoVolume> { cVolume },
				Customer = new CustomerData { Type = eSenderTypeCode.ORGANIZATION },
				DeclaredValue = (int)Math.Round(orderCost) * 100,
				DeliveryPoint = new DeliveryPoint2 { AddressType = eAddressType.STRING, Address = deliveryPoint },
				DeparturePoint = new DeliveryPoint { AddressType = eAddressType.STRING, Address = string.Join(",", new[] { _cityFrom, _streetFrom, _houseFrom }) },
				CashOnDelivery = (int)Math.Round(orderCost) * 100,
				ServiceCodes = serviceCodes
			});
			if (tariffs != null)
			{
				foreach (var tariff in tariffs)
				{
					//ПВЗ
					if (tariff.ServiceCode == 1 && tariff.TotalSum != null)
					{
						string selectedPickPointId = null;

						if (_calculationParameters.ShippingOption?.ShippingType == ((ShippingKeyAttribute)typeof(Sberlogistic).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
						{
							if (_calculationParameters.ShippingOption.GetType() == typeof(SberlogisticMapOption))
								selectedPickPointId = ((SberlogisticMapOption)_calculationParameters.ShippingOption).PickpointId;

							if (_calculationParameters.ShippingOption.GetType() == typeof(SberlogisticOption) && ((SberlogisticOption)_calculationParameters.ShippingOption).SelectedPoint != null)
								selectedPickPointId = ((SberlogisticOption)_calculationParameters.ShippingOption).SelectedPoint.Id.ToString();

							if (_calculationParameters.ShippingOption.GetType() == typeof(SberlogisticWidgetOption) && ((SberlogisticWidgetOption)_calculationParameters.ShippingOption).SelectedPoint != null)
								selectedPickPointId = ((SberlogisticWidgetOption)_calculationParameters.ShippingOption).PickpointId;
						}

						var calculateOption = new SberlogisticCalculateOption { IsCourier = false };
						float rate = 10;
						if (_typeViewPoints == TypeViewPoints.List)
						{
							var listPoints = _sberlogisticApiService.GetPickUpPoints(new PickUpPointParams
							{
								SettlementAddress = deliveryPoint,
								Weight = (int)Math.Ceiling(weight),
								Length = (int)(dimensions[0] / rate),
								Width = (int)(dimensions[1] / rate),
								Height = (int)(dimensions[2] / rate)
							});
							if (listPoints != null)
							{
								var shippingPoints = listPoints.Where(x => x.Limits?.COD is null || orderCost < x.Limits.COD).Select(CastPoint).ToList();
								bool isAvailableCOD = shippingPoints.Any(point => point.AllowedCod); // todo: очень сомнительное такое поведение
								if (isAvailableCOD && !selectedPickPointId.IsNullOrEmpty())
								{
									var pickPoint = shippingPoints.FirstOrDefault(x => x.Id == selectedPickPointId);
									isAvailableCOD = pickPoint.AvailableCardOnDelivery is true || pickPoint.AvailableCashOnDelivery is true;
								}
								var sberOption = new SberlogisticOption(_method, _totalPrice)
								{
									Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
									Rate = tariff.ShippingFee.PriceWithVat / 100f,
									PriceCash = tariff.TotalSum.PriceWithVat / 100f,
									IsAvailablePaymentCashOnDelivery = isAvailableCOD,
									CalculateOption = calculateOption,
									ShippingPoints = shippingPoints,
									HideAddressBlock = true
								};
								if (tariff.Delivery != null)
									sberOption.DeliveryTime = $"{tariff.Delivery.Min + _method.ExtraDeliveryTime} - {tariff.Delivery.Max + _method.ExtraDeliveryTime} дн.";
								sberOption.BasePrice = sberOption.Rate;
								shippingOptions.Add(sberOption);
							}
						}
						else if (_typeViewPoints == TypeViewPoints.YaWidget && !_yaMapsApiKey.IsNullOrEmpty())
						{
							var listPoints = _sberlogisticApiService.GetPickUpPoints(new PickUpPointParams
							{
								SettlementAddress = deliveryPoint,
								Weight = (int)Math.Ceiling(weight),
								Length = (int)(dimensions[0] / rate),
								Width = (int)(dimensions[1] / rate),
								Height = (int)(dimensions[2] / rate)
							});
							if (listPoints != null)
							{
								var shippingPoints = listPoints.Where(x => x.Limits?.COD is null || orderCost < x.Limits.COD).Select(CastPoint).ToList();
								bool isAvailableCOD = shippingPoints.Any(point => point.AllowedCod); // todo: очень сомнительное такое поведение
								if (isAvailableCOD && !selectedPickPointId.IsNullOrEmpty())
								{
									var pickPoint = shippingPoints.FirstOrDefault(x => x.Id == selectedPickPointId);
									var pickPoint1 = _sberlogisticApiService.GetPickUpPoint(selectedPickPointId);
									isAvailableCOD = pickPoint.AvailableCardOnDelivery is true || pickPoint.AvailableCashOnDelivery is true;
								}
								var sberOption = new SberlogisticMapOption(_method, _totalPrice)
								{
									Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
									Rate = tariff.ShippingFee.PriceWithVat / 100f,
									PriceCash = tariff.TotalSum.PriceWithVat / 100f,
									IsAvailablePaymentCashOnDelivery = isAvailableCOD,
									CurrentPoints = shippingPoints,
									CalculateOption = calculateOption,
									HideAddressBlock = true
								};
								if (tariff.Delivery != null)
									sberOption.DeliveryTime = $"{tariff.Delivery.Min + _method.ExtraDeliveryTime} - {tariff.Delivery.Max + _method.ExtraDeliveryTime} дн.";
								sberOption.BasePrice = sberOption.Rate;
								SetMapData(sberOption, deliveryPoint, (int)Math.Ceiling(weight), dimensions);
								shippingOptions.Add(sberOption);
							}
						}
						else
						{
							bool isAvailableCOD = true; // todo: очень сомнительное такое поведение
							if (!selectedPickPointId.IsNullOrEmpty())
							{
								var pickPoint = _sberlogisticApiService.GetPickUpPoint(selectedPickPointId);
								isAvailableCOD = pickPoint?.Options != null && (pickPoint.Options.Cod || pickPoint.Options.Card);
							}
							var kladrDeliveryPoint = tariff.CalculationDetail.DeliveryPoint.Kladr;
							var sberOption = new SberlogisticWidgetOption(_method, _totalPrice)
							{
								Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace", _method.Name),
								Rate = tariff.ShippingFee.PriceWithVat / 100f,
								PriceCash = tariff.TotalSum.PriceWithVat / 100f,
								CalculateOption = calculateOption,
								CurrentKladrId = kladrDeliveryPoint,
								IsAvailablePaymentCashOnDelivery = isAvailableCOD,
								HideAddressBlock = true
							};
							if (tariff.Delivery != null)
								sberOption.DeliveryTime = $"{tariff.Delivery.Min + _method.ExtraDeliveryTime} - {tariff.Delivery.Max + _method.ExtraDeliveryTime} дн.";
							sberOption.BasePrice = sberOption.Rate;
							ConfigShiptorWidget(sberOption, orderCost, weight / 1000, dimensions, kladrDeliveryPoint);
							shippingOptions.Add(sberOption);
						}

					}
					else if (tariff.ServiceCode == 2 && tariff.TotalSum != null)//СберКурьер
					{
						var calculateOption = new SberlogisticCalculateOption { IsCourier = true };
						var sberOption = new SberlogisticOption(_method, _totalPrice)
						{
							Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.ByCourierWithParam", _method.Name),
							Rate = tariff.ShippingFee.PriceWithVat / 100f,
							PriceCash = tariff.TotalSum.PriceWithVat / 100f,
							CalculateOption = calculateOption,
							IsAvailablePaymentCashOnDelivery = true,
						};
						if (tariff.Delivery != null)
							sberOption.DeliveryTime = $"{tariff.Delivery.Min + _method.ExtraDeliveryTime} - {tariff.Delivery.Max + _method.ExtraDeliveryTime} дн.";
						sberOption.BasePrice = sberOption.Rate;
						shippingOptions.Add(sberOption);
					}
				}
			}

			return shippingOptions;
		}

		protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
		{
			if (!_deliveryTypes.Contains(DeliveryType.PVZ))
				return null;
			if (string.IsNullOrEmpty(_cityFrom) || string.IsNullOrEmpty(_streetFrom) || string.IsNullOrEmpty(_houseFrom))
				return null;
			var pickPoint = _sberlogisticApiService.GetPickUpPoint(pointId);
			if (pickPoint is null)
				return null;

			// принимает такой вес посылки
			var weight = GetTotalWeight(1000);
			if (weight > pickPoint.Limits?.Weight)
				return null;

			// принимает посылку таких габаритов
			var dimensions = GetDimensions(10);
			if (pickPoint.Limits != null)
			{
				if (pickPoint.Limits.Height != null
					&& dimensions[2] > pickPoint.Limits.Height)
					return null;
				if (pickPoint.Limits.Width != null
					&& dimensions[1] > pickPoint.Limits.Width)
					return null;
				if (pickPoint.Limits.Length != null
					&& dimensions[0] > pickPoint.Limits.Length)
					return null;

				var dimensionSum = dimensions.Sum();
				if (pickPoint.Limits.DimensionsSum != null
					&& dimensionSum > pickPoint.Limits.DimensionsSum)
					return null;

				// может принять посылку такой стоимости
				if (pickPoint.Limits.COD != null
					&& _totalPrice > pickPoint.Limits.COD)
					return null;
			}
			var orderCost = _totalPrice;
			var deliveryPoint = pickPoint.AddressString;
			if (deliveryPoint.IsNullOrEmpty())
				return null;
			var dimensionsInMillimeter = GetCalcDimensions();
			var cVolume = new CargoVolume
			{
				Weight = (int)Math.Ceiling(weight),
				Length = dimensionsInMillimeter[0],
				Width = dimensionsInMillimeter[1],
				Height = dimensionsInMillimeter[2]
			};
			var tariffs = _sberlogisticApiService.GetTariffs(new Tariff
			{
				Uid = CustomerContext.CustomerId.ToString(),
				CargoVolumes = new List<CargoVolume> { cVolume },
				Customer = new CustomerData { Type = eSenderTypeCode.ORGANIZATION },
				DeclaredValue = (int)Math.Ceiling(orderCost) * 100,
				DeliveryPoint = new DeliveryPoint2 { AddressType = eAddressType.STRING, Address = deliveryPoint },
				DeparturePoint = new DeliveryPoint { AddressType = eAddressType.STRING, Address = string.Join(",", new[] { _cityFrom, _streetFrom, _houseFrom }) },
				CashOnDelivery = (int)Math.Ceiling(orderCost) * 100,
				ServiceCodes = new List<int> { 1 }
			});
			if (tariffs == null)
				return null;
			var tariff = tariffs.FirstOrDefault();
			var point = CastPoint(pickPoint);
			var sberOption = new SberlogisticOption(_method, _totalPrice)
			{
				Name = LocalizationService.GetResourceFormat("Core.Services.Shipping.PickupPointsWithParam", _method.Name),
				BasePrice = tariff.ShippingFee.PriceWithVat / 100f,
				IsAvailablePaymentCashOnDelivery = point.AllowedCod,
				Rate = tariff.TotalSum.PriceWithVat / 100f,
				ShippingPoints = new List<SberlogisticShippingPoint>() { point },
				SelectedPoint = point
			};
			sberOption.PriceCash = sberOption.Rate;

			return new[] { sberOption };
		}

		private int[] GetCalcDimensions(float rate = 1)
		{
			var dimensions = GetDimensions(rate);
			var dim = new int[dimensions.Length];
			for (var i = 0; i < dim.Length; i++)
				dim[i] = (int)Math.Ceiling(dimensions[i]);

			return dim;
		}

		private void SetMapData(SberlogisticMapOption option, string deliveryPoint, int weight, int[] dimensions)
		{
			string lang = "en_US";
			switch (Localization.Culture.Language)
			{
				case Localization.Culture.SupportLanguage.Russian:
					lang = "ru_RU";
					break;
				case Localization.Culture.SupportLanguage.English:
					lang = "en_US";
					break;
				case Localization.Culture.SupportLanguage.Ukrainian:
					lang = "uk_UA";
					break;
			}
			option.MapParams = new PointDelivery.MapParams();
			option.MapParams.Lang = lang;
			option.MapParams.YandexMapsApikey = _yaMapsApiKey;
			option.MapParams.Destination = deliveryPoint;

			option.PointParams = new PointDelivery.PointParams();
			option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
			option.PointParams.PointsByDestination = true;

			if (option.PointParams.IsLazyPoints)
				option.PointParams.LazyPointsParams = new Dictionary<string, object>
				{
					{ "deliveryPoint", deliveryPoint },
					{ "weightInGrams", weight },
					{ "dimensions", string.Join("x", dimensions.Select(x => (x / 10).ToString())) },
				};
			else
				option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
		}

		public object GetLazyData(Dictionary<string, object> data)
		{
			if (data == null || !data.ContainsKey("deliveryPoint")
				|| !data.ContainsKey("weightInGrams")
				|| !data.ContainsKey("dimensions"))
				return null;
			var weight = data["weightInGrams"].ToString().TryParseInt();
			var dimensions = data["dimensions"].ToString().Split('x').Select(x => x.TryParseInt()).ToArray();
			var deliveryPoint = data["deliveryPoint"].ToString();
			var deliveryPoints = _sberlogisticApiService.GetPickUpPoints(new PickUpPointParams
			{
				Weight = weight,
				Length = dimensions[0],
				Width = dimensions[1],
				Height = dimensions[2],
				SettlementAddress = deliveryPoint
			});
			var points = deliveryPoints.Select(CastPoint).ToList();

			return GetFeatureCollection(points);
		}

		public PointDelivery.FeatureCollection GetFeatureCollection(List<SberlogisticShippingPoint> points)
		{
			return new PointDelivery.FeatureCollection
			{
				Features = points.Select(p =>
				{
					var intId = p.Id.GetHashCode();
					return new PointDelivery.Feature
					{
						Id = intId,
						Geometry = new PointDelivery.PointGeometry {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f},
						Options = new PointDelivery.PointOptions {Preset = "islands#dotIcon"},
						Properties = new PointDelivery.PointProperties
						{
							BalloonContentHeader = p.Address,
							HintContent = p.Address,
							BalloonContentBody =
								string.Format(
									"{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
									string.Join("<br />", new[]
									{
										p.TimeWorkStr,
										!p.AllowedCod
											? "Оплата при получении недоступна"
											: p.AvailableCashOnDelivery is true
												? "Способы оплаты: наличные" + (p.AvailableCardOnDelivery is true
													? ", банковские карты"
													: String.Empty)
												: "Способы оплаты: банковские карты"
									}.Where(x => x.IsNotEmpty())),
									"<br />",
									intId,
									p.Id),
							BalloonContentFooter = p.Description
						}
					};
				}).ToList()
			};
		}


		private void ConfigShiptorWidget(SberlogisticWidgetOption option, float orderCost, float weight, int[] dimensions, string kladrKodCity)
		{
			var cod = false;
			var card = false;

			var checkoutInfo = MyCheckout.Factory(CustomerContext.CustomerId);
			if (checkoutInfo?.Data?.SelectPayment?.GetDetails()?.IsCashOnDeliveryPayment is true)
			{
				card = true;
				cod = true;
			}

			var dimensionsObj = new WidgetConfigParamDimensions { height = dimensions[0] / 10f, width = dimensions[1] / 10f, length = dimensions[2] / 10f };

			option.WidgetConfigData = new Dictionary<string, string>();
			option.WidgetConfigParams = new Dictionary<string, object>();

			option.WidgetConfigData.Add("data-mode", "inline");
			option.WidgetConfigData.Add("data-price", cod || card ? orderCost.ToInvariantString() : "0");
			option.WidgetConfigData.Add("data-cod", cod ? "1" : "0");
			option.WidgetConfigData.Add("data-card", card ? "1" : "0");
			option.WidgetConfigData.Add("data-courier", "sberlogistics");
			option.WidgetConfigData.Add("data-pvz-picker", "list");
			option.WidgetConfigData.Add("data-declaredcost", orderCost.ToInvariantString());
			option.WidgetConfigData.Add("data-show-confirm", "1");
			option.WidgetConfigData.Add("data-dimensions", JsonConvert.SerializeObject(dimensionsObj));
			option.WidgetConfigData.Add("data-weight", weight.ToInvariantString());
			option.WidgetConfigData.Add("data-detail", "1");

			option.WidgetConfigData.Add("data-kladr", kladrKodCity);

			option.WidgetConfigParams.Add("location", new WidgetConfigParamLocation { kladr_id = kladrKodCity });
			option.WidgetConfigParams.Add("dimensions", dimensionsObj);
			option.WidgetConfigParams.Add("cod", cod);
			option.WidgetConfigParams.Add("card", card);
			option.WidgetConfigParams.Add("price", cod || card ? orderCost : 0f);
			option.WidgetConfigParams.Add("weight", weight);
			option.WidgetConfigParams.Add("declaredCost", orderCost);

			var copyOption = new SberlogisticWidgetOption(_method, _totalPrice)
			{
				HideAddressBlock = option.HideAddressBlock,
				Rate = option.Rate > 0f ? option.Rate : 1f,
				BasePrice = option.BasePrice,
				PriceCash = option.PriceCash,
				DeliveryId = option.DeliveryId,
				Name = option.Name,
				DeliveryTime = option.DeliveryTime,
				IsAvailablePaymentCashOnDelivery = option.IsAvailablePaymentCashOnDelivery,
				CashOnDeliveryCardAvailable = option.CashOnDeliveryCardAvailable,
				WidgetConfigData = option.WidgetConfigData,
				WidgetConfigParams = option.WidgetConfigParams,
				PickpointId = option.PickpointId,
				PickpointAddress = option.PickpointAddress,
				PickpointAdditionalData = option.PickpointAdditionalData,
				PickpointAdditionalDataObj = option.PickpointAdditionalDataObj,
			};

			var items = new List<BaseShippingOption>() { copyOption };
			var modules = Core.Modules.AttachedModules.GetModules<Core.Modules.Interfaces.IShippingCalculator>();
			foreach (var module in modules)
			{
				if (module != null)
				{
					var classInstance = (Core.Modules.Interfaces.IShippingCalculator)Activator.CreateInstance(module);
					classInstance.ProcessOptions(items, _items, _totalPrice);
				}
			}
			if (_method.ExtrachargeInPercents != 0 && _method.ExtrachargeInNumbers == 0)
			{
				if (_method.ExtrachargeFromOrder)
				{
					option.WidgetConfigParams.Add("markup", Math.Round(_method.ExtrachargeInPercents * _totalPrice / 100, 2).ToInvariantString());
					option.WidgetConfigData.Add("data-markup", Math.Round(_method.ExtrachargeInPercents * _totalPrice / 100, 2).ToInvariantString());
				}
				else
				{
					option.WidgetConfigParams.Add("markup", _method.ExtrachargeInPercents.ToInvariantString() + "%");
					option.WidgetConfigData.Add("data-markup", _method.ExtrachargeInPercents.ToInvariantString() + "%");
				}
			}
			else if (_method.ExtrachargeInPercents == 0 && _method.ExtrachargeInNumbers != 0)
			{
				option.WidgetConfigParams.Add("markup", _method.ExtrachargeInNumbers.ToInvariantString());
				option.WidgetConfigData.Add("data-markup", _method.ExtrachargeInNumbers.ToInvariantString());
			}
			else
			{
				var markup = option.GetExtracharge();

				option.WidgetConfigParams.Add("markup", markup.ToInvariantString());
				option.WidgetConfigData.Add("data-markup", markup.ToInvariantString());
			}

			if (items[0].Rate <= 0f)
			{
				option.WidgetConfigData.Add("data-deliveryPriceText", _method.ZeroPriceMessage);
				option.WidgetConfigParams.Add("textPrice", _method.ZeroPriceMessage);
			}
			else
			{
				option.WidgetConfigData.Add("data-deliveryPriceText", string.Empty);
				option.WidgetConfigParams.Add("textPrice", string.Empty);
			}

		}

		public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude,
			float topLeftLongitude, float bottomRightLatitude, float bottomRightLongitude)
		{
			if (!_deliveryTypes.Contains(DeliveryType.PVZ))
				return null;
			var weight = GetTotalWeight(1000);
			var dimensions = GetCalcDimensions(10);
			var pickPoints = _sberlogisticApiService.GetPickUpPoints(new PickUpPointParams
			{
				Weight = (int)Math.Ceiling(weight),
				Length = dimensions[0],
				Width = dimensions[1],
				Height = dimensions[2]
			});
			return pickPoints
				.Where(point => point.GeoLocation != null)
				.Where(point => topLeftLatitude > point.GeoLocation.Lat
					&& topLeftLongitude < point.GeoLocation.Lon
					&& bottomRightLatitude < point.GeoLocation.Lat
					&& bottomRightLongitude > point.GeoLocation.Lon)
				.Select(CastPoint);
		}

		public override BaseShippingPoint LoadShippingPointInfo(string pointId)
		{
			var pickPoint = _sberlogisticApiService.GetPickUpPoint(pointId);
			return pickPoint != null ? CastPoint(pickPoint) : null;
		}

		private SberlogisticShippingPoint CastPoint(PickupPoint point)
		{
			return new SberlogisticShippingPoint
			{
				Id = point.Id.ToString(),
				Code = point.Id.ToString(),
				Name = point.Type,
				Address = point.AddressString,
				Description = point.TripDescription,
				Phones = point.Phones.ToArray(),
				Longitude = point.GeoLocation?.Lon,
				Latitude = point.GeoLocation?.Lat,
				AllowedCod = point.Options != null && (point.Options.Cod || point.Options.Card),
				AvailableCashOnDelivery = point.Options?.Cod,
				AvailableCardOnDelivery = point.Options?.Card,
				TimeWorkStr = point.Schedule?.Regular?.Readable,
				MaxWeightInGrams = point.Limits?.Weight,
				DimensionVolumeInCentimeters = point.Limits?.DimensionsSum,
				MaxHeightInMillimeters = point.Limits?.Height,
				MaxLengthInMillimeters = point.Limits?.Length,
				MaxWidthInMillimeters = point.Limits?.Width
			};
		}
	}
	public enum TypeViewPoints
	{
		[Localize("Списком")]
		List = 0,

		[Localize("Через виджет ПВЗ")]
		WidgetPVZ = 1,

		[Localize("Через Яндекс.Карты")]
		YaWidget = 2,

	}
}