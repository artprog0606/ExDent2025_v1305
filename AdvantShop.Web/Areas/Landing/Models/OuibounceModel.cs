using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class OuibounceModel:IConvertibleBlockModel
    {
        public int Delay { get; set; }

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
