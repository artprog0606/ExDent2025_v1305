﻿using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Models;

namespace AdvantShop.ViewModel.Common
{
    public partial class MenuBottomViewModel : BaseModel
    {
        public MenuBottomViewModel()
        {
            Categories = new List<Category>();
            MenuItems = new List<MenuItemModel>();
        }

        public List<Category> Categories { get; set; }

        public List<MenuItemModel> MenuItems { get; set; }

        public bool IsShowSocial { get; set;  }

        public bool IsAccordion { get; set; }
        
        public bool IsBonusActive { get; set; }
        
        public string BonusLinkHref {get; set;}

    }

}