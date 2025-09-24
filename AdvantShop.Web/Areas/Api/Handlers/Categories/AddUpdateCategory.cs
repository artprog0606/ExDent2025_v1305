using System;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Catalog;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Areas.Api.Models.MetaInfos;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.SEO;

namespace AdvantShop.Areas.Api.Handlers.Categories
{
    public class AddUpdateCategory : AbstractCommandHandler<AddUpdateCategoryResponse>
    {
        #region Ctor

        private readonly AddUpdateCategoryModel _model;
        private readonly bool _isEditMode;
        private Category _category;
        private const string ModifiedBy = "api";

        public AddUpdateCategory(AddUpdateCategoryModel model) : this(0, model){}

        public AddUpdateCategory(int id, AddUpdateCategoryModel model)
        {
            _model = model;
            _model.Id = id;
            _isEditMode = id != 0;
        }

        #endregion

        protected override void Load()
        {
            if (_isEditMode)
                _category = CategoryService.GetCategory(_model.Id);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Name))
                throw new BlException("Название обязательно для заполения");

            if (string.IsNullOrWhiteSpace(_model.Url))
                throw new BlException("Урл обязателен для заполения");

            if (_model.ParentCategoryId != 0 && CategoryService.GetCategory(_model.ParentCategoryId) == null)
                throw new BlException($"Родительской категории с id = {_model.ParentCategoryId} не существует");    
            
            // if new category or urlpath != previous urlpath
            if (_model.Id == 0 || (UrlService.GetObjUrlFromDb(ParamType.Category, _model.Id) != _model.Url))
            {
                if (!UrlService.IsValidUrl(_model.Url, ParamType.Category))
                {
                    _model.Url = UrlService.GetAvailableValidUrl(0, ParamType.Category, _model.Url);
                }
            }
        }

        private AddUpdateCategoryResponse AddUpdate()
        {
            var category = _category ?? new Category() {CategoryId = _model.Id};

            if (!string.IsNullOrEmpty(_model.ExternalId))
                category.ExternalId = _model.ExternalId;

            category.Name = _model.Name.Trim();
            category.UrlPath = _model.Url.Trim();
            category.ParentCategoryId = _model.ParentCategoryId;
            category.Description =
                _model.Description == null || _model.Description == "<br />" || _model.Description == "&nbsp;" ||
                _model.Description == "\r\n"
                    ? ""
                    : _model.Description;
            category.BriefDescription =
                _model.BriefDescription == null || _model.BriefDescription == "<br />" ||
                _model.BriefDescription == "&nbsp;" || _model.BriefDescription == "\r\n"
                    ? ""
                    : _model.BriefDescription;
            category.SortOrder = _model.SortOrder;
            category.Enabled = !_isEditMode ? _model.Enabled : true;
            category.Hidden = !_isEditMode ? _model.Hidden : false;
            
            if (!_isEditMode)
            {
                var parentCategory = CategoryService.GetCategory(_model.ParentCategoryId);
                
                category.ParentsEnabled = 
                    _model.ParentCategoryId == 0 ||
                    (parentCategory != null && parentCategory.Enabled && parentCategory.ParentsEnabled);
            }
            
            category.DisplayStyle = _model.ShowMode;
            category.DisplayBrandsInMenu = _model.ShowBrandsInMenu;
            category.DisplaySubCategoriesInMenu = _model.ShowSubCategoriesInMenu;
            category.Sorting = _model.Sorting.TryParseEnum(ESortOrder.NoSorting);
            category.Meta =
                _model.SeoMetaInformation != null
                    ? _model.SeoMetaInformation.GetMetaInfo(_model.Id)
                    : new MetaInfo(0, _model.Id, MetaType.Category, "", "", "", "");
            category.ModifiedBy = _model.ModifiedBy ?? ModifiedBy;
            category.ShowOnMainPage = _model.ShowOnMainPage;

            if (_isEditMode)
            {
                var prevCategory = CategoryService.GetCategory(category.CategoryId);
                var enabledChanged = prevCategory != null && prevCategory.Enabled != category.Enabled;

                CategoryService.UpdateCategory(category, true, true, new ChangedBy(category.ModifiedBy));

                if (enabledChanged)
                    CategoryService.RecalculateProductsCountManual();
            }
            else
            {
                category.CategoryId = CategoryService.AddCategory(category, true, true, true, new ChangedBy(category.ModifiedBy));
            }

            return new AddUpdateCategoryResponse() { Id = category.CategoryId };
        }

        protected override AddUpdateCategoryResponse Handle()
        {
            try
            {
                return AddUpdate();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}