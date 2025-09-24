using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public sealed class GetProductApi : AbstractCommandHandler<GetProductResponse>
    {
        private readonly int _id;
        private readonly int? _colorId;
        private readonly int? _sizeId;
        private Product _product;
        
        public GetProductApi(int id, int? colorId, int? sizeId)
        {
            _id = id;
            _colorId = colorId;
            _sizeId = sizeId;
        }
        
        protected override void Load()
        {
            _product = ProductService.GetProduct(_id);
        }

        protected override void Validate()
        {
            if (_product == null)
                throw new BlException("Товар не найден");
        }

        protected override GetProductResponse Handle()
        {
            var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
            var warehouseId = warehouseIds?.FirstOrDefault();
            
            if (warehouseId != null)
                _product.Offers.SetAmountByStocksAndWarehouses(warehouseIds);
            
            var model = new GetProductResponse(_product);
            
            var offerSelected = OfferService.GetMainOffer(_product.Offers, _product.AllowPreOrder, _colorId, _sizeId, warehouseId);
            if (offerSelected != null)
            {
                model.OfferSelectedId = offerSelected.OfferId;
                model.SizeColorPicker.SelectedColorId = offerSelected.ColorID;
                model.SizeColorPicker.SelectedSizeId = offerSelected.SizeID;
            }
            
            foreach (var type in AttachedModules.GetModules<IMarker>())
            {
                var module = (IMarker)Activator.CreateInstance(type);
                var markers = module.GetMarkersByProductId(_product.ProductId);

                if (markers != null && markers.Count > 0)
                {
                    if (model.Markers == null)
                        model.Markers = new List<ProductMarker>();
                    
                    model.Markers.AddRange(markers);
                }
            }

            return model;
        }
    }
}