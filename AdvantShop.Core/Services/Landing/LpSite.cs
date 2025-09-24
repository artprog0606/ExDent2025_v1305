using System;

namespace AdvantShop.Core.Services.Landing
{
    /// <summary>
    /// Сайт Landing Page (объединение страниц)
    /// </summary>
    public class LpSite : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Template { get; set; }
        public string Url { get; set; }
        public string DomainUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ScreenShot { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ScreenShotDate { get; set; }


        public int? ProductId { get; set; }
        

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
