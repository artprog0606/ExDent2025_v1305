using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Pictures;
using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Domain.SubBlocks
{
    public class LpMenuItem : IConvertibleBlockModel
    {
        public string Text { get; set; }
        public string Href { get; set; }
        public bool Target { get; set; }
        public string Class { get; set; }

        public PhotoModel Picture { get; set; }
        public List<LpMenuItem> Childs { get; set; }

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
}
