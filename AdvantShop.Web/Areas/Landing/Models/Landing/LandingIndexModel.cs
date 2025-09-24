using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Landing
{
    public class LandingIndexModel
    {
        public string Url{ get; set; }
        public string LpUrl { get; set; }
        public int? LandingId{ get; set; }
        public bool? Inplace{ get; set; }

        /// <summary>
        /// Order code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Lead id
        /// </summary>
        public string Lid { get; set; }
        
        /// <summary>
        /// Booking id
        /// </summary>
        public string Bid { get; set; }

        /// <summary>
        /// Ignored downsell landing id
        /// </summary>
        public int? Without { get; set; }

        /// <summary>
        /// "?mode=lp"
        /// </summary>
        public string Mode { get; set; }
        public List<string> Errors { get; set; }
    }
}
