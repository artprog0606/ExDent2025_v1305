//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;

namespace AdvantShop.DownloadableContent
{
    [Serializable]
    public class DownloadableContentBox
    {
        public DownloadableContentBox()
        {
            Items = new List<DownloadableContentObject>();
            Message = string.Empty;
        }

        public List<DownloadableContentObject> Items { get; set; }
        public string Message { get; set; }
    }
}