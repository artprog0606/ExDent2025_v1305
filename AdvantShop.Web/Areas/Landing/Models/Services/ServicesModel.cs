using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Landing.Pictures;

namespace AdvantShop.App.Landing.Models
{
    public class ServicesModel : IConvertibleBlockModel
    {
        public PhotoModel Picture { get; set; }
        public string Header { get; set; }

        [JsonProperty(PropertyName = "show_price")]
        public bool ShowPrice { get; set; }

        [JsonProperty(PropertyName = "show_button")]
        public bool ShowButton { get; set; }

        [JsonProperty(PropertyName = "show_text")]
        public bool ShowText { get; set; }

        public string Link { get; set; }

        [JsonProperty(PropertyName = "enable_link")]
        public bool EnableLink { get; set; }

        public string Text { get; set; }

        public string Price { get; set; }
        [JsonProperty(PropertyName = "button")]
        public LpButton Button { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(string))
            {
                return new ServicesModel() { Picture = new PhotoModel() { Src = (string)obj } };
            }

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(OldServicesModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Picture == null && Header == null && Price == null && Text == null && Button == null;
        }

    }

    public class OldServicesModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(ServicesModel))
            {
                return new ServicesModel() { Picture = new PhotoModel() { Src = this.Src, Preview = this.PreviewSrc } };
            }
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Src == null && PreviewSrc == null;
        }
    }

}
