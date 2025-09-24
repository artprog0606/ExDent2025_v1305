using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Pictures;

namespace AdvantShop.App.Landing.Models
{
    public class SubBlockPictureModel : SubBlockModel
    {
        public PictureLoaderTriggerModel PictureModel { get; set; }

        public SubBlockPictureModel(LpSubBlock subBlock):base(subBlock)
        {
        }
    }
}
