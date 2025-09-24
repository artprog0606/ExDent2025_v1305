//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class ServiceService
    {
        public static Service Get(int id)
        {
            return
                SQLDataAccess.ExecuteReadOne<Service>("SELECT * FROM [Booking].[Service] WHERE Id = @Id",
                    CommandType.Text, GetServiceFromReader,
                    new SqlParameter("@Id", id));
        }

        public static Service Get(string artNo)
        {
            return
                SQLDataAccess.ExecuteReadOne<Service>("SELECT * FROM [Booking].[Service] WHERE ArtNo = @ArtNo",
                    CommandType.Text, GetServiceFromReader,
                    new SqlParameter("@ArtNo", artNo));
        }

        public static List<Service> GetList()
        {
            return SQLDataAccess.ExecuteReadList<Service>(
                "SELECT * FROM [Booking].[Service] Order by SortOrder",
                CommandType.Text, GetServiceFromReader);
        }

        public static List<Service> GetList(int categoryId)
        {
            return
                SQLDataAccess.ExecuteReadList<Service>(
                    "SELECT * FROM [Booking].[Service] WHERE CategoryId = @CategoryId Order by SortOrder",
                    CommandType.Text,
                    GetServiceFromReader,
                    new SqlParameter("@CategoryId", categoryId));
        }

        public static bool ExistsArtNo(string artNo, int ignoreServiceId = 0)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                string.Format(
                    "SELECT (CASE WHEN EXISTS(Select 1 FROM [Booking].[Service] WHERE ArtNo=@ArtNo{0}) THEN 1 ELSE 0 END)",
                    ignoreServiceId > 0 ? " AND Id!=@Id" : ""),
                CommandType.Text,
                new SqlParameter("@ArtNo", artNo),
                new SqlParameter("@Id", ignoreServiceId));
        }

        private static Service GetServiceFromReader(SqlDataReader reader)
        {
            return new Service
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                CurrencyId = SQLDataHelper.GetInt(reader, "CurrencyId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                BasePrice = SQLDataHelper.GetFloat(reader, "Price"),
                Image = SQLDataHelper.GetString(reader, "Image"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Duration = SQLDataHelper.IsDbNull(reader, "Duration") ? null : (TimeSpan?)TimeSpan.FromTicks(SQLDataHelper.GetLong(reader, "Duration")),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled")
            };
        }

        public static List<int> GetListId()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[Service]", CommandType.Text, "Id");
        }

        public static List<int> GetListId(int categoryId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[Service] WHERE CategoryId = @CategoryId", CommandType.Text, "Id", new SqlParameter("@CategoryId", categoryId));
        }

        public static int Add(Service service)
        {
            var guidArtNo = Guid.NewGuid().ToString();

            service.Id = SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[Service] " +
                                                    " ([ArtNo], [CategoryId], [CurrencyId], [Name], [Image], [Price], [Description], [SortOrder], [Enabled], [Duration]) " +
                                                    " VALUES (@ArtNo, @CategoryId, @CurrencyId, @Name, @Image, @Price, @Description, @SortOrder, @Enabled, @Duration); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@ArtNo", !string.IsNullOrEmpty(service.ArtNo) ? service.ArtNo : guidArtNo),
                new SqlParameter("@CategoryId", service.CategoryId),
                new SqlParameter("@CurrencyId", service.CurrencyId),
                new SqlParameter("@Name", service.Name ?? (object) DBNull.Value),
                new SqlParameter("@Image", service.Image ?? (object)DBNull.Value),
                new SqlParameter("@Price", service.BasePrice),
                new SqlParameter("@Description", service.Description ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", service.SortOrder),
                new SqlParameter("@Duration", service.Duration.HasValue ? service.Duration.Value.Ticks : (object)DBNull.Value),
                new SqlParameter("@Enabled", service.Enabled)
                );

            if (string.IsNullOrEmpty(service.ArtNo))
            {
                if (!ExistsArtNo(service.Id.ToString()))
                {
                    SQLDataAccess.ExecuteNonQuery(
                        " UPDATE [Booking].[Service] SET [ArtNo] = @ArtNo " +
                        " WHERE Id = @Id", CommandType.Text,
                        new SqlParameter("@Id", service.Id),
                        new SqlParameter("@ArtNo", service.Id.ToString())
                        );

                    service.ArtNo = service.Id.ToString();
                }
            }
            else
            {
                service.ArtNo = guidArtNo;
            }

            return service.Id;
        }

        public static void Update(Service service)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Service] SET [ArtNo] = @ArtNo, [CategoryId] = @CategoryId, [CurrencyId] = @CurrencyId, [Name] = @Name, [Image] = @Image, [Price] = @Price, [Description] = @Description, [SortOrder] = @SortOrder, [Enabled] = @Enabled, [Duration] = @Duration " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", service.Id),
                new SqlParameter("@ArtNo", !string.IsNullOrEmpty(service.ArtNo) ? service.ArtNo : (object) DBNull.Value),
                new SqlParameter("@CategoryId", service.CategoryId),
                new SqlParameter("@CurrencyId", service.CurrencyId),
                new SqlParameter("@Name", service.Name ?? (object) DBNull.Value),
                new SqlParameter("@Image", service.Image ?? (object)DBNull.Value),
                new SqlParameter("@Price", service.BasePrice),
                new SqlParameter("@Description", service.Description ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", service.SortOrder),
                new SqlParameter("@Duration", service.Duration.HasValue ? service.Duration.Value.Ticks : (object)DBNull.Value),
                new SqlParameter("@Enabled", service.Enabled)
                );
        }

        public static void Delete(int id)
        {
            BeforeDelete(id);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[Service] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void DeleteByCategory(int categoryId)
        {
            GetListId(categoryId).ForEach(BeforeDelete);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[Service] WHERE CategoryId = @CategoryId", CommandType.Text,
                new SqlParameter("@CategoryId", categoryId));
        }

        private static void BeforeDelete(int id)
        {
            var service = Get(id);

            if (service != null)
            {
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingService, service.Image));
            }
        }

        #region ReservationResourceService

        public static List<Service> GetListReservationResourceServices(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadList<Service>(
                    @"SELECT s.* FROM [Booking].[Service] as s
                        INNER JOIN [Booking].[ReservationResourceService] as rrs ON rrs.[ServiceId] = s.[Id]
                    WHERE rrs.[AffiliateId] = @AffiliateId AND rrs.[ReservationResourceId] = @ReservationResourceId
                    Order by s.SortOrder",
                    CommandType.Text,
                    GetServiceFromReader,
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static List<int> GetListIdsByReservationResourceServices(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    @"SELECT [ServiceId] FROM [Booking].[ReservationResourceService]
                    WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId",
                    CommandType.Text,
                    "ServiceId",
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static List<int> GetListCategoryIdsByReservationResourceServices(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    @"SELECT distinct s.[CategoryId] FROM [Booking].[Service] as s
                        INNER JOIN [Booking].[ReservationResourceService] as rrs ON rrs.[ServiceId] = s.[Id]
                    WHERE rrs.[AffiliateId] = @AffiliateId AND rrs.[ReservationResourceId] = @ReservationResourceId",
                    CommandType.Text,
                    "CategoryId",
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static void AddReservationResourceService(int affiliateId, int reservationResourceId, int serviceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[ReservationResourceService] WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND [ServiceId] = @ServiceId)
                begin 
                    INSERT INTO [Booking].[ReservationResourceService] ([AffiliateId],[ReservationResourceId],[ServiceId]) VALUES (@AffiliateId, @ReservationResourceId, @ServiceId)
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@ServiceId", serviceId)
                );
        }

        public static void DeleteReservationResourceService(int affiliateId, int reservationResourceId, int serviceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceService] " +
                " WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND [ServiceId] = @ServiceId ",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@ServiceId", serviceId)
                );
        }

        public static void DeleteReservationResourceServices(int affiliateId, int reservationResourceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceService] " +
                " WHERE [AffiliateId] = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId ",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId)
                );
        }

        public static void DeleteReservationResourceServicesByAffiliateAndService(int affiliateId, int serviceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceService] " +
                " WHERE [AffiliateId] = @AffiliateId AND [ServiceId] = @ServiceId ",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ServiceId", serviceId)
                );
        }

        public static void DeleteReservationResourceServicesByAffiliate(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceService] " +
                " WHERE [AffiliateId] = @AffiliateId ",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId)
                );
        }

        #endregion


        #region AffiliateService

        public static void AddRefAffiliate(int serviceId, int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateService] WHERE [AffiliateId] = @AffiliateId AND [ServiceId] = @ServiceId)
                begin 
                    INSERT INTO [Booking].[AffiliateService] ([AffiliateId],[ServiceId]) VALUES (@AffiliateId, @ServiceId)
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ServiceId", serviceId)
                );
        }

        public static void DeleteRefByAffiliate(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateService] WHERE AffiliateId = @AffiliateId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId));

            DeleteReservationResourceServicesByAffiliate(affiliateId);
        }

        public static void DeleteRefAffiliate(int serviceId, int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateService] WHERE AffiliateId = @AffiliateId AND ServiceId = @ServiceId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ServiceId", serviceId));

            DeleteReservationResourceServicesByAffiliateAndService(affiliateId, serviceId);
        }

        #endregion

    }
}