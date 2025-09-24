//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ImapJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var service = new ImapMailService();

                if (!service.IsValid())
                    return;

                var isFirstTime = SettingsMail.ImapLastUpdateLetterId == 0;

                var emails = service.GetLastEmails();
                if (emails.Count <= 0 || isFirstTime)
                    return;

                var storeEmail = SettingsMail.Login.ToLower();

                foreach (var email in emails)
                {
                    if (email.FromEmail == null || email.FromEmail.ToLower() == storeEmail)
                        continue;

                    var customerEmail = email.FromEmail.ToLower();

                    var customer = CustomerService.GetCustomerByEmail(customerEmail);
                    if (customer == null)
                        continue;

                    var informer = new AdminInformer(AdminInformerType.Email, 0, null)
                    {
                        EntityId = customer.InnerId,
                        ObjId = email.Id.TryParseInt(),
                        CustomerId = customer.Id,
                        Title = LocalizationService.GetResource("Core.Scheduler.Jobs.ImapJob.NewMail") + customerEmail,
                        Body =
                            LocalizationService.GetResourceFormat("Core.Scheduler.Jobs.ImapJob.NewMailBody",
                                StringHelper.AggregateStrings(" ", new[] { customer.FirstName, customer.LastName }),
                                email.Subject),
                        Link = "customers/view/" + customer.Id
                    };

                    if (customer.Manager != null && customer.Manager.Customer != null)
                        informer.PrivateCustomerId = customer.Manager.CustomerId;

                    AdminInformerService.Add(informer);
                }
            }
            catch (Exception ex)
            {
                context.LogError(ex.Message);
                Debug.Log.Error(ex);
            }
        }
    }
}