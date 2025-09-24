namespace AdvantShop.Core.Services.Shipping.PickPoint.Postamats
{
    public class Postamat
    {
        public Postamat()
        {
            OwnerName = string.Empty;
            TypeTitle = string.Empty;
            Name = string.Empty;
            City = string.Empty;
            Region = string.Empty;
            Country = string.Empty;
            CountryIso = string.Empty;
            Address = string.Empty;
            WorkTimeSMS = string.Empty;
            AddressDescription = string.Empty;
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public string OwnerName { get; set; }
        public string TypeTitle { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string CountryIso { get; set; }
        public string Address { get; set; }
        public string AddressDescription { get; set; }
        public float? AmountTo { get; set; }
        public string WorkTimeSMS { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public byte Cash { get; set; }
        public byte Card { get; set; }
        public bool PayPassAvailable { get; set; }
        public bool SelfDelivery { get; set; }
        public float? MaxWeight { get; set; }
        public float? DimensionSum { get; set; }
        public float? MaxHeight { get; set; }
        public float? MaxWidth { get; set; }
        public float? MaxLength { get; set; }
    }
}
