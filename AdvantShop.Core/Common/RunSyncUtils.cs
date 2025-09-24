using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.Core.Common
{
    public class RunSyncUtils
    {
        private static readonly TaskFactory factory = new TaskFactory(default(CancellationToken), TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static void RunSync(Func<Task> task)
        {
            factory.StartNew(task).Unwrap().GetAwaiter().GetResult();
        }
        public static T RunSync<T>(Func<Task<T>> task)
        {
            return factory.StartNew(task).Unwrap().GetAwaiter().GetResult();
        }
    }
}
