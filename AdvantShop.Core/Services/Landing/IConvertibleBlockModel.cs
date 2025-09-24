using System;

namespace AdvantShop.Core.Services.Landing
{
    public interface IConvertibleBlockModel
    {
        IConvertibleBlockModel ConvertToType(Type type);
         IConvertibleBlockModel ConvertFromType(object obj, Type type);
        bool IsNull();
    }
}
