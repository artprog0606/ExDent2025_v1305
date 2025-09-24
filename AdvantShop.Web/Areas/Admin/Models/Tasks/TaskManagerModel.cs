using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class TaskManagerModel
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string AvatarSrc
        {
            get { return Avatar.IsNotEmpty() ? FoldersHelper.GetPath(FolderType.Avatar, Avatar, false) : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg"; }
        }
    }
}
