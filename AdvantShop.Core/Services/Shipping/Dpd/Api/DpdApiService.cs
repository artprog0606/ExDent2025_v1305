using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AdvantShop.Shipping.Dpd.Api
{
    public class DpdApiService
    {
        private const string GeographyEndpointAddress = "https://ws.dpd.ru/services/geography2";
        private const string GeographyEndpointAddressTest = "https://wstest.dpd.ru/services/geography2";

        private const string CalculatorEndpointAddress = "https://ws.dpd.ru/services/calculator2";
        private const string CalculatorEndpointAddressTest = "https://wstest.dpd.ru/services/calculator2";

        private readonly bool testServers;
        private readonly long clientNumber;
        private readonly string clientKey;

        public List<string> LastActionErrors { get; set; }

        public DpdApiService(bool testServers, long clientNumber, string clientKey)
        {
            this.testServers = testServers;
            this.clientNumber = clientNumber;
            this.clientKey = clientKey;
        }

        private void ClearErrors()
        {
            LastActionErrors = null;
        }

        private void AddErrors(params string[] message)
        {
            if (LastActionErrors == null)
                LastActionErrors = new List<string>();

            LastActionErrors.AddRange(message);
        }

        #region Geography

        private Geography.auth geographyAuth;
        private Geography.auth GeographyAuth
        {
            get
            {
                if (geographyAuth == null)
                {
                    geographyAuth = new Geography.auth();
                    geographyAuth.clientKey = clientKey;
                    geographyAuth.clientNumber = clientNumber;
                }

                return geographyAuth;
            }
        }

        private Geography.DPDGeography2Client geographyClient;
        private Geography.DPDGeography2Client GeographyClient
        {
            get
            {
                if (geographyClient == null)
                {
                    BasicHttpBinding httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                    httpBinding.MaxReceivedMessageSize = 20 * 1024 * 1024;
                    //Specify the address to be used for the client.
                    EndpointAddress endpointAddress =
                       new EndpointAddress(testServers ? GeographyEndpointAddressTest : GeographyEndpointAddress);

                    geographyClient = new Geography.DPDGeography2Client(httpBinding, endpointAddress);
                }

                return geographyClient;
            }
        }

        public Geography.city[] GetCitiesCashPay(string countryCode = null)
        {
            try
            {
                ClearErrors();

                var request = new Geography.dpdCitiesCashPayRequest();
                request.auth = this.GeographyAuth;

                if (!string.IsNullOrEmpty(countryCode))
                    request.countryCode = countryCode;

                return this.GeographyClient.getCitiesCashPay(request);
            }
            catch (FaultException<Geography.WSFault> ex)
            {
                AddErrors(ex.Detail.code + ": " + ex.Detail.message);
                Debug.Log.Warn("Dpd GetParcelShops, " + ex.Detail.code + ": " + ex.Detail.message, ex);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);
            }

            return null;
        }

        public Geography.parcelShop[] GetParcelShops(string countryCode = null, string regionCode = null, string cityCode = null, string cityName = null)
        {
            try
            {
                ClearErrors();

                var request = new Geography.dpdParcelShopRequest();
                request.auth = this.GeographyAuth;

                if (!string.IsNullOrEmpty(countryCode))
                    request.countryCode = countryCode;

                if (!string.IsNullOrEmpty(regionCode))
                    request.regionCode = regionCode;

                if (!string.IsNullOrEmpty(cityCode))
                    request.cityCode = cityCode;

                if (!string.IsNullOrEmpty(cityName))
                    request.cityName = cityName;

                return this.GeographyClient.getParcelShops(request);
            }
            catch (FaultException<Geography.WSFault> ex)
            {
                AddErrors(ex.Detail.code + ": " + ex.Detail.message);
                Debug.Log.Warn("Dpd GetParcelShops, " + ex.Detail.code + ": " + ex.Detail.message, ex);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);
            }

            return null;
        }

        public Geography.terminalSelf[] GetTerminals()
        {
            try
            {
                ClearErrors();

                return this.GeographyClient.getTerminalsSelfDelivery2(this.GeographyAuth);
            }
            catch (FaultException<Geography.WSFault> ex)
            {
                AddErrors(ex.Detail.code + ": " + ex.Detail.message);
                Debug.Log.Warn("Dpd GetTerminals, " + ex.Detail.code + ": " + ex.Detail.message, ex);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);
            }

            return null;
        }

        #endregion

        #region Calculator


        private Calculator.auth calculatorAuth;
        private Calculator.auth CalculatorAuth
        {
            get
            {
                if (calculatorAuth == null)
                {
                    calculatorAuth = new Calculator.auth();
                    calculatorAuth.clientKey = clientKey;
                    calculatorAuth.clientNumber = clientNumber;
                }

                return calculatorAuth;
            }
        }

        private Calculator.DPDCalculatorClient calculatorClient;
        private Calculator.DPDCalculatorClient CalculatorClient
        {
            get
            {
                if (calculatorClient == null)
                {
                    BasicHttpBinding httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                    //Specify the address to be used for the client.
                    EndpointAddress endpointAddress =
                       new EndpointAddress(testServers ? CalculatorEndpointAddressTest : CalculatorEndpointAddress);

                    calculatorClient = new Calculator.DPDCalculatorClient(httpBinding, endpointAddress);
                }

                return calculatorClient;
            }
        }

        /// <summary>
        /// Рассчитать общую стоимость доставки по России и странам ТС.
        /// </summary>
        /// <param name="pickup">Город отправления</param>
        /// <param name="delivery">Город  доставки</param>
        /// <param name="selfPickup">Самопривоз на терминал</param>
        /// <param name="selfDelivery">Доставка до терминала. Самовывоз с терминала.</param>
        /// <param name="weight">Вес отправки, кг</param>
        /// <param name="volume">Объём, м3</param>
        /// <param name="declaredValue">Объявленная ценность груза</param>
        /// <param name="serviceCodes">Список кодов услуг DPD</param>
        /// <returns></returns>
        public Calculator.serviceCost[] GetServiceCost(Calculator.cityRequest pickup,
            Calculator.cityRequest delivery, bool selfPickup, bool selfDelivery,
            double weight, double? volume, double? declaredValue, string[] serviceCodes,
            out Calculator.ServiceCostFault2 serviceCostFault)
        {
            try
            {
                ClearErrors();

                var request = new Calculator.serviceCostRequest();
                request.auth = this.CalculatorAuth;

                request.pickup = pickup;
                request.delivery = delivery;
                request.selfPickup = selfPickup;
                request.selfDelivery = selfDelivery;
                request.weight = weight;

                if (volume.HasValue)
                {
                    request.volume = volume.Value;
                    request.volumeSpecified = true;
                }

                if (declaredValue.HasValue)
                { 
                    request.declaredValue = declaredValue.Value;
                    request.declaredValueSpecified = true;
                }

                if (serviceCodes != null && serviceCodes.Length > 0)
                    request.serviceCode = string.Join(",", serviceCodes);

                serviceCostFault = null;

                return this.CalculatorClient.getServiceCost2(request);
            }
            catch (FaultException<Calculator.ServiceCostFault2> ex)
            {
                serviceCostFault = ex.Detail;
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);

                serviceCostFault = null;
            }

            return null;
        }

        /// <summary>
        /// Рассчитать стоимость доставки по параметрам  посылок по России и странам ТС.
        /// </summary>
        /// <param name="pickup">Город отправления</param>
        /// <param name="delivery">Город  доставки</param>
        /// <param name="selfPickup">Самопривоз на терминал</param>
        /// <param name="selfDelivery">Доставка до терминала. Самовывоз с терминала.</param>
        /// <param name="declaredValue">Объявленная ценность груза</param>
        /// <param name="serviceCodes">Список кодов услуг DPD</param>
        /// <param name="parcels">Список посылок с параметрами для расчета</param>
        /// <returns></returns>
        public Calculator.serviceCost[] GetServiceCostByParcels(Calculator.cityRequest pickup,
            Calculator.cityRequest delivery, bool selfPickup, bool selfDelivery,
            double? declaredValue, string[] serviceCodes, Calculator.parcelRequest[] parcels,
            out Calculator.ServiceCostFault2 serviceCostFault)
        {
            try
            {
                ClearErrors();

                var request = new Calculator.serviceCostParcelsRequest();
                request.auth = this.CalculatorAuth;

                request.pickup = pickup;
                request.delivery = delivery;
                request.selfPickup = selfPickup;
                request.selfDelivery = selfDelivery;

                if (declaredValue.HasValue)
                {
                    request.declaredValue = declaredValue.Value;
                    request.declaredValueSpecified = true;
                }

                if (serviceCodes != null && serviceCodes.Length > 0)
                    request.serviceCode = string.Join(",", serviceCodes);

                if (parcels != null)
                    request.parcel = parcels;

                serviceCostFault = null;

                return this.CalculatorClient.getServiceCostByParcels2(request);
            }
            catch (FaultException<Calculator.ServiceCostFault2> ex)
            {
                serviceCostFault = ex.Detail;
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);

                serviceCostFault = null;
            }

            return null;
        }

        /// <summary>
        /// Рассчитать общую стоимость доставки по международным направлениям
        /// <para>Самовывоз/Самопривоз можно ставить true только если страна отправления/назначения равна Россия.</para>
        /// </summary>
        /// <param name="pickup">Город отправления</param>
        /// <param name="delivery">Город  доставки</param>
        /// <param name="selfPickup">Самопривоз на терминал</param>
        /// <param name="selfDelivery">Доставка до терминала. Самовывоз с терминала.</param>
        /// <param name="weight">Вес отправки, кг</param>
        /// <param name="length">Длина посылки, см</param>
        /// <param name="width">Ширина посылки, см</param>
        /// <param name="height">Высота посылки, см</param>
        /// <param name="declaredValue">Объявленная ценность груза</param>
        /// <param name="insurance">Дополнительное страховане</param>
        /// <returns></returns>
        public Calculator.serviceCostInternational[] GetServiceCostInternational(Calculator.cityInternationalRequest pickup,
            Calculator.cityInternationalRequest delivery, bool selfPickup, bool selfDelivery,
            double weight, long length, long width, long height,
            double? declaredValue, bool? insurance,
            out Calculator.ServiceCostFault serviceCostFault)
        {
            try
            {
                ClearErrors();

                var request = new Calculator.serviceCostInternationalRequest();
                request.auth = this.CalculatorAuth;

                request.pickup = pickup;
                request.delivery = delivery;
                request.selfPickup = selfPickup;
                request.selfDelivery = selfDelivery;
                request.weight = weight;
                request.length = length;
                request.width = width;
                request.height = height;

                if (declaredValue.HasValue)
                {
                    request.declaredValue = declaredValue.Value;
                    request.declaredValueSpecified = true;
                }

                if (insurance.HasValue)
                    // Дополнительное страхование берется с объявленной ценности, поэтому Insurance = true только если declaredValue >0
                    if (insurance.Value == false || (declaredValue.HasValue && declaredValue > 0))
                    { 
                        request.insurance = insurance.Value;
                        request.insuranceSpecified = true;
                    }

                serviceCostFault = null;

                return this.CalculatorClient.getServiceCostInternational(request);
            }
            catch (FaultException<Calculator.ServiceCostFault> ex)
            {
                serviceCostFault = ex.Detail;
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Warn(ex);

                serviceCostFault = null;
            }

            return null;
        }

        #endregion
    }
}
