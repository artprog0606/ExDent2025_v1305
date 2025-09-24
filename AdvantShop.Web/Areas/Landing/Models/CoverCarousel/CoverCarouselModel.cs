using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Pictures;
using System;

namespace AdvantShop.App.Landing.Models
{
    public class CoverCarouselModel : IConvertibleBlockModel
    {
        public PhotoModel Background { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string TextButton { get; set; }
        public string UrlButton { get; set; }
        public bool ShowButton { get; set; }


        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(string))
            {
                return new CoverCarouselModel() { Background = new PhotoModel() { Src = (string)obj } };
            }

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(OldCoverCarouselModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Background == null && Title == null && Text == null && TextButton == null && UrlButton == null;
        }
    }


    public class OldCoverCarouselModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(CoverCarouselModel))
            {
                return new CoverCarouselModel() { Background = new PhotoModel() { Src = this.Src, Preview = this.PreviewSrc } };
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
