using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.TemplatesDocx;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsTemplatesDocxController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.TemplatesDocx"));
            SetNgController(NgControllers.NgControllersTypes.SettingsTemplatesDocxCtrl);

            return View();
        }

        public JsonResult GetPaging(TemplatesDocxFilterModel model)
        {
            return Json(new GetTemplatesDocxHandler(model).Execute());
        }

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(TemplatesDocxModel model)
        {
            if (ModelState.IsValid)
            {
                var template = TemplatesDocxServices.Get<TemplateDocx>(model.Id);
                if (template == null)
                    return JsonError();

                template.Name = model.Name;
                template.SortOrder = model.SortOrder;

                TemplatesDocxServices.Update(template);

                return JsonOk();
            }

            return JsonError();
        }

        #endregion

        #region Commands

        private void Command(TemplatesDocxFilterModel command, Action<int, TemplatesDocxFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetTemplatesDocxHandler(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTemplates(TemplatesDocxFilterModel command)
        {
            Command(command, (id, c) => TemplatesDocxServices.DeleteTemplate<TemplateDocx>(id));
            return Json(true);
        }

        #endregion

        #region CRUD

        public JsonResult Get(int id)
        {
            var template = TemplatesDocxServices.Get<TemplateDocx>(id);
            if (template == null)
                return JsonError("Шаблон не найден");

            return JsonOk((TemplatesDocxModel)template);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTemplate(TemplatesDocxModel model)
        {
            if (ModelState.IsValid)
            {
                if (System.Web.HttpContext.Current.Request.Files.Count <= 0 ||
                    System.Web.HttpContext.Current.Request.Files[0].ContentLength <= 0)
                    return JsonError("Не указан файл шаблона");

                var file = System.Web.HttpContext.Current.Request.Files[0];

                if (!TemplatesDocxServices.CheckFileExtension(file.FileName))
                    return JsonError("Недопустимый формат файла");
                if (FileHelpers.FileStorageLimitReached(file.ContentLength))
                    return JsonError("Достигнуто ограничение объема файлов");

                var template = new TemplateDocx
                {
                    Id = model.Id,
                    Type = model.Type,
                    Name = model.Name,
                    FileName = string.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName)),
                    FileSize = file.ContentLength,
                    SortOrder = model.SortOrder,
                    DebugMode = model.DebugMode,
                };

                template.Id = TemplatesDocxServices.Add(template);

                if (template.Id != 0)
                    FileHelpers.SaveFile(TemplatesDocxServices.GetPathAbsolut(template), file.InputStream);

                return JsonOk();
            }

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateTemplate(TemplatesDocxModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return JsonError();

            var template = TemplatesDocxServices.Get<TemplateDocx>(model.Id);
            if (template == null)
                return JsonError("Шаблон не найден");

            template.Name = model.Name.DefaultOrEmpty().Trim();
            template.SortOrder = model.SortOrder;
            template.DebugMode = model.DebugMode;

            if (System.Web.HttpContext.Current.Request.Files.Count > 0 &&
                System.Web.HttpContext.Current.Request.Files[0].ContentLength > 0)
            {
                var oldFileName = template.FileName;

                var file = System.Web.HttpContext.Current.Request.Files[0];

                if (!TemplatesDocxServices.CheckFileExtension(file.FileName))
                    return JsonError("Недопустимый формат файла");
                if (FileHelpers.FileStorageLimitReached(file.ContentLength))
                    return JsonError("Достигнуто ограничение объема файлов");

                template.FileName = string.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                template.FileSize = file.ContentLength;

                FileHelpers.DeleteFile(TemplatesDocxServices.GetPathAbsolut(oldFileName));
                FileHelpers.SaveFile(TemplatesDocxServices.GetPathAbsolut(template), file.InputStream);
            }

            TemplatesDocxServices.Update(template);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTemplate(int id)
        {
            TemplatesDocxServices.DeleteTemplate<TemplateDocx>(id);
            return JsonOk();
        }

        public JsonResult GetTemplateForm()
        {
            return JsonOk(new
            {
                Types =
                    Enum.GetValues(typeof (TemplateDocxType))
                        .Cast<TemplateDocxType>()
                        .Where(x => x != TemplateDocxType.None)
                        .Select(x =>
                            new SelectItemModel(x.Localize(), (int) x)),

                FileUploadHelpText = FileHelpers.GetFilesHelpText(EFileType.Document)
            });
        }

        public JsonResult GetTemplateTypes()
        {
            return Json(Enum.GetValues(typeof (TemplateDocxType)).Cast<TemplateDocxType>().Where(x => x != TemplateDocxType.None).Select(x =>
                new SelectItemModel(x.Localize(), (int) x)));
        }

        public JsonResult GetDescription(TemplateDocxType? type)
        {
            if (type.HasValue)
            {
                var handler = new GetDescriptionHandler(type.Value);
                var data = handler.Execute();
                if (handler.Errors.Count <= 0)
                    return JsonOk(new
                    {
                        Fields = data
                    });

                return JsonError(handler.Errors.ToArray());
            }

            return JsonError("Не указан тип шаблона");
        }

        public JsonResult GetTemplatesByType(TemplateDocxType? type)
        {
            if (type.HasValue)
            {
                return JsonOk(new
                {
                    Templates = TemplatesDocxServices.GetList<BookingTemplateDocx>().Select(x => (TemplatesDocxModel) x)
                });
            }

            return JsonError("Не указан тип шаблона");
        }

        #endregion

    }
}
