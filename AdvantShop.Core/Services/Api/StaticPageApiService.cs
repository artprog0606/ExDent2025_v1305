using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Api
{
    public class StaticPageApiService
    {
        public List<StaticPageApi> GetList()
        {
            return SQLDataAccess.Query<StaticPageApi>("Select * from CMS.StaticPageApi").ToList();
        }
        
        public StaticPageApi Get(int id)
        {
            return SQLDataAccess
                .Query<StaticPageApi>("Select * from CMS.StaticPageApi Where Id=@id", new {id})
                .FirstOrDefault();
        }
        
        public void Add(StaticPageApi page)
        {
            page.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into CMS.StaticPageApi (ParentId, Title, Text, Icon, IconName, Enabled, AddDate, ModifyDate, SortOrder, ShowInProfile) " +
                    "Values (@ParentId, @Title, @Text, @Icon, @IconName, @Enabled, getdate(), getdate(), @SortOrder, @ShowInProfile); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@ParentId", page.ParentId ?? (object) DBNull.Value),
                    new SqlParameter("@Title", page.Title ?? ""),
                    new SqlParameter("@Text", page.Text ?? ""),
                    new SqlParameter("@Icon", page.Icon ?? (object) DBNull.Value),
                    new SqlParameter("@IconName", page.IconName ?? (object) DBNull.Value),
                    new SqlParameter("@Enabled", page.Enabled),
                    new SqlParameter("@SortOrder", page.SortOrder),
                    new SqlParameter("@ShowInProfile", page.ShowInProfile));
        }
        
        public void Update(StaticPageApi page)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CMS.StaticPageApi " +
                "Set ParentId=@ParentId, Title=@Title, Text=@Text, Icon=@Icon, IconName=@IconName, Enabled=@Enabled, ModifyDate=getdate(), SortOrder=@SortOrder, ShowInProfile=@ShowInProfile " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", page.Id),
                new SqlParameter("@ParentId", page.ParentId ?? (object) DBNull.Value),
                new SqlParameter("@Title", page.Title ?? ""),
                new SqlParameter("@Text", page.Text ?? ""),
                new SqlParameter("@Icon", page.Icon ?? (object) DBNull.Value),
                new SqlParameter("@IconName", page.IconName ?? (object) DBNull.Value),
                new SqlParameter("@Enabled", page.Enabled),
                new SqlParameter("@SortOrder", page.SortOrder),
                new SqlParameter("@ShowInProfile", page.ShowInProfile));
        }

        public void Delete(int id)
        {
            var page = Get(id);
            if (page != null)
            {
                SQLDataAccess.ExecuteNonQuery(
                    "Delete from CMS.StaticPageApi Where Id=@Id", CommandType.Text,
                    new SqlParameter("@Id", id));
                
                if (!string.IsNullOrEmpty(page.IconName))
                    FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.StaticPageApi, page.IconName));
            }
        }
    }
}