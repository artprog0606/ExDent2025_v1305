using System.Collections.Generic;

namespace AdvantShop.Letters
{
    public interface ILetterTemplateBuilder
    {
        /// <summary>
        /// Format text
        /// </summary>
        string FormatText(string text);
        
        /// <summary>
        /// Get dictionary of key, value used in text
        /// {"#ORDER_ID#", "1"}, {"#NUMBER#", "100-1"}..
        /// </summary>
        Dictionary<string, string> GetFormattedParams(string text);
        
        /// <summary>
        /// Get list of key and key description
        /// </summary>
        /// <returns></returns>
        List<LetterFormatKey> GetKeyDescriptions();
    }
}