using System;
using AdvantShop.ExportImport;

namespace AdvantShop.Core.Common.Attributes
{
    public class CsvV2FieldAttribute : Attribute
    {
        public CsvFieldStatus FieldStatus { get; set; }
        public bool IsOfferField { get; set; }
        public bool IsPostProcessField { get; set; }

        public CsvV2FieldAttribute() { }

        public CsvV2FieldAttribute(CsvFieldStatus fieldStatus = CsvFieldStatus.None)
        {
            FieldStatus = fieldStatus;
        }
    }
}