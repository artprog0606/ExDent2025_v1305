using AdvantShop.Core.Services.Landing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class ServicesGroupModel : IConvertibleBlockModel
    {
        public string Picture { get; set; }
        public string Header { get; set; }

        [JsonProperty(PropertyName = "show_button")]
        public bool ShowButton { get; set; }

        [JsonProperty(PropertyName = "content_items")]
        public List<ServicesContentItemsModel> ContentItems { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            throw new NotImplementedException();
        }
    }

    public class ServicesContentItemsModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Obsolete]
        public string Text { get; set; }

        public string Price { get; set; }

        [JsonProperty(PropertyName = "show_price")]
        public bool ShowPrice { get; set; }
    }
}
