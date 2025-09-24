using AdvantShop.Core.Services.Landing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Landing.Pictures;


namespace AdvantShop.App.Landing.Models
{
    public class ScheduleModel : IConvertibleBlockModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public PhotoModel Picture { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(string))
            {
                return new ScheduleModel() { Picture = new PhotoModel() { Src = (string)obj } };
            }

            throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == this.GetType())
                return this;

            else if (type == typeof(OldScheduleModel))
                throw new NotImplementedException("OldGalleryItemModel is obsolete. You can not convert any class to OldGalleryItemModel");
            else
            {
                throw new NotImplementedException(string.Format("Can not convert from {0} to {1}", this.GetType().ToString(), type.ToString()));
            }
        }

        public bool IsNull()
        {
            return Picture == null && Name == null && Position == null && Time == null && Text == null;
        }

    }

    public class OldScheduleModel : IConvertibleBlockModel
    {
        public string Src { get; set; }
        public string PreviewSrc { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            if (type == typeof(ScheduleModel))
            {
                return new ScheduleModel() { Picture = new PhotoModel() { Src = this.Src, Preview = this.PreviewSrc } };
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
