using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public class BaseResult<T>
    {
        public T Result { get; set; }
    }

    public class BaseResultList<T>
    {
        public List<T> Result { get; set; }
    }

    public class BaseResultItemsList<T> : BaseResult<ResultItems<T>> { }

    public class ResultItems<T>
    {
        public List<T> Items { get; set; }
    }
}
