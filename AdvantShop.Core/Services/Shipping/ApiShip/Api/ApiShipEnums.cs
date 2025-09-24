namespace AdvantShop.Core.Services.Shipping.ApiShip.Api
{
    public enum ApiShipPickupType
    {
        fromClientDoor = 1,
        customerOrderWarehouseSD = 2,
    }

    public enum ApiShipDeliveryType
    {
        toDoor = 1,
        toPVZ = 2,
    }

    public enum ApiShipTypeOpertionOnPoint
    {
        reception = 1,
        extradition = 2,
        receptionAndDelivery = 2,
    }
}
