using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public class GetProductPropertiesApi : AbstractCommandHandler<GetProductPropertiesResponse>
    {
        private readonly int _id;
        
        public GetProductPropertiesApi(int id)
        {
            _id = id;
        }
        
        protected override void Validate()
        {
            if (!ProductService.IsExists(_id))
                throw new BlException("Товар не найден");
        }

        protected override GetProductPropertiesResponse Handle()
        {
            var result = new List<ProductPropertyGroupApi>();
            
            var propertyValues = new List<PropertyValue>();
            var productPropertyValues = 
                PropertyService.GetPropertyValuesByProductId(_id)
                    .Where(x => x.Property.UseInDetails)
                    .ToList();

            foreach (var value in productPropertyValues.Where(propValue => propertyValues.All(x => x.PropertyId != propValue.PropertyId)))
            {
                propertyValues.Add(new PropertyValue()
                {
                    PropertyId = value.PropertyId,
                    Property = value.Property,
                    SortOrder = value.SortOrder,
                    Value = String.Join(", ", productPropertyValues.Where(x => x.PropertyId == value.PropertyId).Select(x => x.Value))
                });
            }

            foreach (var propertyValue in propertyValues)
            {
                var group = result.FirstOrDefault(x => x.GroupId == propertyValue.Property.GroupId);
                if (group != null) 
                    continue;

                var property = propertyValue.Property;
                
                result.Add(new ProductPropertyGroupApi()
                {
                    GroupId = property.GroupId,
                    GroupName =
                        property.GroupId == null || property.Group == null 
                            ? ""
                            : property.Group.NameDisplayed ?? property.Group.Name ?? "",
                    Properties = propertyValues
                        .Where(x => x.Property.GroupId == property.GroupId)
                        .Select(x => new ProductPropertyApi()
                        {
                            Name = x.Property.NameDisplayed ?? x.Property.Name ?? "",
                            Value = x.Value ?? ""
                        })
                        .ToList()
                });
            }

            return new GetProductPropertiesResponse(result);
        }
    }
}