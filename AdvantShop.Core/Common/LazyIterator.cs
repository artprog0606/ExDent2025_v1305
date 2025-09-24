using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Common
{
    /*
     Sample usage:
     
        var lazy = new LazyIterator<int>((page, itemPerPage) => new Service().GetData(page, itemPerPage));
                
        foreach (var item in lazy.GetItems())
            Console.WriteLine(item);
        
        
        public class Service
        {
            public List<int> GetData(int page, int itemsPerPage)
            {
                return Enumerable.Range(0, 10).Skip(page * itemsPerPage).Take(itemsPerPage).ToList();
            }
        }    
     */
    /// <summary>
    /// Ленивый итератор. Достает постранично данные пока может.  
    /// </summary>
    /// <typeparam name="T">Тип возвращаемых данных</typeparam>
    public class LazyIterator<T>
    {
        private Func<int, int, List<T>> getItems;
            
        public LazyIterator(Func<int, int, List<T>> func)
        {
            getItems = func;
        }

        public IEnumerable<T> GetItems(int page = 0, int itemPerPage = 100)
        {
            while (true)
            {
                var items = getItems(page, itemPerPage);

                if (items == null || items.Count == 0)
                    yield break;

                foreach (var item in items)
                    yield return item;
                
                if (items.Count < itemPerPage)
                    yield break;

                page++;
            }
        }
    }
}