//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Repository
{
    public enum EnAdditionalOptionObjectType
    {
        City = 1,
        Trigger = 2,
        Region = 3,
        Country = 4,
    }

    public class AdditionalOptionsService
    {
        #region Get /  Add / Update / Delete 

        public static AdditionalOption Get(int objId, EnAdditionalOptionObjectType objectType, string nameOption)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Settings].[AdditionalOption] "
                + "WHERE ObjId = @ObjId AND ObjType = @ObjType AND [Name] = @Name",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (short) objectType),
                new SqlParameter("@Name", nameOption ?? (object) DBNull.Value));
        }
        
        public static List<AdditionalOption> Get(int objId, EnAdditionalOptionObjectType objectType)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[AdditionalOption] "
                + "WHERE ObjId = @ObjId AND ObjType = @ObjType",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (short) objectType));
        }

        public static List<AdditionalOption> Get(EnAdditionalOptionObjectType objectType, string nameOption)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[AdditionalOption] "
                + "WHERE ObjType = @ObjType AND [Name] = @Name",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@ObjType", (short)objectType),
                new SqlParameter("@Name", nameOption ?? (object)DBNull.Value));
        }

        public static AdditionalOption GetFromReader(SqlDataReader reader)
            => new AdditionalOption()
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                ObjId = SQLDataHelper.GetInt(reader, "ObjId"),
                ObjType = (EnAdditionalOptionObjectType) SQLDataHelper.GetInt(reader, "ObjType"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Value = SQLDataHelper.GetString(reader, "Value"),
            };

        public static void AddOrUpdate(AdditionalOption option)
        {
            option = option ?? throw new ArgumentNullException(nameof(option));

            SQLDataAccess.ExecuteNonQuery(
                @"IF EXISTS(SELECT 1 FROM [Settings].[AdditionalOption] WHERE ObjId = @ObjId AND ObjType = @ObjType AND [Name] = @Name)
                BEGIN 
                    UPDATE [Settings].[AdditionalOption] SET [Value] = @Value WHERE ObjId = @ObjId AND ObjType = @ObjType AND [Name] = @Name
                END 
                ELSE 
                BEGIN 
                    INSERT INTO [Settings].[AdditionalOption] ([ObjId],[ObjType],[Name],[Value]) 
                        VALUES (@ObjId, @ObjType, @Name, @Value); 
                END",
                CommandType.Text,
                new SqlParameter("@ObjId", option.ObjId),
                new SqlParameter("@ObjType", (short) option.ObjType),
                new SqlParameter("@Name", option.Name ?? (object) DBNull.Value),
                new SqlParameter("@Value", option.Value ?? (object) DBNull.Value)
            );
        }

        public static void Delete(int objId, EnAdditionalOptionObjectType objectType, string nameOption)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Settings].[AdditionalOption] WHERE ObjId = @ObjId AND ObjType = @ObjType AND [Name] = @Name",
                CommandType.Text,
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (short) objectType),
                new SqlParameter("@Name", nameOption ?? (object) DBNull.Value));
        }

        public static void Delete(int objId, EnAdditionalOptionObjectType objectType)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Settings].[AdditionalOption] WHERE ObjId = @ObjId AND ObjType = @ObjType",
                CommandType.Text,
                new SqlParameter("@ObjId", objId),
                new SqlParameter("@ObjType", (short) objectType));
        }

        #endregion
    }
}
