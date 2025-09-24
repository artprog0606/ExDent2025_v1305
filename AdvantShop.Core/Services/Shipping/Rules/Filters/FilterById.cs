using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    public class FilterById: BaseFilterBiLogic
    {
        protected readonly IEnumerable<int> Ids;

        /// <param name="ids">Идентификаторы объектов, удовлетворяющих фильтру</param>
        /// <param name="filterIsPositive">true - входит в список, false - не входит в список</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilterById(IEnumerable<int> ids, bool filterIsPositive): base(filterIsPositive)
        {
            ids = ids ?? throw new ArgumentNullException(nameof(ids));
            
            Ids = ids;
        }

        public override bool Check(IObjectForRule obj)
        {
            return FilterIsPositive
                ? Ids.Contains(obj.Id)
                : !Ids.Contains(obj.Id);
        }
    }
}