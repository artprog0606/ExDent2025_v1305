namespace AdvantShop.Core.Services.Triggers
{
    public interface ITriggerObjectFilter
    {
        bool Check(ITriggerObject triggerObject);
    }
}
