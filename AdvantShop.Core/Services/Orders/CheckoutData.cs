using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Orders
{
    #region EnActiveTab, EnUserType

    public enum EnActiveTab
    {
        NoTab = 0,
        DefaultTab = 1,
        UserTab = 2,
        ShippingTab = 3,
        PaymentTab = 4,
        SumTab = 5,
        FinalTab = 6
    }

    public enum EnUserType
    {
        /// <summary>
        /// User without registration
        /// </summary>
        NoUser,

        /// <summary>
        /// User without registration
        /// </summary>
        NewUserWithOutRegistration,

        /// <summary>
        /// User registration on checkout
        /// </summary>
        JustRegistredUser,

        /// <summary>
        /// Registered user
        /// </summary>
        RegisteredUser,

        /// <summary>
        /// Registered user without shipping contact
        /// </summary>
        RegisteredUserWithoutAddress
    }

    #endregion

    public class CheckoutUser
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid? BonusCardId { get; set; }
        public bool WantRegist { get; set; }
        public bool WantBonusCard { get; set; }
        public string Password { get; set; }
        public string Confirm { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? BirthDay { get; set; }
        public CustomerType CustomerType { get; set; }
        public bool? IsAgreeForPromotionalNewsletter { get; set; }
        public bool IsAddRecipient { get; set; }
        public string RecipientLastName { get; set; }
        public string RecipientFirstName { get; set; }
        public string RecipientPatronymic { get; set; }
        public string RecipientPhone { get; set; }

        public List<CustomerFieldWithValue> CustomerFields { get; set; }
    }

    public class CheckoutAddress : IEquatable<CheckoutAddress>
    {
        public Guid? ContactId { get; set; }
        
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Region { get; set; }
        //public string Address { get; set; }
        public string Zip { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }

        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return Equals(obj as CheckoutAddress);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (District != null ? District.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Region != null ? Region.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Zip != null ? Zip.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomField1 != null ? CustomField1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomField2 != null ? CustomField2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomField3 != null ? CustomField3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (House != null ? House.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Apartment != null ? Apartment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Structure != null ? Structure.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Entrance != null ? Entrance.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Floor != null ? Floor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContactId != null ? ContactId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(CheckoutAddress other)
        {
            if (other == null)
                return false;

            return
                this.Country == other.Country &&
                this.City == other.City &&
                this.District == other.District &&
                this.Region == other.Region &&
                this.Zip == other.Zip &&
                this.CustomField1 == other.CustomField1 &&
                this.CustomField2 == other.CustomField2 &&
                this.CustomField3 == other.CustomField3 &&
                this.Street == other.Street &&
                this.House == other.House &&
                this.Apartment == other.Apartment &&
                this.Structure == other.Structure &&
                this.Entrance == other.Entrance &&
                this.Floor == other.Floor &&
                this.ContactId == other.ContactId;
        }

        public static bool operator ==(CheckoutAddress address1, CheckoutAddress address2)
        {
            if ((object)address1 == null || (object)address2 == null)
                return Object.Equals(address1, address2);

            return address1.Equals(address2);
        }

        public static bool operator !=(CheckoutAddress address1, CheckoutAddress address2)
        {
            return !(address1 == address2);
        }

        public static CheckoutAddress Create(CustomerContact contact)
        {
            return new CheckoutAddress()
            {
                ContactId = contact.ContactId,
                Country = contact.Country,
                Region = contact.Region,
                City = contact.City,
                District = contact.District,
                Zip = contact.Zip,
                Street = contact.Street,
                House = contact.House,
                Apartment = contact.Apartment,
                Structure = contact.Structure,
                Entrance = contact.Entrance,
                Floor = contact.Floor,
            };
        }
    }

    public class CheckoutBonus
    {
        public bool UseIt => AppliedBonuses > 0;
        public float BonusPlus { get; set; }
        public float AppliedBonuses { get; set; }
    }

    public class CheckoutData
    {
        public bool HideShippig { get; set; }
        public BaseShippingOption SelectShipping { get; set; }
        public BasePaymentOption SelectPayment { get; set; }

        public int ShopCartHash { get; set; }
        public string CustomerComment { get; set; }
        public bool DontCallBack { get; set; }

        public CheckoutAddress Contact { get; set; }
        public CheckoutUser User { get; set; }
        public CheckoutBonus Bonus { get; set; }

        public int? LpId { get; set; }
        public int? LpUpId { get; set; }
        
        public bool IsMobileApp { get; set; }

        public string PreSelectedShippingId { get; set; }
        public string PreSelectedShippingPointId { get; set; }
        public CalculationVariants? TypeCalculationVariants { get; set; }
        
        public int? WarehouseId { get; set; }
        
        public EnTypeOfReceivingMethod? ReceivingMethod { get; set; }

        public int? CountDevices { get; set; }

        public CheckoutData()
        {
            Contact = new CheckoutAddress();
            User = new CheckoutUser();
            Bonus = new CheckoutBonus();
        }

        private bool? _showContacts;
        public bool ShowContacts()
        {
            if (_showContacts != null)
                return _showContacts.Value;

            var customer = CustomerContext.CurrentCustomer;

            _showContacts = customer.Contacts.Count > 0;

            return _showContacts.Value;
        }

        public bool IsValid(out string error)
        {
            error = null;

            if ((SettingsCheckout.IsShowEmail && SettingsCheckout.IsRequiredEmail && User.Email.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowLastName && SettingsCheckout.IsRequiredLastName && User.LastName.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowPatronymic && SettingsCheckout.IsRequiredPatronymic && User.Patronymic.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowPhone && SettingsCheckout.IsRequiredPhone && User.Phone.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowBirthDay && SettingsCheckout.IsRequiredBirthDay && User.BirthDay == null) 
                )
            {
                error = "Укажите все обязательные поля покупателя";
                return false;
            }

            if (!SettingsDesign.AutodetectCity)
            {
                var requiredCountry = SettingsCheckout.IsShowCountry && SettingsCheckout.IsRequiredCountry && Contact.Country.IsNullOrEmpty();
                var requiredRegion = SettingsCheckout.IsShowState && SettingsCheckout.IsRequiredState && Contact.Region.IsNullOrEmpty();
                var requiredCity = SettingsCheckout.IsShowCity && SettingsCheckout.IsRequiredCity && Contact.City.IsNullOrEmpty();
                var requiredDistrict = SettingsCheckout.IsShowDistrict && SettingsCheckout.IsRequiredDistrict && Contact.District.IsNullOrEmpty();

                if (requiredCountry || requiredRegion || requiredCity || requiredDistrict)
                {
                    error = "Укажите все обязательные поля: " +
                            new List<string>()
                            {
                                requiredCountry ? "страну" : null,
                                requiredRegion ? "регион" : null,
                                requiredCity ? "город" : null,
                                requiredDistrict ? "район" : null
                            }.Where(x => x != null).AggregateString(", ");
                    return false;
                }
            }
            
            if (SettingsCheckout.IsShowZip && SettingsCheckout.ZipDisplayPlace && SettingsCheckout.IsRequiredZip && Contact.Zip.IsNullOrEmpty())
            {
                error = "Укажите индекс";
                return false;
            }
            
            if (SettingsCheckout.IsShowOrderRecipient && User.IsAddRecipient && (User.RecipientFirstName.IsNullOrEmpty() ||
                (SettingsCheckout.IsShowLastName && SettingsCheckout.IsRequiredLastName && User.RecipientLastName.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowPatronymic && SettingsCheckout.IsRequiredPatronymic && User.RecipientPatronymic.IsNullOrEmpty()) ||
                (SettingsCheckout.IsShowPhone && SettingsCheckout.IsRequiredPhone && User.RecipientPhone.IsNullOrEmpty())
                ))
            {
                error = "Укажите все обязательные поля получателя";
                return false;
            }

            if (SettingsCheckout.IsShowPhone && SettingsOAuth.AuthByCodeActive && !CustomerContext.CurrentCustomer.RegistredUser)
            {
                var standardPhone = StringHelper.ConvertToStandardPhone(User.Phone);
                
                if (!new PhoneConfirmationService().IsPhoneConfirmed(standardPhone ?? 0, CustomerContext.CustomerId))
                {
                    error = LocalizationService.GetResource("User.Registration.ErrorPhoneNotConfirmed");
                    return false;
                }
            }
            return true;
        } 
    }

}
