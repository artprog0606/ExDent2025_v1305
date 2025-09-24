namespace AdvantShop.Core.Common.Attributes
{
    public interface IAttribute<T>
    {
        T Value { get; }
    }
    
    public interface IFormatKeyAttribute<out TReturn>
    {
        TReturn Value { get; }
        string Description { get; }
    }

    public interface ICompareAttribute<V, T>
    {
        V Value { get; }
        T Type { get; }

        bool NotLogValue { get; }
    }
}