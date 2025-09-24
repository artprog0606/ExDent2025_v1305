using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Api
{
    public class CarouselApiService
    {
         public static CarouselApi Get(int id)
         {
             return SQLDataAccess.Query<CarouselApi>("Select * From CMS.CarouselApi Where id=@id", new {id})
                 .FirstOrDefault();
         }
         
         public static List<CarouselApi> GetList()
         {
             return SQLDataAccess.Query<CarouselApi>("Select * From CMS.CarouselApi Order by SortOrder").ToList();
         }
         
         public static List<CarouselApi> GetListByExpirationDate()
         {
             return SQLDataAccess.Query<CarouselApi>(
                 "Select * From CMS.CarouselApi " +
                 "Where Enabled = 1 and (ExpirationDate is null or ExpirationDate > getdate()) " +
                 "Order by SortOrder"
                 ).ToList();
         }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From CMS.CarouselApi Where id=@id", CommandType.Text,
                new SqlParameter("@id", id));
            PhotoService.DeletePhotos(id, PhotoType.CarouselApi);
        }

        public static int Add(CarouselApi carousel)
        {
            carousel.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into CMS.CarouselApi ([Title], [ShortDescription], Enabled, SortOrder, FullDescription, ExpirationDate, ShowOnMain, CouponCode, ProductId, CategoryId) " +
                "Values (@Title, @ShortDescription, @Enabled, @SortOrder, @FullDescription, @ExpirationDate, @ShowOnMain, @CouponCode, @ProductId, @CategoryId); " +
                "Select scope_identity();",
                CommandType.Text,
                new SqlParameter("@Title", carousel.Title ?? string.Empty),
                new SqlParameter("@ShortDescription", carousel.ShortDescription ?? (object)DBNull.Value),
                new SqlParameter("@FullDescription", carousel.FullDescription ?? (object)DBNull.Value),
                new SqlParameter("@ExpirationDate", carousel.ExpirationDate ?? (object)DBNull.Value),
                new SqlParameter("@ShowOnMain", carousel.ShowOnMain),
                new SqlParameter("@CouponCode", carousel.CouponCode ?? (object)DBNull.Value),
                new SqlParameter("@ProductId", carousel.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@CategoryId", carousel.CategoryId ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", carousel.Enabled),
                new SqlParameter("@SortOrder", carousel.SortOrder));

            return carousel.Id;
        }

        public static int GetMaxSortOrder()
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("Select max(sortorder) from [CMS].[CarouselApi]", CommandType.Text)) + 10;
        }

        public static void Update(CarouselApi carousel)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CMS.CarouselApi " + 
                "Set [Title]=@Title, [ShortDescription]=@ShortDescription, Enabled=@Enabled, SortOrder=@SortOrder, " +
                "FullDescription=@FullDescription, ExpirationDate=@ExpirationDate, ShowOnMain=@ShowOnMain, " +
                "CouponCode=@CouponCode, ProductId=@ProductId, CategoryId=@CategoryId " +
                "Where Id=@Id", 
                CommandType.Text,
                new SqlParameter("@Id", carousel.Id),
                new SqlParameter("@Title", carousel.Title ?? string.Empty),
                new SqlParameter("@ShortDescription", carousel.ShortDescription ?? (object)DBNull.Value),
                new SqlParameter("@FullDescription", carousel.FullDescription ?? (object)DBNull.Value),
                new SqlParameter("@ExpirationDate", carousel.ExpirationDate ?? (object)DBNull.Value),
                new SqlParameter("@ShowOnMain", carousel.ShowOnMain),
                new SqlParameter("@CouponCode", carousel.CouponCode ?? (object)DBNull.Value),
                new SqlParameter("@ProductId", carousel.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@CategoryId", carousel.CategoryId ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", carousel.Enabled),
                new SqlParameter("@SortOrder", carousel.SortOrder));
        }
    }
}