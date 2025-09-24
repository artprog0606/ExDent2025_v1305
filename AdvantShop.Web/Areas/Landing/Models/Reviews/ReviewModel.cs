using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Pictures;
using System;

namespace AdvantShop.App.Landing.Models
{
    public class ReviewModel : IConvertibleBlockModel
    {
        public string Date { get; set; }
        public PhotoModel Picture { get; set; }
        public string Author { get; set; }
        public string Caption { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int Rating { get; set; }


        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(string))
            {
                return new ReviewModel() { Picture = new PhotoModel() { Src = (string)obj } };
            }

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(OldReviewModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Picture == null && Date == null && Author == null && Caption == null && Text == null && Link == null && Rating == 0;
        }

    }

    public class OldReviewModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(ReviewModel))
            {
                return new ReviewModel() { Picture = new PhotoModel() { Src = this.Src, Preview = this.PreviewSrc } };
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
