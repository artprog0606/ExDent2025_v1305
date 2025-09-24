namespace AdvantShop.Letters
{
    public sealed class LetterFormatKey
    {
        public string Key { get; }
        public string Description { get; }

        public LetterFormatKey(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}