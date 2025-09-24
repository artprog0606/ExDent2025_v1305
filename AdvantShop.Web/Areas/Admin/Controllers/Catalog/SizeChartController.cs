using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.SizeChart;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Catalog.SizeChart;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class SizeChartController : BaseAdminController
    {
        public JsonResult GetSizeCharts(SizeChartFilterModel model)
        {
            return Json(new GetSizeChartsHandler(model).Execute());
        }

        #region Commands

        private void Command(SizeChartFilterModel command, Action<int, SizeChartFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetSizeChartsHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSizeCharts(SizeChartFilterModel command)
        {
            Command(command, (id, c) => SizeChartService.Delete(id));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            SizeChartService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(SizeChartGridModel model)
        {
            if (model == null || model.Name.IsNullOrEmpty() || model.LinkText.IsNullOrEmpty())
                return Json(new { result = false });

            var sizeChart = SizeChartService.Get(model.Id);
            if (sizeChart == null)
                return Json(new { result = false });

            sizeChart.Name = model.Name;
            sizeChart.LinkText = model.LinkText;
            sizeChart.SortOrder = model.SortOrder;
            sizeChart.Enabled = model.Enabled;
            SizeChartService.Update(sizeChart);

            return JsonOk();
        }

        #region Add | Update

        [HttpGet]
        public JsonResult Get(int id)
        {
            var sizeChart = SizeChartService.Get(id);
            return JsonOk(new SizeChartModel
            {
                Enabled = sizeChart.Enabled,
                Id = id,
                Name = sizeChart.Name,
                ModalHeader = sizeChart.ModalHeader,
                LinkText = sizeChart.LinkText,
                SourceType = sizeChart.SourceType,
                Text = sizeChart.Text,
                SortOrder = sizeChart.SortOrder,
                ProductIds = SizeChartService.GetObjIdsForSizeChart(ESizeChartEntityType.Product, id),
                CategoryIds = SizeChartService.GetObjIdsForSizeChart(ESizeChartEntityType.Category, id),
                BrandIds = SizeChartService.GetSizeChartBrandIds(id),
                PropertyValues = SizeChartService.GetSizeChartPropertyValues(id).Select(x => new SizeChartPropertyValueModel
                {
                    PropertyName = x.PropertyName,
                    PropertyValueName = x.PropertyValueName,
                    PropertyValueId = x.PropertyValueId
                }).ToList()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(SizeChartModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            model.Id = SizeChartService.Add(new SizeChart
            {
                Enabled = model.Enabled,
                Name = model.Name,
                ModalHeader = model.ModalHeader,
                LinkText = model.LinkText,
                SourceType = model.SourceType,
                Text = model.Text,
                SortOrder = model.SortOrder
            });

            SizeChartService.ReplaceAllMapsInSizeChart(model.Id, model.ProductIds, ESizeChartEntityType.Product);
            SizeChartService.ReplaceAllMapsInSizeChart(model.Id, model.CategoryIds, ESizeChartEntityType.Category);
            SizeChartService.ReplaceSizeChartBrands(model.Id, model.BrandIds);
            SizeChartService.ReplaceSizeChartPropertyValues(model.Id, model.PropertyValues?.Select(x => x.PropertyValueId).ToList());

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(SizeChartModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            SizeChartService.Update(new SizeChart
            {
                Id = model.Id,
                Enabled = model.Enabled,
                Name = model.Name,
                ModalHeader = model.ModalHeader,
                LinkText = model.LinkText,
                SourceType = model.SourceType,
                Text = model.Text,
                SortOrder = model.SortOrder
            });

            SizeChartService.ReplaceAllMapsInSizeChart(model.Id, model.ProductIds, ESizeChartEntityType.Product);
            SizeChartService.ReplaceAllMapsInSizeChart(model.Id, model.CategoryIds, ESizeChartEntityType.Category);
            SizeChartService.ReplaceSizeChartBrands(model.Id, model.BrandIds);
            SizeChartService.ReplaceSizeChartPropertyValues(model.Id, model.PropertyValues?.Select(x => x.PropertyValueId).ToList());

            return JsonOk();
        }

        public JsonResult GetProperties()
        {
            var props = PropertyService.GetAllProperties();

            return Json(props.Select(x => new { x.PropertyId, x.Name }));
        }

        public JsonResult GetPropertyValues(int propertyId)
        {
            var values = PropertyService.GetValuesByPropertyId(propertyId);

            return Json(values.Select(x => new { x.PropertyValueId, x.Value }));
        }

        #endregion

        public JsonResult GetSourceTypes()
        {
            return Json(Enum.GetValues(typeof(ESizeChartSourceType)).Cast<ESizeChartSourceType>().Select(x => new SelectItemModel(x.Localize(), (int)x)));
        }
    }
}
