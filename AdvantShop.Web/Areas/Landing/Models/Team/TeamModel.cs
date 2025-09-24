using AdvantShop.Core.Services.Landing;
using System;
using AdvantShop.Core.Services.Landing.Pictures;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Landing.Forms;

namespace AdvantShop.App.Landing.Models
{
    public class TeamModel : IConvertibleBlockModel, ILpPagingModel
    {
        public PhotoModel Picture { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

        [JsonProperty(PropertyName = "button")]
        public LpButton Button { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(string))
            {
                return new TeamModel() { Picture = new PhotoModel() { Src = (string)obj } };
            }

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(OldTeamModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Picture == null && Name == null && Position == null;
        }
    }

    public class OldTeamModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(TeamModel))
            {
                return new ServicesModel() { Picture = new PhotoModel() { Src = this.Src, Preview = this.PreviewSrc } };
            }
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Src == null && PreviewSrc == null;
        }
    }
}
