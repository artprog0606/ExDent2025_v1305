using System.Collections.Generic;

namespace AdvantShop.Achievements
{
        public class AchievementsModel
    {
        public bool AllDone { get; set; }
        public bool FirstVisit { get; set; }
        
        public int Bonuses { get; set; }
        public List<AchievementsGroup> Groups { get; set; }
    }
    
    public class AchievementsGroup
    {
        public int Id { get; set; }
        
        public string DisplayName { get; set; }
        
        public int SortOrder { get; set; }
        
        public bool Unlocked { get; set; }
        
        public bool Complete { get; set; }
        
        public List<AchievementsStep> Achievements { get; set; }
        
        public int Bonuses { get; set; }

        public AchievementsGroup(int id, string displayName, int sortOrder, bool unlocked, bool complete, List<AchievementsStep> steps, int bonuses)
        {
            Id = id;
            DisplayName = displayName;
            SortOrder = sortOrder;
            Unlocked = unlocked;
            Complete = complete;
            Achievements = steps;
            Bonuses = bonuses;
        }
    }

    public class AchievementsStep
    {
        public int Id { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Content { get; set; }
        
        public string ContentEnable { get; set; }

        public int Bonuses { get; set; }

        public int SortOrder { get; set; }

        public bool Complete { get; set; }

        public string HelpLinkDescription { get; set; }

        public string HelpLink { get; set; }

        public string ActionButtonName { get; set; }

        public string ActionButtonLink { get; set; }

        public string Svg { get; set; }

        public AchievementsStep(int id, string displayName, string content, string contentEnable,
            int bonuses, int sortOrder, bool complete, string helpLinkDescription,
            string helpLink, string actionButtonName, string actionButtonLink, string svg)
        {
            Id = id;
            DisplayName = displayName;
            Content = content;
            ContentEnable = contentEnable;
            Bonuses = bonuses;
            SortOrder = sortOrder;
            Complete = complete;
            HelpLinkDescription = helpLinkDescription;
            HelpLink = helpLink;
            ActionButtonName = actionButtonName;
            ActionButtonLink = actionButtonLink;
            Svg = svg;
        }
    }
}