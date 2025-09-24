using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using AdvantShop.Repository;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Shipping.Measoft.Api
{
    public class MeasoftXmlConverter
    {
        private const string OnlyLastChanges = "ONLY_LAST";
        private MeasoftAuthOption _authOption;

        public MeasoftXmlConverter(MeasoftAuthOption authOption)
        {
            _authOption = authOption;
        }

        #region calculate

        public string GetXmlCalculate(MeasoftCalcOptionParams calcOption, List<MeasoftItem> items)
        {
            if (calcOption.City.IsNullOrEmpty())
                return null;

            XDocument doc = new XDocument();
            XElement root = new XElement("calculator");
            doc.Add(root);
            root.Add(GetAuthElement());

            XElement order = new XElement("order",
                new XElement("receiver",
                    new XElement("town", calcOption.City,
                        new XAttribute("regioncode", calcOption.RegionCode)),
                    new XElement("address", calcOption.Address),
                    new XElement("pvz", calcOption.PvzCode)));

            var totalPrice = items.Sum(x => x.Price);
            order.Add(
                new XElement("weight", calcOption.Weight),
                new XElement("quantity", 1),
                new XElement("packages", 
                    new XElement("package", 
                        new XAttribute("mass", calcOption.Weight.ToInvariantString()), 
                        new XAttribute("length", calcOption.Dimensions[0]), 
                        new XAttribute("width", calcOption.Dimensions[1]), 
                        new XAttribute("height", calcOption.Dimensions[2]))),
                calcOption.WithPrice ? new XElement("price", totalPrice) : null,
                calcOption.WithInsure ? new XElement("inshprice", totalPrice) : null);
            if (calcOption.DeliveryServiceIds != null && calcOption.DeliveryServiceIds.Count == 1)
                order.Add(new XElement("service", calcOption.DeliveryServiceIds[0]));

            root.Add(order);

            return doc.ToString();
        }

        public List<MeasoftCalcOption> ParseAnswerCalculate(string response, int extraDeliveryDays)
        {
            var shippingOptions = new List<MeasoftCalcOption>();

            XDocument doc;
            var error = CheckResponseErrors(response, out doc);
            if (error.IsNotEmpty())
                return shippingOptions;

            foreach (var el in doc.Root.Elements("calc"))
            {
                var shippingOption = new MeasoftCalcOption();
                var price = el.Element("price").Value.TryParseFloat();
                shippingOption.BasePrice = price;

                var service = el.Element("service");
                var deliveryService = service.Value.TryParseInt();
                var serviceName = service.Attribute("name").Value;
                int minDeliveryTime = el.Element("mindeliverydays").Value.TryParseInt();
                int maxDeliveryTime = el.Element("maxdeliverydays").Value.TryParseInt();

                shippingOption.DeliveryTime = $"{serviceName}. " +
                    (maxDeliveryTime > minDeliveryTime
                        ? minDeliveryTime + extraDeliveryDays + "-"
                        : string.Empty)
                        + (maxDeliveryTime + extraDeliveryDays) + " дн.";

                shippingOption.DeliveryId = deliveryService;
                shippingOption.MinDeliveryDate = el.Element("mindeliverydate").Value.TryParseDateTime();

                shippingOptions.Add(shippingOption);
            }

            return shippingOptions;
        }

        public List<MeasoftPoint> ParseAnswerPoints(string response)
        {
            var pointList = new List<MeasoftPoint>();

            XDocument doc;
            var error = CheckResponseErrors(response, out doc);
            if (error.IsNotEmpty())
                return pointList;

            foreach (var el in doc.Root.Elements("pvz"))
                pointList.Add(ParseAnswerPoint(el));

            return pointList;
        }

        private MeasoftPoint ParseAnswerPoint(XElement el)
        {
            var phone = el.Element("phone").Value;
            var maxweight = el.Element("maxweight").Value.TryParseFloat(true);
            return new MeasoftPoint
            {
                Id = el.Element("code").Value,
                Code = el.Element("code").Value,
                Address = el.Element("address").Value,
                Description = el.Element("comment").Value,
                Phones = phone.IsNotEmpty()
                    ? new[] {phone}
                    : null,
                MaxWeightInGrams =
                    maxweight.HasValue
                        ? MeasureUnits.ConvertWeight(
                            maxweight.Value,
                            MeasureUnits.WeightUnit.Kilogramm,
                            MeasureUnits.WeightUnit.Grams)
                        : (float?) null,
                TimeWorkStr = el.Element("worktime").Value,
                AvailableCardOnDelivery = el.Element("acceptcard").Value == "YES",
                AvailableCashOnDelivery = el.Element("acceptcash").Value == "YES",
                Latitude = el.Element("latitude").Value.TryParseFloat(true),
                Longitude = el.Element("longitude").Value.TryParseFloat(true),
                ParentCode = el.Element("parentcode").Value.TryParseInt()
            };
        }

        public string GetXmlPoints(string region, string city)
        {
            XDocument doc = new XDocument(
                new XElement("pvzlist",
                    GetAuthElement(),
                    new XElement("town", 
                        new XAttribute("regioncode", region), city),
                    new XElement("acceptindividuals", "YES")));

            return doc.ToString();
        }

        #endregion

        #region create order

        public string GetXmlCreateOrder(Order order, float weight, int[] dimensionsInSm, int? paymentCodCardId)
        {
            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            var items = order.OrderItems;
            var calcOption = JsonConvert.DeserializeObject<MeasoftCalcOption>(order.OrderPickPoint.AdditionalData);
            var trackNumber = OrderService.GetOrderAdditionalData(order.OrderID, Measoft.TrackNumberOrderAdditionalDataName);

            XDocument doc = new XDocument();
            XElement root = new XElement("neworder",
                new XAttribute("newfolder", trackNumber.IsNotEmpty() ? "NO" : "YES"));
            root.Add(GetAuthElement());

            XElement orderEl = new XElement("order",
                new XAttribute("orderno", order.Number));
            XElement receiver = new XElement("receiver");

            var address = AggregateAddress(order.OrderCustomer.Street, order.OrderCustomer.House, order.OrderCustomer.Apartment,
                order.OrderCustomer.Structure, order.OrderCustomer.Entrance, order.OrderCustomer.Floor);

            if (order.OrderPickPoint != null && order.OrderPickPoint.PickPointId.IsNotEmpty())
                address = order.OrderPickPoint.PickPointId; // Код пвз можно указывать в теге адрес

            receiver.Add(
                new XElement("town", order.OrderCustomer.City),
                new XElement("address", address),
                new XElement("person", order.OrderRecipient.LastName + " " + order.OrderRecipient.FirstName),
                new XElement("phone", order.OrderRecipient.Phone),
                new XElement("zipcode", order.OrderCustomer.Zip));

            DateTime? date = null;
            if (order.DeliveryDate.HasValue)
                date = order.DeliveryDate.Value;
            if (date.HasValue)
                receiver.Add(new XElement("date", date.Value.ToString("yyyy-MM-dd")));

            if (order.DeliveryTime.IsNotEmpty())
            {
                // Добавляем желаемое время доставки
                if (order.DeliveryInterval.TimeFrom.HasValue)
                {
                    receiver.Add(new XElement("time_min", order.DeliveryInterval.TimeFrom.Value.ToString(@"hh\:mm")));
                    if (order.DeliveryInterval.TimeTo.HasValue)
                        receiver.Add(new XElement("time_max", order.DeliveryInterval.TimeTo.Value.ToString(@"hh\:mm")));
                }
                else
                {
                    var times = Regex.Replace(order.DeliveryTime, @"\D", " ")
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new TimeSpan(Convert.ToInt32(x), 0, 0))
                        .ToList();

                    if (times.Count > 0)
                        receiver.Add(new XElement("time_min", times[0].ToString(@"hh\:mm")));

                    if (times.Count > 1)
                        receiver.Add(new XElement("time_max", times[1].ToString(@"hh\:mm")));
                }
            }
            orderEl.Add(receiver);

            var vatRate = GetVatRate(shippingMethod.TaxType);
            var shippingCost = order.ShippingCostWithDiscount;
            var shippingCurrency = shippingMethod.ShippingCurrency;
            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var paymentsCash = new[]
                {
                    AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.CashOnDelivery)),
                    AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(typeof (Payment.Cash))
                };

            var mustPay = !order.Payed && order.PaymentMethod != null && paymentsCash.Contains(order.PaymentMethod.PaymentKey);
            var paymentType =
                mustPay
                ? order.PaymentMethodId == paymentCodCardId
                ? "CARD"
                : "CASH"
                : "NO";

            orderEl.Add(
                new XElement("weight", weight),
                new XElement("quantity", 1),
                new XElement("packages", 
                    new XElement("package", 
                        new XAttribute("mass", weight.ToInvariantString()), 
                        new XAttribute("length", dimensionsInSm[0]), 
                        new XAttribute("width", dimensionsInSm[1]), 
                        new XAttribute("height", dimensionsInSm[2]))),
                new XElement("receiverpays", "NO"),
                new XElement("deliveryprice", mustPay ? shippingCost : 0, 
                    new XAttribute("VATrate", vatRate)),
                new XElement("service", calcOption.DeliveryId),
                new XElement("pickup", "NO"),
                new XElement("acceptpartially", "NO"),
                new XElement("paytype", paymentType),
                new XElement("return", "NO")
                );
            if (mustPay && (order.TotalDiscount > 0 || order.BonusCost > 0))
                orderEl.Add(new XElement("discount", Math.Round(order.TotalDiscount + order.BonusCost, 2)));

            XElement productItems = new XElement("items");
            foreach (var item in items)
            {
                XElement product = new XElement("item",
                    new XAttribute("quantity", item.Amount),
                    new XAttribute("retprice", mustPay ? Math.Round(item.Price, 2) : 0),
                    new XAttribute("inshprice", calcOption.WithInsure ? Math.Round(item.Price, 2) : 0),
                    new XAttribute("VATrate", item.TaxRate.HasValue ? item.TaxRate.Value : 0),
                    new XAttribute("barcode", item.BarCode),
                    item.Name);
                productItems.Add(product);
            }

            orderEl.Add(productItems);
            root.Add(orderEl);
            doc.Add(root);

            return doc.ToString();
        }

        public MeasoftOrderTrackNumber ParseAnswerCreateOrder(string response)
        {
            XDocument doc;
            var error = CheckResponseErrors(response, out doc, "createorder");
            if (error.IsNotEmpty())
                return new MeasoftOrderTrackNumber() { Error = error };

            var createOrder = doc.Root.Element("createorder");

            var result = new MeasoftOrderTrackNumber();
            result.Barcode = createOrder.Attribute("barcode").Value;
            result.Number = createOrder.Attribute("orderno").Value;
            result.OrderPrice = createOrder.Attribute("orderprice").Value.TryParseInt();

            return result;
        }

        #endregion

        #region delete order

        public string GetXmlDeleteOrder(string orderNo)
        {
            XDocument doc = new XDocument(
                new XElement("cancelorder",
                    GetAuthElement(),
                    new XElement("order", new XAttribute("orderno", orderNo))));

            return doc.ToString();
        }

        public MeasoftDeleteOrderResult ParseAnswerDeleteOrder(string response)
        {
            XDocument doc;
            var error = CheckResponseErrors(response, out doc, "order");
            if (error.IsNotEmpty())
                return new MeasoftDeleteOrderResult() { Error = error };

            var order = doc.Root.Element("order");

            var result = new MeasoftDeleteOrderResult();
            result.Result = order.Attribute("errormsg").Value;

            return result;
        }

        #endregion

        #region sync orders

        public string GetXmlSyncOrderStatus(string trackNumber)
        {
            XDocument doc = new XDocument(
                new XElement("statusreq",
                    GetAuthElement(),
                    new XElement("orderno", trackNumber)
                    ));

            return doc.ToString();
        }

        public string GetXmlSyncOrderStatus()
        {
            XDocument doc = new XDocument(
                new XElement("statusreq",
                    GetAuthElement(),
                    new XElement("changes", OnlyLastChanges)
                    ));

            return doc.ToString();
        }

        public MeasoftOrderStatus ParseAnswerOrderStatus(string response)
        {
            XDocument doc;
            var error = CheckResponseErrors(response, out doc);
            if (error.IsNotEmpty())
                return new MeasoftOrderStatus() { Error = error };

            var order = doc.Root.Element("order");
            if (order == null)
                return new MeasoftOrderStatus() { Error = "Заказ не найден" };

            var statusEl = order.Element("status");
            var result = new MeasoftOrderStatus() { OrderNo = order.Attribute("orderno").Value };
            EMeasoftStatus status;
            if (Enum.TryParse(statusEl.Value, out status))
                result.Status = status;
            else
                result.Error = "Неизвестный статус в системе Measoft: " + statusEl.Value;

            return result;
        }

        public List<MeasoftOrderStatus> ParseAnswerOrderStatusList(string response)
        {
            XDocument doc;
            var error = CheckResponseErrors(response, out doc);
            if (error.IsNotEmpty())
                return new List<MeasoftOrderStatus> { new MeasoftOrderStatus { Error = error } };

            var result = new List<MeasoftOrderStatus>();
            foreach (var order in doc.Root.Elements("order"))
            {
                if (order == null)
                    continue;

                var orderStatus = new MeasoftOrderStatus() { OrderNo = order.Attribute("orderno").Value };
                var statusEl = order.Element("status");
                EMeasoftStatus status;
                if (Enum.TryParse<EMeasoftStatus>(statusEl.Value, out status))
                    orderStatus.Status = status;
                else
                    orderStatus.Error = "Неизвестный статус в системе Measoft: " + statusEl.Value;
                result.Add(orderStatus);
            }
            
            return result;
        }

        public string GetXmlCommitStatuses()
        {
            XDocument doc = new XDocument(
                new XElement("commitlaststatus",
                    GetAuthElement()
                ));

            return doc.ToString();
        }

        #endregion

        #region private methods

        private XElement GetAuthElement() => XElement.Parse(SerializeData(_authOption, false));

        private static string CheckResponseErrors(string response, out XDocument doc, string errorElementName = "error") 
        {
            doc = null;
            if (response.IsNullOrEmpty())
                return "Measoft вернул пустой ответ";

            doc = XDocument.Parse(response);
            if (doc == null)
                return "Не удаёётся создать документ";
            if (doc.Root == null)
                return "Вернулся пустой xml";

            var errorEl = doc.Root.Element(errorElementName);

            string errorCode;
            if (errorEl != null && (errorCode = errorEl.Attribute("error")?.Value) != "0")
            {
                var errorMessage = errorEl.Attribute("errormsg")?.Value;
                var error = $"Measoft: {errorMessage} with error code {errorCode}";
                Debug.Log.Warn(error);

                return error;
            }

            return null;
        }

        #endregion

        #region GetDeliveryServices

        public static string GetXmlDeliveryServices(string extra)
        {
            XDocument doc = new XDocument(
                new XElement("services",
                    new XElement("auth",
                        new XAttribute("extra", extra))));

            return doc.ToString();
        }

        public static List<MeasoftDeliveryService> ParseAnswerDeliveryServices(string response)
        {
            var deliveryServices = new List<MeasoftDeliveryService>();

            XDocument doc;
            var error = CheckResponseErrors(response, out doc);
            if (error.IsNotEmpty())
                return deliveryServices;

            foreach (var el in doc.Root.Elements("service"))
            {
                var service = new MeasoftDeliveryService();

                var code = el.Element("code").Value.TryParseInt();
                var name = el.Element("name").Value;
                service.Name = name;
                service.Code = code;

                deliveryServices.Add(service);
            }

            return deliveryServices;
        }

        #endregion

        #region Help Methods

        private int GetVatRate(TaxType taxType)
        {
            switch (taxType)
            {
                case TaxType.Vat0:
                    return 0;
                case TaxType.Vat10:
                    return 10;
                case TaxType.Vat18:
                    return 18;
                case TaxType.Vat20:
                    return 20;
                case TaxType.Vat5:
                    return 5;
                case TaxType.Vat7:
                    return 7;
                default:
                    return -0;
            }
        }

        private string AggregateAddress(string street, string house, string apartment, string structure, string entrance, string floor)
        {
            var address = new List<string>(6);

            if (street.IsNotEmpty())
                address.Add($" ул. {street}");
            if (house.IsNotEmpty())
                address.Add($" д. {house}");
            if (structure.IsNotEmpty())
                address.Add($" стр. {structure}");
            if (apartment.IsNotEmpty())
                address.Add($" кв. {apartment}");
            if (entrance.IsNotEmpty())
                address.Add($" подъезд {entrance}");
            if (floor.IsNotEmpty())
                address.Add($" этаж {floor}");

            return string.Join(", ", address);
        }

        #endregion
        
        public void WriteDataToStream<T>(Stream stream, T data, bool writeXmlDeclaration = true)
        {
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, CheckCharacters = true, OmitXmlDeclaration = !writeXmlDeclaration }))
            {
                if (data != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                    ns.Add("", "");
                    serializer.Serialize(writer, data, ns);
                }
            }
        }
        
        public string SerializeData<T>(T data, bool writeXmlDeclaration = true)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteDataToStream(stream, data, writeXmlDeclaration);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }

    }
}
