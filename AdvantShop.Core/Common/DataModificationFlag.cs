using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AdvantShop.Core.Common
{
    public static class DataModificationFlag
    {
        private static DateTime? _lastModified;

        private static readonly List<string> Words = new List<string>
        {
            "insert",
            "update",
            "delete"
        };
        
        private static readonly List<string> SpWords = new List<string>
        {
            "add",
            "change",
            "disable",
            "inccount",
            "recalculate",
            "remove",
            "precalc",
            "set",
            "decrement",
            "increment",
            "clear",
            "save"
        };
        
        private static readonly List<string> IgnoreWords = new List<string>
        {
            "recentlyviewsdata",
            "quartzjobruns",
            "orderconfirmation"
        };

        public static DateTime LastModified
        {
            get
            {
                if (!_lastModified.HasValue || (DateTime.Now - _lastModified.Value).TotalSeconds > 60)
                    _lastModified = DateTime.Now;
                
                return _lastModified.Value;
            }
        }

        public static void ResetLastModified()
        {
            _lastModified = DateTime.Now;
        }

        public static void SetLastModifiedSql(string commandText, CommandType commandType)
        {
            if (string.IsNullOrEmpty(commandText) || commandText.StartsWith("select ", StringComparison.OrdinalIgnoreCase))
                return;

            commandText = commandText.ToLower();
            
            if ((Words.Any(x => commandText.Contains(x)) && !IgnoreWords.Any(x => commandText.Contains(x))) ||
                (commandType == CommandType.StoredProcedure && SpWords.Any(x => commandText.Contains(x))))
            {
                _lastModified = DateTime.Now;
            }
        }
    }
}
