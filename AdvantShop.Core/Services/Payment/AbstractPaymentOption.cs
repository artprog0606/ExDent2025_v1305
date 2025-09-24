using System;

namespace AdvantShop.Payment
{
    public abstract class AbstractPaymentOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public float Rate { get; set; }
        public string IconName { get; set; }

        public string NamePostfix { get; set; }

        public Type ModelType => this.GetType();

        public virtual string Template => "";
    }
}