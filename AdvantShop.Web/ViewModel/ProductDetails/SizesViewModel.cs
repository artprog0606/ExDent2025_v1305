using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class SizesViewModel
    {
        public string NgSizes { get; set; }

        public string NgSizeSelected { get; set; }

        public string HeaderText { get; set; }

        public string NgInitSizes { get; set; }

        public string NgChangeSize { get; set; }

        public string SelectedSizes { get; set; }
        
        public SettingsDesign.eSizeColorControlType ControlType { get; set; }
    }
}