using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Core.Services.TemplatesDocx.Templates;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.TemplatesDocx;
using AdvantShop.Web.Admin.Models.Settings.Partners;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings, RoleAction.Partners)]
    public class SettingsPartnersController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Partners.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsPartnersCtrl);

            var model = new PartnersSettingsModel()
            {
                DefaultRewardPercent = SettingsPartners.DefaultRewardPercent,
                PayoutMinCustomersCount = SettingsPartners.PayoutMinCustomersCount,
                PayoutMinBalance = SettingsPartners.PayoutMinBalance,
                PayoutCommissionNaturalPerson = SettingsPartners.PayoutCommissionNaturalPerson,
                AutoApplyPartnerCoupon = SettingsPartners.AutoApplyPartnerCoupon,
                EnableCaptchaInRegistrationPartners = SettingsPartners.EnableCaptchaInRegistrationPartners,
                ActivatePartners = SettingsPartners.ActivatePartners,
                EmailNotification = SettingsPartners.EmailNotification,
                SkipRegPasportStepForIndividualEntity = SettingsPartners.SkipRegPasportStepForIndividualEntity
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(PartnersSettingsModel model)
        {
            SettingsPartners.DefaultRewardPercent = model.DefaultRewardPercent;
            SettingsPartners.PayoutMinCustomersCount = model.PayoutMinCustomersCount;
            SettingsPartners.PayoutMinBalance = model.PayoutMinBalance;
            SettingsPartners.PayoutCommissionNaturalPerson = model.PayoutCommissionNaturalPerson;
            SettingsPartners.AutoApplyPartnerCoupon = model.AutoApplyPartnerCoupon;
            SettingsPartners.EnableCaptchaInRegistrationPartners = model.EnableCaptchaInRegistrationPartners;
            SettingsPartners.ActivatePartners = model.ActivatePartners;
            SettingsPartners.EmailNotification = model.EmailNotification;
            SettingsPartners.SkipRegPasportStepForIndividualEntity = model.SkipRegPasportStepForIndividualEntity;

            ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));

            return Index();
        }

        public JsonResult GetFormData()
        {
            var paymentTypes = PaymentTypeService.GetPaymentTypes();
            var coupon = CouponService.GetPartnersCouponTemplate();
            return JsonOk(new
            {
                paymentTypes,
                coupon
            });
        }

        #region Payment Types

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePaymentTypesSorting(int id, int? prevId, int? nextId)
        {
            var paymentType = PaymentTypeService.GetPaymentType(id);
            if (paymentType == null)
                return JsonError();

            var paymentTypes = PaymentTypeService.GetPaymentTypes().Where(x => x.Id != paymentType.Id).ToList();

            if (prevId != null)
            {
                var index = paymentTypes.FindIndex(x => x.Id == prevId);
                paymentTypes.Insert(index + 1, paymentType);
            }
            else if (nextId != null)
            {
                var index = paymentTypes.FindIndex(x => x.Id == nextId);
                paymentTypes.Insert(index > 0 ? index - 1 : 0, paymentType);
            }

            for (int i = 0; i < paymentTypes.Count; i++)
            {
                paymentTypes[i].SortOrder = i + 10;
                PaymentTypeService.UpdatePaymentType(paymentTypes[i]);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddPaymentType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return JsonError();

            var paymentTypes = PaymentTypeService.GetPaymentTypes();
            var paymentType = new PaymentType
            {
                SortOrder = (paymentTypes.Count > 0 ? paymentTypes.Max(x => x.SortOrder) : 0) + 10,
                Name = name
            };

            PaymentTypeService.AddPaymentType(paymentType);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdatePaymentType(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return JsonError();

            var paymentType = PaymentTypeService.GetPaymentType(id);
            if (paymentType == null)
                return JsonError("Способ выплаты не найден");

            paymentType.Name = name;
            PaymentTypeService.UpdatePaymentType(paymentType);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePaymentType(int id)
        {
            if (PaymentTypeService.IsPaymentTypeInUse(id))
                return JsonError("Удаление невозможно. Способ оплаты указан у партнеров.");
            PaymentTypeService.DeletePaymentType(id);
            return JsonOk();
        }

        #endregion

        #region Category Reward Percent

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategoryRewardPercent(List<int> categoryIds, float rewardPercent)
        {
            if (categoryIds == null || !categoryIds.Any())
                return JsonError("Выберите категории");
            if (rewardPercent < 0)
                return JsonError("Неверный процент вознаграждения");
            foreach (var categoryId in categoryIds)
            {
                if (!CategoryService.IsExistCategory(categoryId))
                    continue;
                PartnerService.AddUpdateCategoryRewardPercent(categoryId, rewardPercent);
            }
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCategoryRewardPercent(int categoryId, float rewardPercent)
        {
            if (rewardPercent < 0)
                return JsonError("Неверный процент вознаграждения");
            PartnerService.UpdateCategoryRewardPercent(categoryId, rewardPercent);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategoryRewardPercent(int categoryId)
        {
            PartnerService.DeleteCategoryRewardPercent(categoryId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetRewardPercentCategories()
        {
            var items = PartnerService.GetRewardPercentCategories();
            var result = new List<CategoryRewardPercentModel>();

            foreach (var item in items)
            {
                var parents = CategoryService.GetParentCategories(item.Key).Reverse().ToList();
                result.Add(new CategoryRewardPercentModel
                {
                    CategoryId = item.Key,
                    Path = parents.Select(x => x.Name).AggregateString(" / "),
                    RewardPercent = item.Value
                });
            }
            result = result.OrderBy(x => x.Path).ToList();

            return JsonOk(result);
        }

        #endregion

        #region Act-Report Templates

        public JsonResult GetActReportTplsData()
        {
            var tplFiles = new Dictionary<string, string>();
            foreach (EPartnerType type in Enum.GetValues(typeof(EPartnerType)).Cast<EPartnerType>().Where(x => x != EPartnerType.None))
                tplFiles.Add(type.ToString(), PartnerReportService.GetActReportTplDocx(type, false));

            return JsonOk(new
            {
                tplFiles,
                actTplsHelpText = FileHelpers.GetFilesHelpText(EFileType.Document)
            });
        }

        public ActionResult GetActReportTplFile(EPartnerType type)
        {
            var file = FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx, PartnerReportService.GetActReportTplDocx(type));
            return File(file, "application/octet-stream", "ActReportTpl_" + type.ToString() + System.IO.Path.GetExtension(file));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ResetActReportTpl(EPartnerType type)
        {
            var oldFileName = PartnerReportService.GetActReportTplDocx(type, false);
            if (oldFileName.IsNotEmpty())
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx, oldFileName));

            PartnerReportService.SetActReportTplDocx(type, null);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveActReportTplFile(EPartnerType type)
        {
            if (Request.Files.Count > 0 && Request?.Files[0]?.ContentLength > 0)
            {
                var oldFileName = PartnerReportService.GetActReportTplDocx(type, false);
                var file = Request.Files[0];

                if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Document))
                    return JsonError("Недопустимый формат файла");
                if (FileHelpers.FileStorageLimitReached(file.ContentLength))
                    return JsonError("Достигнуто ограничение объема файлов");

                if (oldFileName.IsNotEmpty())
                    FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx, oldFileName));

                var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
                FileHelpers.SaveFile(FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx, fileName), file.InputStream);
                PartnerReportService.SetActReportTplDocx(type, fileName);
            }

            return JsonOk();
        }

        public JsonResult GetActReportTplDescription(EPartnerType type)
        {
            var items = new List<TemplateDocxItem>();
            switch (type)
            {
                case EPartnerType.LegalEntity:
                    items = TemplatesDocxServices.TypeToTemplateItems<PartnerLegalEntityActTemplate>();
                    break;
                case EPartnerType.NaturalPerson:
                    items = TemplatesDocxServices.TypeToTemplateItems<PartnerNaturalPersonActTemplate>();
                    break;
            }

            return JsonOk(new
            {
                Fields = new GetDescriptionHandler().GetDescription(items)
            });
        }

        #endregion
    }
}
