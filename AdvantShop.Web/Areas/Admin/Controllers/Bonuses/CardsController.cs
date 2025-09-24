using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Bonuses.Cards;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Statistic;
using System.Linq;

namespace AdvantShop.Web.Admin.Controllers.Bonuses
{
    [Auth(RoleAction.BonusSystem)]
    [SaasFeature(Saas.ESaasProperty.BonusSystem)]
    [SalesChannel(ESalesChannelType.Bonus)]
    public class CardsController : BaseAdminController
    {

        #region List of cards

        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Cards.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CardsCtrl);

            return View();
        }

        public JsonResult GetCards(CardFilterModel model)
        {
            return Json(new GetCardHandler(model).Execute());
        }

        #region Commands

        private void Command(CardFilterModel model, Func<Guid, CardFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None && model.Ids != null)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetCardHandler(model);
                var ids = handler.GetItemsIds("CardId");

                foreach (var id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCards(CardFilterModel model)
        {
            Command(model, (id, c) =>
            {
                new DeleteCardHandler(id).Execute();
                return true;
            });
            return Json(true);
        }

        #endregion

        #endregion

        #region Add | Edit card

        [HttpPost, ValidateJsonAntiForgeryToken, ValidateAjax]
        public JsonResult Add(CardModel model)
        {
            return ProcessJsonResult(new AddUpdateCard(model));
        }

        public ActionResult Edit(Guid id)
        {
            var card = CardService.Get(id);
            if (card == null)
                return RedirectToAction("Index");

            var bdrule = CustomRuleService.Get(ERule.ChangeGrade);

            var model = new CardModel()
            {
                IsEditMode = true,
                CardId = card.CardId,
                CardNumber = card.CardNumber,
                Name = card.Customer.LastName + " " + card.Customer.FirstName,
                Blocked = card.Blocked,
                BonusAmount = BonusService.ActualSum(id),
                GradeId = card.GradeId,
                CreateOn = card.CreateOn,
                DisabledChangeGrade = bdrule != null && bdrule.Enabled && !card.ManualGrade,
                ManualGrade = card.ManualGrade
            };
            model.Grades = GradeService.GetAll();
            SetMetaInformation(T("Admin.Cards.Index.Title") + " - " + card.CardNumber);
            SetNgController(NgControllers.NgControllersTypes.CardsCtrl);

            return View("Edit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CardModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var handler = new AddUpdateCard(model);
                    var result = handler.Execute();
                    if (result != Guid.Empty)
                    {
                        ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                        return RedirectToAction("Edit", new { id = model.CardId });
                    }
                }
                catch (BlException e)
                {
                    ModelState.AddModelError(e.Property, e.Message);
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Cards.Index.Title") + " - " + model.Name);
            SetNgController(NgControllers.NgControllersTypes.CardsCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCard(Guid cardId)
        {
            return ProcessJsonResult(new DeleteCardHandler(cardId));
        }

        #endregion

        [HttpGet]
        public ActionResult _Bonuses(Guid cardId)
        {
            var bonuses = BonusService.GetAll(cardId);
            return PartialView(bonuses);
        }

        [HttpGet]
        public ActionResult _LastPurchase(Guid cardId)
        {
            ViewBag.CardId = cardId;
            var ps = PurchaseService.GetLast(cardId);
            return PartialView(ps);
        }

        [HttpGet]
        public ActionResult _LastTransaction(Guid cardId)
        {
            ViewBag.CardId = cardId;
            var trs = TransactionService.GetLast(cardId);
            return PartialView(trs);
        }

        [HttpGet]
        public ActionResult _UserInfo(Guid cardId)
        {
            var customer = CustomerService.GetCustomer(cardId);
            return PartialView(customer);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddBonus(AddBonusModel model)
        {
            ModelState["Filter.SortingType"]?.Errors.Clear();
            return ProcessJsonResult(new AddBonusHandler(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddBonusMass(CardFilterModelAddBonus model)
        {
            ModelState["Filter.SortingType"]?.Errors.Clear();
            return ProcessJsonResult(new AddBonusMassHandler(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SubtractBonus(SubctractBonusModel model)
        {
            return ProcessJsonResult(new SubtractBonusHandler(model));
        }

        public JsonResult GetBonuses(Guid cardId)
        {
            var bonus = BonusService.GetAll(cardId);
            return JsonOk(bonus);
        }

        public ActionResult AllPurchase(PurcharseFilterModel model)
        {
            ViewBag.CardId = model.CardId;
            SetMetaInformation(T("Admin.Cards.AllPurchase.SalesHistory"));

            return View(new GetPurchaseHandler(model).Execute());
        }

        public ActionResult AllTransaction(TransactionFilterModel model)
        {
            ViewBag.CardId = model.CardId;
            SetMetaInformation(T("Admin.Cards.AllTransaction.TransactionHistory"));

            return View(new GetTransactionHandler(model).Execute());
        }

        public JsonResult Generate()
        {
            var newcard = BonusSystemService.GenerateCardNumber(0);
            return JsonOk(newcard);
        }

        public JsonResult ImportCards(HttpPostedFileBase file, bool accrueBonuses = false)
        {
            if (CommonStatistic.IsRun)
            {
                return Json(new { Result = true });
            }

            using (MemoryStream target = new MemoryStream())
            {
                using (Stream inputStream = file.InputStream)
                {
                    inputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    new ImportCardsHandler(data, accrueBonuses).Execute();
                    //thanks for bad code
                    return Json(new { Result = true });
                }
            }
        }

        public ActionResult HistoryPersent(PersenthistoryFilterModel model)
        {
            ViewBag.CardId = model.CardId;
            SetMetaInformation(T("Admin.Cards.HistoryPersent.ChangesHistory"));

            return View(new GetHistoryPercentHandler(model).Execute());
        }

        public ActionResult ExportCards()
        {
            try
            {
                var export = new ExportCardsHandler();
                var result = export.Execute();
                var fileName = $"cards{DateTime.Now:yyyyMMddhhmmss}.csv";
                return File(result, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return Json(false);
        }

        public JsonResult GetCustomerGroupsSelectItems()
        {
            var items = CustomerGroupService.GetCustomerGroupList().Select(x => new Models.SelectItemModel(x.GroupName, x.CustomerGroupId)).ToList();
            return Json(items);
        }

    }
}
