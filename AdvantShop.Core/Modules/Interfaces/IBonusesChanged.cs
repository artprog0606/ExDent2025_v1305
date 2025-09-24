//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.Bonuses.Model;
using System;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IBonusesChanged
    {
        void DoBonusCardAdded(Card card);
        void DoBonusCardUpdated(Card card);
        void DoBonusCardDeleted(Guid cardId);
        void DoCreateTransaction(Transaction transaction);
    }
}