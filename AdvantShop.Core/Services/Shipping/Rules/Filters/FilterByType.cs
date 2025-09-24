using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    public class FilterByType: BaseFilterBiLogic
    {
        protected readonly IEnumerable<string> Types;
        protected readonly IEqualityComparer<string> Comparer;

        /// <param name="types">Типы объектов, удовлетворяющих фильтру</param>
        /// <param name="filterIsPositive">true - входит в список, false - не входит в список</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilterByType(IEnumerable<string> types, bool filterIsPositive) : this(types, StringComparer.OrdinalIgnoreCase, filterIsPositive) { }

        /// <param name="types">Типы объектов, удовлетворяющих фильтру</param>
        /// <param name="filterIsPositive">true - входит в список, false - не входит в список</param>
        /// <param name="comparer">Компаратор проверки на равенство.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilterByType(IEnumerable<string> types, IEqualityComparer<string> comparer, bool filterIsPositive): base(filterIsPositive)
        {
            types = types ?? throw new ArgumentNullException(nameof(types));
            comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            
            Types = types;
            Comparer = comparer;
        }

        public override bool Check(IObjectForRule obj)
        {
            return FilterIsPositive
                ? Types.Contains(obj.Type, Comparer)
                : !Types.Contains(obj.Type, Comparer);
        }
    }
}