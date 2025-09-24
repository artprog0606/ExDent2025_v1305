using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    public class Rule: IRule
    {
        /// <summary>
        /// Фильтры для срабатывания правила
        /// </summary>
        protected IEnumerable<IFilter> Filters { get; set; } = Enumerable.Empty<IFilter>();
        
        /// <summary>
        /// Редакторы объекта
        /// </summary>
        protected IEnumerable<IEditor> Editors { get; set; } = Enumerable.Empty<IEditor>();

        protected bool IsSuitableObject(IObjectForRule obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            Filters = Filters ?? throw new ArgumentNullException(nameof(Filters));

            return Filters
                  .GroupBy(f => 
                       (Type: f.GetType(), LogicalGrouping: f.GetLogicalGrouping()))
                  .OrderBy(g => 
                       AttributeHelper.GetAttributeValue<DifficultyOfFilterAttribute, int>(g.Key.Type))
                  .All(g =>
                       g.Key.LogicalGrouping == TypesOfLogicalGrouping.Or
                           ? g.Any(f => f.Check(obj))
                           : g.All(f => f.Check(obj)));
        }

        public void Apply(IObjectForRule obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            Editors = Editors ?? throw new ArgumentNullException(nameof(Editors));

            if (!IsSuitableObject(obj))
                return;

            foreach (var editor in Editors) 
                editor.Change(obj);
        }
    }
}