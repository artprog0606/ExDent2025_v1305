using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class AddPropertyWithValue
    {
        private readonly int _productId;
        private readonly int? _propertyId;
        private readonly int? _propertyValueId;
        private readonly string _propertyName;
        private readonly string _propertyValue;

        public AddPropertyWithValue(int productId, int? propertyId, int? propertyValueId, string propertyName, string propertyValue)
        {
            _productId = productId;
            _propertyId = propertyId;
            _propertyValueId = propertyValueId;
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public int Execute()
        {
            var propertyId = GetPropertyId();
            if (propertyId == 0)
                return 0;

            var propertyValueId = GetPropertyValueId(propertyId);
            if (propertyValueId == 0)
                return 0;

            if (!PropertyService.IsExistPropertyValueInProduct(_productId, propertyValueId))
                PropertyService.AddProductProperyValue(propertyValueId, _productId);

            return propertyValueId;
        }

        private int GetPropertyId()
        {
            if (_propertyId != null)
            {
                var property = PropertyService.GetPropertyById(_propertyId.Value);

                if (property.Type == (int)PropertyType.Textarea && PropertyService.IsExistPropertyInProduct(_productId, _propertyId.Value))
                    return 0;

                if (property != null)
                    return property.PropertyId;
            }

            if (!string.IsNullOrWhiteSpace(_propertyName))
                return PropertyService.AddProperty(new Property()
                {
                    Name = _propertyName.Trim(),
                    UseInFilter = true,
                    UseInDetails = true,
                    Type = 1
                });

            return 0;
        }

        private int GetPropertyValueId(int propertyId)
        {
            if (_propertyValueId != null)
            {
                var value = PropertyService.GetPropertyValueById(_propertyValueId.Value);
                if (value != null)
                    return value.PropertyValueId;
            }

            if (!string.IsNullOrWhiteSpace(_propertyValue))
            {
                var value = _propertyValue.Trim();

                var propertyValue = PropertyService.GetPropertyValueByName(propertyId, value);
                if (propertyValue == null)
                {
                    propertyValue = new PropertyValue()
                    {
                        PropertyId = propertyId,
                        Value = value
                    };
                    return PropertyService.AddPropertyValue(propertyValue);
                }
                return propertyValue.PropertyValueId;
            }

            return 0;
        }
    }
}
