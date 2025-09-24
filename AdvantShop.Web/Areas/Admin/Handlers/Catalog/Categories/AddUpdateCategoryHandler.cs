using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Catalog.Categories;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Categories
{
    public class AddUpdateCategoryHandler
    {
        private AdminCategoryModel _model;

        public AddUpdateCategoryHandler(AdminCategoryModel model)
        {
            _model = model;
        }

        public int Execute()
        {
            var parentCategory = CategoryService.GetCategory(_model.ParentCategoryId);
            
            var category = _model.IsEditMode
                ? CategoryService.GetCategory(_model.CategoryId)
                : new Category();

            category.ExternalId = _model.ExternalId;
            category.Name = _model.Name.Trim();
            category.UrlPath = _model.UrlPath.Trim();
            category.ParentCategoryId = parentCategory?.CategoryId ?? 0;
            category.Description =
                _model.Description == null || _model.Description == "<br />" || _model.Description == "&nbsp;" ||
                _model.Description == "\r\n"
                    ? string.Empty
                    : _model.Description;
            category.BriefDescription =
                _model.BriefDescription == null || _model.BriefDescription == "<br />" ||
                _model.BriefDescription == "&nbsp;" || _model.BriefDescription == "\r\n"
                    ? string.Empty
                    : _model.BriefDescription;
            category.SortOrder = _model.SortOrder;
            category.Enabled = _model.CategoryId != 0 || !_model.IsEditMode ? _model.Enabled : true;
            category.Hidden = _model.CategoryId != 0 || !_model.IsEditMode ? _model.Hidden : false;

            if (!_model.IsEditMode)
            {
                category.ParentsEnabled =
                    _model.ParentCategoryId == 0 ||
                    (parentCategory != null && parentCategory.Enabled && parentCategory.ParentsEnabled);
            }

            category.DisplayChildProducts = false;
            category.DisplayStyle = _model.DisplayStyle;
            category.DisplayBrandsInMenu = _model.DisplayBrandsInMenu;
            category.DisplaySubCategoriesInMenu = _model.DisplaySubCategoriesInMenu;
            category.Sorting = _model.Sorting;
            category.Meta =
                new MetaInfo(0, _model.CategoryId, MetaType.Category, _model.SeoTitle.DefaultOrEmpty(),
                    _model.SeoKeywords.DefaultOrEmpty(), _model.SeoDescription.DefaultOrEmpty(),
                    _model.SeoH1.DefaultOrEmpty());
            category.ModifiedBy = CustomerContext.CurrentCustomer.Id.ToString();
            category.ShowOnMainPage = _model.ShowOnMainPage;

            category.Tags = new List<Tag>();
            
            if (_model.Tags != null && _model.Tags.Count > 0)
            {
                if (_model.IsEditMode)
                {
                    var prevTags = TagService.Gets(category.CategoryId, ETagType.Category, onlyEnabled: false).Select(x => x.Name).ToList();
                    if (_model.Tags.Any(x => !prevTags.Contains(x.Value)))
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_AddTagToCategory);
                }
                
                foreach (var tagModel in _model.Tags)
                {
                    var tag = (tagModel.Id != null && tagModel.Id != 0 ? TagService.Get(tagModel.Id.Value) : null) ??
                              new Tag()
                              {
                                  Name = tagModel.Value,
                                  UrlPath = StringHelper.TransformUrl(StringHelper.Translit(tagModel.Value)),
                                  Enabled = true,
                                  VisibilityForUsers = true
                              };

                    category.Tags.Add(tag);
                }
            }

            if (_model.SizeChartId.HasValue)
            {
                if (_model.SizeChartId.Value != category.SizeChart?.Id)
                {
                    category.SizeChart = SizeChartService.Get(_model.SizeChartId.Value);
                }
            }
            else
            {
                category.SizeChart = null;
            }

            try
            {
                if (_model.IsEditMode)
                {
                    var prevCategory = CategoryService.GetCategory(category.CategoryId);
                    var enabledChanged = prevCategory != null && prevCategory.Enabled != category.Enabled;

                    CategoryService.DeleteCategoryBrands(category.CategoryId);

                    if (_model.Brands != null)
                        foreach (var brandId in _model.Brands)
                            CategoryService.AddCategoryBrand(category.CategoryId, brandId);

                    CategoryService.UpdateCategory(category, true, trackChanges:true);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_EditCategory);

                    if (enabledChanged)
                        CategoryService.RecalculateProductsCountManual();
                }
                else
                {
                    category.CategoryId = CategoryService.AddCategory(category, true, true, trackChanges: true);

                    TrialService.TrackEvent(TrialEvents.AddCategory, "");
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_CategoryCreated);
                }

                if (category.CategoryId == 0)
                    return 0;

                if (!_model.IsEditMode)
                {
                    AddPictureLink(_model.PictureId, category.CategoryId);
                    AddPictureLink(_model.MiniPictureId, category.CategoryId);
                    AddPictureLink(_model.IconId, category.CategoryId);
                }

                return category.CategoryId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("AddUpdate category handler", ex);
            }

            return -1;
        }

        private void AddPictureLink(int pictureId, int categoryId)
        {
            if (pictureId == 0) 
                return;
            
            var photo = PhotoService.GetPhoto(pictureId);
            if (photo != null)
                PhotoService.UpdateObjId(photo.PhotoId, categoryId);
        }
    }
}
