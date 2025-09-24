//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class ProductVideoService
    {
        public static List<ProductVideo> GetProductVideos(int productId)
        {
            List<ProductVideo> list = SQLDataAccess.ExecuteReadList<ProductVideo>(
                "SELECT ProductVideoID, ProductID, Name, PlayerCode, Description, VideoSortOrder FROM [Catalog].[ProductVideo] WHERE [ProductID]=@ProductID ORDER BY [VideoSortOrder]",
                CommandType.Text, GetProductVideoFromReader, new SqlParameter { ParameterName = "@ProductID", Value = productId });

            return list;
        }

        public static ProductVideo GetProductVideoFromReader(SqlDataReader reader)
        {
            return new ProductVideo
            {
                ProductVideoId = SQLDataHelper.GetInt(reader, "ProductVideoID"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                PlayerCode = SQLDataHelper.GetString(reader, "PlayerCode"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                VideoSortOrder = SQLDataHelper.GetInt(reader, "VideoSortOrder")
            };
        }

        public static void AddProductVideo(ProductVideo pv)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Catalog].[ProductVideo] ([ProductID], [Name], [PlayerCode], [Description], [VideoSortOrder]) VALUES (@ProductId, @Name, @PlayerCode, @Description, @VideoSortOrder)",
                            CommandType.Text, new[]
                                                        {
                                                         new SqlParameter("@ProductID", pv.ProductId),
                                                         new SqlParameter("@Name", pv.Name),
                                                         new SqlParameter("@PlayerCode", pv.PlayerCode),
                                                         new SqlParameter("@Description", pv.Description),
                                                         new SqlParameter("@VideoSortOrder", pv.VideoSortOrder)
                                                        }
                            );
        }

        public static void UpdateProductVideo(ProductVideo pv)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE Catalog.ProductVideo SET Name=@Name, PlayerCode=@PlayerCode, Description=@Description, VideoSortOrder=@VideoSortOrder WHERE ProductVideoID = @ProductVideoID",
                            CommandType.Text, new[]
                                                        {
                                                         new SqlParameter("@ProductVideoId", pv.ProductVideoId),
                                                         new SqlParameter("@Name", pv.Name),
                                                         new SqlParameter("@PlayerCode", pv.PlayerCode),
                                                         new SqlParameter("@Description", pv.Description),
                                                         new SqlParameter("@VideoSortOrder", pv.VideoSortOrder)
                                                        }
                            );
        }

        public static void UpdateProductVideo(int productVideoId, string name, int videoSortOrder)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE Catalog.ProductVideo SET Name=@Name, VideoSortOrder=@VideoSortOrder WHERE ProductVideoID=@ProductVideoID",
                                                CommandType.Text,
                                                new SqlParameter { ParameterName = "@Name", Value = name },
                                                new SqlParameter { ParameterName = "@VideoSortOrder", Value = videoSortOrder },
                                                new SqlParameter { ParameterName = "@ProductVideoId", Value = productVideoId }
                                                );
        }

        public static void DeleteProductVideo(int productVideoId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[ProductVideo] WHERE ProductVideoId=@ProductVideoId", CommandType.Text,
                                                new SqlParameter { ParameterName = "@ProductVideoId", Value = productVideoId });
        }

        public static void DeleteProductVideos(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[ProductVideo] WHERE ProductId=@ProductId", CommandType.Text,
                                                new SqlParameter { ParameterName = "@ProductId", Value = productId });
        }

        public static ProductVideo GetProductVideo(int productVideoId)
        {
            return
                SQLDataAccess.ExecuteReadOne<ProductVideo>(
                    "SELECT TOP 1 * FROM [Catalog].[ProductVideo] WHERE [ProductVideoID] = @ProductVideoID", CommandType.Text,
                    GetProductVideoFromReader, new SqlParameter("@ProductVideoID", productVideoId));
        }

        public static bool HasVideo(int productId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                 "SELECT Count(ProductVideoID) FROM [Catalog].[ProductVideo] WHERE [ProductID] = @ProductID",
                 CommandType.Text, new SqlParameter("@ProductID", productId)) > 0;

        }

        public static string VideoToString(List<ProductVideo> productVideos)
        {
            if (productVideos.Count < 1) return "";

            var sb = new StringBuilder();
            foreach (var video in productVideos.OrderBy(v => v.VideoSortOrder))
            {
                sb.AppendFormat("{0}" + videoSeparator, video.PlayerCode);
            }
            return sb.ToString().Trim(videoSeparator);
        }

        private const char videoSeparator = ',';
        public static void VideoFromString(int productId, string videos)
        {
            var productVideos = GetProductVideos(productId);

            var incomingVideos = new List<ProductVideo>();
            videos = videos ?? "";

            var arrVideos = videos.Split(new[] { videoSeparator }, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (var videoStr in arrVideos)
            {
                var playerCode = videoStr.SupperTrim();
                if (playerCode.StartsWith("http://") || playerCode.StartsWith("https://"))
                {
                    string error;
                    playerCode = GetPlayerCodeFromLink(playerCode, out error);
                    if (!string.IsNullOrEmpty(error)) continue;
                }

                var item = new ProductVideo
                {
                    Description = string.Empty,
                    Name = string.Empty,
                    PlayerCode = playerCode,
                    ProductId = productId,
                    VideoSortOrder = 0
                };
                incomingVideos.Add(item);
                count++;
            }
            
            var deleteVideos = incomingVideos.Any() && productVideos.Any()
                               ? productVideos.Where(x => !incomingVideos.Select(y=>GetVideoCode(y.PlayerCode)).Contains(GetVideoCode(x.PlayerCode))).ToList()
                               : productVideos;
            
            var newVideos = incomingVideos.Any() && productVideos.Any()
                               ? incomingVideos.Where(x => !productVideos.Select(y => GetVideoCode(y.PlayerCode)).Contains(GetVideoCode(x.PlayerCode))).ToList()
                               : incomingVideos;

            foreach (var item in deleteVideos)
            {
                DeleteProductVideo(item.ProductVideoId);
            }
            foreach (var item in newVideos)
            {
                AddProductVideo(item);
            }
        }

        private static string GetVideoCode(string str)
        {            
            if (str.StartsWith("<iframe"))
            {                        
                Regex regex = new Regex(@"(?<=\bsrc="")[^""]*");
                Match match = regex.Match(str);
                string videocode = match.Value.Trim('/').Replace("https://", "").Replace("http://","");
                return videocode;
            }
            return str;
        }

        public static string GetPlayerCodeFromLink(string videoLink, out string errorMessage)
        {
            videoLink = videoLink.SupperTrim();
            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(videoLink))
                {
                    errorMessage = LocalizationService.GetResource("Core.Catalog.ProductVideo.NoPlayerCode");
                    return string.Empty;
                }

                if (videoLink.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(videoLink);
                    return string.Format("<iframe-responsive data-src=\"//www.youtube.com/embed/{0}?rel=0\"></iframe-responsive>",
                        uri.GetLeftPart(UriPartial.Path).Split(new[] { "youtu.be/" }, StringSplitOptions.None).Last());
                }

                if (videoLink.Contains("youtube.com", StringComparison.OrdinalIgnoreCase))
                {
                    videoLink = videoLink.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                        ? videoLink
                        : videoLink.Replace("http://", "https://");
                    
                    var videoUri = new Uri(videoLink);
                    
                    if (!Uri.IsWellFormedUriString(videoLink, UriKind.Absolute))
                    {
                        errorMessage = LocalizationService.GetResource("Core.Catalog.ProductVideo.WrongLink");
                        return string.Empty;
                    }

                    if (videoLink.Contains("v=", StringComparison.OrdinalIgnoreCase))
                    {
                        var videoId = System.Web.HttpUtility.ParseQueryString(videoUri.Query).Get("v");
                        return $"<iframe-responsive data-src=\"//www.youtube.com/embed/{videoId}?rel=0\"></iframe-responsive>";
                    }

                    if (videoLink.Contains("shorts", StringComparison.OrdinalIgnoreCase))
                    {
                        var videoId = videoUri.LocalPath.Split("/").Last();
                        return $"<iframe-responsive data-src=\"//www.youtube.com/embed/{videoId}?rel=0\"></iframe-responsive>";
                    }

                    return $"<iframe-responsive data-src=\"//www.youtube.com/embed/{videoUri.GetLeftPart(UriPartial.Path)}?rel=0\"></iframe-responsive>";
                }

                if (videoLink.Contains("vimeo.com"))
                {
                    var uri = new Uri(videoLink);
                    return string.Format("<iframe-responsive data-src=\"//player.vimeo.com/video/{0}?title=0&amp;byline=0&amp;portrait=0\"></iframe-responsive>",
                        uri.GetLeftPart(UriPartial.Path).Split(new[] { "vimeo.com/" }, StringSplitOptions.None).Last());
                }

                if (videoLink.Contains("rutube.ru"))
                {
                    var uri = new Uri(videoLink);
                    
                    // https://rutube.ru/video/f537094d1ea7a7d8499feb03f36b5edf/?r=plwd
                    
                    if (videoLink.Contains("rutube.ru/video/")) 
                    {
                        var id = uri.GetLeftPart(UriPartial.Path).TrimEnd('/').Split(new[] { "rutube.ru/video/" }, StringSplitOptions.None).Last();
                        
                        return $"<iframe-responsive data-src=\"https://rutube.ru/play/embed/{id}\"></iframe-responsive>";
                    }

                    // sample https://rutube.ru/shorts/77fdb9310dc52ba29fb365a689c868ca/?r=wd
                    if (videoLink.Contains("rutube.ru/shorts/")) 
                    {
                        var id = uri.GetLeftPart(UriPartial.Path).TrimEnd('/').Split(new[] { "rutube.ru/shorts/" }, StringSplitOptions.None).Last();
                        
                        return $"<iframe-responsive data-src=\"https://rutube.ru/play/embed/{id}\" autoplay=\"true\" loop=\"true\"></iframe-responsive>";
                    }
                }

                if (videoLink.Contains("vk.com"))
                {
                    var uri = new Uri(videoLink);
                    return $"<iframe-responsive data-src=\"{uri}\"></iframe-responsive>";
                }

                errorMessage = LocalizationService.GetResource("Core.Catalog.ProductVideo.WrongLink");
                return string.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = LocalizationService.GetResource("Core.Catalog.ProductVideo.WrongLink");
                Diagnostics.Debug.Log.Error(ex);
                return string.Empty;
            }
        }


        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private const string VimeoLinkRegex = "vimeo\\.com/(?:.*#|.*/videos/)?([0-9]+)";
        private static Regex regexExtractIdYouTube = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        private static Regex regexExtractIdVimeo = new Regex(VimeoLinkRegex, RegexOptions.Compiled);

        public static string GetCoverVideoFromUrl(string str)
        {
            string videoId = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    if (str.Contains("youtu.be") || str.Contains("youtube.com"))
                    {
                        string authority = new UriBuilder(str).Uri.Authority.ToLower();

                        var regRes = regexExtractIdYouTube.Match(str.ToString());
                        if (regRes.Success)
                        {
                            videoId = regRes.Groups[1].Value;

                            return "//img.youtube.com/vi/" + videoId + "/maxresdefault.jpg";
;
                        }
                    }
                    else if (str.Contains("vimeo.com"))
                    {

                        string authority = new UriBuilder(str).Uri.Authority.ToLower();

                        var regRes = regexExtractIdVimeo.Match(str.ToString());

                        if (regRes.Success) {
                            videoId = regRes.Groups[1].Value;
                            return "http://i.vimeocdn.com/video/" + videoId + "_640.jpg";
                        }
                            
                    }

                }

            }
            catch (Exception ex)
            {
                Diagnostics.Debug.Log.Error(ex);
                return string.Empty;
            }

            return str;

            //if (str.StartsWith("<iframe"))
            //{
            //Regex regex = new Regex(@"(?<=\bsrc="")[^""]*");
            //    Match match = regex.Match(str);
            //    string videocode = match.Value.Trim('/').Replace("https://", "").Replace("http://", "");
            //    return videocode;
            //}
        }

        public static int GetProductVideosCount(int productId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Catalog].[ProductVideo] WHERE [ProductID]=@ProductID",
                CommandType.Text,
                new SqlParameter { ParameterName = "@ProductID", Value = productId });
        }
    }
}

