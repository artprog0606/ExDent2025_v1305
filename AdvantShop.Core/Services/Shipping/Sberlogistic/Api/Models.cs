using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Sberlogistic.Api
{
	public interface IGetObjectValues<T>
	where T : class
	{
		List<T> Values { get; set; }
	}

	public enum DeliveryType
	{
        [Localize("Курьер")]
		Courier = 0,
		[Localize("ПВЗ")]
		PVZ = 1
	}


	#region Contract
	public class SberlogisticContract
	{
		/// <summary>
		/// Уникальный идентификатор договора в организации(аккаунте ЛК).
		/// </summary>
		public string Uuid { get; set; }
		/// <summary>
		/// Наименование договора
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Дата начала действия договора
		/// </summary>
		public DateTime StartDate { get; set; }
		/// <summary>
		/// Способы доставки, доступные по договору.
		/// </summary>
		public List<string> MailTypes { get; set; }
		/// <summary>
		/// ИНУ, доступные по договору
		/// </summary>
		public List<IndividualService> IndividualServices { get; set; }

	}

	public class IndividualService
	{
		/// <summary>
		/// Уникальный идентификатор ИНУ в договоре
		/// </summary>
		public string Uuid { get; set; }
		/// <summary>
		/// Наименование ИНУ
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Способ доставки, для которого действует ИНУ.
		/// </summary>
		public string Type { get; set; }
	}

	#endregion

	#region tariffs and delivery times

	public class Tariff
	{
		/// <summary>
		/// Уникальный пользовательский идентификатор объекта расчета
		/// </summary>
		public string Uid { get; set; }
		/// <summary>
		/// Место отправления
		/// </summary>
		public DeliveryPoint DeparturePoint { get; set; }
		/// <summary>
		/// Место назначения
		/// </summary>
		public DeliveryPoint2 DeliveryPoint { get; set; }
		/// <summary>
		/// Описание ВГХ грузовых мест
		/// </summary>
		public List<CargoVolume> CargoVolumes { get; set; }
		/// <summary>
		/// Сумма объявленной ценности
		/// </summary>
		public int DeclaredValue { get; set; } //Обязательно для отправлений с наложенным платежом
		/// <summary>
		/// Данные отправителя
		/// </summary>
		public CustomerData Customer { get; set; }
		/// <summary>
		/// Сумма наложенного платежа
		/// </summary>
		public int? CashOnDelivery { get; set; }
		/// <summary>
		/// Код способа оплаты наложенного платежа
		/// </summary>
		public ECashOnDeliveryType CashOnDeliveryType { get; set; }
		/// <summary>
		/// Коды способов доставки, по которым нужен расчет.  Возвращается успешный результат или список ошибок. Если параметр не был передан, то возвращаются только успешные расчеты.
		/// </summary>
		public List<int> ServiceCodes { get; set; } // 1 - СберПосылка, 2 - СберКурьер
	}

	public enum ECashOnDeliveryType
	{
		CASH,
		CARD
	}

	public class CustomerData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public eSenderTypeCode Type { get; set; }
		/// <summary>
		/// ID индивидуального тарифа Клиента
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Приоритет индивидуальных тарифов при расчете тарифа доставки
		/// <remarks>true — только индивидуальный тариф, т.е. при расчете не будут использоваться общие тарифы. Если его нет, возвращается ошибка</remarks>
		/// </summary>
		public bool IndividualRateOnly { get; set; }
	}

	public enum eSenderTypeCode
	{
		ORGANIZATION
	}

	public class CargoVolume
	{
		public int Weight { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}

	public class DeliveryPoint
	{
		/// <summary>
		/// Тип адреса
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public eAddressType AddressType { get; set; }
		public string Address { get; set; }
		public string String { get; set; }
		public string Fias { get; set; }
		public string Kladr { get; set; }
	}

	public class DeliveryPoint2
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public eAddressType AddressType { get; set; }
		public AddressParams AddressParams { get; set; }
		public string Address { get; set; }

		public string String { get; set; }
		public string Fias { get; set; }
		public string Kladr { get; set; }
	}

	public class AddressParams
	{
		public string Area { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Region { get; set; }
		public string Settlement { get; set; }
	}

	public enum eAddressType
	{
		FIAS_ID,
		KLADR_ID,
		STRING //адресная строка
	}

	public class GetTariffResponse
	{
		public List<TariffCalculation> TariffCalculations { get; set; }
	}

    public class TariffCalculation
	{
		/// <summary>
		/// Дата начала действия тарифа. Формат YYYY-MM-DD.
		/// </summary>
		public string TariffStartDate { get; set; }
		/// <summary>
		/// Сроки доставки
		/// </summary>
		public DeliveryDate Delivery { get; set; }
		/// <summary>
		/// Код вида отправления
		/// </summary>
		public int ServiceCode { get; set; }
		/// <summary>
		/// Детализация расчета
		/// </summary>
		public CalculationDetail CalculationDetail { get; set; }
		/// <summary>
		/// Плата за пересылку
		/// </summary>
		public ShippingFee ShippingFee { get; set; }
		/// <summary>
		/// Плата за объявленную ценность
		/// </summary>
		public ShippingFee DeclaredValueFee { get; set; }
		/// <summary>
		/// Итого за пересылку. Включает: плату за пересылку, плату за объявленную ценность, плату за дополнительные услуги.
		/// </summary>
		public ShippingFee TotalSum { get; set; }
		/// <summary>
		/// Плата за наложенный платеж
		/// </summary>
		public ShippingFee CashOnDeliveryFee { get; set; }
	}

	public class DeliveryDate
	{
		public int Min { get; set; }
		public int Max { get; set; }

	}

	public class ShippingFee
	{
		public int PriceWithoutVat { get; set; }
		public int VatPrice { get; set; }
		public int PriceWithVat { get; set; }
	}

	public class CalculationDetail
	{
		/// <summary>
		/// Реальное место отправления, определенное в процессе расчета тарифа.
		/// </summary>
		public DeliveryPoint DeparturePoint { get; set; }
		/// <summary>
		/// Реальное место назначения, определенное в процессе расчета тарифа.
		/// </summary>
		public DeliveryPoint DeliveryPoint { get; set; }
		/// <summary>
		/// Признак того, что расчет возвращен для индивидуальных тарифов.
		/// </summary>
		public bool IsIndividual { get; set; }
	}

	#endregion

	#region List pick up points

	public class GetPickupPointResponse : IGetObjectValues<PickupPoint>
	{
		[JsonProperty("delivery_point_list")]
		public List<PickupPoint> Values { get; set; }
	}

	public class PickupPoint
	{
		public int Id { get; set; }
		public string Type { get; set; }
		public string Courier { get; set; }

		[JsonProperty("geo_location")]
		public GeoLocation GeoLocation { get; set; }
		public List<string> Phones { get; set; }

		[JsonProperty("trip_description")]
		public string TripDescription { get; set; }
		public Schedule Schedule { get; set; }
		public Address Address { get; set; }

		[JsonProperty("address_string")]
		public string AddressString { get; set; }
		/// <summary>
		/// Способы оплаты доступные в точке выдачи
		/// </summary>
		public Option Options { get; set; }

		[JsonConverter(typeof(SberlogisticArrayToObjectConverter<PickPointLimits>))]
		public PickPointLimits Limits { get; set; }
	}

  public class PickPointLimits
	{
		/// <summary>
		/// Вес, граммы
		/// </summary>
		public float? Weight { get; set; }
		/// <summary>
		/// Длина, см
		/// </summary>
		public float? Length { get; set; }
		/// <summary>
		/// Ширина, см
		/// </summary>
		public float? Width { get; set; }
		/// <summary>
		/// Высота, см
		/// </summary>
		public float? Height { get; set; }
		/// <summary>
		/// Сумма трех сторон отправления, см
		/// </summary>
		public float? DimensionsSum { get; set; }
		/// <summary>
		/// Максимальнаая сумма наложенного платежа, руб
		/// </summary>
		public int? COD { get; set; }
	}

    public class Option
	{
		public bool Cod { get; set; }
		public bool Card { get; set; }
	}

	public class Address
	{
		public string Country { get; set; }

		[JsonProperty("administrative_area")]
		public string AdministrativeArea { get; set; }
		public string Settlement { get; set; }
		public string Street { get; set; }
		public string House { get; set; }
	}

	public class Schedule
	{
		public Regular Regular { get; set; }
	}

	public class Regular
	{
		public string Readable { get; set; }
	}

	public class GeoLocation
	{
		public float Lat { get; set; }
		public float Lon { get; set; }
	}

	public class PickUpPointParams
	{
		public int? Page { get; set; }
		public int? PageCount { get; set; }
		/// <summary>
		/// Адрес доставки. Обязательный, если не указан kladr_settlement
		/// </summary>
		public string SettlementAddress { get; set; }
		/// <summary>
		/// Одиннадцатизначный  КЛАДР-код населенного пункта доставки. Обязательный, если не указан settlement_address
		/// </summary>
		public string KladrSettlement { get; set; }
		/// <summary>
		/// Флаг того, что точка поддерживает оплату наличными (0/1).
		/// </summary>
		public int? CashPayment { get; set; }
		/// <summary>
		/// Флаг того, что точка поддерживает оплату по банковской карте (0/1).
		/// </summary>
		public int? CardPayment { get; set; }
		/// <summary>
		/// Способ доставки (sberlogistics_pickpoint)
		/// </summary>
		public string Mode { get; set; }
		public int Weight { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		/// <summary>
		/// Возможность выдачи отправлений(0/1)
		/// </summary>
		public int DeliveryAvailable { get; set; }
	}

	#endregion

	#region CreateShipment

	public class Shipment
	{
		public string OrderNumber { get; set; }
		public string CourierAddress { get; set; }
		/// <summary>
		/// Адрес пункта выдачи отправления
		/// </summary>
		public string DeliveryPointAddress { get; set; }
		/// <summary>
		/// Идентификатор пункта выдачи отправления
		/// </summary>
		public int DeliveryPointId { get; set; }
		/// <summary>
		/// Адрес доставки отправления курьером
		/// </summary>
		public string RecipientAddress { get; set; }
		/// <summary>
		/// ФИО получателя отправления
		/// </summary>
		public string RecipientName { get; set; }
		/// <summary>
		/// Номер телефона получателя отправления
		/// </summary>
		public string RecipientNumber { get; set; }
		public int Weight { get; set; }
		public Dimensions Dimensions { get; set; }
		/// <summary>
		/// Товарные вложения и услуги внутри отправления
		/// </summary>
		public List<Item> Items { get; set; }
		public int CashOnDelivery { get; set; }
		/// <summary>
		/// Объявленная стоимость отправления, коп.
		/// </summary>
		public int InsuranceSum { get; set; }
		/// <summary>
		/// Способ доставки отправления
		/// </summary>
		public string MailType { get; set; } //SBER_COURIER || SBER_PACKAGE

	}

	public class Item
	{
		/// <summary>
		/// Цена за единицу, коп.
		/// </summary>
		public int CostPerUnit { get; set; }
		/// <summary>
		/// Наименование товара или услуги
		/// </summary>
		public string Name { get; set; }
		public int Quantity { get; set; }
		/// <summary>
		/// НДС на товар или услуги в позиции
		/// </summary>
		public int VatRate { get; set; }
	}

	public class Dimensions
	{
		public int Height { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
	}

	public class ShipmentResponse
	{
		/// <summary>
		/// Уникальный идентификатор отправления
		/// </summary>
		public string Uuid { get; set; }
		/// <summary>
		/// Трек-номер отправления
		/// </summary>
		public string Barcode { get; set; }
		public List<ErrorResponse> ErrorDescriptions { get; set; }
	}

	public class ErrorResponse
	{
		public string Error { get; set; }
		public string Field { get; set; }
		public string Value { get; set; }
	}
	#endregion

	#region Statuses

	public class ListStatuses
	{
		public List<Status> Statuses { get; set; }
	}

	public class Status
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public List<string> Group { get; set; }
	}

	public class ListPackageStatuses
	{
		public List<PackageStatus> Statuses { get; set; }
	}

	public class PackageStatus
	{
		public string Id { get; set; }
		public string Status { get; set; }
		public bool Paid { get; set; }
	}

	#endregion
}
