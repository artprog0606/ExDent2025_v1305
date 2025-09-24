using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.Core.Common.Extensions
{
    public static class Async
    {
        public static Task CancelIfRequestedAsync(this CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested ? FromCanceled(cancellationToken) : null;
        }

        public static Task<T> CancelIfRequestedAsync<T>(this CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested ? FromCanceled<T>(cancellationToken) : null;
        }

        public static Task FromCanceled(this CancellationToken cancellationToken)
        {
            return Task.FromCanceled(cancellationToken);
        }

        public static Task<T> FromCanceled<T>(this CancellationToken cancellationToken)
        {
            return Task.FromCanceled<T>(cancellationToken);
        }

    }
}