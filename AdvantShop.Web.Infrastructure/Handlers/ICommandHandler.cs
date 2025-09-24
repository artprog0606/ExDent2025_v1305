namespace AdvantShop.Web.Infrastructure.Handlers
{
    public interface ICommandHandler<in TIn, out TResult>
    {
        TResult Execute(TIn obj);
    }

    public interface ICommandHandler<out TResult>
    {
        TResult Execute();
    }

    public interface ICommandHandler
    {
        void Execute();
    }
}
