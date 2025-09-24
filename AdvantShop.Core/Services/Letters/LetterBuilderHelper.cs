using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Letters
{
    public class LetterBuilderHelper
    {
        public static List<LetterFormatKey> GetLetterFormatKeys<T>(List<T> items)  where T : Enum
        {
            return items
                    .Select(x => x.LetterFormatKeysAndDescription())
                    .SelectMany(x => x)
                    .ToList();
        }
        
        public static List<LetterFormatKey> GetLetterFormatKeys<T>() where T : Enum
        {
            return 
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => x.LetterFormatKeysAndDescription())
                    .SelectMany(x => x)
                    .ToList();
        }

        public static string GetLetterFormatKeysHtml<T>() where T : Enum
        {
            var keys = GetLetterFormatKeys<T>();
            return GetLetterFormatKeysHtml(keys);
        }
        
        public static string GetLetterFormatKeysHtml(List<LetterFormatKey> keys)
        {
            if (keys == null || keys.Count == 0)
                return "";
            
            var sb = new StringBuilder();

            for (var i = 0; i < keys.Count; i++)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.AppendFormat("<span class=\"letter-format-key\" title=\"{0}\">{1}</span>", 
                    HttpUtility.HtmlAttributeEncode(keys[i].Description), keys[i].Key);
            }

            return sb.ToString();
        }
        
        public static ILetterTemplateBuilder Create<T>(T entity)
        {
            var type = entity.GetType();
            
            if (type == typeof(Customer) && entity is Customer customer)
                return new CustomerLetterBuilder(customer); 

            if (type == typeof(Order) && entity is Order order)
                return new OrderLetterBuilder(order);
            
            if (type == typeof(Lead) && entity is Lead lead)
                return new LeadLetterBuilder(lead);
            
            return null;
        }
    }
}