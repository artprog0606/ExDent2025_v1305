using System;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Export
{
    public static class OkExportProgress
    {
        private static int _total;
        private static int _current;

        public static void Start(int total)
        {
            _total = total;
            _current = 0;
        }

        public static void Inc()
        {
            _current += 1;
        }

        public static Tuple<int, int> State()
        {
            return new Tuple<int, int>(_total, _current);
        }
    }
}