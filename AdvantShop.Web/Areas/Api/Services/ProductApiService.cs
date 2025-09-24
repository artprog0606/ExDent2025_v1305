using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;

namespace AdvantShop.Areas.Api.Services
{
    public class ProductApiService
    {
        public Offer GetOfferByBarcode(string barcode)
        {
            var offerId = 
                SQLDataAccess.ExecuteScalar<int>(
                "Select top(1) OfferId From Catalog.Offer Where Barcode = @Barcode Order By Main Desc",
                CommandType.Text,
                new SqlParameter("@Barcode", barcode));
            
            
            return offerId != 0 ? OfferService.GetOffer(offerId) : null;
        }
    }
}