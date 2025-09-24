//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public interface IShippingOption
    {
        OptionValidationResult Validate();
        string ForMailTemplate();
    }
    
    public interface ICalculateShipping
    {
        IEnumerable<BaseShippingOption> CalculateOptions(CalculationVariants calculationVariants);
        IEnumerable<BaseShippingOption> CalculateOptionsToPoint(string pointId);
        IEnumerable<BaseShippingPoint> CalculateShippingPoints(float[,] coordinatesOfBounds, TypeBonds typeBonds);
    }

    public interface IReceiveShippingPoints
    {
        IEnumerable<BaseShippingPoint> GetShippingPoints();
        BaseShippingPoint GetShippingPointInfo(string pointId);
    }

    [Flags]
    public enum CalculationVariants
    {
        [StringName("courier")]
        Courier = 0b1 << 0,
        
        [StringName("self-delivery")]
        PickPoint = 0b1 << 1,
        
        [StringName("all")]
        All = Courier | PickPoint
    }

    public enum TypeBonds
    {
        TopToBottom,
        BottomToTop
    }

    public class ShippingPointsResult
    {
        public int MethodId { get; set; }
        public IEnumerable<BaseShippingPoint> Points { get; set; }
    }

    public class ShippingPointResult
    {
        public int MethodId { get; set; }
        public BaseShippingPoint Point { get; set; }
    }

    /// <summary>
    /// Реализация синхронизации статусов
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingSyncOfOrderStatus
    {
        void SyncStatusOfOrder(Order order);
        void SyncStatusOfOrders(IEnumerable<Order> orders);

        bool SyncByAllOrders { get; }

        bool StatusesSync { get; }
    }

    /// <summary>
    /// Реализация предоставления информации доставкой о передвижении заказа
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingTheHistoryOfMovement
    {
        bool ActiveHistoryOfMovement { get; }
        List<HistoryOfMovement> GetHistoryOfMovement(Order order);
    }

    public class HistoryOfMovement
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public interface IShippingLazyData
    {
        object GetLazyData(Dictionary<string, object> data);
    }

    /// <summary>
    /// Реализуется при необходимости выполнения фоновых задач
    /// <para>Задача запускается раз в час. Необходимость менее редкого выполнения контролируется самим методом доставки</para>
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    // internal - чтобы модули не имели возможности реализации (для них имеется IModuleTask)
    internal interface IShippingWithBackgroundMaintenance
    {
        void ExecuteJob();
    }

    /// <summary>
    /// Наследование интерфейса делает доступным метод для выбора в насройках платежки COD
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingPaymentCashOnDelivery { }


    /// <summary>
    /// Наследование интерфейса делает доступным метод для выбора в насройках платежки PickPoint
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingSupportingPaymentPickPoint { }

    /// <summary>
    /// Наследование интерфейса приводит к отключению функционала наценки
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseExtracharge { }

    /// <summary>
    /// Наследование интерфейса уведомляет об отсутсвии зависимости метода достаки от конкретной валюты
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseCurrency { }

    /// <summary>
    /// Наследование интерфейса уведомляет об отсутсвии поддержки увеличения времени доставки
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseExtraDeliveryTime { }

    /// <summary>
    /// Наследование интерфейса приводит к отключению функционала настроек налога
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUseTax { }

    /// <summary>
    /// Наследование интерфейса приводит к отключению функционала настроек Предмета и Способа расчета
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingNoUsePaymentMethodAndSubjectTypes { }

    /// <summary>
    /// Проверяет принадлежит ли пункт выдачи данному методу доставки
    /// </summary>
    public interface IComparePickPoint
    {
        bool ComparePickPoint(OrderPickPoint pickPoint);
    }

    /// <summary>
    /// Выбрать пункт выдачи
    /// </summary>
    public interface ISelectShippingPoint
    {
        void SelectShippingPoint(string pointId);
    }

    /// <summary>
    /// Наследование интерфейса уведомляет о наличии поддержки интервалов доставки
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingUseDeliveryInterval { }

    /// <summary>
    /// Наследование интерфейса уведомляет о необходимости уточнить тип доставки (Самовывоз/Курьер)
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingRequiresSpecifyingTypeOfDelivery { }

    /// <summary>
    /// Поддержка общих доп настроек
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingUseParamsForApi 
    {
        ShippingParamsForApi GetParamsForApi();
    }

    /// <summary>
    /// Поддержка выгрузки заказа в систему доставки
    /// </summary>
    public interface IUnloadOrder
    {
        /// <summary>
        /// Передача заказа в систему доставки
        /// </summary>
        /// <param name="order">Заказ, который нужно передать</param>
        UnloadOrderResult UnloadOrder(Order order);
    }

    /// <summary>
    /// Наследование интерфейса уведомляет о том, что доставка выводится только для цифровых товаров
    /// <para>Для объектов реализующих BaseShipping</para>
    /// </summary>
    public interface IShippingMethodsOnlyForDigitalProducts { }

    public class UnloadOrderResult
    {
        private UnloadOrderResult()
        {
        }

        public static UnloadOrderResult CreateSuccessResult(string message = null, object @object = null)
        {
            return new UnloadOrderResult
            {
                Success = true,
                Message = message,
                Object = @object
            };
        }

        public static UnloadOrderResult CreateFailedResult(string message, string errorCode = null, object @object = null)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            
            return new UnloadOrderResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Object = @object
            };
        }

        /// <summary>
        /// Флаг передачи заказа
        /// <value>true - заказ успешно передан в доставку, <para>false - заказа не удалось передать в доставку</para></value>
        /// </summary>
        public bool Success { get; private set; }
        
        /// <summary>
        /// Сопровождающее сообщение
        /// <para>Как для удачной, так и не удачной передачи заказа</para>
        /// </summary>
        public string Message { get; private set; }
        
        /// <summary>
        /// Сопровождающий объект
        /// </summary>
        public object Object { get; private set; }
        
        /// <summary>
        /// Код идентифицирующий тип ошибки, при передаче заказа
        /// <remarks>Для внутреннего использования доставками</remarks>
        /// </summary>
        public string ErrorCode { get; private set; }
    }

    public enum EnTypeOfDelivery
    {
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.Courier")]
        Courier = 0b1 << 0,
        [Localize("AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery")]
        SelfDelivery = 0b1 << 1,
    }

    public class ShippingParamsForApi
    {
        public string SelfDeliveryAddress { get; set; }
    }
}