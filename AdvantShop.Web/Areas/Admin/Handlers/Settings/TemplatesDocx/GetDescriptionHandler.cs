using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Core.Services.TemplatesDocx.Templates;
using AdvantShop.Web.Admin.Models.TemplatesDocx;

namespace AdvantShop.Web.Admin.Handlers.Settings.TemplatesDocx
{
    public class GetDescriptionHandler
    {
        private readonly TemplateDocxType _type;
        public List<string> Errors { get; set; }

        public GetDescriptionHandler() { }

        public GetDescriptionHandler(TemplateDocxType type)
        {
            _type = type;
            Errors = new List<string>();
        }

        public List<DescriptionItem> Execute()
        {
            var list = new List<DescriptionItem>();
            switch (_type)
            {
                case TemplateDocxType.Booking:
                    list.AddRange(
                        GetDescription(TemplatesDocxServices.TypeToTemplateItems<BookingTemplate>()));
                    break;

                case TemplateDocxType.Order:
                    list.AddRange(
                        GetDescription(TemplatesDocxServices.TypeToTemplateItems<OrderTemplate>()));
                    break;

                default:
                    Errors.Add("Неизвестный тип шаблона");
                    break;
            }

            return list;
        }

        public List<DescriptionItem> GetDescription(IEnumerable<TemplateDocxItem> items)
        {
            if (items != null)
            {
                return items.Select(templateDocxItem => new DescriptionItem
                {
                    Key = templateDocxItem.Key,
                    Type = GetShortTypeName(templateDocxItem.TypeValue),
                    Hidden = templateDocxItem.Hidden,
                    Description = templateDocxItem.Description,
                    Childs =
                        templateDocxItem.ChildItems != null && templateDocxItem.ChildItems.Count > 0
                            ? GetDescription(templateDocxItem.ChildItems[0])// для описания достаточно одного элемента
                            : null
                }).ToList();
            }
            return null;
        }

        private string GetShortTypeName(Type type)
        {
            var name = type.Name;
            if (!type.IsGenericType) return name;
            name = name.Split('`')[0];
            name += "<" + string.Join(", ", type.GetGenericArguments()
                                            .Select(t => GetShortTypeName(t))) + ">";
            return name;
        }
    }
}
