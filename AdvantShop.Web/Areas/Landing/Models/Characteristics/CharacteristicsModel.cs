using AdvantShop.Core.Services.Landing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class Characteristics : IConvertibleBlockModel
    {
        public string Header { get; set; }

        [JsonProperty(PropertyName = "content_items")]
        public List<CharacteristicsContentItemsModel> ContentItems { get; set; }

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

    public class CharacteristicsContentItemsModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}