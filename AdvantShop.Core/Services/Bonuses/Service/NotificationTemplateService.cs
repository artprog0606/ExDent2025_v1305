using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Bonuses.Service
{
    public class NotificationTemplateService
    {
        public static NotificationTemplate Get(ENotifcationType type, EBonusNotificationMethod method)
        {
            return SQLDataAccess2.Query<NotificationTemplate>("SELECT * FROM Bonus.NotificationTemplate WHERE NotificationTypeId=@NotificationTypeId AND NotificationMethod=@NotificationMethod", new { NotificationTypeId = type, NotificationMethod = method });
        }
        
        public static NotificationTemplate Get(int id)
        {
            return SQLDataAccess2.Query<NotificationTemplate>("SELECT * FROM Bonus.NotificationTemplate WHERE NotificationTemplateId=@NotificationTemplateId", new { NotificationTemplateId = id });
        }
        
        public static List<NotificationTemplate> Get(ENotifcationType type)
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<NotificationTemplate>("SELECT * FROM Bonus.NotificationTemplate WHERE NotificationTypeId=@NotificationTypeId", new { NotificationTypeId = type}).ToList();
        }

        public static ENotifcationType Add(NotificationTemplate model)
        {
            var temp = SQLDataAccess2.ExecuteScalar<ENotifcationType>("insert into Bonus.NotificationTemplate (NotificationTypeId,NotificationBody,NotificationMethod) values (@NotificationTypeId,@NotificationBody,@NotificationMethod); select @NotificationTypeId", model);
            return temp;
        }

        public static void Update(NotificationTemplate model)
        {
            SQLDataAccess2.ExecuteNonQuery("UPDATE Bonus.NotificationTemplate SET NotificationBody=@NotificationBody WHERE NotificationTypeId=@NotificationTypeId AND NotificationMethod=@NotificationMethod", model);
        }

        public static void AddNotificationLog(NotificationLog model)
        {
            SQLDataAccess2.ExecuteNonQuery("Insert into Bonus.NotificationLog (Body,State,Contact,Created,ContactType) values (@Body,@State,@Contact,@Created,@ContactType)", model);
        }

        public static List<NotificationTemplate> GetAll()
        {
            return SQLDataAccess2.ExecuteReadIEnumerable<NotificationTemplate>("Select * from Bonus.NotificationTemplate").ToList();
        }

        public static void Delete(int id)
        {
            SQLDataAccess2.ExecuteNonQuery("DELETE FROM Bonus.NotificationTemplate WHERE NotificationTemplateId=@NotificationTemplateId",
                new { NotificationTemplateId = id });
        }
    }
}
