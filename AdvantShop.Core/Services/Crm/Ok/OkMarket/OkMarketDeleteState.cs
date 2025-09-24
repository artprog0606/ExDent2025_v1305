namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    public static class OkMarketDeleteState
    {
        private static readonly object SyncObject = new object();

        private static bool _isRun = false;

        public static bool IsRun
        {
            get { return _isRun; }
        }

        public static void Start()
        {
            lock (SyncObject)
            {
                _isRun = true;
            }
        }

        public static void Stop()
        {
            lock (SyncObject)
            {
                _isRun = false;
            }
        }
    }
}
