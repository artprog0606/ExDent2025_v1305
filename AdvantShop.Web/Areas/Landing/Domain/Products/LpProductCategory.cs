using System;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Domain.Products
{
    public class LpProductCategory : IConvertibleBlockModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

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
