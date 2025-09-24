using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Blocks
{
    public class LpBlockConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Picture { get; set; }


        [JsonProperty("subblocks")]
        public List<LpSubBlockConfig> SubBlocks { get; set; }
        
        public dynamic Settings { get; set; }

        public string BlockPath { get; set; }
        
        
        [JsonProperty("depends_on")]
        public List<LpBlockConfigDependency> DependsOn { get; set; }
    }

    [Serializable]
    public class LpSubBlockConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Placeholder { get; set; }
        public dynamic Settings { get; set; }
    }
}
