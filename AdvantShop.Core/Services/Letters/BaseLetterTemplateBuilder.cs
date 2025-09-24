using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;

namespace AdvantShop.Letters
{
    public abstract class BaseLetterTemplateBuilder<TEntity, TEnumKey> : ILetterTemplateBuilder 
                                                                        where TEntity : class
                                                                        where TEnumKey : Enum 
    {
        protected readonly TEntity _entity;
        protected readonly TEnumKey[] _availableKeys;

        private static Regex _regexCompiled;

        /// <summary>
        /// Array enum keys
        /// {OrderLetterTemplateKey.OrderId, OrderLetterTemplateKey.Number}..} 
        /// </summary>
        private static TEnumKey[] _enumKeys;
        
        /// <summary>
        /// Dictionary format key and enum key
        /// {{"#ORDER_ID#", OrderLetterTemplateKey.OrderId}, {"#NUMBER#", OrderLetterTemplateKey.Number}..} 
        /// </summary>
        private static Dictionary<string, TEnumKey> _formatsKeys;
        
        /// <summary>
        /// Common dictionary format key and enum key
        /// {"#LOGO#", CommonLetterTemplateKey.Logo}, {"#MAIN_PHONE#", OrderLetterTemplateKey.StorePhone}..} 
        /// </summary>
        private static Dictionary<string, CommonLetterTemplateKey> _commonFormatKeys;


        protected BaseLetterTemplateBuilder(TEntity entity, TEnumKey[] availableKeys)
        {
            _entity = entity;
            _availableKeys = availableKeys != null && availableKeys.Length > 0 ? availableKeys : GetEnums();

            if (_formatsKeys == null)
                _formatsKeys = GetFormatKeys();
                    
            if (_commonFormatKeys == null)
                _commonFormatKeys = GetCommonFormatKeys();

            if (_regexCompiled == null)
                _regexCompiled = new Regex("#([A-Za-z0-9_]*)#", RegexOptions.Compiled);
        }

        protected virtual string GetValue(TEnumKey key)
        {
            return "";
        }

        /// <summary>
        /// Format text
        /// </summary>
        public string FormatText(string text)
        {
            return _regexCompiled.Replace(text, Replace);
        }

        /// <summary>
        /// Get key params and values used in letter
        /// {"#ORDER_ID#", "1"}, {"#NUMBER#", "100-1"}..
        /// </summary>
        public Dictionary<string, string> GetFormattedParams(string text)
        {
            var letterParams = new Dictionary<string, string>();
            
            var matches = _regexCompiled.Matches(text);
            
            foreach (Match match in matches)
            {
                var collectionKey = match.Value;
                
                if (_formatsKeys.TryGetValue(collectionKey, out var key))
                {
                    if (_availableKeys.Contains(key))
                    {
                        letterParams.Add(collectionKey, GetValue(key));
                        continue;
                    }
                }

                if (_commonFormatKeys.TryGetValue(collectionKey, out var commonKey))
                    letterParams.Add(collectionKey, GetValueByCommonKey(commonKey));
            }
            
            return letterParams;
        }
        
        public virtual List<LetterFormatKey> GetKeyDescriptions()
        {
            return LetterBuilderHelper.GetLetterFormatKeys<TEnumKey>();
        }

        private string Replace(Match match)
        {
            var collectionKey = match.Value;

            if (_formatsKeys.TryGetValue(collectionKey, out var key))
            {
                if (_availableKeys.Contains(key))
                    return GetValue(key);
            }
            
            if (_commonFormatKeys.TryGetValue(collectionKey, out var commonKey))
                return GetValueByCommonKey(commonKey);

            return collectionKey;
        }
        
        private Dictionary<string, TEnumKey> GetFormatKeys()
        {
            var dictFormatKeyEnumKey = new Dictionary<string, TEnumKey>();
            
            foreach (TEnumKey templateKey in GetEnums())
            {
                var keys = templateKey.LetterFormatKeys();

                foreach (var key in keys)
                    dictFormatKeyEnumKey.Add(key, templateKey);
            }
            
            return dictFormatKeyEnumKey;
        }
        
        private TEnumKey[] GetEnums()
        {
            return _enumKeys ?? (_enumKeys = (TEnumKey[]) Enum.GetValues(typeof(TEnumKey)));
        }

        private Dictionary<string, CommonLetterTemplateKey> GetCommonFormatKeys()
        {
            var dictFormatKeyEnumKey = new Dictionary<string, CommonLetterTemplateKey>();
            
            foreach (CommonLetterTemplateKey templateKey in Enum.GetValues(typeof(CommonLetterTemplateKey)))
            {
                var keys = templateKey.LetterFormatKeys();
                foreach (var key in keys)
                    dictFormatKeyEnumKey.Add(key, templateKey);
            }

            return dictFormatKeyEnumKey;
        }
        
        private string GetValueByCommonKey(CommonLetterTemplateKey key)
        {
            switch (key)
            {
                case CommonLetterTemplateKey.Logo:
                    return !string.IsNullOrEmpty(SettingsMain.LogoImageName)
                        ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                            SettingsMain.SiteUrl + '/' +
                            FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.LogoImageName, false),
                            SettingsMain.ShopName)
                        : "";
                
                case CommonLetterTemplateKey.StorePhone: return SettingsMain.Phone;
                case CommonLetterTemplateKey.StoreName: return SettingsMain.ShopName;
                case CommonLetterTemplateKey.StoreUrl: return SettingsMain.SiteUrl;
                default: return null;
            }
        }
    }
}