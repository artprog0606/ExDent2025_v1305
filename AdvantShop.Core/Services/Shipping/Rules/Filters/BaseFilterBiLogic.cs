namespace AdvantShop.Core.Services.Shipping.Rules
{
    public abstract class BaseFilterBiLogic: IFilter
    {
        protected readonly bool FilterIsPositive;

        protected BaseFilterBiLogic(bool filterIsPositive)
        {
            FilterIsPositive = filterIsPositive;
        }

        public abstract bool Check(IObjectForRule obj);


        public virtual TypesOfLogicalGrouping GetLogicalGrouping() =>
            FilterIsPositive
                ? TypesOfLogicalGrouping.Or
                : TypesOfLogicalGrouping.And;
    }
}