namespace AdvantShop.Core.Services.Crm.DealStatuses
{
    public enum SalesFunnelStatusType
    {
        None = 0,
        FinalSuccess = 1,
        Canceled = 2
    }

    public class DealStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Color { get; set; }
        public SalesFunnelStatusType Status { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as DealStatus;
            if (other == null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   SortOrder == other.SortOrder &&
                   Status == other.Status;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ (Name ?? "").GetHashCode() ^ SortOrder.GetHashCode() ^ Status.GetHashCode();
        }
    }

    public class DealStatusWithCount : DealStatus
    {
        public int LeadsCount { get; set; }
    }
}
