//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public class ReviewService
    {
        public const string ReviewCacheKey = "Review_";

        public static Review GetReview(int reviewId)
        {
            return SQLDataAccess.ExecuteReadOne(
                    "SELECT TOP 1 ParentReview.*, Photo.PhotoName, (SELECT Count(*) FROM [CMS].[Review] WHERE [ParentId] = ParentReview.ReviewId) as ChildrenCount, Ratio.ProductRatio as Rating " +
                    "FROM [CMS].[Review] as ParentReview " +
                    "LEFT JOIN Catalog.Photo ON ParentReview.ReviewId = Photo.ObjId AND Main = 1 AND Photo.Type = @PhotoType " +
                    "LEFT JOIN Catalog.Ratio ON ParentReview.Type = @EntityTypeProduct AND Ratio.ProductID = ParentReview.EntityId AND Ratio.CustomerId = ParentReview.CustomerId " +
                    "WHERE ReviewId = @ReviewId",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@ReviewId", reviewId),
                    new SqlParameter("@PhotoType", PhotoType.Review.ToString()),
                    new SqlParameter("@EntityTypeProduct", EntityType.Product));
        }

        public static IEnumerable<Review> GetReviews(int entityId, EntityType entityType)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Review>(
                    "SELECT ParentReview.*, Photo.PhotoName, (SELECT Count(*) FROM [CMS].[Review] WHERE [ParentId] = ParentReview.ReviewId) as ChildrenCount, Ratio.ProductRatio as Rating " +
                    "FROM [CMS].[Review] as ParentReview " +
                    "LEFT JOIN Catalog.Photo ON ParentReview.ReviewId = Photo.ObjId AND Main = 1 AND Photo.Type = @PhotoType " +
                    "LEFT JOIN Catalog.Ratio ON ParentReview.Type = @EntityTypeProduct AND Ratio.ProductID = ParentReview.EntityId AND Ratio.CustomerId = ParentReview.CustomerId " +
                    "WHERE [EntityId] = @EntityId AND ParentReview.[Type] = @entityType AND (@DisplayReviewsImage = 1 OR ParentReview.Text != '') order by AddDate desc",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@EntityId", entityId),
                    new SqlParameter("@entityType", (int)entityType),
                    new SqlParameter("@PhotoType", PhotoType.Review.ToString()),
                    new SqlParameter("@DisplayReviewsImage", SettingsCatalog.DisplayReviewsImage),
                    new SqlParameter("@EntityTypeProduct", EntityType.Product));
        }
        
        public static int GetReviewsCount(int entityId, EntityType entityType, bool onlyApprovedReviews = false, bool onlyFirstLevelReviews = false)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                    "SELECT count(ReviewID) FROM [CMS].[Review] " +
                    "WHERE [EntityId] = @EntityId AND [Type] = @Type AND (@DisplayReviewsImage = 1 OR [Text] != '')" +
                    (onlyApprovedReviews ? " AND [Checked] = 1" : "") + 
                    (onlyFirstLevelReviews ? " AND ParentId = 0 " : ""),
                    CommandType.Text,
                    new SqlParameter("@EntityId", entityId),
                    new SqlParameter("@Type", (int) entityType),
                    new SqlParameter("@DisplayReviewsImage", SettingsCatalog.DisplayReviewsImage));
        }

        public static List<Review> GetReviewsByParentId(int parentReviewId)
        {
            return SQLDataAccess.ExecuteReadList<Review>(
                "SELECT ParentReview.*, Photo.PhotoName, (SELECT Count(*) FROM [CMS].[Review] WHERE [ParentId] = ParentReview.ReviewId) as ChildrenCount, Ratio.ProductRatio as Rating " +
                "FROM [CMS].[Review] as ParentReview " +
                "LEFT JOIN Catalog.Ratio ON ParentReview.Type = @EntityTypeProduct AND Ratio.ProductID = ParentReview.EntityId AND Ratio.CustomerId = ParentReview.CustomerId " +
                "LEFT JOIN Catalog.Photo ON ParentReview.ReviewId = Photo.ObjId AND Main = 1 AND Photo.Type = @PhotoType " +
                "WHERE [ParentId] = @ParentId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@ParentId", parentReviewId),
                new SqlParameter("@PhotoType", PhotoType.Review.ToString()),
                new SqlParameter("@EntityTypeProduct", EntityType.Product));
        }

        public static List<int> GetReviewChildrenIds(int reviewId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT [ReviewId] FROM [CMS].[Review] WHERE [ParentId] = @ParentId",
                CommandType.Text, "ReviewId",
                new SqlParameter("@ParentId", reviewId));
        }

        public static IEnumerable<Review> GetReviewList()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Review>(
                "SELECT ParentReview.*, Photo.PhotoName, (SELECT Count(*) FROM [CMS].[Review] WHERE [ParentId] = ParentReview.ReviewId) as ChildrenCount, Ratio.ProductRatio as Rating " +
                "FROM [CMS].[Review] as ParentReview "+ 
                "LEFT JOIN Catalog.Photo ON ParentReview.ReviewId = Photo.ObjId AND Main = 1 AND Photo.Type = @PhotoType " +
                "LEFT JOIN Catalog.Ratio ON ParentReview.Type = @EntityTypeProduct AND Ratio.ProductID = ParentReview.EntityId AND Ratio.CustomerId = ParentReview.CustomerId " +
                "order by AddDate desc",
                CommandType.Text, GetFromReader,
                new SqlParameter("@PhotoType", PhotoType.Review.ToString()),
                new SqlParameter("@EntityTypeProduct", EntityType.Product));
        }

        public static List<Review> GetTopReviewListOnMain(int count, ReviewsSortingOnMainPage sorting)
        {
            return
                CacheManager.Get(ReviewCacheKey + nameof(GetTopReviewListOnMain) + count + "_" + sorting,
                    () =>
                    {
                        var orderBy = "";
                        
                        switch (sorting)
                        {
                            case ReviewsSortingOnMainPage.ByLikes:
                                orderBy = "ParentReview.RatioByLikes DESC, ParentReview.AddDate DESC";
                                break;
                            case ReviewsSortingOnMainPage.ByDate:
                                orderBy = "ParentReview.AddDate DESC";
                                break;
                            case ReviewsSortingOnMainPage.ByReviewRatingThenDate:
                                orderBy = "Rating DESC, ParentReview.AddDate DESC";
                                break;
                        }
                        
                        return SQLDataAccess.ExecuteReadList(
                            "SELECT TOP " + count +
                            " ParentReview.*, Photo.PhotoName, (SELECT Count(*) FROM [CMS].[Review] WHERE [ParentId] = ParentReview.ReviewId) as ChildrenCount, Ratio.ProductRatio as Rating " +
                            "FROM [CMS].[Review] AS ParentReview " +
                            "LEFT JOIN Catalog.Photo ON ParentReview.ReviewId = Photo.ObjId AND MAIN = 1 AND Photo.Type = @PhotoType " +
                            "LEFT JOIN Catalog.Ratio ON ParentReview.Type = @EntityTypeProduct AND Ratio.ProductID = ParentReview.EntityId AND Ratio.CustomerId = ParentReview.CustomerId " +
                            "WHERE ParentReview.ParentId = 0 AND ParentReview.ShowOnMain = 1 " +
                            "ORDER BY " + orderBy,
                            CommandType.Text,
                            GetFromReader,
                            new SqlParameter("@PhotoType", PhotoType.Review.ToString()),
                            new SqlParameter("@EntityTypeProduct", EntityType.Product));
                    });
        }

        private static Review GetFromReader(SqlDataReader reader)
        {
            return new Review
            {
                ReviewId = SQLDataHelper.GetInt(reader, "ReviewId"),
                ParentId = SQLDataHelper.GetNullableInt(reader, "ParentId") ?? 0,
                EntityId = SQLDataHelper.GetInt(reader, "EntityId"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Text = SQLDataHelper.GetString(reader, "Text"),
                Checked = SQLDataHelper.GetBoolean(reader, "Checked"),
                AddDate = SQLDataHelper.GetDateTime(reader, "AddDate"),
                Ip = SQLDataHelper.GetString(reader, "IP"),
                ChildrenCount = SQLDataHelper.GetInt(reader, "ChildrenCount"),
                PhotoName = SQLDataHelper.GetString(reader, "PhotoName"),
                LikesCount = SQLDataHelper.GetInt(reader, "LikesCount"),
                DislikesCount = SQLDataHelper.GetInt(reader, "DislikesCount"),
                RatioByLikes = SQLDataHelper.GetInt(reader, "RatioByLikes"),
                ShowOnMain = SQLDataHelper.GetBoolean(reader, "ShowOnMain"),
                Rating = SQLDataHelper.GetNullableInt(reader, "Rating")
            };
        }

        public static void AddReview(Review review)
        {
            review.ReviewId = SQLDataHelper.GetInt(
                SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [CMS].[Review] " +
                    " ([ParentId], [EntityId], [Type], [CustomerId], [Name], [Email], [Text], [Checked], [AddDate], [IP], [ShowOnMain], [LikesCount], [DislikesCount], [RatioByLikes]) " +
                    " VALUES (@ParentId, @EntityId, @Type, @CustomerId, @Name, @Email, @Text, @Checked, @AddDate, @IP, @ShowOnMain, 0, 0, 0); SELECT SCOPE_IDENTITY(); ",
                    CommandType.Text,
                    new SqlParameter("@ParentId", review.ParentId),
                    new SqlParameter("@EntityId", review.EntityId),
                    new SqlParameter("@Type", (int) review.Type),
                    new SqlParameter("@CustomerId", review.CustomerId),
                    new SqlParameter("@Name", review.Name),
                    new SqlParameter("@Email", review.Email),
                    new SqlParameter("@Text", review.Text),
                    new SqlParameter("@Checked", review.Checked),
                    new SqlParameter("@IP", review.Ip),
                    new SqlParameter("@AddDate", review.AddDate),
                    new SqlParameter("@ShowOnMain", review.ShowOnMain))
                );

            if (review.Type == EntityType.Product)
                ProductService.PreCalcProductComments(review.EntityId);
            
            BizProcessExecuter.ReviewAdded(review);
            CacheManager.RemoveByPattern(ReviewCacheKey);
        }

        public static void UpdateReview(Review review)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [CMS].[Review] SET [ParentId] = @ParentId, [EntityId] = @EntityId, [Type] = @Type, [CustomerId] = @CustomerId, [Name] = @Name, [Email] = @Email, [Text] = @Text , [Checked] = @Checked, AddDate=@AddDate, [ShowOnMain] = @ShowOnMain  WHERE reviewId = @reviewId",
                CommandType.Text,
                new SqlParameter("@reviewId", review.ReviewId),
                new SqlParameter("@ParentId", review.ParentId),
                new SqlParameter("@EntityId", review.EntityId),
                new SqlParameter("@Type", review.Type),
                new SqlParameter("@CustomerId", review.CustomerId),
                new SqlParameter("@Name", review.Name),
                new SqlParameter("@Email", review.Email),
                new SqlParameter("@Checked", review.Checked),
                new SqlParameter("@Text", review.Text),
                new SqlParameter("@AddDate", review.AddDate),
                new SqlParameter("@ShowOnMain", review.ShowOnMain));

            if (review.Type == EntityType.Product)
                ProductService.PreCalcProductComments(review.EntityId);

            CacheManager.RemoveByPattern(ReviewCacheKey);
        }

        public static void DeleteReview(int reviewId)
        {
            var review = GetReview(reviewId);

            // Список удаляемых коментов
            var deleteIds = new List<int> { reviewId };
            var newIds = new List<int> { reviewId };

            // Пока есть новые коментарии для удаления
            while (newIds.Count > 0)
            {
                var listIds = new List<int>();

                // Берём все дочерние коменты, у всех комметариев с прошлой итерации
                foreach (var newId in newIds)
                {
                    listIds.AddRange(GetReviewChildrenIds(newId));
                }

                // Добавляем в список
                deleteIds.AddRange(listIds);

                newIds.Clear();
                newIds.AddRange(listIds);
            }

            // Удаляем комменты
            foreach (var deleteId in deleteIds)
            {
                PhotoService.DeletePhotos(deleteId, PhotoType.Review);
                DeleteCommentFromDb(deleteId);
            }

            if (review != null && review.Type == EntityType.Product)
            {
                if (review.ParentId == 0 && !IsCustomerHaveReview(review.EntityId, EntityType.Product, review.CustomerId))
                    RatingService.DeleteUserProductRating(review.EntityId, review.CustomerId);

                ProductService.PreCalcProductComments(review.EntityId);
            }

            CacheManager.RemoveByPattern(ReviewCacheKey);
        }
        
        private static void DeleteCommentFromDb(int commentId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CMS].[Review] WHERE ReviewId = @ReviewId", CommandType.Text,
                new SqlParameter("@ReviewId", commentId));
        }

        public static void AddVote(int reviewId, bool like)
        {
            var customerId = Customers.CustomerContext.CurrentCustomer.Id;

            SQLDataAccess.ExecuteNonQuery("[CMS].[sp_AddReviewLike]", CommandType.StoredProcedure,
                new SqlParameter("@ReviewId", reviewId),
                new SqlParameter("@IsLike", like),
                new SqlParameter("@CustomerId", customerId));

            CacheManager.RemoveByPattern(ReviewCacheKey);
        }

        public static bool IsCustomerHaveReview(int entityId, EntityType entityType, Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT TOP 1 1 FROM [CMS].[Review] WHERE [ParentId] = 0 AND [EntityId] = @EntityId AND [Type] = @Type AND [CustomerId] = @CustomerId", 
                CommandType.Text,
                new SqlParameter("@EntityId", entityId),
                new SqlParameter("@Type", entityType),
                new SqlParameter("@CustomerId", customerId));
        }
    }
}