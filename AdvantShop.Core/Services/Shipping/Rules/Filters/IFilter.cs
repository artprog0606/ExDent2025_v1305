namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    public interface IFilter
    {
        /// <summary>
        /// Проверка - удовлетворяет ли объект условию
        /// </summary>
        /// <param name="obj">объект проверки</param>
        /// <returns>true - если объект удовлетворяет условию; в противном случае - false</returns>
        bool Check(IObjectForRule obj);

        /// <summary>
        /// Получить тип логической группировки результата проверки
        /// </summary>
        /// <returns>Тип логической группировки</returns>
        TypesOfLogicalGrouping GetLogicalGrouping();
    }
    
    public enum TypesOfLogicalGrouping
    {
        Or = 0,
        And = 1,
    }
}