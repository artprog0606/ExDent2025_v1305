using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;

using AdvantShop.Shipping.DDelivery;
using AdvantShop.Core.Services.Shipping.DDelivery;
using System.Net;
using System.IO;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("DDelivery")]
    public class DDeliveryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ApiKey
        {
            get { return Params.ElementOrDefault(DDeliveryTemplate.ApiKey); }
            set
            {
                //не работает проверка, выполняется всегда
                //if (Params.ElementOrDefault(DDeliveryTemplate.ApiKey) != value)
                //{
                //    var request = (HttpWebRequest)WebRequest.Create("https://sdk.ddelivery.ru/api/v1/integration/" + value + "/link.json");
                //    var response = (HttpWebResponse)request.GetResponse();
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {
                //        var responseString = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                //        if (!responseString.Contains("id"))
                //        {
                //            return;
                //        }
                //    }
                //    else if (response.StatusCode == HttpStatusCode.NotFound)
                //    {
                //        return;
                //    }
                //    response.Close();
                //}
                Params.TryAddValue(DDeliveryTemplate.ApiKey, value.DefaultOrEmpty());
            }
        }

        public string Token
        {
            get { return Params.ElementOrDefault(DDeliveryTemplate.Token); }
            set { Params.TryAddValue(DDeliveryTemplate.Token, value.DefaultOrEmpty()); }
        }

        public string ShopId
        {
            get { return Params.ElementOrDefault(DDeliveryTemplate.ShopId); }
            set { Params.TryAddValue(DDeliveryTemplate.ShopId, value.DefaultOrEmpty()); }
        }

        public bool CreateDraftOrder
        {
            //  get { return Params.ElementOrDefault(DDeliveryTemplate.CreateDraftOrder).TryParseBool(); }
            get { return true; }
            set { Params.TryAddValue(DDeliveryTemplate.CreateDraftOrder, value.ToString()); }
        }
        public bool GroupingShippingOptions
        {
            get { return Params.ElementOrDefault(DDeliveryTemplate.GroupingShippingOptions).TryParseBool(); }
            set { Params.TryAddValue(DDeliveryTemplate.GroupingShippingOptions, value.ToString()); }
        }

        public bool UseWidget
        {
            get { return Params.ElementOrDefault(DDeliveryTemplate.UseWidget).TryParseBool(); }
            set { Params.TryAddValue(DDeliveryTemplate.UseWidget, value.ToString()); }
        }

        //public List<DDeliveryObjectCompany> ListReceptionCompanies
        //{
        //    get
        //    {
        //        return new DDeliveryApiService(
        //            this.ApiKey,
        //            this.ShopId,
        //            this.ReceptionCompanyId,
        //            this.CreateDraftOrder,
        //            this.UseWidget,
        //            float.Parse(this.DefaultWeight),
        //            float.Parse(this.DefaultHeight),
        //            float.Parse(this.DefaultWidth),
        //            float.Parse(this.DefaultLength)).GetDeliveryCompanies()
        //            ?? new List<DDeliveryObjectCompany>();
        //    }
        //}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
