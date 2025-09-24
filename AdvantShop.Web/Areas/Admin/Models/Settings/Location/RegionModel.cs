using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class RegionModel
    {
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string RegionCode { get; set; }
        public int SortOrder { get; set; }
        public string CountryIso2 { get; set; }
    }
}
