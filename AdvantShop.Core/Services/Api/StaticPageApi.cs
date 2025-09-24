using System;

namespace AdvantShop.Core.Services.Api
{
    public class StaticPageApi
    {
        public int Id { get; set; }
        
        public int? ParentId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
        
        /// <summary>
        /// svg icon text content
        /// </summary>
        public string Icon { get; set; }
        
        /// <summary>
        /// svg icon name
        /// </summary>
        public string IconName { get; set; }
        
        public bool Enabled { get; set; }

        public DateTime AddDate { get; set; }

        public DateTime ModifyDate { get; set; }
        
        public int SortOrder { get; set; }

        public bool ShowInProfile { get; set; }
    }
}