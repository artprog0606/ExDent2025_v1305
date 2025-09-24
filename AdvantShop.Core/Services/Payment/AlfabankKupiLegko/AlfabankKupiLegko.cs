using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("AlfabankKupiLegko")]
    public class AlfabankKupiLegko : PaymentMethod, ICreditPaymentMethod
    {
        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        public static string Inn { get; set; }
        public static string PromoCode { get; set; }
        public bool TestMode { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {AlfabankKupiLegkoTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {AlfabankKupiLegkoTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {AlfabankKupiLegkoTemplate.FirstPayment, FirstPayment.ToInvariantString()},
                               {AlfabankKupiLegkoTemplate.Inn, Inn},
                               {AlfabankKupiLegkoTemplate.PromoCode, PromoCode},
                               {AlfabankKupiLegkoTemplate.TestMode, TestMode.ToString()}
                           };
            }
            set
            {
                MinimumPrice = value.ElementOrDefault(AlfabankKupiLegkoTemplate.MinimumPrice).TryParseFloat();
                MaximumPrice = value.ElementOrDefault(AlfabankKupiLegkoTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(AlfabankKupiLegkoTemplate.FirstPayment).TryParseFloat();
                Inn = value.ElementOrDefault(AlfabankKupiLegkoTemplate.Inn) ?? "";
                PromoCode = value.ElementOrDefault(AlfabankKupiLegkoTemplate.PromoCode) ?? "";
                TestMode = value.ElementOrDefault(AlfabankKupiLegkoTemplate.TestMode).TryParseBool();
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }
      
        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;
  
        public override PaymentForm GetPaymentForm(Order order)
        {
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://anketa.alfabank.ru/alfaform-pos/endpoint",
                InputValues = GetParam(order, TestMode)
            };
        }

        private NameValueCollection GetParam(Order order, bool isTestMode)
        {
            var result = new NameValueCollection();

            if (isTestMode)
                result.Add(name: "testMode", value: "true");

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {
                    xmlWriter.WriteStartElement("inParams");


                    xmlWriter.WriteStartElement("companyInfo");

                    xmlWriter.WriteStartElement("inn");
                    xmlWriter.WriteString(Inn);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("referer");
                    xmlWriter.WriteString(SettingsMain.SiteUrl);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();//companyInfo


                    xmlWriter.WriteStartElement("creditInfo");

                    xmlWriter.WriteStartElement("reference");
                    xmlWriter.WriteString(!isTestMode ? order.OrderID.ToString() : Guid.NewGuid().ToString().Reduce(16));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("firstPayment");
                    xmlWriter.WriteString(FirstPayment.ToString(CultureInfo.InvariantCulture));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();//creditInfo


                    if (order.OrderCustomer != null)
                    {
                        xmlWriter.WriteStartElement("clientInfo");

                        if (!string.IsNullOrEmpty(order.OrderCustomer.LastName))
                        {
                            xmlWriter.WriteStartElement("lastname");
                            xmlWriter.WriteString(order.OrderCustomer.LastName.Reduce(35));
                            xmlWriter.WriteEndElement();
                        }

                        if (!string.IsNullOrEmpty(order.OrderCustomer.FirstName))
                        {
                            xmlWriter.WriteStartElement("firstname");
                            xmlWriter.WriteString(order.OrderCustomer.FirstName.Reduce(35));
                            xmlWriter.WriteEndElement();
                        }

                        if (!string.IsNullOrEmpty(order.OrderCustomer.Patronymic))
                        {
                            xmlWriter.WriteStartElement("middlename");
                            xmlWriter.WriteString(order.OrderCustomer.Patronymic.Reduce(35));
                            xmlWriter.WriteEndElement();
                        }

                        if (!string.IsNullOrEmpty(order.OrderCustomer.Patronymic))
                        {
                            xmlWriter.WriteStartElement("email");
                            xmlWriter.WriteString(order.OrderCustomer.Email.Reduce(140));
                            xmlWriter.WriteEndElement();
                        }

                        if (order.OrderCustomer.StandardPhone.HasValue)
                        {
                            var phone = order.OrderCustomer.StandardPhone.Value.ToString();

                            if (phone.Length == 10 || phone.Length == 11)
                            {
                                if (phone.Length == 11)
                                    phone = phone.Substring(1);

                                xmlWriter.WriteStartElement("mobphone");
                                xmlWriter.WriteString(phone);
                                xmlWriter.WriteEndElement();
                            }
                        }

                        xmlWriter.WriteEndElement(); //clientInfo
                    }


                    xmlWriter.WriteStartElement("specificationList");

                    var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
                    var orderItems = order.GetOrderItemsForFiscal(paymentCurrency);
                    foreach (var item in orderItems)
                    {
                        xmlWriter.WriteStartElement("specificationListRow");

                        var poduct = item.ProductID.HasValue ? ProductService.GetProduct(item.ProductID.Value) : null;
                        xmlWriter.WriteStartElement("category");
                        xmlWriter.WriteString(
                            poduct != null
                                ? poduct.MainCategory != null ? poduct.MainCategory.CategoryId.ToString() : string.Empty
                                : string.Empty
                            );
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("code");
                        xmlWriter.WriteString(item.ArtNo.Reduce(20));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("description");
                        xmlWriter.WriteString(item.Name.Reduce(50));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("amount");
                        xmlWriter.WriteString(item.Amount.ToString(CultureInfo.InvariantCulture));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("price");
                        xmlWriter.WriteString(Math.Round(item.Price, 2).ToString(CultureInfo.InvariantCulture));
                        xmlWriter.WriteEndElement();

                        if (!string.IsNullOrEmpty(PromoCode))
                        {
                            xmlWriter.WriteStartElement("action");
                            xmlWriter.WriteString(PromoCode);
                            xmlWriter.WriteEndElement();
                        }

                        if (item.Photo != null)
                        {
                            xmlWriter.WriteStartElement("image");
                            xmlWriter.WriteString(item.Photo.ImageSrcXSmall());
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();//specificationListRow
                    }

                    if (order.OrderCertificates != null)
                    { 
                        foreach (var item in order.OrderCertificates.ConvertCurrency(order.OrderCurrency, paymentCurrency))
                        {
                            xmlWriter.WriteStartElement("specificationListRow");

                            xmlWriter.WriteStartElement("category");
                            xmlWriter.WriteString(LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"));
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("code");
                            xmlWriter.WriteString(LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName").Reduce(20));
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("description");
                            xmlWriter.WriteString(LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName").Reduce(50));
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("amount");
                            xmlWriter.WriteString("1");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("price");
                            xmlWriter.WriteString(Math.Round(item.Sum, 2).ToString(CultureInfo.InvariantCulture));
                            xmlWriter.WriteEndElement();

                            if (!string.IsNullOrEmpty(PromoCode))
                            {
                                xmlWriter.WriteStartElement("action");
                                xmlWriter.WriteString(PromoCode);
                                xmlWriter.WriteEndElement();
                            }

                            xmlWriter.WriteEndElement();//specificationListRow
                        }
                    }

                    xmlWriter.WriteEndElement();//specificationList


                    xmlWriter.WriteEndElement();//inParams

                    result.Add(name: "InXML", value: sw.ToString());
                }
            }

            return result;
        }

    }
}
