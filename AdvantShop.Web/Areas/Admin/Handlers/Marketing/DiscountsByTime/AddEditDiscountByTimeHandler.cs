using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Models.Marketing.DiscountsByTime;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Certificates
{
    public class AddEditDiscountByTimeHandler : AbstractCommandHandler
    {
        private readonly bool _isEdit;
        private readonly DiscountsByTimeModel _model;
        private DiscountByTime _discountByTime;

        public AddEditDiscountByTimeHandler(DiscountsByTimeModel model)
        {
            _isEdit = (model?.Id ?? 0) != 0;
            _model = model;
        }

        protected override void Load()
        {
            if (_isEdit)
                _discountByTime = DiscountByTimeService.Get(_model.Id);
        }
        
        protected override void Validate()
        {
            if (_model == null || _isEdit && _discountByTime == null)
                throw new BlException(T("Admin.DiscountsByTime.DiscountNotFound"));
        }

        protected override void Handle()
        {
            var dateTimeNow = DateTime.Now;
            var dateFrom = _model.DateFrom.TryParseDateTime();
            var dateTo = _model.DateTo.TryParseDateTime();
            var currentDiscounts = DiscountByTimeService.GetCurrentDiscountsByTime();
            var currentActiveByTimeCategories = new List<int>();
            foreach (var currentDiscount in currentDiscounts)
            {
                if (_isEdit && currentDiscount.Id == _discountByTime.Id)
                    continue;
                currentActiveByTimeCategories.AddRange(currentDiscount.ActiveByTimeCategories.Select(x => x.CategoryId));
            }
            currentActiveByTimeCategories = currentActiveByTimeCategories.Distinct().ToList();
            if (_isEdit && _discountByTime.Enabled)
                if (dateTimeNow.TimeOfDay > _discountByTime.TimeFrom && dateTimeNow.TimeOfDay < _discountByTime.TimeTo)// выбранные ранее категории уже активны
                    // меняем активность у категорий, которые должны быть активны на время скидки, но не трогаем категории, которые также есть и в другой активной скидке 
                    DiscountByTimeService.ChangeCategoryEnabled(
                        DiscountByTimeService.GetDiscountCategories(_discountByTime.Id, activeByTime: true)
                            .Where(x => !currentActiveByTimeCategories.Contains(x.CategoryId))
                            .Select(x => x.CategoryId)
                            .ToList(), 
                        false);
            
            if (_isEdit)
            {
                _discountByTime.Enabled = _model.Enabled;
                _discountByTime.Discount = _model.Percent;
                _discountByTime.TimeFrom = _model.DateFrom.TryParseTimeSpan();
                _discountByTime.TimeTo = _model.DateTo.TryParseTimeSpan();
                _discountByTime.PopupText = _model.PopupText;
                _discountByTime.ShowPopup = _model.ShowPopup;
                _discountByTime.SortOrder = _model.SortOrder;

                DiscountByTimeService.Update(_discountByTime);
            }
            else
            {
                _discountByTime = new DiscountByTime
                {
                    Enabled = _model.Enabled,
                    Discount = _model.Percent,
                    TimeFrom = _model.DateFrom.TryParseTimeSpan(),
                    TimeTo = _model.DateTo.TryParseTimeSpan(),
                    PopupText = _model.PopupText,
                    ShowPopup = _model.ShowPopup,
                    SortOrder = _model.SortOrder
                };
                DiscountByTimeService.Add(_discountByTime);
            }

            DiscountByTimeService.DeleteDiscountByTimeDaysOfWeek(_discountByTime.Id);
            foreach (var dayOfWeek in _model.SelectedDays) 
                DiscountByTimeService.AddDiscountByTimeDayOfWeek(_discountByTime.Id, dayOfWeek);

            DiscountByTimeService.DeleteDiscountCategories(_discountByTime.Id);

            if (_model.DiscountCategories != null)
                foreach (var categoryId in _model.DiscountCategories)
                    DiscountByTimeService.AddOrUpdateDiscountCategory(_discountByTime.Id, categoryId, applyDiscount: true);
            if (_model.ActiveByTimeCategories != null)
                foreach (var categoryId in _model.ActiveByTimeCategories)
                    DiscountByTimeService.AddOrUpdateDiscountCategory(_discountByTime.Id, categoryId, activeByTime: true);

            if (_model.Enabled)
            {
                if (_model.SelectedDays.Contains(dateTimeNow.DayOfWeek)
                    && dateTimeNow.TimeOfDay > dateFrom.TimeOfDay
                    && dateTimeNow.TimeOfDay < dateTo.TimeOfDay)
                    DiscountByTimeService.ChangeCategoryEnabled(_model.ActiveByTimeCategories, true);
                else if (_model.SelectedDays.Contains(dateTimeNow.DayOfWeek)
                        || dateTimeNow.TimeOfDay > dateTo.TimeOfDay
                        || dateTimeNow.TimeOfDay < dateTo.TimeOfDay) // не деактивировать категории, которые активны по другой скидке
                    DiscountByTimeService.ChangeCategoryEnabled(_model.ActiveByTimeCategories?.Where(x => !currentActiveByTimeCategories.Contains(x)).ToList(), false);
                
                DiscountByTimeService.AddUpdateDiscountByTimeTasks(_discountByTime);
            }
            else
            {
                DiscountByTimeService.RemoveDiscountByTimeTasks(_discountByTime);
            }
        }
    }
}