namespace AdvantShop.Core.Services.Shipping.Rules
{
    public interface IEditor
    {
        /// <summary>
        /// Изменить объект
        /// </summary>
        /// <param name="obj">Изменяемый объект</param>
        void Change(IObjectForRule obj);
    }
}