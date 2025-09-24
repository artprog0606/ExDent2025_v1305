using System;
using System.Threading;

namespace AdvantShop.Core.Common
{
    public class LazyWithoutExceptionsCashing<T>
    {
        private readonly Func<T> _valueFactory;
        private readonly LazyThreadSafetyMode _lazyThreadSafetyMode;
        private Lazy<T> _lazy;

        private readonly object _lockObj = new object();
        private bool _isValueFaulted;

        public LazyWithoutExceptionsCashing(Func<T> valueFactory, LazyThreadSafetyMode lazyThreadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication)
        {
            _lazyThreadSafetyMode = lazyThreadSafetyMode;
            _valueFactory = valueFactory;
            _lazy = new Lazy<T>(valueFactory, lazyThreadSafetyMode);
        }

        public T Value
        {
            get
            {
                try
                {
                    return _lazy.Value;
                }
                catch (Exception)
                {
                    _isValueFaulted = true;
                    lock (_lockObj)
                    {
                        if (_isValueFaulted)
                        {
                            _lazy = new Lazy<T>(_valueFactory, _lazyThreadSafetyMode);
                            _isValueFaulted = false;
                        }
                    }
                    throw;
                }
            }
        }

        public bool IsValueCreated => _lazy.IsValueCreated;
    }
}
