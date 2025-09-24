using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Areas.Api.Models.Products
{
    public sealed class CustomOptionApi
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsRequired { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public List<CustomOptionItemApi> Options { get; }
        public int? SelectedOptionId { get; }

        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public string Description { get; set; }

        public CustomOptionApi(CustomOption option)
        {
            Id = option.CustomOptionsId;
            Title = option.Title;
            IsRequired = option.IsRequired;
            Type = option.InputType.ToString();
            Options = option.Options.Select(x => new CustomOptionItemApi(x)).ToList();
            SelectedOptionId = option.SelectedOptions?.FirstOrDefault()?.OptionId ?? default;
            MinQuantity = option.MinQuantity;
            MaxQuantity = option.MaxQuantity;
            Description = option.Description;
        }
    }

    public sealed class CustomOptionItemApi
    {
        public int Id { get; }
        public string Name { get; }
        public string Title { get; }
        public string PictureUrl { get; set; }

        public int? ProductId { get; set; }
        public int? OfferId { get; set; }
        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public float? DefaultQuantity { get; set; }
        public string Description { get; set; }

        public CustomOptionItemApi(OptionItem item)
        {
            Id = item.OptionId;
            Name = item.Title;
            Title = item.GetOptionText();
            PictureUrl = item.PictureData?.PictureUrl;
            ProductId = item.ProductId;
            OfferId = item.OfferId;
            MinQuantity = item.MinQuantity;
            MaxQuantity = item.MaxQuantity;
            DefaultQuantity = item.DefaultQuantity;
            Description = item.Description;
        }
    }

    public sealed class SelectedCustomOptionApi
    {
        public int Id { get; set; }
        
        [Obsolete("Use OptionItems")]
        public List<int> Options { get; set; }
        
        public List<SelectedCustomOptionItemApi> OptionItems { get; set; }

        /// <summary>
        /// Метод для поддержки старых моб приложений, которые еще используют св-во Options
        /// </summary>
        public void ConvertOptionsToOptionItems()
        {
            if ((OptionItems == null || OptionItems.Count == 0) && 
                (Options != null && Options.Count > 0))
            {
                OptionItems = Options.Select(x => new SelectedCustomOptionItemApi() { OptionId = x }).ToList();
            }
        }
    }
    
    public sealed class SelectedCustomOptionItemApi
    {
        public int OptionId { get; set; }
        public float? Amount { get; set; }
    }

    public sealed class SelectedCartCustomOptionItemApi
    {
        public string Title { get; set; }
        public string Value { get; set; }

        public SelectedCartCustomOptionItemApi(EvaluatedCustomOptions item)
        {
            Title = item.CustomOptionTitle;
            Value = item.OptionTitle +
                    (item.OptionTitle.IsNotEmpty() && item.OptionAmount > 1 ? " " : "") +
                    (item.OptionAmount > 1 ? "x " + item.OptionAmount : "");
        }
    }
}