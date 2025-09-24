using AdvantShop.Web.Admin.Models.Attachments;

namespace AdvantShop.Web.Admin.Models.Tasks
{
    public class UploadAttachmentsResult
    {
        public bool Result { get; set; }
        public string Error { get; set; }
        public AttachmentModel Attachment { get; set; }
    }
}
