using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments;
using AdvantShop.Web.Admin.Models.Customers.CustomerSegments;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Controllers.Customers
{
    [Auth(RoleAction.Customers)]
    [SaasFeature(ESaasProperty.HaveCustomerSegmets)]
    public partial class CustomerSegmentsCrmController : CustomerSegmentsController { }
    
    [Auth(RoleAction.Customers)]
    [SaasFeature(ESaasProperty.HaveCustomerSegmets)]
    public partial class CustomerSegmentsController : BaseAdminController
    {
        #region Segments List

        public ActionResult Index(string recreate)
        {
            if (recreate == "true")
                new CustomerSegmentsJob().Execute(null);

            SetMetaInformation(T("Admin.CustomerSegments.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerSegmentsCtrl);

            return View("~/Areas/Admin/Views/CustomerSegments/Index.cshtml");
        }


        public ActionResult GetList(CustomerSegmentsFilterModel model)
        {
            return Json(new GetCustomerSegments(model).Execute());
        }

        #region Commands

        private void Command(CustomerSegmentsFilterModel command, Action<int, CustomerSegmentsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetCustomerSegments(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSegments(CustomerSegmentsFilterModel command)
        {
            Command(command, (id, c) => CustomerSegmentService.Delete(id));
            return JsonOk();
        }


        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSegment(int id)
        {
            CustomerSegmentService.Delete(id);
            return JsonOk();
        }

        #endregion

        #region Add | Edit segment

        public ActionResult Add()
        {
            var model = new CustomerSegmentModel() {SegmentFilter = new CustomerSegmentFilter()};
            if (SettingsCustomers.IsRegistrationAsLegalEntity)
            {
                model.ShowCustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity;
                if (model.ShowCustomerType)
                    model.CustomerTypes = Enum.GetValues(typeof(CustomerType))
                        .Cast<CustomerType>()
                        .Select(x => new SelectListItem()
                        {
                            Text = x.Localize(),
                            Value = x.ToString()
                        }).ToList();
                else
                    model.SegmentFilter.CustomerType = CustomerType.LegalEntity;
            }
            else
                model.SegmentFilter.CustomerType = CustomerType.PhysicalEntity;
            SetMetaInformation(T("Admin.CustomerSegments.Add.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerSegmentCtrl);

            return View("~/Areas/Admin/Views/CustomerSegments/AddEdit.cshtml", model);
        }

        public ActionResult Edit(int id)
        {
            var segment = CustomerSegmentService.Get(id);
            if (segment == null)
                return Error404();

            var model = new CustomerSegmentModel()
            {
                IsEditMode = true,

                Id = segment.Id,
                Name = segment.Name,
                SegmentFilter = segment.SegmentFilter ?? new CustomerSegmentFilter()
            };

            if (model.SegmentFilter.Categories != null)
            {
                foreach (var categoryId in model.SegmentFilter.Categories)
                {
                    var c = CategoryService.GetCategory(categoryId);
                    if (c != null)
                        model.Categories.Add(c.Name);
                }
            }

            model.Countries = model.SegmentFilter.Countries;
            model.Cities = model.SegmentFilter.Cities;

            model.ShowCustomerType = SettingsCustomers.IsRegistrationAsPhysicalEntity && SettingsCustomers.IsRegistrationAsLegalEntity;

            model.CustomerTypes = Enum.GetValues(typeof(CustomerType))
                .Cast<CustomerType>()
                .Select(x => new SelectListItem()
                {
                    Text = x.Localize(),
                    Value = x.ToString()
                }).ToList();

            SetMetaInformation(T("Admin.CustomerSegments.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerSegmentCtrl);

            return View("~/Areas/Admin/Views/CustomerSegments/AddEdit.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddEdit(CustomerSegmentModel model)
        {
            if (ModelState.IsValid)
            {
                var id = new SaveCustomerSegment(model).Execute();
                if (id != 0)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = id });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Customers.Edit.Title"));
            SetNgController(NgControllers.NgControllersTypes.CustomerSegmentCtrl);

            if (model.IsEditMode)
                return RedirectToAction("Edit", new { id = model.Id });

            return RedirectToAction("Add");
        }

        public JsonResult GetCategories()
        {
            var categories = CategoryService.GetCategories().Select(x => x.Name).Distinct();
            return Json(new { categories });
        }

        public JsonResult GetCities()
        {
            var cities = CityService.GetAll().Select(x => x.Name).Distinct();
            return Json(new { cities });
        }

        public JsonResult GetCountries()
        {
            var countries = CountryService.GetAllCountryIdAndName().Select(x => x.Name).Distinct();
            return Json(new { countries });
        }

        #endregion

        public ActionResult GetCustomersBySegment(CustomersBySegmentFilterModel model)
        {
            var result = new GetCustomersBySegment(model).Execute();

            if (model.OutputDataType == FilterOutputDataType.Csv)
            {
                var fileName = "export_grid_customersbysegment_" + model.Id + ".csv";
                var fullFilePath = new ExportCustomersToCsv(result, fileName).Execute();
                return File(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(result);
        }

        public ActionResult GetCustomerIdsBySegment(CustomersBySegmentFilterModel model)
        {
            return Json(new GetCustomersBySegment(model, true).Execute());
        }

        public JsonResult GetCustomerSegmentsSelectOptions()
        {
            var segments = CustomerSegmentService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
            return Json(segments);
        }
    }
}
