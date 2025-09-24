namespace AdvantShop.Core.Primitives
{
    public sealed class WrapResult<T>
    {
        public T Value { get; }

        public WrapResult(T value)
        {
            Value = value;
        }
    }
}