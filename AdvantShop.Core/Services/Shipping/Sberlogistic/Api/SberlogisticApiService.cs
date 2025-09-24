using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Shipping.Sberlogistic.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdvantShop.Shipping.Sberlogistic
{
	public class SberlogisticApiService
	{
		private readonly string _authorizationToken;

		public JsonSerializerSettings SerializationSettings { get; set; }
		public JsonSerializerSettings DeserializationSettings { get; set; }

		private const string Url = "https://module-gateway.sblogistica.ru/api/v1";
		//private const string Url = "https://module-gateway-dev.sblogistica.ru"; //тестовый

		public SberlogisticApiService(string authorizationToken)
		{
			_authorizationToken = authorizationToken;
			SerializationSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore,
				ContractResolver = new DefaultContractResolver()
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				}

			};
			DeserializationSettings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				ContractResolver = new DefaultContractResolver()
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				}

			};
		}

		public SberlogisticContract GetContract()
		{
			var url = Url + "/contracts";
			try
			{
				var headers = new Dictionary<string, string> { { "Authorization", _authorizationToken } };
				var contract = RequestHelper.MakeRequest<List<SberlogisticContract>>(url, headers: headers, method: ERequestMethod.GET);
				return contract.FirstOrDefault();
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public List<TariffCalculation> GetTariffs(Tariff tariffParams)
		{
			var url = Url + "/tariffs/calculate";
			try
			{
				var contract = GetContract();
				if (contract != null && contract.IndividualServices != null && contract.IndividualServices.Count > 0)
					tariffParams.Customer.Id = contract.IndividualServices.FirstOrDefault().Uuid;
				var headers = new Dictionary<string, string> { { "Authorization", _authorizationToken } };
				var result = RequestHelper.MakeRequest<GetTariffResponse>(url, tariffParams, headers, ERequestMethod.POST, ERequestContentType.TextJsonUtf8, jsonSettings: SerializationSettings);
				return result.TariffCalculations;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public IEnumerable<PickupPoint> GetPickUpPoints(PickUpPointParams pointParams)
		{
			string url = Url + "/deliveryPoint/list?mode=sberlogistics_pickpoint&delivery_available=1";
			if(pointParams is null)
				return new EnumeratObject<PickupPoint, GetPickupPointResponse>(url, 2000, true);

			string urlWithParams = string.Format("{0}{1}{2}&max_weight={3}&max_length={4}&max_width={5}&max_height={6}{7}",
					url,
					pointParams.CardPayment.HasValue 
						? "&card_payment=" + pointParams.CardPayment 
						: string.Empty,
					pointParams.CashPayment.HasValue 
						? "&cash_payment=" + pointParams.CashPayment 
						: string.Empty,
					pointParams.Weight,
					pointParams.Length,
					pointParams.Width,
					pointParams.Height,
					string.IsNullOrEmpty(pointParams.KladrSettlement) && !string.IsNullOrEmpty(pointParams.SettlementAddress)
						? "&settlement_address=" + pointParams.SettlementAddress
						: !string.IsNullOrEmpty(pointParams.KladrSettlement)
							? "&kladr_settlement=" + pointParams.KladrSettlement
							: string.Empty
				);
			return new EnumeratObject<PickupPoint, GetPickupPointResponse>(urlWithParams, pointParams.PageCount ?? 2000, true);
		}

		public PickupPoint GetPickUpPoint(string id)
		{
			string url = $"{Url}/deliveryPoint/{id}";
			try
			{
				var result = RequestHelper.MakeRequest<PickupPoint>(url, method: ERequestMethod.GET);
				return result;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public ShipmentResponse CreateShipment(Shipment shipmentParams)
		{
			string url = Url + "/shipments";
			try
			{
				var headers = new Dictionary<string, string> { { "Authorization", _authorizationToken } };
				var response = RequestHelper.MakeRequest<ShipmentResponse>(url, shipmentParams, headers, ERequestMethod.POST, jsonSettings: SerializationSettings);
				return response;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public ShipmentResponse UpdateOrderDraft(Shipment shipmentParams, string uuid)
		{
			string url = Url + "/shipments/" + uuid;
			try
			{
				var headers = new Dictionary<string, string> { { "Authorization", _authorizationToken } };
				var response = RequestHelper.MakeRequest<ShipmentResponse>(url, shipmentParams, headers, ERequestMethod.PUT, jsonSettings: SerializationSettings);
				return response;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public bool DeleteOrderDraft(string uuid)
		{
			string url = Url + "/shipments/" + uuid;
			try
			{
				var headers = new Dictionary<string, string> { { "Authorization", _authorizationToken } };
				var response = RequestHelper.MakeRequest<string>(url, headers: headers, method: ERequestMethod.DELETE);
				return true;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return false;
		}

		public ListStatuses GetListStatuses()
		{
			string url = Url + "/package/statuses/list";
			try
			{
				var response = RequestHelper.MakeRequest<ListStatuses>(url, method: ERequestMethod.GET);
				return response;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}

		public ListPackageStatuses GetListPackageStatuses(string[] listTrackNumbers)
		{
			string trackNumbers = string.Join(",", listTrackNumbers);
			string url = $"{Url}/package/statuses/current?track_numbers={trackNumbers}";
			try
			{
				var response = RequestHelper.MakeRequest<ListPackageStatuses>(url, method: ERequestMethod.GET);
				return response;
			}
			catch (Exception ex)
			{
				Debug.Log.Error(Regex.Unescape(ex.Message));
			}
			return null;
		}
	}

	public class EnumeratObject<T, TR> : IEnumerator<T>, IEnumerable<T>
		where T : class
		where TR : IGetObjectValues<T>
	{
		private List<T> _collection;
		private int _index;
		private readonly string _url;
		private int _limit;
		private readonly bool _keepActualCollection; // хранить только последнюю полученную коллекцию, чтобы не хранить всю коллекцию в памяти
		private int _page;
		private T _current;

		private EnumeratObject()
		{
			_index = -1;
		}

		/// <param name="url">Размещение списка</param>
		/// <param name="limit">Максимальное количество сущностей для извлечения.</param>
		/// <param name="keepActualCollection">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>
		public EnumeratObject(string url, int limit = 2000, bool keepActualCollection = false) : this()
		{
			_url = url;
			_limit = limit;
			_keepActualCollection = keepActualCollection;
			_page = 1;
		}

		public bool MoveNext()
		{
			if (_collection == null || (_index + 1 >= _collection.Count))
			{
				string url = string.Format("{0}{1}page={2}&page_count={3}", _url, (_url.Contains("?") ? "&" : "?"), _page, _limit);
				var response = RequestHelper.MakeRequest<TR>(url, method: ERequestMethod.GET);
				if (response?.Values?.Count > 0)
				{
					_collection = _collection ?? (_collection = new List<T>());
					if (_keepActualCollection)
					{
						_collection.Clear();
						_index = -1;
					}

                    _collection.AddRange(response.Values);
					_page++;
				}
			}
			if (_collection == null || ++_index >= _collection.Count)
			{
				return false;
			}
			_current = _collection[_index];
			return true;
		}

		public void Reset()
		{
			_index = -1;
			if (_keepActualCollection)
			{
				_collection = null;
				_page = 0;
			}
		}

		public T Current
		{
			get { return _current; }
		}


		object IEnumerator.Current
		{
			get { return Current; }
		}

		public void Dispose() { }

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}