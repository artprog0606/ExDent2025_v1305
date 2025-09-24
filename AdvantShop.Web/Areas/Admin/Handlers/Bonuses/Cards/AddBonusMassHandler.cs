using System;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Diagnostics;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class AddBonusMassHandler : AbstractCommandHandler<bool>
    {
        private readonly CardFilterModelAddBonus _model;
        private readonly string _taskIdempotenceKey;
        private readonly CardFilterModel _filter;

        public AddBonusMassHandler(CardFilterModelAddBonus model)
        {
            _model = model;
            _taskIdempotenceKey = _model.IdempotenceKey;
            _filter = _model.Filter;
        }

        protected override void Validate()
        {
            if (_model.Amount <= 0)
                throw new BlException("Сумма бонуса должна быть > 0");
            
            if (_model.IdempotenceKey.IsNullOrEmpty())
                throw new BlException("Отсутствует идентификатор задачи");
            
            if (_filter == null)
                throw new BlException("Фильтр не найден");
        }

        protected override bool Handle()
        {
            if (CommonStatistic.IsRun)
                return false;

            CommonStatistic.StartNew(() =>
                {
                    try
                    {
                        var isFirstRun = !TransactionService.ExistsByIdempotenceKey(_taskIdempotenceKey);
                        
                        Process((id, c) =>
                        {
                            if (isFirstRun
                                || !TransactionService.ExistsByIdempotenceKey(id, _taskIdempotenceKey)) 
                                AddBonus(id);

                            CommonStatistic.RowPosition++;
                            CommonStatistic.TotalUpdateRow++;
                            return true;
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        CommonStatistic.WriteLog(ex.Message);
                        CommonStatistic.TotalErrorRow++;
                    }
                }, 
                "cards/AddBonusMass",
                "начисление бонусов");

            return true;
        }

        private void Process(Func<Guid, CardFilterModelAddBonus, bool> func)
        {
            if (_filter.SelectMode == SelectModeCommand.None && _filter.Ids != null)
            {
                CommonStatistic.TotalRow = _filter.Ids.Count;
                
                foreach (var id in _filter.Ids)
                    func(id, _model);
            }
            else
            {
                var ids = new GetCardHandler(_filter).GetItemsIds("CardId");
                CommonStatistic.TotalRow = ids.Count;
                
                foreach (var id in ids)
                {
                    if (_filter.Ids == null || !_filter.Ids.Contains(id))
                        func(id, _model);
                }
            }
        }

        private void AddBonus(Guid id)
        {
            var card = CardService.Get(id);
            if (card == null) return;
            if (card.Blocked) return;

            BonusSystemService.AcceptBonuses(card.CardId, _model.Amount, _model.Reason, _model.Name, _model.StartDate, _model.EndDate, null, _model.SendSms, transactionIdempotenceKey: _taskIdempotenceKey);
        }
    }
}