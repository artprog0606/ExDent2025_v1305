using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.ExportImport
{
    public class ExportCategoriesService
    {
        private readonly int _exportFeedId;

        public ExportCategoriesService(int exportFeedId)
        {
            _exportFeedId = exportFeedId;
        }

        public List<ExportCategoryModel> GetCategories(bool exportNotAvailable)
        {
            return SQLDataAccess.ExecuteReadList("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                reader => new ExportCategoryModel
                {
                    Id = SQLDataHelper.GetInt(reader, "CategoryID"),
                    ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                    Name = SQLDataHelper.GetString(reader, "Name")
                },
                new SqlParameter("@exportFeedId", _exportFeedId),
                new SqlParameter("@onlyCount", false),
                new SqlParameter("@exportNotAvailable", exportNotAvailable));
        }

        public int GetCategoriesCount(bool exportNotAvailable)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", _exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNotAvailable", exportNotAvailable));
        }
    }
}