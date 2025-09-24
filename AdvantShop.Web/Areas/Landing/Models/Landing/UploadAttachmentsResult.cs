namespace AdvantShop.App.Landing.Models.Landing
{
    public class UploadAttachmentsResult
    {
        public bool Result { get; set; }
        public string Error { get; set; }
        public AttachmentModel Attachment { get; set; }
    }
    
    public class AttachmentModel
    {
        public int Id { get; set; }
        public int ObjId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FilePathAdmin { get; set; }
        public string FileSize { get; set; }
    }
}