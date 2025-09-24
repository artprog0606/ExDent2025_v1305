using AdvantShop.CMS;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Models.BonusSystemModule;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.SEO;

namespace AdvantShop.Controllers
{
    public partial class BonusesController : BaseClientController
    {
        public ActionResult GetBonusCard()
        {
            if (!BonusSystem.IsActive)
                return Error404();
            
            var breadCrumbs = new List<BreadCrumbs>()
            {
                new BreadCrumbs(T("MainPage"), Url.AbsoluteRouteUrl("Home")),
                new BreadCrumbs(T("Module.BonusSystem.GetBonusCardTitle"), Url.AbsoluteRouteUrl("GetBonusCardRoute"))
            };
            
            var h1 = MetaType.BonusProgram.ToString();
            
            var metaInformation = SetMetaInformation(MetaInfoService.GetDefaultMetaInfo(MetaType.BonusProgram, h1), h1);

            var model = new GetBonusCardViewModel
            {
                BreadCrumbs = breadCrumbs,
                BonusTextBlock = BonusSystem.BonusTextBlock,
                BonusRightTextBlock = BonusSystem.BonusRightTextBlock,
                Grades = BonusSystem.IsActive && BonusSystem.BonusShowGrades
                        ? BonusSystemService.GetGrades()
                        : null,
                MetaId = metaInformation.MetaId,
                H1 = metaInformation.H1,
                Title = metaInformation.Title
            };

            SetNgController(Web.Infrastructure.Controllers.NgControllers.NgControllersTypes.BonusPageCtrl);

            return View(model);
        }

        public JsonResult BonusJson()
        {
            var customer = CustomerContext.CurrentCustomer;

            var bonusCard = BonusSystemService.GetCard(customer.Id);

            if (bonusCard == null)
            {
                var checkoutData = OrderConfirmationService.Get(customer.Id);
                if (checkoutData != null && checkoutData.User.BonusCardId != null)
                {
                    bonusCard = BonusSystemService.GetCard(checkoutData.User.BonusCardId.Value);
                }
            }

            if (bonusCard == null && Session["bonuscard"] != null)
            {
                bonusCard = BonusSystemService.GetCard((long)Session["bonuscard"]);
            }

            if (bonusCard != null)
            {
                var current = MyCheckout.Factory(customer.Id);
                var bonusAmount = (float)bonusCard.BonusesTotalAmount.SimpleRoundPrice();

                
                var maxBonus = bonusAmount;
                var shippingPrice = 0f;
                if (current.Data.SelectShipping != null || current.AvailableShippingOptions().Count == 0)
                {
                    shippingPrice = current.Data.SelectShipping?.FinalRate ?? 0;
                    var bonusCost = BonusSystemService.GetBonusCost(current.Cart, shippingPrice, bonusAmount, current.Data.User.WantRegist || current.Data.User.WantBonusCard);

                    if (bonusAmount > bonusCost.BonusPrice)
                        maxBonus = bonusCost.BonusPrice;
                }
                maxBonus = (float)Math.Truncate(maxBonus);
                var bonusPlus = BonusSystemService.GetBonusCost(current.Cart, shippingPrice, 0, current.Data.User.WantRegist || current.Data.User.WantBonusCard);
                
                var temporaryBonuses = BonusService.GetAllTemporary(bonusCard.CardId)
                    .Select(bonus => new 
                    {
                        Name = bonus.Name,
                        Amount = bonus.Amount,
                        StartDate = bonus.StartDate?.ToString("dd MMMM yyyy"),
                        EndDate = bonus.EndDate?.ToString("dd MMMM yyyy")
                    });
                
                var transactions = TransactionService.GetLastUnitedByDateAndType(bonusCard.CardId)
                    .Select(transaction => new
                    {
                        IsAdd = (int)transaction.OperationType % 2 == 0 ? false : true,
                        Amount = (int)transaction.OperationType % 2 == 0 ? -transaction.Amount : transaction.Amount,
                        Foundation = transaction.Basis,
                        Date = transaction.CreateOn.Date == DateTime.Now.Date 
                            ? $"Сегодня {transaction.CreateOn.ToString("H:mm")}" 
                            : transaction.CreateOn.Date == DateTime.Now.Date.AddDays(-1) 
                                ? $"Вчера {transaction.CreateOn.ToString("H:mm")}"
                                : transaction.CreateOn.ToString("d.MM.yyyy H:mm")
                    });
                
                return Json(new
                {
                    bonus = new
                    {
                        CardNumber = bonusCard.CardNumber,
                        BonusAmount = bonusAmount,
                        BonusPercent = bonusCard.Grade.BonusPercent,
                        BonusGradeName = bonusCard.Grade.Name,
                        Blocked = bonusCard.Blocked,
                        TemporaryBonuses = temporaryBonuses,
                        Transactions = transactions,
                        TransactionsMode = 0
                    },
                    bonusText = $"{T("Bonuses.YourBonuses")} {bonusAmount.FormatBonuses()}" + 
                                (maxBonus > 0 && BonusSystem.AllowSpecifyBonusAmount ? $". {T("Bonuses.AvailableBonuses")} {maxBonus.FormatBonuses()}. <br/>{T("Bonuses.SelectBonusAmount")}" : null),
                    bonusPlus = bonusPlus.BonusPlus,
                    maxBonus
                });
            }
            
            return Json(null);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateBonusCard()
        {
            var customer = CustomerContext.CurrentCustomer;
            if (!customer.RegistredUser)
                return Json(new { result = false, error = T("Bonuses.UserNotRegistred") });

            var bonusCard = BonusSystemService.GetCard(customer.Id);
            if (bonusCard != null)
                return Json(new { result = false, error = T("Bonuses.BonusCardAlreadyRegistered") });

            customer.BonusCardNumber = BonusSystemService.AddCard(new Card { CardId = customer.Id });
            CustomerService.UpdateCustomer(customer);

            return Json(new {result = true});
        }
    }
}