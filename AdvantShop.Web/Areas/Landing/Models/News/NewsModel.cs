using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Pictures;
using System;

namespace AdvantShop.App.Landing.Models
{
    public class NewsModel : IConvertibleBlockModel, ILpPagingModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Annotation { get; set; }
        public string Url { get; set; }
        public PhotoModel Picture { get; set; }
        public string Date { get; set; }
        
        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            if (type == typeof(string))
                return new NewsModel() { Picture = new PhotoModel() { Src = (string)obj } };

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType(), type));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            if (type == typeof(OldNewsModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType(), type));
        }

        public bool IsNull()
        {
            return Picture == null && Title == null && Text == null && Annotation == null && Url == null;
        }
    }

    public class OldNewsModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(NewsModel))
                return new NewsModel() {Picture = new PhotoModel() {Src = Src, Preview = PreviewSrc}};
            
            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", GetType(), type));
        }

        public bool IsNull()
        {
            return Src == null && PreviewSrc == null;
        }
    }
}
