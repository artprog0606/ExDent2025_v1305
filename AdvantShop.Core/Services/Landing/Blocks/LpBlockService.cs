using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Blocks
{
    public class LpBlockService
    {
        #region Block

        public int Add(LpBlock block)
        {
            block.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "INSERT INTO [CMS].[LandingBlock] ([LandingId],[Name],[ContentHtml],[Type],[Settings],[SortOrder],[Enabled],[NoCache], [ShowOnAllPagesTop], [ShowOnAllPagesBottom]) " +
                "VALUES (@LandingId,@Name,@ContentHtml,@Type,@Settings,@SortOrder,@Enabled,@NoCache, @ShowOnAllPagesTop, @ShowOnAllPagesBottom); Select scope_identity(); ",
                CommandType.Text,
                new SqlParameter("@LandingId", block.LandingId),
                new SqlParameter("@Name", block.Name),
                new SqlParameter("@ContentHtml", block.ContentHtml ?? string.Empty),
                new SqlParameter("@Type", block.Type),
                new SqlParameter("@Settings", block.Settings ?? string.Empty),
                new SqlParameter("@SortOrder", block.SortOrder),
                new SqlParameter("@Enabled", block.Enabled),
                new SqlParameter("@ShowOnAllPagesTop", block.ShowOnAllPagesTop),
                new SqlParameter("@ShowOnAllPagesBottom", block.ShowOnAllPagesBottom),
                new SqlParameter("@NoCache", block.NoCache)
                ));

            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);

            LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);

            return block.Id;
        }

        public void Update(LpBlock block)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingBlock] " +
                "Set [LandingId]=@LandingId, [Name]=@Name, [ContentHtml]=@ContentHtml, [Type]=@Type, [Settings]=@Settings, " +
                "[SortOrder]=@SortOrder, [Enabled]=@Enabled, [ShowOnAllPagesTop]=@ShowOnAllPagesTop, [ShowOnAllPagesBottom]=@ShowOnAllPagesBottom, [NoCache]=@NoCache " +
                "Where Id = @Id ",
                CommandType.Text,
                new SqlParameter("@Id", block.Id),
                new SqlParameter("@LandingId", block.LandingId),
                new SqlParameter("@Name", block.Name),
                new SqlParameter("@ContentHtml", block.ContentHtml ?? string.Empty),
                new SqlParameter("@Type", block.Type),
                new SqlParameter("@Settings", block.Settings ?? string.Empty),
                new SqlParameter("@SortOrder", block.SortOrder),
                new SqlParameter("@Enabled", block.Enabled),
                new SqlParameter("@ShowOnAllPagesTop", block.ShowOnAllPagesTop),
                new SqlParameter("@ShowOnAllPagesBottom", block.ShowOnAllPagesBottom),
                new SqlParameter("@NoCache", block.NoCache)
                );

            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);

            LpSiteService.UpdateModifiedDateByLandingId(block.LandingId);
        }

        public void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete [CMS].[LandingBlock] Where Id = @Id ", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);

            LpSiteService.UpdateModifiedDateByLandingId(id);
        }

        public LpBlock Get(int id)
        {
            return CacheManager.Get(LpConstants.BlockCachePrefix + id, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpBlock>("Select * From [CMS].[LandingBlock] Where Id = @id",
                        new { id }).FirstOrDefault());
        }

        public LpBlock Get(int lpId, string name)
        {
            return
                SQLDataAccess.Query<LpBlock>(
                    "Select * From [CMS].[LandingBlock] Where LandingId = @lpId and Name = @name",
                    new { lpId, name }).FirstOrDefault();
        }

        public List<LpBlock> GetList(int lpId)
        {
            return CacheManager.Get(LpConstants.BlockListCachePrefix + lpId, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpBlock>("Select * From [CMS].[LandingBlock] Where LandingId = @lpId Order By SortOrder",
                        new { lpId }).ToList());
        }

        public List<LpBlock> GetBlocksShowOnAllPages(int lpSiteId, int lpId, bool isTop)
        {
            return CacheManager.Get($"{LpConstants.BlockListCachePrefix}_on_all_pages_{lpSiteId}_{lpId}_{isTop}", LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpBlock>(
                        "Select * From [CMS].[LandingBlock] " +
                        "Where LandingId in (Select Id From [CMS].[Landing] Where LandingSiteId=@lpSiteId and Id <> @lpId) and "+
                        (isTop ? "ShowOnAllPagesTop = 1 " : "ShowOnAllPagesBottom = 1 ") +
                        "Order By SortOrder",
                        new { lpSiteId, lpId }).ToList());
        }

        public List<LpBlock> GetReferenceBlocks()
        {
            return CacheManager.Get(LpConstants.BlockListCachePrefix + "_references", LpConstants.LpCacheTime,
                () => SQLDataAccess.Query<LpBlock>("Select * From [CMS].[LandingBlock] Where [Type] = 'reference'").ToList());
        }

        public List<LpBlock> GetReferenceBlocksWithBlock(int blockId)
        {
            var result = new List<LpBlock>();

            foreach (var block in GetReferenceBlocks())
            {
                if (string.IsNullOrEmpty(block.Settings))
                    continue;

                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(block.Settings);
                if (settings != null && settings.ContainsKey("blockId"))
                {
                    var blockIds = settings["blockId"] as string;
                    if (!string.IsNullOrEmpty(blockIds))
                    {
                        var ids = blockIds.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                        if (ids.Contains(blockId.ToString()))
                            result.Add(block);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Sub block

        public int AddSubBlock(LpSubBlock block)
        {
            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);

            block.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "INSERT INTO [CMS].[LandingSubBlock] ([LandingBlockId],[Name],[ContentHtml],[Type],[Settings],[SortOrder]) VALUES (@LandingBlockId,@Name,@ContentHtml,@Type,@Settings,@SortOrder); Select scope_identity(); ",
                CommandType.Text,
                new SqlParameter("@LandingBlockId", block.LandingBlockId),
                new SqlParameter("@Name", block.Name),
                new SqlParameter("@ContentHtml", block.ContentHtml ?? string.Empty),
                new SqlParameter("@Type", block.Type),
                new SqlParameter("@Settings", block.Settings ?? string.Empty),
                new SqlParameter("@SortOrder", block.SortOrder)
                ));

            return block.Id;
        }

        public void UpdateSubBlock(LpSubBlock block)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingSubBlock] Set [LandingBlockId]=@LandingBlockId, [Name]=@Name, [ContentHtml]=@ContentHtml, [Type]=@Type, [Settings]=@Settings, [SortOrder]=@SortOrder Where Id = @Id ",
                CommandType.Text,
                new SqlParameter("@Id", block.Id),
                new SqlParameter("@LandingBlockId", block.LandingBlockId),
                new SqlParameter("@Name", block.Name),
                new SqlParameter("@ContentHtml", block.ContentHtml ?? string.Empty),
                new SqlParameter("@Type", block.Type),
                new SqlParameter("@Settings", block.Settings ?? string.Empty),
                new SqlParameter("@SortOrder", block.SortOrder)
                );

            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);
        }

        public void DeleteSubBlock(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [CMS].[LandingBlock] Where Id = @Id ", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);
        }

        public LpSubBlock GetSubBlock(int id)
        {
            return CacheManager.Get(LpConstants.SubBlockCachePrefix + id, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpSubBlock>("Select * From [CMS].[LandingSubBlock] Where Id = @id", new { id })
                        .FirstOrDefault());
        }

        public LpSubBlock GetSubBlock(int blockId, string name)
        {
            return CacheManager.Get(LpConstants.SubBlockCachePrefix + blockId + name, LpConstants.LpCacheTime,
                () =>
                    SQLDataAccess.Query<LpSubBlock>(
                        "Select * From [CMS].[LandingSubBlock] Where LandingBlockId = @blockId and Name=@name",
                        new { blockId, name }).FirstOrDefault());
        }

        public List<LpSubBlock> GetSubBlocks(int blockId)
        {
            return
                SQLDataAccess.Query<LpSubBlock>(
                    "Select * From [CMS].[LandingSubBlock] Where LandingBlockId = @blockId Order by SortOrder", new { blockId }).ToList();
        }

        #endregion

        public static List<Type> GetTypes(string typeName)
        {
            return CacheManager.Get("LpTypes_" + typeName,
                () =>
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => x.FullName.StartsWith("AdvantShop") && !x.FullName.Contains("AdvantShop.Module"))
                        .SelectMany(x => x.GetTypes())
                        .Where(x => x.GetInterface(typeName) != null)
                        .ToList());
        }


        public void TryToUpdateBlock(LpBlock block, LpBlockConfig config)
        {
            Dictionary<string, object> blockSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(block.Settings);
            Dictionary<string, object> configSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(config.Settings ?? ""));

            foreach (var key in configSettings.Keys)
            {
                if (!blockSettings.ContainsKey(key))
                {
                    blockSettings.Add(key, configSettings[key]);
                    continue;
                }
            }

            //ChangeValue("color_scheme", ref blockSettings, ref configSettings);
            //ChangeValue("classes", ref blockSettings, ref configSettings);
            //ChangeValue("padding_top", ref blockSettings, ref configSettings);
            //ChangeValue("padding_bottom", ref blockSettings, ref configSettings);

            block.Settings = JsonConvert.SerializeObject(blockSettings);
            new LpBlockService().Update(block);
        }

        //private void ChangeValue(string name, ref Dictionary<string, object> blockSettings, ref Dictionary<string, object> configSettings)
        //{
        //    if (configSettings.ContainsKey(name) && blockSettings.ContainsKey(name) && configSettings[name] != blockSettings[name])
        //    {
        //        blockSettings[name] = configSettings[name];
        //    }
        //}

        public void UpdateAllBlocksIfRequired()
        {

            if (SettingsLandingPage.LastLandingblocksUpdate == SettingsGeneral.SiteVersionDev)
            {
                return;
            }

            foreach (var lp in new LpService().GetList())
            {
                foreach (var block in new LpBlockService().GetList(lp.Id))
                {
                    var config = new LpBlockConfigService().Get(block.Name, lp.Template);

                    if (config == null)
                    {
                        continue;
                    }

                    try
                    {
                        TryToUpdateBlock(block, config);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
            }

            SettingsLandingPage.LastLandingblocksUpdate = SettingsGeneral.SiteVersionDev;

        }

    }
}
