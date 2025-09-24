
using System;

namespace AdvantShop.Core.Services.Landing.Blocks
{
    public class LpBlock : ICloneable
    {
        public int Id { get; set; }
        public int LandingId { get; set; }
        public string Name { get; set; }
        public string ContentHtml { get; set; }
        public string Type { get; set; }
        public string Settings { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool ShowOnAllPagesTop { get; set; }
        public bool ShowOnAllPagesBottom { get; set; }
        public bool NoCache { get; set; }

        public dynamic MappedSettings;
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class LpSubBlock
    {
        public int Id { get; set; }
        public int LandingBlockId { get; set; }
        public string Name { get; set; }
        public string ContentHtml { get; set; }
        public string Type { get; set; }
        public string Settings { get; set; }
        public int SortOrder { get; set; }

        public dynamic MappedSettings;
    }
}