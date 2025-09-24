using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class AddReviewModel
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public bool Agreement { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        
        public int? Rating { get; set; }
    }
}