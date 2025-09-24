using AdvantShop.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.FullSearch
{
    public class ProductDocument : BaseDocument
    {
        private string _artNo;
        [SearchField]
        public string ArtNo
        {
            get => _artNo;
            set
            {
                _artNo = value;
                AddParameterToDocumentNoStoreAnalyzed(_artNo, boost: HighBoost);
            }
        }

        private string _artNoNotAnalyzed;
        [SearchField]
        public string ArtNoNotAnalyzed
        {
            get => _artNoNotAnalyzed;
            set
            {
                _artNoNotAnalyzed = value;
                AddParameterToDocumentNoStoreNotAnalyzed(_artNoNotAnalyzed, boost: HighBoost);
            }
        }

        private string _name;
        [SearchField]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                AddParameterToDocumentNoStoreAnalyzed(_name, boost: HighBoost);
            }
        }

        private string _nameNotAnalyzed;
        [SearchField]
        public string NameNotAnalyzed
        {
            get => _nameNotAnalyzed;
            set
            {
                _nameNotAnalyzed = value;
                AddParameterToDocumentNoStoreNotAnalyzed(_nameNotAnalyzed, boost: HighBoost);
            }
        }

        private IEnumerable<string> _offerArtNo;
        [SearchField]
        public IEnumerable<string> OfferArtNo
        {
            get => _offerArtNo;
            set
            {
                _offerArtNo = value;
                foreach (var offerArtNo in _offerArtNo)
                {
                    AddParameterToDocumentNoStoreAnalyzed(offerArtNo, boost: MediumBoost);
                }
            }
        }

        private IEnumerable<string> _offerArtNoNotAnalyzed;
        [SearchField]
        public IEnumerable<string> OfferArtNoNotNotAnalyzed
        {
            get => _offerArtNoNotAnalyzed;
            set
            {
                _offerArtNoNotAnalyzed = value;
                foreach (var offerArtNo in _offerArtNoNotAnalyzed)
                {
                    AddParameterToDocumentNoStoreNotAnalyzed(offerArtNo, boost: MediumBoost);
                }
            }
        }

        private IEnumerable<string> _tags;
        [SearchField]
        public IEnumerable<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                foreach (var tag in _tags)
                {
                    AddParameterToDocumentNoStoreAnalyzed(tag, boost: LowBoost);
                }
            }
        }

        private string _desc;
        [SearchField]
        public string Desc
        {
            get => _desc;
            set
            {
                _desc = value;
                if (!string.IsNullOrWhiteSpace(_desc))
                {
                    _desc = _desc.Replace("<\br>", " ").Replace("<br />", " ");
                    _desc = StringHelper.RemoveHTML(_desc);
                }
                AddParameterToDocumentNoStoreAnalyzed(_desc, boost: LowBoost);
            }
        }

        private string _mainCategoryName;

        [SearchField]
        public string MainCategoryName
        {
            get => _mainCategoryName;
            set
            {
                _mainCategoryName = value;
                AddParameterToDocumentNoStoreAnalyzed(_mainCategoryName, boost: MediumBoost);
            }
        }

        private string _parentCategoryName;

        [SearchField]
        public string ParentCategoryName
        {
            get => _parentCategoryName;
            set
            {
                _parentCategoryName = value;
                AddParameterToDocumentNoStoreAnalyzed(_parentCategoryName, boost: MediumBoost);
            }
        }

        private bool _enabled;
        public bool Enabled
        {

            get => _enabled;
            set
            {
                _enabled = value;
                AddParameterToDocumentNoStoreAnalyzed(_enabled);
            }
        }

        private bool _hidden;
        public bool Hidden
        {

            get => _hidden;
            set
            {
                _hidden = value;
                AddParameterToDocumentNoStoreAnalyzed(_hidden);
            }
        }

        public static explicit operator ProductDocument(Product model)
        {
            var artno = StringHelper.ReplaceCirilikSymbol(model.ArtNo);
            var name = StringHelper.ReplaceCirilikSymbol(model.Name);
            var offers = model.Offers ?? new List<Offer>();

            var pDocument = new ProductDocument()
            {
                Id = model.ProductId,
                ArtNo  = artno,
                ArtNoNotAnalyzed = artno,
                Name = name,
                NameNotAnalyzed = name,
                Desc = StringHelper.ReplaceCirilikSymbol(model.Description),
                OfferArtNo = offers.Select(x => StringHelper.ReplaceCirilikSymbol(x.ArtNo)),
                OfferArtNoNotNotAnalyzed = offers.Select(x => StringHelper.ReplaceCirilikSymbol(x.ArtNo)),

                Tags = model.Tags != null && (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                    ? model.Tags.Select(x => StringHelper.ReplaceCirilikSymbol(x.Name))
                    : Enumerable.Empty<string>(),

                Enabled = model.Enabled && model.CategoryEnabled,
                Hidden = model.MainCategory?.Hidden ?? false,
                MainCategoryName = StringHelper.ReplaceCirilikSymbol(model.MainCategory?.Name),
                ParentCategoryName = StringHelper.ReplaceCirilikSymbol(model.MainCategory?.ParentCategory?.Name),
            };

            if (offers.Any(offer => offer.Amount > 0))
            {
                pDocument.Boost(HighBoost);
            }
            else
            {
                if (!model.AllowPreOrder)
                {
                    pDocument.Boost(LowBoost);
                }
            }
            return pDocument;
        }

        public static explicit operator ProductDocument(ProductLuceneDto model)
        {
            var artno = StringHelper.ReplaceCirilikSymbol(model.ArtNo);
            var name = StringHelper.ReplaceCirilikSymbol(model.Name);
            var offers = model.Offers ?? new List<ProductOfferLuceneDto>();

            var pDocument = new ProductDocument()
            {
                Id = model.ProductId,
                ArtNo = artno,
                ArtNoNotAnalyzed = artno,
                Name = name,
                NameNotAnalyzed = name,
                Desc = StringHelper.ReplaceCirilikSymbol(model.Description),
                OfferArtNo = offers.Select(x => StringHelper.ReplaceCirilikSymbol(x.ArtNo)),
                OfferArtNoNotNotAnalyzed = offers.Select(x => StringHelper.ReplaceCirilikSymbol(x.ArtNo)),

                Tags = model.Tags != null && (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                    ? model.Tags.Select(x => StringHelper.ReplaceCirilikSymbol(x.Name))
                    : Enumerable.Empty<string>(),

                Enabled = model.Enabled && model.CategoryEnabled,
                Hidden = model.Hidden,
                MainCategoryName = StringHelper.ReplaceCirilikSymbol(model.MainCategoryName),
                ParentCategoryName = StringHelper.ReplaceCirilikSymbol(model.ParentCategoryName),
            };

            if (offers.Any(offer => offer.Amount > 0))
            {
                pDocument.Boost(HighBoost);
            }
            else
            {
                if (!model.AllowPreOrder)
                {
                    pDocument.Boost(LowBoost);
                }
            }
            return pDocument;
        }
    }
}
