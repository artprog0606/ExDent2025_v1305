//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using AdvantShop.Core.Services.FullSearch;
using System;

namespace AdvantShop.FullSearch
{
    public class LuceneSearch 
    {
        public static void CreateNewIndex<T>() where T : BaseDocument
        {
            var type = typeof(T);
            
            if (type == typeof(ProductDocument))
                ProductWriter.CreateIndex();
            
            else if (type == typeof(CategoryDocument))
                CategoryWriter.CreateIndex();
            
            else
                throw new Exception(type + " is unknown type");
        }

        public static void CreateAllIndex()
        {
            ProductWriter.CreateIndex();
            CategoryWriter.CreateIndex();
        }

        public static void CreateAllIndexInBackground()
        {
            ProductWriter.CreateIndexInTask();
            CategoryWriter.CreateIndexInTask();
        }
    }
}