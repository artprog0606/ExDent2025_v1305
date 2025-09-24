using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Saas;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.FullSearch
{
    public class CategoryDocument : BaseDocument
    {
        private string _name;
        [SearchField]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                AddParameterToDocumentNoStoreAnalyzed(_name);
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

        private IEnumerable<string> _tags;
        [SearchField]
        public IEnumerable<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                
                foreach (var tag in _tags)
                    AddParameterToDocumentNoStoreAnalyzed(tag);
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

        public static explicit operator CategoryDocument(Category model)
        {
            var name = StringHelper.ReplaceCirilikSymbol(model.Name);

            return new CategoryDocument()
            {
                Id = model.CategoryId,
                Name = name,
                NameNotAnalyzed = name,
                Tags =
                    model.Tags != null &&
                    (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveTags))
                        ? model.Tags.Select(x => StringHelper.ReplaceCirilikSymbol(x.Name))
                        : Enumerable.Empty<string>(),
                Enabled = model.Enabled && model.ParentsEnabled,
                Hidden = model.Hidden
            };
        }
    }
}
