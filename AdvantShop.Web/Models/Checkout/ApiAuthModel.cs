namespace AdvantShop.Models.Checkout
{
    public class ApiAuthModel
    {
        public string Type { get; set; }
        
        /// <summary>
        /// Billing order code
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Billing order hash
        /// </summary>
        public string Hash { get; set; }
    }
}