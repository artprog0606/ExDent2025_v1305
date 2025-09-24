using System;
using AdvantShop.Core.Primitives;

namespace AdvantShop.Core.Services.Auth
{
    public interface IPhoneConfirmationService
    {
        /// <summary>
        /// Выслать код подтверждения на номер телефона
        /// </summary>
        string SendCode(long phone, bool? addHash);
        
        /// <summary>
        /// Подтвердить номер телефона по коду
        /// </summary>
        bool ConfirmPhoneByCode(long phone, string code);

        /// <summary>
        /// Сохранить состояние "телефон подтвержден" для покупателя 
        /// </summary>
        void SetPhoneConfirmedState(long phone, Guid customerId);

        /// <summary>
        /// Подтвержден ли телефон для покупателя? 
        /// </summary>
        bool IsPhoneConfirmed(long phone, Guid customerId);
        
        /// <summary>
        /// Забанен по номеру телефона или ip?
        /// </summary>
        bool IsBannedByPhoneOrIp(long phone, string ip);

        /// <summary>
        /// Забанить по номеру телефона или ip до даты
        /// </summary>
        void Ban(long? phone, string ip, DateTime untilDate);

        /// <summary>
        /// Модуль установлен и активен? 
        /// </summary>
        Result IsModuleActive();

        /// <summary>
        /// Получить описание подсказки для ввода кода
        /// </summary>
        string GetHintDescription();
    }
}