namespace AdvantShop.Core.Services.Shipping.Rules
{
    public interface IRule
    {
        /// <summary>
        /// Применить правило
        /// </summary>
        /// <param name="obj">Объект для изменений</param>
        void Apply(IObjectForRule obj);
    }
}