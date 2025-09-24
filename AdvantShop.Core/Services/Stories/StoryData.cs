using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Stories
{
    public class StoryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime DateStartShow { get; set; }
        public DateTime? DateDisappeared { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }

        public List<SlideData> Slides { get; set; }
        
    }

    public class SlideData
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string Action { get; set; }
        public string ScreenAddress { get; set; }
        public string ScreenParameter { get; set; }
        public string SlideText { get; set; }
        public string ButtonText { get; set; }
        public bool UseStoreColorScheme { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string SlideTextColor { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
