namespace AdvantShop.Core.Services.Repository
{
    public class AdditionalOption
    {
        public int Id { get; set; }
        public int ObjId { get; set; }
        public EnAdditionalOptionObjectType ObjType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}