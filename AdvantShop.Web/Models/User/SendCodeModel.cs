namespace AdvantShop.Models.User
{
    public class SendCodeModel
    {
        public string Phone { get; set; }
        
        /// <summary>
        /// Is registration (else authorization)
        /// </summary>
        public bool SignUp { get; set; } 
    }
}