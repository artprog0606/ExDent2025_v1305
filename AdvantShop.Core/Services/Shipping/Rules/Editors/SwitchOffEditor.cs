namespace AdvantShop.Core.Services.Shipping.Rules
{
    public class SwitchOffEditor: IEditor
    {
        public void Change(IObjectForRule obj) => 
            obj.ChangeEnable(false);
    }
}