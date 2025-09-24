using System.Collections.Generic;

namespace AdvantShop.Letters
{
    public sealed class LetterBuilder
    {
        private readonly List<ILetterTemplateBuilder> _builders = new List<ILetterTemplateBuilder>();
        
        public LetterBuilder Add(ILetterTemplateBuilder builder)
        {
            _builders.Add(builder);
            return this;
        }

        public string FormatText(string text)
        {
            var formattedText = text;
            
            foreach (var builder in _builders)
                formattedText = builder.FormatText(formattedText);

            return formattedText;
        }
        
        public Dictionary<string, string> GetFormattedParams(string text)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var builder in _builders)
            {
                var formattedParams = builder.GetFormattedParams(text);
                
                foreach (var pair in formattedParams)
                    if (!dictionary.ContainsKey(pair.Key))
                        dictionary.Add(pair.Key, pair.Value);
            }

            return dictionary;
        }

        public List<LetterFormatKey> GetKeyDescriptions()
        {
            var list = new List<LetterFormatKey>();

            foreach (var builder in _builders)
            {
                var items = builder.GetKeyDescriptions();
                if (items != null && items.Count > 0)
                    list.AddRange(items);
            }

            return list;
        }
    }
}