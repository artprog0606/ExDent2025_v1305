using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Cart
{
    public class CartApiModel
    {
        public List<CartItemApiModel> Items { get; set; }
        public bool AddItemsToCurrentCart { get; set; }
    }

    public sealed class CartItemApiModel
    {
        public int Index { get; set; }
        public int OfferId { get; set; }
        public float Amount { get; set; }
        public List<SelectedCartCustomOptionApi> CustomOptions { get; set; }
    }

    public sealed class SelectedCartCustomOptionApi
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public float? OptionAmount { get; set; }
        public string OptionText { get; set; }
    }
}