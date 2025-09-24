using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.App.Landing.Domain.ColorSchemes
{
    public class LpColorSchemeService
    {
        public LpColorScheme GetByBlockId(int blockId)
        {
            return
                SQLDataAccess.Query<LpColorScheme>("Select * From [CMS].[LandingColorScheme] Where LpBlockId = @blockId", new { blockId })
                    .FirstOrDefault();
        }

        public void AddUpdate(LpColorScheme colorScheme)
        {
            SQLDataAccess.ExecuteNonQuery(
                "if (Exists(Select 1 From [CMS].[LandingColorScheme] Where LpBlockId = @LpBlockId)) " +
                "begin " +
                    "Update [CMS].[LandingColorScheme] Set Name=@Name, Class=@Class, BackgroundColor=@BackgroundColor, BackgroundColorAlt=@BackgroundColorAlt, " +
                        "TitleColor=@TitleColor, TitleBold=@TitleBold, SubTitleColor=@SubTitleColor, SubTitleBold=@SubTitleBold, TextColor=@TextColor, TextColorAlt=@TextColorAlt, TextBold=@TextBold, LinkColor=@LinkColor, LinkColorHover=@LinkColorHover, LinkColorActive=@LinkColorActive, " +
                        "ButtonTextColor=@ButtonTextColor, ButtonTextColorHover=@ButtonTextColorHover, ButtonTextColorActive=@ButtonTextColorActive, ButtonBorderColor=@ButtonBorderColor, ButtonBorderWidth=@ButtonBorderWidth, ButtonBorderRadius=@ButtonBorderRadius, " +
                        "ButtonBackgroundColor=@ButtonBackgroundColor, ButtonBackgroundColorHover=@ButtonBackgroundColorHover, ButtonBackgroundColorActive=@ButtonBackgroundColorActive, " +
                        "ButtonSecondaryTextColor=@ButtonSecondaryTextColor,ButtonSecondaryTextColorHover=@ButtonSecondaryTextColorHover,ButtonSecondaryTextColorActive=@ButtonSecondaryTextColorActive, ButtonSecondaryBorderColor=@ButtonSecondaryBorderColor, ButtonSecondaryBorderWidth=@ButtonSecondaryBorderWidth, ButtonSecondaryBorderRadius=@ButtonSecondaryBorderRadius, " +
                        "ButtonSecondaryBackgroundColor=@ButtonSecondaryBackgroundColor, ButtonSecondaryBackgroundColorHover=@ButtonSecondaryBackgroundColorHover, ButtonSecondaryBackgroundColorActive=@ButtonSecondaryBackgroundColorActive, " +
                        "DelimiterColor=@DelimiterColor, ButtonTextBold=@ButtonTextBold, ButtonSecondaryTextBold=@ButtonSecondaryTextBold " +
                    "Where LpBlockId = @LpBlockId " +
                "end " +
                "Else " +
                "begin " +
                    "Insert Into [CMS].[LandingColorScheme] " +
                        "([LpId],[LpBlockId],[Name],[Class],[BackgroundColor],[BackgroundColorAlt],[TitleColor],[TitleBold],[SubTitleColor],[SubTitleBold],[TextColor],[TextColorAlt],[TextBold],[LinkColor],[LinkColorHover],[LinkColorActive],[ButtonTextColor],[ButtonTextColorHover],[ButtonTextColorActive],[ButtonBorderColor],[ButtonBorderWidth],[ButtonBorderRadius],[ButtonBackgroundColor],[ButtonBackgroundColorHover],[ButtonBackgroundColorActive],[ButtonSecondaryTextColor],[ButtonSecondaryTextColorHover],[ButtonSecondaryTextColorActive],[ButtonSecondaryBorderColor],[ButtonSecondaryBorderWidth],[ButtonSecondaryBorderRadius],[ButtonSecondaryBackgroundColor],[ButtonSecondaryBackgroundColorHover],[ButtonSecondaryBackgroundColorActive],[DelimiterColor],[ButtonTextBold],[ButtonSecondaryTextBold]) Values " +
                        "(@LpId,@LpBlockId,@Name,@Class,@BackgroundColor,@BackgroundColorAlt,@TitleColor,@TitleBold,@SubTitleColor,@SubTitleBold,@TextColor,@TextColorAlt,@TextBold,@LinkColor,@LinkColorHover,@LinkColorActive,@ButtonTextColor,@ButtonTextColorHover,@ButtonTextColorActive,@ButtonBorderColor,@ButtonBorderWidth,@ButtonBorderRadius,@ButtonBackgroundColor,@ButtonBackgroundColorHover,@ButtonBackgroundColorActive,@ButtonSecondaryTextColor,@ButtonSecondaryTextColorHover,@ButtonSecondaryTextColorActive,@ButtonSecondaryBorderColor,@ButtonSecondaryBorderWidth,@ButtonSecondaryBorderRadius,@ButtonSecondaryBackgroundColor,@ButtonSecondaryBackgroundColorHover,@ButtonSecondaryBackgroundColorActive,@DelimiterColor,@ButtonTextBold,@ButtonSecondaryTextBold) " +
                "end",
                CommandType.Text,
               new SqlParameter("@LpId", colorScheme.LpId),
               new SqlParameter("@LpBlockId", colorScheme.LpBlockId),
               new SqlParameter("@Name", colorScheme.Name),
               new SqlParameter("@Class", colorScheme.Class),
               
               new SqlParameter("@BackgroundColor", colorScheme.BackgroundColor ?? ""),
               new SqlParameter("@BackgroundColorAlt", colorScheme.BackgroundColorAlt ?? ""),

               new SqlParameter("@TitleColor", colorScheme.TitleColor ?? ""),
               new SqlParameter("@TitleBold", colorScheme.TitleBold ?? ""),
               new SqlParameter("@SubTitleColor", colorScheme.SubTitleColor ?? ""),
               new SqlParameter("@SubTitleBold", colorScheme.SubTitleBold ?? ""),
               new SqlParameter("@TextColor", colorScheme.TextColor ?? ""),
               new SqlParameter("@TextColorAlt", colorScheme.TextColorAlt ?? ""),
               new SqlParameter("@TextBold", colorScheme.TextBold ?? ""),

               new SqlParameter("@LinkColor", colorScheme.LinkColor ?? ""),
               new SqlParameter("@LinkColorHover", colorScheme.LinkColorHover ?? ""),
               new SqlParameter("@LinkColorActive", colorScheme.LinkColorActive ?? ""),

               new SqlParameter("@ButtonTextColor", colorScheme.ButtonTextColor ?? ""),
               new SqlParameter("@ButtonTextColorHover", colorScheme.ButtonTextColorHover ?? ""),
               new SqlParameter("@ButtonTextColorActive", colorScheme.ButtonTextColorActive ?? ""),
               new SqlParameter("@ButtonBorderColor", colorScheme.ButtonBorderColor ?? ""),
               new SqlParameter("@ButtonBorderWidth", colorScheme.ButtonBorderWidth ?? ""),
               new SqlParameter("@ButtonBorderRadius", colorScheme.ButtonBorderRadius ?? ""),
               new SqlParameter("@ButtonTextBold", colorScheme.ButtonTextBold ?? ""),

               new SqlParameter("@ButtonBackgroundColor", colorScheme.ButtonBackgroundColor ?? ""),
               new SqlParameter("@ButtonBackgroundColorHover", colorScheme.ButtonBackgroundColorHover ?? ""),
               new SqlParameter("@ButtonBackgroundColorActive", colorScheme.ButtonBackgroundColorActive ?? ""),

               new SqlParameter("@ButtonSecondaryTextColor", colorScheme.ButtonSecondaryTextColor ?? ""),
               new SqlParameter("@ButtonSecondaryTextColorHover", colorScheme.ButtonSecondaryTextColorHover ?? ""),
               new SqlParameter("@ButtonSecondaryTextColorActive", colorScheme.ButtonSecondaryTextColorActive ?? ""),
               new SqlParameter("@ButtonSecondaryBorderColor", colorScheme.ButtonSecondaryBorderColor ?? ""),
               new SqlParameter("@ButtonSecondaryBorderWidth", colorScheme.ButtonSecondaryBorderWidth ?? ""),
               new SqlParameter("@ButtonSecondaryBorderRadius", colorScheme.ButtonSecondaryBorderRadius ?? ""),
               new SqlParameter("@ButtonSecondaryTextBold", colorScheme.ButtonSecondaryTextBold ?? ""),

               new SqlParameter("@ButtonSecondaryBackgroundColor", colorScheme.ButtonSecondaryBackgroundColor ?? ""),
               new SqlParameter("@ButtonSecondaryBackgroundColorHover", colorScheme.ButtonSecondaryBackgroundColorHover ?? ""),
               new SqlParameter("@ButtonSecondaryBackgroundColorActive", colorScheme.ButtonSecondaryBackgroundColorActive ?? ""),

               new SqlParameter("@DelimiterColor", colorScheme.DelimiterColor ?? "")
               );
        }

        public void Delete(int lpId, int blockId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete from [CMS].[LandingColorScheme] Where LpId = @lpId and LpBlockId = @blockId", CommandType.Text,
                new SqlParameter("@lpId", lpId),
                new SqlParameter("@blockId", blockId));
        }

        public List<LpColorScheme> GetListByLandingId(int lpId)
        {
            return
                SQLDataAccess.Query<LpColorScheme>("Select * From [CMS].[LandingColorScheme] Where LpId = @lpId", new {lpId})
                    .ToList();
        }

        public List<LpColorScheme> GetListShowOnAllPage(int siteId, int ignoreLpId)
        {
            return
                SQLDataAccess.Query<LpColorScheme>(
                    "Select * From [CMS].[LandingColorScheme] Where LpId in (Select Id From CMS].[Landing] Where LandingSiteId = @siteId) and LpId <> @ignoreLpId",
                    new {siteId, ignoreLpId})
                    .ToList();
        }
    }
}
