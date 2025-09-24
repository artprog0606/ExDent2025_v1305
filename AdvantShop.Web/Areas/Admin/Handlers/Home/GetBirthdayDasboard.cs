using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using System.Linq;
using AdvantShop.Web.Admin.ViewModels.Home;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetBirthdayDasboard
    {
        public GetBirthdayDasboard()
        {
        }

        public BirthdayDashboardViewModel Execute()
        {
            var customers = CustomerService.GetCustomerBirthdayInTheNextWeek();

            var model = new BirthdayDashboardViewModel() { Birthday = new List<BirtdayItem>() };

            foreach (var customer in customers.Where(x => x.BirthDay.HasValue))
            {
                var item = new BirtdayItem();
                item.SrcImage = customer.Avatar.IsNotEmpty()
                    ? FoldersHelper.GetPath(FolderType.Avatar, customer.Avatar, false)
                    : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
                item.CustomerId = customer.Id.ToString();
                item.FirstName = customer.FirstName;
                item.LastName = customer.LastName;

                var bd = customer.BirthDay.Value.Date.AddYears((DateTime.Now.Year - customer.BirthDay.Value.Year));
                var today = DateTime.Now.Date;

                var div = (bd.Date - DateTime.Now.Date).TotalDays;

                var strDays = (div > 0 ? LocalizationService.GetResource("Admin.Home.BirthdayDashboard.Description.In") + " " + Math.Abs(div) + " " : div != 0 ? Math.Abs(div) + " " : "") +
                                                    Strings.Numerals((float)Math.Abs(div),
                                                    LocalizationService.GetResource("Admin.Home.BirthdayDashboard.Description.Today"),
                                                    LocalizationService.GetResource("Core.Numerals.Days.One"),
                                                    LocalizationService.GetResource("Core.Numerals.Days.Two"),
                                                    LocalizationService.GetResource("Core.Numerals.Days.Five")) +
                                (div < 0 ? " " + LocalizationService.GetResource("Admin.Home.BirthdayDashboard.Description.Ago") : "");

                item.Description = string.Format("{0} ({1}) {2}", bd.ToString("d MMMM"), strDays, div < 0 
                                                                        ? LocalizationService.GetResource("Admin.Home.BirthdayDashboard.Description.WasBirthday")  
                                                                        : LocalizationService.GetResource("Admin.Home.BirthdayDashboard.Description.HappyBirthday"));
                item.IsToday = div == 0;
                item.Sorting = (int)div;
                model.Birthday.Add(item);
            }

            model.Birthday = model.Birthday.OrderBy(x => x.Sorting).ToList();
            return model;
        }
    }
}
