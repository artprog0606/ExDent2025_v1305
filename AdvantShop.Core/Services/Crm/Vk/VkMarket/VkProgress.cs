using System;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{

    public static class VkProgress
    {
        private static int _total;
        private static int _current;
        private static string _errorMessage;
        private static bool _processing;

        private static readonly object Sync = new object();

        public static void Start()
        {
            lock (Sync)
                _processing = true;
        }
        
        public static void Set(int total)
        {
            _total = total;
            _errorMessage = null;
            _current = 0;
        }
        
        public static void Stop()
        { 
            lock (Sync)
                _processing = false;
        }

        public static bool IsProcessing()
        {
            lock (Sync)
                return _processing;
        }

        public static void Inc()
        {
            _current += 1;
        }

        public static void Error(string error)
        {
            _errorMessage = error;
        }

        public static Tuple<int, int, string> State()
        {
            return new Tuple<int, int, string>(_total, _current, _errorMessage);
        }
    }
}
