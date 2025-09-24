using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.Core.Common
{
    public enum ReturnException
    {
        AggregateException,
        LastException
    }

    public static class RetryHelper
    {
        
        /// <summary>
        /// Repeat action ned times before throw exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval">
        /// retry interval
        /// <remarks>default is a second</remarks></param>
        /// <param name="retryCount">count of repeats</param>
        public static void Do(Action action, TimeSpan? retryInterval = null, int retryCount = 5, 
            ReturnException returnException = ReturnException.AggregateException, 
            Action actionOnFail = null, Func<Exception, bool> funcHandleException = null)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount, returnException, actionOnFail, funcHandleException);
        }

        /// <summary>
        /// Repeat action ned times before throw exception
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval">
        /// retry interval
        /// <remarks>default is a second</remarks></param>
        /// <param name="retryCount">count of repeats</param>
        /// <param name="funcHandleException">
        /// Exceptions to be handled.
        /// <remarks>If null, then all are processed.</remarks>
        /// </param>
        public static T Do<T>(Func<T> action, TimeSpan? retryInterval = null, int retryCount = 5, 
            ReturnException returnException = ReturnException.AggregateException, 
            Action actionOnFail = null, Func<Exception, bool> funcHandleException = null)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval ?? TimeSpan.FromSeconds(1));
                    return action();
                }
                catch (Exception ex)
                {
                    if (funcHandleException != null
                        && !funcHandleException.Invoke(ex))
                        throw;
                    
                    if (actionOnFail != null)
                        try
                        {
                            actionOnFail();
                        }
                        catch (Exception)
                        {
                            // ignore
                        }

                    if (returnException == ReturnException.LastException && retry == (retryCount - 1))
                        throw;
                    
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }

        /// <param name="retryInterval">
        /// retry interval
        /// <remarks>default is a second</remarks></param>
        /// <param name="retryCount">count of repeats</param>
        /// <param name="funcHandleException">
        /// Exceptions to be handled.
        /// <remarks>If null, then all are processed.</remarks>
        /// </param>
        public static async Task DoAsync(Func<Task> task, TimeSpan? retryInterval = null, int retryCount = 3, 
            ReturnException returnException = ReturnException.AggregateException, 
            Action actionOnFail = null, Func<Exception, bool> funcHandleException = null)
        {
            var exceptions = new List<Exception>();
            for (int attempted = 0; attempted < retryCount; attempted++)
            {
                try
                {
                    if (attempted > 0) 
                        await Task.Delay(retryInterval ?? TimeSpan.FromSeconds(1));

                    await task();
                    return;
                }
                catch (Exception ex)
                {
                    if (funcHandleException != null
                        && !funcHandleException.Invoke(ex))
                        throw;
                    
                    if (actionOnFail != null)
                        try
                        {
                            actionOnFail();
                        }
                        catch (Exception)
                        {
                            // ignore
                        }

                    if (returnException == ReturnException.LastException && attempted == (retryCount - 1))
                        throw;
                    
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
        
        /// <param name="retryInterval">
        /// retry interval
        /// <remarks>default is a second</remarks></param>
        /// <param name="retryCount">count of repeats</param>
        /// <param name="funcHandleException">
        /// Exceptions to be handled.
        /// <remarks>If null, then all are processed.</remarks>
        /// </param>
        public static async Task<T> DoAsync<T>(Func<Task<T>> task, TimeSpan? retryInterval = null, int retryCount = 3, 
            ReturnException returnException = ReturnException.AggregateException, 
            Action actionOnFail = null, Func<Exception, bool> funcHandleException = null)
        {
            var exceptions = new List<Exception>();
            for (int attempted = 0; attempted < retryCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                        await Task.Delay(retryInterval ?? TimeSpan.FromSeconds(1));
                    
                    return await task();
                }
                catch (Exception ex)
                {
                    if (funcHandleException != null
                        && !funcHandleException.Invoke(ex))
                        throw;
                    
                    if (actionOnFail != null)
                        try
                        {
                            actionOnFail();
                        }
                        catch (Exception)
                        {
                            // ignore
                        }

                    if (returnException == ReturnException.LastException && attempted == (retryCount - 1))
                    {
                        throw;
                    }
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
